using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorApp.Common;
using VisitorApp.Models;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;

namespace VisitorApp.ViewModels
{
    public class MainViewModel
    {
        public List<VisitorGlobal> VisitorList { get; set; }

        public List<GuestGlobal> guestList { get; set; }

        public List<DisplayDetails> DisplayList { get; set; }

        public MainViewModel()
        {
            #region My Test 1
            //VisitorList = new List<Visitors>();
            //for (int i = 0; i < 10; i++)
            //{
            //    VisitorList.Add(new Visitors
            //    {
            //        VisitorID = "Visitor ID " + i.ToString(),
            //        VisitorFullname = "Visitor Name " + i.ToString(),
            //        VisitorCompany = "Visitor Company " + i.ToString(),
            //        VisitorEmail = "Visitor Email " + i.ToString(),
            //        VisitorPhoneNumber = "Visitor PhoneNumber " + i.ToString()
            //    });
            //}
            #endregion

            #region Test Perfect?

            VisitorList = VisitorHelper(); //For Visitors
            guestList = GuestHelper();      //For Guests

            #region Testing Segment
            //TesterLists = new List<TestList>();
            //if (TesterLists.Count == 0)
            //{
            //    //To see what happens when Zero
            //    VisitorList = new List<Visitors>();
            //    for (int i = 0; i < 10; i++)
            //    {
            //        VisitorList.Add(new Visitors
            //        {
            //            VisitorID = "Visitor ID " + i.ToString(),
            //            VisitorFullname = "Visitor Name " + i.ToString(),
            //            VisitorCompany = "Visitor Company " + i.ToString(),
            //            VisitorEmail = "Visitor Email " + i.ToString(),
            //            VisitorPhoneNumber = "Visitor PhoneNumber " + i.ToString()
            //        });
            //    }
            //}
            //else
            //{
            //VisitorList = new List<Visitors>();
            //foreach (var item in TesterLists)
            //{
            //    VisitorList.Add(new Visitors
            //    {
            //        VisitorID = item.visitorID,
            //        VisitorFullname = item.visitorFullname,
            //        VisitorCompany = item.visitorCompany,
            //        VisitorEmail = item.visitorEmail,
            //        VisitorPhoneNumber = item.visitorPhoneNumber
            //    });
            //}
            //} 
            #endregion

            GetVisitors();

            #endregion
        }

        public List<VisitorGlobal> VisitorHelper()
        {
            try
            {
                RemoteService service = new RemoteService();
                VisitorDataPayLoad payload = new VisitorDataPayLoad();

                var response = service.VisitorListControllerService(payload);
                var visitorList = response.VisitorList;
                return visitorList;
                //var result = Newtonsoft.Json.JsonConvert.DeserializeObject<>

                #region Impo was couldn't run
                //VisitorList = new List<Visitors>();
                //foreach (var item in response.VisitorList)
                //{
                //    VisitorList.Add(new Visitors
                //    {
                //        VisitorID = item.VisitorId.ToString(),
                //        VisitorFullname = item.VisitorFullName.ToString(),
                //        VisitorCompany = item.CompanyName.ToString(),
                //        VisitorEmail = item.emailAddress.ToString(),
                //        VisitorPhoneNumber = item.phoneNumber.ToString()
                //    });
                //}


                //VisitorList = new List<Visitors>();
                //for (int i = 0; i < 10; i++)
                //{
                //    VisitorList.Add(new Visitors
                //    {
                //        VisitorID = "Visitor ID " + i.ToString(),
                //        VisitorFullname = "Visitor Name " + i.ToString(),
                //        VisitorCompany = "Visitor Company " + i.ToString(),
                //        VisitorEmail = "Visitor Email " + i.ToString(),
                //        VisitorPhoneNumber = "Visitor PhoneNumber " + i.ToString()
                //    });
                //} 
                #endregion

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message);
                msg.ShowAsync();
                return null;
            }
        }

