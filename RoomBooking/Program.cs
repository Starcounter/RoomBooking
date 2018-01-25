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
            RoomScreenRelation.RegisterHooks();
            RoomBookingEvent.RegisterHooks();

            UpdateGuiHooks.Register();

            // Handlers
            ScreenSettingHandlers.RegisterHandlers();
            ScreenContentHandlers.RegisterHandlers();
            RoomHandlers.RegisterHandlers();
            RegisterHandlers();

            // Blending
            RegisterBlending();
        }

        private static void RegisterHandlers()
        {
            Handle.GET("/RoomBooking", (Request request) =>

            {
                MainPage mainPage = Utils.GetMainPage();
                try
                {

                    //User user = UserSession.GetSignedInUser();
                    //if (user == null)
                    //{
                    //    ViewModels.MessageBox.Show("Access Denied", "You need to be signed in");
                    //    return mainPage;
                    //}

                    //UserRoomRelation userRoomRelation = Program.AssureDefaultUserRoom(user);

                    //RoomsPage roomsPage = new RoomsPage();
                    //mainPage.Content = roomsPage;
                }
                catch (Exception e)
                {
                    ViewModels.ErrorMessageBox.Show(e);
                }
                return mainPage;
            });
        }

        private static void RegisterBlending()
        {
            Handle.GET("/RoomBooking/menumapping", () =>
            {
                Menu menu = new Menu();
                menu.Init();
                return menu;
            });
            Blender.MapUri("/RoomBooking/menumapping", "menu");

        }
    }
}