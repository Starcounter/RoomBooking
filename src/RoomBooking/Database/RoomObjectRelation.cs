using Starcounter;

namespace RoomBooking
{
    [Database]
    public class RoomObjectRelation
    {
        public Room Room { get; set; }
        public string ObjId { get; set; }
        public bool Enabled { get; set; }
        public static void RegisterHooks()
        {
            Hook<Room>.BeforeDelete += (sender, room) =>
            {
                Db.SQL($"DELETE FROM {typeof(RoomObjectRelation)} WHERE {nameof(RoomObjectRelation.Room)} = ?", room);
            };
        }
    }
}
