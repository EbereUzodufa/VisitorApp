//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Threading.Tasks;
//using Windows.Data.Json;
//using Windows.Storage;
//using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Media.Imaging;

//namespace VisitorApp.Common
//{
//    public class VisitorData
//    {
//        public VisitorData(String visitorId, String visitorFullName, String companyName, String emailAddress, String phoneNumber)
//        {
//            this.VisitorId = visitorId;
//            this.VisitorFullName = visitorFullName;
//            this.CompanyName = companyName;
//            this.EmailAddress = emailAddress;
//            this.PhoneNumber = phoneNumber;
//            //this.PhotoString = photoString;
//        }

//        public string VisitorId { get; private set; }
//        public string VisitorFullName { get; private set; }
//        public string CompanyName { get; private set; }
//        public string EmailAddress { get; private set; }
//        public string PhoneNumber { get; private set; }
//        public Task<BitmapImage> PhotoString { get; private set; }

//        public override string ToString()
//        {
//            return this.VisitorFullName;
//        }
//    }

//    public sealed class VisitorDataSource
//    {

//        private static VisitorDataSource _visitorDataSource = new VisitorDataSource();

//        private ObservableCollection<VisitorData> _visitorGroups = new ObservableCollection<VisitorData>();

//        public ObservableCollection<VisitorData> VisitorGroups
//        {
//            get { return this._visitorGroups; }
//        }

//        public static async Task<IEnumerable<VisitorData>> GetGroupsAsync()
//        {
//            await _visitorDataSource.GetVisitorDataAsync();

//            return _visitorDataSource.VisitorGroups;
//        }

//        public static async Task<VisitorData> GetGroupAsync(string visitorId)
//        {
//            await _visitorDataSource.GetVisitorDataAsync();
//            // Simple linear search is acceptable for small data sets
//            var matches = _visitorDataSource.VisitorGroups.Where((group) => group.VisitorId.Equals(visitorId));
//            if (matches.Count() == 1) return matches.First();
//            return null;
//        }

//        private async Task GetVisitorDataAsync()
//        {
//            if (this._visitorGroups.Count != 0)
//                return;

//            RemoteService service = new RemoteService();
//            VisitorDataPayLoad payload = new VisitorDataPayLoad();

//            var response = await service.VisitorListControllerService(payload);

//            foreach (var item in response.VisitorList)
//            {
//                VisitorData visitorDetail = new VisitorData(
//                    item.VisitorId.ToString(),
//                    item.VisitorFullName,
//                    item.CompanyName,
//                    item.emailAddress,
//                    item.phoneNumber 
//                );
//            //        ImageProcessor.Base64StringToBitmap(item.photoString)
               
//                this.VisitorGroups.Add(visitorDetail);
//            }

//        }
//    }

//}
