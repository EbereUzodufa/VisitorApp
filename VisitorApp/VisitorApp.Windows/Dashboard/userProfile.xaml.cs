using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VisitorApp.Common;
using VisitorApp.Dashboard.Admin;
using VisitorApp.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class userProfile : Page
    {
        AccountDetails _activePage = new AccountDetails();
        GetDataFromDB GetDataFromDB = new GetDataFromDB();
        RemoteService service = new RemoteService();
        UserProcess VisitorAppUserProcess = new UserProcess();
        VisitorAppHelper VisitorAppHelper = new VisitorAppHelper();
        int CompId;

        public userProfile()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _activePage = (AccountDetails)e.Parameter;
            if (_activePage != null)
            {
                txbUserCompanyName.Text = (_activePage.CompanyName).ToUpper();
                txbUsername.Text = (_activePage.UserStaffName).ToUpper();
                imgCompanyLogo.Source = _activePage.CompanyLogo;
                imgUser.Source = _activePage.UserStaffPhoto;
                CompId = _activePage.CompanyId;
            }

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (_activePage.UserStaffRole.ToUpper() == StaffRoles.Staff.ToString().ToUpper())
            {
                this.Frame.Navigate(typeof(Appointment), _activePage);
            }
            else
            {
                this.Frame.Navigate(typeof(FrontDeskHome), _activePage);
            }
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtNewPassword.Password.Trim()))
                {
                    MessageDialog msg = new MessageDialog("Enter New Password", "Hello");
                    await msg.ShowAsync();
                    return;
                }
                else if (string.IsNullOrEmpty(txtOldPassword.Password.Trim()))
                {
                    MessageDialog msg = new MessageDialog("Enter Old Password", "Hello");
                    await msg.ShowAsync();
                    return;
                }
                else if (txtNewPassword.Password == txtOldPassword.Password)
                {
                    MessageDialog msg = new MessageDialog("Old and New Password are the same.\n" + "Enter a new password.", "Hello");
                    await msg.ShowAsync();
                    return;
                }
                else
                {
                    UserLoginDataPayLoad userDetail = new UserLoginDataPayLoad();
                    userDetail.StaffId = _activePage.UserStaffId;
                    userDetail.userPassword = txtNewPassword.Password.Trim();
                    userDetail.userStatus = txtOldPassword.Password.Trim();

                    ResponseMessage msgUpdateUser = await service.ChangeThisUSerPasswordControllerService(userDetail);

                    if (msgUpdateUser.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                    {
                        MessageDialog ms = new MessageDialog("Password Changed Successful");
                        await ms.ShowAsync();
                        txtOldPassword.Password = "";
                        txtNewPassword.Password = "";
                    }
                    else
                    {
                        MessageDialog ms = new MessageDialog(msgUpdateUser.Message, "Error");
                        await ms.ShowAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msd = new MessageDialog(ex.Message);
            }
        }

        private void txtPassword_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            PasswordBox txt = sender as PasswordBox;
            txt.IsPasswordRevealButtonEnabled = true;
        }

        private void txtPassword_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PasswordBox txt = sender as PasswordBox;
            txt.IsPasswordRevealButtonEnabled = true;
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

    }
}
