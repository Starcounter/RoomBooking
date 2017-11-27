using Starcounter;
using System.Linq;
using System.Collections.Generic;

namespace RoomBooking.ViewModels.Partials
{
    partial class ScreenPartial : Json
    {
        public string ScreenId;

        public IEnumerable<Room> Rooms => GetUserRooms(UserSession.GetSignedInUser());

        private IEnumerable<Room> GetUserRooms(User user)
        {
            return Db.SQL<Room>($"SELECT o.{nameof(UserRoomRelation.Room)} FROM {typeof(UserRoomRelation)} o WHERE o.{nameof(UserRoomRelation.User)} = ?", user);
        }

        public void Init(string screenId)
        {
            this.ScreenId = screenId;

            RoomScreenRelation roomScreenRelation = Db.SQL<RoomScreenRelation>($"SELECT o FROM {typeof(RoomScreenRelation)} o WHERE o.{nameof(RoomScreenRelation.ScreenId)} = ?", this.ScreenId).FirstOrDefault();
            if (roomScreenRelation != null)
            {
                this.SelectedRoomId = roomScreenRelation.Room.GetObjectID();
                this.Enable = roomScreenRelation.Enabled;
            }
        }

        public void Handle(Input.Enable action)
        {
            RoomScreenRelation roomScreenRelation = Db.SQL<RoomScreenRelation>($"SELECT o FROM {typeof(RoomScreenRelation)} o WHERE o.{nameof(RoomScreenRelation.ScreenId)} = ?", this.ScreenId).FirstOrDefault();

            if (roomScreenRelation == null)
            {

                Room room = this.Rooms.FirstOrDefault();
                if (room != null)
                {
                    roomScreenRelation = new RoomScreenRelation();
                    roomScreenRelation.ScreenId = this.ScreenId;
                    roomScreenRelation.Room = room;
                    this.SelectedRoomId = roomScreenRelation.Room.GetObjectID();
                }
            }

            if (roomScreenRelation != null)
            {
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
                    RoomScreenRelation oldRoomScreenRelation = Db.SQL<RoomScreenRelation>($"SELECT o FROM {typeof(RoomScreenRelation)} o WHERE o.{nameof(RoomScreenRelation.ScreenId)} = ? AND o.{nameof(RoomScreenRelation.Room)} = ?", this.ScreenId, oldRoom).FirstOrDefault();
                    oldRoomScreenRelation.Delete();
                }
            }
            catch
            {
                // TODO:
            }
            try
            {
                Room newRoom = Db.FromId(action.Value) as Room;

                if (newRoom != null)
                {
                    // Create old room relation
                    RoomScreenRelation roomScreenRelation = Db.SQL<RoomScreenRelation>($"SELECT o FROM {typeof(RoomScreenRelation)} o WHERE o.{nameof(RoomScreenRelation.ScreenId)}  = ? AND o.{nameof(RoomScreenRelation.Room)} = ?", this.ScreenId, newRoom).FirstOrDefault();
                    if (roomScreenRelation == null)
                    {
                        roomScreenRelation = new RoomScreenRelation();
                        roomScreenRelation.ScreenId = this.ScreenId;
                        roomScreenRelation.Room = newRoom;
                    }
                }
            }
            catch
            {
                // TODO:
            }
        }
    }

    [ScreenPartial_json.Rooms]
    partial class ScreenPartialRoomItem : Json, IBound<Room>
    {
        public string Id => this.Data?.GetObjectID();
    }
}
