using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using System.Threading;
using RoomBooking.ViewModels;

namespace RoomBooking
{
    public class Utils
    {
        static public string MAIN_PAGE_TYPE = "MainPage";
        static public string CONTENT_PAGE_TYPE = "ContentPage";
        static private Object thisLock = new Object();

        internal static void PushChanges()
        {
            lock (Utils.thisLock)
            {
                Session.RunTaskForAll((s, sessionId) =>
                {
                    s.CalculatePatchAndPushOnWebSocket();
                });
            }
        }

        internal static MainPage GetMainPage()
        {
            var session = Session.Ensure();
            
            MainPage mainPage = session.Store[nameof(MainPage)] as MainPage;

            if (mainPage == null)
            {
                mainPage = new MainPage();
                
                // Menu blending point
                mainPage.Menu = Self.GET("/RoomBooking/menu");

                session.Store[nameof(MainPage)] = mainPage;
            }

            return mainPage;
        }

        internal static ContentPage AssureContentPage()
        {
            var session = Session.Ensure();

            ContentPage mainPage = session.Store[nameof(ContentPage)] as ContentPage;

            if (mainPage == null)
            {
                mainPage = new ContentPage();
                session.Store[nameof(ContentPage)] = mainPage;
            }

            return mainPage;
        }

        static UserRoomRelation AssureDefaultUserRoom(User user)
        {
            UserRoomRelation userRoomRelation = Db.SQL<UserRoomRelation>($"SELECT o FROM {typeof(UserRoomRelation)} o WHERE o.{nameof(UserRoomRelation.User)} = ?", user).FirstOrDefault();
            if (userRoomRelation == null)
            {
                Db.Transact(() =>
                {
                    userRoomRelation = new UserRoomRelation();
                    userRoomRelation.User = user;
                    userRoomRelation.Room = new Room() { Name = "Default", Description = "This is your default room" };
                });
            }

            return userRoomRelation;
        }

        static public IEnumerable<Room> GetAllRooms()
        {
            return Db.SQL<Room>($"SELECT r FROM {nameof(Room)} r");
        }
    }
}
