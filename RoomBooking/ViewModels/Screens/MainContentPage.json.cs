using Starcounter;
using System;

namespace RoomBooking.ViewModels.Screens
{
    partial class MainContentPage : Json
    {
        public Room Room;
        public void Init(Room room)
        {
            this.Room = room;

            Db.Scope(() =>
            {
                CalendarPage calendarePage = new CalendarPage();

                calendarePage.Init(room.TimeZoneInfo);

                calendarePage.OnEventSelected = (roomBookingEvent) =>
                {
                    if (calendarePage.Transaction != null)
                    {
                        calendarePage.Transaction.Rollback();
                    }
                    this.ContentPartial = GetEventPage(roomBookingEvent);
                };

                calendarePage.OnNewBooking = (beginUtcDate, endUtcDate) =>
                {
                    if (calendarePage.Transaction != null)
                    {
                        calendarePage.Transaction.Rollback();
                    }

                    this.ContentPartial = this.GetNewBookingPage(room, beginUtcDate, endUtcDate);

                };

                this.CalendarPartial = calendarePage;
                this.ContentPartial = GetDefaultPage();
            });
        }


        private EventPage GetEventPage(RoomBookingEvent roomBookingEvent)
        {
            EventPage eventPage = new EventPage() { Data = roomBookingEvent };
            eventPage.OnClose = () => this.ContentPartial = GetDefaultPage();

            return eventPage;
        }

        private FreePage GetFreePage(Room room)
        {
            FreePage freePage = new FreePage();
            freePage.Init(room);
            freePage.OnNewBooking = () => this.ContentPartial = GetNewBookingPage(room, DateTime.UtcNow, DateTime.UtcNow.AddHours(1));

            return freePage;
        }

        private NewBookingPage GetNewBookingPage(Room room, DateTime defaultBeginUtcDate, DateTime defaultEndUtcDate)
        {
            RoomBookingEvent roomBookingEvent = new RoomBookingEvent()
            {
                BeginUtcDate = defaultBeginUtcDate,
                EndUtcDate = defaultEndUtcDate,
                Room = room
            };

            NewBookingPage newBookingPage = new NewBookingPage() { Data = roomBookingEvent };
            newBookingPage.OnClose = () => this.ContentPartial = GetDefaultPage();

            return newBookingPage;
        }

        /// <summary>
        /// 
        /// </summary>
        private Json GetDefaultPage()
        {
            // Show "FREE" page or "BUSY" page
            return GetFreePage(this.Room);
        }
    }
}
