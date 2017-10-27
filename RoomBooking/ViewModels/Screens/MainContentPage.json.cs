using Starcounter;
using System;

namespace RoomBooking.ViewModels.Screens
{
    partial class MainContentPage : Json
    {
        //                 string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        //calendarePage.NewBooking.BeginDate = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
        public TimeZoneInfo TimeZoneInfo;
        public Room Room;
        public void Init(Room room, TimeZoneInfo timeZoneInfo)
        {

            this.TimeZoneInfo = timeZoneInfo;
            this.Room = room;

            CalendarPage calendarePage = new CalendarPage();


            calendarePage.Init(this.TimeZoneInfo);
            //            calendarePage.NewBooking.BeginDate = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"); // TODO: Not standard format
            //            calendarePage.NewBooking.EndDate = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"); // TODO: Not standard format

            // 2015-01-02T11:42:13.510
            calendarePage.OnEventSelected = (roomBookingEvent) =>
            {
                this.ContentPartial = ShowEventPage(roomBookingEvent);

            };

            calendarePage.OnNewBooking = (beginUtcDate, endUtcDate) =>
            {

//                Db.Scope(() => {

                NewBookingPage newBookingPage = CreateNewBookingPage(beginUtcDate, endUtcDate);
                newBookingPage.OnClose = () =>
                {
                    this.ContentPartial = CreateFreePage();
                };
                this.ContentPartial = newBookingPage;
  //              });

            };

            this.CalendarPartial = calendarePage;
            this.ContentPartial = CreateFreePage();
        }

        /// <summary>
        /// Get content page.
        /// Depending on next upcomming event we show that event or the Free page
        /// </summary>
        /// <returns></returns>
        public static void RefreshContentPage()
        {
            MainContentPage mainContentPage = Program.GetMainContentPage();
            if (mainContentPage == null)
            {
                return;
            }
            int noticeTime = 15; // Show notice 15min before event begins

            //            DateTime roomLocalTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, room.TimeZoneInfo);


            DateTime noticeDate = DateTime.UtcNow.AddMinutes(noticeTime);

            RoomBookingEvent roomBookingEvent = Db.SQL<RoomBookingEvent>("SELECT o FROM RoomBooking.RoomBookingEvent o WHERE ? >= o.BeginUtcDate AND ? <= o.EndUtcDate ", noticeDate, DateTime.UtcNow).First;

            if (roomBookingEvent != null)
            {
                BusyPage busyPage = mainContentPage.ContentPartial as BusyPage;

                // Show event
                if (busyPage == null || busyPage.Booking.Data != roomBookingEvent)
                {
                    mainContentPage.ContentPartial = mainContentPage.CreateBusyPage(roomBookingEvent);
                }

            }
            else
            {
                // Show free page

                if (!(mainContentPage.ContentPartial is FreePage))
                {
                    mainContentPage.ContentPartial = mainContentPage.CreateFreePage();
                }
            }
        }

        private BusyPage CreateBusyPage(RoomBookingEvent roomBookingEvent)
        {

            BusyPage page = new BusyPage();
            page.OnClose = () =>
            {
                this.ContentPartial = CreateFreePage();
            };
            page.Data = roomBookingEvent;
            this.ContentPartial = page;
            return page;
        }

        private EventPage ShowEventPage(RoomBookingEvent roomBookingEvent)
        {
            EventPage page = new EventPage();
            page.OnClose = () =>
            {
                this.ContentPartial = CreateFreePage();
            };
            page.Data = roomBookingEvent;
            this.ContentPartial = page;
            return page;
        }

        private FreePage CreateFreePage()
        {

//            Room room = Program.GetRoom();

            FreePage freePage = new FreePage();
            freePage.Init(this.Room);
            freePage.OnNewBooking = () =>
            {
                // TODO Fix default dates
                NewBookingPage newBookingPage = CreateNewBookingPage(DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
                newBookingPage.OnClose = () =>
                {
                    this.ContentPartial = freePage;
                };
                this.ContentPartial = newBookingPage;
            };
            return freePage;
        }

        private NewBookingPage CreateNewBookingPage(DateTime defaultBeginUtcDate, DateTime defaultEndUtcDate)
        {
            //return Db.Scope(() =>
            //{            // TEST


                NewBookingPage newBookingPage = new NewBookingPage();
            newBookingPage.Init(this.Room);

                //RoomBookingEvent newBooking = new RoomBookingEvent();
                //newBooking.BeginUtcDate = DateTime.UtcNow;
                //newBooking.EndUtcDate = DateTime.UtcNow.AddHours(1);
                //newBooking.Title = "This is a temp booking";
                //newBooking.Description = "This is a temp booking";  // TODO;
                //newBooking.Room = this.Room;

                //newBookingPage.Data = newBooking;

            DateTime beginDate = TimeZoneInfo.ConvertTimeFromUtc(defaultBeginUtcDate, this.TimeZoneInfo);
            //            DateTime endDate = TimeZoneInfo.ConvertTimeFromUtc(defaultEndUtcDate, room.TimeZoneInfo);

            newBookingPage.BeginDay = beginDate.ToString("yyyy-MM-dd");
            newBookingPage.StartTime = beginDate.ToString("HH:mm");

            newBookingPage.DurationHours = (long)(defaultEndUtcDate - defaultBeginUtcDate).TotalHours;
            newBookingPage.DurationMinutes = (long)(defaultEndUtcDate - defaultBeginUtcDate).Minutes;


            return newBookingPage;
            //});

        }

    }
}
