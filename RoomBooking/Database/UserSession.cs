using System.Linq;
using Starcounter;

namespace RoomBooking
{
    [Database]
    public class UserSession
    {
        public User User;
        public string SessionId;

        public static void RegisterHooks()
        {
            Hook<User>.BeforeDelete += (sender, user) =>
            {
                Db.SQL($"DELETE FROM {typeof(UserSession)} WHERE {nameof(UserSession.User)} = ?", user);
            };
        }

        /// <summary>
        /// TODO:
        /// </summary>
        /// <returns></returns>
        public static User GetSignedInUser()
        {
            User user = Db.SQL<User>($"SELECT o FROM {typeof(RoomBooking.User)} o").FirstOrDefault();

            if( user == null)
            {
                Db.Transact(() =>
                {
                    user = new User() { FirstName = "Anonymouse", LastName = "Anonymouse" };
                });
            }

            return user;
            //return Db.SQL<User>($"SELECT o.{nameof(UserSession.User)} FROM {typeof(RoomBooking.UserSession)} o WHERE o.{nameof(UserSession.SessionId)} = ?", Starcounter.Session.Current.SessionId).FirstOrDefault();
        }
    }


}
