using System;
using System.Collections.Generic;
using System.Text;
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
        //
        public string Photo { get; set; }
        public string Signature { get; set; }
        public string ThumbPrint { get; set; }
    }
    public interface IRemoteService
    {
        Task<ResponseMessage> RegisterNewUser(VisitorDataPayLoad data);
        Task<ResponseMessage> CheckInRegisteredUser(VisitorDataPayLoad data);
        Task<ResponseMessage> CheckInWithInvitation(VisitorDataPayLoad data);
        Task<ResponseMessage> CheckOutVisitor(VisitorDataPayLoad data);

    }
}
