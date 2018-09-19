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
            RoomBookingEvent.RegisterHooks();

            UpdateGuiHooks.Register();

            // Handlers
            MainHandlers.Register();
        }
    }
}