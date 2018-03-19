using Starcounter;

namespace RoomBooking.ViewModels
{
    partial class Menu : Json
    {

        public void Init()
        {
            var item = this.Items.Add();
            item.Name = "Rooms";
            item.Url = "/RoomBooking/Rooms";
        }

    }
}
