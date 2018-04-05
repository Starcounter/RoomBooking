using Starcounter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RoomBooking.ViewModels.Screens
{
    partial class CalendarPage : Json
    {
        public Action<DateTime, DateTime> OnNewBooking;
        public Action<RoomBookingEvent> OnEventSelected;
        public Action<DateTime> OnSelectedDate;
        public TimeZoneInfo TimeZoneInfo;
        public DateTime SelectedUtcDate;

        public IEnumerable<RoomBookingEvent> Bookings => Db.SQL<RoomBookingEvent>($"SELECT o FROM {nameof(RoomBooking)}.\"{nameof(RoomBookingEvent)}\" o WHERE o.{nameof(RoomBookingEvent.BeginUtcDate)} >= ? AND o.{nameof(RoomBookingEvent.BeginUtcDate)} < ? AND o.{nameof(RoomBookingEvent.EndUtcDate)} >= o.{nameof(RoomBookingEvent.BeginUtcDate)} ORDER BY o.{nameof(RoomBookingEvent.BeginUtcDate)}", SelectedUtcDate, SelectedUtcDate.AddDays(1));

        public void Init(TimeZoneInfo timeZoneInfo)
        {
            this.TimeZoneInfo = timeZoneInfo;

            DateTime clientLocalTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.Date, this.TimeZoneInfo).Date;
//            this.SelectedDate = clientLocalTime.ToString("yyyy-MM-dd");
            this.SelectedUtcDate = TimeZoneInfo.ConvertTimeToUtc(clientLocalTime, this.TimeZoneInfo);
        }

        public string SelectedDate {
            get {
                DateTime clientLocalTime = TimeZoneInfo.ConvertTimeFromUtc(this.SelectedUtcDate, this.TimeZoneInfo).Date;
                return clientLocalTime.ToString("yyyy-MM-dd");

            }
        }

        public void Handle(Input.SelectedDate action)
        {
            // ValueType should be in the format of "yyyy-MM-dd"
            try
            {
                DateTime dateTime = DateTime.ParseExact(action.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                this.SelectedUtcDate = TimeZoneInfo.ConvertTimeToUtc(dateTime, this.TimeZoneInfo);
                this.OnSelectedDate?.Invoke(dateTime);
            }
            catch (Exception)
            {
                this.SelectedUtcDate = DateTime.UtcNow.Date;
                this.OnSelectedDate?.Invoke(DateTime.Now);
            }

        }
    }


    [CalendarPage_json.Bookings]
    partial class CalendarItem : Json, IBound<RoomBookingEvent>
    {


        public DateTime BeginDateStr {

            get {

                CalendarPage calendarPage = this.Parent.Parent as CalendarPage;
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(this.Data.BeginUtcDate, calendarPage.TimeZoneInfo);
                return datetime;
            }

        }

        public DateTime EndDateStr {

            get {
                CalendarPage calendarPage = this.Parent.Parent as CalendarPage;
                return TimeZoneInfo.ConvertTimeFromUtc(this.Data.EndUtcDate, calendarPage.TimeZoneInfo);
            }

        }

        public bool IsSelectedFlag {
            get {

                CalendarPage calendarPage = this.Parent.Parent as CalendarPage;
                ContentPage mainContentPage = calendarPage.Parent as ContentPage;

                if (mainContentPage.ContentPartial != null)
                {
                    RoomBookingEvent roomBookingEvent = mainContentPage.ContentPartial.Data as RoomBookingEvent;
                    return this.Data.Equals(roomBookingEvent);
                }


                return false;// TODO:
            }
        }


        public bool Overlapping {
            get {

                return Db.SQL<RoomBookingEvent>($"SELECT o FROM {nameof(RoomBooking)}.\"{nameof(RoomBookingEvent)}\" o WHERE o <> ? AND o.{nameof(RoomBookingEvent.Room)} = ? AND o.{nameof(RoomBookingEvent.BeginUtcDate)} < ? AND ? < o.{nameof(RoomBookingEvent.EndUtcDate)}", this.Data, this.Data.Room, this.Data.EndUtcDate, this.Data.BeginUtcDate).FirstOrDefault() != null;
            }
        }


        public bool IsActive => GetIsActive();

        public bool GetIsActive()
        {
            DateTime utcNow = DateTime.UtcNow;
            return (utcNow >= this.Data.BeginUtcDate) && (utcNow < this.Data.EndUtcDate);
        }


        public void Handle(Input.SelectedTrigger action)
        {
            CalendarPage calendarPage = this.Parent.Parent as CalendarPage;
            calendarPage.OnEventSelected?.Invoke(this.Data);
        }
    }

    [CalendarPage_json.NewBooking]
    partial class CalendarNewBooking : Json
    {
        public void Handle(Input.CreateTrigger action)
        {
            // TODO Validate

            CalendarPage calendarPage = this.Parent as CalendarPage;
            if (string.IsNullOrEmpty(this.BeginDate) || string.IsNullOrEmpty(this.EndDate))
            {
                return;
            }

            // Incomming time "2017-10-17T00:00:00.000Z" is in local room time so we need to remove the 'Z'
            // "2017-10-25T02:00:00.000Z"

            

            DateTime beginDate = DateTime.ParseExact(this.BeginDate.Remove(this.BeginDate.Length-1), "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime endDate = DateTime.ParseExact(this.EndDate.Remove(this.EndDate.Length-1), "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None);

            DateTime beginUtcDate = TimeZoneInfo.ConvertTimeToUtc(beginDate, calendarPage.TimeZoneInfo);
            DateTime endUtcDate = TimeZoneInfo.ConvertTimeToUtc(endDate, calendarPage.TimeZoneInfo);

            calendarPage.OnNewBooking?.Invoke(beginUtcDate, endUtcDate);
        }
    }




}
