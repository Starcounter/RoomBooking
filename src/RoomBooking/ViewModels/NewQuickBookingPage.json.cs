using Starcounter;
using System;
using System.Globalization;
using System.Linq;
using Starcounter.Templates;

namespace RoomBooking.ViewModels
{
    partial class NewQuickBookingPage : Json, IBound<RoomBookingEvent>
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
                CreateToTimes();
            }

        }

        private void CreateToTimes()
        {
            int step = 15;


            DateTime localStart = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, this.Data.Room.TimeZoneInfo);
            // Reset Ms,sec,min
            int min = localStart.Minute;

            DateTime resetedLocalStart = localStart.AddMilliseconds(-localStart.Millisecond);
            resetedLocalStart = resetedLocalStart.AddSeconds(-resetedLocalStart.Second);
            resetedLocalStart = resetedLocalStart.AddMinutes(-resetedLocalStart.Minute);

            // Get next step
            int minutes = (int)(Math.Ceiling((double)min / step) * step);
            int startMinutes = resetedLocalStart.Hour * 60 + minutes;
            int maxMinutes = (24 * 60);

            startMinutes = startMinutes + step;

            //    startMinutes = startMinutes + step;

            for (int i = startMinutes; i <= maxMinutes; i = i + step)
            {
                var item = this.ToTimeItems.Add();
                TimeSpan span = TimeSpan.FromMinutes(i);
                item.Name = span.ToString(@"hh\:mm");


                item.EndUtcTime = this.Data.BeginUtcDate.AddMinutes(-(this.Data.BeginUtcDate.Hour * 60 + this.Data.BeginUtcDate.Minute) + (i- this.Data.Room.TimeZoneInfo.BaseUtcOffset.TotalMinutes));

                int duration = i - ((localStart.Hour * 60) + localStart.Minute);

                if (duration > 120)
                {
                    TimeSpan durationSpan = TimeSpan.FromMinutes(duration);
                    item.Duration = durationSpan.ToString(@"h\h\ m\m\i\n");
                }
                else
                {
                    item.Duration = duration.ToString() + "min";
                    item.Minutes = duration;
                }
            }


            foreach (var timeItem in this.ToTimeItems)
            {
                
                if( timeItem.Minutes >= 60)
                {
                    this.SelectEndTime(timeItem);
                    break;
                }

            }

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

            if (string.IsNullOrEmpty(this.Name))
            {
                this.ValidationMessage = "Please enter an name for this event";
                return;
            }

            // TODO: Validate
            this.Transaction.Commit();
            this.OnClose?.Invoke();
        }

        private bool IsBookedOrOverlappingOtherEvents()
        {
            RoomBookingEvent roomBookingEvent = Db.SQL<RoomBookingEvent>($"SELECT o FROM {nameof(RoomBooking)}.\"{nameof(RoomBookingEvent)}\" o WHERE o <> ? AND o.{nameof(RoomBookingEvent.Room)} = ? AND o.{nameof(RoomBookingEvent.BeginUtcDate)} < ? AND ? < o.{nameof(RoomBookingEvent.EndUtcDate)}", this.Data, this.Data.Room, this.Data.EndUtcDate, this.Data.BeginUtcDate).FirstOrDefault();
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

        public void SelectEndTime(ToTimeItem item)
        {

            foreach( var timeItem in this.ToTimeItems)
            {
                timeItem.Selected = timeItem == item;
            }

            this.Data.EndUtcDate = item.EndUtcTime;
        }

    }

    [NewQuickBookingPage_json.ToTimeItems]
    partial class ToTimeItem : Json
    {

        public DateTime EndUtcTime;

        public void Handle(Input.SelectTrigger action)
        {

            NewQuickBookingPage page = this.Parent.Parent as NewQuickBookingPage;

            page.SelectEndTime(this);

//            RoomBookingEvent roomBookingEvent = page.Data as RoomBookingEvent;
//            roomBookingEvent.EndUtcDate = this.EndUtcTime;


        }

    }
}
