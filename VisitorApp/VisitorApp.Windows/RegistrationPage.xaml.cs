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
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Storage.Pickers;

// The Hub Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=321224

namespace VisitorApp
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class RegistrationPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private string photoString = "";
        private string signatureString = "";
        private string thumbString = "";

        private IRandomAccessStream stream;
        private string fullname;
        private string emailAddress;
        private string companyName;
        private long phoneNumber;
        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        /// 


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

        public RegistrationPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
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

        private async void PhotoButton_Click(object sender, RoutedEventArgs e)
        {
            //capturePhoto();

            MessageDialog selectPicture = new MessageDialog("Select Capture Method");
            selectPicture.Commands.Add(new UICommand("Take Photo") { Id = 0 });
            selectPicture.Commands.Add(new UICommand("Select from Gallery") { Id = 1 });
            selectPicture.Commands.Add(new UICommand("Cancel") { Id = 2 });

            selectPicture.CancelCommandIndex = 2;

            var result = await selectPicture.ShowAsync();

            if (Convert.ToInt32(result.Id) == 0)
            {
                //Take Photo
                capturePhoto();
            }

            if (Convert.ToInt32(result.Id) == 1)
            {
                //Select from Gallery
                pictureFromDevice();
            }

        }

        private async void pictureFromDevice()
        {
            var newPicture = new FileOpenPicker();
            newPicture.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            newPicture.ViewMode = PickerViewMode.List;
            newPicture.FileTypeFilter.Add(".jpeg");
            newPicture.FileTypeFilter.Add(".png");
            newPicture.FileTypeFilter.Add(".jpg");
            newPicture.FileTypeFilter.Add(".bmp");

            StorageFile photo = await newPicture.PickSingleFileAsync();

            //make sure the fill is not null

            if (photo == null)
            {
                //if null
                return;
            }

            //Convert Image
            stream = await photo.OpenAsync(FileAccessMode.Read);
            convertImage();
        }

        private async void capturePhoto()
        {
            //Capture Photo from Webcam
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
            stream = await photo.OpenAsync(FileAccessMode.Read);
            convertImage();
        }

        private async void convertImage()
        {
            //This is to covert image to a format b4 it's saved
            //Open stream for selected file

            // IRandomAccessStream stream = await photo.OpenAsync(FileAccessMode.Read);
            var bitmap = new BitmapImage();
            bitmap.SetSource(stream);
            PhotoControl.Source = bitmap;
            //
            // Create a byte array for the image
            Byte[] bytes = new Byte[0];
            var reader = new DataReader(stream.GetInputStreamAt(0));
            bytes = new Byte[stream.Size];
            await reader.LoadAsync((uint)stream.Size);
            reader.ReadBytes(bytes);
            // Convert the byte array to Base 64 string
            photoString = Convert.ToBase64String(bytes);
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                fullname = txtFullName.Text;
                phoneNumber = Convert.ToInt64(txtPhoneNumber.Text);
                emailAddress = txtEmail.Text;
                companyName = txtCompanyName.Text;
                if (string.IsNullOrEmpty(fullname))
                {
                    MessageDialog md = new MessageDialog("Please enter full name");
                    await md.ShowAsync();
                    return;
                }

                else if (string.IsNullOrEmpty(phoneNumber.ToString()))
                {
                    MessageDialog md = new MessageDialog("Please enter phone Number");
                    await md.ShowAsync();
                    return;
                }

                else if (string.IsNullOrEmpty(companyName))
                {
                    MessageDialog md = new MessageDialog("Please enter company Name");
                    await md.ShowAsync();
                    return;
                }
                else if (string.IsNullOrEmpty(photoString))
                {
                    MessageDialog md = new MessageDialog("Please Select Photo");
                    await md.ShowAsync();
                    return;
                }

                else
                {
                    phoneNumber = Convert.ToInt64(txtPhoneNumber.Text);

                    RemoteService service = new RemoteService();
                    VisitorDataPayLoad payload = new VisitorDataPayLoad
                    {
                        PhoneNumber = phoneNumber
                    };

                    var response = await service.CheckIfUserExistService(payload);


                    if (response.ResponseStatusCode == System.Net.HttpStatusCode.Found)
                    {
                        MessageDialog checkInMsg = new MessageDialog("User already exist");
                        await checkInMsg.ShowAsync();
                        return;
                    }
                    else if (response.ResponseStatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        //Send to Registration DB
                        newVisitor();
                    }
                    else
                    {
                        MessageDialog checkInMsg = new MessageDialog("Server error");
                        await checkInMsg.ShowAsync();
                    }
                }

                //After Registration

                clearContents();

                MessageDialog afterRegistration = new MessageDialog("Choose Next Step");
                afterRegistration.Commands.Add(new UICommand("Check-In This User") { Id = 0 });
                afterRegistration.Commands.Add(new UICommand("Register Another User") { Id = 1 });
                afterRegistration.Commands.Add(new UICommand("Cancel") { Id = 2 });

                var result = await afterRegistration.ShowAsync();

                if (Convert.ToInt32(result.Id) == 0)
                {
                    //Call Check In Button
                    CheckInButton_Click(sender, e);
                }

                else
                {
                    //Do nothing
                    //This is  equivalent to Cancel since either way you are going back
                }


            }
            catch (Exception ex)
            {
                MessageDialog tag = new MessageDialog(ex.Message);
                //throw;
            }

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HubPage));
        }

        private void CheckInwithIv()
        {
            this.Frame.Navigate(typeof(CheckInWithInvitationPage));
        }

        private void CheckInWithNoIv()
        {
            this.Frame.Navigate(typeof(CheckInRegistered));
        }

        private void PhotoTapped(object sender, TappedRoutedEventArgs e)
        {
            PhotoButton_Click(sender, e);
        }

        private async void CheckInButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageDialog checkIn = new MessageDialog("Select Check-In Method");
                checkIn.Commands.Add(new UICommand("Check In With Invitation Code") { Id = 0 });
                checkIn.Commands.Add(new UICommand("Check In, No Invitation Code") { Id = 1 });
                checkIn.Commands.Add(new UICommand("Cancel") { Id = 2 });

                checkIn.CancelCommandIndex = 2;

                var result = await checkIn.ShowAsync();

                if (Convert.ToInt32(result.Id) == 0)
                {
                    CheckInwithIv();
                }

                if (Convert.ToInt32(result.Id) == 1)
                {
                    CheckInWithNoIv();
                }
            }
            catch (Exception ex)
            {
                MessageDialog errMsg = new MessageDialog(ex.Message);
                await errMsg.ShowAsync();
            }
        }

        //These Class below interacts with DB

        private async void newVisitor()
        {
            //Register new Visitor
            VisitorDataPayLoad visitor = new VisitorDataPayLoad();
            visitor.CompanyName = companyName;
            visitor.EmailAddress = emailAddress;
            visitor.FullName = fullname;
            visitor.GuestName = "";
            visitor.HostName = "";
            visitor.InvitationCode = "";
            visitor.PhoneNumber = Convert.ToInt64(txtPhoneNumber.Text);
            visitor.Photo = photoString;
            visitor.Signature = "@";
            visitor.ThumbPrint = "@";
            RemoteService service = new RemoteService();
            ResponseMessage msg = await service.RegisterNewUser(visitor);
            if (msg.ResponseCode != 0)
            {
                MessageDialog md = new MessageDialog("Could not complete registration: " + msg.Message);
                await md.ShowAsync();
                return;
            }
            //photoString = "";
            //this.Frame.Navigate(typeof(HubPage));
            //MessageDialog tag = new MessageDialog("Please pick your tag");
            //await tag.ShowAsync();
            //this.Frame.Navigate(typeof(HubPage));

            MessageDialog regMsg = new MessageDialog("Registration Successful");
            await regMsg.ShowAsync();
        }

        private void clearContents()
        {
            //Clear Contents of Fields.

            txtCompanyName.Text = "";
            txtEmail.Text = "";
            txtFullName.Text = "";
            txtPhoneNumber.Text = "";
            photoString = signatureString = thumbString = "";
            fullname  = emailAddress = companyName = "";
            PhotoControl.Source = null;
        }
    }
}
