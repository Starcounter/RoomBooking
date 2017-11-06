using Starcounter;
using System;

namespace RoomBooking.ViewModels.Screens
{
    partial class EventPage : Json, IBound<RoomBookingEvent>
    {
        public Action OnClose;

        public void Handle(Input.CloseTrigger action)
        {
            this.OnClose?.Invoke();
        }

        public void Handle(Input.DeleteTrigger action)
        {
            Db.Transact(() => this.Data.Delete());

            this.OnClose?.Invoke();
        }
    }

}
