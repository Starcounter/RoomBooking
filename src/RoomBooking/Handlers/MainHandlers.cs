using System.Linq;
using System;
using Starcounter;
using RoomBooking.ViewModels;

namespace RoomBooking.Handlers
{
    public class MainHandlers
    {
        public static void Register()
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
                    ViewModels.ErrorMessageBox.Show(e, Utils.MAIN_PAGE_TYPE);
                }
                return mainPage;
            });

            Handle.GET("/RoomBooking/menu", () =>
            {
                Menu menu = new Menu();
                menu.Init();
                return menu;
            });

            Handle.GET("/roomBooking/rooms", (Request request) =>
            {
                MainPage mainPage = Utils.GetMainPage();
                User user = UserSession.GetSignedInUser();
                if (user == null)
                {
                    ViewModels.MessageBox.Show("Access Denied", "You must be signed in", Utils.MAIN_PAGE_TYPE);
                    return mainPage;
                }

                RoomsPage roomsPage = new RoomsPage();
                mainPage.Content = roomsPage;
                return mainPage;
            });

            Handle.GET("/roomBooking/addroom", (Request request) =>
            {
                MainPage mainPage = Utils.GetMainPage();
                User user = UserSession.GetSignedInUser();
                if (user == null)
                {
                    ViewModels.MessageBox.Show("Access Denied", "You must be signed in", Utils.MAIN_PAGE_TYPE);
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
                MainPage mainPage = Utils.GetMainPage();
                User user = UserSession.GetSignedInUser();
                if (user == null)
                {
                    ViewModels.MessageBox.Show("Access Denied", "You must be signed in", Utils.MAIN_PAGE_TYPE);
                    return mainPage;
                }

                Room room = Db.SQL<Room>($"SELECT o FROM {typeof(Room)} o WHERE o.ObjectID=?", id).FirstOrDefault();
                if (room == null)
                {
                    ViewModels.MessageBox.Show("Not found", "Room not found", Utils.MAIN_PAGE_TYPE); // TODO: Show page error instead of popup
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
        }
    }
}
