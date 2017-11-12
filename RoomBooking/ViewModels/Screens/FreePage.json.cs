using CalendarSync.Database;
using Starcounter;
using System;
using System.Linq;

namespace RoomBooking.ViewModels.Screens
{
    partial class FreePage : Json
    {

        public Action OnNewBooking = null;

        public void Init(SyncedCalendar room)
        {
            this.Room.Data = room;
        }


        public DateTime NextEventDate => GetNextEventDate();


        private DateTime GetNextEventDate()
        {
            SyncedCalendar room = this.Room.Data as SyncedCalendar;

            SyncedEvent roomBookingEvent = Db.SQL<SyncedEvent>($"SELECT o FROM CalendarSync.Database.\"{nameof(SyncedEvent)}\" o WHERE o.{nameof(SyncedEvent.Calendar)} = ? AND o.{nameof(SyncedEvent.BeginUtcDate)} >= ? ORDER BY o.{nameof(SyncedEvent.BeginUtcDate)}", room, DateTime.UtcNow).FirstOrDefault();
            if (roomBookingEvent != null)
            {
                return TimeZoneInfo.ConvertTimeFromUtc(roomBookingEvent.BeginUtcDate, room.TimeZoneInfo); ;
            }

            return DateTime.MaxValue;
        }


        


        public void Handle(Input.ClaimTrigger action)
        {
            this.OnNewBooking?.Invoke();
        }

    }
}
