using System;
using Starcounter;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        }
    }
}
