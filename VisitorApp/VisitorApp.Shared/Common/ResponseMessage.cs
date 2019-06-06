using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using VisitorApp.Data;

namespace VisitorApp.Common
{
    public class ResponseMessage
    {
        public int ResponseCode { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }

        public string GuestName { get; set; }
        public string GuestPhoneNumber { get; set; }
        public string GuestEmail { get; set; }
        public string GuestPhotstring { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }

        public List<GuestGlobal> GuestList { get; set; }
        public List<VisitorGlobal> VisitorList { get; set; }
        public List<AppointmentGlobal> AppointmentList { get; set; }
        public List<SecureLocationGlobal> SecureLocationList { get; set; }
        public List<DepartmentGlobal> DepartmentList { get; set; }
        public List<StaffGlobal> StaffList { get; set; }
        public List<CompanyGlobal> CompanyList { get; set; }
 
        public int AppointmentId { get; set; }
        public int StaffId { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public int LocationId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string CompanyEmailAddress { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyLogo { get; set; }
    }

    public class GuestGlobal
    {
        public int GuestId { get; set; }
        public int VisitorId { get; set; }
        public string GuestName { get; set; }
        public string HostName { get; set; }
        public string CheckInCode { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public string ExtraGuest { get; set; }
        public string Status { get; set; }
        public string GuestPhotoString { get; set; }
        public string GuestCompany { get; set; }
        public string GuestPhoneNumber { get; set; }
        public string GuestEmail { get; set; }
        public string GuestSignature { get; set; }
        public string GuestThumbnail { get; set; }
        //
        public int CompanyId { get; set; }
    }


    public class VisitorGlobal
    {
        public int VisitorId { get; set; }
        public string VisitorFullName { get; set; }
        public string CompanyName { get; set; }
        public string emailAddress { get; set; }
        public string phoneNumber { get; set; }
        public string photoString { get; set; }
        public DateTime EntryDate { get; set; }
        public string Signature { get; set; }
        public string ThumbPrint { get; set; }
        public int CompanyId { get; set; }
    }

    public class AppointmentGlobal
    {
        public int AppointmentId { get; set; }
        public string GuestName { get; set; }
        public string GuestPhoneNumber { get; set; }
        public string GuestCompanyName { get; set; }
        public int HostStaffId { get; set; }
        public int CreatedByStaffId { get; set; }
        public string StaffName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string InvitationCode { get; set; }
        public DateTime MeetingStartDateTime { get; set; }
        public DateTime MeetingEndDateTime { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int CompanyId { get; set; }
    }

    public class SecureLocationGlobal
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string LocationCode { get; set; }
        public string PhoneNumber { get; set; }
        public int FloorNumber { get; set; }
        public string MapPoint { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int CompanyId { get; set; }
    }

    public class DepartmentGlobal
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentPhoneNumber { get; set; }
        public int DeptSize { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }
        public int StaffCount { get; set; }
    }

    public class StaffGlobal
    {
        public int StaffId { get; set; }
        public string StaffFirstName { get; set; }
        public string StaffLastName { get; set; }
        public string StaffName { get; set; }
        public string StaffPhoneNumber { get; set; }
        public string StaffEmail { get; set; }
        public string StaffIdNumber { get; set; }
        public string StaffStatus { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentPhoneNo { get; set; }
        public string DepartmentName { get; set; }
        public string StaffPhoto { get; set; }
        public string Roles { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }
    }

    public class CompanyGlobal
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string CompanyEmailAddress { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyStatus { get; set; }
    }
}
