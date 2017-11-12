using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using Simplified.Ring3;

namespace CalendarSync.Database
{

    [Database]
    public class SyncedCalendar
    {
        public string Provider { get; set; }
        public string ProviderId { get; set; }
        public string CalendarId { get; set; }
        public string Name { get; set; }
        public DateTime? LastSync { get; set; }
        public SystemUser Owner { get; set; }

        public IEnumerable<SyncedEvent> Events => Db.SQL<SyncedEvent>("SELECT se FROM CalendarSync.Database.SyncedEvent se WHERE se.Calendar=?", this);

        /* INTERNAL - DO NOT SET/UPDATE */
        public string SyncToken { get; internal set; }
        public string WatchId { get; internal set; }
        public string WatchRemoteId { get; internal set; }
        public DateTime WatchExpires { get; internal set; }
        /* INTERNEL - END */


        /* MAPPING BEGIN */

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

        /* MAPPING END */

    }


    [Database]
    public class SyncedEvent
    {
        public SyncedCalendar Calendar;

        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool FullDay { get; set; }

        public string Name { get; set; }
        public string Descriotion { get; set; }
        public string Location { get; set; }

        public string Organizer { get; set; }
        public string Participants { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public SystemUser Owner { get; set; }

        /* INTERNAL - DO NOT SET/UPDATE */
        public bool RemoteOrigin { get; internal set; }
        public string RemoteId { get; internal set; }
        public string iCalUID { get; internal set; }
        /* INTERNEL - END */

        /* MAPPING BEGIN */
        public DateTime BeginUtcDate {
            get {
                return BeginTime;
            }
            set {
                BeginTime = value;
            }
        }

        public DateTime EndUtcDate {
            get {
                return EndTime;
            }
            set {
                EndTime = value;
            }
        }
  

        public string Description {
            get {
                return Descriotion;
            }
            set {
                Descriotion = value;
            }
        }

        /* MAPPING END */

    }

}
