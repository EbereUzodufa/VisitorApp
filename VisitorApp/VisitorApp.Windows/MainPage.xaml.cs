using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using VisitorApp.Data;
using VisitorApp.Common;
using Windows.UI.Popups;
using Windows.UI;
using System.Net.NetworkInformation;
using Windows.Networking.Connectivity;
using VisitorApp.Models;
//using static VisitorApp.ViewModels.MainViewModel;

// The Universal Hub Application project template is documented at http://go.microsoft.com/fwlink/?LinkID=391955

namespace VisitorApp
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        double sizeIncrement = 10;
        bool shouldContinue;
        AccountDetails _activePage;

        /// <summary>
        /// Gets the NavigationHelper used to aid in navigation and process lifetime management.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the DefaultViewModel. This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            //Loaded += HubPage_Loaded;
        }

        //private void HubPage_Loaded(object sender, RoutedEventArgs e)
        //{
        //    this.Frame.Navigate(typeof(login));
        //}

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-4");
            this.DefaultViewModel["Section3Items"] = sampleDataGroup;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _activePage = (AccountDetails)e.Parameter;
            if (_activePage != null)
            {
                txbTitle.Text= ("Welcome To " + _activePage.CompanyName).ToUpper();
                txbUserName.Text = _activePage.UserStaffName;
            }
        }

        /// <summary>
        /// Invoked when a HubSection header is clicked.
        /// </summary>
        /// <param name="sender">The Hub that contains the HubSection whose header was clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            var group = section.DataContext;
            this.Frame.Navigate(typeof(sample), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((Data.SampleDataItem)e.ClickedItem).UniqueId;
            //this.Frame.Navigate(typeof(ItemPage), itemId);
        }
        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </summary>
        #endregion

        private void UploadCSV_Click(object sender, RoutedEventArgs e)
        {
            proceedCriteria();
            if (shouldContinue != false)
            {
                this.Frame.Navigate(typeof(UploadCsv), _activePage);

            }
        }

        private void RegisteredButton_Click(object sender, RoutedEventArgs e)
        {
            proceedCriteria();
            if (shouldContinue != false)
            {
                this.Frame.Navigate(typeof(RegistrationPage), _activePage);

            }
        }

        private void InvitationButton_Click(object sender, RoutedEventArgs e)
        {
            proceedCriteria();
            if (shouldContinue != false)
            {
                this.Frame.Navigate(typeof(CheckInWithInvitationPage), _activePage);

            }
        }

        private void CheckOutButton_Click(object sender, RoutedEventArgs e)
        {
            proceedCriteria();
            if (shouldContinue != false)
            {
                this.Frame.Navigate(typeof(CheckOutPage), _activePage);
            }
        }

        private void SeeGuestList_Click(object sender, RoutedEventArgs e)
        {
            proceedCriteria();
            if (shouldContinue != false)
            {
                this.Frame.Navigate(typeof(DisplayTV), _activePage);
            }
        }

        private void SeeGuestListAllDetails_Click(object sender, RoutedEventArgs e)
        {
            proceedCriteria();
            if (shouldContinue!=false)
            {
                this.Frame.Navigate(typeof(TV), _activePage);

            }
        }

        private void proceedCriteria()
        {
            shouldContinue = false;
            ConnectionProfile profile =
                   NetworkInformation.GetInternetConnectionProfile();

            if (profile.GetNetworkConnectivityLevel() >=
                        NetworkConnectivityLevel.InternetAccess)
            {
                shouldContinue = true;
            }
            else
            {
                MessageDialog msg = new MessageDialog("No Internet Connection." + "\n" + "Please connect to Internet and Try Again");
                msg.ShowAsync();
            }
        }

        private void StackPanel_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Button bordy = sender as Button;

            bordy.Width += sizeIncrement;
            bordy.Height += sizeIncrement;
        }

        private void StackPanel_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Button bordy = sender as Button;

            bordy.Width -= sizeIncrement;
            bordy.Height -= sizeIncrement;
        }

        private void Border_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Border borddy = sender as Border;
            borddy.Background = new SolidColorBrush(Colors.Pink);
        }

        private void Border_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Border borddy = sender as Border;
            borddy.Background = new SolidColorBrush(Colors.Transparent);
        }
    }
}
