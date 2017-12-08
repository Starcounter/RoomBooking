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

        public int WarnNotificationMinutes { get; set; }
        
        public DateTime WarnUtcDate {
            get {
                return this.BeginUtcDate.Subtract(new TimeSpan(0, (int)this.WarnNotificationMinutes, 0));
            }
        }

        public static void RegisterHooks()
        {
            Hook<Room>.BeforeDelete += (sender, room) =>
            {
                Db.SQL($"DELETE FROM {typeof(RoomBookingEvent)} WHERE {nameof(RoomBookingEvent.Room)} = ?", room);
            };

        }
    }
}
