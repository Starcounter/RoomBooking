using Starcounter;
using System;
using System.Collections.Generic;

namespace RoomBooking.ViewModels.Screens
{


    partial class MessageBox : Json
    {

        private Action<MessageBoxResult> CallBack = null;

        public static void Show(string title, string text, MessageBoxButton button1, MessageBoxButton button2, Action<MessageBoxResult> callback = null)
        {

            List<MessageBoxButton> buttons = new List<MessageBoxButton>();
            if (button1 != null)
            {
                buttons.Add(button1);
            }
            if (button2 != null)
            {
                buttons.Add(button2);
            }
            Show(title, text, buttons, callback);
        }

        public static void Show(string title, string text, Action<MessageBoxResult> callback = null)
        {

            MessageBoxButton btn = new MessageBoxButton();
            btn.ID = (long)MessageBoxResult.OK;
            btn.Text = "Ok";

            Show(title, text, btn, null, callback);
        }

        public static void Show(string title, string text, IList<MessageBoxButton> buttons, Action<MessageBoxResult> callback = null)
        {

            ScreenContentPage holderPage = Program.AssureScreenContentPage();
            if (holderPage == null)
            {
                // TODO: Show error
                return;
            }

            MessageBox messageBox = holderPage.MessageBox;

            if (messageBox.CallBack != null)
            {
                throw new NotImplementedException("Nested Messageboxes is not supported");
            }

            messageBox.Reset();
            messageBox.CallBack = callback;
            messageBox.Title = title;
            messageBox.Text = text;

            bool hasDefault = false;

            foreach (MessageBoxButton btn in buttons)
            {
                if (btn.Default == true)
                {
                    hasDefault = true;
                }

                messageBox.Buttons.Add(btn);
            }

            if (hasDefault == false)
            {
                MessageBoxButton lastBtn = buttons[buttons.Count - 1];
                lastBtn.Default = true;
            }

            // Fix style
            foreach (MessageBoxButton btn in buttons)
            {

                if (!string.IsNullOrEmpty(btn.CssClass))
                {
                    continue;
                }

                if (btn.Default == true)
                {
                    btn.CssClass = "btn btn-sm btn-primary";
                }
                else
                {
                    btn.CssClass = "btn btn-sm btn-default";
                }

            }
            messageBox.Visible = true;
        }

        void Handle(Input.Close action)
        {
            try
            {
                this.HideWindow();
                this.InvokeCallback(MessageBoxResult.None);
            }
            catch (Exception e)
            {
                ErrorMessageBox.Show(e);
            }

        }

        public void OnButtonClick(MessageBoxButton button)
        {
            this.HideWindow();
            this.InvokeCallback((MessageBoxResult)button.ID);
        }

        private void InvokeCallback(MessageBoxResult result)
        {

            if (this.CallBack != null)
            {
                var callback = this.CallBack;
                this.CallBack = null;
                callback(result);
            }
        }

        private void HideWindow()
        {
            this.Visible = false;

            if (Session.Current != null)
            {
                Session.Current.CalculatePatchAndPushOnWebSocket();
            }
        }

        private void Reset()
        {
            this.Visible = false;
            this.Buttons.Clear();
            this.Title = null;
            this.Text = null;
        }

        public enum MessageBoxResult
        {
            // Summary:
            //     The message box returns no result.
            None = 0,
            //
            // Summary:
            //     The result value of the message box is OK.
            OK = 1,
            //
            // Summary:
            //     The result value of the message box is Cancel.
            Cancel = 2,
            //
            // Summary:
            //     The result value of the message box is Yes.
            Yes = 6,
            //
            // Summary:
            //     The result value of the message box is No.
            No = 7,
        }


    }

    [MessageBox_json.Buttons]
    partial class MessageBoxButton : Json
    {

        void Handle(Input.Click action)
        {
            try
            {
                MessageBox messageBox = this.Parent.Parent as MessageBox;
                messageBox.OnButtonClick(this);
            }
            catch (Exception e)
            {
                ErrorMessageBox.Show(e);
            }

        }
    }

}
