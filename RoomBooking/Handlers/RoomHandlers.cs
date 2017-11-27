using System.Linq;
using Starcounter;
using RoomBooking.ViewModels;

namespace RoomBooking.Handlers
{
    public class RoomHandlers
    {

        public static void RegisterHandlers()
        {
            Handle.GET("/roomBooking/rooms", (Request request) =>
            {
                MainPage mainPage = Utils.GetMainPage();
                User user = UserSession.GetSignedInUser();
                if (user == null)
                {
                    ViewModels.MessageBox.Show("Access Denied", "You must be signed in");
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
                    ViewModels.MessageBox.Show("Access Denied", "You must be signed in");
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
                    ViewModels.MessageBox.Show("Access Denied", "You must be signed in");
                    return mainPage;
                }

                Room room = Db.SQL<Room>($"SELECT o FROM {typeof(Room)} o WHERE o.ObjectID=?", id).FirstOrDefault();
                if (room == null)
                {
                    ViewModels.MessageBox.Show("Not found", "Room not found"); // TODO: Show page error instead of popup
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
