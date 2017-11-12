using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter.Templates;
using System.Threading;
using CalendarSync.Database;

namespace RoomBooking.ViewModels.Screens
{
    partial class ScreenContentPage : Json, IBound<RoomScreenRelation>
    {

        public void OnActiveEvent()
        {
            this.ContentPartial = GetDefaultPage();
        }



        protected override void OnData()
        {
            base.OnData();

            if (this.Data != null)
            {
                this.Setup();
            }
            else
            {
                this.CalendarPartial = new Json();
                this.ContentPartial = new Json();
            }
        }


        public SyncedEvent ActiveEvent {
            get {
                if (this.Data == null) return null;
                DateTime utcNow = DateTime.UtcNow;
                SyncedEvent roomBookingEvent = Db.SQL<SyncedEvent>($"SELECT o FROM CalendarSync.Database.\"{nameof(SyncedEvent)}\" o WHERE o.{nameof(SyncedEvent.Calendar)} = ? AND ? >= o.{nameof(SyncedEvent.BeginUtcDate)} AND ? < o.{nameof(SyncedEvent.EndUtcDate)} AND o.{nameof(SyncedEvent.EndUtcDate)} >= o.{nameof(SyncedEvent.BeginUtcDate)} ORDER BY o.{nameof(SyncedEvent.BeginUtcDate)}", this.Data.Room, utcNow, utcNow).FirstOrDefault();
                return roomBookingEvent;
            }
        }

        private void Setup()
        {
            CalendarPage calendarePage = new CalendarPage();

            calendarePage.Init(this.Data.Room.TimeZoneInfo);

            calendarePage.OnEventSelected = (roomBookingEvent) =>
            {


                if (this.ContentPartial is NewBookingPage && ((NewBookingPage)this.ContentPartial).Data.Equals(roomBookingEvent))
                {
                    return;
                }

                if (this.ContentPartial is EventPage && ((EventPage)this.ContentPartial).Data.Equals(roomBookingEvent))
                {
                    return;
                }

                if (calendarePage.Transaction != null)
                {
                    calendarePage.Transaction.Rollback();
                }

                this.ContentPartial = CreateEventPage(roomBookingEvent);
            };

            calendarePage.OnNewBooking = (beginUtcDate, endUtcDate) =>
            {
                if (calendarePage.Transaction != null)
                {
                    calendarePage.Transaction.Rollback();
                }

                this.ContentPartial = this.CreateNewBookingPage(this.Data.Room, beginUtcDate, endUtcDate);

            };

            calendarePage.OnSelectedDate = (selectedDate) =>
            {
                // If there is a new booking page open, then change the date

                if (this.ContentPartial is NewBookingPage)
                {
                    NewBookingPage newBookingPage = this.ContentPartial as NewBookingPage;

                    // Change Date (year,month day)
                    // Note: Do not touch the time

                    // Get duration
                    TimeSpan duration = newBookingPage.Data.EndUtcDate - newBookingPage.Data.BeginUtcDate;
                    // 1 Get room time

                    DateTime roomBeginTime = TimeZoneInfo.ConvertTimeFromUtc(newBookingPage.Data.BeginUtcDate, this.Data.Room.TimeZoneInfo);

                    // 2. change year,month,day
                    DateTime newRoomBeginTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, roomBeginTime.Hour, roomBeginTime.Minute, roomBeginTime.Second, roomBeginTime.Millisecond, DateTimeKind.Unspecified);

                    // 3. convert to UTC
                    newBookingPage.Data.BeginUtcDate = TimeZoneInfo.ConvertTimeToUtc(newRoomBeginTime, this.Data.Room.TimeZoneInfo);
                    newBookingPage.Data.EndUtcDate = newBookingPage.Data.BeginUtcDate + duration;


                }
            };

            this.CalendarPartial = calendarePage;
            this.ContentPartial = GetDefaultPage();

            // this.RegisterTimer();
        }


        #region Pages

