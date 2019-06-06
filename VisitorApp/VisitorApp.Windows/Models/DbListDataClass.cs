using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace VisitorApp.Models
{
    public class DisplayDetails
    {
        //Handle Guests Details List

        //Serialize
        public string sn { get; set; }

        //Display Details
        public string GuestFullName { get; set; }
        public string GuestHostName { get; set; }
        public string GuestCompany { get; set; }
        public string GuestPhoneNumber { get; set; }
        public string GuestEmail { get; set; }
        public string GuestCheckInCode { get; set; }
        public DateTime GuestCheckInTime { get; set; }
        public DateTime GuestCheckOutTime { get; set; }
        public string GuestStatus { get; set; }
        public string GuestSignature { get; set; }      //Guest Signature
        public string GuestThumbPrint { get; set; }      //ThumbPrint of Guest
        public BitmapImage Picture { get; set; }    //Guest Picture
    }

    public class CsvUsersDetails
    {
        //Handles CSV Upload List
        public string sn { get; set; }
        public string Fullname { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class CsvDeptsDetails
    {
        //Handles CSV Upload List
        public string sn { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentPhoneNumber { get; set; }
    }

    public class AccountDetails
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string CompanyLogoString { get; set; }
        public string CompanyStatus { get; set; }
        public BitmapImage CompanyLogo { get; set; }
        public string UserStaffName { get; set; }
        public string UserStaffRole { get; set; }
        //public string UserId { get; set; }
        public string UserStaffPhotoString { get; set; }
        public BitmapImage UserStaffPhoto { get; set; }
        public int UserStaffId { get; set; }
        public string UserStaffPhoneNumber { get; set; }
        public string UserStaffIdNumber { get; set; }
        public int UserDepartmentId { get; set; }
        public string UserDepartmentName { get; set; }
        public string PageMsg { get; set; } //any message for the next page from exiting page
        public string LogOutGuestphoneNumber { get; set; }  //phone number of guest to be checked out
    }

    public class GuestColleagueDetails
    {
        public string GuestColleagueFullName { get; set; }
        public string GuestColleagueEmail { get; set; }
        public string GuestColleaguePhoneNumber { get; set; }
        public string GuestColleaguePhotoString { get; set; }
    }
}
