// Google Client ID: 525016583199-77s56n08s4uuoir2oppc8gs1biv5t6q9.apps.googleusercontent.com
// Google client secret: hTMyupyDfY8LFFCVnghhBPzK

using System;
using Starcounter;
using RoomBooking.ViewModels;
using System.Linq;
using RoomBooking.ViewModels.Screens;
using System.Collections.Specialized;
using Screens.Common;
using RoomBooking.ViewModels.Partials;

namespace RoomBooking
{
    class Program
    {
        static void Main()
        {

            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Screens.Common.Utils.RegisterHooks();

            Room.RegisterHooks();
            RoomBookingEvent.RegisterHooks();
            UserRoomRelation.RegisterHooks();


            Handle.GET("/RoomBooking", (Request request) =>
            {

                MainPage mainPage = GetMainPage();

                User user = UserSession.GetSignedInUser();
                if (user == null)
                {
                    MessageBox.Show("Access Denied", "You need to be signed in");
                    return mainPage;
                }

                Room room = Program.AssureDefaultUserRoom(user);
                MainContentPage mainContentPage = new MainContentPage();
                mainContentPage.Init(room, room.TimeZoneInfo);// TODO: Fix args
                mainPage.Content = mainContentPage;

                return mainPage;
            });

            Handle.GET("/RoomBooking/screenContent/{?}", (string screenId) =>
            {

                Screen screen = Db.FromId(screenId) as Screen;

                RoomScreenRelation roomScreenRelation = Db.SQL<RoomScreenRelation>("SELECT o FROM RoomBooking.RoomScreenRelation o WHERE o.Screen=?", screen).First;

                if (roomScreenRelation != null && roomScreenRelation.Enabled)
                {
                    MainContentPage mainContentPage = new MainContentPage();
                    mainContentPage.Init(roomScreenRelation.Room, roomScreenRelation.Room.TimeZoneInfo);// TODO: Fix args
                    return mainContentPage;
                }
                else
                {
                    // No releation = nothing to show
                    return new Json();
                }
            });

            Blender.MapUri("/RoomBooking/screenContent/{?}", "screenContent");

            Handle.GET("/RoomBooking/partial/screen/{?}", (string screenId) =>
            {
                Screen screen = Db.FromId(screenId) as Screen;

                if (screen != null)
                {
                    User user = UserSession.GetSignedInUser();
                    if (user != null)
                    {
                        Program.AssureDefaultUserRoom(user);
                        ScreenPartial screenPartial = new ScreenPartial();
                        screenPartial.Data = screen;
                        return screenPartial;
                    }
                }
                return new Json();
            });

            Blender.MapUri("/RoomBooking/partial/screen/{?}", "screen");

            #region Room

            Handle.GET("/roomBooking/rooms", (Request request) =>
            {

                MainPage mainPage = GetMainPage();
                User user = UserSession.GetSignedInUser();
                if (user == null)
                {
                    MessageBox.Show("Access Denied", "You must be signed in");
                    return mainPage;
                }

                RoomsPage roomsPage = new RoomsPage();
                mainPage.Content = roomsPage;
                return mainPage;
            });

            Handle.GET("/roomBooking/addroom", (Request request) =>
            {

                MainPage mainPage = GetMainPage();
                User user = UserSession.GetSignedInUser();

                if (user == null)
                {
                    MessageBox.Show("Access Denied", "You must be signed in");
                    return mainPage;
                }

                return Db.Scope<MainPage>(() =>
                {
                    RoomPage roomPage = new RoomPage();
                    mainPage.Content = roomPage;
                    roomPage.Data = new Room();

                    UserRoomRelation userRoomRelation = new UserRoomRelation();
                    userRoomRelation.Room = roomPage.Data;
                    userRoomRelation.User = user;

                    return mainPage;
                });
            });

            Handle.GET("/roomBooking/rooms/{?}", (string id, Request request) =>
            {

                MainPage mainPage = GetMainPage();

                User user = UserSession.GetSignedInUser();
                if (user == null)
                {
                    MessageBox.Show("Access Denied", "You must be signed in");
                    return mainPage;
                }


                Room room = Db.SQL<Room>("SELECT o FROM RoomBooking.Room o WHERE o.ObjectID=?", id).First;
                if (room == null)
                {
                    MessageBox.Show("Not found", "Room not found"); // TODO: Show page error instead of popup
                    mainPage.Content = new RoomsPage();
                    return mainPage;
                }

                return Db.Scope<MainPage>(() =>
                {
                    RoomPage roomPage = new RoomPage();
                    roomPage.Data = room;
                    mainPage.Content = roomPage;
                    return mainPage;
                });

            });

            #endregion
        }


        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public static Room AssureDefaultUserRoom(User user)
        {

            Room room = Db.SQL<Room>("SELECT o.Room FROM RoomBooking.UserRoomRelation o WHERE o.User = ?", user).First;
            if (room == null)
            {
                Db.Transact(() =>
                {
                    room = new Room() { Name = "Default", Description = "This is your default room" };
                    UserRoomRelation userRoomRelation = new UserRoomRelation();
                    userRoomRelation.User = user;
                    userRoomRelation.Room = room;
                });
            }

            return room;
        }


        public static MainPage GetMainPage()
        {
            var session = Session.Ensure();

            MainPage mainPage = session.Store[nameof(MainPage)] as MainPage;

            if (mainPage == null)
            {
                mainPage = new MainPage();
                session.Store[nameof(MainPage)] = mainPage;
            }

            return mainPage;
        }

        public static MainContentPage GetMainContentPage()
        {

            ContentPage contentPage = GetMainPage().Content as ContentPage;
            if (contentPage != null && contentPage.Content is MainContentPage)
            {
                return contentPage.Content as MainContentPage;
            }

            return null;

        }

    }
}