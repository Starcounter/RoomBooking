using Starcounter;
using System;
using System.Globalization;

namespace RoomBooking.ViewModels.Screens
{
    partial class NewBookingPage : Json
    {

        public Action OnClose = null;
        public Room Room;

        public void Init(Room room)
        {
            this.Room = room;
        }

        /// <summary>
        /// Create booking event
        /// </summary>
        /// <param name="action"></param>
        public void Handle(Input.CreateBookingTrigger action)
        {

            // TODO: Validate

            Db.Transact(() =>
            {
                RoomBookingEvent newBooking = new RoomBookingEvent();

                // this.BeginDay    2017-10-17
                // this.StartTime   00:00

                string startDatestring = string.Format("{0}T{1}", this.BeginDay, this.StartTime);  // 2017-10-17T00:00
                DateTime beginDate = DateTime.ParseExact(startDatestring, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None);

                DateTime beginUtcDate = TimeZoneInfo.ConvertTimeToUtc(beginDate, this.Room.TimeZoneInfo);
                DateTime endUtcDate = beginUtcDate.AddHours(this.DurationHours).AddMinutes(this.DurationMinutes);

                newBooking.BeginUtcDate = beginUtcDate;
                newBooking.EndUtcDate = endUtcDate;
                newBooking.Title = this.Title;
                newBooking.Description = "Description - todo";  // TODO;
                newBooking.Room = this.Room;
            });

            this.OnClose?.Invoke();
        }

        /// <summary>
        /// Close page
        /// </summary>
        /// <param name="action"></param>
        public void Handle(Input.CloseTrigger action)
        {
            this.OnClose?.Invoke();
        }


    }
}
