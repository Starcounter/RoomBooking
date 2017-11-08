using Starcounter;
using System;
using System.Globalization;
using System.Linq;
using Starcounter.Templates;

namespace RoomBooking.ViewModels.Screens
{
    partial class NewBookingPage : Json, IBound<RoomBookingEvent>
    {
        public Action OnClose = null;

        protected override void HasChanged(TValue property)
        {
            base.HasChanged(property);
            if (property.PropertyName != "ValidationMessage" &&
                property.PropertyName != "CreateBookingTrigger" &&
                property.PropertyName != "CloseTrigger")
            {

                if (IsBookedOrOverlappingOtherEvents())
                {
                    this.ValidationMessage = "You can not double book this room";
                }
                else
                {
                    this.ValidationMessage = "";
                }
            }
        }

        protected override void OnData()
        {
            base.OnData();
            if (this.Data != null)
            {
                // Default values
                this._DurationHours = (long)(this.Data.EndUtcDate - this.Data.BeginUtcDate).TotalHours;
                this._DurationMinutes = (long)(this.Data.EndUtcDate - this.Data.BeginUtcDate).Minutes;
            }

        }

        public string BeginDay {


            get {
                return TimeZoneInfo.ConvertTimeFromUtc(this.Data.BeginUtcDate, this.Data.Room.TimeZoneInfo).ToString("yyyy-MM-dd");
            }
            set {
                this.UpdateDateTime(value, this.StartTime);
            }


        }

        public string StartTime {

            get {
                return TimeZoneInfo.ConvertTimeFromUtc(this.Data.BeginUtcDate, this.Data.Room.TimeZoneInfo).ToString("HH:mm");
            }
            set {
                this.UpdateDateTime(this.BeginDay, value);
            }
        }

        public long _DurationHours;
        public long DurationHours {
            get {
                return _DurationHours;
            }
            set {
                if (value < 0) return;
                _DurationHours = value;
                this.UpdateEndUtcDate();
            }
        }

        public long _DurationMinutes;

        public long DurationMinutes {

            get {
                return _DurationMinutes;
            }
            set {
                if (value < 0) return;
                _DurationMinutes = value;
                this.UpdateEndUtcDate();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="date">Room local date (yyyy-MM-dd)</param>
        /// <param name="time">Room local time (HH:mm)</param>
        public void UpdateDateTime(string date, string time)
        {
            // TODO: Validate
            string beginRoomLocalDateString = string.Format("{0}T{1}", date, time);  // 2017-10-17T00:00
            DateTime beginRoomLocalDate = DateTime.ParseExact(beginRoomLocalDateString, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None);
            this.Data.BeginUtcDate = TimeZoneInfo.ConvertTimeToUtc(beginRoomLocalDate, this.Data.Room.TimeZoneInfo);

            // End date

            UpdateEndUtcDate();
            //            this.Data.EndUtcDate = this.Data.BeginUtcDate.AddHours((double)this.DurationHours).AddMinutes(this.DurationMinutes);
        }

        private void UpdateEndUtcDate()
        {
            this.Data.EndUtcDate = this.Data.BeginUtcDate.AddHours((double)this.DurationHours).AddMinutes(this.DurationMinutes);
        }


        /// <summary>
        /// Create booking event
        /// </summary>
        /// <param name="action"></param>
        public void Handle(Input.CreateBookingTrigger action)
        {

            if (IsBookedOrOverlappingOtherEvents())
            {
                this.ValidationMessage = "You can not double book this room";
                return;
            }

            if (string.IsNullOrEmpty(this.Title))
            {
                this.ValidationMessage = "Please enter an title for this event";
                return;
            }

            // TODO: Validate
            this.Transaction.Commit();
            this.OnClose?.Invoke();
        }

        private bool IsBookedOrOverlappingOtherEvents()
        {
            //    bool overlap = a.start < b.end && b.start < a.end;

            RoomBookingEvent roomBookingEvent = Db.SQL<RoomBookingEvent>("SELECT o FROM RoomBooking.RoomBookingEvent o WHERE o <> ? AND o.Room = ? AND o.BeginUtcDate < ? AND ? < o.EndUtcDate", this.Data, this.Data.Room, this.Data.EndUtcDate, this.Data.BeginUtcDate).FirstOrDefault();
            return roomBookingEvent != null;

        }

        /// <summary>
        /// Close page
        /// </summary>
        /// <param name="action"></param>
        public void Handle(Input.CloseTrigger action)
        {
            this.Transaction.Rollback();
            this.OnClose?.Invoke();
        }
    }
}