        /// <summary>
        /// Show default page
        /// Free, Busy or Event
        /// </summary>
        private Json GetDefaultPage()
        {
            if (this.Data == null) return new Json();

            // If user is viewing an event do not switch the view
            if (this.ContentPartial is EventPage)
            {
                return this.ContentPartial;
            }

            // Get current event
            SyncedEvent roomBookingEvent = this.ActiveEvent;
            if (roomBookingEvent != null)
            {
                if (this.ContentPartial is BusyPage)
                {
                    // TODO: Booking.Data was null
                    if (((BusyPage)this.ContentPartial).Booking.Data.Equals(roomBookingEvent))
                    {
                        return this.ContentPartial;
                    }
                }
                return CreateBusyPage(roomBookingEvent);
            }

            if (this.ContentPartial is FreePage)
            {
                return this.ContentPartial;
            }
            return CreateFreePage(this.Data.Room);
        }


        /// <summary>
        /// Create Event page
        /// </summary>
        /// <param name="roomBookingEvent"></param>
        /// <returns></returns>
        private EventPage CreateEventPage(SyncedEvent roomBookingEvent)
        {
            EventPage eventPage = new EventPage() { Data = roomBookingEvent };
            eventPage.OnClose = () =>
            {
                this.ContentPartial = new Json();   // Workaround
                this.ContentPartial = GetDefaultPage();
            };

            return eventPage;
        }

        /// <summary>
        /// Create Free page
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        private FreePage CreateFreePage(SyncedCalendar room)
        {
            FreePage freePage = new FreePage();
            freePage.Init(room);
            freePage.OnNewBooking = () => this.ContentPartial = CreateNewBookingPage(room, DateTime.UtcNow, DateTime.UtcNow.AddHours(1), "Quick booking");   // TODO:

            return freePage;
        }

        /// <summary>
        /// Create Busy page
        /// </summary>
        /// <param name="roomBookingEvent"></param>
        /// <returns></returns>
        private BusyPage CreateBusyPage(SyncedEvent roomBookingEvent)
        {
            BusyPage busyPage = new BusyPage();
            busyPage.Booking.Data = roomBookingEvent;
            busyPage.OnClose = () => this.ContentPartial = GetDefaultPage();
            busyPage.OnClaim = () =>
            {
                this.ContentPartial = CreateNewBookingPage(roomBookingEvent.Calendar, DateTime.UtcNow, DateTime.UtcNow.AddHours(1));

                Db.Transact(() =>
                {
                    roomBookingEvent.Delete();  // TODO: Do not delete "claimed" booking
                });
            };

            return busyPage;
        }

        /// <summary>
        /// Create New booking page
        /// </summary>
        /// <param name="room"></param>
        /// <param name="defaultBeginUtcDate"></param>
        /// <param name="defaultEndUtcDate"></param>
        /// <returns></returns>
        private NewBookingPage CreateNewBookingPage(SyncedCalendar room, DateTime defaultBeginUtcDate, DateTime defaultEndUtcDate, string name = null)
        {
            SyncedEvent roomBookingEvent = new SyncedEvent()
            {
                BeginUtcDate = defaultBeginUtcDate,
                EndUtcDate = defaultEndUtcDate,
                Calendar = room,
                Name = name
            };

            NewBookingPage newBookingPage = new NewBookingPage() { Data = roomBookingEvent };
            newBookingPage.OnClose = () => this.ContentPartial = GetDefaultPage();

            return newBookingPage;
        }

        #endregion


        //#region Timer


        //public void RegisterTimer()
        //{
        //    EventTimer = new Timer(TimerCallback);
        //    SetEventTimer();
        //}

        //private Timer EventTimer;

        //private void SetEventTimer()
        //{
        //    DateTime utcNow = DateTime.UtcNow;

        //    RoomBookingEvent firstEvent = RoomBookingEvent.GetNextEvent(this.Data.Room);
        //    if (firstEvent != null)
        //    {
        //        TimeSpan timeSpan = firstEvent.BeginUtcDate - utcNow;
        //        EventTimer.Change(timeSpan, TimeSpan.FromTicks(-1));
        //    }
        //}

        //public void TimerCallback(Object state)
        //{
        //    Scheduling.ScheduleTask(() =>
        //    {
        //        // Set timer to next event
        //        SetEventTimer();

        //        this.ContentPartial = GetDefaultPage();

        //        Program.PushChanges();

        //    }, false);

        //}


        //#endregion

    }
}
