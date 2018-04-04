using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;

namespace RoomBooking
{
    [Database]
    public class Room
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TimeZoneId { get; set; }
        public int WarnNotificationMinutes { get; set; }

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
    }

}
