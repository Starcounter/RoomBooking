using Starcounter;
using System;

namespace RoomBooking.ViewModels
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
        public static void Show(Exception e, string page_type, Action callback = null)
        {
            if (e != null)
            {
                ErrorMessageBox.Show(null, e.Message, (e.StackTrace == null) ? null : e.StackTrace.ToString(), e.HelpLink, (ushort)System.Net.HttpStatusCode.InternalServerError, page_type, callback);
            }
            else
            {
                ErrorMessageBox.Show("Unknown error, Exception object is null", page_type);
            }
        }

        /// <summary>
        /// Show Error Message
        /// </summary>
        /// <remarks>
        /// This should only be used for system error message. Not for user behavior errors.
        /// </remarks>
        /// <param name="e"></param>
        public static void Show(string message, string page_type, Action callback = null)
        {
            ErrorMessageBox.Show(null, message, null, null, (ushort)System.Net.HttpStatusCode.InternalServerError, page_type, callback);
        }

        /// <summary>
        /// Show Error Message
        /// </summary>
        /// <remarks>
        /// This should only be used for system error message. Not for user behavior errors.
        /// </remarks>
        /// <param name="response"></param>
        public static void Show(Response response, string page_type, Action callback = null)
        {
            if (response != null)
            {
                ErrorMessageBox.Show(null, "Error:" + ((System.Net.HttpStatusCode)response.StatusCode).ToString(), null, null, response.StatusCode, page_type, callback);
            }
            else
            {
                ErrorMessageBox.Show("Unknown error, Response object is null", page_type);
            }
        }

        /// <summary>
        /// Show Error Message
        /// </summary>
        /// <remarks>
        /// This should only be used for system error message. Not for user behavior errors.
        /// </remarks>
        /// <param name="e"></param>
        public static void Show(string title, string message, string page_type, Action callback = null)
        {
            ErrorMessageBox.Show(title, message, null, null, (ushort)System.Net.HttpStatusCode.InternalServerError, page_type, callback);
        }

        public static void Show(string title, string text, string stackTrace, string helpLink, ushort statusCode, string page_type, Action callback = null)
        {
            if (page_type == Utils.CONTENT_PAGE_TYPE)
            {
                ShowContent(title, text, stackTrace, helpLink, statusCode, page_type, callback);
            }
            else
            {
                ShowMain(title, text, stackTrace, helpLink, statusCode, page_type, callback);
            }
        }

        public static void ShowMain(string title, string text, string stackTrace, string helpLink, ushort statusCode, string page_type, Action callback = null)
        {
            MainPage holderPage = Utils.GetMainPage();
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
            messageBox.Type = Utils.MAIN_PAGE_TYPE;
        }

        public static void ShowContent(string title, string text, string stackTrace, string helpLink, ushort statusCode, string page_type, Action callback = null)
        {
            Screens.ContentPage holderPage = Utils.AssureContentPage();
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
            messageBox.Type = Utils.CONTENT_PAGE_TYPE;
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
            this.Type = null;
            this.StackTrace = null;
            this.Helplink = null;
            this.StatusCode = 0;
        }
    }
}
