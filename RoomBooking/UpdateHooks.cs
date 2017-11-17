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
                SetEventTimer();
                Utils.PushChanges();
            };

            Hook<RoomBookingEvent>.CommitInsert += (sender, room) =>
            {
                SetEventTimer();
                Utils.PushChanges();
            };

            Hook<RoomBookingEvent>.CommitDelete += (sender, room) =>
            {
                SetEventTimer();
                Utils.PushChanges();
            };
        }

        private static void RegisterHook_RoomScreenRelation()
        {
            // Push updates to client sessions
            Hook<RoomScreenRelation>.CommitUpdate += (sender, room) => Utils.PushChanges();
            Hook<RoomScreenRelation>.CommitInsert += (sender, room) => Utils.PushChanges();
            Hook<RoomScreenRelation>.CommitDelete += (sender, room) => Utils.PushChanges();
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
            EventTimer = new Timer(TimerCallback);
            SetEventTimer();
        }

        private static void TimerCallback(Object state)
        {
            Scheduling.ScheduleTask(() =>
            {
                // Set timer to next event
                SetEventTimer();
                Utils.PushChanges();
            }, false);
        }


        private static void SetEventTimer()
        {
            DateTime utcNow = DateTime.UtcNow;
            RoomBookingEvent firstEvent = Db.SQL<RoomBookingEvent>($"SELECT o FROM {typeof(RoomBookingEvent)} o WHERE o.{nameof(RoomBookingEvent.BeginUtcDate)} >= ? ORDER BY o.{nameof(RoomBookingEvent.BeginUtcDate)}", utcNow).FirstOrDefault();

            if (firstEvent != null)
            {
                TimeSpan timeSpan = firstEvent.BeginUtcDate - utcNow;
                EventTimer.Change(timeSpan, TimeSpan.FromTicks(-1));
            }
        }

        #endregion
    }
}
