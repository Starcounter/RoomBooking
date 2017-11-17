using Starcounter;
using System.Collections.Generic;

namespace RoomBooking.ViewModels
{
    partial class RoomsPage : Json
    {
        public IEnumerable<Room> Rooms => Db.SQL<Room>($"SELECT o.{nameof(UserRoomRelation.Room)} FROM {typeof(UserRoomRelation)} o WHERE o.{nameof(UserRoomRelation.User)} = ? ORDER BY o.{nameof(UserRoomRelation.Room)}.{nameof(Room.Name)}", UserSession.GetSignedInUser());
    }

    [RoomsPage_json.Rooms]
    partial class RoomsPageRoomItem : Json, IBound<Room>
    {

        public string Url => string.Format("/roomBooking/rooms/{0}", this.Data?.GetObjectID());



    }
}
