using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace VisitorApp.Common
{
    public class RemoteService : IRemoteService
    {
        private const string RegisterNewUserUrl = "http://localhost:49535/api/RegisterNewUser";
        private const string CheckInRegisteredUserUrl = "http://localhost:49535/api/CheckInRegisteredUser";
        private const string CheckInWithInvitationUrl = "http://localhost:49535/api/CheckInWithInvitation";
        private const string CheckOutVisitorUrl = "http://localhost:49535/api/CheckOutVisitor";

        private const string CheckIfUserExist = "http://localhost:49535/api/CheckIfUserExist";

        private const string GetDetailOnUser = "http://localhost:49535/api/GetDetailOnUser";


        private readonly HttpClient _client = new HttpClient();

        public async System.Threading.Tasks.Task<ResponseMessage> RegisterNewUser(VisitorDataPayLoad data)
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

        public async System.Threading.Tasks.Task<ResponseMessage> CheckIfUserExistService(VisitorDataPayLoad data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(CheckIfUserExist, content);
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
            var response = await _client.PostAsync(GetDetailOnUser, content);
            //
            var rep = response.Content.ReadAsStringAsync();
            //response.EnsureSuccessStatusCode();
            //string responseString = await response.Content.ReadAsStringAsync();
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
    }

}
