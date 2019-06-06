using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorApp.Common
{
    public enum AppointmentStatus
    {
        Pending = 1,
        Done =2,
        Cancel = 3,
        Delete
    }

    public enum StaffStatus
    {
        Active = 1,
        Suspended =2,
        Deleted
    }

    public enum DeptStatus
    {
        Active = 1,
        Archived = 2,
        Deleted
    }

    public enum SecureLocationStatus
    {
        Active = 1,
        Archived = 2,
        Deleted
    }

    public enum StaffRoles
    {
        Admin = 1,
        FrontDesk = 2,
        Staff = 3
    }

    public enum _activePageMsg
    {
        DashBoardTodayGuest = 1, //This tells the app that the dashboard today's guest button navigated it to this page
        DashBoardStillCheckInGuest = 2  //This tells the app that the dashboard still check-in guest button navigated it to this page
    }

    public enum CompanyStatus
    {
        Active = 1,
        Suspend = 2,
        Deleted
    }

    public enum GuestStatus
    {
        StillCheckedIn = 1,
        CheckedOut
    }

    public enum Pages
    {
        Dashboard = 1,
        Department = 2,
        Staff = 3,
        Appointment = 4,
        CheckInGuest = 5,
        CheckOut = 6,
        Location = 7,
        SeeGuestList = 8
    }
}
