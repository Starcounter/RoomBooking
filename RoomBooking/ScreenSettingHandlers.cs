using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using RoomBooking.ViewModels.Partials;

namespace RoomBooking
{
    public class ScreenSettingHandlers
    {

        public static void RegisterHandlers()
        {
      
            Handle.GET("/RoomBooking/partial/screen/{?}", (string screenId) =>
            {
                User user = UserSession.GetSignedInUser();
                if (user != null)
                {
                    AssureDefaultUserRoom(user);
                    ScreenPartial screenPartial = new ScreenPartial();
                    screenPartial.Init(screenId);
                    return screenPartial;
                }
                return new Json();
            });

            Blender.MapUri("/RoomBooking/partial/screen/{?}", "screen");
        
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
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

    }
}
