using Starcounter;

namespace RoomBooking
{
    [Database]
    public class RoomScreenRelation
    {
        public Room Room;
        public string ScreenId;
        public bool Enabled;
        public static void RegisterHooks()
        {
            Hook<Room>.BeforeDelete += (sender, room) =>
            {
                Db.SQL($"DELETE FROM {typeof(RoomScreenRelation)} WHERE {nameof(RoomScreenRelation.Room)} = ?", room);
            };
        }
    }
}
