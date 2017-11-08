using Starcounter;
using System;

namespace RoomBooking.ViewModels.Screens
{

    partial class ErrorMessageBox : Json
    {

        private Action CallBack = null;

        /// <summary>
        /// Show Error Message
        /// </summary>
        /// <remarks>
        /// This should only be used for system error message. Not for user behavior errors.
        /// </remarks>
        /// <param name="e"></param>
        public static void Show(Exception e, Action callback = null)
        {

            if (e != null)
            {
                ErrorMessageBox.Show(null, e.Message, (e.StackTrace == null) ? null : e.StackTrace.ToString(), e.HelpLink, (ushort)System.Net.HttpStatusCode.InternalServerError, callback);
            }
            else
            {
                ErrorMessageBox.Show("Unknown error, Exception object is null");
            }

        }

        /// <summary>
        /// Show Error Message
        /// </summary>
        /// <remarks>
        /// This should only be used for system error message. Not for user behavior errors.
        /// </remarks>
        /// <param name="e"></param>
        public static void Show(string message, Action callback = null)
        {

            ErrorMessageBox.Show(null, message, null, null, (ushort)System.Net.HttpStatusCode.InternalServerError, callback);

        }

        /// <summary>
        /// Show Error Message
        /// </summary>
        /// <remarks>
        /// This should only be used for system error message. Not for user behavior errors.
        /// </remarks>
        /// <param name="response"></param>
        public static void Show(Response response, Action callback = null)
        {

            if (response != null)
            {
                ErrorMessageBox.Show(null, "Error:" + ((System.Net.HttpStatusCode)response.StatusCode).ToString(), null, null, response.StatusCode, callback);
            }
            else
            {
                ErrorMessageBox.Show("Unknown error, Response object is null");
            }
        }

        /// <summary>
        /// Show Error Message
        /// </summary>
        /// <remarks>
        /// This should only be used for system error message. Not for user behavior errors.
        /// </remarks>
        /// <param name="e"></param>
        public static void Show(string title, string message, Action callback = null)
        {

            ErrorMessageBox.Show(title, message, null, null, (ushort)System.Net.HttpStatusCode.InternalServerError, callback);
        }

        public static void Show(string title, string text, string stackTrace, string helpLink, ushort statusCode, Action callback = null)
        {

            ScreenContentPage holderPage = Program.AssureScreenContentPage();
            if (holderPage == null)
            {
                // TODO: Show error
                return;
            }

            ErrorMessageBox messageBox = holderPage.ErrorMessage;

            if (messageBox.CallBack != null)
            {
                throw new NotImplementedException("Nested ErrorMessageboxes is not supported");
            }

            messageBox.Reset();

            messageBox.Title = title ?? "Opps! Something went wrong";
            messageBox.Text = text;
            messageBox.StackTrace = stackTrace;
            messageBox.Helplink = helpLink;
            messageBox.StatusCode = statusCode;
            messageBox.CallBack = callback;
            messageBox.Visible = true;
        }


        void Handle(Input.Close action)
        {
            this.HideWindow();
            this.InvokeCallback();
        }


        private void InvokeCallback()
        {

            if (this.CallBack != null)
            {
                var callback = this.CallBack;
                this.CallBack = null;
                callback();
            }
        }

        private void HideWindow()
        {
            this.Visible = false;
        }

        private void Reset()
        {
            this.Visible = false;
            this.Title = null;
            this.Text = null;
            this.StackTrace = null;
            this.Helplink = null;
            this.StatusCode = 0;
        }

    }

}
