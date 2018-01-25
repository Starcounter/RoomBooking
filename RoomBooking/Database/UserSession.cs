using System;
using System.Linq;
using Starcounter;

namespace RoomBooking
{
    [Database]
    public class UserSession
    {
        public User User;
        public string SessionId;
        public DateTime ExpiresAt { get; set; }

        public static void RegisterHooks()
        {
            Hook<User>.BeforeDelete += (sender, user) =>
            {
                Db.SQL($"DELETE FROM {typeof(UserSession)} WHERE {nameof(UserSession.User)} = ?", user);
            };
        }

        public static User GetSignedInUser()
        {
            if (Session.Current == null)
            {
                return null;
            }

            return Db.SQL<User>($"SELECT o.{nameof(UserSession.User)} FROM {typeof(UserSession)} o WHERE o.{nameof(UserSession.SessionId)} = ?", Session.Current?.SessionId).FirstOrDefault();
        }
    }


}
