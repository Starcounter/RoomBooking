using Screens.Common;
using Starcounter;
using System.Linq;
using System.Collections.Generic;
using CalendarSync.Database;

namespace RoomBooking.ViewModels.Partials
{
    partial class ScreenPartial : Json, IBound<Screen>
    {
        public IEnumerable<SyncedCalendar> Rooms => GetUserRooms(UserSession.GetSignedInUser());

        private IEnumerable<SyncedCalendar> GetUserRooms(User user)
        {
            return Db.SQL<SyncedCalendar>($"SELECT o.{nameof(UserRoomRelation.Room)} FROM {nameof(RoomBooking)}.\"{nameof(UserRoomRelation)}\" o WHERE o.{nameof(UserRoomRelation.User)} = ?", user);
//            return Db.SQL<Room>("SELECT o.Room FROM RoomBooking.UserRoomRelation o WHERE o.User = ?", user);
        }

        protected override void OnData()
        {
            base.OnData();


            if (this.Data != null)
            {
                RoomScreenRelation roomScreenRelation = Db.SQL<RoomScreenRelation>($"SELECT o FROM {nameof(RoomBooking)}.\"{nameof(RoomScreenRelation)}\" o WHERE o.{nameof(RoomScreenRelation.Screen)} = ?", this.Data).FirstOrDefault();
                if (roomScreenRelation != null)
                {
                    this.SelectedRoomId = roomScreenRelation.Room.GetObjectID();
                    this.Enable = roomScreenRelation.Enabled;
                }
            }
        }


        public void Handle(Input.Enable action)
        {
            RoomScreenRelation roomScreenRelation = Db.SQL<RoomScreenRelation>($"SELECT o FROM {nameof(RoomBooking)}.\"{nameof(RoomScreenRelation)}\" o WHERE o.{nameof(RoomScreenRelation.Screen)} = ?", this.Data).FirstOrDefault();
            if (roomScreenRelation != null) {
                roomScreenRelation.Enabled = action.Value;
            }
        }

        public void Handle(Input.SelectedRoomId action)
        {

            try
            {
                SyncedCalendar oldRoom = Db.FromId(this.SelectedRoomId) as SyncedCalendar;

                if (oldRoom != null)
                {
                    // Remove old room relation
                    RoomScreenRelation oldRoomScreenRelation = Db.SQL<RoomScreenRelation>($"SELECT o FROM {nameof(RoomBooking)}.\"{nameof(RoomScreenRelation)}\" o WHERE o.{nameof(RoomScreenRelation.Screen)} = ? AND o.{nameof(RoomScreenRelation.Room)} = ?", this.Data, oldRoom).FirstOrDefault();
                    oldRoomScreenRelation.Delete();
                }
            }
            catch
            {

            }
            try
            {
                SyncedCalendar newRoom = Db.FromId(action.Value) as SyncedCalendar;

                if (newRoom != null)
                {
                    // Create old room relation
                    RoomScreenRelation roomScreenRelation = Db.SQL<RoomScreenRelation>($"SELECT o FROM {nameof(RoomBooking)}.\"{nameof(RoomScreenRelation)}\" o WHERE o.{nameof(RoomScreenRelation.Screen)}  = ? AND o.{nameof(RoomScreenRelation.Room)} = ?", this.Data, newRoom).FirstOrDefault();
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
    partial class ScreenPartialRoomItem : Json, IBound<SyncedCalendar>
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
