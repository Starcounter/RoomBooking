using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using RoomBooking.ViewModels.Partials;
using System.Threading;
using RoomBooking.ViewModels.Screens;

namespace RoomBooking
{
    public class UpdateGuiHooks
    {
        private static Timer EventTimer;

        public static void Register()
        {
            RegisterHook_Room();
            RegisterHook_RoomBookingEvent();
            RegisterHook_RoomScreenRelation();
            RegisterHook_UserRoomRelation();
        }

        private static void RegisterHook_Room()
        {
            // Push updates to client sessions
            Hook<Room>.CommitUpdate += (sender, room) => Utils.PushChanges();
            Hook<Room>.CommitInsert += (sender, room) => Utils.PushChanges();
            Hook<Room>.CommitDelete += (sender, room) => Utils.PushChanges();
        }

        private static void RegisterHook_RoomBookingEvent()
        {
            RegisterTimer();

            // Push updates to client sessions
            Hook<RoomBookingEvent>.CommitUpdate += (sender, room) =>
            {
                SetNextEventTimer();
                Utils.PushChanges();
            };

            Hook<RoomBookingEvent>.CommitInsert += (sender, room) =>
            {
                SetNextEventTimer();
                Utils.PushChanges();
            };

            Hook<RoomBookingEvent>.CommitDelete += (sender, room) =>
            {
                SetNextEventTimer();
                Utils.PushChanges();
            };
        }

        private static void RegisterHook_RoomScreenRelation()
        {
            // Push updates to client sessions
            Hook<RoomObjectRelation>.CommitUpdate += (sender, room) => Utils.PushChanges();
            Hook<RoomObjectRelation>.CommitInsert += (sender, room) => Utils.PushChanges();
            Hook<RoomObjectRelation>.CommitDelete += (sender, room) => Utils.PushChanges();
        }

        private static void RegisterHook_UserRoomRelation()
        {
            // Push updates to client sessions
            Hook<UserRoomRelation>.CommitUpdate += (sender, room) => Utils.PushChanges();
            Hook<UserRoomRelation>.CommitInsert += (sender, room) => Utils.PushChanges();
            Hook<UserRoomRelation>.CommitDelete += (sender, room) => Utils.PushChanges();
        }

        #region Timer

        private static void RegisterTimer()
        {
            //EventTimer = new Timer(TimerCallback);
            SetNextEventTimer();
        }

        private static void TimerCallback(Object state)
        {
            Scheduling.RunTask(() =>
            {
                SetNextEventTimer();
                Utils.PushChanges();
            });
        }

        /// <summary>
        /// Set timer to next event
        /// </summary>
        private static void SetNextEventTimer()
        {
            SetNextEventTimer2();

            //DateTime utcNow = DateTime.UtcNow;
            //RoomBookingEvent firstEvent = Db.SQL<RoomBookingEvent>($"SELECT o FROM {typeof(RoomBookingEvent)} o WHERE o.{nameof(RoomBookingEvent.BeginUtcDate)} >= ? ORDER BY o.{nameof(RoomBookingEvent.BeginUtcDate)}", utcNow).FirstOrDefault();

            //if (firstEvent != null)
            //{
            //    TimeSpan timeSpan = firstEvent.BeginUtcDate - utcNow;
            //    EventTimer.Change(timeSpan, TimeSpan.FromTicks(-1));
            //}
        }

