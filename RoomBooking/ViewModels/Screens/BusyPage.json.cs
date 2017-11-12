using CalendarSync.Database;
using Starcounter;
using System;
using System.Threading;

namespace RoomBooking.ViewModels.Screens
{


    partial class BusyPage : Json
    {
        public Action OnClaim;
        public Action OnClose;

        public void Handle(Input.ClaimTrigger action)
        {

            MessageBoxButton deleteButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Yes, Text = "Claim", CssClass = "btn btn-sm btn-danger" };
            MessageBoxButton cancelButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Cancel, Text = "Cancel" };

            MessageBox.Show("Claim Room", "This Room will be claimed and current event be deleted.", cancelButton, deleteButton, (result) =>
            {
                if (result == MessageBox.MessageBoxResult.Yes)
                {
                       this.OnClaim?.Invoke();
                }
            });
        }

    }

    [BusyPage_json.Booking]
    partial class BusyPageBooking : Json, IBound<SyncedEvent>
    {
        private Timer EventTimer = null;

        public DateTime BeginDate {
            get {

                if (this.Data != null)
                {
                    SetEventTimer();
                    return TimeZoneInfo.ConvertTimeFromUtc(this.Data.BeginUtcDate, this.Data.Calendar.TimeZoneInfo);
                }

                return DateTime.MaxValue;
            }
        }

        public DateTime EndDate {
            get {

                if (this.Data != null)
                {
                    SetEventTimer();
                    return TimeZoneInfo.ConvertTimeFromUtc(this.Data.EndUtcDate, this.Data.Calendar.TimeZoneInfo);
                }

                return DateTime.MaxValue;
            }
        }




        protected override void OnData()
        {
            base.OnData();
            if (this.Data != null)
            {
                RegisterTimer();
            }
            else
            {
                if (this.EventTimer != null)
                {
                    this.EventTimer.Dispose();
                }
            }
        }

        public void RegisterTimer()
        {
            EventTimer = new Timer(TimerCallback);
            SetEventTimer();
        }


        private DateTime previousEndUtcDate;
        private void SetEventTimer()
        {

            if (this.previousEndUtcDate == this.Data.EndUtcDate)
            {
                return;
            }

            DateTime utcNow = DateTime.UtcNow;

            //EventTimer.Change(new TimeSpan(0,0,0,10,0), TimeSpan.FromTicks(Timeout.Infinite));
            TimeSpan timeSpan = this.Data.EndUtcDate - utcNow;

            if (timeSpan.TotalSeconds < 0)
            {
                if (this.EventTimer != null)
                {
                    this.EventTimer.Dispose();
                }
            }
            else
            {
                this.EventTimer.Change(timeSpan, TimeSpan.FromTicks(Timeout.Infinite));
                this.previousEndUtcDate = this.Data.EndUtcDate;
            }

        }
        public void TimerCallback(Object state)
        {
            Scheduling.ScheduleTask(() =>
            {
                BusyPage busyPage = this.Parent as BusyPage;
                busyPage.OnClose?.Invoke();

                Session.ForAll((session, sessionId) =>
                {
                    session.CalculatePatchAndPushOnWebSocket();
                });

            }, false);
        }


    }


}
