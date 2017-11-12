using System;
using Starcounter;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using RoomBooking.ViewModels.Screens;
using RoomBooking;

namespace CalendarSync.Database
{
    [Database]
    public class SyncedEvent2
    {
        //public DateTime BeginUtcDate;
        //public DateTime EndUtcDate;
        //public string Title;
        //public string Description;

        public SyncedCalendar Room;


        //public static void RegisterHooks()
        //{

        //    // Push updates to client sessions
        //    Hook<SyncedEvent>.CommitUpdate += (sender, room) =>
        //    {
        //        SetEventTimer();
        //        Program.PushChanges();
        //    };

        //    Hook<SyncedEvent>.CommitInsert += (sender, room) =>
        //    {
        //        SetEventTimer();
        //        Program.PushChanges();
        //    };

        //    Hook<SyncedEvent>.CommitDelete += (sender, room) =>
        //    {
        //        SetEventTimer();
        //        Program.PushChanges();
        //    };
        //}

 
        //public static SyncedEvent GetNextEvent(SyncedCalendar room)
        //{
        //    return Db.SQL<SyncedEvent>($"SELECT o FROM CalendarSync.Database.\"{nameof(SyncedEvent)}\" o WHERE o.{nameof(SyncedEvent.Calendar)} = ? AND ? < o.{nameof(SyncedEvent.BeginUtcDate)} ORDER BY o.{nameof(SyncedEvent.BeginUtcDate)}", room, DateTime.UtcNow).FirstOrDefault();
        //}

        //public static SyncedEvent GetNextEvent()
        //{
        //    return Db.SQL<SyncedEvent>($"SELECT o FROM CalendarSync.Database.\"{nameof(SyncedEvent)}\" o WHERE ? < o.{nameof(SyncedEvent.BeginUtcDate)} ORDER BY o.{nameof(SyncedEvent.BeginUtcDate)}", DateTime.UtcNow).FirstOrDefault();
        //}


        //public static void RegisterTimer()
        //{
        //    EventTimer = new Timer(TimerCallback);
        //    SetEventTimer();
        //}

        //private static Timer EventTimer;

        //private static void SetEventTimer()
        //{

        //    DateTime utcNow = DateTime.UtcNow;

        //    //EventTimer.Change(new TimeSpan(0,0,0,10,0), TimeSpan.FromTicks(-1));

        //    SyncedEvent firstEvent = Db.SQL<SyncedEvent>($"SELECT o FROM CalendarSync.Database.\"{nameof(SyncedEvent)}\" o WHERE o.{nameof(SyncedEvent.BeginUtcDate)} >= ? ORDER BY o.{nameof(SyncedEvent.BeginUtcDate)}", utcNow).FirstOrDefault();
        //    if (firstEvent != null)
        //    {
        //        TimeSpan timeSpan = firstEvent.BeginUtcDate - utcNow;
        //        EventTimer.Change(timeSpan, TimeSpan.FromTicks(-1));
        //    }
        //}

        //public static void TimerCallback(Object state)
        //{
        //    Scheduling.ScheduleTask(() =>
        //    {
        //        // Set timer to next event
        //        SetEventTimer();

        //        Session.ForAll((session, sessionId) =>
        //        {

        //            ScreenContentPage screenContentPage = session.Store[nameof(ScreenContentPage)] as ScreenContentPage;
        //            if(screenContentPage != null)
        //            {
        //                screenContentPage.OnActiveEvent();
        //            }

        //            session.CalculatePatchAndPushOnWebSocket();
        //        });

        //    }, false);
        //}
    }
}
