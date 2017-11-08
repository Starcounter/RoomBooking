using System;
using Starcounter;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using RoomBooking.ViewModels.Screens;

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

 
        public static RoomBookingEvent GetNextEvent(Room room)
        {
            return Db.SQL<RoomBookingEvent>("SELECT o FROM RoomBooking.RoomBookingEvent o WHERE o.Room = ? AND ? < o.BeginUtcDate ORDER BY o.BeginUtcDate", room, DateTime.UtcNow).FirstOrDefault();
        }

        public static RoomBookingEvent GetNextEvent()
        {
            return Db.SQL<RoomBookingEvent>("SELECT o FROM RoomBooking.RoomBookingEvent o WHERE ? < o.BeginUtcDate ORDER BY o.BeginUtcDate",  DateTime.UtcNow).FirstOrDefault();
        }
        

        public static void RegisterTimer()
        {
            EventTimer = new Timer(TimerCallback);
            SetEventTimer();
        }

        private static Timer EventTimer;

        private static void SetEventTimer()
        {

            DateTime utcNow = DateTime.UtcNow;

            //EventTimer.Change(new TimeSpan(0,0,0,10,0), TimeSpan.FromTicks(-1));

            RoomBookingEvent firstEvent = Db.SQL<RoomBookingEvent>("SELECT o FROM RoomBooking.RoomBookingEvent o WHERE o.BeginUtcDate >= ? ORDER BY o.BeginUtcDate", utcNow).FirstOrDefault();
            if (firstEvent != null)
            {
                TimeSpan timeSpan = firstEvent.BeginUtcDate - utcNow;
                EventTimer.Change(timeSpan, TimeSpan.FromTicks(-1));
            }
        }

        public static void TimerCallback(Object state)
        {
            Scheduling.ScheduleTask(() =>
            {
                // Set timer to next event
                SetEventTimer();

                Session.ForAll((session, sessionId) =>
                {

                    ScreenContentPage screenContentPage = session.Store[nameof(ScreenContentPage)] as ScreenContentPage;
                    if(screenContentPage != null)
                    {
                        screenContentPage.OnActiveEvent();
                    }

                    session.CalculatePatchAndPushOnWebSocket();
                });

            }, false);
        }
    }
}
