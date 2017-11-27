using System;
using System.Linq;
using Starcounter;
using RoomBooking.ViewModels.Screens;

namespace RoomBooking.Handlers
{
    public class ScreenContentHandlers
    {

        public static void RegisterHandlers()
        {
      
            Handle.GET("/RoomBooking/screenContent/{?}", (Func<string, Response>)((string screenId) =>
            {
                try
                {
                    RoomScreenRelation roomScreenRelation = Db.SQL<RoomScreenRelation>($"SELECT o FROM {typeof(RoomScreenRelation)} o WHERE o.{nameof(RoomScreenRelation.ScreenId)} = ?", screenId).FirstOrDefault();
                    return Db.Scope(() =>
                    {
                        ScreenContentPage mainScreenPage = Utils.AssureScreenContentPage();
                        mainScreenPage.Data = roomScreenRelation;
                        return mainScreenPage;
                    });
                }
                catch (Exception e)
                {
                    ViewModels.Screens.ErrorMessageBox.Show(e);
                    return Utils.AssureScreenContentPage();
                }
            }));

            Blender.MapUri("/RoomBooking/screenContent/{?}", "screenContent");
        }

    }
}
