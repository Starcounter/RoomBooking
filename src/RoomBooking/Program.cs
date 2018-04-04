using System;
using Starcounter;
using RoomBooking.ViewModels;
using RoomBooking.Handlers;

namespace RoomBooking
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            // Hooks
            UserSession.RegisterHooks();
            UserRoomRelation.RegisterHooks();
            RoomObjectRelation.RegisterHooks();
            RoomBookingEvent.RegisterHooks();

            UpdateGuiHooks.Register();

            // Handlers
            ScreenSettingHandlers.Register();
            ScreenContentHandlers.Register();
            MainHandlers.Register();

            // Blending
            BlenderMapping.Register();
        }
    }
}