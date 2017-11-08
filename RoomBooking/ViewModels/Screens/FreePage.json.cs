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

            RoomBookingEvent roomBookingEvent = Db.SQL<RoomBookingEvent>("SELECT o FROM RoomBooking.RoomBookingEvent o WHERE o.Room = ? AND o.BeginUtcDate >= ? ORDER BY o.BeginUtcDate", room, DateTime.UtcNow).FirstOrDefault();
            if(roomBookingEvent != null)
            {
                return TimeZoneInfo.ConvertTimeFromUtc(roomBookingEvent.BeginUtcDate, room.TimeZoneInfo); ;
            }

            return DateTime.MaxValue;
        }


        //public string TimeUntilNextEventStr {
        //    get {

        //        Room room = this.Room.Data as Room;

        //        RoomBookingEvent roomBookingEvent = Db.SQL<RoomBookingEvent>("SELECT o FROM RoomBooking.RoomBookingEvent o WHERE o.Room = ? AND o.BeginUtcDate >= ? ORDER BY o.BeginUtcDate", room, DateTime.UtcNow).FirstOrDefault();
        //        if (roomBookingEvent == null)
        //        {
        //            return "+days"; // TODO: Maximum booking time (rest of the day)
        //        }

        //        TimeSpan nextEvent = roomBookingEvent.BeginUtcDate - DateTime.UtcNow;

        //        if (nextEvent.TotalDays > 1)
        //        {
        //            return string.Format("{0} days", nextEvent.TotalDays);

        //        }

        //        if (nextEvent.Hours == 0)
        //        {
        //            return string.Format("{0}min", nextEvent.Minutes);
        //        }

        //        return string.Format("{0}h {1}min", nextEvent.Hours, nextEvent.Minutes);
        //    }
        //}


        public void Handle(Input.ClaimTrigger action)
        {
            this.OnNewBooking?.Invoke();
        }

    }
}
