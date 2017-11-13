using System;
using Starcounter;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using RoomBooking.ViewModels.Screens;
using RoomBooking;

namespace RoomBooking
{
    [Database]
    public class RoomBookingEvent
    {
        public Room Room;

        public DateTime BeginUtcDate { get; set; }
        public DateTime EndUtcDate { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
    }

}
