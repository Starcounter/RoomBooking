using CalendarSync.Database;
using Screens.Common;
using Starcounter;
using System.Collections.Generic;

namespace RoomBooking.ViewModels
{
    partial class RoomsPage : Json
    {
        public IEnumerable<SyncedCalendar> Rooms => Db.SQL<SyncedCalendar>("SELECT o.Room FROM RoomBooking.UserRoomRelation o WHERE o.User = ? ORDER BY o.Room.Name", UserSession.GetSignedInUser());
    }

    [RoomsPage_json.Rooms]
    partial class RoomsPageRoomItem : Json, IBound<SyncedCalendar>
    {

        public string Url => string.Format("/roomBooking/rooms/{0}", this.Data?.GetObjectID());



    }
}
