using Screens.Common;
using Starcounter;

namespace RoomBooking.ViewModels
{
    partial class ContentPage : Json
    {
        public void Init(Screen screen)
        {
            this.Content = Self.GET("/RoomBooking/screencontentmapping/" + screen.PluginName);
        }

    }
}
