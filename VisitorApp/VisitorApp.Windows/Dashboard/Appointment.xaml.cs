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
    public sealed partial class Appointment : Page
    {
        AccountDetails _activePage = new AccountDetails();
        GetDataFromDB GetDataFromDB = new GetDataFromDB();
        RemoteService service = new RemoteService();
        VisitorAppHelper VisitorAppHelper = new VisitorAppHelper();
        int CompId;

        List<string> staffIdNumber = new List<string>();
        List<string> staffIdNumberOrdered = new List<string>();
        List<int> staffIds = new List<int>();

        List<string> Locations = new List<string>();
        List<int> LocationIds = new List<int>();
        List<int> FloorNumber = new List<int>();

        int staffId;
        int LocationId;
        int FloorNo;

        string UserRole;
        int LstPopSize { get; set; }    //intitial size of listview

        public Appointment()
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

            #region For stackClickedItemHostName
            //if (_activePage.UserRole.ToUpper() == Roles.Admin.ToString().ToUpper())
            //{
            //    stackClickedItemHostName.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    stackClickedItemHostName.Visibility = Visibility.Visible;
            //}
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
            //The same Page
            //this.Frame.Navigate(typeof(Appointment), _activePage);
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
            CreateBinding();
            noted();
        }

        private void CreateBinding()
        {
            try
            {
                ResponseMessage theAppointmentSource = new ResponseMessage();
                List<AppointmentGlobal> theAppointmentSourceList = new List<AppointmentGlobal>();

                ResponseMessage theStaffSource = new ResponseMessage();
                List<StaffGlobal> theStaffSourceList = new List<StaffGlobal>();

                theStaffSource = GetDataFromDB.GetDataCompanyStaff(CompId).Result;

                if (_activePage.UserStaffRole.ToUpper() == "ADMIN")
                {
                    theAppointmentSource = GetDataFromDB.GetDataCompanyAppointment(CompId).Result;
                    //theStaffSource = GetDataFromDB.GetDataCompanyStaff(CompId).Result;    //Only admin get to see all staff
                }
                else
                {
                    var thisStaffId = _activePage.UserStaffId;
                    //theStaffSource = GetDataFromDB.GetDataThisStaff(thisStaffId).Result;  //Only 
                    theAppointmentSource = GetDataFromDB.GetDataStaffAppointment(thisStaffId).Result;
                    //staffId = thisStaffId;
                    //stackHostName.Visibility = Visibility.Collapsed;
                }

                theStaffSourceList = theStaffSource.StaffList;

                theAppointmentSourceList = theAppointmentSource.AppointmentList;

                if (theAppointmentSourceList != null)
                {
                    LstAppointment.ItemsSource = theAppointmentSourceList;
                }

                //theStaffSource = GetDataFromDB.GetDataCompanyStaff(CompId).Result;
                //theStaffSourceList = theStaffSource.StaffList;

                var theLocationSource = GetDataFromDB.GetDataCompanySecureLocation(CompId).Result;
                var theLocationSourceList = theLocationSource.SecureLocationList;

                staffIdNumber = new List<string>();
                staffIds = new List<int>();


                Locations = new List<string>();
                LocationIds = new List<int>();
                FloorNumber = new List<int>();

                if (theStaffSourceList != null)
                {
                    foreach (var item in theStaffSourceList)
                    {
                        staffIdNumber.Add(item.StaffIdNumber);
                        staffIds.Add(item.StaffId);
                    }

                    //THis is probably LAME!!! but that's teh ay to get teh cmbobox to have the IDs in ascending order
                    foreach (var item in theStaffSourceList.OrderBy(x => x.StaffIdNumber))
                    {
                        staffIdNumberOrdered.Add(item.StaffIdNumber);
                    }
                    //staffIdNumber = staffIdNumber.OrderBy();
                    cmbstaffIdNumber.ItemsSource = null;
                    cmbstaffIdNumber.ItemsSource = staffIdNumberOrdered;
                }

                if (theLocationSourceList != null)
                {
                    foreach (var item in theLocationSourceList)
                    {
                        Locations.Add(item.LocationName);
                        LocationIds.Add(item.LocationId);
                        FloorNumber.Add(item.FloorNumber);
                    }
                    cmbMeetingLocation.ItemsSource = null;
                    cmbMeetingLocation.ItemsSource = Locations;
                }
                lastRefreshed();
            }
            catch (Exception ex)
            {
                checkInternet();

                MessageDialog msgErr = new MessageDialog(ex.Message, "Error!");
            }
        }

        private void ShowAppointmentListGrid()
        {
            GridAppointmentList.Visibility = Visibility.Visible;
        }

        private async void btnAddNewAppointment_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDateTime = new DateTime();
            DateTime endDateTime = new DateTime();

            var startDate = dtpMeetingStartDate.Date;
            var startTime = tpMeetingStartTime.Time;

            var endDate = dtpMeetingEndDate.Date;
            var endTime = tpMeetingEndTime.Time;

            string startTimeString = startDate.Year.ToString() + "/" + startDate.Month.ToString() + "/" +startDate.Day.ToString() + " "+ startTime.ToString();

            startDateTime = DateTime.Parse(startTimeString);


            string endTimeString = endDate.Year.ToString() + "/" + endDate.Month.ToString() + "/" + endDate.Day.ToString() + " " + endTime.ToString();

            endDateTime = DateTime.Parse(endTimeString);

            MessageDialog msg = new MessageDialog("");

            if (endDateTime==startDateTime)
            {
                msg = new MessageDialog("Meeting End and Start time are the same");
                msg.ShowAsync();
                return;
            }

            else if (endDateTime < startDateTime)
            {
                msg = new MessageDialog("Meeting End time cannot be before Meeting start time");
                msg.ShowAsync();
                return;
            }
            else if (endDateTime > startDateTime)
            {              
                string GuestName = txtGuestName.Text;
                string GuestCompanyName = txtGuestCompanyName.Text;
                string GuestPhoneNumber = txtGuestPhoneNumber.Text;
                //string InvitationCode = txtIvCode.Text;
                DateTime MeetingStartDateTime = startDateTime;
                DateTime MeetingEndDateTime = endDateTime;
                string desc = txtDescription.Text;
                MessageDialog msgEmpty = new MessageDialog("");

                if (string.IsNullOrEmpty(GuestName.Trim()))
                {
                    msgEmpty = new MessageDialog("Enter Guest Name", "Fill Missing");
                    await msgEmpty.ShowAsync();
                    return;
                }
                else if (string.IsNullOrEmpty(GuestCompanyName.Trim()))
                {
                    msgEmpty = new MessageDialog("Enter Company Name", "Fill Missing");
                    await msgEmpty.ShowAsync();
                    return;
                }
                else if (string.IsNullOrEmpty(GuestPhoneNumber.Trim()))
                {
                    msgEmpty = new MessageDialog("Enter Guest Phone Number", "Fill Missing");
                    await msgEmpty.ShowAsync();
                    return;
                }
                else if (string.IsNullOrEmpty(desc.Trim()))
                {
                    msgEmpty = new MessageDialog("Enter Appointment Description", "Fill Missing");
                    await msgEmpty.ShowAsync();
                    return;
                }
                else if (cmbMeetingLocation.SelectedItem == null)
                {
                    msgEmpty = new MessageDialog("Select Meeting Location", "Fill Missing");
                    await msgEmpty.ShowAsync();
                    return;
                }
                else if (cmbstaffIdNumber.SelectedItem == null && (_activePage.UserStaffRole.ToUpper() != StaffRoles.Admin.ToString().ToUpper()))
                {
                    msgEmpty = new MessageDialog("Select Staff ID Number", "Fill Missing");
                    await msgEmpty.ShowAsync();
                    return;
                }
                else if ((!string.IsNullOrEmpty(txtGuestEmail.Text.Trim())) && (!txtGuestEmail.Text.Contains('@') || !txtGuestEmail.Text.Contains('.')))
                {
                    MessageDialog md = new MessageDialog("Please Enter a Valid Email for Guest.\n" + "If you do not have the email leave it blank.", "Fill Missing");
                    await md.ShowAsync();
                    return;
                }
                else
                {
                    newAppointment(GuestName, GuestCompanyName, GuestPhoneNumber, MeetingStartDateTime, MeetingEndDateTime);

                }
            }
        }

        private void btnCloseAddAppointmentList_Click(object sender, RoutedEventArgs e)
        {
            ShowAppointmentListGrid();
        }

        private void btnAddNewAppointmentList_Click(object sender, RoutedEventArgs e)
        {
            //Add List of New Appointments

        }

        private void btnCloseAddAppointment_Click(object sender, RoutedEventArgs e)
        {
            CloseAddAppointment();
        }

        private void CloseAddAppointment()
        {
            GridAddNewAppointment.Visibility = Visibility.Collapsed;
        }

        private async void newAppointment(string GuesttName, string GuestCompany, string GuestPhoneNumber, DateTime MeetingStartDateTime, DateTime MeetingEndDateTime)
        {
            try
            {
                //Register new Appointment.

                string ccode = await VisitorAppHelper.GenerateIvCode();
                txtIvCode.Text = ccode;
                string InvitationCode = ccode;

                string LocationName = cmbMeetingLocation.SelectedItem.ToString();
                for (int i = 0; i < LocationIds.Count; i++)
                {
                    if (LocationName == Locations[i].ToString())
                    {
                        LocationId = Convert.ToInt32(LocationIds[i].ToString());
                        FloorNo = Convert.ToInt32(FloorNumber[i].ToString());
                        txbFloorNumber.Text = FloorNo.ToString();
                        break;
                    }
                }

                string staffIdno="";

                var selectedIndex = cmbstaffIdNumber.SelectedIndex;
                staffIdno = cmbstaffIdNumber.Items[selectedIndex].ToString();
                for (int i = 0; i <= staffIds.Count - 1; i++)
                {
                    if (staffIdno == staffIdNumber[i].ToString())
                    {
                        staffId = Convert.ToInt32(staffIds[i].ToString());
                        break;
                    }
                }
               
                AppointmentDataPayload appointment = new AppointmentDataPayload();
                appointment.GuestName = GuesttName;
                appointment.GuestCompanyName = GuestCompany;
                appointment.GuestPhoneNumber = GuestPhoneNumber;
                appointment.GuestEmail = txtGuestEmail.Text.Trim();
                appointment.InvitationCode = InvitationCode;
                appointment.HostStaffId = staffId;
                appointment.CreatedByStaffId = _activePage.UserStaffId;
                appointment.LocationId = LocationId;
                appointment.CompanyId = CompId;
                appointment.MeetingStartDateTime = MeetingStartDateTime;
                appointment.MeetingEndDateTime = MeetingEndDateTime;
                appointment.Description = txtDescription.Text + ". Added by " + _activePage.UserStaffId;
                appointment.Status = AppointmentStatus.Pending.ToString();//"Pending";
                   
                ResponseMessage msgExist = await service.CheckIfAppointmentExistService(appointment);

                if (msgExist.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ResponseMessage msg = await service.RegisterNewAppointment(appointment);

                    //if (!string.IsNullOrEmpty(appointment.GuestEmail))
                    //{
                        ResponseMessage msgEmail = await service.EmailRegisterNewAppointment(appointment);
                    //}

                    if (msg.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        newVisitor();
                        MessageDialog ms = new MessageDialog("Successfully Registered");
                        ms.ShowAsync();
                        CreateBinding();
                        noted();
                        ClearAddNewAppointmentFields();
                        GridEditItemDetail.Visibility = Visibility.Collapsed;
                        GridAddNewAppointment.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        MessageDialog ms = new MessageDialog("Err: " + msg.Message);
                        ms.ShowAsync();
                    }
                }
                else
                {
                    MessageDialog ms = new MessageDialog("Err: " + msgExist.Message);
                    ms.ShowAsync();
                }

            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msg = new MessageDialog(ex.Message + " Void - newAppointment");
                //msg.ShowAsync();
            }
        }

        private async void btnShowAddNewAppointment_Click(object sender, RoutedEventArgs e)
        {
            ShowAppointmentListGrid();
            GridAddNewAppointment.Visibility = Visibility.Visible;          
        }

        private void ClearAddNewAppointmentFields()
        {
            txtGuestName.Text = "";
            txtGuestCompanyName.Text = "";
            txtGuestPhoneNumber.Text = "";
            txtIvCode.Text = "";
            txtDescription.Text = "";
            dtpMeetingEndDate.Date = DateTime.Now;
            dtpMeetingStartDate.Date = DateTime.Now;
            tpMeetingStartTime.Time = DateTime.Now.TimeOfDay;
            tpMeetingEndTime.Time = DateTime.Now.TimeOfDay;
            CloseAddAppointment();
        }

        private void txtCheckNumber_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            CheckSyntax.checkOnlyNumber(sender, e);
        }

        private void cmbstaffIdNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                //var selectedIndex = cmbstaffIdNumber.SelectedIndex;
                //var staffIdno = cmbstaffIdNumber.Items[selectedIndex].ToString();
                //for (int i = 0; i <= staffIds.Count-1; i++)
                //{
                //    if (staffIdno == staffIdNumber[i].ToString())
                //    {
                //        staffId = Convert.ToInt32(staffIds[i].ToString());
                //        break;
                //    }
                //}

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - cmbstaffIdNumber_SelectionChanged");
                //msg.ShowAsync();
            }
        }

        private void cmbMeetingLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbMeetingLocation.SelectedItem != null)
                {
                    string LocationName = cmbMeetingLocation.SelectedItem.ToString();
                    for (int i = 0; i < LocationIds.Count; i++)
                    {
                        if (LocationName == Locations[i].ToString())
                        {
                            LocationId = Convert.ToInt32(LocationIds[i].ToString());
                            FloorNo = Convert.ToInt32(FloorNumber[i].ToString());
                            txbFloorNumber.Text = FloorNo.ToString();
                            break;
                        }
                    } 
                }
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msgErr = new MessageDialog(ex.Message + "cmbMeetingLocation_SelectionChanged");
                //msgErr.ShowAsync();
            }
        }

        private void cmbClickedItemMeetingLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                MessageDialog msgErr = new MessageDialog(ex.Message + "cmbClickedItemMeetingLocation_SelectionChanged");
                //msgErr.ShowAsync();
            }
        }

        private void btnItemDetailClose_Click(object sender, RoutedEventArgs e)
        {
            GridEditItemDetail.Visibility = Visibility.Collapsed;
            GridItemDetail.Visibility = Visibility.Collapsed;
        }

        private void btnEditAppointmentDetails_Click(object sender, RoutedEventArgs e)
        {
            GridEditItemDetail.Visibility = Visibility.Visible;
        }

        private void LstAppointment_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (LstAppointment.Items.Count > 0)
            {
                LstAppointment.SelectedItem = e.ClickedItem;
                selectedItem();

                string LocationName = cmbClickedItemMeetingLocation.SelectedItem.ToString();
                for (int i = 0; i < LocationIds.Count; i++)
                {
                    if (LocationName == Locations[i].ToString())
                    {
                        LocationId = Convert.ToInt32(LocationIds[i].ToString());
                        var LocationFloorNumber = Convert.ToInt32(FloorNumber[i].ToString());
                        txbClickedItemFloorNumber.Text = LocationFloorNumber.ToString();
                        break;
                    }
                }

                string staffIdno = cmbClickedItemstaffIdNumber.SelectedItem.ToString();
                if (staffIdno != null)
                {
                    for (int i = 0; i < staffIds.Count; i++)
                    {
                        if (staffIdno == staffIdNumber[i].ToString())
                        {
                            staffId = Convert.ToInt32(staffIds[i].ToString());
                            break;
                        }
                    }

                }

                GridItemDetail.Visibility = Visibility.Visible;
                GridAddNewAppointment.Visibility = Visibility.Collapsed;
            }
        }

        private void selectedItem()
        {
            try
            {
                var Appointment = (AppointmentGlobal)LstAppointment.SelectedItem;
                List<StaffGlobal> theStaffSourceList = new List<StaffGlobal>();
                ResponseMessage theStaffSource = new ResponseMessage();
                theStaffSource = GetDataFromDB.GetDataCompanyStaff(CompId).Result;
                theStaffSourceList = theStaffSource.StaffList;
                //var staffIdNumber = (StaffDataPayload)LstAppointment.SelectedItem;

                cmbClickedItemMeetingLocation.ItemsSource = cmbMeetingLocation.ItemsSource;
                cmbClickedItemstaffIdNumber.ItemsSource = cmbstaffIdNumber.ItemsSource;

                for (int i = 0; i < cmbClickedItemMeetingLocation.Items.Count; i++)
                {
                    string LocationName = cmbMeetingLocation.Items[i].ToString();
                    if (LocationName == Appointment.LocationName)
                    {
                        cmbClickedItemMeetingLocation.SelectedIndex = i;
                        break;
                    }
                }


                for (int i = 0; i <= cmbClickedItemstaffIdNumber.Items.Count - 1; i++)
                {
                    string staffIdNo = cmbstaffIdNumber.Items[i].ToString();
                    foreach (var item in theStaffSourceList)
                    {
                        if (staffIdNo == item.StaffIdNumber && item.StaffId == Appointment.HostStaffId)
                        {
                            var selectedStaffId = item.StaffId;

                            if (selectedStaffId == Appointment.HostStaffId)
                            {
                                cmbClickedItemstaffIdNumber.SelectedIndex = i;
                                //break;
                            }

                            if (selectedStaffId == _activePage.UserStaffId)
                            {
                                txbClickedItemHostName.Text = "Me";
                            }
                            else
                            {
                                txbClickedItemHostName.Text = Appointment.StaffName;
                            }
                        }
                    }
                }

                if (Appointment.CreatedByStaffId != _activePage.UserStaffId)
                {
                    foreach (var item in theStaffSourceList)
                    {
                        if (Appointment.CreatedByStaffId == item.StaffId)
                        {
                            txbClickedItemCreatedBy.Text = item.StaffName;
                        }
                    }
                }
                else
                {
                    txbClickedItemCreatedBy.Text = "Me";
                }

                txtClickedItemGuestName.Text = Appointment.GuestName;
                txtClickedItemGuestCompanyName.Text = Appointment.GuestCompanyName;
                txtClickedItemGuestPhoNo.Text = Appointment.GuestPhoneNumber;
                txbClickedItemFloorNumber.Text = "0";
                txbClickedItemIvCode.Text = Appointment.InvitationCode;
                dtpClickedItemMeetingStartDate.Date = Appointment.MeetingStartDateTime.Date;
                dtpClickedItemMeetingEndDate.Date = Appointment.MeetingEndDateTime.Date;
                tpClickedItemMeetingStartTime.Time = Appointment.MeetingStartDateTime.TimeOfDay;
                tpClickedItemMeetingEndTime.Time = Appointment.MeetingEndDateTime.TimeOfDay;
                //staffId = Appointment.StaffId;
                stackClickedItemHostName.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msg = new MessageDialog(ex.Message + " Void - selectedItem");
                //msg.ShowAsync();
            }
        }

        private async void newVisitor()
        {
            try
            {
                var GuestName = txtGuestName.Text;
                var GuestCompany = txtGuestCompanyName.Text;
                var GuestPhoneNumber = txtGuestPhoneNumber.Text;
                var GuestEmail = txtGuestEmail.Text;

                //Register new Visitor
                VisitorDataPayLoad visitor = new VisitorDataPayLoad();
                visitor.CompanyName = GuestCompany;
                visitor.FullName = GuestName;
                visitor.PhoneNumber = Convert.ToInt64(GuestPhoneNumber);
                if (!string.IsNullOrEmpty(GuestEmail))
                {
                    visitor.EmailAddress = GuestEmail;
                }
                visitor.Signature = "@";
                visitor.ThumbPrint = "@";
                visitor.CompanyId = _activePage.CompanyId;
                visitor.Description = "Added via Appointmemt by " + _activePage.UserStaffId;
                //RemoteService service = new RemoteService();
                ResponseMessage msg = await service.RegisterNewVisitor(visitor);
                if (msg.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageDialog md = new MessageDialog("Registered: " + msg.Message);
                    //await md.ShowAsync();
                    //return;
                }
                else if (msg.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                {
                    MessageDialog md = new MessageDialog("Already Registered: " + msg.Message);
                    //await md.ShowAsync();
                    return;
                }
                else
                {
                    MessageDialog md = new MessageDialog("Could not complete registration: " + msg.Message);
                    //await md.ShowAsync();
                    return;
                }
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msg = new MessageDialog(ex.Message + " Void - newVisitor");
                //msg.ShowAsync();
            }
        }

        private async void btnDoneAppointmentDetails_Click(object sender, RoutedEventArgs e)
        {
            var action = AppointmentStatus.Done.ToString();
            MessageDialog msgNote = new MessageDialog("Do you want to mark as " + action + " for this appointment?", "Alert");
            msgNote.Commands.Add(new UICommand("Yes, Continue") { Id = 0 });
            msgNote.Commands.Add(new UICommand("No") { Id = 1 });

            msgNote.CancelCommandIndex = 1;

            var result = await msgNote.ShowAsync();

            if (Convert.ToInt32(result.Id) == 0)
            {
                updateAppointment(AppointmentStatus.Done.ToString());
            }
        }

        private async void btnCancelAppointmentDetails_Click(object sender, RoutedEventArgs e)
        {
            var action = AppointmentStatus.Cancel.ToString();
            MessageDialog msgNote = new MessageDialog("Do you want to " + action + " this appointment?", "Alert");
            msgNote.Commands.Add(new UICommand("Yes, Continue") { Id = 0 });
            msgNote.Commands.Add(new UICommand("No") { Id = 1 });

            msgNote.CancelCommandIndex = 1;

            var result = await msgNote.ShowAsync();

            if (Convert.ToInt32(result.Id) == 0)
            {
                updateAppointment(AppointmentStatus.Cancel.ToString());
            }
        }

        private async void btnDeleteAppointmentDetails_Click(object sender, RoutedEventArgs e)
        {
            var action = AppointmentStatus.Delete.ToString();
            MessageDialog msgNote = new MessageDialog("Do you want to " + action + " this appointment?", "Alert");
            msgNote.Commands.Add(new UICommand("Yes, Continue") { Id = 0 });
            msgNote.Commands.Add(new UICommand("No") { Id = 1 });

            msgNote.CancelCommandIndex = 1;

            var result = await msgNote.ShowAsync();

            if (Convert.ToInt32(result.Id) == 0)
            {
                updateAppointment(AppointmentStatus.Delete.ToString());
            }
        }

        private async void btnPendAppointmentDetails_Click(object sender, RoutedEventArgs e)
        {
            var action = AppointmentStatus.Pending.ToString();
            MessageDialog msgNote = new MessageDialog("Do you want to mark as " + action + " for this appointment?", "Alert");
            msgNote.Commands.Add(new UICommand("Yes, Continue") { Id = 0 });
            msgNote.Commands.Add(new UICommand("No") { Id = 1 });

            msgNote.CancelCommandIndex = 1;

            var result = await msgNote.ShowAsync();

            if (Convert.ToInt32(result.Id) == 0)
            {
                updateAppointment(AppointmentStatus.Pending.ToString());
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

            updateAppointment(AppointmentStatus.Pending.ToString());
        }

        private async void updateAppointment(string Action)
        {
            try
            {
                //Register new Appointment or Update an appointment
               #region Perform Action

                DateTime startDateTime = new DateTime();
                DateTime endDateTime = new DateTime();

                var startDate = dtpClickedItemMeetingStartDate.Date;
                var startTime = tpClickedItemMeetingStartTime.Time;

                var endDate = dtpClickedItemMeetingEndDate.Date;
                var endTime = tpClickedItemMeetingEndTime.Time;

                string startTimeString = startDate.Year.ToString() + "/" + startDate.Month.ToString() + "/" + startDate.Day.ToString() + " " + startTime.ToString();

                startDateTime = DateTime.Parse(startTimeString);


                string endTimeString = endDate.Year.ToString() + "/" + endDate.Month.ToString() + "/" + endDate.Day.ToString() + " " + endTime.ToString();

                endDateTime = DateTime.Parse(endTimeString);

                MessageDialog msg = new MessageDialog("");

                if (endDateTime == startDateTime)
                {
                    msg = new MessageDialog("Meeting End and Start time are Equal");
                    msg.ShowAsync();
                    return;
                }

                else if (endDateTime < startDateTime)
                {
                    msg = new MessageDialog("Meeting End time is less than Meeting start time");
                    msg.ShowAsync();
                    return;
                }

                else if (endDateTime > startDateTime)
                {
                    if (_activePage.UserStaffRole.ToUpper() == StaffRoles.Admin.ToString().ToUpper())
                    {
                        string staffIdno = cmbClickedItemstaffIdNumber.SelectedItem.ToString();
                        for (int i = 0; i < staffIds.Count; i++)
                        {
                            if (staffIdno == staffIdNumber[i].ToString())
                            {
                                staffId = Convert.ToInt32(staffIds[i].ToString());
                                break;
                            }
                        }
                    }
                    else
                    {
                        staffId = _activePage.UserStaffId;
                    }

                    string LocationName = cmbClickedItemMeetingLocation.SelectedItem.ToString();
                    for (int i = 0; i < LocationIds.Count; i++)
                    {
                        if (LocationName == Locations[i].ToString())
                        {
                            LocationId = Convert.ToInt32(LocationIds[i].ToString());
                            var LocationFloorNumber = Convert.ToInt32(FloorNumber[i].ToString());
                            txbClickedItemFloorNumber.Text = LocationFloorNumber.ToString();
                            break;
                        }
                    }

                    string GuestName = txtClickedItemGuestName.Text;
                    string GuestCompanyName = txtClickedItemGuestCompanyName.Text;
                    string GuestPhoneNumber = txtClickedItemGuestPhoNo.Text;
                    string InvitationCode = txbClickedItemIvCode.Text;
                    DateTime MeetingStartDateTime = startDateTime;
                    DateTime MeetingEndDateTime = endDateTime;

                    AppointmentDataPayload appointment = new AppointmentDataPayload();
                    appointment.GuestName = GuestName;
                    appointment.GuestCompanyName = GuestCompanyName;
                    appointment.GuestPhoneNumber = GuestPhoneNumber;
                    appointment.InvitationCode = InvitationCode;
                    appointment.HostStaffId = staffId;
                    appointment.LocationId = LocationId;
                    appointment.CompanyId = CompId;
                    appointment.MeetingStartDateTime = MeetingStartDateTime;
                    appointment.MeetingEndDateTime = MeetingEndDateTime;
                    appointment.Description = Action + " by " + _activePage.UserStaffId + " @ " + DateTime.Now.ToString();
                    appointment.Status = Action;//"Pending";


                    ResponseMessage msgExist = await service.CheckIfAppointmentExistService(appointment);

                    if (msgExist.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                    {
                        appointment.AppointmentId = msgExist.AppointmentId;
                        ResponseMessage msgApp = await service.UpdateThisAppointmentControllerService(appointment);

                        if (msgApp.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            CreateBinding();
                            noted();
                            GridEditItemDetail.Visibility = Visibility.Collapsed;
                            GridItemDetail.Visibility = Visibility.Collapsed;                          
                            GridAddNewAppointment.Visibility = Visibility.Collapsed;
                            MessageDialog ms = new MessageDialog("Action Successful");
                            await ms.ShowAsync();
                        }
                        else
                        {
                            MessageDialog ms = new MessageDialog("Err: " + msgApp.Message);
                            ms.ShowAsync();
                        }
                    }
                    else
                    {
                        MessageDialog ms = new MessageDialog("Err: " + msgExist.Message);
                        ms.ShowAsync();
                    }

                }

                #endregion
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msgError = new MessageDialog("Err: " + ex.Message);
                //msgError.ShowAsync();
            }
        }

        private void cmbClickedItemstaffIdNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - cmbItemClickedStaffIdNumber_SelectionChanged");
                //msg.ShowAsync();
            }
        }

        private async void btnShowRefreshAppointmentList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LstPopSize != LstAppointment.Items.Count())
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
                checkInternet();
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
            LstPopSize = LstAppointment.Items.Count;    //Current number of Appointments.
        }

        private void cmbClickedItemstaffIdNumber_Tapped(object sender, TappedRoutedEventArgs e)
        {
            stackClickedItemHostName.Visibility = Visibility.Collapsed;
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