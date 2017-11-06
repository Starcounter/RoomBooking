using System;
using Starcounter;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Screens.Common;

namespace RoomBooking
{
    /// <summary>
    /// A room can be "public" so everyone signed in should be able to book it (For a fee). or private (Default)
    /// </summary>
    [Database]
    public class Room
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

        public static void RegisterHooks()
        {

            // Cleanup
            Hook<Room>.BeforeDelete += (sender, room) =>
            {
                Db.SQL("DELETE FROM RoomBooking.RoomBookingEvent WHERE Room = ?", room);
                Db.SQL("DELETE FROM RoomBooking.UserRoomRelation WHERE Room = ?", room);
            };

            // Cleanup
            Hook<User>.BeforeDelete += (sender, user) =>
            {
                Db.SQL("DELETE FROM RoomBooking.Room o WHERE o.User = ?", user);
            };

            // Push updates to client sessions
            Hook<Room>.CommitUpdate += (sender, room) =>
            {
                Program.PushChanges();
            };

            Hook<Room>.CommitInsert += (sender, room) =>
            {
                Program.PushChanges();
            };

            Hook<Room>.CommitDelete += (sender, room) =>
            {
                Program.PushChanges();
            };
        }

    }
}

// https://stackoverflow.com/questions/2961848/how-to-use-timezoneinfo-to-get-local-time-during-daylight-savings-time

