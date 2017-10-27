using Starcounter;
using System;

namespace RoomBooking.ViewModels.Screens
{
    partial class FreePage : Json
    {

        public Action OnNewBooking = null;

        public void Init(Room room)
        {
            this.Room.Data = room;
        }

        public string TimeUntilNextEventStr {
            get {

                Room room = this.Room.Data as Room;

                RoomBookingEvent roomBookingEvent = Db.SQL<RoomBookingEvent>("SELECT o FROM RoomBooking.RoomBookingEvent o WHERE o.Room = ? AND o.BeginUtcDate >= ? ORDER BY o.BeginUtcDate", room, DateTime.UtcNow).First;
                if(roomBookingEvent == null)
                {
                    return "days"; // TODO: Maximum booking time (rest of the day)
                }

                //                DateTime roomTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, room.TimeZoneInfo);

                TimeSpan nextEvent = roomBookingEvent.BeginUtcDate - DateTime.UtcNow;

                if( nextEvent.TotalDays > 1)
                {
                    return string.Format("{0} days", nextEvent.TotalDays);

                }

                if( nextEvent.Hours == 0)
                {
                    return string.Format("{0}m", nextEvent.Minutes);
                }

                return string.Format("{0}h {1}m", nextEvent.Hours, nextEvent.Minutes);
            }
        }


        public void Handle(Input.ClaimTrigger action)
        {
            this.OnNewBooking?.Invoke();
        }

    }
}
