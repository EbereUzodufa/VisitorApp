using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VisitorApp.Common;
using VisitorApp.Dashboard.Admin;
using VisitorApp.Dashboard.Staff;
using VisitorApp.Models;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Input.Inking;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VisitorApp.Dashboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CheckInGuest : Page
    {
        //Delcare Important variables
        private string GuestName;
        private string GuestCompany;
        private string Guestemail;
        private string GuestPhoneNumber;
        private string GuestPhotoString;
        private bool GuestFound = false;

        private int HostId;
        private string HostName;
        private string HostPhoneNumber;
        private string HostDept;
        private string HostDeptPhoneNo;
        private string HostPhotoString;
        private bool HostFound = false;
        private bool halfOne = false;
        private bool halfTwo = false;

        //Signature and IV
        private string ivCode;
        private string base64String;

        private string meetingStartTime;
        private string meetingEndTime;
        private string meetingLocationName;
        private string meetingLocationFloor;
        string JSONGuestColleague { get; set; }

        AccountDetails _activePage = new AccountDetails();
        RemoteService service = new RemoteService();
        GetDataFromDB GetDataFromDB = new GetDataFromDB();
        VisitorAppHelper VisitorAppHelper = new VisitorAppHelper();
        VisitorAppGuestColleague VisitorAppGuestColleague = new VisitorAppGuestColleague();

        //Delcare Guest Colleague variables
        private string GuestColleagueName;
        private string GuestColleagueCompany;
        private string GuestColleagueemail;
        private string GuestColleaguePhoneNumber;
        private string GuestColleaguePhotoString;

        // Scenario specific constants and variables.
        const double STROKETHICKNESS = 5;
        Point _previousContactPt;
        uint _penID = 0;
        uint _touchID = 0;

        // Create the InkManager instance.
        InkManager _inkManager = new InkManager();

        public int CompId { get; private set; }

        List<GuestColleagueDetails> GuestColleagues = new List<GuestColleagueDetails>();

        public CheckInGuest()
        {
            this.InitializeComponent();
            InkCanvas.PointerPressed += new PointerEventHandler(InkCanvas_PointerPressed);
            InkCanvas.PointerMoved += new PointerEventHandler(InkCanvas_PointerMoved);
            InkCanvas.PointerReleased += new PointerEventHandler(InkCanvas_PointerReleased);
            InkCanvas.PointerExited += new PointerEventHandler(InkCanvas_PointerReleased);
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
            //The same Page
            //this.Frame.Navigate(typeof(CheckInGuest), _activePage);
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

        public void InkCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                // Get information about the pointer location.
                PointerPoint pt = e.GetCurrentPoint(InkCanvas);
                _previousContactPt = pt.Position;

                // Accept input only from a pen or mouse with the left button pressed. 
                PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;
                if (pointerDevType == PointerDeviceType.Pen ||
                        pointerDevType == PointerDeviceType.Mouse &&
                        pt.Properties.IsLeftButtonPressed)
                {
                    // Pass the pointer information to the InkManager.
                    _inkManager.ProcessPointerDown(pt);
                    _penID = pt.PointerId;

                    e.Handled = true;
                }

                else if (pointerDevType == PointerDeviceType.Touch)
                {
                    // Process touch input
                }

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - InkCanvas_PointerPressed");
                //msg.ShowAsync();
            }
        }

        public void InkCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (e.Pointer.PointerId == _penID)
                {
                    PointerPoint pt = e.GetCurrentPoint(InkCanvas);

                    // Render a red line on the canvas as the pointer moves. 
                    // Distance() is an application-defined function that tests
                    // whether the pointer has moved far enough to justify 
                    // drawing a new line.
                    Point currentContactPt = pt.Position;
                    if (Distance(currentContactPt, _previousContactPt) > 2)
                    {
                        Line line = new Line()
                        {
                            X1 = _previousContactPt.X,
                            Y1 = _previousContactPt.Y,
                            X2 = currentContactPt.X,
                            Y2 = currentContactPt.Y,
                            StrokeThickness = STROKETHICKNESS,
                            Stroke = new SolidColorBrush(Windows.UI.Colors.Red)
                        };

                        _previousContactPt = currentContactPt;

                        // Draw the line on the canvas by adding the Line object as
                        // a child of the Canvas object.
                        InkCanvas.Children.Add(line);

                        // Pass the pointer information to the InkManager.
                        _inkManager.ProcessPointerUpdate(pt);
                    }
                }

                else if (e.Pointer.PointerId == _touchID)
                {
                    // Process touch input
                }

                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - InkCanvas_PointerMoved");
                //msg.ShowAsync();
            }
        }

        public void InkCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (e.Pointer.PointerId == _penID)
                {
                    PointerPoint pt = e.GetCurrentPoint(InkCanvas);

                    // Pass the pointer information to the InkManager. 
                    _inkManager.ProcessPointerUp(pt);
                }

                else if (e.Pointer.PointerId == _touchID)
                {
                    // Process touch input
                }

                _touchID = 0;
                _penID = 0;

                // Call an application-defined function to render the ink strokes.
                RenderAllStrokes();

                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - InkCanvas_PointerReleased");
                //msg.ShowAsync();
            }
        }

        private void RenderAllStrokes()
        {
            try
            {
                // Clear the canvas.
                InkCanvas.Children.Clear();

                // Get the InkStroke objects.
                IReadOnlyList<InkStroke> inkStrokes = _inkManager.GetStrokes();

                // Process each stroke.
                foreach (InkStroke inkStroke in inkStrokes)
                {
                    PathGeometry pathGeometry = new PathGeometry();
                    PathFigureCollection pathFigures = new PathFigureCollection();
                    PathFigure pathFigure = new PathFigure();
                    PathSegmentCollection pathSegments = new PathSegmentCollection();

                    // Create a path and define its attributes.
                    Windows.UI.Xaml.Shapes.Path path = new Windows.UI.Xaml.Shapes.Path();
                    path.Stroke = new SolidColorBrush(Colors.Red);
                    path.StrokeThickness = STROKETHICKNESS;

                    // Get the stroke segments.
                    IReadOnlyList<InkStrokeRenderingSegment> segments;
                    segments = inkStroke.GetRenderingSegments();

                    // Process each stroke segment.
                    bool first = true;
                    foreach (InkStrokeRenderingSegment segment in segments)
                    {
                        // The first segment is the starting point for the path.
                        if (first)
                        {
                            pathFigure.StartPoint = segment.BezierControlPoint1;
                            first = false;
                        }

                        // Copy each ink segment into a bezier segment.
                        BezierSegment bezSegment = new BezierSegment();
                        bezSegment.Point1 = segment.BezierControlPoint1;
                        bezSegment.Point2 = segment.BezierControlPoint2;
                        bezSegment.Point3 = segment.Position;

                        // Add the bezier segment to the path.
                        pathSegments.Add(bezSegment);
                    }

                    // Build the path geometerty object.
                    pathFigure.Segments = pathSegments;
                    pathFigures.Add(pathFigure);
                    pathGeometry.Figures = pathFigures;

                    // Assign the path geometry object as the path data.
                    path.Data = pathGeometry;

                    // Render the path by adding it as a child of the Canvas object.
                    InkCanvas.Children.Add(path);
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - RenderAllStrokes");
                //msg.ShowAsync();
            }
        }

        private double Distance(Point currentContact, Point previousContact)
        {
            return Math.Sqrt(Math.Pow(currentContact.X - previousContact.X, 2) +
                    Math.Pow(currentContact.Y - previousContact.Y, 2));
        }

        private async void CheckInButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                base64String = "";
                if (_inkManager.GetStrokes() != null && _inkManager.GetStrokes().Count > 0)
                {
                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
                    await renderTargetBitmap.RenderAsync(InkCanvas);
                    //PhotoControl.Source = renderTargetBitmap;
                    using (var stream = new InMemoryRandomAccessStream())
                    {
                        await _inkManager.SaveAsync(stream);
                        await stream.FlushAsync();
                        stream.Seek(0);
                        //byte[] bytes = new byte[stream.Size];

                        Byte[] bytes = new Byte[0];
                        var reader = new DataReader(stream.GetInputStreamAt(0));
                        bytes = new Byte[stream.Size];
                        await reader.LoadAsync((uint)stream.Size);
                        reader.ReadBytes(bytes);
                        // Convert the byte array to Base 64 string
                        base64String = Convert.ToBase64String(bytes);
                        ////PhotoCopy.Source = await ImageProcessor.Base64StringToBitmap(base64String);
                    }

                    if (stackPanelIV.Visibility==Visibility.Visible)
                    {
                        if (txtInvitationCode.Text.Trim()!="")
                        {
                            btnCheckIV_Click(sender,e);
                            if (string.IsNullOrEmpty(txtGuestFullName.Text.Trim()))
                            {
                                MessageDialog md = new MessageDialog("Please enter Guest full name");
                                await md.ShowAsync();
                                return;
                            }

                            else if (string.IsNullOrEmpty(txtGuestCompanyName.Text.Trim()))
                            {
                                MessageDialog md = new MessageDialog("Please enter Guest company Name");
                                await md.ShowAsync();
                                return;
                            }
                            else if (imgGuestPhoto.Source == null)
                            {
                                MessageDialog md = new MessageDialog("Please Select Photo");
                                await md.ShowAsync();
                                return;
                            }
                            else if (string.IsNullOrEmpty(txtGuestEmail.Text.Trim()))
                            {
                                MessageDialog md = new MessageDialog("Please enter Email");
                                await md.ShowAsync();
                                return;
                            }
                            else if (!txtGuestEmail.Text.Contains('@') || !txtGuestEmail.Text.Contains('.'))
                            {
                                MessageDialog md = new MessageDialog("Please Enter a Valid Email");
                                await md.ShowAsync();
                                return;
                            }
                            else
                            {
                                if (GuestFound == true && HostFound == true)
                                {
                                    VisitorDataPayLoad payload = new VisitorDataPayLoad
                                    {
                                        PhoneNumber = Convert.ToInt64(GuestPhoneNumber)
                                    };

                                    var response = await service.CheckIfVisitorExistService(payload);
                                    if (response.ResponseStatusCode == System.Net.HttpStatusCode.NotFound || response.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                                    {
                                        getGuestDetailfromInput();
                                        newVisitor();
                                        closeEditAddColleague();
                                    }
                                } 
                            }
                        }
                    }
                    else if (stackNoIV.Visibility == Visibility.Visible  && stackGuestDetail.Visibility == Visibility.Collapsed)
                    {
                        newVisitor();
                    }
                    else if (stackGuestDetail.Visibility == Visibility.Visible)
                    {
                        if (string.IsNullOrEmpty(txtGuestFullName.Text.Trim()))
                        {
                            MessageDialog md = new MessageDialog("Please enter Guest full name");
                            await md.ShowAsync();
                            return;
                        }

                        else if (string.IsNullOrEmpty(txtHostPhoneNumber.Text.Trim()))
                        {
                            MessageDialog md = new MessageDialog("Please enter Host phone Number");
                            await md.ShowAsync();
                            return;
                        }

                        else if (string.IsNullOrEmpty(txtGuestPhoneNumber.Text.Trim()))
                        {
                            MessageDialog md = new MessageDialog("Please enter Guest phone Number");
                            await md.ShowAsync();
                            return;
                        }

                        else if (string.IsNullOrEmpty(txtGuestCompanyName.Text.Trim()))
                        {
                            MessageDialog md = new MessageDialog("Please enter Guest company Name");
                            await md.ShowAsync();
                            return;
                        }
                        else if (GuestPhotoString == "")
                        {
                            MessageDialog md = new MessageDialog("Please Select Photo");
                            await md.ShowAsync();
                            return;
                        }
                        else if (string.IsNullOrEmpty(txtGuestEmail.Text.Trim()))
                        {
                            MessageDialog md = new MessageDialog("Please enter Email");
                            await md.ShowAsync();
                            return;
                        }
                        else if (!txtGuestEmail.Text.Contains('@') || !txtGuestEmail.Text.Contains('.'))
                        {
                            MessageDialog md = new MessageDialog("Please Enter a Valid Email");
                            await md.ShowAsync();
                            return;
                        }
                        else
                        {
                            btnCheckHost_Click(sender, e);
                            if (HostFound == true)
                            {
                                getGuestDetailfromInput();
                                newVisitor();
                            }
                        }
                    }
                }

                else
                {
                    MessageDialog md = new MessageDialog("Please input signature");
                    bool result = false;
                    md.Commands.Add(new UICommand("OK", new UICommandInvokedHandler((cmd) => result = true)));
                    await md.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msg = new MessageDialog(ex.Message + " Void - CheckInButton");
                //msg.ShowAsync();
            }
        }

        private async void newGuest()
        {
            //Send to DB
            try
            {
                VisitorDataPayLoad visitor = new VisitorDataPayLoad();
                visitor.CompanyName = GuestCompany;
                visitor.EmailAddress = Guestemail;
                visitor.FullName = GuestName;   //I think the guest name is the same as username. I suggest we add another field in Register to get 
                visitor.GuestName = GuestName;  //a particular name to link to change name since name can be changed.
                visitor.HostName = HostName;
                visitor.InvitationCode = ivCode;
                visitor.PhoneNumber = Convert.ToInt64(GuestPhoneNumber);
                visitor.Photo = GuestPhotoString;   //Reason - what if user change name or picture later? This to identify what he/she looked like during that visit. 
                visitor.Signature = base64String;
                visitor.CompanyId = _activePage.CompanyId;
                if (LstGuestCollegue.Items.Count > 0)
                {
                    ConvertGuestColleagueToJSon();
                    visitor.ExtraGuest = GuestStatus.StillCheckedIn.ToString() + "\n" + JSONGuestColleague;
                }
                //visitor
                visitor.ThumbPrint = "@";
                visitor.Description = txtComment.Text + ". Added by " + _activePage.UserStaffId;
                RemoteService service = new RemoteService();
                ResponseMessage msg = new ResponseMessage();
                if (string.IsNullOrEmpty(ivCode))
                {
                    msg = await service.CheckInRegisteredUser(visitor);
                }
                else
                {
                    msg = await service.CheckInWithInvitation(visitor);
                }

                if (msg.ResponseCode != 0)
                {
                    MessageDialog md = new MessageDialog("Could not complete Check In: " + msg.Message);
                    await md.ShowAsync();
                    return;
                }
                MessageDialog tag = new MessageDialog("Please pick your tag : Your Check In Code is " + msg.Message + ". Please click button to choose next action after copying" + "\n" + "Choose Next Step");
                tag.Commands.Add(new UICommand("Check-In Another Guest") { Id = 0 });
                tag.Commands.Add(new UICommand("Cancel") { Id = 1 });
                await tag.ShowAsync();
                //this.Frame.Navigate(typeof(CheckInGuest));
                Reset();
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msg = new MessageDialog(ex.Message + " Void - newGuest()");
                //msg.ShowAsync();
            }
        }

        private async void newVisitor()
        {
            try
            {
                //Register new Visitor
                VisitorDataPayLoad visitor = new VisitorDataPayLoad();
                visitor.CompanyName = GuestCompany;
                visitor.EmailAddress = Guestemail;
                visitor.FullName = GuestName;
                visitor.PhoneNumber = Convert.ToInt64(GuestPhoneNumber);
                visitor.Photo = GuestPhotoString;
                visitor.Comment = txtComment.Text.Trim();
                visitor.Signature = "@";
                visitor.ThumbPrint = "@";
                visitor.Description = "Added by " + _activePage.UserStaffName + "|" + _activePage.UserStaffId;
                visitor.CompanyId = _activePage.CompanyId;
                //RemoteService service = new RemoteService();
                ResponseMessage msg = await service.RegisterNewVisitor(visitor);
                if (msg.ResponseStatusCode == System.Net.HttpStatusCode.NotFound || msg.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                {
                    RegisterColleagueToDB();
                    newGuest();
                }
                else
                {
                    MessageDialog md = new MessageDialog("Could not complete registration: " + msg.Message);
                    await md.ShowAsync();
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

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), _activePage);
        }

        private async void getGuestDetail(string GuestPhoneNo)
        {
            //This helps get user detail while data is filled out.
            try
            {
                GuestFound = false;
                if (GuestPhoneNo.Trim()!=null || GuestPhoneNo.Trim() != "")
                {
                    VisitorDataPayLoad payload = new VisitorDataPayLoad
                    {
                        PhoneNumber = Convert.ToInt64(GuestPhoneNo)
                    };

                    var response = await service.GetDetailOnUserService(payload);

                    if (response.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                    {
                        //When we find user
                        GuestName = response.GuestName;
                        GuestCompany = response.CompanyName;
                        Guestemail = response.GuestEmail;
                        GuestPhoneNumber = response.GuestPhoneNumber;
                        GuestPhotoString = response.GuestPhotstring;
                        GuestFound = true;

                    }

                    if (GuestFound == true)
                    {
                        showGuestDetail();
                    }
                    else
                    {
                        MessageDialog msg = new MessageDialog("No detail found For Guest." + "\n" + "Fill required fields");
                        await msg.ShowAsync();
                        stackGuestDetail.Visibility = Visibility.Visible;
                    }
                    halfOne = true;
                    showCheckInbtn();
                }
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msg = new MessageDialog(ex.Message + " Void - getUserDetail");
                //msg.ShowAsync();
            }
        }

        private async void getHostDetail(string PhoneNo)
        {
            //This helps get user detail while data is filled out.
            try
            {
                HostFound = false;
                if (PhoneNo.Trim() != null || PhoneNo.Trim() != "")
                {
                    HostPhoneNumber = PhoneNo;
                    var response = GetDataFromDB.GetDataThisStaffFromPhoNo(_activePage.CompanyId, HostPhoneNumber).Result;

                    if (response.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                    {
                        //When we find user

                        List<StaffGlobal> thisStaff = new List<StaffGlobal>();
                        thisStaff = response.StaffList;

                        foreach (var item in thisStaff)
                        {
                            HostName = item.StaffName;
                            HostPhoneNumber = item.StaffPhoneNumber;
                            HostDept = item.DepartmentName;
                            HostDeptPhoneNo = item.DepartmentPhoneNo;
                            HostPhotoString = item.StaffPhoto;
                        }

                        HostFound = true;
                        halfTwo = true;
                        showCheckInbtn();
                    }

                }
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msg = new MessageDialog(ex.Message + " Void - getHostDetail");
                //msg.ShowAsync();
            }
        }

        private async void getIvDetail(string IvtCode)
        {
            //This helps get user detail while data is filled out.
            try
            {
                GuestFound = false;
                HostFound = false;
                ivCode = "";
                if (IvtCode.Trim() != null || IvtCode.Trim() != "")
                {
                    var response = GetDataFromDB.GetDataThisAppointmentFromIv(_activePage.CompanyId, IvtCode).Result;

                    if (response.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                    {
                        //When we find user
                        ivCode = IvtCode;
                        List<AppointmentGlobal> thisAppoint = new List<AppointmentGlobal>();
                        thisAppoint = response.AppointmentList;

                        foreach (var item in thisAppoint)
                        {
                            HostId = item.HostStaffId;
                            GuestName = item.GuestName;
                            GuestPhoneNumber = item.GuestPhoneNumber;
                            GuestCompany = item.GuestCompanyName;
                            GuestCompany = item.GuestCompanyName;
                            meetingEndTime = item.MeetingEndDateTime.ToString();
                            meetingStartTime = item.MeetingStartDateTime.ToString();

                            List<SecureLocationGlobal> thisLocation = new List<SecureLocationGlobal>();
                            var responseLoction = GetDataFromDB.GetDataThisLocation(item.LocationId).Result;
                            thisLocation = responseLoction.SecureLocationList;
                            foreach (var itemL in thisLocation)
                            {
                                meetingLocationName = itemL.LocationName;
                                meetingLocationFloor = itemL.FloorNumber.ToString();
                            }

                            VisitorDataPayLoad visitor = new VisitorDataPayLoad
                            {
                                PhoneNumber = Convert.ToInt64(item.GuestPhoneNumber)
                            };
                            var thisVisitor = service.VisitorListControllerService(visitor);
                            var visitorDetail = thisVisitor.VisitorList;
                            foreach (var itemDetail in visitorDetail)
                            {
                                if (itemDetail.photoString != null)
                                {
                                    GuestPhotoString = itemDetail.photoString;
                                }
                                if (itemDetail.emailAddress != null)
                                {
                                    Guestemail = itemDetail.emailAddress;
                                    txtGuestEmail.Text = Guestemail;
                                }
                            }

                        }

                        GuestFound = true;

                        var responseStaff = GetDataFromDB.GetDataThisStaff(HostId).Result;

                        if (responseStaff.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                        {
                            List<StaffGlobal> thisStaff = new List<StaffGlobal>();
                            thisStaff = responseStaff.StaffList;

                            foreach (var item in thisStaff)
                            {
                                HostName = item.StaffName;
                                HostPhoneNumber = item.StaffPhoneNumber;
                                HostDept = item.DepartmentName;
                                HostPhotoString = item.StaffPhoto;
                                var responseDept = GetDataFromDB.GetDataThisDepartment(item.DepartmentId).Result;

                                List<DepartmentGlobal> thisDept = new List<DepartmentGlobal>();
                                thisDept = responseDept.DepartmentList;
                                foreach (var itemi in thisDept)
                                {
                                    HostDeptPhoneNo = itemi.DepartmentPhoneNumber;
                                }
                            }

                        }

                        HostFound = true;
                        halfOne = true;
                        halfTwo = true;
                        showCheckInbtn();
                        transferFromIv();
                    }
                }
                else
                {
                    MessageDialog msg = new MessageDialog("Please Enter Invitation Code");
                    msg.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msg = new MessageDialog(ex.Message + " Void - getHostDetail");
                //msg.ShowAsync();
            }
        }

        private void txtPhoneNumber_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                //For check sytax entered in PhoneNumber field
                CheckSyntax.checkOnlyNumber(sender, e);
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - txtPhoneNumber_KeyUp");
                //msg.ShowAsync();
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton radio = sender as RadioButton;
                txtHostPhoneNumber.Text = "";
                if (radio.Tag != null)
                {
                    if (radio.Tag.ToString() == "yes")
                    {
                        stackPanelIV.Visibility = Visibility.Visible;
                        stackNoIV.Visibility = Visibility.Collapsed;
                        stackGuestDetail.Visibility = Visibility.Collapsed;                        
                    }
                    else
                    {
                        stackPanelIV.Visibility = Visibility.Collapsed;
                        stackNoIV.Visibility = Visibility.Visible;
                    }
                    stackDisplayGuestDetail.Visibility = Visibility.Collapsed;
                    stackDisplayHostDetail.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - RadioButton_Checked");
                //msg.ShowAsync();
            }
        }

        private async void btnCheckGuest_Click(object sender, RoutedEventArgs e)
        {
            //Check if Guest is registered
            getGuestDetail(txtGuestPhoneNumber.Text);
            
        }

        private void btnCheckHost_Click(object sender, RoutedEventArgs e)
        {
            //Check if Host is registered
            getHostDetail(txtHostPhoneNumber.Text);
            if (HostFound == true)
            {
                showHostDetail();
            }
            else
            {
                MessageDialog msg = new MessageDialog("No detail found for Host.");
                msg.ShowAsync();
            }
        }

        private void btnCheckIV_Click(object sender, RoutedEventArgs e)
        {
            //Check if IV exist
            getIvDetail(txtInvitationCode.Text);
            if (GuestFound == true && HostFound==true)
            {
                showGuestDetail();
                showHostDetail();
                showAppointmentDetail();
            }
            else
            {
                MessageDialog msg = new MessageDialog("No detail found For Guest." + "\n" + "Fill required fields");
                msg.ShowAsync();
                stackPanelIV.Visibility = Visibility.Collapsed;
                stackNoIV.Visibility = Visibility.Visible;
                radioNoIv.IsChecked = true;
                //RadioButton_Checked(sender, e);
            }
        }

        private async void showGuestDetail()
        {
            if (GuestName!= null)
            {
                txbGuestName.Text = GuestName;
            }

            if (GuestCompany != null)
            {
                txbGuestCompany.Text = GuestCompany;
            }

            if (Guestemail != null)
            {
                txbGuestEmail.Text = Guestemail;

            }

            if (GuestPhoneNumber != null)
            {
                txbGuestPhoneNo.Text = GuestPhoneNumber;

            }

            if (GuestPhotoString != null)
            {
                imgGuestPhoto.Source = await VisitorAppHelper.Base64StringToBitmap(GuestPhotoString);

            }

            stackDisplayGuestDetail.Visibility = Visibility.Visible;
        }

        private async void showHostDetail()
        {
            if (HostName!=null)
            {
                txbHostName.Text = HostName;

            }

            if (HostPhoneNumber != null)
            {
                txbHostPhoneNumber.Text = HostPhoneNumber;

            }
            if (HostDept != null)
            {
                txbHostDept.Text = HostDept;

            }
            if (HostDeptPhoneNo != null)
            {
                txbHostDeptPhoNo.Text = HostDeptPhoneNo;

            }
            if (HostPhotoString != null)
            {
                imgHostPhoto.Source = await VisitorAppHelper.Base64StringToBitmap(HostPhotoString);

            }
            stackDisplayHostDetail.Visibility = Visibility.Visible;
        }

        private async void showAppointmentDetail()
        {
            txbMeetingEndTime.Text = meetingEndTime;
            txbMeetingStartTime.Text = meetingStartTime;
            txbMeetingLocation.Text = meetingLocationName;
            txbMeetingLocationFloor.Text = meetingLocationFloor;

            GridAppointmentDetails.Visibility = Visibility.Visible;
        }

        private void clearAll()
        {
            clearGuestDetails();
            clearHostDetails();
            clearFilledIn();
            clearAppointmentDetails();

            clearResetAddGuestColleague();
            clearResetEditGuestColleague();
            LstGuestCollegue.ItemsSource = null;
            GuestColleagues = new List<GuestColleagueDetails>();
            txbColleagueCount.Text = "";
        }

        private void clearHostDetails()
        {
            HostDept = "";
            HostDeptPhoneNo = "";
            HostName = "";
            HostPhotoString = "";
            HostPhoneNumber = "";
            HostFound = false;

            txbHostName.Text = "";
            txbHostPhoneNumber.Text = "";
            txbHostDept.Text = "";
            txbHostDeptPhoNo.Text = "";
            imgHostPhoto.Source = null;

            stackDisplayHostDetail.Visibility = Visibility.Collapsed;
        }

        private void clearGuestDetails()
        {
            Guestemail = "";
            GuestName = "";
            GuestPhotoString = "";
            GuestPhoneNumber = "";
            GuestCompany = "";
            GuestFound = false;

            txbGuestName.Text = "";
            txbGuestEmail.Text = "";
            txbGuestCompany.Text = "";
            txbGuestPhoneNo.Text = "";
            imgGuestPhoto.Source = null;

            stackDisplayGuestDetail.Visibility = Visibility.Collapsed;
        }

        private void clearFilledIn()
        {
            txtGuestCompanyName.Text = "";
            txtGuestEmail.Text = "";
            txtGuestFullName.Text = "";
            txtGuestPhoneNumber.Text = "";
            txtComment.Text = "";
            txtHostPhoneNumber.Text = "";
            txtInvitationCode.Text = "";
            InkCanvas.Children.Clear();
            InkCanvas.Children.Remove(InkCanvas);
            //InkCanvas.StrokeContainer.Clear();
        }

        private void clearAppointmentDetails()
        {
            meetingEndTime = "";
            meetingStartTime = "";
            meetingLocationName = "";
            meetingLocationFloor = "";

            txbMeetingEndTime.Text = ""; 
            txbMeetingStartTime.Text = ""; 
            txbMeetingLocation.Text = ""; 
            txbMeetingLocationFloor.Text = "";

            GridAppointmentDetails.Visibility = Visibility.Collapsed;
        }

        private void Reset()
        {
            clearAll();
            closeEditAddColleague();
            radioNoIv.IsChecked = true;
            stackGuestDetail.Visibility = Visibility.Collapsed;
            stackNoIV.Visibility = Visibility.Visible;
            stackPanelIV.Visibility = Visibility.Collapsed;
        }

        private async void btnGuestPhoto_Click(object sender, RoutedEventArgs e)
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
                GuestPhotoString = Convert.ToBase64String(bytes);
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
                GuestPhotoString = await VisitorAppHelper.ConvertImageToBase64();

            }


            //Set Image
            imgGuestPhoto.Source =null;
            imgGuestPhoto.Source =img;

            // Convert the byte array to Base 64 string
            txbGuestPicture.Text = "Captured";

            getGuestDetailfromInput();
        }

        private async void getGuestDetailfromInput()
        {
            GuestName = txtGuestFullName.Text.Trim();
            Guestemail = txtGuestEmail.Text.Trim();
            GuestCompany = txtGuestCompanyName.Text.Trim();
            GuestPhoneNumber = txtGuestPhoneNumber.Text.Trim();

            showGuestDetail();
        }

        private void showCheckInbtn()
        {
            if (halfOne == true && halfTwo == true)
            {
                btnCheckInButton.Visibility = Visibility.Visible;
            }
        }

        private async void transferFromIv()
        {
            try
            {
                txtGuestFullName.Text = GuestName;
                txtGuestCompanyName.Text = GuestCompany;
                txtGuestPhoneNumber.Text = GuestPhoneNumber;
                stackGuestDetail.Visibility = Visibility.Visible;

                VisitorDataPayLoad payload = new VisitorDataPayLoad
                {
                    PhoneNumber = Convert.ToInt64(GuestPhoneNumber)
                };

                var response = await service.GetDetailOnUserService(payload);

                if (response.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                {
                    if (response.GuestEmail != null)
                    {
                        Guestemail = response.GuestEmail;
                        txtGuestEmail.Text = Guestemail;
                        txbGuestEmail.Text = Guestemail;
                    }
                    else if (string.IsNullOrEmpty(txtGuestEmail.Text))
                    {
                        Guestemail = "";
                        txbGuestEmail.Text = Guestemail;
                    }
                    if (response.GuestPhotstring != null)
                    {
                        GuestPhotoString = response.GuestPhotstring;
                        txbGuestPicture.Text = "Captured";
                        imgGuestPhoto.Source = await VisitorAppHelper.Base64StringToBitmap(GuestPhotoString);
                    }
                    else
                    {
                        GuestPhotoString = "";
                        imgGuestPhoto.Source = null;
                    }

                }
            }
            catch (Exception ex)
            {
                checkInternet();

                MessageDialog msgErr = new MessageDialog(ex.Message, "Error!");
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            clearAll();
        }

        private void LstGuestCollegue_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (LstGuestCollegue.Items.Count>0)
            {
                LstGuestCollegue.SelectedItem = e.ClickedItem;
                selectedColleague();
            }
        }

        private async void selectedColleague()
        {
            var selectGuestColleague = (GuestColleagueDetails)LstGuestCollegue.SelectedItem;

            txtEditColleagueEmail.Text = selectGuestColleague.GuestColleagueEmail;
            txtEditColleagueName.Text = selectGuestColleague.GuestColleagueFullName;
            txtEditColleaguePhoNo.Text = selectGuestColleague.GuestColleaguePhoneNumber;
            imgEditGuestColleague.Source = await VisitorAppHelper.Base64StringToBitmap(selectGuestColleague.GuestColleaguePhotoString);
            GuestColleaguePhotoString = selectGuestColleague.GuestColleaguePhotoString;

            GridEditRemoveGuestColleague.Visibility = Visibility.Visible;
        }

        private void closeEditAddColleague()
        {
            GridEditGuestColleague.Visibility = Visibility.Collapsed;
            GridEditRemoveGuestColleague.Visibility = Visibility.Collapsed;
            GridAddColleague.Visibility = Visibility.Collapsed;
        }

        private void btnShowAddNewGuestColleauge_Click(object sender, RoutedEventArgs e)
        {
            closeEditAddColleague();
            clearResetAddGuestColleague();
            GridAddColleague.Visibility = Visibility.Visible;
        }

        private void btnGuestColleagueDetailClose_Click(object sender, RoutedEventArgs e)
        {
            GridEditRemoveGuestColleague.Visibility = Visibility.Collapsed;
        }

        private void btnEditGuestColleagueDetails_Click(object sender, RoutedEventArgs e)
        {
            closeEditAddColleague();
            GridEditGuestColleague.Visibility = Visibility.Visible;
        }

        private void btnRemoveGuestColleague_Click(object sender, RoutedEventArgs e)
        {
            var i = LstGuestCollegue.SelectedIndex;
            GuestColleagues.RemoveAt(i);
            bindGuestColleauge();
        }

        private async void btnAddGuestColleague_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GuestColleagueName = txtColleagueName.Text.Trim();
                GuestColleagueemail = txtColleagueEmail.Text.Trim();
                GuestColleaguePhoneNumber = txtColleaguePhoNo.Text.Trim();

                if (string.IsNullOrEmpty(GuestColleagueName))
                {
                    MessageDialog msg = new MessageDialog("Enter guest colleague name", "Error!");
                    msg.ShowAsync();
                    return;
                }
                else if (string.IsNullOrEmpty(GuestColleagueemail))
                {
                    MessageDialog msg = new MessageDialog("Enter guest colleague email", "Error!");
                    msg.ShowAsync();
                    return;
                }
                else if (!GuestColleagueemail.Contains('@') || !GuestColleagueemail.Contains('.'))
                {
                    MessageDialog msg = new MessageDialog("Enter valid email for guest colleague", "Error!");
                    await msg.ShowAsync();
                    return;
                }
                else if (imgGuestColleague.Source == null)
                {
                    MessageDialog msg = new MessageDialog("Please Select Photo for guest colleague");
                    await msg.ShowAsync();
                    return;
                }
                else if (string.IsNullOrEmpty(GuestColleaguePhoneNumber))
                {
                    MessageDialog msg = new MessageDialog("Enter guest colleague phone number", "Error!");
                    msg.ShowAsync();
                    return;
                }
                else
                {
                    if (GuestColleagues != null)
                    {
                        foreach (var item in GuestColleagues)
                        {
                            if (GuestColleaguePhoneNumber == item.GuestColleaguePhoneNumber)
                            {
                                MessageDialog msg = new MessageDialog("Colleague with Phone Number already exist", "Error!");
                                msg.ShowAsync();
                                return;
                            }
                            else if (GuestColleaguePhoneNumber == txtGuestPhoneNumber.Text.Trim())
                            {
                                MessageDialog msg = new MessageDialog("This Phone Number is filled in the check in main guest phone number section", "Error!");
                                msg.ShowAsync();
                                return;
                            }
                            else if (GuestColleagueName == item.GuestColleagueFullName)
                            {
                                MessageDialog msg = new MessageDialog("Colleague with Name already exist", "Error!");
                                msg.ShowAsync();
                                return;
                            }
                            else if (GuestColleagueemail == item.GuestColleagueEmail)
                            {
                                MessageDialog msg = new MessageDialog("Colleague with Email already exist", "Error!");
                                msg.ShowAsync();
                                return;
                            }
                        }
                    }

                    GuestColleagues.Add(new GuestColleagueDetails
                    {
                        GuestColleagueFullName = GuestColleagueName,
                        GuestColleagueEmail = GuestColleagueemail,
                        GuestColleaguePhoneNumber = GuestColleaguePhoneNumber,
                        GuestColleaguePhotoString = GuestColleaguePhotoString
                    });

                    bindGuestColleauge();

                    clearResetAddGuestColleague();
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message, "btnAddGuestColleague_Click");
                //msg.ShowAsync();
            }
        }

        private void bindGuestColleauge()
        {
            try
            {
                LstGuestCollegue.ItemsSource = null;
                LstGuestCollegue.ItemsSource = GuestColleagues;
                if (LstGuestCollegue.ItemsSource != null)
                {
                    txbColleagueCount.Text = LstGuestCollegue.Items.Count.ToString() + " Colleague(s) Added";
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message, "bindGuestColleauge");
                //msg.ShowAsync();
            }
        }

        private void btnCancelAddGuestColleague_Click(object sender, RoutedEventArgs e)
        {
            GridAddColleague.Visibility = Visibility.Collapsed;
        }

        private void btnAddGuestColleagueClose_Click(object sender, RoutedEventArgs e)
        {
            GridAddColleague.Visibility = Visibility.Collapsed;
        }

        private void btnEditGuestColleagueClose_Click(object sender, RoutedEventArgs e)
        {
            GridEditGuestColleague.Visibility = Visibility.Collapsed;
        }

        private async void btnUpdateGuestColleague_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtEditColleagueName.Text.Trim()))
            {
                MessageDialog msg = new MessageDialog("Enter guest colleague name", "Error!");
                msg.ShowAsync();
                return;
            }
            else if (string.IsNullOrEmpty(txtEditColleagueEmail.Text.Trim()))
            {
                MessageDialog msg = new MessageDialog("Enter guest colleague email", "Error!");
                msg.ShowAsync();
                return;
            }
            else if (!txtEditColleagueEmail.Text.Trim().Contains('@') || !txtEditColleagueEmail.Text.Trim().Contains('.'))
            {
                MessageDialog msg = new MessageDialog("Enter valid email for guest colleague", "Error!");
                await msg.ShowAsync();
                return;
            }
            else if (imgEditGuestColleague.Source == null)
            {
                MessageDialog msg = new MessageDialog("Please Select Photo for guest colleague");
                await msg.ShowAsync();
                return;
            }
            else if (string.IsNullOrEmpty(txtEditColleaguePhoNo.Text.Trim()))
            {
                MessageDialog msg = new MessageDialog("Enter guest colleague phone number", "Error!");
                msg.ShowAsync();
                return;
            }
            else
	        {
                GuestColleagueDetails UpdateGuestColleagueDetail = new GuestColleagueDetails();
                UpdateGuestColleagueDetail.GuestColleagueEmail = txtEditColleagueEmail.Text.Trim();
                UpdateGuestColleagueDetail.GuestColleagueFullName = txtEditColleagueName.Text.Trim();
                UpdateGuestColleagueDetail.GuestColleaguePhoneNumber = txtEditColleaguePhoNo.Text.Trim();
                UpdateGuestColleagueDetail.GuestColleaguePhotoString = GuestColleaguePhotoString;

                var thisGuestColleague = GuestColleagues.FirstOrDefault(x => x.GuestColleaguePhoneNumber == txtEditColleaguePhoNo.Text.Trim());
                if (thisGuestColleague != null)
                {
                    thisGuestColleague.GuestColleagueEmail = UpdateGuestColleagueDetail.GuestColleagueEmail;
                    thisGuestColleague.GuestColleagueFullName = UpdateGuestColleagueDetail.GuestColleagueFullName;
                    thisGuestColleague.GuestColleaguePhoneNumber = UpdateGuestColleagueDetail.GuestColleaguePhoneNumber;
                    thisGuestColleague.GuestColleaguePhotoString = UpdateGuestColleagueDetail.GuestColleaguePhotoString;

                    bindGuestColleauge();
                    clearResetEditGuestColleague();
                    GridEditGuestColleague.Visibility = Visibility.Collapsed;
                } 
            }
        }

        private void btnCancelEditGuestColleague_Click(object sender, RoutedEventArgs e)
        {
            GridEditGuestColleague.Visibility = Visibility.Collapsed;
        }

        private async void btnCheckDetail_Click(object sender, RoutedEventArgs e)
        {
            //Get Colleague detail
            try
            {
                if (txtColleaguePhoNo.Text.Trim() != null)
                {
                    var phoNo = Convert.ToInt64(txtColleaguePhoNo.Text.Trim());
                    var resp = GetDataFromDB.GetDataThisVisitorDetail(phoNo).Result;

                    if (resp.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                    {
                        var visitor = resp.VisitorList;
                        if (visitor != null)
                        {
                            foreach (var item in visitor)
                            {
                                txtColleagueEmail.Text = item.emailAddress;
                                txtColleagueName.Text = item.VisitorFullName;
                                GuestColleaguePhotoString = item.photoString;
                            }
                            imgGuestColleague.Source = await VisitorAppHelper.Base64StringToBitmap(GuestColleaguePhotoString);
                        }
                    }
                    else
                    {
                        MessageDialog msg = new MessageDialog("Please Fill in Guest Colleauge details", "Hi. Details not found");
                        await msg.ShowAsync();
                    }
                }
                else
                {
                    MessageDialog msg = new MessageDialog("Please Fill in Guest Colleauge details", "Hi. Details not found");
                    await msg.ShowAsync();
                }
                stackGuestColleagueDetails.Visibility = Visibility.Visible;
                stackGuestColleaguePhoto.Visibility = Visibility.Visible;

            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msgErr = new MessageDialog(ex.Message, "Err - btnCheckDetail_Click");
                //await msgErr.ShowAsync();
            }
        }

        private async void btnSelectColleauguPhoto_Click(object sender, RoutedEventArgs e)
        {
            try
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
                    GuestColleaguePhotoString = Convert.ToBase64String(bytes);
                }

                if (Convert.ToInt32(result.Id) == 1)
                {
                    //Select from Gallery
                    StorageFile GuestColleagueImage = await VisitorAppHelper.selectImage();
                    //make sure the fill is not null

                    if (GuestColleagueImage == null)
                    {
                        //if null
                        return;
                    }

                    img = await VisitorAppHelper.GetImage(GuestColleagueImage);
                    GuestColleaguePhotoString = await VisitorAppHelper.ConvertImageToBase64();

                }


                //Set Image
                imgGuestColleague.Source = null;
                imgGuestColleague.Source = img;
            }
            catch (Exception ex)
            {
                MessageDialog msgErr = new MessageDialog(ex.Message, "Err - btnSelectColleauguPhoto_Click");
                //await msgErr.ShowAsync();
            }
        }

        private void clearResetAddGuestColleague()
        {
            txtColleagueEmail.Text = "";
            txtColleagueName.Text = "";
            txtColleaguePhoNo.Text = "";
            imgGuestColleague.Source = null;
            stackGuestColleagueDetails.Visibility = Visibility.Collapsed;
            stackGuestColleaguePhoto.Visibility = Visibility.Collapsed;
        }

        private void clearResetEditGuestColleague()
        {
            txtEditColleaguePhoNo.Text = "";
            txtEditColleagueName.Text = "";
            txtEditColleagueEmail.Text = "";
            imgEditGuestColleague.Source = null;
        }

        private async void btnEditSelectColleauguPhoto_Click(object sender, RoutedEventArgs e)
        {
            try
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
                    GuestColleaguePhotoString = Convert.ToBase64String(bytes);
                }

                if (Convert.ToInt32(result.Id) == 1)
                {
                    //Select from Gallery
                    StorageFile GuestColleagueImage = await VisitorAppHelper.selectImage();
                    //make sure the fill is not null

                    if (GuestColleagueImage == null)
                    {
                        //if null
                        return;
                    }

                    img = await VisitorAppHelper.GetImage(GuestColleagueImage);
                    GuestColleaguePhotoString = await VisitorAppHelper.ConvertImageToBase64();

                }


                //Set Image
                imgEditGuestColleague.Source = null;
                imgEditGuestColleague.Source = img;
            }
            catch (Exception ex)
            {
                MessageDialog msgErr = new MessageDialog(ex.Message, "Err - btnEditSelectColleauguPhoto_Click");
                //await msgErr.ShowAsync();
            }
        }

        private async void RegisterColleagueToDB()
        {
            try
            {
                if (LstGuestCollegue.Items.Count > 0)
                {
                    for (int i = 0; i < LstGuestCollegue.Items.Count; i++)
                    {
                        LstGuestCollegue.SelectedIndex = i;
                        var thisVisitor = (GuestColleagueDetails)LstGuestCollegue.SelectedItem;

                        VisitorDataPayLoad visitor = new VisitorDataPayLoad();
                        visitor.CompanyName = GuestCompany;
                        visitor.EmailAddress = thisVisitor.GuestColleagueEmail;
                        visitor.FullName = thisVisitor.GuestColleagueFullName;
                        visitor.PhoneNumber = Convert.ToInt64(thisVisitor.GuestColleaguePhoneNumber);
                        visitor.Photo = thisVisitor.GuestColleaguePhotoString;
                        visitor.Signature = "@";
                        visitor.ThumbPrint = "@";
                        visitor.Description = "Added by " + _activePage.UserStaffName + "|" + _activePage.UserStaffId;
                        visitor.CompanyId = _activePage.CompanyId;
                        //RemoteService service = new RemoteService();
                        ResponseMessage msg = await service.RegisterNewVisitor(visitor);
                        if (msg.ResponseStatusCode != System.Net.HttpStatusCode.NotFound || msg.ResponseStatusCode != System.Net.HttpStatusCode.Found)
                        {
                            MessageDialog md = new MessageDialog("Could not complete registration: " + msg.Message);
                            //await md.ShowAsync();
                        }

                    } 
                }
            }
            catch (Exception ex)
            {
                checkInternet();
                MessageDialog msgErr = new MessageDialog(ex.Message, "RegisterColleagueToDB");
                //await msgErr.ShowAsync();
            }
        }

        private async void ConvertGuestColleagueToJSon()
        {
            try
            {
                if (LstGuestCollegue.Items.Count > 0)
                {
                    JSONGuestColleague = "";
                    string eachCoolleagueiInJSON = "";
                    for (int i = 0; i < LstGuestCollegue.Items.Count; i++)
                    {
                        LstGuestCollegue.SelectedIndex = i;
                        var thisVisitor = (GuestColleagueDetails)LstGuestCollegue.SelectedItem;

                        #region Using my Codes. NewtoJSON made life easier shaa
                        //eachCoolleagueiInJSON = await VisitorAppGuestColleague.SerializeGuestColleagueToJSON(("Guest Colleague " + (i+1).ToString()),
                        //    await VisitorAppGuestColleague.FormatToGuestColleagueJSON(thisVisitor.GuestColleagueFullName, thisVisitor.GuestColleaguePhoneNumber));

                        #endregion
                        eachCoolleagueiInJSON = JsonConvert.SerializeObject(await VisitorAppGuestColleague.FormatToGuestColleagueJSON(thisVisitor.GuestColleagueFullName, thisVisitor.GuestColleaguePhoneNumber, thisVisitor.GuestColleaguePhotoString));

                        JSONGuestColleague += eachCoolleagueiInJSON + "\n";
                    }
                    //JSONGuestColleague = await VisitorAppGuestColleague.PrintGuestColleagueToJSON(JSONGuestColleague); Using my Codes. NewtoJSON made life easier shaa
                    JSONGuestColleague = JSONGuestColleague.Trim();
                }
            }
            catch (Exception ex)
            {
                MessageDialog msgErr = new MessageDialog(ex.Message, "ConvertGuestColleagueToJSon");
                //await msgErr.ShowAsync();
            }
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            //Testing
            ConvertJSonToGuestColleague();
        }

        private async void ConvertJSonToGuestColleague()
        {
            //TEsting for output of deserialization
            List<GuestColleagueJSON> ListGuestColleagueJSON = new List<GuestColleagueJSON>();

            string JSON = txbTest.Text;
            string[] ListGuestColleagueArray = await VisitorAppGuestColleague.DeFormatToGuestColleagueJSON(JSON);

            for (int i = 1; i < ListGuestColleagueArray.Length; i++)
            {
                GuestColleagueJSON deserializedGuestColleague = JsonConvert.DeserializeObject<GuestColleagueJSON>(ListGuestColleagueArray[i]);
                ListGuestColleagueJSON.Add(deserializedGuestColleague);
            }
            var mainGuestStatus = ListGuestColleagueArray[0];
            var ob = ListGuestColleagueJSON;
            LstTest.ItemsSource = ob;
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
