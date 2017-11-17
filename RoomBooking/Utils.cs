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
        static private Object thisLock = new Object();

        internal static void PushChanges()
        {
            lock (Utils.thisLock)
            {
                Session.ForAll((s, sessionId) =>
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
                session.Store[nameof(MainPage)] = mainPage;
            }

            return mainPage;
        }

        internal static ScreenContentPage AssureScreenContentPage()
        {

            var session = Session.Ensure();

            ScreenContentPage mainPage = session.Store[nameof(ScreenContentPage)] as ScreenContentPage;

            if (mainPage == null)
            {
                mainPage = new ScreenContentPage();
                session.Store[nameof(ScreenContentPage)] = mainPage;
            }

            return mainPage;
        }
    }
}
