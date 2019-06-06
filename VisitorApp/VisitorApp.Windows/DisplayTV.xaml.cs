using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VisitorApp.Common;
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

namespace VisitorApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DisplayTV : Page
    {
        private const string StatusCheckedInText = "Checked In";
        private const string StatusCheckedOutText = "Checked Out";

        private const string InvitationCodeNoText = "";

        public List<VisitorGlobal> VisitorList { get; set; }
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

        #endregion

        public DisplayTV()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _activePage = (AccountDetails)e.Parameter;
            if (_activePage != null)
            {
                txbTitle.Text = ("Welcome To " + _activePage.CompanyName).ToUpper();
                txbUserName.Text = _activePage.UserStaffName;
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), _activePage);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                getData();
                btnShowTodayGuest_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - Page_Loaded");
                //msg.ShowAsync();
            }
        }

        private void getData()
        {
            try
            {
                VisitorList = new List<VisitorGlobal>();
                guestList = new List<GuestGlobal>();

                RemoteService service = new RemoteService();
                VisitorDataPayLoad payload = new VisitorDataPayLoad();

                var response = service.VisitorListControllerService(payload);
                VisitorList = response.VisitorList;

                var response2 = service.GuestListControllerService(payload);
                guestList = response2.GuestList;

                DisplayGuestList = new List<DisplayDetails>();
                var serialNo = 0;

                foreach (var item in guestList)
                {
                    BitmapImage PhotoCopy = null;
                    string guestPhotoString = null;
                    string guestPhoneNumber = null;
                    string guestCompany = null;
                    string guestEmail = null;
                    string guestSignature = null;
                    string guestThumbnail = null;

                    foreach (var visitor in VisitorList)
                    {
                        if (item.VisitorId == visitor.VisitorId)
                        {
                            guestPhotoString = visitor.photoString;
                            guestCompany = visitor.CompanyName;
                            guestPhoneNumber = visitor.phoneNumber;
                            guestEmail = visitor.emailAddress;
                            guestSignature = visitor.Signature;
                            guestThumbnail = visitor.ThumbPrint;
                        }
                    }

                    if (guestPhotoString == null)
                    {
                        PhotoCopy.UriSource = new Uri(@"/Assets/no image found.jpeg");
                    }
                    else
                    {
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
                    }

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
                MessageDialog msg = new MessageDialog(ex.Message + " Void - getData()");
                //msg.ShowAsync();
            }

            //populateGrid();
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
                txtClickedItemGuestPhoneNumber.Text = Guest.GuestPhoneNumber;
                txtClickedItemGuestHostName.Text = Guest.GuestHostName;
                txtClickedItemGuestCheckIn.Text = Guest.GuestCheckInTime.Date.ToString();
                txtClickedItemGuestCheckOut.Text = Guest.GuestCheckOutTime.Date.ToString();
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

        private void btnCheckOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string txtPhoneNumber = txtClickedItemGuestPhoneNumber.Text;
                _activePage.LogOutGuestphoneNumber = txtPhoneNumber;
                this.Frame.Navigate(typeof(CheckOutPage), _activePage);
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
                        TodayGuestList.Add((AddItemDetail(serialNo.ToString(), item.GuestFullName, item.GuestCompany, item.GuestEmail, item.GuestPhoneNumber, item.GuestPhoneNumber,
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
    }
}
