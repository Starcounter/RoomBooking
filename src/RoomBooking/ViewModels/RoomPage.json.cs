using Starcounter;
using System;
using System.Collections.Generic;
using Starcounter.Templates;

namespace RoomBooking.ViewModels
{
    partial class RoomPage : Json, IBound<Room>
    {

        public override void Checkpoint()
        {
            base.Checkpoint();
        }

        protected override void OnData()
        {

            // This "workaround" is needed see github issue
            // https://github.com/Starcounter/Home/issues/316

            if (this.Data == null && !string.IsNullOrEmpty(this.RedirectUrl))
            {
                this.RedirectUrl = this.RedirectUrl;
            }


            base.OnData();
            if (this.Data != null)
            {
                this.SelectedTimeZoneId = this.Data.TimeZoneId;
            }
        }

        public string SelectedTimeZoneId {
            get {

                if (this.Data == null || string.IsNullOrEmpty(this.Data.TimeZoneId))
                {
                    return TimeZoneInfo.Local.Id;
                }

                return this.Data.TimeZoneId;
            }
            set {
                this.Data.TimeZoneId = value;
            }
        }

        public IEnumerable<TimeZoneInfo> TimeZones => TimeZoneInfo.GetSystemTimeZones();


        public void Handle(Input.SaveTrigger action)
        {
            this.Transaction.Commit();
            this.RedirectUrl = "/roomBooking/rooms";
        }

        public void Handle(Input.CloseTrigger action)
        {
            this.Transaction.Rollback();
            this.RedirectUrl = "/roomBooking/rooms";
        }

        public void Handle(Input.DeleteTrigger action)
        {

            MessageBoxButton deleteButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Yes, Text = "Delete", CssClass = "btn btn-sm btn-danger" };
            MessageBoxButton cancelButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Cancel, Text = "Cancel" };

            MessageBox.Show("Delete Room", "This Room will be deleted.", cancelButton, deleteButton, (result) =>
            {

                if (result == MessageBox.MessageBoxResult.Yes)
                {
                    Db.Transact(() =>
                    {
                        this.Data.Delete();
                    });
                }
            });
        }
    }
}
