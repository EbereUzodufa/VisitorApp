using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VisitorApp.Models;
using VisitorApp.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VisitorApp.Dashboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChooseEvent : Page
    {
        AccountDetails _activePage = new AccountDetails();
        GetDataFromDB GetDataFromDB = new GetDataFromDB();
        int CompId;

        public ChooseEvent()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateBinding();
        }

        private void CreateBinding()
        {
            var theDeptSource = GetDataFromDB.GetDataCompanyDepartments(CompId).Result;
            var theDeptSourceList = theDeptSource.DepartmentList;
            if (theDeptSourceList != null)
            {
                GridEvents.ItemsSource = theDeptSourceList;
            }
        }

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
        }

        private async void GridEvents_ItemClick(object sender, ItemClickEventArgs e)
        {
            GridEvents.SelectedItem = e.ClickedItem;
            var events = (DepartmentGlobal)GridEvents.SelectedItem;
            MessageDialog msg = new MessageDialog(events.DepartmentName + "\n" + events.DepartmentPhoneNumber);
            await msg.ShowAsync();
        }

        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBackEvents_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FrontDeskHome), _activePage);
        }

        private void btnCreateOne_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Events), _activePage);
        }
    }
}
