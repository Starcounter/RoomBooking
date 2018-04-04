using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using RoomBooking.ViewModels.Partials;
using System.Threading;
using RoomBooking.ViewModels.Screens;
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
    }
}
