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
//    public class Tester
//    {
//        public int GuestId { get; set; }
//        public string GuestName { get; set; }
//        public string HostName { get; set; }
//        public string ImagePath { get; set; }
//    }

//    public class TestManagers
//    {
//        public async static List<Tester> GetGuest()
//        {
//            var guest = new List<Tester>();

//            RemoteService service = new RemoteService();
//            VisitorDataPayLoad payload = new VisitorDataPayLoad();

//            var response = await service.GuestListControllerService(payload);

//            foreach (var item in response.GuestList)
//            {
//                guest.Add(new Tester
//                {
//                    GuestId = item.GuestId,
//                    GuestName = item.GuestName,
//                    HostName = item.HostName,
//                    ImagePath = ""
//                });

//                return guest;
//            }
//        }

//    }
//}
