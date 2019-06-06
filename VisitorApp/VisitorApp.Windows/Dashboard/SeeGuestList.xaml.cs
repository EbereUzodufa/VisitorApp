using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VisitorApp.Common;
using VisitorApp.Dashboard.Admin;
using VisitorApp.Dashboard.Staff;
using VisitorApp.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
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

namespace VisitorApp.Dashboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SeeGuestList : Page
    {
        private const string StatusCheckedInText = "Checked In";
        private const string StatusCheckedOutText = "Checked Out";

        private const string InvitationCodeNoText = "";

        public List<GuestGlobal> guestList { get; set; }
        public List<DisplayDetails> DisplayGuestList { get; set; }

        #region Help Variables
        int sn;
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Company { get; set; }
        public string HostName { get; set; }
        public DateTime DateCheckedIn { get; set; }
        public string Status { get; set; }
        public string InvitationCode { get; set; }

        List<DisplayDetails> SearchedList { get; set; }
        List<string> _header;
        List<string> Header;

        public double Angle;

        int fullnameIndex;
        int companyIndex;
        int emailIndex;
        int phoneNumberIndex;
        int hostnameIndex;
        int dateCheckedInIndex;
        int statusIndex;
        int invitationCodeIndex;

        AccountDetails _activePage = new AccountDetails();
        VisitorAppHelper VisitorAppHelper = new VisitorAppHelper();
        private int CompId;

        #endregion

        public SeeGuestList()
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
                stackAppointment.Visibility = Visibility.Visible;
                stackUpload.Visibility = Visibility.Visible;
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
            this.Frame.Navigate(typeof(FrontDeskHome),_activePage);
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
            //The same Page
            //this.Frame.Navigate(typeof(SeeGuestList), _activePage);
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
            try
            {
                getData();
                //btnShowAll_Click(sender, e);
                if (_activePage.PageMsg == _activePageMsg.DashBoardTodayGuest.ToString())
                {
                    btnShowTodayGuest_Click(sender, e);
                    btnSort.Content = btnSortTodayGuest.Text;
                }
                else
                {
                    btnShowCheckedIn_Click(sender, e);
                    btnSort.Content = btnSortCheckedIn.Text;
                }
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msg = new MessageDialog(ex.Message + " Void - Page_Loaded");
                //msg.ShowAsync();
            }
        }

        private void getData()
        {
            try
            {
                guestList = new List<GuestGlobal>();

                RemoteService service = new RemoteService();
                VisitorDataPayLoad payloadGuest = new VisitorDataPayLoad
                {
                    CompanyId = CompId
                };

                var response = service.GuestListControllerService(payloadGuest);
                guestList = response.GuestList;

                DisplayGuestList = new List<DisplayDetails>();
                var serialNo = 0;

                foreach (var item in guestList)
                {
                    BitmapImage PhotoCopy = null;
                    string guestPhotoString = item.GuestPhotoString;
                    string guestPhoneNumber = item.GuestPhoneNumber;
                    string guestCompany = item.GuestCompany;
                    string guestEmail = item.GuestEmail;
                    string guestSignature = item.GuestSignature;
                    string guestThumbnail = item.GuestThumbnail;

                    if (guestPhotoString == null)
                    {
                        #region MyRegion
                        guestPhotoString = "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"; 
                        #endregion
                    }

                    byte[] Bytes = Convert.FromBase64String(guestPhotoString);

                    var stream = new InMemoryRandomAccessStream();
                    //var bytes = Convert.FromBase64String(source);
                    var dataWriter = new DataWriter(stream);
                    dataWriter.WriteBytes(Bytes);
                    dataWriter.StoreAsync();
                    stream.Seek(0);
                    var img = new BitmapImage();
                    img.SetSource(stream);
                    PhotoCopy = img;

                    serialNo += 1;

                    DisplayGuestList.Add(new DisplayDetails
                    {
                        sn = serialNo.ToString(),
                        GuestFullName = item.GuestName,
                        GuestHostName = item.HostName,
                        GuestStatus = item.Status,
                        GuestCompany = guestCompany,
                        GuestEmail = guestEmail,
                        GuestPhoneNumber = guestPhoneNumber,
                        GuestCheckInTime = item.CheckInTime,
                        GuestCheckOutTime = item.CheckOutTime,
                        GuestCheckInCode = item.CheckInCode,
                        GuestSignature = guestSignature,
                        GuestThumbPrint = guestThumbnail,
                        Picture = PhotoCopy
                    });
                }
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msg = new MessageDialog(ex.Message + " Void - getData()");
                //msg.ShowAsync();
            }

        }

        private void populateGrid()
        {
            //LstPopulate.Items.Clear();
            try
            {
                LstPopulate.ItemsSource = null;
                LstPopulate.ItemsSource = DisplayGuestList;

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - populateGrid()");
                //msg.ShowAsync();
            }
        }

        private async void btnPopulateGridfromSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region For Searching through List
                string txt = txtSearchItem.Text.Trim();

                if (LstPopulate.Items.Count != 0)
                {
                    searchedItems();
                }
                else
                {
                    MessageDialog ms = new MessageDialog("Empty" + "\n" + "\n" + "Click Select Text File to Select File");
                    await ms.ShowAsync(); //Await makes everyother unctiomn wait for the main time.
                }
                #endregion

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnPopulateGridfromSearch_Click");
                //msg.ShowAsync();
            }
        }

        private async void btnFlterSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (stackPanelFilter.Visibility == Visibility.Collapsed)
                {
                    stackPanelFilter.Visibility = Visibility.Visible;
                }
                else
                {
                    stackPanelFilter.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnFilterSearch_Click()");
                //msg.ShowAsync();
            }
        }

        public DisplayDetails AddItemDetail(string sn, string Fullname, string Company, string Email, string PhoneNumber, string HostName, DateTime DateCheckedIn, DateTime DateCheckedOut,
                                            string Status, string InvitationCode, string Signature, string ThumbPrint, BitmapImage PhotoCopy)
        {
            var AddItem = new DisplayDetails();
            AddItem.sn = sn;
            AddItem.GuestFullName = Fullname;
            AddItem.GuestCompany = Company;
            AddItem.GuestEmail = Email;
            AddItem.GuestPhoneNumber = PhoneNumber;
            AddItem.GuestHostName = HostName;
            AddItem.GuestCheckInTime = DateCheckedIn;
            AddItem.GuestCheckOutTime = DateCheckedOut;
            AddItem.GuestStatus = Status;
            AddItem.GuestCheckInCode = InvitationCode;
            AddItem.GuestSignature = Signature;
            AddItem.GuestThumbPrint = ThumbPrint;
            AddItem.Picture = PhotoCopy;
            return AddItem;
        }

        private void searchedItems()
        {
            try
            {
                #region USing Binding to populate
                SearchedList = new List<DisplayDetails>();

                if (DisplayGuestList.Count != 0)
                {
                    LstSearch.ItemsSource = null;

                    var serialNo = 0;
                    string searchItem = txtSearchItem.Text.Trim().ToLower();
                    foreach (var item in DisplayGuestList)
                    {
                        FullName = item.GuestFullName;
                        Company = item.GuestCompany;
                        Email = item.GuestEmail;
                        PhoneNumber = item.GuestPhoneNumber;
                        HostName = item.GuestHostName;
                        DateCheckedIn = item.GuestCheckInTime;
                        Status = item.GuestStatus;
                        InvitationCode = item.GuestCheckInCode;     //There should be a field indicating if the code came from from the computer or assgned by a Host. This what I wanted to measure here

                        string _FullName = FullName.ToLower();
                        string _Company = Company.ToLower();
                        string _Email = Email.ToLower();
                        string _PhoneNumber = PhoneNumber.ToLower();
                        string _HostName = HostName.ToLower();

                        if (chbFullName.IsChecked == true && _FullName.Contains(searchItem))
                        {
                            serialNo += 1;

                            SearchedList.Add(AddItemDetail(serialNo.ToString(), FullName, Company, Email, PhoneNumber, HostName, DateCheckedIn, item.GuestCheckOutTime, Status, InvitationCode,
                                item.GuestSignature, item.GuestThumbPrint, item.Picture));
                        }

                        else if (chbHostName.IsChecked == true && _HostName.Contains(searchItem))
                        {
                            serialNo += 1;

                            SearchedList.Add(AddItemDetail(serialNo.ToString(), FullName, Company, Email, PhoneNumber, HostName, DateCheckedIn, item.GuestCheckOutTime, Status, InvitationCode,
                                item.GuestSignature, item.GuestThumbPrint, item.Picture));
                        }

                        else if (chbCompany.IsChecked == true && _Company.Contains(searchItem))
                        {
                            serialNo += 1;

                            SearchedList.Add(AddItemDetail(serialNo.ToString(), FullName, Company, Email, PhoneNumber, HostName, DateCheckedIn, item.GuestCheckOutTime, Status, InvitationCode,
                                item.GuestSignature, item.GuestThumbPrint, item.Picture));
                        }

                        else if (chbEmail.IsChecked == true && _Email.Contains(searchItem))
                        {
                            serialNo += 1;

                            SearchedList.Add(AddItemDetail(serialNo.ToString(), FullName, Company, Email, PhoneNumber, HostName, DateCheckedIn, item.GuestCheckOutTime, Status, InvitationCode,
                                item.GuestSignature, item.GuestThumbPrint, item.Picture));
                        }

                        else if (chbPhoneNumber.IsChecked == true && _PhoneNumber.Contains(searchItem))
                        {
                            serialNo += 1;

                            SearchedList.Add(AddItemDetail(serialNo.ToString(), FullName, Company, Email, PhoneNumber, HostName, DateCheckedIn, item.GuestCheckOutTime, Status, InvitationCode,
                                item.GuestSignature, item.GuestThumbPrint, item.Picture));
                        }

                    }

                    extraSearch();

                    if (SearchedList.Count != 0)
                    {
                        LstSearch.ItemsSource = null;
                        LstSearch.ItemsSource = SearchedList;
                        txtSearchListNo.Text = LstSearch.Items.Count.ToString();
                    }

                    else
                    {
                        LstSearch.ItemsSource = null;

                        MessageDialog msg = new MessageDialog("No Result Found");
                        txtSearchListNo.Text = "";
                        hideGridSearchPopulated();
                        msg.ShowAsync();
                    }

                    //SerializeItems();
                    stackPanelFilter.Visibility = Visibility.Collapsed;
                    showGridSearchPopulated();
                    //showPop();
                }
                #endregion

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - searchedItems()");
                //msg.ShowAsync();
            }
        }

        private void extraSearch()
        {
            try
            {
                var SearchListExtra = SearchedList;

                List<DisplayDetails> SearchListExtraStatus = new List<DisplayDetails>();
                List<DisplayDetails> SearchListExtraInvitationCode = new List<DisplayDetails>();
                List<DisplayDetails> SearchListExtraDate = new List<DisplayDetails>();

                #region Search in Status

                #region All Status Checked
                if (chbStatus.IsChecked == true)
                {
                    SearchListExtraStatus = SearchListExtra;
                }
                #endregion

                #region Only Checked In Status
                else if (chbStatusCheckIn.IsChecked == true && chbStatusCheckOut.IsChecked == false)
                {
                    var serialNo = 0;
                    foreach (var item in SearchListExtra)
                    {
                        if (item.GuestCheckInTime == item.GuestCheckOutTime)
                        {
                            serialNo += 1;
                            SearchListExtraStatus.Add(AddItemDetail(serialNo.ToString(), item.GuestFullName, item.GuestCompany, item.GuestEmail, item.GuestPhoneNumber, item.GuestPhoneNumber,
                                item.GuestCheckInTime, item.GuestCheckOutTime, item.GuestStatus, item.GuestCheckInCode, item.GuestSignature, item.GuestThumbPrint, item.Picture));
                        }
                    }
                }
                #endregion

                #region Only Chcked Out Status
                else if (chbStatusCheckIn.IsChecked == false && chbStatusCheckOut.IsChecked == true)
                {
                    var serialNo = 0;
                    foreach (var item in SearchListExtra)
                    {
                        if (item.GuestCheckInTime != item.GuestCheckOutTime)
                        {
                            serialNo += 1;
                            SearchListExtraStatus.Add(AddItemDetail(serialNo.ToString(), item.GuestFullName, item.GuestCompany, item.GuestEmail, item.GuestPhoneNumber, item.GuestPhoneNumber,
                                item.GuestCheckInTime, item.GuestCheckOutTime, item.GuestStatus, item.GuestCheckInCode, item.GuestSignature, item.GuestThumbPrint, item.Picture));
                        }
                    }
                }
                #endregion

                #endregion

                #region Search in Invitation Code

                #region All Invitation Code Status Checked
                if (chbInvitationCode.IsChecked == true)
                {
                    SearchListExtraInvitationCode = SearchListExtraStatus;
                }
                #endregion

                #region Only with Invitation Code Checked
                else if (chbInvitationCodeYes.IsChecked == true && chbInvitationCodeNo.IsChecked == false)
                {
                    var serialNo = 0;
                    foreach (var item in SearchListExtraStatus)
                    {
                        if (item.GuestCheckInCode != InvitationCodeNoText)
                        {
                            serialNo += 1;
                            SearchListExtraInvitationCode.Add(AddItemDetail(serialNo.ToString(), item.GuestFullName, item.GuestCompany, item.GuestEmail, item.GuestPhoneNumber, item.GuestPhoneNumber,
                                item.GuestCheckInTime, item.GuestCheckOutTime, item.GuestStatus, item.GuestCheckInCode, item.GuestSignature, item.GuestThumbPrint, item.Picture));
                        }
                    }
                }
                #endregion

                #region Only no Invitation Code Checked
                else if (chbInvitationCodeYes.IsChecked == false && chbInvitationCodeNo.IsChecked == true)
                {
                    var serialNo = 0;
                    foreach (var item in SearchListExtraStatus)
                    {
                        if (item.GuestCheckInCode == InvitationCodeNoText)
                        {
                            serialNo += 1;
                            SearchListExtraInvitationCode.Add(AddItemDetail(serialNo.ToString(), item.GuestFullName, item.GuestCompany, item.GuestEmail, item.GuestPhoneNumber, item.GuestPhoneNumber,
                                item.GuestCheckInTime, item.GuestCheckOutTime, item.GuestStatus, item.GuestCheckInCode, item.GuestSignature, item.GuestThumbPrint, item.Picture));
                        }
                    }
                }
                #endregion

                #endregion

                #region Search in Date

                #region All Date Checked
                if (chbDate.IsChecked == true)
                {
                    SearchListExtraDate = SearchListExtraInvitationCode;
                }
                #endregion

                #region From x to y Date Checked
                else if (chbDateFromTo.IsChecked == true)
                {
                    var serialNo = 0;
                    var startDate = dtpFrom.Date;
                    var EndDate = dtpTo.Date;

                    foreach (var item in SearchListExtraInvitationCode)
                    {
                        if ((item.GuestCheckInTime.Date >= startDate) && (item.GuestCheckInTime.Date <= EndDate))
                        {
                            serialNo += 1;
                            SearchListExtraDate.Add(AddItemDetail(serialNo.ToString(), item.GuestFullName, item.GuestCompany, item.GuestEmail, item.GuestPhoneNumber, item.GuestPhoneNumber,
                                item.GuestCheckInTime, item.GuestCheckOutTime, item.GuestStatus, item.GuestCheckInCode, item.GuestSignature, item.GuestThumbPrint, item.Picture));
                        }
                    }
                }
                #endregion

                #region Yesterday Date Checked
                else if (chbDateFromYesterday.IsChecked == true)
                {
                    var serialNo = 0;
                    var yesterday = DateTime.Today.AddDays(-1);

                    foreach (var item in SearchListExtraInvitationCode)
                    {
                        if (item.GuestCheckInTime.Date >= yesterday.Date)
                        {
                            serialNo += 1;
                            SearchListExtraDate.Add(AddItemDetail(serialNo.ToString(), item.GuestFullName, item.GuestCompany, item.GuestEmail, item.GuestPhoneNumber, item.GuestPhoneNumber,
                                item.GuestCheckInTime, item.GuestCheckOutTime, item.GuestStatus, item.GuestCheckInCode, item.GuestSignature, item.GuestThumbPrint, item.Picture));
                        }
                    }
                }
                #endregion

                #region This Week Date Checked
                else if (chbDateThisWeek.IsChecked == true)
                {
                    var serialNo = 0;
                    DayOfWeek fdow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek; //First Day of the Week
                    int offset = fdow - DateTime.Now.DayOfWeek;
                    DateTime fdowDate = DateTime.Now.AddDays(offset);   //Date of First Day of the Week
                    DateTime LdowDate = fdowDate.AddDays(6);    //Date of Last Day of the Week
                    DayOfWeek Ldow = LdowDate.DayOfWeek;    //Last Day of the Week

                    foreach (var item in SearchListExtraInvitationCode)
                    {
                        if ((item.GuestCheckInTime.Date >= fdowDate.Date) && (item.GuestCheckInTime.Date <= LdowDate.Date))
                        {
                            serialNo += 1;
                            SearchListExtraDate.Add(AddItemDetail(serialNo.ToString(), item.GuestFullName, item.GuestCompany, item.GuestEmail, item.GuestPhoneNumber, item.GuestPhoneNumber,
                                item.GuestCheckInTime, item.GuestCheckOutTime, item.GuestStatus, item.GuestCheckInCode, item.GuestSignature, item.GuestThumbPrint, item.Picture));
                        }
                    }
                }
                #endregion

                #region This Month Date Checked
                else if (chbDateThisMonth.IsChecked == true)
                {
                    var serialNo = 0;
                    var thisMonth = DateTime.Today.Month;
                    var thisYear = DateTime.Today.Year;

                    foreach (var item in SearchListExtraInvitationCode)
                    {
                        if ((item.GuestCheckInTime.Month == thisMonth) && (item.GuestCheckInTime.Year == thisYear))
                        {
                            serialNo += 1;
                            SearchListExtraDate.Add(AddItemDetail(serialNo.ToString(), item.GuestFullName, item.GuestCompany, item.GuestEmail, item.GuestPhoneNumber, item.GuestPhoneNumber,
                                item.GuestCheckInTime, item.GuestCheckOutTime, item.GuestStatus, item.GuestCheckInCode, item.GuestSignature, item.GuestThumbPrint, item.Picture));
                        }
                    }
                }
                #endregion

                #region This Year Date Checked
                else if (chbDateThisYear.IsChecked == true)
                {
                    var serialNo = 0;
                    var thisYear = DateTime.Today.Year;

                    foreach (var item in SearchListExtraInvitationCode)
                    {
                        if (item.GuestCheckInTime.Year == thisYear)
                        {
                            serialNo += 1;
                            SearchListExtraDate.Add(AddItemDetail(serialNo.ToString(), item.GuestFullName, item.GuestCompany, item.GuestEmail, item.GuestPhoneNumber, item.GuestPhoneNumber,
                                item.GuestCheckInTime, item.GuestCheckOutTime, item.GuestStatus, item.GuestCheckInCode, item.GuestSignature, item.GuestThumbPrint, item.Picture));
                        }
                    }
                }
                #endregion

                #endregion


                SearchedList = new List<DisplayDetails>();
                SearchedList = SearchListExtraDate;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - extraSearch()");
                //msg.ShowAsync();
            }
        }

        private void showGridSearchPopulated()
        {
            try
            {
                GridSearchSection.Visibility = Visibility.Visible;
                GridPopulateSection.Visibility = Visibility.Collapsed;
                btnCloseSearch.Visibility = Visibility.Visible;

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - showGridSearchPopulated()");
                //msg.ShowAsync();
            }
        }

        private void hideGridSearchPopulated()
        {
            try
            {
                GridSearchSection.Visibility = Visibility.Collapsed;
                GridPopulateSection.Visibility = Visibility.Visible;
                btnCloseSearch.Visibility = Visibility.Collapsed;
                chbAllItemChecked();

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - hideGridSearchPopulated()");
                //msg.ShowAsync();
            }
        }

        private void chbAll_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chbEmail.IsChecked == false || chbFullName.IsChecked == false || chbPhoneNumber.IsChecked == false || chbCompany.IsChecked == false || chbHostName.IsChecked == false)
                {
                    chbAllItemChecked();
                }
                else if (chbEmail.IsChecked == true && chbFullName.IsChecked == true && chbPhoneNumber.IsChecked == true && chbCompany.IsChecked == true && chbHostName.IsChecked == true)
                {
                    //Headers
                    chbFullName.IsChecked = false;
                    chbHostName.IsChecked = false;
                    chbCompany.IsChecked = false;
                    chbEmail.IsChecked = false;
                    chbPhoneNumber.IsChecked = false;
                    chbDate.IsChecked = false;
                    chbInvitationCode.IsChecked = false;
                    chbStatus.IsChecked = false;

                    //For Status
                    chbStatusCheckIn.IsChecked = true;
                    chbStatusCheckOut.IsChecked = false;

                    //For Invitation Code
                    chbInvitationCodeNo.IsChecked = true;
                    chbInvitationCodeYes.IsChecked = false;
                }
                chbItemChecked_Checked(sender, e);
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - chbAll_Checked()");
                //msg.ShowAsync();
            }
        }

        private void chbItemChecked_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                //For Major Headers
                if (chbEmail.IsChecked == true && chbFullName.IsChecked == true && chbPhoneNumber.IsChecked == true && chbCompany.IsChecked == true && chbHostName.IsChecked == true
                    && chbInvitationCode.IsChecked == true && chbStatus.IsChecked == true && chbDate.IsChecked == true)
                {
                    chbAll.IsChecked = true;
                    btnFlterSearch.Background = new SolidColorBrush(Colors.Transparent);
                }
                else
                {
                    chbAll.IsChecked = false;
                    btnFlterSearch.Background = new SolidColorBrush(Colors.CadetBlue);
                }

                CheckBox checkedBox = sender as CheckBox;

                #region For Status
                //For Status
                if (checkedBox == chbStatus && chbStatus.IsChecked == false)
                {
                    chbStatusCheckIn.IsChecked = true;
                    chbStatusCheckOut.IsChecked = false;
                }
                else if (checkedBox == chbStatus && chbStatus.IsChecked == true)
                {
                    chbStatusCheckIn.IsChecked = true;
                    chbStatusCheckOut.IsChecked = true;
                }

                #endregion

                #region Invitation Code
                if (checkedBox == chbInvitationCode && chbInvitationCode.IsChecked == false)
                {
                    chbInvitationCodeNo.IsChecked = true;
                    chbInvitationCodeYes.IsChecked = false;
                }
                else if (checkedBox == chbInvitationCode && chbInvitationCode.IsChecked == true)
                {
                    chbInvitationCodeNo.IsChecked = true;
                    chbInvitationCodeYes.IsChecked = true;
                }

                #endregion

                #region For Date
                if (checkedBox == chbDate && chbDate.IsChecked == false)
                {
                    chbDateFromTo.IsChecked = true;
                    chbDateFromYesterday.IsChecked = false;
                    chbDateThisWeek.IsChecked = false;
                    chbDateThisMonth.IsChecked = false;
                    chbDateThisYear.IsChecked = false;
                    chbDateAllTime.IsChecked = false;
                }
                else if (checkedBox == chbDate && chbDate.IsChecked == true)
                {
                    chbDateFromTo.IsChecked = false;
                    chbDateFromYesterday.IsChecked = false;
                    chbDateThisWeek.IsChecked = false;
                    chbDateThisMonth.IsChecked = false;
                    chbDateThisYear.IsChecked = false;
                    chbDateAllTime.IsChecked = true;
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - chbItem()");
                //msg.ShowAsync();
            }
        }

        private void chbAllItemChecked()
        {
            try
            {
                chbAll.IsChecked = true;
                chbFullName.IsChecked = true;
                chbHostName.IsChecked = true;
                chbCompany.IsChecked = true;
                chbEmail.IsChecked = true;
                chbPhoneNumber.IsChecked = true;
                chbDate.IsChecked = true;
                chbInvitationCode.IsChecked = true;
                chbStatus.IsChecked = true;

                //For Status
                chbStatusCheckIn.IsChecked = true;
                chbStatusCheckOut.IsChecked = true;

                //For Invitation Code
                chbInvitationCodeNo.IsChecked = true;
                chbInvitationCodeYes.IsChecked = true;

                stackPanelFilter.Visibility = Visibility.Collapsed;
                btnFlterSearch.Background = new SolidColorBrush(Colors.Transparent);
                //chbAll.IsChecked = true;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - chbAllItemChecked()");
                //msg.ShowAsync();
            }
        }

        #region When ExtraSearch() is used
        private void chbStatusItemChecked_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chbStatusCheckOut.IsChecked == false && chbStatusCheckIn.IsChecked == false)
                {
                    CheckBox checkedBox = sender as CheckBox;
                    checkedBox.IsChecked = true;

                    chbStatus.IsChecked = false;
                }
                else if (chbStatusCheckOut.IsChecked == false || chbStatusCheckIn.IsChecked == false)
                {
                    chbStatus.IsChecked = false;
                }
                else if (chbStatusCheckOut.IsChecked == true && chbStatusCheckIn.IsChecked == true)
                {
                    chbStatus.IsChecked = true;
                }
                chbItemChecked_Checked(sender, e);
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - chbStatusItemChceked_Checked()");
                //msg.ShowAsync();
            }
        }

        private void chbInvitationCodeItemChecked_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chbInvitationCodeNo.IsChecked == false && chbInvitationCodeYes.IsChecked == false)
                {
                    CheckBox checkedBox = sender as CheckBox;
                    checkedBox.IsChecked = true;
                    chbInvitationCode.IsChecked = false;
                }
                else if (chbInvitationCodeNo.IsChecked == false || chbInvitationCodeYes.IsChecked == false)
                {
                    chbInvitationCode.IsChecked = false;
                }
                else if (chbInvitationCodeNo.IsChecked == true && chbInvitationCodeYes.IsChecked == true)
                {
                    chbInvitationCode.IsChecked = true;
                }
                chbItemChecked_Checked(sender, e);
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - chbInvitationCodeItemChecked_Checked");
                //msg.ShowAsync();
            }
        }

        private void chbDateItemChecked_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                chbDate.IsChecked = false;
                chbDateFromTo.IsChecked = false;
                chbDateFromYesterday.IsChecked = false;
                chbDateThisWeek.IsChecked = false;
                chbDateThisMonth.IsChecked = false;
                chbDateThisYear.IsChecked = false;
                chbDateAllTime.IsChecked = false;

                CheckBox checkedBox = sender as CheckBox;
                checkedBox.IsChecked = true;

                if (checkedBox == chbDateAllTime)
                {
                    chbDate.IsChecked = true;
                }
                chbItemChecked_Checked(sender, e);
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - chbDateItemChecked_Checked");
                //msg.ShowAsync();
            }
        }
        #endregion

        private void LstPopulate_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (LstPopulate.Items.Count > 0)
                {
                    LstPopulate.SelectedItem = e.ClickedItem;
                    selectedItem();
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - LstPopulate_ItemClick");
                //msg.ShowAsync();
            }
        }

        private void LstSearch_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var Guest = (DisplayDetails)e.ClickedItem;

                int selectedIndex = 0;
                foreach (var item in DisplayGuestList)
                {
                    if (Guest.GuestFullName == item.GuestFullName)
                    {
                        LstPopulate.SelectedIndex = selectedIndex;
                        LstPopulate.ScrollIntoView(LstPopulate.SelectedItem);
                        hideGridSearchPopulated();
                        LstPopulate_ItemClick(sender, e);
                        return;
                    }
                    selectedIndex += 1;
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - LstSearch_ItemClick");
                //msg.ShowAsync();
            }
        }

        private void selectedItem()
        {
            try
            {
                var Guest = (DisplayDetails)LstPopulate.SelectedItem;

                txtClickedItemGuestFullname.Text = Guest.GuestFullName;
                txtClickedItemGuestCompany.Text = Guest.GuestCompany;
                txtClickedItemGuestEmail.Text = Guest.GuestEmail;
                txtClickedItemGuestHostName.Text = Guest.GuestHostName;
                txtClickedItemGuestPhoneNumber.Text = Guest.GuestPhoneNumber;
                txtClickedItemGuestCheckIn.Text = Guest.GuestCheckInTime.ToString();
                txtClickedItemGuestCheckOut.Text = Guest.GuestCheckOutTime.ToString();
                txtClickedItemGuestStatus.Text = Guest.GuestStatus;

                ImgClickedItemGuestPicture.Source = Guest.Picture;

                if (Guest.GuestCheckInTime == Guest.GuestCheckOutTime)
                {
                    stackGuestCheckOut.Visibility = Visibility.Collapsed;
                    txtClickedItemGuestStatus.Foreground = new SolidColorBrush(Colors.Red);
                    btnCheckOut.Visibility = Visibility.Visible;
                }
                else
                {
                    stackGuestCheckOut.Visibility = Visibility.Visible;
                    txtClickedItemGuestStatus.Foreground = new SolidColorBrush(Colors.Black);
                    btnCheckOut.Visibility = Visibility.Collapsed;
                }

                gridGuestDetail.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - selectedItem");
                //msg.ShowAsync();
            }
        }

        private void btnGuestDetailClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                gridGuestDetail.Visibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnGuestDetailClose_Click");
                //msg.ShowAsync();
            }
        }

        private void btnCloseSearch_Click(object sender, RoutedEventArgs e)
        {
            //showPop();
            try
            {
                hideGridSearchPopulated();

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnCloseSearch_Click");
                //msg.ShowAsync();
            }
        }

        private void txtSearchItem_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                if (e.Key == VirtualKey.Enter)
                {
                    btnPopulateGridfromSearch_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msg = new MessageDialog(ex.Message + " Void - txtSearchItem_Keydown");
                //msg.ShowAsync();
            }
        }

        private void btnShowCheckedIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var checkedinList = new List<DisplayDetails>();
                var serialNo = 0;
                foreach (var item in DisplayGuestList)
                {
                    if (item.GuestCheckInTime == item.GuestCheckOutTime)
                    {
                        serialNo += 1;
                        checkedinList.Add((AddItemDetail(serialNo.ToString(), item.GuestFullName, item.GuestCompany, item.GuestEmail, item.GuestPhoneNumber, item.GuestPhoneNumber,
                                item.GuestCheckInTime, item.GuestCheckOutTime, item.GuestStatus, item.GuestCheckInCode, item.GuestSignature, item.GuestThumbPrint, item.Picture)));
                    }
                }

                LstPopulate.ItemsSource = null;
                LstPopulate.ItemsSource = checkedinList;
                txtPopulateNumber.Text = LstPopulate.Items.Count().ToString();
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnShowCheckedIn_Click");
                //msg.ShowAsync();
            }
        }

        private void btnShowCheckedOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var checkedOutList = new List<DisplayDetails>();
                var serialNo = 0;
                foreach (var item in DisplayGuestList)
                {
                    if (item.GuestCheckInTime != item.GuestCheckOutTime)
                    {
                        serialNo += 1;
                        checkedOutList.Add((AddItemDetail(serialNo.ToString(), item.GuestFullName, item.GuestCompany, item.GuestEmail, item.GuestPhoneNumber, item.GuestPhoneNumber,
                                item.GuestCheckInTime, item.GuestCheckOutTime, item.GuestStatus, item.GuestCheckInCode, item.GuestSignature, item.GuestThumbPrint, item.Picture)));
                    }
                }

                LstPopulate.ItemsSource = null;
                LstPopulate.ItemsSource = checkedOutList;
                txtPopulateNumber.Text = LstPopulate.Items.Count().ToString();
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnShowCheckedOut_Click");
                //msg.ShowAsync();
            }
        }

        private void btnShowAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LstPopulate.ItemsSource = null;
                LstPopulate.ItemsSource = DisplayGuestList;
                txtPopulateNumber.Text = LstPopulate.Items.Count().ToString();
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnShowAll_Click");
                //msg.ShowAsync();
            }
        }

        private void menuButton(object sender)
        {
            try
            {
                var menuBtn = sender as MenuFlyoutItem;
                btnSort.Content = menuBtn.Text;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message, "Void - MenuButton");
                //msg.ShowAsync();
            }
        }

        private void btnCheckOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string txtPhoneNumber = txtClickedItemGuestPhoneNumber.Text;
                _activePage.LogOutGuestphoneNumber = txtPhoneNumber;
                this.Frame.Navigate(typeof(CheckOutGuest), _activePage);
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnCheckedOut_Click");
                //msg.ShowAsync();
            }
        }

        private void btnShowTodayGuest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var TodayGuestList = new List<DisplayDetails>();
                var serialNo = 0;
                foreach (var item in DisplayGuestList)
                {
                    if (item.GuestCheckInTime.Date == DateTime.Now.Date)
                    {
                        serialNo += 1;
                        TodayGuestList.Add((AddItemDetail(serialNo.ToString(), item.GuestFullName, item.GuestCompany, item.GuestEmail, item.GuestPhoneNumber, item.GuestHostName,
                                item.GuestCheckInTime, item.GuestCheckOutTime, item.GuestStatus, item.GuestCheckInCode, item.GuestSignature, item.GuestThumbPrint, item.Picture)));
                    }
                }

                LstPopulate.ItemsSource = null;
                LstPopulate.ItemsSource = TodayGuestList;
                txtPopulateNumber.Text = LstPopulate.Items.Count().ToString();
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnShowTodayGuest_Click");
                //msg.ShowAsync();
            }
        }

        private void btnSortCheckedIn_Click(object sender, RoutedEventArgs e)
        {
            btnShowCheckedIn_Click(sender, e);
            menuButton(sender);
        }

        private void btnSortCheckedOut_Click(object sender, RoutedEventArgs e)
        {
            btnShowCheckedOut_Click(sender, e);
            menuButton(sender);
        }

        private void btnSortShowAll_Click(object sender, RoutedEventArgs e)
        {
            btnShowAll_Click(sender, e);
            menuButton(sender);
        }

        private void btnSortTodayGuest_Click(object sender, RoutedEventArgs e)
        {
            btnShowTodayGuest_Click(sender, e);
            menuButton(sender);
        }

        private void txbUserName_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(userProfile), _activePage);
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
