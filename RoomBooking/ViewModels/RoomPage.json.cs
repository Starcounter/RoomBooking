using Starcounter;
using System;
using System.Collections.Generic;

namespace RoomBooking.ViewModels
{
    partial class RoomPage : Json, IBound<Room>
    {

        protected override void OnData()
        {
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
    }
}
