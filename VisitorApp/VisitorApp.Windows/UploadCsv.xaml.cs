using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisitorApp.Common;
using VisitorApp.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI;
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
    /// 

    public class serialize
    {
        public string sn { get; set; }
    }

    public sealed partial class UploadCsv : Page
    {
        private const char DEFAULT_DELIMITER = ',';
        private const char DEFAULT_LINE_DELIMITER = '\n';

        private const string StatusCheckedInText = "Checked In";
        private const string StatusCheckedOutText = "Checked Out";

        private const string InvitationCodeNoText = "No";
        private const string InvitationCodeYesText = "Yes";

        public char Delimiter { get; set; }
        public char LineDelimiter { get; set; }
        public string RawText { get; set; }
        public bool HasHeaderRow { get; set; }

        int sn;
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Company { get; set; }

        List<CsvUsersDetails> SearchedList { get; set; }
        List<string> _header;
        List<string> Header;

        public double Angle;

        int fullnameIndex;
        int companyIndex;
        int emailIndex;
        int phoneNumberIndex;

        List<CsvUsersDetails> UserDetail;

        private bool continueValidation = false;
        AccountDetails _activePage = new AccountDetails();

        public UploadCsv()
        {
            this.InitializeComponent();
            this.Delimiter = DEFAULT_DELIMITER;
            this.LineDelimiter = DEFAULT_LINE_DELIMITER;
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), _activePage);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _activePage = (AccountDetails)e.Parameter;
            if (_activePage != null)
            {
                txbTitle.Text = (_activePage.CompanyName).ToUpper();
                txbUserName.Text = _activePage.UserStaffName;
            }
        }

        private async void SelectCSVFile()
        {
            try
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                openPicker.FileTypeFilter.Add(".csv");

                string testString = null;

                StorageFile file = await openPicker.PickSingleFileAsync();

                if (file != null)
                {
                    //LstPopulate.Items.Clear();
                    LstPopulate.ItemsSource = null;
                    txtPopulateNumber.Text = "";
                    //listViewSN.Items.Clear();

                    var stream = await file.OpenAsync(FileAccessMode.Read);
                    using (StreamReader reader = new StreamReader(stream.AsStream()))
                    {
                        testString = reader.ReadToEnd();
                    }

                    HasHeaderRow = true;
                    RawText = testString;
                    Header = LoadFieldNamesFromHeaderRow();

                    populateComboBox();
                    resetGridItemDetail();
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - SelectCSVFile");
                //msg.ShowAsync();
            }
        }

        private void populateGrid()
        {
            try
            {
                List<string> Header = LoadFieldNamesFromHeaderRow();
                List<Dictionary<string, string>> Body = Parse();

                UserDetail = new List<CsvUsersDetails>();

                string body = null;

                sn = 0;

                foreach (var item in Body)
                {
                    string _body = null;

                    if (item.Keys.Count == Header.Count)
                    {
                        var content = item.Values.ToList();

                        FullName = content[fullnameIndex].ToString().Trim();
                        Company = content[companyIndex].ToString().Trim();
                        Email = content[emailIndex].ToString().Trim();
                        PhoneNumber = content[phoneNumberIndex].ToString().Trim();

                        sn += 1;

                        CreateGridViaBinding();
                    }
                }

                LstPopulate.ItemsSource = UserDetail;

                //FileText.Text = body;

                showComboBoxStack();
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - populateGrid()");
                //msg.ShowAsync();
            }
        }

        private void btnSelectCSVFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectCSVFile();
                gridSearchSection.Visibility = Visibility.Collapsed;
                stackComboBox.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnSelectCSVFile");
                //msg.ShowAsync();
            }
        }

        public List<Dictionary<string, string>> Parse()
        {
            List<Dictionary<string, string>> parsedResult = new List<Dictionary<string, string>>();
            string[] records = RawText.Split(this.LineDelimiter);

            int startingRow = 0;
            List<string> fieldList = new List<string>();

            if (this.HasHeaderRow)
            {
                startingRow = 1;
                fieldList = LoadFieldNamesFromHeaderRow();
            }

            for (int i = startingRow; i < records.Length; i++)
            {
                string record = records[i];

                string[] fields = record.Split(this.Delimiter);
                Dictionary<string, string> recordItem = new Dictionary<string, string>();

                int fieldIncrementer = 0;


                foreach (var field in fields)
                {
                    string key = fieldIncrementer.ToString();

                    if (this.HasHeaderRow)
                    {
                        if (fields.Length == fieldList.Count)
                        {
                            key = fieldList[fieldIncrementer];
                        }
                    }

                    recordItem.Add(key, field);
                    fieldIncrementer++;

                }

                parsedResult.Add(recordItem);
            }

            return parsedResult;
        }

        private List<string> LoadFieldNamesFromHeaderRow()
        {
            var fieldList = new List<string>();

            var firstLine = this.RawText.Split(this.LineDelimiter).FirstOrDefault();

            if (firstLine != null)
            {
                fieldList = firstLine.TrimEnd().Split(this.Delimiter).ToList();
            }

            return fieldList;
        }

        private void CreateGridViaBinding()
        {
            try
            {
                #region Check Phone Number

                string _UserPhoneNumber = null;
                string value = null;

                string _PhoneNumberValue = PhoneNumber.ToLower();

                if (_PhoneNumberValue.Contains("e+") == true && _PhoneNumberValue[0].ToString() != "e") //
                {
                    var positionNumber = 0;
                    foreach (char character in _PhoneNumberValue)
                    {
                        if (character.ToString() == "e")
                        {
                            break;
                        }
                        positionNumber += 1;
                    }
                    //var power = ui.Length - (sn + 2); // The length of Power; "E+" is two Char

                    string number = _PhoneNumberValue.Substring(0, positionNumber);
                    string toPower = _PhoneNumberValue.Substring(positionNumber + 2);//, power); //it's E+ ie two chars

                    bool numberAllNumber = true;
                    bool toPowerAllNumber = true;

                    for (int i = 0; i < number.Length - 1; i++)
                    {
                        if ((char.IsNumber(number[i])) || (number[i].ToString() == "."))
                        {
                            numberAllNumber = true;
                        }
                        else
                        {
                            numberAllNumber = false;
                            break;
                        }
                    }

                    for (int i = 0; i < toPower.Length - 1; i++)
                    {
                        if ((char.IsNumber(toPower[i])) || (toPower[i].ToString() == "."))
                        {
                            toPowerAllNumber = true;
                        }
                        else
                        {
                            toPowerAllNumber = false;
                            break;
                        }
                    }

                    if (numberAllNumber == true && toPowerAllNumber == true)
                    {
                        var valueNumber = Convert.ToDouble(number) * Math.Pow(10, Convert.ToDouble(toPower)); //(10 ^ Convert.ToInt64(toPower));
                        value = valueNumber.ToString();
                    }
                    else
                    {
                        value = _PhoneNumberValue;
                    }
                    _UserPhoneNumber = value;
                }
                else if (_PhoneNumberValue.Contains("+") == true && _PhoneNumberValue[0].ToString() == "+")
                {
                    value = _PhoneNumberValue.Substring(1);
                    _UserPhoneNumber = value;
                }
                else
                {
                    _UserPhoneNumber = _PhoneNumberValue;
                }

                #endregion

                UserDetail.Add(new CsvUsersDetails
                {
                    sn = sn.ToString(),
                    Fullname = FullName,
                    Email = Email,
                    Company = Company,
                    PhoneNumber = _UserPhoneNumber
                });
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - createGridBinding");
                //msg.ShowAsync();
            }
        }

        private void LstPopulate_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (LstPopulate.Items.Count > 0)
            {
                LstPopulate.SelectedItem = e.ClickedItem;
                selectedItem();
                GridUserDetailOpen();
            }
        }

        private void LstSearch_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var user = (CsvUsersDetails)e.ClickedItem;

                int selectedIndex = 0;
                foreach (var item in UserDetail)
                {
                    if (user.Fullname == item.Fullname)
                    {
                        LstPopulate.SelectedIndex = selectedIndex;
                        LstPopulate.ScrollIntoView(LstPopulate.SelectedItem);
                        hideGridSearchPoulated();
                        LstPopulate_ItemClick(sender, e);
                        return;
                    }
                    selectedIndex += 1;
                }
            }
            catch (Exception ex)
            {
                //Do not. The only possible error is when Item is "NO RESULT FOUND".
            }
        }

        private void populateComboBox()
        {
            clearComboBoxes();

            foreach (var head in Header)
            {
                cmbxName.Items.Add(head);
                cmbxCompany.Items.Add(head);
                cmbxEmail.Items.Add(head);
                cmbxPhoneNumber.Items.Add(head);
            }
            autoMatchComboBox();
        }

        private void autoMatchComboBox()
        {
            try
            {
                for (int i = 0; i < Header.Count; i++)
                {
                    string textHeader = Header[i].ToString().Trim().ToLower();
                    if (textHeader.Contains("name"))
                    {
                        cmbxName.SelectedIndex = i;
                    }
                    else if (textHeader.Contains("company"))
                    {
                        cmbxCompany.SelectedIndex = i;
                    }
                    else if (textHeader.Contains("email"))
                    {
                        cmbxEmail.SelectedIndex = i;
                    }
                    else if (textHeader.Contains("phone"))
                    {
                        cmbxPhoneNumber.SelectedIndex = i;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - autoMatchComboBox");
                //msg.ShowAsync();
            }
        }

        private void checkPopulateComboBox(object sender)
        {
            try
            {
                cmbxName.Background = new SolidColorBrush(Colors.Transparent);
                cmbxCompany.Background = new SolidColorBrush(Colors.Transparent);
                cmbxEmail.Background = new SolidColorBrush(Colors.Transparent);
                cmbxPhoneNumber.Background = new SolidColorBrush(Colors.Transparent);
                btnPopulateGrid.Visibility = Visibility.Visible;
                txbError.Visibility = Visibility.Collapsed;

                if (cmbxName.SelectedItem != null && cmbxCompany.SelectedItem != null && cmbxEmail.SelectedItem != null && cmbxPhoneNumber.SelectedItem != null)
                {
                    #region Check 2

                    #region Check Company
                    if (cmbxCompany.SelectedItem.ToString() == cmbxName.SelectedItem.ToString())
                    {
                        cmbxName.Background = new SolidColorBrush(Colors.Red);
                        cmbxCompany.Background = new SolidColorBrush(Colors.Red);
                        txbError.Text = "Remove Repeated Field at 'Company' and 'Name'";
                        txbError.Visibility = Visibility.Visible;
                        btnPopulateGrid.Visibility = Visibility.Collapsed;
                    }

                    if (cmbxCompany.SelectedItem.ToString() == cmbxEmail.SelectedItem.ToString())
                    {
                        cmbxEmail.Background = new SolidColorBrush(Colors.Red);
                        cmbxCompany.Background = new SolidColorBrush(Colors.Red);
                        txbError.Text = "Remove Repeated Field at 'Email' and 'Company'";
                        txbError.Visibility = Visibility.Visible;
                        btnPopulateGrid.Visibility = Visibility.Collapsed;
                        //return;
                    }

                    if (cmbxCompany.SelectedItem.ToString() == cmbxPhoneNumber.SelectedItem.ToString())
                    {
                        cmbxPhoneNumber.Background = new SolidColorBrush(Colors.Red);
                        cmbxCompany.Background = new SolidColorBrush(Colors.Red);
                        txbError.Text = ("Remove Repeated Field at 'Company' and 'Phone Number'");
                        txbError.Visibility = Visibility.Visible;
                        btnPopulateGrid.Visibility = Visibility.Collapsed;
                        //return;
                    }
                    #endregion

                    #region Check Email
                    if (cmbxEmail.SelectedItem.ToString() == cmbxName.SelectedItem.ToString())
                    {
                        cmbxName.Background = new SolidColorBrush(Colors.Red);
                        cmbxEmail.Background = new SolidColorBrush(Colors.Red);
                        txbError.Text = ("Remove Repeated Field at 'Email' and 'Name'");
                        txbError.Visibility = Visibility.Visible;
                        btnPopulateGrid.Visibility = Visibility.Collapsed;
                        //return;
                    }

                    if (cmbxEmail.SelectedItem.ToString() == cmbxPhoneNumber.SelectedItem.ToString())
                    {
                        cmbxPhoneNumber.Background = new SolidColorBrush(Colors.Red);
                        cmbxEmail.Background = new SolidColorBrush(Colors.Red);
                        txbError.Text = ("Remove Repeated Field at 'Email' and 'Phone Number'");
                        txbError.Visibility = Visibility.Visible;
                        btnPopulateGrid.Visibility = Visibility.Collapsed;
                        //return;
                    }
                    #endregion

                    #region Check Phone Number
                    if (cmbxName.SelectedItem.ToString() == cmbxPhoneNumber.SelectedItem.ToString())
                    {
                        cmbxName.Background = new SolidColorBrush(Colors.Red);
                        cmbxPhoneNumber.Background = new SolidColorBrush(Colors.Red);
                        txbError.Text = ("Remove Repeated Field at 'Name' and 'Phone Number'");
                        txbError.Visibility = Visibility.Visible;
                        btnPopulateGrid.Visibility = Visibility.Collapsed;
                        //return;
                    }
                    #endregion

                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - checkPopulateComboBox");
                //msg.ShowAsync();
            }
        }

        private void cmbxItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            checkPopulateComboBox(sender);
        }

        private void clearComboBoxes()
        {
            try
            {
                cmbxName.Items.Clear();
                cmbxEmail.Items.Clear();
                cmbxPhoneNumber.Items.Clear();
                cmbxCompany.Items.Clear();
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - clearComboBoxes");
                //msg.ShowAsync();
            }
        }

        private void checkComboBoxNoRepeat()
        {
            try
            {
                if ((cmbxName.Background == new SolidColorBrush(Colors.Red)) || (cmbxCompany.Background == new SolidColorBrush(Colors.Red))
            || (cmbxEmail.Background == new SolidColorBrush(Colors.Red)) || (cmbxPhoneNumber.Background == new SolidColorBrush(Colors.Red))
            )
                {
                    txbError.Text = ("Remove Repeated Field");
                    txbError.Visibility = Visibility.Visible;
                    return;
                }

                else
                {
                    txbError.Visibility = Visibility.Collapsed;
                    LstPopulate.Items.Clear();
                    populateGrid();
                }

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - checkComboBoxNoRepeat");
                //msg.ShowAsync();
            }
        }

        private void btnPopulateGrid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbxName.SelectedItem != null && cmbxCompany.SelectedItem != null && cmbxEmail.SelectedItem != null && cmbxPhoneNumber.SelectedItem != null)
                {
                    fullnameIndex = cmbxName.SelectedIndex;
                    companyIndex = cmbxCompany.SelectedIndex;
                    emailIndex = cmbxEmail.SelectedIndex;
                    phoneNumberIndex = cmbxPhoneNumber.SelectedIndex;

                    LstPopulate.ItemsSource = null;
                    //FileText.Text = "";
                    populateGrid();
                    txtPopulateNumber.Text = LstPopulate.Items.Count().ToString();
                    gridSearchSection.Visibility = Visibility.Visible;
                    stackPanelFilter.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageDialog msg = new MessageDialog("Empty Field(s)");
                    msg.ShowAsync();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnPopulateGrid_Click");
                //msg.ShowAsync();
            }
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                showComboBoxStack();
                btnCloseSearch_Click(sender, e);
                resetGridItemDetail();
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnShow_Click");
                //msg.ShowAsync();
            }
        }

        private void showComboBoxStack()
        {
            try
            {
                if (stackComboBox.Visibility == Visibility.Visible)
                {
                    stackComboBox.Visibility = Visibility.Collapsed;
                    btnShow.Content = "v";
                }
                else if (stackComboBox.Visibility == Visibility.Collapsed)
                {
                    stackComboBox.Visibility = Visibility.Visible;
                    btnShow.Content = "^";
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - showComboBoxStack");
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
                MessageDialog msg = new MessageDialog(ex.Message);
                //await msg.ShowAsync();
            }
        }

        private void searchedItems()
        {

            try
            {
                #region USing Binding to populate
                SearchedList = new List<CsvUsersDetails>();

                if (UserDetail.Count != 0)
                {
                    LstSearch.ItemsSource = null;

                    var serialNo = 0;
                    string searchItem = txtSearchItem.Text.Trim().ToLower();
                    foreach (var item in UserDetail)
                    {
                        FullName = item.Fullname;
                        Company = item.Company;
                        Email = item.Email;
                        PhoneNumber = item.PhoneNumber;

                        string _FullName = FullName.ToLower();
                        string _Company = Company.ToLower();
                        string _Email = Email.ToLower();
                        string _PhoneNumber = PhoneNumber.ToLower();

                        if (chbFullName.IsChecked == true && _FullName.Contains(searchItem))
                        {
                            serialNo += 1;

                            SearchedList.Add(AddItemDetail(serialNo.ToString(), FullName, Company, Email, PhoneNumber));
                        }

                        else if (chbCompany.IsChecked == true && _Company.Contains(searchItem))
                        {
                            serialNo += 1;

                            SearchedList.Add(AddItemDetail(serialNo.ToString(), FullName, Company, Email, PhoneNumber));
                        }

                        else if (chbEmail.IsChecked == true && _Email.Contains(searchItem))
                        {
                            serialNo += 1;

                            SearchedList.Add(AddItemDetail(serialNo.ToString(), FullName, Company, Email, PhoneNumber));
                        }

                        else if (chbPhoneNumber.IsChecked == true && _PhoneNumber.Contains(searchItem))
                        {
                            serialNo += 1;

                            SearchedList.Add(AddItemDetail(serialNo.ToString(), FullName, Company, Email, PhoneNumber));
                        }

                    }

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
                        hideGridSearchPoulated();
                        msg.ShowAsync();
                    }

                    //SerializeItems();

                    showGridSearchPoulated();
                    //showPop();
                }
                #endregion

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - searchedItems");
                //msg.ShowAsync();
            }
        }

        private CsvUsersDetails AddItemDetail(string sn, string Fullname, string Company, string Email, string PhoneNumber)
        {
            var AddItem = new CsvUsersDetails();
            AddItem.sn = sn;
            AddItem.Fullname = Fullname;
            AddItem.Company = Company;
            AddItem.Email = Email;
            AddItem.PhoneNumber = PhoneNumber;

            return AddItem;
        }

        private void showGridSearchPoulated()
        {
            try
            {
                GridSearchSection.Visibility = Visibility.Visible;
                GridPopulateSection.Visibility = Visibility.Collapsed;
                btnCloseSearch.Visibility = Visibility.Visible;

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - showGridSearchPopulated");
                //msg.ShowAsync();
            }
        }

        private void hideGridSearchPoulated()
        {
            try
            {
                GridSearchSection.Visibility = Visibility.Collapsed;
                GridPopulateSection.Visibility = Visibility.Visible;
                btnCloseSearch.Visibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - hideGridSearchPopulated");
                //msg.ShowAsync();
            }
        }

        private void resetGridDetailItems()
        {
            try
            {
                var resetColor = new SolidColorBrush(Colors.White);

                txtClickedItemFullname.Background = resetColor;
                txtClickedItemCompany.Background = resetColor;
                txtClickedItemPhoneNumber.Background = resetColor;
                txtClickedItemEmail.Background = resetColor;

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - resetGridDetaiItems");
                //msg.ShowAsync();
            }
        }

        private void chbAll_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chbEmail.IsChecked == false || chbFullName.IsChecked == false || chbPhoneNumber.IsChecked == false || chbCompany.IsChecked == false || chbHostName.IsChecked == false)
                {
                    chbFullName.IsChecked = true;
                    chbHostName.IsChecked = true;
                    chbCompany.IsChecked = true;
                    chbEmail.IsChecked = true;
                    chbPhoneNumber.IsChecked = true;

                    btnFlterSearch.Background = new SolidColorBrush(Colors.Transparent);
                    //chbAll.IsChecked = true;
                }
                else if (chbEmail.IsChecked == true && chbFullName.IsChecked == true && chbPhoneNumber.IsChecked == true && chbCompany.IsChecked == true && chbHostName.IsChecked == true)
                {
                    chbFullName.IsChecked = false;
                    chbHostName.IsChecked = false;
                    chbCompany.IsChecked = false;
                    chbEmail.IsChecked = false;
                    chbPhoneNumber.IsChecked = false;
                }
                chbItemChecked_Checked(sender, e);

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - chbAll_Checked");
                //msg.ShowAsync();
            }
        }

        private void chbItemChecked_Checked(object sender, RoutedEventArgs e)
        {
            //For Major Headers
            try
            {
                if (chbEmail.IsChecked == true && chbFullName.IsChecked == true && chbPhoneNumber.IsChecked == true && chbCompany.IsChecked == true && chbHostName.IsChecked == true)
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
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - chbItemChecked_Checked");
                //msg.ShowAsync();
            }

        }

        private void btnCloseSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                hideGridSearchPoulated();
                resetGridDetailItems();

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnClosSearch_Clicked");
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
                MessageDialog msg = new MessageDialog(ex.Message + " Void - txtSearchItem_KeyDown");
               // msg.ShowAsync();
            }
        }

        private async void checkSyntax()
        {
            try
            {
                string fullName = FullName;
                string email = Email;
                string company = Company;

                string txtPhoneNuber = PhoneNumber;

                bool stringFullname = false;
                bool stringEmail = false;
                bool stringCompany = false;
                bool stringPhoneNumber = false;


                #region Validate PhoneNumber
                if (PhoneNumber != null)
                {
                    for (int i = 0; i < PhoneNumber.Length; i++)
                    {
                        if (!char.IsNumber(txtPhoneNuber[i]))
                        {
                            continueValidation = false;
                            MessageDialog msg = new MessageDialog("The Phone Number contains non-numeric character; " + txtPhoneNuber[i]);
                            await msg.ShowAsync();
                            LstPopulate.SelectedIndex = 0;
                            LstPopulate.ScrollIntoView(LstPopulate.SelectedItem);
                            txtClickedItemPhoneNumber.Background = new SolidColorBrush(Colors.Red);
                            selectedItem();
                            return;
                        }
                    }
                }
                #endregion

                #region Validate Email
                if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                {
                    continueValidation = false;
                    MessageDialog msg = new MessageDialog("The email is invalid at Serial Number " + sn.ToString());
                    await msg.ShowAsync();
                    LstPopulate.SelectedIndex = 0;
                    txtClickedItemEmail.Background = new SolidColorBrush(Colors.Red);
                    selectedItem();
                    return;
                }
                #endregion

                #region Validate Full Name
                if (fullName != null)
                {
                    for (int i = 0; i < FullName.Length; i++)
                    {
                        if (!char.IsLetter(fullName[i]) && fullName[i].ToString() != "-" && fullName[i].ToString() != " ")
                        {
                            continueValidation = false;
                            MessageDialog msg = new MessageDialog("The Full Name contains an invalid character; " + fullName[i] + "\n" + "Please remove to proceed at Serial Number " + sn.ToString());
                            await msg.ShowAsync();
                            LstPopulate.SelectedIndex = 0;
                            txtClickedItemFullname.Background = new SolidColorBrush(Colors.Red);
                            selectedItem();
                            return;
                        }
                    }
                }
                #endregion
                //else
                //{
                long phoneNumber = Convert.ToInt64(PhoneNumber);

                MessageDialog msgbox = new MessageDialog(
                        "Full Name: " + FullName + "\n" +
                        "Company: " + Company + "\n" +
                        "email: " + Email + "\n" +
                        "Phone Number: " + PhoneNumber
                    );
                //msgbox.ShowAsync();

                var ser = sn - 1;
                //}

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - checkSyntax");
                //msg.ShowAsync();
            }
        }

        private void refreshData()
        {
            try
            {
                for (int i = 0; i < UserDetail.Count; i++)
                {
                    var user = UserDetail[i];

                    user.sn = (i + 1).ToString();
                    UserDetail.Insert(i, user);
                    UserDetail.RemoveAt(i + 1);
                }

                txtPopulateNumber.Text = LstPopulate.Items.Count().ToString();

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - refreshData");
                //msg.ShowAsync();
            }
        }

        private void selectedItem()
        {
            try
            {
                var user = (CsvUsersDetails)LstPopulate.SelectedItem;

                txtClickedItemFullname.Text = user.Fullname;
                txtClickedItemCompany.Text = user.Company;
                txtClickedItemEmail.Text = user.Email;
                txtClickedItemPhoneNumber.Text = user.PhoneNumber;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - selectedItem");
                //msg.ShowAsync();
            }
        }

        private void btnItemDetailClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GridItemDetail.Visibility = Visibility.Collapsed;
                GridEditItemDetail.Visibility = Visibility.Collapsed;
                resetClickItemBAckground();

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnItemDetailClose");
                //msg.ShowAsync();
            }
        }

        private void resetClickItemBAckground()
        {
            try
            {
                var resetBackground = new SolidColorBrush(Colors.White);

                txtClickedItemFullname.Background = resetBackground;
                txtClickedItemCompany.Background = resetBackground;
                txtClickedItemEmail.Background = resetBackground;
                txtClickedItemPhoneNumber.Background = resetBackground;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " resetClickItemBackground");
                //msg.ShowAsync();
            }
        }

        private void resetClickItemText()
        {
            try
            {
                txtClickedItemFullname.Text = "";
                txtClickedItemCompany.Text = "";
                txtClickedItemEmail.Text = "";
                txtClickedItemPhoneNumber.Text = "";
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " resetClickItemText");
                //msg.ShowAsync();
            }
        }

        private void updateGridItem()
        {
            try
            {
                var selectedIndex = LstPopulate.SelectedIndex;
                var newUserDetail = AddItemDetail((selectedIndex + 1).ToString(), txtClickedItemFullname.Text, txtClickedItemCompany.Text, txtClickedItemEmail.Text, txtClickedItemPhoneNumber.Text);

                UserDetail.Insert(selectedIndex, newUserDetail);
                UserDetail.RemoveAt(selectedIndex + 1);

                LstPopulate.ItemsSource = null;
                LstPopulate.ItemsSource = UserDetail;

                GridEditItemDetail.Visibility = Visibility.Collapsed;
                GridItemDetail.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " updateGridItem");
                //msg.ShowAsync();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            updateGridItem();
        }

        private void resetGridItemDetail()
        {
            try
            {
                GridEditItemDetail.Visibility = Visibility.Collapsed;
                GridItemDetail.Visibility = Visibility.Collapsed;
                resetClickItemBAckground();
                resetClickItemText();

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " updateGridItem");
                //msg.ShowAsync();
            }
        }

        private void GridUserDetailOpen()
        {
            GridItemDetail.Visibility = Visibility.Visible;
        }

        private void btnEditUserDetails_Click(object sender, RoutedEventArgs e)
        {
            GridEditItemDetail.Visibility = Visibility.Visible;
        }

        private void btnDeleteUserDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedIndex = LstPopulate.SelectedIndex;
                UserDetail.RemoveAt(selectedIndex);

                refreshData();
                LstPopulate.ItemsSource = null;
                LstPopulate.ItemsSource = UserDetail;

                GridEditItemDetail.Visibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " btnDeleteUserDetails_Click");
                //msg.ShowAsync();
            }
        }

        private void btnPushFill_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtEventName.Text.Trim() != "" && txtInvitationCode.Text.Trim() != "")
                {
                    fileCsvToDb();
                }
                else
                {
                    MessageDialog msg = new MessageDialog("Please, ensure that Invitation Code and Event Name are entered");
                    msg.ShowAsync();
                }

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " btnPushFill_Click");
                //msg.ShowAsync();
            }
        }

        private async void fileCsvToDb()
        {
            try
            {
                var ity = LstPopulate.Items.Count();
                continueValidation = true;

                for (int i = 0; i < ity; i++)
                {
                    if (continueValidation == true)
                    {
                        var user = UserDetail[0];

                        FullName = user.Fullname;
                        Company = user.Company;
                        Email = user.Email;
                        PhoneNumber = user.PhoneNumber;

                        sn = i + 1;

                        checkSyntax();

                        RemoteService service = new RemoteService();
                        VisitorDataPayLoad payload = new VisitorDataPayLoad
                        {
                            PhoneNumber = Convert.ToInt64(PhoneNumber)
                        };

                        var response = await service.CheckIfVisitorExistService(payload);


                        if (response.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                        {
                            MessageDialog checkInMsg = new MessageDialog("User already exist");
                            LstPopulate.SelectedIndex = 0;
                            LstPopulate.ScrollIntoView(LstPopulate.SelectedItem);
                            await checkInMsg.ShowAsync();
                            return;
                        }
                        else if (response.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            //Send to Registration DB
                            newVisitor();
                        }
                        else if (response.ResponseStatusCode == System.Net.HttpStatusCode.InternalServerError)
                        {
                            continueValidation = false;
                            MessageDialog md = new MessageDialog("Could not complete registration: " + "Err: ");
                            await md.ShowAsync();
                            return;
                        }
                        else
                        {
                            MessageDialog checkInMsg = new MessageDialog("Server error");
                            await checkInMsg.ShowAsync();
                            return;
                        }

                    }
                    else
                    {
                        return;
                    }
                }

                MessageDialog msg = new MessageDialog("Uploaded Successfully");
                await msg.ShowAsync();
            }
            catch (Exception ex)
            {

                MessageDialog msg = new MessageDialog(ex.Message + " fileCsvToDb");
                //msg.ShowAsync();
            }
        }

        private async void newVisitor()
        {
            //Register new Visitor
            try
            {
                VisitorDataPayLoad visitor = new VisitorDataPayLoad();
                visitor.CompanyName = Company;
                visitor.EmailAddress = Email;
                visitor.FullName = FullName;
                visitor.PhoneNumber = Convert.ToInt64(PhoneNumber);

                visitor.Photo = "";
                visitor.Signature = "@";
                visitor.ThumbPrint = "@";

                if (LstPopulate.Items.Count > 0)
                {
                    UserDetail.RemoveAt(0);
                }

                refreshData();
                LstPopulate.ItemsSource = null;
                LstPopulate.ItemsSource = UserDetail;

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " newVisitor");
                //await msg.ShowAsync();
            }
        }
    }
}
