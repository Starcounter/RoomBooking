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



    }
}
