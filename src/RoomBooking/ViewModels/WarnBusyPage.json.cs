using Starcounter;
using System;

namespace RoomBooking.ViewModels
{
    partial class WarnBusyPage : Json
    {
        public Action OnClaim;
        public Action OnClose;

        public void Handle(Input.SyncTimeTrigger action)
        {
            // 2008-09-22T14:01:54.9571247Z
            this.ServerUTCDate = DateTime.UtcNow.ToString("o");
        }

        public void Handle(Input.ClaimTrigger action)
        {

            MessageBoxButton deleteButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Yes, Text = "Claim", CssClass = "btn btn-sm btn-danger" };
            MessageBoxButton cancelButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Cancel, Text = "Cancel" };

            MessageBox.Show("Claim Room", "This Room will be claimed and current event be deleted.", cancelButton, deleteButton, Utils.CONTENT_PAGE_TYPE, (result) =>
            {
                if (result == MessageBox.MessageBoxResult.Yes)
                {
                    this.OnClaim?.Invoke();
                }
            });
        }

    }


    [WarnBusyPage_json.Booking]
    partial class WarnBusyPageBooking : Json, IBound<RoomBookingEvent>
    {
        public DateTime BeginDate {
            get {

                if (this.Data != null)
                {
                    //SetEventTimer();
                    return TimeZoneInfo.ConvertTimeFromUtc(this.Data.BeginUtcDate, this.Data.Room.TimeZoneInfo);
                }

                return DateTime.MaxValue;
            }
        }

        public DateTime EndDate {
            get {

                if (this.Data != null)
                {
                    //SetEventTimer();
                    return TimeZoneInfo.ConvertTimeFromUtc(this.Data.EndUtcDate, this.Data.Room.TimeZoneInfo);
                }

                return DateTime.MaxValue;
            }
        }

        public DateTime WarnDate {
            get {

                if (this.Data != null)
                {
                    return TimeZoneInfo.ConvertTimeFromUtc(this.Data.WarnUtcDate, this.Data.Room.TimeZoneInfo);
                }

                return DateTime.MaxValue;
            }
        }


        protected override void OnData()
        {
            base.OnData();
            if (this.Data == null)
            {
                WarnBusyPage warnBusyPage = this.Parent as WarnBusyPage;
                warnBusyPage.OnClose?.Invoke();
            }
        }


    }


}
