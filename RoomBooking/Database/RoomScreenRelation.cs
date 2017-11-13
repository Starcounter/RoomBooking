using System;
using Starcounter;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Screens.Common;

namespace RoomBooking
{
    [Database]
    public class RoomScreenRelation
    {
        public Screen Screen;
        public Room Room;
        public bool Enabled;
        public static void RegisterHooks()
        {

            // Push updates to client sessions
            Hook<RoomScreenRelation>.CommitUpdate += (sender, room) =>
            {
                Program.PushChanges();
            };

            Hook<RoomScreenRelation>.CommitInsert += (sender, room) =>
            {
                Program.PushChanges();
            };

            Hook<RoomScreenRelation>.CommitDelete += (sender, room) =>
            {
                Program.PushChanges();
            };

        }
    }
}
