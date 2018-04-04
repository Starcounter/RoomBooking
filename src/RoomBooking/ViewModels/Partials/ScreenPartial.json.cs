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

            RoomObjectRelation RoomObjectRelation = Db.SQL<RoomObjectRelation>($"SELECT o FROM {typeof(RoomObjectRelation)} o WHERE o.{nameof(RoomObjectRelation.ObjId)} = ?", this.ScreenId).FirstOrDefault();
            if (RoomObjectRelation != null)
            {
                this.SelectedRoomId = RoomObjectRelation.Room.GetObjectID();
                this.Enable = RoomObjectRelation.Enabled;
            }
        }

        public void Handle(Input.Enable action)
        {
            RoomObjectRelation RoomObjectRelation = Db.SQL<RoomObjectRelation>($"SELECT o FROM {typeof(RoomObjectRelation)} o WHERE o.{nameof(RoomObjectRelation.ObjId)} = ?", this.ScreenId).FirstOrDefault();

            if (RoomObjectRelation == null)
            {

                Room room = this.Rooms.FirstOrDefault();
                if (room != null)
                {
                    Db.Transact(() =>
                    {
                        RoomObjectRelation = new RoomObjectRelation();
                        RoomObjectRelation.ObjId = this.ScreenId;
                        RoomObjectRelation.Room = room;
                        this.SelectedRoomId = RoomObjectRelation.Room.GetObjectID();
                    });
                }
            }

            if (RoomObjectRelation != null)
            {
                Db.Transact(() =>
                {
                    RoomObjectRelation.Enabled = action.Value;
                });
            }
        }

        public void Handle(Input.SelectedRoomId action)
        {

            try
            {
                if (this.SelectedRoomId != "")
                {
                    Room oldRoom = Db.FromId(this.SelectedRoomId) as Room;

                    if (oldRoom != null)
                    {
                        // Remove old room relation
                        Db.Transact(() =>
                        {
                            RoomObjectRelation oldRoomScreenRelation = Db.SQL<RoomObjectRelation>($"SELECT o FROM {typeof(RoomObjectRelation)} o WHERE o.{nameof(RoomObjectRelation.ObjId)} = ? AND o.{nameof(RoomObjectRelation.Room)} = ?", this.ScreenId, oldRoom).FirstOrDefault();
                            oldRoomScreenRelation.Delete();
                        });
                    }
                }
            }
            catch
            {
                // TODO:
            }
            try
            {
                if (action.Value != "")
                {
                    Room newRoom = Db.FromId(action.Value) as Room;

                    if (newRoom != null)
                    {
                        // Create old room relation
                        RoomObjectRelation RoomObjectRelation = Db.SQL<RoomObjectRelation>($"SELECT o FROM {typeof(RoomObjectRelation)} o WHERE o.{nameof(RoomObjectRelation.ObjId)}  = ? AND o.{nameof(RoomObjectRelation.Room)} = ?", this.ScreenId, newRoom).FirstOrDefault();
                        if (RoomObjectRelation == null)
                        {
                            Db.Transact(() =>
                            {
                                RoomObjectRelation = new RoomObjectRelation();
                                RoomObjectRelation.ObjId = this.ScreenId;
                                RoomObjectRelation.Room = newRoom;
                            });
                        }
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
