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
            string firstName = "Anonymouse";
            string lastName = "Anonymouse";

            User user = Db.SQL<User>($"SELECT o FROM {typeof(RoomBooking.User)} o WHERE o.{nameof(RoomBooking.User.FirstName)} = ? AND o.{nameof(RoomBooking.User.LastName)} = ?", firstName, lastName).FirstOrDefault();

            if( user == null)
            {
                Db.Transact(() =>
                {
                    user = new User() { FirstName = firstName, LastName = lastName };
                });
            }

            return user;
        }
    }


}
