using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace VisitorApp.Common
{
    public class RemoteService : IRemoteService
    {
        private const string RegisterNewVisitorUrl = "http://localhost:49535/api/RegisterNewVisitor";
        private const string GetVisitorDetailUrl = "http://localhost:49535/api/GetVisitorDetail";
        private const string CheckInRegisteredUserUrl = "http://localhost:49535/api/CheckInRegisteredUser";
        private const string CheckInWithInvitationUrl = "http://localhost:49535/api/CheckInWithInvitation";
        private const string CheckOutVisitorUrl = "http://localhost:49535/api/CheckOutVisitor";
        private const string ConfirmTokenUrl = "http://localhost:49535/api/EmailUserConfirmToken";

        private const string CheckIfVisitorExistUrl = "http://localhost:49535/api/CheckIfVisitorExist";
        private const string GetDetailOnUserUrl = "http://localhost:49535/api/GetDetailOnUser";
        private const string GuestListControllerUrl = "http://localhost:49535/api/GuestList";
        private const string GuestStillCheckInListControllerUrl = "http://localhost:49535/api/GuestStillCheckInList";
        private const string VisitorListControllerUrl = "http://localhost:49535/api/VisitorList"; 
        private const string EmailAppointmentControllerUrl = "http://localhost:49535/api/SendEmailToGuest";

        #region Company
        private const string RegisterNewCompanyUrl = "http://localhost:49535/api/AddCompany";
        private const string CheckIfCompanyExistUrl = "http://localhost:49535/api/CheckIfCompanyExist";
        private const string GetCompanyListControllerUrl = "http://localhost:49535/api/GetCompaniesDetail";
        private const string GetThisCompanyControllerUrl = "http://localhost:49535/api/GetThisCompaniesDetail";
        private const string UpdateCompanyControllerUrl = "http://localhost:49535/api/UpdateComapny";
        #endregion

        #region Staff
        private const string RegisterNewStaffUrl = "http://localhost:49535/api/AddStaff";
        private const string CheckIfStaffExistUrl = "http://localhost:49535/api/CheckIfStaffExist";
        private const string GetStaffListControllerUrl = "http://localhost:49535/api/GetStaffDetail";
        private const string GetThisStaffControllerUrl = "http://localhost:49535/api/GetThisStaffDetail";
        private const string GetThisStaffFromPhoneNoControllerUrl = "http://localhost:49535/api/GetThisStaffDetailFromPhoNo";
        private const string UpdateStaffControllerUrl = "http://localhost:49535/api/UpdateStaff";
        private const string StaffForgotPasswordTokenControllerUrl = "http://localhost:49535/api/EmailUserNewPasswordToken";
        #endregion

        #region Department
        private const string RegisterNewDepartmentUrl = "http://localhost:49535/api/AddDepartment";
        private const string CheckIfDepartmentExistUrl = "http://localhost:49535/api/CheckIfDepartmentExist";
        private const string GetDepartmentListControllerUrl = "http://localhost:49535/api/GetDepartmentDetail";
        private const string GetDepartmentStaffListControllerUrl = "http://localhost:49535/api/GetStaffinDeptList";
        private const string GetThisDepartmentControllerUrl = "http://localhost:49535/api/GetThisDepartmentDetail";
        private const string UpdateThisDepartmentControllerUrl = "http://localhost:49535/api/UpdateDepartment";
        #endregion

        #region UsersLogin
        private const string RegisterNewUserUrl = "http://localhost:49535/api/AddUser";
        private const string UpdateUserUrl = "http://localhost:49535/api/UpdateUser";
        private const string CheckIfUserExistUrl = "http://localhost:49535/api/CheckIfUserExist";
        private const string ChangePasswordUserControllerUrl = "http://localhost:49535/api/ChangeUserPassword";
        private const string ChangeUserPasswordFromTokenControllerUrl = "http://localhost:49535/api/ChangeUserPasswordFromToken";
        #endregion

        #region Secure Location
        private const string RegisterNewSecureLocationUrl = "http://localhost:49535/api/AddSecureLocation";
        private const string CheckIfSecureLocationExistUrl = "http://localhost:49535/api/CheckIfSecureLocationExist";
        private const string GetSecureLocationListControllerUrl = "http://localhost:49535/api/GetSecureLocationDetail";
        private const string GetThisSecureLocationControllerUrl = "http://localhost:49535/api/GetThisSecureLocationDetail";
        private const string UpdateThisSecureLocationControllerUrl = "http://localhost:49535/api/UpdateSecureLocation";
        #endregion

        #region Appointment
        private const string RegisterNewAppointmentUrl = "http://localhost:49535/api/AddAppointment";
        private const string CheckIfAppointmentExistUrl = "http://localhost:49535/api/CheckIfAppointmentExist";
        private const string GetAppointmentListControllerUrl = "http://localhost:49535/api/GetAppointmentDetail";
        private const string GetThisAppointmentControllerUrl = "http://localhost:49535/api/GetThisAppointmentDetail";
        private const string GetThisAppointmentFromIvControllerUrl = "http://localhost:49535/api/GetThisStaffFromAppointmentIvCode";
        private const string GetThisStaffAppointmentListControllerUrl = "http://localhost:49535/api/GetThisStaffAppointmentDetail";
        private const string UpdateThisAppointmentControllerUrl = "http://localhost:49535/api/UpdateAppointment";
        #endregion

        private readonly HttpClient _client = new HttpClient();

        public async System.Threading.Tasks.Task<ResponseMessage> RegisterNewVisitor(VisitorDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(RegisterNewVisitorUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            //string responseString = await response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> CheckIfVisitorExistService(VisitorDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(CheckIfVisitorExistUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            //string responseString = await response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> GetDetailOnUserService(VisitorDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(GetDetailOnUserUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            //string responseString = await response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> GuestControllerService(VisitorDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(GuestListControllerUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage GuestListControllerService(VisitorDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response =  _client.PostAsync(GuestListControllerUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage GuestStillCheckInListControllerService(VisitorDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(GuestStillCheckInListControllerUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage VisitorListControllerService(VisitorDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response =  _client.PostAsync(VisitorListControllerUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage VisitorDetailControllerService(VisitorDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(GetVisitorDetailUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> CheckInRegisteredUser(VisitorDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(CheckInRegisteredUserUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            //string responseString = await response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> CheckInWithInvitation(VisitorDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(CheckInWithInvitationUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            //string responseString = await response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> CheckOutVisitor(VisitorDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(CheckOutVisitorUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            //string responseString = await response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }


        public async System.Threading.Tasks.Task<ResponseMessage> EmailRegisterNewAppointment(AppointmentDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(EmailAppointmentControllerUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            //string responseString = await response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }


        #region Company
        public async System.Threading.Tasks.Task<ResponseMessage> RegisterNewCompany(CompanyDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(RegisterNewCompanyUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> CheckIfCompanyExistService(CompanyDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(CheckIfCompanyExistUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            //string responseString = await response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage GetCompanyListControllerService(CompanyDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(GetCompanyListControllerUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage GetThisCompanyControllerService(CompanyDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(GetThisCompanyControllerUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> UpdateThisCompanyControllerService(CompanyDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(UpdateCompanyControllerUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }
        #endregion

        #region Staff
        public async System.Threading.Tasks.Task<ResponseMessage> RegisterNewStaff(StaffDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(RegisterNewStaffUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> CheckIfStaffExistService(StaffDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(CheckIfStaffExistUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            //string responseString = await response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage GetStaffListControllerService(StaffDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(GetStaffListControllerUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage GetThisStaffControllerService(StaffDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(GetThisStaffControllerUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage GetThisStaffFromPhoNoControllerService(StaffDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(GetThisStaffFromPhoneNoControllerUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> UpdateThisStaffControllerService(StaffDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(UpdateStaffControllerUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> ConfirmTokenControllerService(EmailTokenDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(ConfirmTokenUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> StaffForgotPasswordTokenControllerTokenService(EmailTokenDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(StaffForgotPasswordTokenControllerUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }
        #endregion

        #region Department
        public async System.Threading.Tasks.Task<ResponseMessage> RegisterNewDepartment(DepartmentDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(RegisterNewDepartmentUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> CheckIfDepartmentExistService(DepartmentDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(CheckIfDepartmentExistUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            //string responseString = await response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage GetDepartmentListControllerService(DepartmentDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(GetDepartmentListControllerUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage GetDepartmentStaffListControllerService(DepartmentDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(GetDepartmentStaffListControllerUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage GetThisDepartmentControllerService(DepartmentDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(GetThisDepartmentControllerUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> UpdateThisDepartmentControllerService(DepartmentDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(UpdateThisDepartmentControllerUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }
        #endregion

        #region New User
        public async System.Threading.Tasks.Task<ResponseMessage> RegisterNewUser(UserLoginDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(RegisterNewUserUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            //string responseString = await response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> CheckIfUserExistService(UserLoginDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(CheckIfUserExistUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            //string responseString = await response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> UpdateThisUserService(UserLoginDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(UpdateUserUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> ChangeThisUSerPasswordControllerService(UserLoginDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(ChangePasswordUserControllerUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> ChangeUserPasswordFromTokenControllerService(UserLoginDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(ChangeUserPasswordFromTokenControllerUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }
        #endregion

        #region Secure Location
        public async System.Threading.Tasks.Task<ResponseMessage> RegisterNewSecureLocation(SecureLocationDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(RegisterNewSecureLocationUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            //string responseString = await response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> CheckIfSecureLocationExistService(SecureLocationDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(CheckIfSecureLocationExistUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            //string responseString = await response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage GetSecureLocationListControllerService(SecureLocationDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(GetSecureLocationListControllerUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage GetThisSecureLocationControllerService(SecureLocationDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(GetThisSecureLocationControllerUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> UpdateThisSecureLocationControllerService(SecureLocationDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(UpdateThisSecureLocationControllerUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }
        #endregion

        #region Appointment
        public async System.Threading.Tasks.Task<ResponseMessage> RegisterNewAppointment(AppointmentDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(RegisterNewAppointmentUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            //string responseString = await response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> CheckIfAppointmentExistService(AppointmentDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(CheckIfAppointmentExistUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            //string responseString = await response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage GetAppointmentListControllerService(AppointmentDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(GetAppointmentListControllerUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage GetThisAppointmentControllerService(AppointmentDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(GetThisAppointmentControllerUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage GetThisAppointmentFromIvControllerService(AppointmentDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(GetThisAppointmentFromIvControllerUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public ResponseMessage GetThisStaffAppointmentListControllerService(AppointmentDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = _client.PostAsync(GetThisStaffAppointmentListControllerUrl, content).Result;
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }

        public async System.Threading.Tasks.Task<ResponseMessage> UpdateThisAppointmentControllerService(AppointmentDataPayload data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(UpdateThisAppointmentControllerUrl, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            ResponseMessage msg = JsonConvert.DeserializeObject<ResponseMessage>(rep.Result);
            //
            return msg;
        }
        #endregion
    }
}
