using Starcounter;

namespace RoomBooking
{
    [Database]
    public class UserRoomRelation
    {
        public User User { get; set; }
        public Room Room { get; set; }
        public static void RegisterHooks()
        {
            Hook<Room>.BeforeDelete += (sender, room) =>
            {
                Db.SQL($"DELETE FROM {typeof(UserRoomRelation)} WHERE {nameof(UserRoomRelation.Room)} = ?", room);
            };

            Hook<User>.BeforeDelete += (sender, user) =>
            {
                Db.SQL($"DELETE FROM {typeof(UserRoomRelation)} WHERE {nameof(UserRoomRelation.User)} = ?", user);
            };

        }
    }
}
