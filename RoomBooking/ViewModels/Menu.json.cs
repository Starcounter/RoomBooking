using Starcounter;

namespace RoomBooking.ViewModels
{
    partial class Menu : Json
    {

        public void Init()
        {
            //var item = this.Items.Add();
            //item.Name = "RoomBooking";
            //item.Url = "/RoomBooking";
            var item2 = this.Items.Add();
            item2.Name = "Rooms";
            item2.Url = "/RoomBooking/Rooms";

        }

    }
}
