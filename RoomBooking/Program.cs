// Google Client ID: 525016583199-77s56n08s4uuoir2oppc8gs1biv5t6q9.apps.googleusercontent.com
// Google client secret: hTMyupyDfY8LFFCVnghhBPzK

//public IEnumerable<Order> Orders => Db.SQL<Order>($"SELECT so FROM {nameof(Flexovital)}.\"{nameof(Flexovital.Order)}\" so WHERE so.Subscription.Customer = ?", this);

using System;
using Starcounter;
using RoomBooking.ViewModels;
using System.Linq;
using RoomBooking.ViewModels.Screens;
using System.Collections.Specialized;
using RoomBooking.ViewModels.Partials;
using System.Threading;

namespace RoomBooking
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            UpdateGuiHooks.Register();

            UserSession.RegisterHooks();
            UserRoomRelation.RegisterHooks();
            RoomScreenRelation.RegisterHooks();
            RoomBookingEvent.RegisterHooks();

          

        


            ScreenSettingHandlers.RegisterHandlers();
            ScreenContentHandlers.RegisterHandlers();
            RoomHandlers.RegisterHandlers();


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

            Handle.GET("/RoomBooking/menumapping", () => {

                Menu menu = new Menu();
                menu.Init();
                return menu;
            });
            Blender.MapUri("/RoomBooking/menumapping", "menu");


        }
    }
}