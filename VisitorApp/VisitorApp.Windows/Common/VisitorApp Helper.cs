using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorApp.Common;
using VisitorApp.Dashboard.Admin;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace VisitorApp.Models
{

    public class VisitorAppHelper
    {
        IRandomAccessStream stream;
        BitmapImage bitmap;
        DataReader reader;

        private char Delimiter = ',';
        private char LineDelimiter = '\n';

        public Frame Frame = new Frame();

        public async Task<BitmapImage> GetImage(StorageFile LogoImage)
        {
            //Read Selected Image to Bitmap for application Image Rendering
            stream = await LogoImage.OpenAsync(FileAccessMode.Read);
            bitmap = new BitmapImage();
            bitmap.SetSource(stream);
            return bitmap;
        }

        public async Task<string> ConvertImageToBase64()
        {
            // Create a byte array for the image to convert image to Base 64
            string photoString;
            Byte[] bytes = new Byte[0];
            reader = new DataReader(stream.GetInputStreamAt(0));
            bytes = new Byte[stream.Size];
            await reader.LoadAsync((uint)stream.Size);
            reader.ReadBytes(bytes);
            // Convert the byte array to Base 64 string
            photoString = Convert.ToBase64String(bytes);
            return photoString;
        }

        public async Task<StorageFile> selectImage()
        {
            //Open Drives and Search for Pictures of these format
            var image = new FileOpenPicker();
            image.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            image.ViewMode = PickerViewMode.List;
            image.FileTypeFilter.Add(".jpeg");
            image.FileTypeFilter.Add(".png");
            image.FileTypeFilter.Add(".jpg");
            image.FileTypeFilter.Add(".bmp");
            StorageFile ImageFile = await image.PickSingleFileAsync();

            return ImageFile;
        }

        public void signOut()
        {
            this.Frame.Navigate(typeof(HubPage));        
        }

        public static async Task<BitmapImage> Base64StringToBitmap(string base64String)
        {
            BitmapImage bmp = new BitmapImage();
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    var stream = ms.AsRandomAccessStream();
                    stream.Seek(0);
                    // create bitmap and assign
                    await bmp.SetSourceAsync(stream);
                    return bmp; // where image is an Image control in XAML
                }
            }
            catch (Exception ex)
            {

                var x = ex.Message;
                return bmp;
            }

        }

        public async Task<StorageFile> selectCsv()
        {
            //Open Drives and Search for Pictures of these format
            var csv = new FileOpenPicker();
            csv.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            csv.ViewMode = PickerViewMode.List;
            csv.FileTypeFilter.Add(".csv");
            StorageFile CsvFile = await csv.PickSingleFileAsync();

            return CsvFile;
        }

        public async Task<string> GetCsv(StorageFile csv)
        {
            //Read CSV
            string testString = null;

            stream = await csv.OpenAsync(FileAccessMode.Read);
            using (StreamReader reader = new StreamReader(stream.AsStream()))
            {
                testString = reader.ReadToEnd();
            }

            return testString;
        }

        public async Task<List<string>> GetCsvHeader(string csv)
        {
            //Read CSV Header
            var fieldList = new List<string>();

            var firstLine = csv.Split(this.LineDelimiter).FirstOrDefault();

            if (firstLine != null)
            {
                fieldList = firstLine.TrimEnd().Split(this.Delimiter).ToList();
            }

            return fieldList;
        }

        public async Task<List<Dictionary<string, string>>> GetCsvBody(string csv, List<string> HeaderList)
        {
            //Read CSV Body

            List<Dictionary<string, string>> parsedResult = new List<Dictionary<string, string>>();
            string[] records = csv.Split(this.LineDelimiter);

            int startingRow = 1;

            //if (this.HasHeaderRow)
            //{
            //    startingRow = 1;
            //    fieldList = LoadFieldNamesFromHeaderRow();
            //}

            for (int i = startingRow; i < records.Length; i++)
            {
                string record = records[i];

                string[] fields = record.Split(this.Delimiter);
                Dictionary<string, string> recordItem = new Dictionary<string, string>();

                int fieldIncrementer = 0;


                foreach (var field in fields)
                {
                    string key = fieldIncrementer.ToString();

                        if (fields.Length == HeaderList.Count)
                        {
                            key = HeaderList[fieldIncrementer];
                        }

                    recordItem.Add(key, field);
                    fieldIncrementer++;

                }

                parsedResult.Add(recordItem);
            }

            return parsedResult;

        }

        public async Task<string> GenerateIvCode()
        {
            string ivCode = DateTime.Now.Millisecond.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
            return ivCode;
        }

        public async Task<List<string>> isInternetAvailible()
        {
            List<string> response = new List<string>();
            bool shouldContinue = false;
            string msg = "No Internet Connection." + "\n" + "Please connect to Internet and Try Again";
            ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();

            if (profile.GetNetworkConnectivityLevel() >= NetworkConnectivityLevel.InternetAccess)
            {
                shouldContinue = true;
                msg = "Yes internet";
            }

            response.Add(shouldContinue.ToString());
            response.Add(msg);

            return response;
        }
    }

    public class UserProcess
    {
        private string adsString = "@live.com";

        public async Task<string> CreateUserName(string email)
        {
            var positionNumber = 0;
            foreach (char character in email)
            {
                if (character.ToString() == "@")
                {
                    break;
                }
                positionNumber += 1;
            }

            string staffEmailName = email.Substring(0, positionNumber);
            string remalinPart = email.Substring(positionNumber + 1);

            var positionNumberh = 0;

            foreach (char character in remalinPart)
            {
                if (character.ToString() == ".")
                {
                    break;
                }
                positionNumberh += 1;
            }

            string companyEmailName = remalinPart.Substring(0, positionNumberh);
            string companyDot = remalinPart.Substring(positionNumberh+1);

            string userName = staffEmailName + "_Company_" + companyEmailName  + adsString;
            //string userName = staffEmailName + "_Company_" + companyEmailName + "_Dot_"+companyDot + adsString;
            return userName;
        }
    }

    public class GetDataFromDB
    {
        RemoteService service = new RemoteService();

        public async Task<ResponseMessage> GetDataThisStaff(int staffId)
        {
            StaffDataPayload ThisStaffPayLoad = new StaffDataPayload()
            {
                StaffId = staffId
            };

            var response = service.GetThisStaffControllerService(ThisStaffPayLoad);
            return response;
        }

        public async Task<ResponseMessage> GetDataThisStaffFromPhoNo(int compID, string PhoneNumber)
        {
            StaffDataPayload ThisStaffPayLoad = new StaffDataPayload()
            {
                CompanyId = compID,
                StaffPhoneNumber=PhoneNumber
            };

            var response = service.GetThisStaffFromPhoNoControllerService(ThisStaffPayLoad);
            return response;
        }

        public async Task<ResponseMessage> GetDataThisCompany(int CompanyId)
        {
            CompanyDataPayLoad ThisCompanyPayLoad = new CompanyDataPayLoad()
            {
                CompanyId = CompanyId
            };

            var response = service.GetThisCompanyControllerService(ThisCompanyPayLoad);
            return response;
        }

        public async Task<ResponseMessage> GetDataThisDepartment(int DepartmentId)
        {
            DepartmentDataPayload ThisDepartmentPayLoad = new DepartmentDataPayload()
            {
                DepartmentId = DepartmentId
            };

            var response = service.GetThisDepartmentControllerService(ThisDepartmentPayLoad);
            return response;
        }

        public async Task<ResponseMessage> GetDataThisAppointment(int AppointmentId)
        {
            AppointmentDataPayload ThisAppointmentPayLoad = new AppointmentDataPayload()
            {
                AppointmentId = AppointmentId
            };

            var response = service.GetThisAppointmentControllerService(ThisAppointmentPayLoad);
            return response;
        }

        public async Task<ResponseMessage> GetDataThisAppointmentFromIv(int CompId, string IvCode)
        {
            AppointmentDataPayload ThisAppointmentPayLoad = new AppointmentDataPayload()
            {
                CompanyId = CompId,
                InvitationCode = IvCode
            };

            var response = service.GetThisAppointmentFromIvControllerService(ThisAppointmentPayLoad);
            return response;
        }

        public async Task<ResponseMessage> GetDataThisLocation(int LocationId)
        {
            SecureLocationDataPayload ThisLocationPayLoad = new SecureLocationDataPayload()
            {
                LocationId = LocationId
            };

            var response = service.GetThisSecureLocationControllerService(ThisLocationPayLoad);
            return response;
        }

        public async Task<ResponseMessage> GetDataThisVisitorDetail(long visitorPhoNo)
        {
            VisitorDataPayLoad visitor = new VisitorDataPayLoad
            {
                PhoneNumber = visitorPhoNo
            };

            var response = service.VisitorDetailControllerService(visitor);
            return response;
        }

        public async Task<ResponseMessage> GetDataCompanyDepartments(int CompanyId)
        {
            DepartmentDataPayload DepartmentPayLoad = new DepartmentDataPayload()
            {
                CompanyId = CompanyId
            };

            var response = service.GetDepartmentListControllerService(DepartmentPayLoad);
            return response;
        }

        public async Task<ResponseMessage> GetDataStaffOfDepartment(int DeptId, int compId)
        {
            DepartmentDataPayload DepartmentPayLoad = new DepartmentDataPayload()
            {
                DepartmentId = DeptId,
                CompanyId = compId
            };

            var response = service.GetDepartmentStaffListControllerService(DepartmentPayLoad);
            return response;
        }

        public async Task<ResponseMessage> GetDataCompanyStaff(int CompanyId)
        {
            StaffDataPayload StaffPayLoad = new StaffDataPayload()
            {
                CompanyId = CompanyId
            };

            var response = service.GetStaffListControllerService(StaffPayLoad);
            return response;
        }

        public async Task<ResponseMessage> GetDataCompanyAppointment(int CompanyId)
        {
            AppointmentDataPayload AppointmentPayLoad = new AppointmentDataPayload()
            {
                CompanyId = CompanyId
            };

            var response = service.GetAppointmentListControllerService(AppointmentPayLoad);
            return response;
        }

        public async Task<ResponseMessage> GetDataCompanySecureLocation(int CompanyId)
        {
            SecureLocationDataPayload SecureLocationPayLoad = new SecureLocationDataPayload()
            {
                CompanyId = CompanyId
            };

            var response = service.GetSecureLocationListControllerService(SecureLocationPayLoad);
            return response;
        }


        public async Task<ResponseMessage> GetDataStaffAppointment(int staffId)
        {
            AppointmentDataPayload AppointmentPayLoad = new AppointmentDataPayload()
            {
                HostStaffId = staffId
            };

            var response = service.GetThisStaffAppointmentListControllerService(AppointmentPayLoad);
            return response;
        }

    }

    public class AddDataToDB
    {
        RemoteService service = new RemoteService();

        public async Task<ResponseMessage> RegisterNewCompany (CompanyDataPayLoad company)
        {
            var response = await service.RegisterNewCompany(company);
            return response;
        }

        public async Task<ResponseMessage> RegisterNewDepartment(DepartmentDataPayload department)
        {
            var response = await service.RegisterNewDepartment(department);
            return response;
        }

        public async Task<ResponseMessage> RegisterNewStaff(StaffDataPayload staff)
        {
            var response = await service.RegisterNewStaff(staff);
            return response;
        }

        public async Task<ResponseMessage> RegisterNewSecureLocation(SecureLocationDataPayload secureLocation)
        {
            var response = await service.RegisterNewSecureLocation(secureLocation);
            return response;
        }

        public async Task<ResponseMessage> RegisterNewAppointment(AppointmentDataPayload appointment)
        {
            var response = await service.RegisterNewAppointment(appointment);
            return response;
        }
    }

    public class GuestColleagueJSON
    {
        public string GuestColleagueFullName { get; set; }
        public string GuestColleaguePhoNo { get; set; }
        public string GuestColleagueCheckInTime { get; set; }
        public string GuestColleagueCheckOutTime { get; set; }
        public string GuestColleagueStatus { get; set; }
        public string GuestColleaguePhotoString { get; set; }
    }

    public class VisitorAppGuestColleague
    {
        char separateLines = '\n';
        char separateFields = ';';

        public async Task<GuestColleagueJSON> FormatToGuestColleagueJSON(string GuestColleagueName, string GuestColleaguePhoNo, string GuestColleaguePhotoString)
        {
            DateTime time = DateTime.Now;

            GuestColleagueJSON GuestColleagueJSON = new GuestColleagueJSON();

            GuestColleagueJSON.GuestColleagueFullName = GuestColleagueName;
            GuestColleagueJSON.GuestColleaguePhoNo = GuestColleaguePhoNo;
            GuestColleagueJSON.GuestColleagueCheckInTime = time.ToString();
            GuestColleagueJSON.GuestColleagueCheckOutTime = time.ToString();
            GuestColleagueJSON.GuestColleagueStatus = GuestStatus.StillCheckedIn.ToString();
            GuestColleagueJSON.GuestColleaguePhotoString = GuestColleaguePhotoString;

            return GuestColleagueJSON;
        }

        public async Task<string> SerializeGuestColleagueToJSON(string title,GuestColleagueJSON GuestColleagueJSON)
        {
            string JSON="";
            JSON = title + "["
                        + "Name:" + GuestColleagueJSON.GuestColleagueFullName + ";"
                        + "Phone Number:" + GuestColleagueJSON.GuestColleaguePhoNo + ";"
                        + "Check in Time:" + GuestColleagueJSON.GuestColleagueCheckInTime + ";"
                        + "Check out Time:" + GuestColleagueJSON.GuestColleagueCheckOutTime + ";"
                        + "Status:" + GuestColleagueJSON.GuestColleagueStatus + ";"
                        + "]" ;

            return JSON;
        }

        public async Task<string> PrintGuestColleagueToJSON(string JSON)
        {
            return ("{" + JSON.Trim() + "}");
        }

        public async Task<List<GuestColleagueJSON>> DeserializeGuestColleagueToJSON(string JSON)
        {
            List<GuestColleagueJSON> ListGuestColleagueToJSON = new List<GuestColleagueJSON>();

            JSON = JSON.Substring(1, JSON.Length - 2);

            string[] JSONLines = JSON.Split(separateLines);

            for (int i = 0; i < JSONLines.Length; i++)
            {
                string LineJSON = JSONLines[i];
                int startChar = LineJSON.IndexOf('[');
                LineJSON = LineJSON.Substring(startChar, (LineJSON.Length - (startChar + 2)));

            }

            return ListGuestColleagueToJSON;
        }

        public async Task<string[]> DeFormatToGuestColleagueJSON (string JSON)
        {
            string[] JSONLines = JSON.Split(separateLines);
            return JSONLines;
        }
    }
}
