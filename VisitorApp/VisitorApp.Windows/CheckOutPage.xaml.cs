using VisitorApp.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using VisitorApp.Models;


// The Hub Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=321224

namespace VisitorApp
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class CheckOutPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

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
        //public NavigationHelper NavigationHelper
        //{
        //    get { return this.navigationHelper; }
        //}

        public CheckOutPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _activePage = (AccountDetails)e.Parameter;
            if (_activePage != null)
            {
                txbTitle.Text = ("Goodbye From " + _activePage.CompanyName).ToUpper();
                txbUserName.Text = _activePage.UserStaffName;
                if (_activePage.LogOutGuestphoneNumber!=null)
                {
                    txtPhoneNumber.Text = _activePage.LogOutGuestphoneNumber;
                    getUserDetail();
                }
            }
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

        #endregion

        private  async void CheckOutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var checkInCode = txtCheckInCode.Text;

                if (string.IsNullOrEmpty(checkInCode))
                {
                    MessageDialog md = new MessageDialog("Please enter your check in code");
                    await md.ShowAsync();
                    return;
                }

                VisitorDataPayLoad visitor = new VisitorDataPayLoad();
                visitor.CheckInCode = checkInCode;

                RemoteService service = new RemoteService();
                ResponseMessage msg = await service.CheckOutVisitor(visitor);
                if (msg.ResponseCode != 0)
                {
                    MessageDialog md = new MessageDialog("Could not complete Check Out: " + msg.Message);
                    await md.ShowAsync();
                    return;
                }

                this.Frame.Navigate(typeof(HubPage));
                MessageDialog tag = new MessageDialog("You have been checked out! Goodbye");
                await tag.ShowAsync();
                this.Frame.Navigate(typeof(HubPage));

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - CheckOutButton_Click");
                //msg.ShowAsync();
            }
        }

        private async void getUserDetail()
        {
            //This helps get user detail while data is filled out.
            try
            {
                if (txtPhoneNumber.Text != null)
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
                        txtVisitorName.Text = response.GuestName;
                        txtVisitorName.Visibility = Visibility.Visible;
                        string photoString = response.GuestPhotstring;

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
                        txtVisitorName.Text = "User not Registered";
                        PhotoCopy.Source = null;
                    } 
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - getUserDetail()");
                //msg.ShowAsync();
            }
        }

        private void txtPhoneNumber_keyUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                //For check sytax entered in PhoneNumber field
                CheckSyntax.checkOnlyNumber(sender, e);

                if (txtPhoneNumber.Text.Length > 7)
                {
                    getUserDetail();
                    txtVisitorName.Visibility = Visibility.Visible;
                }
                else
                {
                    txtVisitorName.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Void - txtPhoneNumber_keyUp");
                //msg.ShowAsync();
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage),_activePage);
        }
    }
}