        public List<GuestGlobal> GuestHelper()
        {
            try
            {
                RemoteService service = new RemoteService();
                VisitorDataPayLoad payload = new VisitorDataPayLoad();

                var response = service.GuestListControllerService(payload);
                var guestList = response.GuestList;
                return guestList;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message);
                msg.ShowAsync();
                return null;
            }
        }

        private void GetGuest()
        {
            #region Test for Guest - Successful
            DisplayList = new List<DisplayDetails>();
            foreach (var item in guestList)
            {
                BitmapImage PhotoCopy = null;
                string guestPhotoString = null;
                string guestPhoneNumber = null;
                string guestCompany = null;

                foreach (var visitor in VisitorList)
                {
                    if (item.VisitorId == visitor.VisitorId)
                    {
                        guestPhotoString = visitor.photoString;
                        guestCompany = visitor.CompanyName;
                        guestPhoneNumber = visitor.phoneNumber;
                    }
                }

                if (guestPhotoString == null)
                {
                    //Do nothing, no image
                }
                else
                {
                    byte[] Bytes = Convert.FromBase64String(guestPhotoString);

                    var stream = new InMemoryRandomAccessStream();
                    //var bytes = Convert.FromBase64String(source);
                    var dataWriter = new DataWriter(stream);
                    dataWriter.WriteBytes(Bytes);
                    dataWriter.StoreAsync();
                    stream.Seek(0);
                    var img = new BitmapImage();
                    img.SetSource(stream);
                    PhotoCopy = img;
                }

                DisplayList.Add(new DisplayDetails
                {
                    GuestId = item.GuestId.ToString(),
                    GuestName = item.GuestName,
                    GuestHostName = item.HostName,
                    GuestStatus = item.Status,
                    GuestCompany = guestCompany,
                    GuestPhoneNumber = guestPhoneNumber,
                    Picture = PhotoCopy
                });
            }
            #endregion
        }

        private void GetVisitors()
        {
            #region Test for Visitor - Successful
            DisplayList = new List<DisplayDetails>();
            foreach (var item in VisitorList)
            {
                BitmapImage PhotoCopy = null;

                if (item.photoString == null)
                {
                    //Do nothing, no image
                    PhotoCopy.UriSource = new Uri("Assets\no image found.jpeg");
                }
                else
                {
                    byte[] Bytes = Convert.FromBase64String(item.photoString);

                    var stream = new InMemoryRandomAccessStream();
                    //var bytes = Convert.FromBase64String(source);
                    var dataWriter = new DataWriter(stream);
                    dataWriter.WriteBytes(Bytes);
                    dataWriter.StoreAsync();
                    stream.Seek(0);
                    var img = new BitmapImage();
                    img.SetSource(stream);
                    //PhotoCopy = img;
                    PhotoCopy.UriSource = new Uri("Assets\no image found.jpeg");
                }

                DisplayList.Add(new DisplayDetails
                {
                    VisitorID = item.VisitorId.ToString(),
                    VisitorFullname = item.VisitorFullName,
                    VisitorCompany = item.CompanyName,
                    VisitorEmail = item.emailAddress,
                    VisitorPhoneNumber = item.phoneNumber,
                    Picture = PhotoCopy
                });
            }
            #endregion
        }
    }

    #region This Region hads the view on The UI
    public class viewModelTV
    {
        public List<VisitorGlobal> VisitorList { get; set; }

        public List<GuestGlobal> guestList { get; set; }

        public List<DisplayDetails> DisplayGuestList { get; set; }

        public List<DisplayDetails> DisplayVisitorList { get; set; }

        public viewModelTV()
        {
            try
            {
                VisitorList = new List<VisitorGlobal>();
                guestList = new List<GuestGlobal>();

                RemoteService service = new RemoteService();
                VisitorDataPayLoad payload = new VisitorDataPayLoad();

                var response = service.VisitorListControllerService(payload);
                VisitorList = response.VisitorList;

                var response2 = service.GuestListControllerService(payload);
                guestList = response2.GuestList;

                GetVisitors();

                GetGuest();
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message);
                msg.ShowAsync();
            }
        }

        #region Worrked Perfectly
        //public List<VisitorGlobal> VisitorHelper()
        //{
        //    try
        //    {
        //        RemoteService service = new RemoteService();
        //        VisitorDataPayLoad payload = new VisitorDataPayLoad();

        //        var response = service.VisitorListControllerService(payload);
        //        var visitorList = response.VisitorList;
        //        return visitorList;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageDialog msg = new MessageDialog(ex.Message);
        //        msg.ShowAsync();
        //        return null;
        //    }
        //}

        //public List<GuestGlobal> GuestHelper()
        //{
        //    try
        //    {
        //        RemoteService service = new RemoteService();
        //        VisitorDataPayLoad payload = new VisitorDataPayLoad();

        //        var response = service.GuestListControllerService(payload);
        //        var guestList = response.GuestList;
        //        return guestList;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageDialog msg = new MessageDialog(ex.Message);
        //        msg.ShowAsync();
        //        return null;
        //    }
        //} 
        #endregion

        private void GetGuest()
        {
            #region Test for Guest - Successful
            DisplayGuestList = new List<DisplayDetails>();
            foreach (var item in guestList)
            {
                BitmapImage PhotoCopy = null;
                string guestPhotoString = null;
                string guestPhoneNumber = null;
                string guestCompany = null;
                string guestEmail = null;

                foreach (var visitor in VisitorList)
                {
                    if (item.VisitorId == visitor.VisitorId)
                    {
                        guestPhotoString = visitor.photoString;
                        guestCompany = visitor.CompanyName;
                        guestPhoneNumber = visitor.phoneNumber;
                        guestEmail = visitor.emailAddress;
                    }
                }

                if (guestPhotoString == null)
                {
                    //Do nothing, no image
                }
                else
                {
                    byte[] Bytes = Convert.FromBase64String(guestPhotoString);

                    var stream = new InMemoryRandomAccessStream();
                    //var bytes = Convert.FromBase64String(source);
                    var dataWriter = new DataWriter(stream);
                    dataWriter.WriteBytes(Bytes);
                    dataWriter.StoreAsync();
                    stream.Seek(0);
                    var img = new BitmapImage();
                    img.SetSource(stream);
                    PhotoCopy = img;
                }

                DisplayGuestList.Add(new DisplayDetails
                {
                    GuestFullName = item.GuestName,
                    GuestHostName = item.HostName,
                    GuestStatus = item.Status,
                    GuestCompany = guestCompany,
                    GuestEmail = guestEmail,
                    GuestPhoneNumber = guestPhoneNumber,
                    GuestCheckInTime = item.CheckInTime,
                    GuestCheckOutTime = item.CheckOutTime,
                    Picture = PhotoCopy
                });
            }
            #endregion
        }

        private void GetVisitors()
        {
            #region Test for Visitor - Successful
            DisplayVisitorList = new List<DisplayDetails>();
            foreach (var item in VisitorList)
            {
                BitmapImage PhotoCopy = null;

                if (item.photoString == null)
                {
                    //Do nothing, no image
                }
                else
                {
                    byte[] Bytes = Convert.FromBase64String(item.photoString);

                    var stream = new InMemoryRandomAccessStream();
                    //var bytes = Convert.FromBase64String(source);
                    var dataWriter = new DataWriter(stream);
                    dataWriter.WriteBytes(Bytes);
                    dataWriter.StoreAsync();
                    stream.Seek(0);
                    var img = new BitmapImage();
                    img.SetSource(stream);
                    //PhotoCopy = img;
                    PhotoCopy = img;
                }

                DisplayVisitorList.Add(new DisplayDetails
                {
                    //VisitorID = item.VisitorId.ToString(),
                    //VisitorFullname = item.VisitorFullName,
                    //VisitorCompany = item.CompanyName,
                    //VisitorEmail = item.emailAddress,
                    //VisitorPhoneNumber = item.phoneNumber,
                    Picture = PhotoCopy
                });
            }
            #endregion
        }

    }
    #endregion
}
