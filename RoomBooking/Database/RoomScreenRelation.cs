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
        }
    }
}
