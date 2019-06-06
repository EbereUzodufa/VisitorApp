using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VisitorApp.Common;
using VisitorApp.Dashboard.Admin;
using VisitorApp.Dashboard.Staff;
using VisitorApp.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
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

namespace VisitorApp.Dashboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterGuest : Page
    {
        //private NavigationHelper navigationHelper;
        //private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private string photoString = "";
        private string signatureString = "";
        private string thumbString = "";

        private IRandomAccessStream stream;
        private string fullname;
        private string emailAddress;
        private string companyName;
        private long phoneNumber;

        AccountDetails _activePage = new AccountDetails();
        private int CompId;

        public RegisterGuest()
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
            stackRegister.Visibility = Visibility.Collapsed;
            stackCheckIn.Visibility = Visibility.Collapsed;
            stackCheckOut.Visibility = Visibility.Collapsed;
            stackGuestList.Visibility = Visibility.Collapsed;

            if (_activePage.UserStaffRole.ToUpper() == "ADMIN")
            {
                stackDashboard.Visibility = Visibility.Visible;
                stackDepartment.Visibility = Visibility.Visible;
                stackStaff.Visibility = Visibility.Visible;
                stackLocation.Visibility = Visibility.Visible;
                stackAppointment.Visibility = Visibility.Visible;
                stackUpload.Visibility = Visibility.Visible;
                stackRegister.Visibility = Visibility.Visible;
                stackCheckIn.Visibility = Visibility.Visible;
                stackCheckOut.Visibility = Visibility.Visible;
                stackGuestList.Visibility = Visibility.Visible;
            }
            else if (_activePage.UserStaffRole.ToUpper() == "FRONTDESK")
            {
                stackDashboard.Visibility = Visibility.Visible;
                stackAppointment.Visibility = Visibility.Visible;
                stackUpload.Visibility = Visibility.Visible;
                stackRegister.Visibility = Visibility.Visible;
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
            //The same Page
            //this.Frame.Navigate(typeof(RegisterGuest), _activePage);
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
            this.Frame.Navigate(typeof(Location), _activePage);
        }
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
                //msg.ShowAsync();
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
                if (txtFullName.Text.Trim()=="")
                {
                    MessageDialog md = new MessageDialog("Please enter full name");
                    await md.ShowAsync();
                    return;
                }

                else if (txtPhoneNumber.Text.Trim()=="")
                {
                    MessageDialog md = new MessageDialog("Please enter phone Number");
                    await md.ShowAsync();
                    return;
                }

                else if (txtCompanyName.Text.Trim()=="")
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
                _activePage.LogOutGuestphoneNumber = phoneNumber.ToString();
                MessageDialog afterRegistration = new MessageDialog("Choose Next Step");
                afterRegistration.Commands.Add(new UICommand("Check-In This User") { Id = 0 });
                afterRegistration.Commands.Add(new UICommand("Register Another User") { Id = 1 });
                afterRegistration.Commands.Add(new UICommand("Cancel") { Id = 2 });

                var result = await afterRegistration.ShowAsync();

                if (Convert.ToInt32(result.Id) == 0)
                {
                    this.Frame.Navigate(typeof(CheckInGuest),_activePage);
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
