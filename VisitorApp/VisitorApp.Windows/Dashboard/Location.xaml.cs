using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VisitorApp.Common;
using VisitorApp.Dashboard.Staff;
using VisitorApp.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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

namespace VisitorApp.Dashboard.Admin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Location : Page
    {
        AccountDetails _activePage = new AccountDetails();
        GetDataFromDB GetDataFromDB = new GetDataFromDB();
        RemoteService service = new RemoteService();
        VisitorAppHelper VisitorAppHelper = new VisitorAppHelper();
        int CompId;

        public string RawText { get; set; }
        //public bool HasHeaderRow { get; set; }
        List<string> Header { get; set; }
        List<Dictionary<string, string>> Body;

        int ThisLocationId { get; set; }

        public Location()
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
            this.Frame.Navigate(typeof(AddStaff), _activePage);
        }
        private void btnAppointment_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Appointment), _activePage);
        }
        private void btnLocation_Click(object sender, RoutedEventArgs e)
        {
            //The same Page
            //this.Frame.Navigate(typeof(Location), _activePage);
        }
        private void btnCompSetting_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CompanyProfile), _activePage);
        }
        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateBinding();
        }

        private void CreateBinding()
        {
            var theLocationSource = GetDataFromDB.GetDataCompanySecureLocation(CompId).Result;
            var theLocationSourceList = theLocationSource.SecureLocationList;
            if (theLocationSourceList != null)
            {
                LstLocation.ItemsSource = theLocationSourceList;
            }
        }
        private void ShowLocationListGrid()
        {
            GridLocationList.Visibility = Visibility.Visible;
            GridAddNewLocationList.Visibility = Visibility.Collapsed;
        }

        private void ShowAddNewLocationListGrid()
        {
            GridLocationList.Visibility = Visibility.Collapsed;
            GridAddNewLocationList.Visibility = Visibility.Visible;
            GridAddNewLocation.Visibility = Visibility.Collapsed;
        }

        private async void btnAddNewLocation_Click(object sender, RoutedEventArgs e)
        {
            //Add One New Location
            var LocationName = txtLocationName.Text.Trim();
            var LocationCode = txtLocationCode.Text.Trim();
            if (string.IsNullOrEmpty(LocationCode))
            {
                LocationCode = "0";
            }
            var LocationPhNumber = txtLocationPhoneNumber.Text.Trim();
            var LocationFloorNumber =Convert.ToInt32(txtFloorNumber.Text.Trim());

            if (string.IsNullOrEmpty(LocationName))
            {
                MessageDialog msg = new MessageDialog("Provide Appropriate value for Location Name");
                await msg.ShowAsync();
                return;
            }
            else if (string.IsNullOrEmpty(LocationCode))
            {
                MessageDialog msg = new MessageDialog("Provide Appropriate value for Location Code");
                await msg.ShowAsync();
                return;
            }
            else if (string.IsNullOrEmpty(LocationPhNumber))
            {
                MessageDialog msg = new MessageDialog("Provide Appropriate value for Location Phone Number");
                await msg.ShowAsync();
                return;
            }
            else if (string.IsNullOrEmpty(LocationFloorNumber.ToString()))
            {
                MessageDialog msg = new MessageDialog("Provide Appropriate value for Location Floor Number");
                await msg.ShowAsync();
                return;
            }
            else
            {
                newLocation(LocationName, LocationCode, LocationPhNumber, LocationFloorNumber);
            }
        }

        private void btnCloseAddLocationList_Click(object sender, RoutedEventArgs e)
        {
            ShowLocationListGrid();
        }

        private void btnAddNewLocationList_Click(object sender, RoutedEventArgs e)
        {
            //Add List of New Locations

        }

        private void btnCloseAddLocation_Click(object sender, RoutedEventArgs e)
        {
            CloseAddLocation();
        }

        private void CloseAddLocation()
        {
            GridAddNewLocation.Visibility = Visibility.Collapsed;
        }

        private async void newLocation(string LocationName, string LocationCode, string LocationPhoneNumber, int LocationFloorNumber)
        {
            try
            {
                //Register new Location
                SecureLocationDataPayload Location = new SecureLocationDataPayload();
                Location.LocationName = LocationName;
                Location.LocationCode = LocationCode;
                Location.PhoneNumber = LocationPhoneNumber;
                Location.FloorNumber = LocationFloorNumber;
                Location.CompanyId = CompId;
                Location.Status = SecureLocationStatus.Active.ToString();
                Location.Description = "Added by " + _activePage.UserStaffName;
                Location.Status = SecureLocationStatus.Active.ToString();
                Location.MapPoint = "@";

                ResponseMessage msgExist = await service.CheckIfSecureLocationExistService(Location);

                if (msgExist.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ResponseMessage msg = await service.RegisterNewSecureLocation(Location);

                    if (msg.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        MessageDialog ms = new MessageDialog("Successfully Registered");
                        await ms.ShowAsync();
                        CreateBinding();
                        ClearAddNewLocationFields();
                    }
                    else
                    {
                        MessageDialog ms = new MessageDialog("Err: newLocation-Else1 - " + msg.Message);
                        await ms.ShowAsync();
                    } 
                }
                else
                {
                    MessageDialog ms = new MessageDialog("Err: newLocation-Else2 - " + msgExist.Message);
                    await ms.ShowAsync();
                }

            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msg = new MessageDialog(ex.Message + " Void - newLocation");
                //await msg.ShowAsync();
            }
        }

        private void btnShowAddNewLocation_Click(object sender, RoutedEventArgs e)
        {
            ShowLocationListGrid();
            GridAddNewLocation.Visibility = Visibility.Visible;
            GridEditItemDetail.Visibility = Visibility.Collapsed;
            GridItemDetail.Visibility = Visibility.Collapsed;

        }

        private void btnShowAddNewLocationList_Click(object sender, RoutedEventArgs e)
        {
            ShowAddNewLocationListGrid();
        }

        private void ClearAddNewLocationFields()
        {
            txtLocationName.Text = "";
            txtLocationCode.Text = "";
            txtLocationPhoneNumber.Text = "";
            txtFloorNumber.Text = "";
            CloseAddLocation();
        }

        private void txtCheckNumber_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            CheckSyntax.checkOnlyNumber(sender, e);
        }

        private void btnItemDetailClose_Click(object sender, RoutedEventArgs e)
        {
            GridEditItemDetail.Visibility = Visibility.Collapsed;
            GridItemDetail.Visibility = Visibility.Collapsed;
        }

        private async void LstLocation_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (LstLocation.Items.Count > 0)
                {
                    LstLocation.SelectedItem = e.ClickedItem;
                    selectedItem();
                    GridItemDetail.Visibility = Visibility.Visible;
                    GridAddNewLocation.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + "- LstLocation_ItemClick");
                //await msg.ShowAsync();
            }
        }

        private async void selectedItem()
        {
            try
            {
                var Location = (SecureLocationGlobal)LstLocation.SelectedItem;

                txtClickedItemLocationName.Text = Location.LocationName;
                txtClickedItemLocationCode.Text = Location.LocationCode;
                txtClickedItemLocationPhoNo.Text = Location.PhoneNumber;
                txtClickedItemLocationFloorNo.Text = Location.FloorNumber.ToString();
                ThisLocationId = Location.LocationId;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - selectedItem");
                //await msg.ShowAsync();
            }
        }

        private void btnEditLocationDetails_Click(object sender, RoutedEventArgs e)
        {
            GridEditItemDetail.Visibility = Visibility.Visible;
        }

        private async void btnDeleteLocationDetails_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msgNote = new MessageDialog("Do you want to delete this location?", "Alert");
            msgNote.Commands.Add(new UICommand("Yes, Continue") { Id = 0 });
            msgNote.Commands.Add(new UICommand("No") { Id = 1 });

            msgNote.CancelCommandIndex = 1;

            var result = await msgNote.ShowAsync();

            if (Convert.ToInt32(result.Id) == 0)
            {
                UpdateLocation(SecureLocationStatus.Deleted.ToString());
            }
        }

        private async void btnArchiveLocationDetails_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msgNote = new MessageDialog("Do you want to archive this location?", "Alert");
            msgNote.Commands.Add(new UICommand("Yes, Continue") { Id = 0 });
            msgNote.Commands.Add(new UICommand("No") { Id = 1 });

            msgNote.CancelCommandIndex = 1;

            var result = await msgNote.ShowAsync();

            if (Convert.ToInt32(result.Id) == 0)
            {
                UpdateLocation(SecureLocationStatus.Archived.ToString());
            }
        }

        private async void btnActiveLocationDetails_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msgNote = new MessageDialog("Do you want to activate this location?", "Alert");
            msgNote.Commands.Add(new UICommand("Yes, Continue") { Id = 0 });
            msgNote.Commands.Add(new UICommand("No") { Id = 1 });

            msgNote.CancelCommandIndex = 1;

            var result = await msgNote.ShowAsync();

            if (Convert.ToInt32(result.Id) == 0)
            {
                UpdateLocation(SecureLocationStatus.Active.ToString());
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateLocation(SecureLocationStatus.Active.ToString());
        }

        private async void UpdateLocation(string action)
        {
            try
            {

                var LocationName = txtClickedItemLocationName.Text;
                var LocationCode = txtClickedItemLocationCode.Text;
                var LocationPhoneNumber = txtClickedItemLocationPhoNo.Text;
                var LocationFloorNumber = Convert.ToInt32(txtClickedItemLocationFloorNo.Text);

                if (string.IsNullOrEmpty(LocationName))
                {
                    MessageDialog msg = new MessageDialog("Provide Appropriate value for Location Name");
                    await msg.ShowAsync();
                    return;
                }
                else if (string.IsNullOrEmpty(LocationCode))
                {
                    MessageDialog msg = new MessageDialog("Provide Appropriate value for Location Code");
                    await msg.ShowAsync();
                    return;
                }
                else if (string.IsNullOrEmpty(LocationPhoneNumber))
                {
                    MessageDialog msg = new MessageDialog("Provide Appropriate value for Location Phone Number");
                    await msg.ShowAsync();
                    return;
                }
                else if (string.IsNullOrEmpty(LocationFloorNumber.ToString()))
                {
                    MessageDialog msg = new MessageDialog("Provide Appropriate value for Location Floor Number");
                    await msg.ShowAsync();
                    return;
                }
                else
                {
                    SecureLocationDataPayload Location = new SecureLocationDataPayload();
                    //Location.LocationId = ThisLocationId;
                    Location.LocationName = LocationName;
                    Location.LocationCode = LocationCode;
                    Location.PhoneNumber = LocationPhoneNumber;
                    Location.FloorNumber = LocationFloorNumber;
                    Location.CompanyId = CompId;
                    Location.Status = action;
                    Location.Description = "Status Changed to " + Location.Status+ " by " + _activePage.UserStaffName;
                    Location.MapPoint = "@";

                    ResponseMessage msgExist = await service.CheckIfSecureLocationExistService(Location);

                    if (msgExist.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                    {
                        Location.LocationId = ThisLocationId;
                        ResponseMessage msgApp = await service.UpdateThisSecureLocationControllerService(Location);

                        if (msgApp.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            CreateBinding();
                            MessageDialog ms = new MessageDialog("Action Successful");
                            await ms.ShowAsync();
                            resetGridItemDetail();
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
                }
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msg = new MessageDialog("Error from btnAddDept " + ex.Message);
                //await msg.ShowAsync();
            }
        }

        private void btnShowRefreshDeptList_Click(object sender, RoutedEventArgs e)
        {

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

        private async void populateComboBox()
        {
            //clearComboBoxes();
            try
            {
                foreach (var head in Header)
                {
                    cmbxLocation.Items.Add(head);
                    cmbxPhoneNumber.Items.Add(head);
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " - populateComboBox");
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

                //txtClickedItemDept.Background = resetBackground;
                //txtClickedItemPhoNo.Background = resetBackground;
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
                txtClickedItemLocationCode.Text = "";
                txtClickedItemLocationFloorNo.Text = "";
                txtClickedItemLocationName.Text = "";
                txtClickedItemLocationPhoNo.Text = "";
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " resetClickItemText");
                //await msg.ShowAsync();
            }
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
