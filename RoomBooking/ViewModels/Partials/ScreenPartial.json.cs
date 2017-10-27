using Screens.Common;
using Starcounter;
using System.Collections.Generic;
namespace RoomBooking.ViewModels.Partials
{
    partial class ScreenPartial : Json, IBound<Screen>
    {
//        public IEnumerable<Room> Rooms => Db.SQL<Room>("SELECT o FROM RoomBooking.Room o");
//        public IEnumerable<Room> Rooms => Db.SQL<Room>("SELECT o.Room FROM RoomBooking.UserRoomRelation o WHERE o.User = ?", UserSession.GetSignedInUser());
        public IEnumerable<Room> Rooms => GetUserRooms(UserSession.GetSignedInUser());

        private IEnumerable<Room> GetUserRooms(User user)
        {
            return Db.SQL<Room>("SELECT o.Room FROM RoomBooking.UserRoomRelation o WHERE o.User = ?", user);
        }

        protected override void OnData()
        {
            base.OnData();


            if (this.Data != null)
            {
                RoomScreenRelation roomScreenRelation = Db.SQL<RoomScreenRelation>("SELECT o FROM RoomBooking.RoomScreenRelation o WHERE o.Screen = ?", this.Data).First;
                if (roomScreenRelation != null)
                {
                    this.SelectedRoomId = roomScreenRelation.Room.GetObjectID();
                    this.Enable = roomScreenRelation.Enabled;
                }
            }
        }


        public void Handle(Input.Enable action)
        {
            RoomScreenRelation roomScreenRelation = Db.SQL<RoomScreenRelation>("SELECT o FROM RoomBooking.RoomScreenRelation o WHERE o.Screen = ?", this.Data).First;
            if (roomScreenRelation != null) {
                roomScreenRelation.Enabled = action.Value;
            }
        }

        public void Handle(Input.SelectedRoomId action)
        {

            try
            {
                Room oldRoom = Db.FromId(this.SelectedRoomId) as Room;

                if (oldRoom != null)
                {
                    // Remove old room relation
                    RoomScreenRelation oldRoomScreenRelation = Db.SQL<RoomScreenRelation>("SELECT o FROM RoomBooking.RoomScreenRelation o WHERE o.Screen = ? AND o.Room = ?", this.Data, oldRoom).First;
                    oldRoomScreenRelation.Delete();
                }
            }
            catch
            {

            }
            try
            {
                Room newRoom = Db.FromId(action.Value) as Room;

                if (newRoom != null)
                {
                    // Create old room relation
                    RoomScreenRelation roomScreenRelation = Db.SQL<RoomScreenRelation>("SELECT o FROM RoomBooking.RoomScreenRelation o WHERE o.Screen = ? AND o.Room = ?", this.Data, newRoom).First;
                    if (roomScreenRelation == null)
                    {
                        roomScreenRelation = new RoomScreenRelation();
                        roomScreenRelation.Screen = this.Data;
                        roomScreenRelation.Room = newRoom;
                    }

                }

            }
            catch
            {

            }


        }
    }


    [ScreenPartial_json.Rooms]
    partial class ScreenPartialRoomItem : Json, IBound<Room>
    {

        public string Id => this.Data?.GetObjectID();

        public string UrlString {
            get {
                return string.Format("/roomBooking/rooms/{0}", this.Data?.GetObjectID());
            }
        }
        public void Handle(Input.SelectTrigger action)
        {

            //MessageBoxButton deleteButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Yes, Text = "Delete", CssClass = "btn btn-sm btn-danger" };
            //MessageBoxButton cancelButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Cancel, Text = "Cancel" };

            //MessageBox.Show("Delete Room", "This Room will be deleted.", cancelButton, deleteButton, (result) =>
            //{

            //    if (result == MessageBox.MessageBoxResult.Yes)
            //    {
            //        Db.Transact(() =>
            //        {
            //            this.Data.Delete();
            //        });
            //    }
            //});


        }


    }

}
