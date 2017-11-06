using Starcounter;
using System;
using System.Globalization;

namespace RoomBooking.ViewModels.Screens
{
    partial class NewBookingPage : Json, IBound<RoomBookingEvent>
    {
        public Action OnClose = null;

        public string BeginDay {
            get => TimeZoneInfo.ConvertTimeFromUtc(this.Data.BeginUtcDate, this.Data.Room.TimeZoneInfo).ToString("yyyy-MM-dd");
            set => this.UpdateDateTime(value, this.StartTime);
        }

        public string StartTime {
            get => TimeZoneInfo.ConvertTimeFromUtc(this.Data.BeginUtcDate, this.Data.Room.TimeZoneInfo).ToString("HH:mm");
            set => this.UpdateDateTime(this.BeginDay, value);
        }

        public long DurationHours => (long)(this.Data.EndUtcDate - this.Data.BeginUtcDate).TotalHours;
        public long DurationMinutes => (long)(this.Data.EndUtcDate - this.Data.BeginUtcDate).Minutes;

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
            this.Data.EndUtcDate = this.Data.BeginUtcDate.AddHours((double)this.DurationHours).AddMinutes(this.DurationMinutes);
        }

        //public void Init(Room room)
        //{
        //    this.Room = room;
        //}

        /// <summary>
        /// Create booking event
        /// </summary>
        /// <param name="action"></param>
        public void Handle(Input.CreateBookingTrigger action)
        {

            // TODO: Validate
            this.Transaction.Commit();
            this.OnClose?.Invoke();
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
