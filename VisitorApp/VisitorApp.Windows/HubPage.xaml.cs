using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using VisitorApp.Data;
using VisitorApp.Common;
using Windows.UI.Popups;
using Windows.UI;
using System.Net.NetworkInformation;
using Windows.Networking.Connectivity;
using VisitorApp.Models;
using VisitorApp.Dashboard;
using Windows.System;
using VisitorApp.Dashboard;
using VisitorApp.Dashboard.Admin;
using VisitorApp.Dashboard.Staff;
//using static VisitorApp.ViewModels.MainViewModel;

// The Universal Hub Application project template is documented at http://go.microsoft.com/fwlink/?LinkID=391955

namespace VisitorApp
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        double sizeIncrement = 10;
        bool shouldContinue;
        AccountDetails _activePage = new AccountDetails();

        UserProcess VisitorAppUserProcess = new UserProcess();
        VisitorAppHelper VisitorAppHelper = new VisitorAppHelper();
        GetDataFromDB GetDataFromDB = new GetDataFromDB();

        /// <summary>
        /// Gets the NavigationHelper used to aid in navigation and process lifetime management.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the DefaultViewModel. This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public HubPage()
        {
            this.InitializeComponent();
           // this.navigationHelper = new NavigationHelper(this);
            //this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            //Loaded += HubPage_Loaded;
        }

        //private void HubPage_Loaded(object sender, RoutedEventArgs e)
        //{
        //    this.Frame.Navigate(typeof(login));
        //}

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-4");
            this.DefaultViewModel["Section3Items"] = sampleDataGroup;
        }

        /// <summary>
        /// Invoked when a HubSection header is clicked.
        /// </summary>
        /// <param name="sender">The Hub that contains the HubSection whose header was clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            var group = section.DataContext;
            this.Frame.Navigate(typeof(sample), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((Data.SampleDataItem)e.ClickedItem).UniqueId;
            //this.Frame.Navigate(typeof(ItemPage), itemId);
        }
        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </summary>
        #endregion

        private async void loginDetail()
        {
            try
            {
                string userName = await VisitorAppUserProcess.CreateUserName(txtUserName.Text);

                RemoteService service = new RemoteService();
                UserLoginDataPayLoad userDetail = new UserLoginDataPayLoad
                {
                    username = userName,
                    userPassword = txtPassword.Password
                };
                var response = await service.CheckIfUserExistService(userDetail);

                if (response.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                {
                    //MessageDialog Msg = new MessageDialog("User already exist");
                    //await Msg.ShowAsync();
                    //return;
                    _activePage.UserStaffId = response.StaffId;
                    var staffId = _activePage.UserStaffId;

                    var ThisStaff = new List<StaffGlobal>();
                    #region MyRegion

                    //RemoteService serviceThisStaff = new RemoteService();
                    //StaffDataPayload ThisStaffPayLoad = new StaffDataPayload()
                    //{
                    //    StaffId = staffId
                    //};

                    //var responseThisStaff = service.GetThisStaffControllerService(ThisStaffPayLoad);
                    //ThisStaff = responseThisStaff.StaffList;


                    #endregion

                    var responseThisStaff = GetDataFromDB.GetDataThisStaff(staffId).Result;
                    ThisStaff = responseThisStaff.StaffList;
                    string staffStatus = "";
                    foreach (var item in ThisStaff)
                    {
                        _activePage.UserStaffId = item.StaffId;
                        _activePage.UserStaffRole = item.Roles;
                        _activePage.UserStaffName = item.StaffName;
                        _activePage.UserStaffPhotoString = item.StaffPhoto;
                        _activePage.CompanyId = item.CompanyId;
                        _activePage.UserDepartmentId = item.DepartmentId;
                        _activePage.UserStaffIdNumber = item.StaffIdNumber;
                        _activePage.UserStaffPhoneNumber = item.StaffPhoneNumber;
                        _activePage.UserStaffPhoto = await VisitorAppHelper.Base64StringToBitmap(item.StaffPhoto);
                        staffStatus = item.StaffStatus;
                    }

                    if (staffStatus.ToUpper() == StaffStatus.Suspended.ToString().ToUpper())
                    {
                        MessageDialog msh = new MessageDialog("You are suspended and cannot currently access this application", "Alert!");
                        await msh.ShowAsync();
                        return;
                    }

                    var CompanyId = _activePage.CompanyId;
                    var ThisCompany = new List<CompanyGlobal>();

                    #region MyRegion
                    ////RemoteService serviceThisCompany = new RemoteService();
                    //CompanyDataPayLoad ThisCompanyPayLoad = new CompanyDataPayLoad()
                    //{
                    //    CompanyId = CompanyId
                    //}; 
                    #endregion

                    var responseThisCompany = GetDataFromDB.GetDataThisCompany(CompanyId).Result;
                    ThisCompany = responseThisCompany.CompanyList;

                    foreach (var item in ThisCompany)
                    {
                        _activePage.CompanyName = item.CompanyName;
                        _activePage.CompanyLogoString = item.CompanyLogo;
                        _activePage.CompanyLogo = await VisitorAppHelper.Base64StringToBitmap(item.CompanyLogo);
                        _activePage.CompanyPhoneNumber = item.CompanyPhoneNumber;
                        _activePage.CompanyAddress = item.CompanyAddress;
                        _activePage.CompanyEmail = item.CompanyEmailAddress;
                        _activePage.CompanyStatus = item.CompanyStatus;
                    }

                    if (_activePage.CompanyStatus.ToUpper() == CompanyStatus.Suspend.ToString().ToUpper())
                    {
                        MessageDialog msgComp = new MessageDialog("Company License is not found.\n" + "Contact +234-XXX", "Alert!!!");
                        msgComp.ShowAsync();
                        return;
                    }

                    if (_activePage.CompanyStatus.ToUpper() == CompanyStatus.Deleted.ToString().ToUpper())
                    {
                        MessageDialog msgComp = new MessageDialog("Company is not registered", "Alert!!!");
                        msgComp.Commands.Add(new UICommand("Register Company") {Id = 0});
                        msgComp.Commands.Add(new UICommand("Cancel") {Id = 1});
                        msgComp.CancelCommandIndex = 1;
                        var result = await msgComp.ShowAsync();


                        if (Convert.ToInt32(result.Id) == 0)
                        {
                            this.Frame.Navigate(typeof(RegisterNewCompany));
                        }
                    }
                    var DepartmentId = _activePage.UserDepartmentId;
                    var ThisDepartment = new List<DepartmentGlobal>();

                    #region MyRegion

                    //RemoteService serviceThisDepartment = new RemoteService();
                    //DepartmentDataPayload ThisDepartmentPayLoad = new DepartmentDataPayload()
                    //{
                    //    DepartmentId = DepartmentId
                    //};

                    #endregion
                    var responseThisDepartment = GetDataFromDB.GetDataThisDepartment(DepartmentId).Result;
                    ThisDepartment = responseThisDepartment.DepartmentList;

                    foreach (var item in ThisDepartment)
                    {
                        _activePage.UserDepartmentName = item.DepartmentName;
                        //_activePage.de = item.CompanyLogo;
                    }

                    if (_activePage.UserStaffRole.ToUpper() == StaffRoles.Staff.ToString().ToUpper())
                    {
                        this.Frame.Navigate(typeof(Appointment), _activePage);
                    }
                    else
                    {
                        this.Frame.Navigate(typeof(FrontDeskHome), _activePage);
                        //this.Frame.Navigate(typeof(CheckInWithInvitationPage), _activePage);
                    }
                }

                else
                {
                    MessageDialog msg = new MessageDialog("Details not Correct");
                    await msg.ShowAsync();
                    //this.Frame.Navigate(typeof(RegisterNewCompany));
                }
            }
            catch (Exception ex)
            {
                MessageDialog msgd = new MessageDialog(ex.Message);
                MessageDialog msg = new MessageDialog("Try Again","Hello");
                await msg.ShowAsync();
            }
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //checkInternet();
                proceedCriteria();
                if (shouldContinue == true)
                {
                    loginDetail();
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message);
                //await msg.ShowAsync();
            }
        }

        private async void proceedCriteria()
        {
            shouldContinue = false;
            ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();

            if (profile.GetNetworkConnectivityLevel() >= NetworkConnectivityLevel.InternetAccess)
            {
                shouldContinue = true;
            }
            else
            {
                MessageDialog msg = new MessageDialog("No Internet Connection." + "\n" + "Please connect to Internet and Try Again");
                await msg.ShowAsync();
            }
        }

        private async void txtEnter_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                if (e.Key == VirtualKey.Enter)
                {
                    btnLogin_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - txtEnter_KeyDown");
                //await msg.ShowAsync();
            }
        }

        private async void btnForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //txtUserName.Text = "ebere@viaphonenig.com";
                //txtPassword.Password = "qwertyuiop";
                //loginDetail();
                this. Frame.Navigate(typeof(ForgotPassword));
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message);
                //await msg.ShowAsync();
            }       
        }

        private void btnNewCompany_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RegisterNewCompany));
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
