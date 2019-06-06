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
using VisitorApp;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI;
using VisitorApp.Models;
using VisitorApp.Common;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Globalization;
using VisitorApp.Dashboard.Admin;
using VisitorApp.Dashboard.Staff;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VisitorApp.Dashboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FrontDeskHome : Page
    {
        AccountDetails _activePage = new AccountDetails();
        public List<VisitorGlobal> VisitorList { get; set; }
        public List<GuestGlobal> guestList { get; set; }
        public List<DisplayDetails> DisplayGuestList { get; set; }
        int CompId;
        RemoteService service = new RemoteService();
        GetDataFromDB GetDataFromDB = new GetDataFromDB();
        VisitorAppHelper VisitorAppHelper = new VisitorAppHelper();
        
        public FrontDeskHome()
        {
            this.InitializeComponent();
        }

        #region All Page - Maybe Amateur

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _activePage = (AccountDetails)e.Parameter;
            if (_activePage != null)
            {
                txbUserCompanyName.Text = (_activePage.CompanyName).ToUpper();
                txbUserName.Text = (_activePage.UserStaffName).ToUpper();
                imgCompanyLogo.Source = _activePage.CompanyLogo;
                imgUserPhoto.ImageSource = _activePage.UserStaffPhoto;
                CompId = _activePage.CompanyId;
            }

            #region Role Show Button
            stackDashboard.Visibility = Visibility.Collapsed;
            stackDepartment.Visibility = Visibility.Collapsed;
            stackStaff.Visibility = Visibility.Collapsed;
            stackLocation.Visibility = Visibility.Collapsed;
            stackAppointment.Visibility = Visibility.Collapsed;
            stackUpload.Visibility = Visibility.Collapsed;
            stackCheckIn.Visibility = Visibility.Collapsed;
            stackCheckOut.Visibility = Visibility.Collapsed;
            stackGuestList.Visibility = Visibility.Collapsed;
            stackCompany.Visibility = Visibility.Collapsed;

            if (_activePage.UserStaffRole.ToUpper() == "ADMIN")
            {
                stackDashboard.Visibility = Visibility.Visible;
                stackDepartment.Visibility = Visibility.Visible;
                stackStaff.Visibility = Visibility.Visible;
                stackLocation.Visibility = Visibility.Visible;
                stackAppointment.Visibility = Visibility.Visible;
                stackUpload.Visibility = Visibility.Visible;
                stackCheckIn.Visibility = Visibility.Visible;
                stackCheckOut.Visibility = Visibility.Visible;
                stackGuestList.Visibility = Visibility.Visible;
                stackCompany.Visibility = Visibility.Visible;
            }
            else if (_activePage.UserStaffRole.ToUpper() == "FRONTDESK")
            {
                stackDashboard.Visibility = Visibility.Visible;
                stackUpload.Visibility = Visibility.Visible;
                stackAppointment.Visibility = Visibility.Visible;
                stackCheckIn.Visibility = Visibility.Visible;
                stackCheckOut.Visibility = Visibility.Visible;
                stackGuestList.Visibility = Visibility.Visible;
            }
            else if (_activePage.UserStaffRole.ToUpper() == "STAFF")
            {
                stackAppointment.Visibility = Visibility.Visible;
            }

            stackUpload.Visibility = Visibility.Collapsed;  //Till update
            #endregion
        }

        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HubPage));
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            //The same Page
            //this.Frame.Navigate(typeof(FrontDeskHome),_activePage);
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RegisterGuest), _activePage);
        }

        private void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CheckInGuest), _activePage);
        }

        private void btnCheckOutGuest_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CheckOutGuest), _activePage);
        }

        private void btnSeeGuestList_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SeeGuestList), _activePage);
        }

        private void btnUploadCsv_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UploadCsvFile), _activePage);
        }
        private void btnDepartment_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddDepartment), _activePage);
        }
        private void btnStaff_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddStaff), _activePage);
        }
        private void btnAppointment_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Appointment), _activePage);
        }
        private void btnLocation_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Location), _activePage);
        }

        private void btnCompSetting_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CompanyProfile), _activePage);
        }
        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            getData();
        }

        private void getData()
        {
            try
            {
                VisitorList = new List<VisitorGlobal>();
                guestList = new List<GuestGlobal>();

                VisitorDataPayLoad payload = new VisitorDataPayLoad{
                    CompanyId = _activePage.CompanyId
                };

                //var response = service.GuestStillCheckInListControllerService(payload);
                //VisitorList = response.VisitorList;

                var response = service.GuestListControllerService(payload);
                guestList = response.GuestList;

                DisplayGuestList = new List<DisplayDetails>();
                var serialNo = 0;

                int GuestCheckedInToday = 0;
                int GuestStillCheckedInToday = 0;
                int GuestThisWeek = 0;
                int GuestThisMonth = 0;
                int GuestThisYear = 0;

                foreach (var item in guestList)
                {
                    if (item.CheckInTime == item.CheckOutTime)
                    {
                        GuestStillCheckedInToday += 1;
                    }

                    if (item.CheckInTime.Date == DateTime.Today)
                    {
                        GuestCheckedInToday += 1;
                    }

                    DayOfWeek fdow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek; //First Day of the Week
                    int offset = fdow - DateTime.Now.DayOfWeek;
                    DateTime fdowDate = DateTime.Now.AddDays(offset);   //Date of First Day of the Week
                    DateTime LdowDate = fdowDate.AddDays(6);    //Date of Last Day of the Week
                    DayOfWeek Ldow = LdowDate.DayOfWeek;    //Last Day of the Week
                    var thisMonth = DateTime.Today.Month;
                    var thisYear = DateTime.Today.Year;

                    if ((item.CheckInTime.Date >= fdowDate.Date) && (item.CheckInTime.Date <= LdowDate.Date))
                    {
                        GuestThisWeek += 1;
                    }

                    if ((item.CheckInTime.Month == thisMonth) && (item.CheckInTime.Year == thisYear))
                    {
                        GuestThisMonth += 1;
                    }

                    if (item.CheckInTime.Year == thisYear)
                    {
                        GuestThisYear += 1;
                    }
                }

                txbGuestToday.Text = GuestCheckedInToday.ToString();
                txbStillCheckinGuestToday.Text = GuestStillCheckedInToday.ToString();
                txbGuestthisWeek.Text = GuestThisWeek.ToString();
                txbGuestthisMonth.Text = GuestThisMonth.ToString();
                txbGuestthisYear.Text = GuestThisYear.ToString();

                var dateFormatter = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("day month year");
                txbToday.Text = dateFormatter.Format(DateTime.Today).ToString();
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msg = new MessageDialog(ex.Message + " Void - getData()");
                //msg.ShowAsync();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(ChooseEvent), _activePage);
            this.Frame.Navigate(typeof(CompanyProfile), _activePage);
        }

        private void txbUserName_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(userProfile), _activePage);
        }

        private void btnTodayGuest_Click(object sender, RoutedEventArgs e)
        {
            _activePage.PageMsg = _activePageMsg.DashBoardTodayGuest.ToString();
            btnSeeGuestList_Click(sender, e);
        }

        private void btnStillCheckInGuest_Click(object sender, RoutedEventArgs e)
        {
            _activePage.PageMsg = _activePageMsg.DashBoardStillCheckInGuest.ToString();
            btnSeeGuestList_Click(sender, e);
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            var phoNo = Convert.ToInt64(txtPhono.Text.Trim());
            var resp = GetDataFromDB.GetDataThisVisitorDetail(phoNo).Result;

            if (resp.ResponseStatusCode == System.Net.HttpStatusCode.Found)
            {
                MessageDialog n = new MessageDialog("uu");
                n.ShowAsync();
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
