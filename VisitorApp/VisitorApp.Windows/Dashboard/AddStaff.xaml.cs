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
using Windows.Media.Capture;
using Windows.Storage;
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

namespace VisitorApp.Dashboard.Staff
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddStaff : Page
    {
        AccountDetails _activePage = new AccountDetails();
        GetDataFromDB GetDataFromDB = new GetDataFromDB();
        RemoteService service = new RemoteService();
        UserProcess VisitorAppUserProcess = new UserProcess();
        VisitorAppHelper VisitorAppHelper = new VisitorAppHelper();
        int CompId;

        List<string> departments = new List<string>();
        List<int> departmentIds = new List<int>();
        List<string> staffRolesList = new List<string>();

        private string staffFirstName;
        private string staffLastName;
        private string staffName;
        private string StaffPhoneNumber;
        private string StaffEmail;
        private string StaffRole;
        private string StaffIdNumber;
        private string StaffPhotoString;
        private int DepartmentId;
        private int StaffId;

        public AddStaff()
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
            this.Frame.Navigate(typeof(AddDepartment), _activePage);
        }
        private void btnStaff_Click(object sender, RoutedEventArgs e)
        {
            //The same Page
            //this.Frame.Navigate(typeof(AddStaff), _activePage);
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

        private void btnShowAddNewStaff_Click(object sender, RoutedEventArgs e)
        {
            ShowStaffListGrid();
            GridAddNewStaff.Visibility = Visibility.Visible;
        }

        private void btnShowAddNewStaffList_Click(object sender, RoutedEventArgs e)
        {
            ShowAddNewStaffListGrid();
        }

        private void btnCloseAddStaff_Click(object sender, RoutedEventArgs e)
        {
            CloseAddStaff();
        }

        private async void btnAddNewStaff_Click(object sender, RoutedEventArgs e)
        {
            //Add One New Department

            staffFirstName = txtStaffFirstName.Text;
            staffLastName = txtStaffLastName.Text;
            staffName = staffFirstName + " " + staffLastName;
            StaffPhoneNumber = txtStaffPhoneNumber.Text;
            StaffEmail = txtStaffEmail.Text;
            StaffIdNumber = txtStaffIdNumber.Text;
            var DeptName = cmbDeptName.SelectedItem.ToString();
            StaffRole = cmbStaffRole.SelectedItem.ToString();

            for (int i = 0; i < departmentIds.Count; i++)
            {
                if (DeptName == departments[i].ToString())
                {
                    DepartmentId = Convert.ToInt32(departmentIds[i].ToString());
                    break;
                }
            }

            if (string.IsNullOrEmpty(staffLastName) || string.IsNullOrEmpty(staffFirstName))
            {
                MessageDialog msg = new MessageDialog("Provide Appropriate value for Staff First and Last Name");
                await msg.ShowAsync();
                return;
            }
            else if (string.IsNullOrEmpty(StaffPhoneNumber))
            {
                MessageDialog msg = new MessageDialog("Provide Appropriate value for Staff Phone Number");
                await msg.ShowAsync();
                return;
            }
            else if (string.IsNullOrEmpty(StaffIdNumber))
            {
                MessageDialog msg = new MessageDialog("Provide Appropriate value for Staff ID Number");
                await msg.ShowAsync();
                return;
            }
            else if (string.IsNullOrEmpty(StaffEmail))
            {
                MessageDialog msg = new MessageDialog("Provide Appropriate value for Staff Email");
                await msg.ShowAsync();
                return;
            }
            else if (string.IsNullOrEmpty(DeptName))
            {
                MessageDialog msg = new MessageDialog("Provide Appropriate value for Department");
                await msg.ShowAsync();
                return;
            }
            else if (string.IsNullOrEmpty(StaffRole))
            {
                MessageDialog msg = new MessageDialog("Provide Appropriate value for Staff Role");
                await msg.ShowAsync();
                return;
            }
            else if (string.IsNullOrEmpty(StaffPhotoString))
            {
                MessageDialog msg = new MessageDialog("Provide Staff Photo");
                await msg.ShowAsync();
                return;
            }
            else
            {
                newStaff(staffFirstName, staffLastName,staffName, StaffEmail, StaffPhoneNumber, StaffIdNumber, DepartmentId, StaffRole, StaffPhotoString);

            }
        }

        private async void newStaff(string staffFirstName, string staffLastName, string staffName, string StaffEmail,string StaffPhoneNumber, string StaffIdNumber, int DepartmentId, string StaffRole, string StaffPhotoString)
        {
            try
            {
                //Register new Staff
                StaffDataPayload staff = new StaffDataPayload();
                staff.StaffFirstName = staffFirstName;
                staff.StaffLastName = staffLastName;
                staff.StaffName = staffName;
                staff.StaffEmail = StaffEmail;
                staff.StaffPhoneNumber = StaffPhoneNumber;
                staff.StaffIdNumber = StaffIdNumber;
                staff.DepartmentId = DepartmentId;
                staff.Role = StaffRole;
                staff.CompanyId = _activePage.CompanyId;
                staff.StaffPhoto = StaffPhotoString;
                staff.Description = "Added by " + _activePage.UserStaffName;
                staff.StaffStatus = StaffStatus.Active.ToString();

                ResponseMessage msgExist = await service.CheckIfStaffExistService(staff);

                UserLoginDataPayLoad userDetail = new UserLoginDataPayLoad
                {
                    username = await VisitorAppUserProcess.CreateUserName(StaffEmail)
                };
                ResponseMessage msgUserExist = await service.CheckIfUserExistService(userDetail);

                if (msgExist.ResponseStatusCode == System.Net.HttpStatusCode.NotFound  && msgUserExist.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ResponseMessage msg = await service.RegisterNewStaff(staff);

                    StaffId = msg.ResponseCode;

                    if (msg.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        CreateBinding();
                        newUser();
                        ClearAddNewStaffields();
                    } 
                }
                else
                {
                    MessageDialog ms = new MessageDialog("Err: " + msgExist.Message +"\n" + msgUserExist.Message);
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

        private void btnCloseAddStaffList_Click(object sender, RoutedEventArgs e)
        {
            CloseAddStaff();
        }

        private void staffRoles()
        {
            staffRolesList = new List<string>();
            staffRolesList.Add("Admin");
            staffRolesList.Add("FrontDesk");
            staffRolesList.Add("Staff");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateBinding();

            #region Staff Roles
            staffRoles();
            cmbStaffRole.ItemsSource = staffRolesList;
            #endregion
        }

        private async void CreateBinding()
        {
            try
            {
                var theStaffSource = GetDataFromDB.GetDataCompanyStaff(CompId).Result;
                var theStaffSourceList = theStaffSource.StaffList;
                if (theStaffSourceList != null)
                {
                    LstStaff.ItemsSource = theStaffSourceList;
                }

                var theDeptSource = GetDataFromDB.GetDataCompanyDepartments(CompId).Result;
                var theDeptSourceList = theDeptSource.DepartmentList;

                departments = new List<string>();
                departmentIds = new List<int>();

                if (theDeptSourceList != null)
                {
                    foreach (var item in theDeptSourceList)
                    {
                        departments.Add(item.DepartmentName);
                        departmentIds.Add(item.DepartmentId);
                    }

                    cmbDeptName.ItemsSource = departments;
                }

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message);
                //await msg.ShowAsync();
            }
        }

        private void ShowStaffListGrid()
        {
            GridStaffList.Visibility = Visibility.Visible;
            //GridAddNewStaffList.Visibility = Visibility.Collapsed;
        }

        private void ShowAddNewStaffListGrid()
        {
            GridStaffList.Visibility = Visibility.Collapsed;
            //GridAddNewStaffList.Visibility = Visibility.Visible;
            GridAddNewStaff.Visibility = Visibility.Collapsed;
        }

        private void ClearAddNewStaffields()
        {
            txtStaffLastName.Text = "";
            txtStaffFirstName.Text = "";
            txtStaffPhoneNumber.Text = "";
            txtStaffEmail.Text = "";
            txtStaffIdNumber.Text = "";
            txtTempPassword.Text = "";
            imgNewStaffPicture.Source = null;
            cmbStaffRole.SelectedItem = null;
            cmbDeptName.SelectedItem = null;
            CloseAddStaff();
        }

        private void ClearMoveStaffields()
        {
            txbMovingStaffDept.Text = "";
            txbMovingStaffId.Text = "";
            txbMovingStaffName.Text = "";
            txbMovingStaffPhoNo.Text = "";
            cmbMovingNewDept.SelectedItem = null;
            GridMoveStaff.Visibility = Visibility.Collapsed;
        }

        private void CloseAddStaff()
        {
            GridAddNewStaff.Visibility = Visibility.Collapsed;
        }

        private async void btnSelectStaffPicture_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage img = null;
            MessageDialog selectPicture = new MessageDialog("Select Capture Method");
            if (_activePage.UserStaffRole.ToUpper() != StaffRoles.Admin.ToString().ToUpper())
            {
                selectPicture.Commands.Add(new UICommand("Take Photo") { Id = 0 });
            }
            selectPicture.Commands.Add(new UICommand("Select from Gallery") { Id = 1 });
            selectPicture.Commands.Add(new UICommand("Cancel") { Id = 2 });

            selectPicture.CancelCommandIndex = 2;

            var result = await selectPicture.ShowAsync();

            if (Convert.ToInt32(result.Id) == 0)
            {
                //Take Photo
                CameraCaptureUI captureUI = new CameraCaptureUI();
                captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
                captureUI.PhotoSettings.CroppedSizeInPixels = new Size(400, 400);

                var photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

                if (photo == null)
                {
                    // User cancelled photo capture
                    return;
                }

                //Convert Image
                var stream = await photo.OpenAsync(FileAccessMode.Read);
                var bitmap = new BitmapImage();
                bitmap.SetSource(stream);
                img = bitmap;

                Byte[] bytes = new Byte[0];
                var reader = new DataReader(stream.GetInputStreamAt(0));
                bytes = new Byte[stream.Size];
                await reader.LoadAsync((uint)stream.Size);
                reader.ReadBytes(bytes);
                // Convert the byte array to Base 64 string
                StaffPhotoString = Convert.ToBase64String(bytes);
            }

            if (Convert.ToInt32(result.Id) == 1)
            {
                //Select from Gallery
                StorageFile GuestImage = await VisitorAppHelper.selectImage();
                //make sure the fill is not null

                if (GuestImage == null)
                {
                    //if null
                    return;
                }

                img = await VisitorAppHelper.GetImage(GuestImage);
                StaffPhotoString = await VisitorAppHelper.ConvertImageToBase64();

            }


            //Set Image
            imgNewStaffPicture.Source = null;
            imgNewStaffPicture.Source = img;
        }

        private async void newUser()
        {
            try
            {
                string userName = await VisitorAppUserProcess.CreateUserName(txtStaffEmail.Text.Trim());
                string password = txtTempPassword.Text.Trim();

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

                MessageDialog ms = new MessageDialog("Successfully Registered");
                await ms.ShowAsync();

                //MessageDialog md = new MessageDialog("Registration Complete");
                //await md.ShowAsync();
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msg = new MessageDialog(ex.Message + " Void - newUser");
                //await msg.ShowAsync();
            }
        }

        private void txtCheckNumber_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            CheckSyntax.checkOnlyNumber(sender, e);
        }

        private async void btnClickedItemSelectStaffPicture_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage img = null;
            MessageDialog selectPicture = new MessageDialog("Select Capture Method");
            selectPicture.Commands.Add(new UICommand("Take Photo") { Id = 0 });
            selectPicture.Commands.Add(new UICommand("Select from Gallery") { Id = 1 });
            selectPicture.Commands.Add(new UICommand("Cancel") { Id = 2 });

            selectPicture.CancelCommandIndex = 2;

            var result = await selectPicture.ShowAsync();

            if (Convert.ToInt32(result.Id) == 0)
            {
                //Take Photo
                CameraCaptureUI captureUI = new CameraCaptureUI();
                captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
                captureUI.PhotoSettings.CroppedSizeInPixels = new Size(400, 400);

                var photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

                if (photo == null)
                {
                    // User cancelled photo capture
                    return;
                }

                //Convert Image
                var stream = await photo.OpenAsync(FileAccessMode.Read);
                var bitmap = new BitmapImage();
                bitmap.SetSource(stream);
                img = bitmap;

                Byte[] bytes = new Byte[0];
                var reader = new DataReader(stream.GetInputStreamAt(0));
                bytes = new Byte[stream.Size];
                await reader.LoadAsync((uint)stream.Size);
                reader.ReadBytes(bytes);
                // Convert the byte array to Base 64 string
                StaffPhotoString = Convert.ToBase64String(bytes);
            }

            if (Convert.ToInt32(result.Id) == 1)
            {
                //Select from Gallery
                StorageFile GuestImage = await VisitorAppHelper.selectImage();
                //make sure the fill is not null

                if (GuestImage == null)
                {
                    //if null
                    return;
                }

                img = await VisitorAppHelper.GetImage(GuestImage);
                StaffPhotoString = await VisitorAppHelper.ConvertImageToBase64();

            }


            //Set Image
            imgClickedItemStaffPicture.Source = null;
            imgClickedItemStaffPicture.Source = img;
        }

        private void btnItemDetailClose_Click(object sender, RoutedEventArgs e)
        {
            GridEditItemDetail.Visibility = Visibility.Collapsed;
            GridItemDetail.Visibility = Visibility.Collapsed;
        }

        private void LstStaff_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (LstStaff.Items.Count > 0)
            {
                LstStaff.SelectedItem = e.ClickedItem;
                selectedItem();
                GridItemDetail.Visibility = Visibility.Visible;
                GridAddNewStaff.Visibility = Visibility.Collapsed;
            }
        }

        private async void selectedItem()
        {
            try
            {
                var staff = (StaffGlobal)LstStaff.SelectedItem;

                if (string.IsNullOrEmpty(staff.StaffLastName) || string.IsNullOrEmpty(staff.StaffFirstName))
                {
                    txtClickedItemStaffLastName.Text = "";
                    txtClickedItemStaffFirstName.Text = "";

                }
                else
                {
                    txtClickedItemStaffLastName.Text = staff.StaffLastName;
                    txtClickedItemStaffFirstName.Text = staff.StaffFirstName;
                }

                txtClickedItemStaffIdNumber.Text = staff.StaffIdNumber;
                txtClickedItemStaffPhoneNumber.Text = staff.StaffPhoneNumber;
                txtClickedItemEmail.Text = staff.StaffEmail;
                DepartmentId = staff.DepartmentId;
                StaffPhotoString = staff.StaffPhoto;

                cmbClickedItemStaffRole.ItemsSource = cmbStaffRole.ItemsSource;
                cmbClickedItemDeptName.ItemsSource = cmbDeptName.ItemsSource;
                cmbMovingNewDept.ItemsSource = cmbDeptName.ItemsSource;

                for (int i = 0; i < cmbClickedItemStaffRole.Items.Count; i++)
                {
                    StaffRole = cmbStaffRole.Items[i].ToString();
                    if (StaffRole == staff.Roles)
                    {
                        cmbClickedItemStaffRole.SelectedIndex = i;
                        cmbMovingNewDept.SelectedIndex = i;
                    }
                }

                for (int i = 0; i < cmbClickedItemDeptName.Items.Count; i++)
                {
                    string StaffDept = cmbDeptName.Items[i].ToString();
                    if (StaffDept == staff.DepartmentName)
                    {
                        cmbClickedItemDeptName.SelectedIndex = i;
                    }
                }

                imgClickedItemStaffPicture.Source = await VisitorAppHelper.Base64StringToBitmap(staff.StaffPhoto);

                //For to Move
                txbMovingStaffDept.Text = staff.DepartmentName;
                txbMovingStaffName.Text = staff.StaffName;
                txbMovingStaffPhoNo.Text = staff.StaffPhoneNumber;
                txbMovingStaffId.Text = staff.StaffIdNumber;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - selectedItem");
                //await msg.ShowAsync();
            }
        }

        private void btnEditStaffDetails_Click(object sender, RoutedEventArgs e)
        {
            GridEditItemDetail.Visibility = Visibility.Visible;
            GridMoveStaff.Visibility = Visibility.Collapsed;
        }

        private async void btnDeleteStaffDetails_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msgNote = new MessageDialog("Do you want to remove this staff?", "Alert");
            msgNote.Commands.Add(new UICommand("Yes, Continue") { Id = 0 });
            msgNote.Commands.Add(new UICommand("No") { Id = 1 });

            msgNote.CancelCommandIndex = 1;

            var result = await msgNote.ShowAsync();

            if (Convert.ToInt32(result.Id) == 0)
            {
                UpdateStaff(StaffStatus.Deleted.ToString());
            }
        }

        private async void btnActiveStaffDetails_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msgNote = new MessageDialog("Do you want to activate this staff?", "Alert");
            msgNote.Commands.Add(new UICommand("Yes, Continue") { Id = 0 });
            msgNote.Commands.Add(new UICommand("No") { Id = 1 });

            msgNote.CancelCommandIndex = 1;

            var result = await msgNote.ShowAsync();

            if (Convert.ToInt32(result.Id) == 0)
            {
                UpdateStaff(StaffStatus.Active.ToString());
            }
        }

        private async void btnSuspendStaffDetails_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msgNote = new MessageDialog("Do you want to suspend this staff?", "Alert");
            msgNote.Commands.Add(new UICommand("Yes, Continue") { Id = 0 });
            msgNote.Commands.Add(new UICommand("No") { Id = 1 });

            msgNote.CancelCommandIndex = 1;

            var result = await msgNote.ShowAsync();

            if (Convert.ToInt32(result.Id) == 0)
            {
                UpdateStaff(StaffStatus.Suspended.ToString());
            }
        }

        private void btnUpdateStaff_Click(object sender, RoutedEventArgs e)
        {
            UpdateStaff(StaffStatus.Active.ToString());
        }

        private async void UpdateStaff(string action)
        {
            try
            {

                staffFirstName = txtClickedItemStaffFirstName.Text;
                staffLastName = txtClickedItemStaffLastName.Text;
                staffName = staffFirstName + " " + staffLastName;
                StaffPhoneNumber = txtClickedItemStaffPhoneNumber.Text;
                StaffEmail = txtClickedItemEmail.Text;
                StaffIdNumber = txtClickedItemStaffIdNumber.Text;

                var DeptName = cmbClickedItemDeptName.SelectedItem.ToString();
                for (int i = 0; i < departmentIds.Count; i++)
                {
                    if (DeptName == departments[i].ToString())
                    {
                        DepartmentId = Convert.ToInt32(departmentIds[i].ToString());
                        break;
                    }
                }

                if (string.IsNullOrEmpty(staffName))
                {
                    MessageDialog msg = new MessageDialog("Provide Appropriate value for Staff Name");
                    await msg.ShowAsync();
                    return;
                }
                else if (string.IsNullOrEmpty(StaffPhoneNumber))
                {
                    MessageDialog msg = new MessageDialog("Provide Appropriate value for Staff Phone Number");
                    await msg.ShowAsync();
                    return;
                }
                else if (string.IsNullOrEmpty(StaffIdNumber))
                {
                    MessageDialog msg = new MessageDialog("Provide Appropriate value for Staff ID Number");
                    await msg.ShowAsync();
                    return;
                }
                else if (string.IsNullOrEmpty(StaffEmail))
                {
                    MessageDialog msg = new MessageDialog("Provide Appropriate value for Staff Email");
                    await msg.ShowAsync();
                    return;
                }
                else if (string.IsNullOrEmpty(StaffPhotoString))
                {
                    MessageDialog msg = new MessageDialog("Provide Staff Photo");
                    await msg.ShowAsync();
                    return;
                }
                else
                {
                    StaffDataPayload staff = new StaffDataPayload();
                    staff.StaffFirstName = staffFirstName;
                    staff.StaffLastName = staffLastName;
                    staff.StaffName = staffName;
                    staff.StaffEmail = StaffEmail;
                    staff.StaffPhoneNumber = StaffPhoneNumber;
                    staff.StaffIdNumber = StaffIdNumber;
                    staff.DepartmentId = DepartmentId;
                    staff.Role = cmbClickedItemStaffRole.SelectedItem.ToString();
                    staff.CompanyId = _activePage.CompanyId;
                    staff.StaffPhoto = StaffPhotoString;
                    staff.Description = "Status Changed to " + staff.StaffStatus + " by " + _activePage.UserStaffName;
                    staff.StaffStatus = action;

                    ResponseMessage msgExist = await service.CheckIfStaffExistService(staff);

                    if (msgExist.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                    {
                        staff.StaffId = msgExist.StaffId;

                        string userName = await VisitorAppUserProcess.CreateUserName(StaffEmail);
                        UserLoginDataPayLoad userDetail = new UserLoginDataPayLoad();
                        userDetail.username = userName;
                        userDetail.StaffId = staff.StaffId;
                        userDetail.userStatus = action;

                        ResponseMessage msgUpdateUser = await service.UpdateThisUserService(userDetail);
                        ResponseMessage msgUpdateStaff = await service.UpdateThisStaffControllerService(staff);

                        if (msgUpdateStaff.ResponseStatusCode == System.Net.HttpStatusCode.NotFound && msgUpdateUser.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                        {
                            CreateBinding();
                            MessageDialog ms = new MessageDialog("Action Successful");
                            await ms.ShowAsync();
                        }
                        else
                        {
                            MessageDialog ms = new MessageDialog("Err: msgUpdateUser" + msgUpdateUser.Message + "\n" + "Err: msgApp" + msgUpdateStaff.Message);
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
                checkInternet();
                MessageDialog msg = new MessageDialog("Error from Update " + ex.Message);
                //await msg.ShowAsync();
            }
        }

        private void cmbClickedItemDeptName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void btnMoveStaffDept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var staffSelected = (StaffGlobal)LstStaff.SelectedItem;

                staffFirstName = staffSelected.StaffFirstName;
                staffLastName = staffSelected.StaffLastName;
                staffName = staffSelected.StaffName;
                StaffPhoneNumber = staffSelected.StaffPhoneNumber;
                StaffEmail = staffSelected.StaffEmail;
                StaffIdNumber = staffSelected.StaffIdNumber;
                var DeptName = cmbMovingNewDept.SelectedItem.ToString();
                StaffRole = staffSelected.Roles;

                for (int i = 0; i < departmentIds.Count; i++)
                {
                    if (DeptName == departments[i].ToString())
                    {
                        DepartmentId = Convert.ToInt32(departmentIds[i].ToString());
                        break;
                    }
                }

                if (DepartmentId == staffSelected.DepartmentId)
                {
                    MessageDialog msg = new MessageDialog("Select new department for staff", "Alert!");
                    await msg.ShowAsync();
                }
                else
                {
                    StaffDataPayload staff = new StaffDataPayload();
                    staff.StaffFirstName = staffFirstName;
                    staff.StaffLastName = staffLastName;
                    staff.StaffName = staffName;
                    staff.StaffEmail = StaffEmail;
                    staff.StaffPhoneNumber = StaffPhoneNumber;
                    staff.StaffIdNumber = StaffIdNumber;
                    staff.DepartmentId = DepartmentId;
                    staff.Role = StaffRole;
                    staff.CompanyId = _activePage.CompanyId;
                    staff.StaffPhoto = staffSelected.StaffPhoto;
                    staff.Description = "staff moved from department " + staffSelected.DepartmentName + " to " + DeptName + "by " + _activePage.UserStaffName + "|" + _activePage.UserStaffId;
                    staff.StaffStatus = staffSelected.StaffStatus;

                    ResponseMessage msgExist = await service.CheckIfStaffExistService(staff);

                    if (msgExist.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                    {
                        staff.StaffId = msgExist.StaffId;

                        ResponseMessage msgApp = await service.UpdateThisStaffControllerService(staff);

                        if (msgApp.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            CreateBinding();
                            ClearMoveStaffields();
                            MessageDialog ms = new MessageDialog("Action Successful");
                            await ms.ShowAsync();
                        }
                        else
                        {
                            MessageDialog ms = new MessageDialog("Err: msgApp" + msgApp.Message);
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
                checkInternet();
                MessageDialog msg = new MessageDialog(ex.Message, "btnMove_Click");
                //await msg.ShowAsync();
            }
        }

        private void btnChangeStaffDept_Click(object sender, RoutedEventArgs e)
        {
            GridEditItemDetail.Visibility = Visibility.Collapsed;
            GridMoveStaff.Visibility = Visibility.Visible;
        }

        private void btnMoveCancel_Click(object sender, RoutedEventArgs e)
        {
            GridMoveStaff.Visibility = Visibility.Collapsed;
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
