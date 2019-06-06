using VisitorApp.Common;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Input.Inking;
using Windows.Devices.Input;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input;

using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;
using VisitorApp.Models;
using VisitorApp.Common;


// The Hub Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=321224

namespace VisitorApp
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class CheckInWithInvitationPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        //Delcare Important variables
        private string userName;
        private string userCompany;
        private string userEmail;
        private string userPhoneNumber;
        private string userPhotoString;
        private string userRegistrationStatus;  //Tell us if user is registered

        //Signature and IV
        private string ivCode;
        private string base64String;

        AccountDetails _activePage = new AccountDetails(); 

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }



        // Scenario specific constants and variables.
        const double STROKETHICKNESS = 5;
        Point _previousContactPt;
        uint _penID = 0;
        uint _touchID = 0;

        // Create the InkManager instance.
        InkManager _inkManager = new Windows.UI.Input.Inking.InkManager();

        public CheckInWithInvitationPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            InkCanvas.PointerPressed += new PointerEventHandler(InkCanvas_PointerPressed);
            InkCanvas.PointerMoved += new PointerEventHandler(InkCanvas_PointerMoved);
            InkCanvas.PointerReleased += new PointerEventHandler(InkCanvas_PointerReleased);
            InkCanvas.PointerExited += new PointerEventHandler(InkCanvas_PointerReleased);
        }

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
               // msg.ShowAsync();
            }
        }

        private double Distance(Point currentContact, Point previousContact)
        {
            return Math.Sqrt(Math.Pow(currentContact.X - previousContact.X, 2) +
                    Math.Pow(currentContact.Y - previousContact.Y, 2));
        }


        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Assign a collection of bindable groups to this.DefaultViewModel["Groups"]
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

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        //protected override void OnNavigatedFrom(NavigationEventArgs e)
        //{
        //    navigationHelper.OnNavigatedFrom(e);
        //}

        #endregion

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

                    var phoneNumber = txtPhoneNumber.Text;
                    var ivCode = txtInvitationCode.Text;
                    //
                    if (string.IsNullOrEmpty(phoneNumber))
                    {
                        MessageDialog md = new MessageDialog("Please enter phone Number");
                        await md.ShowAsync();
                        return;
                    }

                    if ((string.IsNullOrEmpty(ivCode)) && (stackPanelIV.Visibility==Visibility.Visible))
                    {
                        MessageDialog md = new MessageDialog("Please enter your invitation code");
                        await md.ShowAsync();
                        return;
                    }

                    else
                    {
                        //Check user

                        if (userRegistrationStatus == "Registered")
                        {
                            //This is to check-IN
                            newGuest();
                        }

                        else
                        {
                            MessageDialog notRegistration = new MessageDialog("Visitor not registered");
                            notRegistration.Commands.Add(new UICommand("Register Visitor") { Id = 0 });
                            notRegistration.Commands.Add(new UICommand("Cancel") { Id = 1 });
                            var result = await notRegistration.ShowAsync();

                            if (Convert.ToInt32(result.Id) == 0)
                            {
                                //Navigate to the Registration Page
                                this.Frame.Navigate(typeof(RegistrationPage));
                            }

                            else
                            {
                                //Do nothing
                                //This is  equivalent to Cancel since either way you are going back
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
                MessageDialog msg = new MessageDialog(ex.Message + " Void - CheckInButton");
                //msg.ShowAsync();
            }
        }

        private async void newGuest()
        {
            //Send to DB
            try
            {

                var phoneNumber = txtPhoneNumber.Text;
                var hostName = txtHostName.Text;

                VisitorDataPayLoad visitor = new VisitorDataPayLoad();
                visitor.CompanyName = userCompany;
                visitor.EmailAddress = userEmail;
                visitor.FullName = userName;                            //I think the guest name is the same as username. I suggest we add another field in Register to get 
                visitor.GuestName = userName;                           //a particular name to link to change name since name can be changed.
                visitor.HostName = hostName;
                visitor.InvitationCode = ivCode;
                visitor.PhoneNumber = Convert.ToInt64(phoneNumber);
                visitor.Photo = userPhotoString;                        //Reason - what if user change name or picture later? This to identify what he/she looked like during that visit. 
                visitor.Signature = base64String;
                visitor.CompanyId = _activePage.CompanyId;
                visitor.ThumbPrint = "@";
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

                this.Frame.Navigate(typeof(MainPage));
                MessageDialog tag = new MessageDialog("Please pick your tag : Your Check In Code is " + msg.Message + ". Please click OK button after copy");
                await tag.ShowAsync();
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - newGuest()");
                //msg.ShowAsync();
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), _activePage);
        }

        private async void getUserDetail()
        {
            //This helps get user detail while data is filled out.
            try
            {
                RemoteService service = new RemoteService();
                VisitorDataPayLoad payload = new VisitorDataPayLoad
                {
                    PhoneNumber = Convert.ToInt64(txtPhoneNumber.Text)
                };

                var response = await service.GetDetailOnUserService(payload);

                if (response.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                {
                    //When we find user
                    userRegistrationStatus = "Registered";
                    userName = response.GuestName;
                    userCompany = response.CompanyName;
                    userEmail = response.GuestEmail;
                    userPhoneNumber = response.GuestPhoneNumber;
                    userPhotoString = response.GuestPhotstring;

                    txtVisitorName.Text = response.GuestName;
                    string photoString = userPhotoString;           //Repetition based on flow/arrangement

                    byte[] Bytes = Convert.FromBase64String(photoString);

                    var stream = new InMemoryRandomAccessStream();
                    //var bytes = Convert.FromBase64String(source);
                    var dataWriter = new DataWriter(stream);
                    dataWriter.WriteBytes(Bytes);
                    await dataWriter.StoreAsync();
                    stream.Seek(0);
                    var img = new BitmapImage();
                    img.SetSource(stream);
                    PhotoCopy.Source = img;
                }
                else
                {
                    userRegistrationStatus = "Not Registered";
                    PhotoCopy.Source = null;
                    txtVisitorName.Text = "";
                }

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - getUserDetail");
                //msg.ShowAsync();
            }
        }

        private void  txtPhoneNumber_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                //For check sytax entered in PhoneNumber field
                CheckSyntax.checkOnlyNumber(sender, e);

                //if (txtPhoneNumber.Text.Length > 10)
                //{
                //    getUserDetail();
                //    txtVisitorName.Visibility = Visibility.Visible;
                //}
                //else
                //{
                //    txtVisitorName.Visibility = Visibility.Collapsed;
                //}
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
                txtHostName.Text = "";
                if (radio.Tag != null)
                {
                    if (radio.Tag.ToString() == "yes")
                    {
                        stackPanelIV.Visibility = Visibility.Visible;
                        txtHostName.IsReadOnly = true;
                    }
                    else
                    {
                        stackPanelIV.Visibility = Visibility.Collapsed;
                        txtHostName.IsReadOnly = false;
                        txtInvitationCode.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - RadioButton_Checked");
                //msg.ShowAsync();
            }
        }

        private async void test()
        {
            try
            {
                base64String = "";
                if (_inkManager.GetStrokes() != null && _inkManager.GetStrokes().Count > 0)
                {
                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
                    await renderTargetBitmap.RenderAsync(InkCanvas);
                    //PhotoCopy.Source = renderTargetBitmap;
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
                        //PhotoCopy.Source = await ImageProcessor.Base64StringToBitmap(base64String);
                        MessageDialog msg = new MessageDialog(base64String);
                        await msg.ShowAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - test()");
                //msg.ShowAsync();
            }

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            test();
        }

        private void btnUU_Click(object sender, RoutedEventArgs e)
        {
            getUserDetail();
        }
    }
}