        private static void SetNextEventTimer2()
        {
            DateTime utcNow = DateTime.UtcNow;

            // 1. Get next Start
            RoomBookingEvent nextStartEvent = Db.SQL<RoomBookingEvent>($"SELECT o FROM {typeof(RoomBookingEvent)} o WHERE o.{nameof(RoomBookingEvent.BeginUtcDate)} >= ? ORDER BY o.{nameof(RoomBookingEvent.BeginUtcDate)}", utcNow).FirstOrDefault();


            // 2. Get next Stop
            RoomBookingEvent nextStopEvent = Db.SQL<RoomBookingEvent>($"SELECT o FROM {typeof(RoomBookingEvent)} o WHERE o.{nameof(RoomBookingEvent.EndUtcDate)} >= ? ORDER BY o.{nameof(RoomBookingEvent.EndUtcDate)}", utcNow).FirstOrDefault();


            // 3. Get next warning date
            RoomBookingEvent nextWarnEvent = Db.SQL<RoomBookingEvent>($"SELECT o FROM {typeof(RoomBookingEvent)} o WHERE o.{nameof(RoomBookingEvent.WarnUtcDate)} >= ? ORDER BY o.{nameof(RoomBookingEvent.WarnUtcDate)}", utcNow).FirstOrDefault();

            #region tried to optimize next warn date (we cant index on WarnUtcDate)
            //            long? maxAlertTime = Db.SQL<long?>($"SELECT MAX(o.{nameof(RoomBookingEvent.WarnNotificationMinutes)}) FROM {typeof(RoomBookingEvent)} o").FirstOrDefault();
            //            if (maxAlertTime == null)
            //            {
            //                maxAlertTime = 0;
            //            }

            //            DateTime warnEventTime = utcNow.Subtract(new TimeSpan(0, (int)maxAlertTime, 0));

            //            RoomBookingEvent nextWarnEvent = null;
            //            IEnumerable<RoomBookingEvent> result = Db.SQL<RoomBookingEvent>($"SELECT o FROM {typeof(RoomBookingEvent)} o WHERE o.{nameof(RoomBookingEvent.BeginUtcDate)} >= ? ORDER BY o.{nameof(RoomBookingEvent.BeginUtcDate)}", warnEventTime);
            //            foreach (RoomBookingEvent roomBookingEvent in result)
            //            {
            //                if ( DateTime.Compare(roomBookingEvent.WarnUtcDate, warnEventTime) < 0)
            //                {
            //                    nextWarnEvent = roomBookingEvent;
            //                }


            //                if (utcNow >= roomBookingEvent.WarnUtcDate)
            //                {
            ////                    nextWarnEvent = roomBookingEvent;
            //                    break;
            //                }

            //                //TimeSpan ts = new TimeSpan(0, roomBookingEvent.WarnNotificationMinutes, 0);
            //                //DateTime warnEventTime2 = roomBookingEvent.BeginUtcDate.Subtract(ts);
            //                //if (utcNow >= warnEventTime2)
            //                //{
            //                //    nextWarnEvent = roomBookingEvent;
            //                //    break;
            //                //}
            //            }
            #endregion
            // Figure of the next event between Warn, Start and Stop

            DateTime nextAlertTime = DateTime.MaxValue;

            if (nextStartEvent != null && DateTime.Compare(nextStartEvent.BeginUtcDate, nextAlertTime) < 0)
            {
                nextAlertTime = nextStartEvent.BeginUtcDate;
            }

            if (nextStopEvent != null && DateTime.Compare(nextStopEvent.EndUtcDate, nextAlertTime) < 0)
            {
                nextAlertTime = nextStopEvent.EndUtcDate;
            }

            if (nextWarnEvent != null && DateTime.Compare(nextWarnEvent.WarnUtcDate, nextAlertTime) < 0)
            {
                nextAlertTime = nextStopEvent.WarnUtcDate;
            }


            if (nextAlertTime != DateTime.MaxValue && nextAlertTime >= utcNow)
            {
                if (EventTimer == null)
                {
                    EventTimer = new Timer(TimerCallback);
                }
                TimeSpan timeSpan = nextAlertTime - utcNow;
                EventTimer.Change(timeSpan, TimeSpan.FromTicks(-1));
            }
            else
            {
                // Stop timer
                if (EventTimer != null)
                {
                    EventTimer.Dispose();
                    EventTimer = null;
                }
            }
        }

        #endregion
    }
}
