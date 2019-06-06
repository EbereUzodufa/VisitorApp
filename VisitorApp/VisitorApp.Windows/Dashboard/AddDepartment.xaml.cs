using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VisitorApp.Models;
using VisitorApp.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using VisitorApp.Dashboard.Staff;
using Windows.Storage;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VisitorApp.Dashboard.Admin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddDepartment : Page
    {
        AccountDetails _activePage = new AccountDetails();
        GetDataFromDB GetDataFromDB = new GetDataFromDB();
        RemoteService service = new RemoteService();
        VisitorAppHelper VisitorAppHelper = new VisitorAppHelper();
        List<DepartmentGlobal> DeptItemSource = new List<DepartmentGlobal>();
        int CompId;

        private const char DEFAULT_DELIMITER = ',';
        private const char DEFAULT_LINE_DELIMITER = '\n';

        public string RawText { get; set; }
        //public bool HasHeaderRow { get; set; }
        List<string> Header { get; set; }
        List<Dictionary<string, string>> Body;
        public string ClickedDeptName { get; set; }

        List<CsvDeptsDetails> DeptDetail;

        int sn;
        public string DeptName { get; set; }
        public string DeptPhoneNumber { get; set; }

        int deptnameIndex;
        int deptPhoneNoIndex;
        private bool continueValidation;
        int LstPopSize { get; set; }    //intitial size of listview

        public AddDepartment()
        {
            this.InitializeComponent();
        }

        #region All Page - Maybe Amateur
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
            this.Frame.Navigate(typeof(SeeGuestList), _activePage);
        }

        private void btnUploadCsv_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UploadCsvFile), _activePage);
        }
        private void btnDepartment_Click(object sender, RoutedEventArgs e)
        {
            //The same Page
            //this.Frame.Navigate(typeof(AddDepartment), _activePage);
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateBinding();
            noted();
        }

        private async void CreateBinding()
        {
            try
            {
                var theDeptSource = GetDataFromDB.GetDataCompanyDepartments(CompId).Result;
                var theDeptSourceList = theDeptSource.DepartmentList;
                if (theDeptSourceList != null)
                {
                    LstDepartment.ItemsSource = theDeptSourceList;
                    DeptItemSource = theDeptSourceList;
                }
                lastRefreshed();

                //txbLastRefresh.Text = "Last refreshed by " + DateTime.Now.TimeOfDay.ToString();
                //List<DepartmentGlobal> theDeptSourceListFilter = new List<DepartmentGlobal>();

                //if (theDeptSourceList != null)
                //{
                //    foreach (var item in theDeptSourceList)
                //    {
                //        if (item.Status.ToUpper() != DeptStatus.Deleted.ToString().ToUpper())
                //        {
                //            theDeptSourceListFilter.Add(item);
                //        }
                //    }
                //    LstDepartment.ItemsSource = theDeptSourceListFilter;    //.Sort((x, y) => y.Status.CompareTo(x.Status));
                //}
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msg = new MessageDialog(ex.Message);
                //await msg.ShowAsync();
            }
        }

        private void ShowDeptListGrid()
        {
            GridDeptList.Visibility = Visibility.Visible;
            GridAddNewDeptList.Visibility = Visibility.Collapsed;
            GridEditItemDetail.Visibility = Visibility.Collapsed;
            GridItemDetail.Visibility = Visibility.Collapsed;
        }

        private void ShowAddNewDeptListGrid()
        {
            GridDeptList.Visibility = Visibility.Collapsed;
            GridAddNewDeptList.Visibility = Visibility.Visible;
            GridAddNewDept.Visibility = Visibility.Collapsed;
            GridEditItemDetail.Visibility = Visibility.Collapsed;
            GridItemDetail.Visibility = Visibility.Collapsed;
        }

        private async void btnAddNewDept_Click(object sender, RoutedEventArgs e)
        {
            //Add One New Department
            try
            {
                
                var deptName = txtDeptName.Text.Trim();
                var deptPhNumber = txtDeptPhoneNumber.Text.Trim();

                if (string.IsNullOrEmpty(deptName))
                {
                    MessageDialog msg = new MessageDialog("Provide Appropriate value for Department Name");
                    await msg.ShowAsync();
                    return;
                }
                else if (string.IsNullOrEmpty(deptPhNumber))
                {
                    MessageDialog msg = new MessageDialog("Provide Appropriate value for Department Phone Number");
                    await msg.ShowAsync();
                    return;
                }
                else
                {
                    newDepartment(deptName, deptPhNumber);
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog("Error from btnAddDept " + ex.Message);
                //await msg.ShowAsync();
            }
        }

        private void btnCloseAddDeptList_Click(object sender, RoutedEventArgs e)
        {
            ShowDeptListGrid();
        }

        private void btnAddNewDeptList_Click(object sender, RoutedEventArgs e)
        {
            //Add List of New Departments

        }

        private void btnCloseAddDept_Click(object sender, RoutedEventArgs e)
        {
            CloseAddDept();
        }

        private void CloseAddDept()
        {
            GridAddNewDept.Visibility = Visibility.Collapsed;
        }

        private async void newDepartment(string DeptName, string DeptPhoneNumber)
        {
            try
            {
                //Register new Department
                DepartmentDataPayload department = new DepartmentDataPayload();
                department.DepartmentName = DeptName;
                department.DepartmentPhoneNumber = DeptPhoneNumber;
                department.CompanyId = CompId;
                department.Status = DeptStatus.Active.ToString();
                department.Description = "Added by "+ _activePage.UserStaffName;

                ResponseMessage msgExist = await service.CheckIfDepartmentExistService(department);

                if (msgExist.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ResponseMessage msg = await service.RegisterNewDepartment(department);

                    if (msg.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        MessageDialog ms = new MessageDialog("Successfully Registered");
                        await ms.ShowAsync();
                        CreateBinding();
                        ClearAddNewDeptFields();
                    }
                    else
                    {
                        MessageDialog ms = new MessageDialog("Err: Else1 - " + msg.Message);
                        await ms.ShowAsync();
                    } 
                }
                else
                {
                    MessageDialog ms = new MessageDialog("Err: Else2 - " + msgExist.Message);
                    await ms.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msg = new MessageDialog(ex.Message + " Void - newDepartment");
                //await msg.ShowAsync();
            }
        }

        private void btnShowAddNewDept_Click(object sender, RoutedEventArgs e)
        {
            ShowDeptListGrid();
            GridAddNewDept.Visibility = Visibility.Visible;
        }

        private void btnShowAddNewDeptList_Click(object sender, RoutedEventArgs e)
        {
            ShowAddNewDeptListGrid();
        }

        private void ClearAddNewDeptFields()
        {
            txtDeptName.Text = "";
            txtDeptPhoneNumber.Text = "";
            CloseAddDept();
        }

        private void txtCheckNumber_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            CheckSyntax.checkOnlyNumber(sender, e);
        }

        private void LstDepartment_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (LstDepartment.Items.Count > 0)
            {
                LstDepartment.SelectedItem = e.ClickedItem;
                selectedItem();
                GridItemDetail.Visibility = Visibility.Visible;
                GridAddNewDept.Visibility = Visibility.Collapsed;
            }
        }

        private async void selectedItem()
        {
            try
            {
                var dept = (DepartmentGlobal)LstDepartment.SelectedItem;

                txtClickedItemDept.Text = dept.DepartmentName;
                txtClickedItemPhoNo.Text = dept.DepartmentPhoneNumber;
                ClickedDeptName = dept.DepartmentName;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - selectedItem");
                //await msg.ShowAsync();
            }
        }

        private void btnItemDetailClose_Click(object sender, RoutedEventArgs e)
        {
            GridEditItemDetail.Visibility = Visibility.Collapsed;
            GridItemDetail.Visibility = Visibility.Collapsed;
        }

        private void btnEditDeptDetails_Click(object sender, RoutedEventArgs e)
        {
            GridEditItemDetail.Visibility = Visibility.Visible;
        }

        private async void btnArchiveDeptDetails_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msgNote = new MessageDialog("Do you want to archive this department?", "Alert");
            msgNote.Commands.Add(new UICommand("Yes, Continue") { Id = 0 });
            msgNote.Commands.Add(new UICommand("No") { Id = 1 });

            msgNote.CancelCommandIndex = 1;

            var result = await msgNote.ShowAsync();

            if (Convert.ToInt32(result.Id) == 0)
            {
                UpdateDepartment(DeptStatus.Archived.ToString());
            }
        }

        private async void btnActiveDeptDetails_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msgNote = new MessageDialog("Do you want to activate this department?", "Alert");
            msgNote.Commands.Add(new UICommand("Yes, Continue") { Id = 0 });
            msgNote.Commands.Add(new UICommand("No") { Id = 1 });

            msgNote.CancelCommandIndex = 1;

            var result = await msgNote.ShowAsync();

            if (Convert.ToInt32(result.Id) == 0)
            {
                UpdateDepartment(DeptStatus.Active.ToString());
            }
        }

        private async void btnDeleteDeptDetails_Click(object sender, RoutedEventArgs e)
        {
            var numberStaff = (DepartmentGlobal)LstDepartment.SelectedItem;

            if (numberStaff.DeptSize <= 0)
            {
                MessageDialog msgNote = new MessageDialog("Do you want to delete this department?", "Alert");
                msgNote.Commands.Add(new UICommand("Yes, Continue") { Id = 0 });
                msgNote.Commands.Add(new UICommand("No") { Id = 1 });

                msgNote.CancelCommandIndex = 1;

                var result = await msgNote.ShowAsync();

                if (Convert.ToInt32(result.Id) == 0)
                {
                    UpdateDepartment(DeptStatus.Deleted.ToString());
                } 
            }
            else
            {
                MessageDialog msgNote = new MessageDialog("Cannot delete department because it has "+ numberStaff.DeptSize.ToString() + " staff", "Alert");
                msgNote.Commands.Add(new UICommand("Move staff to another department") { Id = 0 });
                msgNote.Commands.Add(new UICommand("Cancel") { Id = 1 });

                msgNote.CancelCommandIndex = 1;

                var result = await msgNote.ShowAsync();

                if (Convert.ToInt32(result.Id) == 0)
                {
                    //MessageDialog hh = new MessageDialog("Movie");
                    //await hh.ShowAsync();
                    var dept = (DepartmentGlobal)LstDepartment.SelectedItem;
                    _activePage.PageMsg = dept.DepartmentId.ToString();
                    this.Frame.Navigate(typeof(DepartmentStaff), _activePage);
                }
            }
        }

        private void btnUpdateDept_Click(object sender, RoutedEventArgs e)
        {
            UpdateDepartment(DeptStatus.Active.ToString());
        }

        private async void UpdateDepartment(string action)
        {
            try
            {

                var deptName = txtClickedItemDept.Text;
                var deptPhNumber = txtClickedItemPhoNo.Text;

                if (string.IsNullOrEmpty(deptName))
                {
                    MessageDialog msg = new MessageDialog("Provide Appropriate value for Department Name");
                    await msg.ShowAsync();
                    return;
                }
                else if (string.IsNullOrEmpty(deptPhNumber))
                {
                    MessageDialog msg = new MessageDialog("Provide Appropriate value for Department Phone Number");
                    await msg.ShowAsync();
                    return;
                }
                else
                {
                    DepartmentDataPayload department = new DepartmentDataPayload();
                    department.DepartmentName = ClickedDeptName;
                    foreach (var item in DeptItemSource)
                    {
                        if (department.DepartmentName == item.DepartmentName)
                        {
                            department.DepartmentId = item.DepartmentId;
                        }
                    }
                    department.DepartmentPhoneNumber = deptPhNumber;
                    department.CompanyId = CompId;
                    department.Status = action;
                    department.Description = "Added by " + _activePage.UserStaffName;

                    ResponseMessage msgExist = await service.CheckIfDepartmentExistService(department);

                    if (msgExist.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                    {
                        department.DepartmentId = msgExist.DepartmentId;
                        department.DepartmentName = deptName;

                        ResponseMessage msgApp = await service.UpdateThisDepartmentControllerService(department);

                        if (msgApp.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            CreateBinding();
                            GridEditItemDetail.Visibility = Visibility.Collapsed;
                            GridItemDetail.Visibility = Visibility.Collapsed;
                            MessageDialog ms = new MessageDialog("Action Successful");
                            await ms.ShowAsync();
                        }
                        else
                        {
                            MessageDialog ms = new MessageDialog("Err: " + msgApp.Message);
                            await ms.ShowAsync();
                        }
                    }
                    else
                    {
                        MessageDialog ms = new MessageDialog("Err: " + msgExist.Message);
                        await ms.ShowAsync();
                    }
                    GridEditItemDetail.Visibility = Visibility.Collapsed;
                    GridItemDetail.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog("Error from btnAddDept " + ex.Message);
                //await msg.ShowAsync();
            }
        }

        private async void btnSelectTextFile_Click(object sender, RoutedEventArgs e)
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
                //await msg.ShowAsync();
            }
        }

        private async void btnShow_Click(object sender, RoutedEventArgs e)
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
                //await msg.ShowAsync();
            }
        }

        private async void btnCloseSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                hideGridSearchPoulated();
                resetGridDetailItems();

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnClosSearch_Clicked");
                //await msg.ShowAsync();
            }
        }

        private async void hideGridSearchPoulated()
        {
            try
            {
                GridSearchSection.Visibility = Visibility.Collapsed;
                GridPopulateSection.Visibility = Visibility.Visible;

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - hideGridSearchPopulated");
                //await msg.ShowAsync();
            }
        }

        private async void resetGridDetailItems()
        {
            try
            {
                var resetColor = new SolidColorBrush(Colors.White);

                txtClickedItemDept.Background = resetColor;
                txtClickedItemPhoNo.Background = resetColor;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - resetGridDetaiItems");
                //await msg.ShowAsync();
            }
        }
        
        private void cmbxItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            checkPopulateComboBox(sender);
        }

        private async void checkPopulateComboBox(object sender)
        {
            try
            {
                cmbxDept.Background = new SolidColorBrush(Colors.Transparent);
                cmbxPhoneNumber.Background = new SolidColorBrush(Colors.Transparent);
                btnPopulateGrid.Visibility = Visibility.Visible;
                txbError.Visibility = Visibility.Collapsed;

                if (cmbxDept.SelectedItem != null  && cmbxPhoneNumber.SelectedItem != null)
                {
                    #region Check 2

                    #region Check Company
                    if (cmbxDept.SelectedItem.ToString() == cmbxPhoneNumber.SelectedItem.ToString())
                    {
                        cmbxDept.Background = new SolidColorBrush(Colors.Red);
                        cmbxPhoneNumber.Background = new SolidColorBrush(Colors.Red);
                        txbError.Text = "Remove Repeated Field at 'Company' and 'Name'";
                        txbError.Visibility = Visibility.Visible;
                        btnPopulateGrid.Visibility = Visibility.Collapsed;
                    }

                    #endregion

                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - checkPopulateComboBox");
                //await msg.ShowAsync();
            }
        }

        private async void SelectCSVFile()
        {
            try
            {
                //Select from Gallery
                StorageFile DeptFile = await VisitorAppHelper.selectCsv();
                //make sure the fill is not null
                string testString = null;

                if (DeptFile != null)
                {
                    RawText = await VisitorAppHelper.GetCsv(DeptFile);
                    Header = await VisitorAppHelper.GetCsvHeader(RawText);
                    Body = await VisitorAppHelper.GetCsvBody(RawText, Header);

                    populateComboBox();
                    resetGridItemDetail();
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - SelectCSVFile");
                //await msg.ShowAsync();
            }
        }

        private async void resetGridItemDetail()
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
                //await msg.ShowAsync();
            }
        }

        private async void resetClickItemBAckground()
        {
            try
            {
                var resetBackground = new SolidColorBrush(Colors.White);

                txtClickedItemDept.Background = resetBackground;
                txtClickedItemPhoNo.Background = resetBackground;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " resetClickItemBackground");
                //await msg.ShowAsync();
            }
        }

        private async void resetClickItemText()
        {
            try
            {
                txtClickedItemDept.Text = "";
                txtClickedItemPhoNo.Text = "";
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " resetClickItemText");
                //await msg.ShowAsync();
            }
        }

        private void populateComboBox()
        {
            clearComboBoxes();
            foreach (var head in Header)
            {
                cmbxDept.Items.Add(head);
                cmbxPhoneNumber.Items.Add(head);
            }
        }

        private async void clearComboBoxes()
        {
            try
            {
                cmbxPhoneNumber.Items.Clear();
                cmbxDept.Items.Clear();
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - clearComboBoxes");
                //await msg.ShowAsync();
            }
        }

        private async void checkComboBoxNoRepeat()
        {
            try
            {
                if ((cmbxDept.Background == new SolidColorBrush(Colors.Red)) || (cmbxPhoneNumber.Background == new SolidColorBrush(Colors.Red)))
                {
                    txbError.Text = ("Remove Repeated Field");
                    txbError.Visibility = Visibility.Visible;
                    return;
                }

                else
                {
                    txbError.Visibility = Visibility.Collapsed;
                    LstUploadPopulate.Items.Clear();
                    populateGrid();
                }

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - checkComboBoxNoRepeat");
                //await msg.ShowAsync();
            }
        }

        private async void populateGrid()
        {
            try
            {
                DeptDetail = new List<CsvDeptsDetails>();

                string body = null;

                sn = 0;

                foreach (var item in Body)
                {
                    string _body = null;

                    if (item.Keys.Count == Header.Count)
                    {
                        var content = item.Values.ToList();

                        DeptName = content[deptnameIndex].ToString().Trim();
                        DeptPhoneNumber = content[deptPhoneNoIndex].ToString().Trim();

                        sn += 1;

                        CreateGridViaBinding();
                    }
                }

                LstUploadPopulate.ItemsSource = DeptDetail;

                //FileText.Text = body;

                showComboBoxStack();
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - populateGrid()");
                //await msg.ShowAsync();
            }
        }

        private async void showComboBoxStack()
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
                //await msg.ShowAsync();
            }
        }

        private async void CreateGridViaBinding()
        {
            try
            {
                #region Check Phone Number

                string _DeptPhoneNumber = null;
                string value = null;

                string _PhoneNumberValue = DeptPhoneNumber.ToLower();

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
                    _DeptPhoneNumber = value;
                }
                else if (_PhoneNumberValue.Contains("+") == true && _PhoneNumberValue[0].ToString() == "+")
                {
                    value = _PhoneNumberValue.Substring(1);
                    _DeptPhoneNumber = value;
                }
                else
                {
                    _DeptPhoneNumber = _PhoneNumberValue;
                }

                #endregion

                DeptDetail.Add(new CsvDeptsDetails
                {
                    sn = sn.ToString(),
                    DepartmentName = DeptName,
                    DepartmentPhoneNumber = DeptPhoneNumber,
                });
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - createGridBinding");
                //await msg.ShowAsync();
            }
        }

        private async void btnPopulateGrid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbxDept.SelectedItem != null && cmbxPhoneNumber.SelectedItem != null)
                {
                    deptnameIndex = cmbxDept.SelectedIndex;
                    deptPhoneNoIndex = cmbxPhoneNumber.SelectedIndex;

                    LstUploadPopulate.ItemsSource = null;
                    //FileText.Text = "";
                    populateGrid();
                    txtPopulateNumber.Text = LstUploadPopulate.Items.Count().ToString();
                    gridSearchSection.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageDialog msg = new MessageDialog("Empty Field(s)");
                    await msg.ShowAsync();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnPopulateGrid_Click");
                //await msg.ShowAsync();
            }
        }

        private void btnUpdateList_Click(object sender, RoutedEventArgs e)
        {
            updateListGridItem();
        }

        private async void updateListGridItem()
        {
            try
            {
                var selectedIndex = LstUploadPopulate.SelectedIndex;
                var newUserDetail = AddItemListDetail((selectedIndex + 1).ToString(), txtClickedItemListDept.Text, txtClickedItemListPhoNo.Text);

                DeptDetail.Insert(selectedIndex, newUserDetail);
                DeptDetail.RemoveAt(selectedIndex + 1);

                LstUploadPopulate.ItemsSource = null;
                LstUploadPopulate.ItemsSource = DeptDetail;

                GridEditItemDetail.Visibility = Visibility.Collapsed;
                GridItemDetail.Visibility = Visibility.Collapsed;
                txtClickedItemListDept.Background = new SolidColorBrush(Colors.Transparent);
                txtClickedItemListPhoNo.Background = new SolidColorBrush(Colors.Transparent);
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " updateGridItem");
                //await msg.ShowAsync();
            }
        }

        private CsvDeptsDetails AddItemListDetail(string sn, string DeptName, string DeptPhoneNumber)
        {
            var AddItem = new CsvDeptsDetails();
            AddItem.sn = sn;
            AddItem.DepartmentName = DeptName;
            AddItem.DepartmentPhoneNumber = DeptPhoneNumber;
            return AddItem;
        }

        private async void btnListDeleteDeptDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedIndex = LstUploadPopulate.SelectedIndex;
                DeptDetail.RemoveAt(selectedIndex);

                refreshData();
                LstUploadPopulate.ItemsSource = null;
                LstUploadPopulate.ItemsSource = DeptDetail;

                GridEditItemDetail.Visibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " btnDeleteUserDetails_Click");
                //await msg.ShowAsync();
            }
        }

        private async void refreshData()
        {
            try
            {
                for (int i = 0; i < DeptDetail.Count; i++)
                {
                    var user = DeptDetail[i];

                    user.sn = (i + 1).ToString();
                    DeptDetail.Insert(i, user);
                    DeptDetail.RemoveAt(i + 1);
                }

                txtPopulateNumber.Text = LstUploadPopulate.Items.Count().ToString();

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - refreshData");
                //await msg.ShowAsync();
            }
        }

        private void btnListEditUserDetails_Click(object sender, RoutedEventArgs e)
        {
            GridListEditItemDetail.Visibility = Visibility.Visible;
        }

        private async void btnListItemDetailClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GridListEditItemDetail.Visibility = Visibility.Collapsed;
                GridListItemDetail.Visibility = Visibility.Collapsed;
                resetClickItemBAckground();

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - btnListItemDetailClose_Click");
                //await msg.ShowAsync();
            }
        }

        private void LstUploadPopulate_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (LstUploadPopulate.Items.Count > 0)
            {
                LstUploadPopulate.SelectedItem = e.ClickedItem;
                selectedListItem();
                GridListDetailOpen();
            }
        }

        private void GridListDetailOpen()
        {
            GridListItemDetail.Visibility = Visibility.Visible;
        }

        private async void selectedListItem()
        {
            try
            {
                var dept = (CsvDeptsDetails)LstUploadPopulate.SelectedItem;

                txtClickedItemListDept.Text = dept.DepartmentName;
                txtClickedItemListPhoNo.Text = dept.DepartmentPhoneNumber;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - selectedListItem");
                //await msg.ShowAsync();
            }
        }

        private async void checkSyntax()
        {
            try
            {
                string deptName = DeptName;
                string deptPhoNo = DeptPhoneNumber;

                bool stringDeptName = false;
                bool stringDeptPhoNo = false;

                string txtPhoneNuber = DeptPhoneNumber;

                #region Validate PhoneNumber
                if (deptPhoNo != null)
                {
                    for (int i = 0; i < deptPhoNo.Length; i++)
                    {
                        if (!char.IsNumber(txtPhoneNuber[i]))
                        {
                            continueValidation = false;
                            MessageDialog msg = new MessageDialog("The Phone Number contains non-numeric character; " + txtPhoneNuber[i]);
                            LstUploadPopulate.ScrollIntoView(LstUploadPopulate.SelectedItem);
                            txtClickedItemListPhoNo.Background = new SolidColorBrush(Colors.Red);
                            selectedListItem();
                            await msg.ShowAsync();
                            return;
                        }
                    }
                }
                #endregion

                #region Validate Dept Name
                if (deptName != null)
                {
                    for (int i = 0; i < DeptName.Length; i++)
                    {
                        if (!char.IsLetter(deptName[i]) && deptName[i].ToString() != "-" && deptName[i].ToString() != " ")
                        {
                            continueValidation = false;
                            MessageDialog msg = new MessageDialog("The Department Name contains an invalid character; " + deptName[i] + "\n" + "Please remove to proceed at Serial Number " + sn.ToString());
                            LstUploadPopulate.SelectedIndex = sn - 1;
                            LstUploadPopulate.ScrollIntoView(LstUploadPopulate.SelectedItem);
                            txtClickedItemListDept.Background = new SolidColorBrush(Colors.Red);
                            selectedListItem();
                            await msg.ShowAsync();
                            return;
                        }
                    }
                }
                #endregion
                //else
                //{
                long phoneNumber = Convert.ToInt64(DeptPhoneNumber);

                MessageDialog msgbox = new MessageDialog(
                        "Dept Name: " + DeptName + "\n" +
                        "Phone Number: " + DeptPhoneNumber
                    );
                //msgbox.ShowAsync();

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - checkSyntax");
                //await msg.ShowAsync();
            }
        }

        private async void fileCsvToDb()
        {
            try
            {
                var ity = LstUploadPopulate.Items.Count();
                continueValidation = true;

                for (int i = 0; i < ity; i++)
                {
                    
                    var dept = DeptDetail[i];

                    DeptName = dept.DepartmentName;
                    DeptPhoneNumber = dept.DepartmentPhoneNumber;

                    sn = i + 1;

                    checkSyntax();

                    if (continueValidation == true)
                    {
                        RemoteService service = new RemoteService();
                        DepartmentDataPayload payload = new DepartmentDataPayload();

                        DepartmentDataPayload department = new DepartmentDataPayload();
                        department.DepartmentName = DeptName;
                        department.DepartmentPhoneNumber = DeptPhoneNumber;
                        department.CompanyId = CompId;
                        department.Status = DeptStatus.Active.ToString();
                        department.Description = "Added by " + _activePage.UserStaffName;
                       
                        ResponseMessage response = await service.CheckIfDepartmentExistService(department);

                        if (response.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                        {
                            //Found

                            MessageDialog msgTrial = new MessageDialog("Dept: " + department.DepartmentName + "\n" +
                                                        "Dept Phone Number: " + department.DepartmentPhoneNumber + "\n" +
                                                        "Dept Status: " + department.Status, "Found");
                            //await msgTrial.ShowAsync();
                        }
                        else if (response.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            //Send to Registration DB

                            ResponseMessage msgDept = await service.RegisterNewDepartment(department);
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
                CreateBinding();
                await msg.ShowAsync();
                LstUploadPopulate.ItemsSource = null;
                ShowDeptListGrid();
            }
            catch (Exception ex)
            {
                checkInternet();

                MessageDialog msg = new MessageDialog(ex.Message + " fileCsvToDb");
                //await msg.ShowAsync();
            }
        }

        private async void btnPushFill_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                fileCsvToDb();

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " btnPushFill_Click");
                //await msg.ShowAsync();
            }
        }

        private async void btnShowRefreshDeptList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LstPopSize != LstDepartment.Items.Count())
                {
                    CreateBinding();
                    noted();
                }

                else
                {
                    lastRefreshed();
                }
            }
            catch (Exception ex)
            {
                MessageDialog msgErr = new MessageDialog(ex.Message, "btnShowRefreshAppointmentList_Click");
                //await msgErr.ShowAsync();
            }
        }

        private void lastRefreshed()
        {
            txbLastRefresh.Text = "Last refreshed by " + DateTime.Now.TimeOfDay.Hours + ":" + DateTime.Now.TimeOfDay.Minutes + ":" + DateTime.Now.TimeOfDay.Seconds;
        }

        private void noted()
        {
            //Note the following
            LstPopSize = LstDepartment.Items.Count;    //Current number of Appointments.
        }

        private void btnListSeeDeptStaff_Click(object sender, RoutedEventArgs e)
        {
            var dept = (DepartmentGlobal)LstDepartment.SelectedItem;
            _activePage.PageMsg = dept.DepartmentId.ToString();
            this.Frame.Navigate(typeof(DepartmentStaff),_activePage);
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
