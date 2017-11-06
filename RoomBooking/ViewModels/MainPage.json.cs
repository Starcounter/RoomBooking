using Starcounter;
using System;
using Screens.Common;
using System.Linq;

namespace RoomBooking.ViewModels
{
    partial class MainPage : Json
    {
        public Cookie Cookie { get; set; }


        public string SignInRedirectUrl;

    
        public void Handle(Input.TestErrorMessageBoxTrigger action)
        {
            ErrorMessageBox.Show("Message");
        }

        public void Handle(Input.TestMessageBoxTrigger action)
        {

            MessageBoxButton deleteButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Yes, Text = "Delete", CssClass = "btn btn-sm btn-danger" };
            MessageBoxButton cancelButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Cancel, Text = "Cancel" };

            MessageBox.Show("Delete Version", "This Version will be deleted.", cancelButton, deleteButton, (result) =>
            {

                if (result == MessageBox.MessageBoxResult.Yes)
                {
                    //Db.Transact(() => {
                    //    this.Data.Delete();
                    //});
                }
            });

        }

    }


    [MainPage_json.User]
    partial class MainPageUser : Json, IBound<User>
    {


        public bool IsSignedInFlag {

            get {
                return (this.Data != null);
            }

        }


    }


    [MainPage_json.GoogleUser]
    partial class GoogleUserItem : Json
    {
        public void Handle(Input.SignedIn action)
        {

            MainPage mainPage = Program.GetMainPage();

            if (action.Value)
            {
                // google user signed in

                User user = Db.SQL<User>("SELECT o FROM Screens.Common.User o WHERE o.GoogleId = ?", this.Id).FirstOrDefault();

                if (user == null)
                {
                    // Create user
                    Db.Transact(() =>
                    {
                        user = new User();
                        user.GoogleId = this.Id;
                        user.FirstName = this.FirstName;
                        user.LastName = this.LastName;
                        user.Email = this.Email;
                    });
                }

                // Update values
                if (!string.Equals(user.Email, this.Email, StringComparison.CurrentCultureIgnoreCase))
                {
                    Db.Transact(() =>
                    {
                        user.Email = this.Email;
                    });
                }
                if (!string.Equals(user.FirstName, this.FirstName, StringComparison.CurrentCultureIgnoreCase))
                {
                    Db.Transact(() =>
                    {
                        user.FirstName = this.FirstName;
                    });
                }
                if (!string.Equals(user.LastName, this.LastName, StringComparison.CurrentCultureIgnoreCase))
                {
                    Db.Transact(() =>
                    {
                        user.LastName = this.LastName;
                    });
                }

                mainPage.User.Data = user;

                if (!string.IsNullOrEmpty(mainPage.SignInRedirectUrl))
                {
                    mainPage.RedirectUrl = mainPage.SignInRedirectUrl;
                }
            }
            else
            {
                // signed out or was never signed in
                mainPage.User.Data = null;
            }


        }
    }

}
