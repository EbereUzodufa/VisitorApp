using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VisitorApp.Dashboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class test : Page
    {
        public test()
        {
            this.InitializeComponent();
        }

        private static void SendEmail (string emailBody)
        {
            //MailMessage 
        }

        //private async Task ComposeEmail(Windows.ApplicationModel.Contacts.Contact recipient, string subject, string messageBody)
        //{
        //    var emailMessage = new Windows.ApplicationModel.Email.EmailMessage();
        //    emailMessage.Body = messageBody;

        //    var email = recipient.Emails.FirstOrDefault<Windows.ApplicationModel.Contacts.ContactEmail>();
        //    if (email != null)
        //    {
        //        var emailRecipient = new Windows.ApplicationModel.Email.EmailRecipient(email.Address);
        //        emailMessage.To.Add(emailRecipient);
        //        emailMessage.Subject = subject;
        //    }

        //    await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);
        //}
    }
}
