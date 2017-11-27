using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter.Templates;
using System.Threading;

namespace RoomBooking.ViewModels.Screens
{
    partial class ScreenContentPage : Json, IBound<RoomScreenRelation>
    {
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


        public RoomBookingEvent ActiveEvent {
            get {
                if (this.Data == null) return null;
                DateTime utcNow = DateTime.UtcNow;
                RoomBookingEvent roomBookingEvent = Db.SQL<RoomBookingEvent>($"SELECT o FROM {nameof(RoomBooking)}.\"{nameof(RoomBookingEvent)}\" o WHERE o.{nameof(RoomBookingEvent.Room)} = ? AND ? >= o.{nameof(RoomBookingEvent.BeginUtcDate)} AND ? < o.{nameof(RoomBookingEvent.EndUtcDate)} AND o.{nameof(RoomBookingEvent.EndUtcDate)} >= o.{nameof(RoomBookingEvent.BeginUtcDate)} ORDER BY o.{nameof(RoomBookingEvent.BeginUtcDate)}", this.Data.Room, utcNow, utcNow).FirstOrDefault();
                return roomBookingEvent;
            }
        }

        public bool DefaultPageTrigger {
            get {
                Json page = GetDefaultPage();
                if (page == this.ContentPartial)
                {
                    return true;
                }
                this.ContentPartial = page;
                return true;
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
                if (this.ContentPartial is NewQuickBookingPage && ((NewQuickBookingPage)this.ContentPartial).Data.Equals(roomBookingEvent))
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
                else if (this.ContentPartial is NewQuickBookingPage)
                {
                    NewQuickBookingPage newQuickBookingPage = this.ContentPartial as NewQuickBookingPage;

                    // Change Date (year,month day)
                    // Note: Do not touch the time

                    // Get duration
                    TimeSpan duration = newQuickBookingPage.Data.EndUtcDate - newQuickBookingPage.Data.BeginUtcDate;
                    // 1 Get room time

                    DateTime roomBeginTime = TimeZoneInfo.ConvertTimeFromUtc(newQuickBookingPage.Data.BeginUtcDate, this.Data.Room.TimeZoneInfo);

                    // 2. change year,month,day
                    DateTime newRoomBeginTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, roomBeginTime.Hour, roomBeginTime.Minute, roomBeginTime.Second, roomBeginTime.Millisecond, DateTimeKind.Unspecified);

                    // 3. convert to UTC
                    newQuickBookingPage.Data.BeginUtcDate = TimeZoneInfo.ConvertTimeToUtc(newRoomBeginTime, this.Data.Room.TimeZoneInfo);
                    newQuickBookingPage.Data.EndUtcDate = newQuickBookingPage.Data.BeginUtcDate + duration;
                }


            };

            this.CalendarPartial = calendarePage;

//            RegisterTimer();
//            SetEventTimer();

        }


        #region Pages

        private Json GetDefaultPage()
        {
            if (this.Data == null) return new Json();

            // If user is viewing an event do not switch the view
            if (this.ContentPartial is EventPage)
            {
                return this.ContentPartial;
            }

            // User is making a new booking event
            if (this.ContentPartial is NewBookingPage)
            {
                return this.ContentPartial;
            }


            // If there is an active event, show it
            RoomBookingEvent roomBookingEvent = this.ActiveEvent;
            if (roomBookingEvent != null)
            {
                if (this.ContentPartial is BusyPage)
                {
                    // TODO: Booking.Data was null
                    //if (((BusyPage)this.ContentPartial).Booking.Data.Equals(roomBookingEvent))
                    //{
                    //    return this.ContentPartial;
                    //}
                    return this.ContentPartial;
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
        private EventPage CreateEventPage(RoomBookingEvent roomBookingEvent)
        {
            EventPage eventPage = new EventPage() { Data = roomBookingEvent };
            eventPage.OnClose = () =>
            {
                this.ContentPartial = new Json();
            };

            return eventPage;
        }

        /// <summary>
        /// Create Free page
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        private FreePage CreateFreePage(Room room)
        {
            FreePage freePage = new FreePage();
            freePage.Init(room);
            freePage.OnNewBooking = () =>
            {
                this.ContentPartial = CreateNewBookingPage(room, DateTime.UtcNow, DateTime.UtcNow.AddHours(1), "Booked from screen");
            };

            return freePage;
        }

        /// <summary>
        /// Create Busy page
        /// </summary>
        /// <param name="roomBookingEvent"></param>
        /// <returns></returns>
        private BusyPage CreateBusyPage(RoomBookingEvent roomBookingEvent)
        {
            BusyPage busyPage = new BusyPage();
            busyPage.Booking.Data = roomBookingEvent;
            busyPage.OnClose = () =>
            {
                this.ContentPartial = new Json();   // Workaround

            };
            busyPage.OnClaim = () =>
            {
                this.ContentPartial = CreateNewBookingPage(roomBookingEvent.Room, DateTime.UtcNow, DateTime.UtcNow.AddHours(1));

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
        private NewBookingPage CreateNewBookingPage(Room room, DateTime defaultBeginUtcDate, DateTime defaultEndUtcDate, string name = null)
        {
            RoomBookingEvent roomBookingEvent = new RoomBookingEvent()
            {
                BeginUtcDate = defaultBeginUtcDate,
                EndUtcDate = defaultEndUtcDate,
                Room = room,
                Name = name
            };

            NewBookingPage newBookingPage = new NewBookingPage() { Data = roomBookingEvent };
            newBookingPage.OnClose = () =>
            {
                this.ContentPartial = new Json();
            };

            return newBookingPage;
        }

        #endregion


        #region Timer

        private static Timer EventTimer;

        private void RegisterTimer()
        {
            EventTimer = new Timer(TimerCallback);
            SetEventTimer();
        }



        private void TimerCallback(Object state)
        {
            Scheduling.RunTask(() =>
            {
                // Set timer to next event
                SetEventTimer();
                Utils.PushChanges();
            });
        }


        private void SetEventTimer()
        {
            TimeSpan timeSpan = new TimeSpan(0, 0, 5);
            EventTimer.Change(1000*5,1000*5);

            //DateTime utcNow = DateTime.UtcNow;
            //RoomBookingEvent firstEvent = Db.SQL<RoomBookingEvent>($"SELECT o FROM {typeof(RoomBookingEvent)} o WHERE o.{nameof(RoomBookingEvent.BeginUtcDate)} >= ? ORDER BY o.{nameof(RoomBookingEvent.BeginUtcDate)}", utcNow).FirstOrDefault();

            //if (firstEvent != null)
            //{
            //    TimeSpan timeSpan = firstEvent.BeginUtcDate - utcNow;
            //    EventTimer.Change(timeSpan, TimeSpan.FromTicks(-1));
            //}
        }

        #endregion


    }
}
