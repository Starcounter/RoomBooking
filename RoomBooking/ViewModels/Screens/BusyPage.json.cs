using Starcounter;
using System;

namespace RoomBooking.ViewModels.Screens
{


    partial class BusyPage : Json
    {
        public Action OnClose;


        public void Handle(Input.CloseTrigger action)
        {

            this.OnClose?.Invoke();

        }

    }
}
