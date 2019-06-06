using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VisitorApp.Common;
using VisitorApp.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class CompanyProfile : Page
    {
        AccountDetails _activePage = new AccountDetails();
        GetDataFromDB GetDataFromDB = new GetDataFromDB();
        RemoteService service = new RemoteService();
        UserProcess VisitorAppUserProcess = new UserProcess();
        VisitorAppHelper VisitorAppHelper = new VisitorAppHelper();
        int CompId;

        string companyName;
        string companyLogoString;
        string companyAddress;
        string companyEmailAddress;
        string companyPhoneNumber;

        public CompanyProfile()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _activePage = (AccountDetails)e.Parameter;
            if (_activePage != null)
            {
                txtCompanyName.Text = (_activePage.CompanyName);
                txtCompanyEmail.Text = (_activePage.CompanyEmail);
                txtCompanyAddress.Text = (_activePage.CompanyAddress);
                txtCompanyPhoneNum.Text = (_activePage.CompanyPhoneNumber);
                imgCompanyLogo.Source = _activePage.CompanyLogo;
                CompId = _activePage.CompanyId;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FrontDeskHome), _activePage);
        }

        private async void btnChangeLogo_Click(object sender, RoutedEventArgs e)
        {
            //Select from Gallery
            StorageFile CompanyImage = await VisitorAppHelper.selectImage();
            //make sure the fill is not null

            if (CompanyImage == null)
            {
                //if null
                return;
            }

            imgCompanyLogo.Source = await VisitorAppHelper.GetImage(CompanyImage);
            companyLogoString = await VisitorAppHelper.ConvertImageToBase64();
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (imgCompanyLogo.Source == null)
                {
                    MessageDialog msg = new MessageDialog("Select Company Logo", "Alert - Empty Field!");
                    await msg.ShowAsync();
                }
                else if (string.IsNullOrEmpty(txtCompanyAddress.Text.Trim()))
                {
                    MessageDialog msg = new MessageDialog("Add Company Address", "Alert - Empty Field!");
                    await msg.ShowAsync();
                }
                else if (string.IsNullOrEmpty(txtCompanyEmail.Text.Trim()))
                {
                    MessageDialog msg = new MessageDialog("Add Company Email", "Alert - Empty Field!");
                    await msg.ShowAsync();
                }
                else if (string.IsNullOrEmpty(txtCompanyName.Text.Trim()))
                {
                    MessageDialog msg = new MessageDialog("Add Company Name", "Alert - Empty Field!");
                    await msg.ShowAsync();
                }
                else if (string.IsNullOrEmpty(txtCompanyPhoneNum.Text.Trim()))
                {
                    MessageDialog msg = new MessageDialog("Add Company Phone Number", "Alert - Empty Field!");
                    await msg.ShowAsync();
                }
                else
                {
                    companyName = txtCompanyName.Text.Trim();
                    companyAddress = txtCompanyAddress.Text.Trim();
                    companyPhoneNumber = txtCompanyPhoneNum.Text.Trim();
                    companyEmailAddress = txtCompanyEmail.Text.Trim();

                    CompanyDataPayLoad company = new CompanyDataPayLoad();
                    company.CompanyName = companyName;
                    company.CompanyId = CompId;
                    company.CompanyAddress = companyAddress;
                    company.CompanyEmailAddress = companyEmailAddress;
                    company.CompanyPhoneNumber = companyPhoneNumber;
                    company.CompanyLogo = companyLogoString;
                    company.Description = "Company Profile updated by " + _activePage.UserStaffName + " | " + _activePage.UserStaffId;

                    ResponseMessage msgExist = await service.CheckIfCompanyExistService(company);

                    if (msgExist.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                    {
                        ResponseMessage msgComp = await service.UpdateThisCompanyControllerService(company);

                        if (msgComp.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            _activePage.CompanyName = companyName;
                            _activePage.CompanyId = CompId;
                            _activePage.CompanyAddress = companyAddress;
                            _activePage.CompanyEmail = companyEmailAddress;
                            _activePage.CompanyPhoneNumber = companyPhoneNumber;
                            _activePage.CompanyLogoString = companyLogoString;
                            _activePage.CompanyLogo = await VisitorAppHelper.Base64StringToBitmap(_activePage.CompanyLogoString);
                            MessageDialog ms = new MessageDialog("Update Successful");
                            await ms.ShowAsync();
                        }
                        else
                        {
                            MessageDialog ms = new MessageDialog(msgComp.Message, "Err: ");
                            await ms.ShowAsync();
                        }
                    }
                    else
                    {
                        MessageDialog ms = new MessageDialog(msgExist.Message, "Err: ");
                        await ms.ShowAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msgErr = new MessageDialog(ex.Message, "Error - btnUpdate_Click");
                //msgErr.ShowAsync();
            }
        }

        private void txtPhoneNumber_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                //For check sytax entered in PhoneNumber field
                CheckSyntax.checkOnlyNumber(sender, e);
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message , "Err - txtPhoneNumber_KeyUp");
                //msg.ShowAsync();
            }
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
