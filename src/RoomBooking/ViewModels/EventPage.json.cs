using Starcounter;
using System;

namespace RoomBooking.ViewModels
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

            MessageBoxButton deleteButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Yes, Text = "Delete", CssClass = "btn btn-sm btn-danger" };
            MessageBoxButton cancelButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Cancel, Text = "Cancel" };

            MessageBox.Show("Delete Event", "This event will be deleted.", cancelButton, deleteButton, Utils.CONTENT_PAGE_TYPE, (result) =>
            {

                if (result == MessageBox.MessageBoxResult.Yes)
                {
                    Db.Transact(() => {
                        this.Data.Delete();
                    });
                    this.OnClose?.Invoke();
                }
            });


        }
    }

}
