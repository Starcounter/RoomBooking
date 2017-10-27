using Screens.Common;
using Starcounter;
using System.Collections.Generic;

namespace RoomBooking.ViewModels
{
    partial class RoomsPage : Json
    {
        public IEnumerable<Room> Rooms => Db.SQL<Room>("SELECT o.Room FROM RoomBooking.UserRoomRelation o WHERE o.User = ? ORDER BY o.Room.Name", UserSession.GetSignedInUser());
    }

    [RoomsPage_json.Rooms]
    partial class RoomsPageRoomItem : Json, IBound<Room>
    {

        public string Url => string.Format("/roomBooking/rooms/{0}", this.Data?.GetObjectID());


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
