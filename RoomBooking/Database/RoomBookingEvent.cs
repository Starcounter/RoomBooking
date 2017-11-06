using System;
using Starcounter;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace RoomBooking
{
    [Database]
    public class RoomBookingEvent
    {
        public DateTime BeginUtcDate;
        public DateTime EndUtcDate;
        public string Title;
        public string Description;

        public Room Room;

        public static void RegisterHooks()
        {

            // Push updates to client sessions
            Hook<RoomBookingEvent>.CommitUpdate += (sender, room) =>
            {
                SetEventTimer();
                Program.PushChanges();
            };

            Hook<RoomBookingEvent>.CommitInsert += (sender, room) =>
            {
                SetEventTimer();
                Program.PushChanges();
            };

            Hook<RoomBookingEvent>.CommitDelete += (sender, room) =>
            {
                SetEventTimer();
                Program.PushChanges();
            };
        }

        public static void RegisterTimer()
        {
            EventTimer = new Timer(timerCallback);
            SetEventTimer();
        }

        private static Timer EventTimer;

        private static void SetEventTimer()
        {

            DateTime utcNow = DateTime.UtcNow;

            RoomBookingEvent firstEvent = Db.SQL<RoomBookingEvent>("SELECT o FROM RoomBooking.RoomBookingEvent o WHERE o.BeginUtcDate >= ? ORDER BY o.BeginUtcDate", utcNow).FirstOrDefault();
            if (firstEvent != null)
            {
                TimeSpan timeSpan = firstEvent.BeginUtcDate - utcNow;
                EventTimer.Change(timeSpan, TimeSpan.FromTicks(-1));
            }
        }

        public static void timerCallback(Object state)
        {
            Scheduling.ScheduleTask(() => {
                // Set to next event
                SetEventTimer();
                Program.PushChanges();
            }, true);

        }



    }
}
