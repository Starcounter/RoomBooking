using System;
using Starcounter;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Screens.Common;
using CalendarSync.Database;
namespace RoomBooking
{
    [Database]
    public class UserRoomRelation
    {
        public User User;
        public SyncedCalendar Room;
        public static void RegisterHooks()
        {

            // Push updates to client sessions
            Hook<UserRoomRelation>.CommitUpdate += (sender, room) =>
            {
                Program.PushChanges();
            };

            Hook<UserRoomRelation>.CommitInsert += (sender, room) =>
            {
                Program.PushChanges();
            };

            Hook<UserRoomRelation>.CommitDelete += (sender, room) =>
            {
                Program.PushChanges();
            };
        }
    }
}
