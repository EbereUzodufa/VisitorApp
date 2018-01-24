using VisitorApp.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Input.Inking;
using Windows.Devices.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;


// The Hub Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=321224

namespace VisitorApp
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class CheckInRegistered : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

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

        public CheckInRegistered()
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

        public void InkCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
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

        public void InkCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
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

        private void RenderAllStrokes()
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

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HubPage));
        }



        private async void CheckInButton_Click(object sender, RoutedEventArgs e)
        {
            //string base64String = "";

            if (_inkManager.GetStrokes() != null && _inkManager.GetStrokes().Count > 0)
            {
            //    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            //    await renderTargetBitmap.RenderAsync(InkCanvas);
                //PhotoControl.Source = renderTargetBitmap;
            //    using (var stream = new InMemoryRandomAccessStream())
            //    {
            //        await _inkManager.SaveAsync(stream);
            //        await stream.FlushAsync();
            //        stream.Seek(0);
            //        //byte[] bytes = new byte[stream.Size];

                    //        Byte[] bytes = new Byte[0];
                    //        var reader = new DataReader(stream.GetInputStreamAt(0));
                    //        bytes = new Byte[stream.Size];
                    //        await reader.LoadAsync((uint)stream.Size);
                    //        reader.ReadBytes(bytes);
                    //        // Convert the byte array to Base 64 string
                    //        base64String = Convert.ToBase64String(bytes);
                    //        PhotoCopy.Source = await ImageProcessor.Base64StringToBitmap(base64String);                   

                    //}

                var phoneNumber = txtPhoneNumber.Text;
                var hostName = txtHostName.Text;
                //
                if (string.IsNullOrEmpty(phoneNumber))
                {
                    MessageDialog md = new MessageDialog("Please enter phone Number");
                    await md.ShowAsync();
                    return;
                }

                if (string.IsNullOrEmpty(hostName))
                {
                    MessageDialog md = new MessageDialog("Please enter your host name");
                    await md.ShowAsync();
                    return;
                }
                
                VisitorDataPayLoad visitor = new VisitorDataPayLoad();
                visitor.CompanyName = "";
                visitor.EmailAddress = "";
                visitor.FullName = "";
                visitor.GuestName = "";
                visitor.HostName = hostName;
                visitor.InvitationCode = "";
                visitor.PhoneNumber = Convert.ToInt64(phoneNumber);
                visitor.Photo = "@";
                //visitor.Signature = base64String;
                visitor.ThumbPrint = "@";
                RemoteService service = new RemoteService();
                ResponseMessage msg = await service.CheckInRegisteredUser(visitor);
                if (msg.ResponseCode != 0)
                {
                    MessageDialog md = new MessageDialog("Could not complete Check In: " + msg.Message);
                    await md.ShowAsync();
                    return;
                }
                
                this.Frame.Navigate(typeof(HubPage));
                MessageDialog tag = new MessageDialog("Please pick your tag : Your Check In Code is " + msg.Message + ". Please click OK button after copy");
                await tag.ShowAsync();


            }
            else
            {

                MessageDialog md = new MessageDialog("Please input signature");
                bool result = false;
                md.Commands.Add(new UICommand("OK", new UICommandInvokedHandler((cmd) => result = true)));
                await md.ShowAsync();
            }
        }

        private async void  button_Click(object sender, RoutedEventArgs e)
        {
            RemoteService service = new RemoteService();
            VisitorDataPayLoad payload = new VisitorDataPayLoad {
                PhoneNumber = Convert.ToInt64(txtPhoneNumber.Text)
            };

            var response = await service.GetDetailOnUserService(payload);

            if (response.ResponseStatusCode == System.Net.HttpStatusCode.Found)
            {

                txtVisitorName.Text = response.userName;
                string photoString = response.photstring;

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

        }
    }
}
