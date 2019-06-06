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
using VisitorApp.Models;


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

        AccountDetails _activePage = new AccountDetails();

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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _activePage = (AccountDetails)e.Parameter;
            if (_activePage != null)
            {
                txbTitle.Text = ( _activePage.CompanyName).ToUpper();
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

        private async void PhotoButton_Click(object sender, RoutedEventArgs e)
        {
            //capturePhoto();

            try
            {
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
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - PhotoButton_Click");
                //msg.ShowAsync();
            }

        }

        private async void pictureFromDevice()
        {
            try
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
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - pictureFromDevice()");
               // msg.ShowAsync();
            }
        }

        private async void capturePhoto()
        {
            try
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
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - capturePhoto()");
                //msg.ShowAsync();
            }
        }

        private async void convertImage()
        {
            try
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
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - convertImage");
                //msg.ShowAsync();
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtFullName.Text.Trim() == "")
                {
                    MessageDialog md = new MessageDialog("Please enter full name");
                    await md.ShowAsync();
                    return;
                }

                else if (txtPhoneNumber.Text.Trim() == "")
                {
                    MessageDialog md = new MessageDialog("Please enter phone Number");
                    await md.ShowAsync();
                    return;
                }

                else if (txtCompanyName.Text.Trim() == "")
                {
                    MessageDialog md = new MessageDialog("Please enter company Name");
                    await md.ShowAsync();
                    return;
                }
                else if (photoString == "")
                {
                    MessageDialog md = new MessageDialog("Please Select Photo");
                    await md.ShowAsync();
                    return;
                }
                else if (txtEmail.Text.Trim() == "")
                {
                    MessageDialog md = new MessageDialog("Please enter Email");
                    await md.ShowAsync();
                    return;
                }
                else if (!txtEmail.Text.Contains('@') || !txtEmail.Text.Contains('.'))
                {
                    MessageDialog md = new MessageDialog("Please Enter a Valid Email");
                    await md.ShowAsync();
                    return;
                }
                else
                {
                    fullname = txtFullName.Text;
                    emailAddress = txtEmail.Text;
                    companyName = txtCompanyName.Text;
                    phoneNumber = Convert.ToInt64(txtPhoneNumber.Text);

                    RemoteService service = new RemoteService();
                    VisitorDataPayLoad payload = new VisitorDataPayLoad
                    {
                        PhoneNumber = phoneNumber
                    };

                    var response = await service.CheckIfVisitorExistService(payload);


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
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - RegisterButton_Click");
                //msg.ShowAsync();
            }

        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), _activePage);
        }

        private void CheckInwithIv()
        {
            this.Frame.Navigate(typeof(CheckInWithInvitationPage));
        }

        private void PhotoTapped(object sender, TappedRoutedEventArgs e)
        {
            PhotoButton_Click(sender, e);
        }

        //These Class below interacts with DB

        private async void newVisitor()
        {
            try
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
                ResponseMessage msg = await service.RegisterNewVisitor(visitor);
                if (msg.ResponseCode != 0)
                {
                    MessageDialog md = new MessageDialog("Could not complete registration: " + msg.Message);
                    await md.ShowAsync();
                    return;
                }

                MessageDialog regMsg = new MessageDialog("Registration Successful");
                await regMsg.ShowAsync();

                MessageDialog afterRegistration = new MessageDialog("Choose Next Step");
                afterRegistration.Commands.Add(new UICommand("Check-In This User") { Id = 0 });
                afterRegistration.Commands.Add(new UICommand("Register Another User") { Id = 1 });
                afterRegistration.Commands.Add(new UICommand("Cancel") { Id = 2 });

                var result = await afterRegistration.ShowAsync();

                if (Convert.ToInt32(result.Id) == 0)
                {
                    //Call Check In Button
                    this.Frame.Navigate(typeof(CheckInWithInvitationPage));
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - newVisitor");
                //msg.ShowAsync();
            }
        }

        private void clearContents()
        {
            //Clear Contents of Fields.

            try
            {
                txtCompanyName.Text = "";
                txtEmail.Text = "";
                txtFullName.Text = "";
                txtPhoneNumber.Text = "";
                photoString = signatureString = thumbString = "";
                fullname = emailAddress = companyName = "";
                PhotoControl.Source = null;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - clearContents");
                //msg.ShowAsync();
            }
        }

        private void txtFullName_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                //For check sytax entered in NAme field
                CheckSyntax.checkOnlyName(sender, e);

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - txtFullNAme_KeyUp");
                //msg.ShowAsync();
            }
        }

        private void txtPhoneNumber_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                //For check sytax entered in Name field
                CheckSyntax.checkOnlyNumber(sender, e);
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - txtPhoneNumber_KeyUp");
                //msg.ShowAsync();
            }
        }
    }
}
