using System.Linq;
using Starcounter;
using RoomBooking.ViewModels.Partials;

namespace RoomBooking.Handlers
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
