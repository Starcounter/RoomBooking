using System;
using Starcounter;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Screens.Common;
using CalendarSync.Database;
using RoomBooking;

namespace CalendarSync.Database
{
    /// <summary>
    /// A room can be "public" so everyone signed in should be able to book it (For a fee). or private (Default)
    /// </summary>
    [Database]
    public class SyncedCalendar2
    {
        public string Name;
        public string Description;
        public string TimeZoneId;
        public bool DaylightSavings;

        public TimeZoneInfo TimeZoneInfo {
            get {

                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(this.TimeZoneId);
                }
                catch (Exception)
                {
                    return TimeZoneInfo.Local;
                }
            }
        }


        public double TimeOffset {

            get {
                return TimeZoneInfo.GetUtcOffset(DateTime.UtcNow).TotalMinutes;
                 
            }

        }

//        public static void RegisterHooks()
//        {

//            // Cleanup
//            Hook<SyncedCalendar>.BeforeDelete += (sender, room) =>
//            {
//                Db.SQL($"DELETE FROM CalendarSync.Database.\"{nameof(SyncedEvent)} WHERE {nameof(SyncedEvent.Calendar)} = ?", room);
//                Db.SQL($"DELETE FROM {nameof(RoomBooking)}.\"{nameof(UserRoomRelation)} WHERE {nameof(UserRoomRelation.Room)} = ?", room);
//            };

//            // Cleanup
//            Hook<User>.BeforeDelete += (sender, user) =>
//            {
////                Db.SQL("DELETE FROM RoomBooking.Room o WHERE o.User = ?", user);
//            };

//            // Push updates to client sessions
//            Hook<SyncedCalendar>.CommitUpdate += (sender, room) =>
//            {
//                Program.PushChanges();
//            };

//            Hook<SyncedCalendar>.CommitInsert += (sender, room) =>
//            {
//                Program.PushChanges();
//            };

//            Hook<SyncedCalendar>.CommitDelete += (sender, room) =>
//            {
//                Program.PushChanges();
//            };
//        }

    }
}

// https://stackoverflow.com/questions/2961848/how-to-use-timezoneinfo-to-get-local-time-during-daylight-savings-time

