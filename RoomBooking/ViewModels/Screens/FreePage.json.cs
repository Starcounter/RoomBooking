using Starcounter;
using System;
using System.Linq;

namespace RoomBooking.ViewModels.Screens
{
    partial class FreePage : Json
    {

        public Action OnNewBooking = null;

        public void Init(Room room)
        {
            this.Room.Data = room;
        }


        public DateTime NextEventDate => GetNextEventDate();


        private DateTime GetNextEventDate()
        {
            Room room = this.Room.Data as Room;

            RoomBookingEvent roomBookingEvent = Db.SQL<RoomBookingEvent>($"SELECT o FROM {nameof(RoomBooking)}.\"{nameof(RoomBookingEvent)}\" o WHERE o.{nameof(RoomBookingEvent.Room)} = ? AND o.{nameof(RoomBookingEvent.BeginUtcDate)} >= ? ORDER BY o.{nameof(RoomBookingEvent.BeginUtcDate)}", room, DateTime.UtcNow).FirstOrDefault();
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
