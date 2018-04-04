using System;
using System.Linq;
using Starcounter;
using RoomBooking.ViewModels.Screens;

namespace RoomBooking.Handlers
{
    public class ScreenContentHandlers
    {

        public static void Register()
        {      
            Handle.GET("/RoomBooking/Content/{?}", (string objId) =>
            {
                try
                {
                    RoomObjectRelation roomObjectRelation = Db.SQL<RoomObjectRelation>($"SELECT o FROM {typeof(RoomObjectRelation)} o WHERE o.{nameof(RoomObjectRelation.ObjId)} = ?", objId).FirstOrDefault();
                    return Db.Scope(() =>
                    {
                        ContentPage mainScreenPage = Utils.AssureContentPage();
                        mainScreenPage.Data = roomObjectRelation;
                        return mainScreenPage;
                    });
                }
                catch (Exception e)
                {
                    ViewModels.ErrorMessageBox.Show(e, Utils.CONTENT_PAGE_TYPE);
                    return Utils.AssureContentPage();
                }
            });
        }
    }
}
