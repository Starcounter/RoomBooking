using System;
using System.Linq;
using Starcounter;

namespace RoomBooking
{
    [Database]
    public class UserSession
    {
        public User User { get; set; }
        public string SessionId { get; set; }
        public DateTime ExpiresAt { get; set; }

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
            string userName = "Anonymous";
            string userEmail = "none";

            User user = Db.SQL<User>($"SELECT o FROM {typeof(RoomBooking.User)} o WHERE o.{nameof(RoomBooking.User.Username)} = ?", userName).FirstOrDefault();

            if( user == null)
            {
                Db.Transact(() =>
                {
                    user = new User() { Username = userName, Email = userEmail };
                });
            }

            return user;
        }
    }


}
