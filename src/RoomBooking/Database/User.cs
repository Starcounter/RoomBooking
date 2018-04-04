using Starcounter;

namespace RoomBooking
{
    [Database]
    public class User
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
