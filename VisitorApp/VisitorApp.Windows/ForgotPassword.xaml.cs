using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using VisitorApp.Common;
using VisitorApp.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VisitorApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ForgotPassword : Page
    {
        VisitorAppHelper VisitorAppHelper = new VisitorAppHelper();
        RemoteService service = new RemoteService();
        string email { get; set; }
        string token { get; set; }
        string password { get; set; }
        string ipAddress { get; set; }

        public ForgotPassword()
        {
            this.InitializeComponent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HubPage));
        }

        private async void checkInternet()
        {
            List<string> info = new List<string>();
            info = await VisitorAppHelper.isInternetAvailible();

            bool internetStatus = Convert.ToBoolean(info[0]);
            string msgText = info[1];

            if (internetStatus == false)
            {
                MessageDialog msg = new MessageDialog(msgText, "Alert!");
                await msg.ShowAsync();
            }
        }

        private async void EmailToken(string StaffEmail)
        {
            try
            {
                var icp = NetworkInformation.GetInternetConnectionProfile();

                if (icp.NetworkAdapter != null)
                {
                    var hostname = NetworkInformation.GetHostNames().SingleOrDefault(hn => hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId == icp.NetworkAdapter.NetworkAdapterId);
                    ipAddress = hostname.ToString();

                    EmailTokenDataPayLoad tokenDetail = new EmailTokenDataPayLoad
                    {
                        email = StaffEmail,
                        ipAddress = ipAddress
                    };

                    ResponseMessage response = await service.StaffForgotPasswordTokenControllerTokenService(tokenDetail);
                    if (response.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        MessageDialog md = new MessageDialog("You are not registered on Visitors Manager", "Email not Found");
                        await md.ShowAsync();                      
                    }
                    else if (response.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                    {
                        //txtEmail.Text = "";
                        hideStackPanels();
                        stackSentToken.Visibility = Visibility.Visible;
                        MessageDialog md = new MessageDialog("Token Sent to Email (" + email + ")", "Check your email");
                        await md.ShowAsync();                    
                    }
                }
                else
                {
                    checkInternet();
                }
            }
            catch (Exception ex)
            {
                //
                checkInternet();
            }
        }

        public async void ConfirmToken(string theSenttoken)
        {
            try
            {
                EmailTokenDataPayLoad tokenDetail = new EmailTokenDataPayLoad
                {
                    email = email,
                    ipAddress = ipAddress,
                    sentToken = theSenttoken
                };

                ResponseMessage response = await service.ConfirmTokenControllerService(tokenDetail);
                if (response.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageDialog md = new MessageDialog("Token Does Not Exist or Expired", "Invalid Token");
                    await md.ShowAsync();
                }
                else if (response.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                {
                    //txtEmail.Text = "";
                    hideStackPanels();
                    stackNewPassword.Visibility = Visibility.Visible;
                    MessageDialog md = new MessageDialog("Enter new Password", "Confirmed!");
                    await md.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                checkInternet();
            }
        }

        public async void ChangePassword(string password)
        {
            try
            {
                UserLoginDataPayLoad userDetail = new UserLoginDataPayLoad();
                userDetail.userPassword = password;
                userDetail.username = email;

                ResponseMessage msgUpdateUser = await service.ChangeUserPasswordFromTokenControllerService(userDetail);

                if (msgUpdateUser.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                {
                    MessageDialog ms = new MessageDialog("Password Changed Successful");
                    await ms.ShowAsync();
                    this.Frame.Navigate(typeof(HubPage));
                    //txtDesiredPassword.Text = "";
                }
                else
                {
                    MessageDialog ms = new MessageDialog(msgUpdateUser.Message, "Error");
                    await ms.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                checkInternet();
            }
        }

        private async void btnEmailToken_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string StaffEmail = txtEmail.Text.Trim();
                if (string.IsNullOrEmpty(StaffEmail))
                {
                    MessageDialog msg = new MessageDialog("Enter Email", "Alert");
                    await msg.ShowAsync();
                }
                else if (!txtEmail.Text.Contains('@') || !txtEmail.Text.Contains('.'))
                {
                    MessageDialog md = new MessageDialog("Please Enter a Valid Email");
                    await md.ShowAsync();
                    return;
                }
                else
                {
                    email = StaffEmail;
                    EmailToken(StaffEmail);
                }
            }
            catch (Exception ex)
            {
                //
                checkInternet();
            }
        }

        private async void btnConfirmToken_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string theSenttoken = txtToken.Text.Trim();
                if (string.IsNullOrEmpty(theSenttoken))
                {
                    MessageDialog msg = new MessageDialog("Enter send token in your email", "Alert");
                    await msg.ShowAsync();
                }
                else
                {
                    token = theSenttoken;
                    ConfirmToken(token);
                }
            }
            catch (Exception ex)
            {
                //
                checkInternet();
            }
        }

        private async void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string password = txtDesiredPassword.Password.Trim();
                if (string.IsNullOrEmpty(password))
                {
                    MessageDialog msg = new MessageDialog("Enter New Password", "Alert");
                    await msg.ShowAsync();
                }
                else
                {
                    ChangePassword(password);
                }
            }
            catch (Exception ex)
            {
                //
                checkInternet();
            }
        }

        private void hideStackPanels()
        {
            stackEmail.Visibility = Visibility.Collapsed;
            stackSentToken.Visibility = Visibility.Collapsed;
            stackNewPassword.Visibility = Visibility.Collapsed;
        }

        private void btnResendToken_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EmailToken(email);
            }
            catch (Exception ex)
            {
                //MessageDialog msg = new MessageDialog(ex.Message);
                checkInternet();
            }
        }
    }
}
