using Starcounter;
using System;
using System.Linq;

namespace RoomBooking.ViewModels
{
    partial class FreePage : Json
    {

        public Action OnNewBooking = null;

        public void Init(Room room)
        {
            this.Room.Data = room;
        }

        public DateTime NextEventBeginUtcDate => GetNextEventBeginUtcDate();
        public DateTime ServerUTCDate => GetServerUTCDate();

        private DateTime GetNextEventBeginUtcDate()
        {
            Room room = this.Room.Data as Room;

            RoomBookingEvent roomBookingEvent = Db.SQL<RoomBookingEvent>($"SELECT o FROM {nameof(RoomBooking)}.\"{nameof(RoomBookingEvent)}\" o WHERE o.{nameof(RoomBookingEvent.Room)} = ? AND o.{nameof(RoomBookingEvent.BeginUtcDate)} >= ? ORDER BY o.{nameof(RoomBookingEvent.BeginUtcDate)}", room, DateTime.UtcNow).FirstOrDefault();
            if (roomBookingEvent != null)
            {
                return roomBookingEvent.BeginUtcDate;
                //return TimeZoneInfo.ConvertTimeFromUtc(roomBookingEvent.BeginUtcDate, room.TimeZoneInfo);
            }

            return DateTime.Now;
        }

        private DateTime GetServerUTCDate()
        {
            return DateTime.UtcNow;
        }
        
        public void Handle(Input.SyncTimeTrigger action)
        {
            //Room room = this.Room.Data as Room;

            // UTC time  :2008-09-22T14:01:54.9571247Z
            // local time: 1970-01-01T00:00:00-0500  
            //this.ServerUTCDate = DateTime.UtcNow.ToString("o");
        }


        public void Handle(Input.ClaimTrigger action)
        {
            this.OnNewBooking?.Invoke();
        }

    }
}
