using System;
using System.Threading.Tasks;

namespace VisitorApp.Common
{
    public class VisitorDataPayLoad
    {
        public string FullName { get; set; }
        public long PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string CompanyName { get; set; }
        public string HostName { get; set; }
        public string GuestName { get; set; }
        public string InvitationCode { get; set; }
        public string CheckInCode { get; set; }
        public string Comment { get; set; }
        public string ExtraGuest { get; set; }
        //
        public string Photo { get; set; }
        public string Signature { get; set; }
        public string ThumbPrint { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }
        public int VisitorId { get; set; }
        public bool AllGuestOut { get; set; }
    }

    public class CompanyDataPayLoad
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string CompanyEmailAddress { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyStatus { get; set; }
        public string Description { get; set; }

    }

    public class StaffDataPayload
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
        public int CompanyId { get; set; }
        public string StaffPhoto { get; set; }
        public string Role { get; set; }
        public string Description { get; set; }
    }

    public class DepartmentDataPayload
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentPhoneNumber { get; set; }
        public int deptSize { get; set; }
        public int CompanyId { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }

    public class AppointmentDataPayload
    {
        public int AppointmentId { get; set; }
        public string GuestName { get; set; }
        public string GuestPhoneNumber { get; set; }
        public string GuestCompanyName { get; set; }
        public string GuestEmail { get; set; }
        public int HostStaffId { get; set; }
        public int LocationId { get; set; }
        public string InvitationCode { get; set; }
        public DateTime MeetingStartDateTime { get; set; }
        public DateTime MeetingEndDateTime { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int CompanyId { get; set; }
        public int CreatedByStaffId { get; set; }
    }

    public class SecureLocationDataPayload
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

    public class UserLoginDataPayLoad
    {
        public string username { get; set; }
        public string userPassword { get; set; }
        public string userStatus { get; set; }
        public int StaffId { get; set; }
    }

    public class EmailTokenDataPayLoad
    {
        public string email { get; set; }
        public string ipAddress { get; set; }
        public string sentToken { get; set; }
    }

    public interface IRemoteService
    {
        Task<ResponseMessage> RegisterNewVisitor(VisitorDataPayLoad data);
        Task<ResponseMessage> CheckInRegisteredUser(VisitorDataPayLoad data);
        Task<ResponseMessage> CheckInWithInvitation(VisitorDataPayLoad data);
        Task<ResponseMessage> CheckOutVisitor(VisitorDataPayLoad data);

        Task<ResponseMessage> RegisterNewCompany(CompanyDataPayLoad data);
        Task<ResponseMessage> RegisterNewStaff(StaffDataPayload data);
        Task<ResponseMessage> RegisterNewDepartment(DepartmentDataPayload data);
    }
}
