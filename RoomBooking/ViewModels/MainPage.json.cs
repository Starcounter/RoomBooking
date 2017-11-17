using Starcounter;
using System;
using System.Linq;

namespace RoomBooking.ViewModels
{
    partial class MainPage : Json
    {
        public Cookie Cookie { get; set; }
        public string SignInRedirectUrl;
    }
}
