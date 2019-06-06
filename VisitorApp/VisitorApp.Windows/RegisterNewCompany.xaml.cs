using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using VisitorApp.Common;
using VisitorApp.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VisitorApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterNewCompany : Page
    {
        private string companyName;
        private string companyEmail;
        private string companyAddress;
        private string companyPhoneNumber;
        private string logoString;

        private string staffName;
        private string StaffPhoneNumber;
        private string StaffEmail;
        private string StaffIdNumber;
        private string StaffPhotoString;
        private int DepartmentId;
        private int CompanyId;
        private int StaffId;

        private string adsString = "@live.com";

        VisitorAppHelper VisitorAppHelper = new VisitorAppHelper();
        UserProcess VisitorAppUserProcess = new UserProcess();
        public RegisterNewCompany()
        {
            this.InitializeComponent();
        }

        private async void btnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            StorageFile CompanyLogoImage = await VisitorAppHelper.selectImage();

            //make sure the fill is not null

            if (CompanyLogoImage == null)
            {
                //if null
                return;
            }

            //Convert Image
            imgCompanyLogo.Source = await VisitorAppHelper.GetImage(CompanyLogoImage);

            // Convert the byte array to Base 64 string
            logoString = await VisitorAppHelper.ConvertImageToBase64();
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtCompanyName.Text.Trim() == "")
                {
                    MessageDialog md = new MessageDialog("Please enter Company full name");
                    await md.ShowAsync();
                    return;
                }

                else if (txtCompanyPhoneNumber.Text.Trim() == "")
                {
                    MessageDialog md = new MessageDialog("Please enter Company phone Number");
                    await md.ShowAsync();
                    return;
                }

                else if (txtCompanyAddress.Text.Trim() == "")
                {
                    MessageDialog md = new MessageDialog("Please enter Company Address");
                    await md.ShowAsync();
                    return;
                }
                else if (logoString == "")
                {
                    MessageDialog md = new MessageDialog("Please Select Company Logo");
                    await md.ShowAsync();
                    return;
                }
                else if (txtCompanyEmail.Text.Trim() == "")
                {
                    MessageDialog md = new MessageDialog("Please enter Company Email");
                    await md.ShowAsync();
                    return;
                }
                else if (!txtCompanyEmail.Text.Contains('@') || !txtCompanyEmail.Text.Contains('.'))
                {
                    MessageDialog md = new MessageDialog("Please Enter a Valid Company Email");
                    await md.ShowAsync();
                    return;
                }
                else if (txtAdminName.Text.Trim() == "")
                {
                    MessageDialog md = new MessageDialog("Please enter Admin name");
                    await md.ShowAsync();
                    return;
                }

                else if (txtAdminPhoneNumber.Text.Trim() == "")
                {
                    MessageDialog md = new MessageDialog("Please enter Admin phone Number");
                    await md.ShowAsync();
                    return;
                }

                else if (txtAdminStaffIdNumber.Text.Trim() == "")
                {
                    MessageDialog md = new MessageDialog("Please enter Admin Staff ID Number");
                    await md.ShowAsync();
                    return;
                }
                else if (txtAdminEmail.Text.Trim() == "")
                {
                    MessageDialog md = new MessageDialog("Please enter Admin Email");
                    await md.ShowAsync();
                    return;
                }
                else if (!txtAdminEmail.Text.Contains('@') || !txtAdminEmail.Text.Contains('.'))
                {
                    MessageDialog md = new MessageDialog("Please Enter a Valid Admin Email");
                    await md.ShowAsync();
                    return;
                }
                else
                {
                    companyName = txtCompanyName.Text;
                    companyEmail = txtCompanyEmail.Text;
                    companyAddress = txtCompanyAddress.Text;
                    companyPhoneNumber = txtCompanyPhoneNumber.Text;

                    staffName = txtAdminName.Text;
                    StaffIdNumber = txtAdminStaffIdNumber.Text;
                    StaffPhoneNumber = txtAdminPhoneNumber.Text;
                    StaffEmail = txtAdminEmail.Text;

                    RemoteService service = new RemoteService();
                    CompanyDataPayLoad company = new CompanyDataPayLoad
                    {
                        CompanyName = companyName
                    };

                    StaffDataPayload staff = new StaffDataPayload
                    {
                        StaffPhoneNumber = StaffPhoneNumber
                    };
                    var response = await service.CheckIfCompanyExistService(company);
                    var response2 = await service.CheckIfStaffExistService(staff);


                    if (response.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                    {
                        MessageDialog CompanyRegMsg = new MessageDialog("Company already exist");
                        await CompanyRegMsg.ShowAsync();
                        return;
                    }
                    else if (response2.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                    {
                        MessageDialog AdminRegMsg = new MessageDialog("Admin already exist");
                        await AdminRegMsg.ShowAsync();
                        return;
                    }
                    else if (response.ResponseStatusCode == System.Net.HttpStatusCode.NotFound && response2.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        //Send to Registration DB
                        newCompany();
                    }
                    else
                    {
                        ResponseMessage msg = await service.RegisterNewCompany(company);
                        MessageDialog checkInMsg = new MessageDialog("Server error");
                        checkInMsg.ShowAsync();
                    }
                }

                //After Registration

                //clearContents();
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnSubmit_Click");
                checkInternet();
                //await msg.ShowAsync();
            }
        }

        private async void newCompany()
        {
            try
            {
                //Register new Company
                CompanyDataPayLoad company = new CompanyDataPayLoad();
                company.CompanyName = companyName;
                company.CompanyEmailAddress = companyEmail;
                company.CompanyAddress = companyAddress;
                company.CompanyPhoneNumber = companyPhoneNumber;
                company.CompanyLogo = logoString;
                company.Description = "Added by Company Admin " + txtAdminName.Text + " on " + DateTime.Now.ToString();

                RemoteService service = new RemoteService();
                ResponseMessage msg = await service.RegisterNewCompany(company);

                CompanyId = msg.ResponseCode;

                newDepartmet();
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - newCompany");
                checkInternet();
                //msg.ShowAsync();
            }
        }

        private async void newDepartmet()
        {
            try
            {
                //Register new Department
                DepartmentDataPayload department = new DepartmentDataPayload();
                department.DepartmentName = "Admin";
                department.DepartmentPhoneNumber = StaffPhoneNumber;
                department.CompanyId = CompanyId;
                department.Description = "Newly Added on by " + txtAdminName.Text + " on " + DateTime.Now.ToString();
                department.Status = DeptStatus.Active.ToString();

                RemoteService service = new RemoteService();
                ResponseMessage msg = await service.RegisterNewDepartment(department);

                DepartmentId = msg.ResponseCode;

                if (msg.ResponseStatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    MessageDialog ms = new MessageDialog( " Void - newDepartment" + "\n" + msg.Message);
                    ms.ShowAsync();
                    return;
                }
                else
                {

                    newStaff();
                }

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - newDepartment");
                checkInternet();
                //msg.ShowAsync();
            }
        }

        private async void newStaff()
        {
            try
            {
                StaffDataPayload staff = new StaffDataPayload();
                staff.StaffName = staffName;
                staff.StaffEmail = StaffEmail;
                staff.StaffPhoneNumber = StaffPhoneNumber;
                staff.StaffIdNumber = StaffIdNumber;
                staff.DepartmentId = DepartmentId;
                staff.Role = "Admin";
                staff.CompanyId = CompanyId;
                staff.StaffPhoto = StaffPhotoString;
                staff.Description = "Admin for Company " + companyName;
                RemoteService service = new RemoteService();
                ResponseMessage msg = await service.RegisterNewStaff(staff);

                StaffId = msg.ResponseCode;

                newUser();
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - newStaff");
                checkInternet();
                //msg.ShowAsync();
            }
        }

        private async void newUser()
        {
            try
            {
                string userName = await VisitorAppUserProcess.CreateUserName(txtAdminEmail.Text);
                string password = txtAdminPassword.Password;

                UserLoginDataPayLoad userDetail = new UserLoginDataPayLoad();
                userDetail.username = userName;
                userDetail.userPassword = password;
                userDetail.StaffId = StaffId;

                RemoteService service = new RemoteService();
                ResponseMessage msg = await service.RegisterNewUser(userDetail);

                if (msg.ResponseStatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    MessageDialog mdd = new MessageDialog("Could not complete registration: " + msg.Message);
                    await mdd.ShowAsync();
                    return;
                }

                MessageDialog md = new MessageDialog("Registration Complete");
                await md.ShowAsync();
                Clear();

                this.Frame.Navigate(typeof(HubPage));
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - newUser");
                checkInternet();
                //msg.ShowAsync();
            }
        }

        private void txtCompanyPhoneNumber_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                //For check sytax entered in Name field
                CheckSyntax.checkOnlyNumber(sender, e);
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - txtPhoneNumber_KeyUp");
                checkInternet();
                //msg.ShowAsync();
            }
        }

        private async void btnSelectStaffPicture_Click(object sender, RoutedEventArgs e)
        {
            StorageFile StaffImage = await VisitorAppHelper.selectImage();

            //make sure the fill is not null

            if (StaffImage == null)
            {
                //if null
                return;
            }

            //Convert Image
            imgStaffPicture.Source = await VisitorAppHelper.GetImage(StaffImage);

            // Convert the byte array to Base 64 string
            StaffPhotoString = await VisitorAppHelper.ConvertImageToBase64();
        }

        private async void btnTester_Click(object sender, RoutedEventArgs e)
        {
            string userName = await VisitorAppUserProcess.CreateUserName(txtAdminEmail.Text);

            MessageDialog msg = new MessageDialog(userName);
            await msg.ShowAsync();
        }

        private void Clear()
        {
            txtAdminEmail.Text="";
            txtAdminName.Text = "";
            txtAdminPassword.Password = "";
            txtAdminPhoneNumber.Text = "";
            txtAdminStaffIdNumber.Text = "";
            txtCompanyAddress.Text = "";
            txtCompanyEmail.Text = "";
            txtCompanyName.Text = "";
            txtCompanyPhoneNumber.Text = "";

            imgCompanyLogo.Source = null;
            imgStaffPicture.Source = null;
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
    }
}
