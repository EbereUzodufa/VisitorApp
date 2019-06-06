using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VisitorApp.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VisitorApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestPage : Page
    {
        AccountDetails _activePage = new AccountDetails();

        public TestPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _activePage = (AccountDetails)e.Parameter;
            if (_activePage != null)
            {
                txbComoanyName.Text = (_activePage.CompanyName).ToUpper();
                txbStaffName.Text = (_activePage.UserStaffName).ToUpper();
                txbStaffPhonenUmber.Text = (_activePage.UserStaffPhoneNumber).ToUpper();
                //txbDepartmentName.Text = (_activePage.DepartmentName).ToUpper();
                txbStaffEmail.Text = (_activePage.UserStaffRole).ToUpper();
                txbStaffID.Text = (_activePage.UserStaffIdNumber).ToUpper();

                imgCompanyPhoto.Source = _activePage.CompanyLogo;
                imgUserPhoto.Source = _activePage.UserStaffPhoto;
            }
        }
    }
}
