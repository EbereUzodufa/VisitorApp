using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using VisitorApp.Common;
using Windows.Storage.Streams;
using VisitorApp.Models;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VisitorApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class helperTV : Page
    {
       // public List<VisitorGlobal> VisitorList { get; set; }

        //public List<GuestGlobal> guestList { get; set; }

        public List<DisplayDetails> DisplayList { get; set; }


        public helperTV()
        {
            try
            {
                this.InitializeComponent();

                //gridviewData.DataContext = this;

                DisplayList = new List<DisplayDetails>();
                for (int i = 0; i < 10; i++)
                {
                    DisplayList.Add(new DisplayDetails
                    {
                        VisitorFullname = "VisitorID " + i.ToString(),
                        VisitorID = i.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message);
                msg.ShowAsync();
            }
        }

        private void btnshowVisitors_Click(object sender, RoutedEventArgs e)
        {
            
        }


        //public List<VisitorGlobal> VisitorHelper()
        //{
        //    try
        //    {
        //        RemoteService service = new RemoteService();
        //        VisitorDataPayLoad payload = new VisitorDataPayLoad();

        //        var response = service.VisitorListControllerService(payload);
        //        var visitorList = response.VisitorList;
        //        return visitorList;
        //        //var result = Newtonsoft.Json.JsonConvert.DeserializeObject<>
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageDialog msg = new MessageDialog(ex.Message);
        //        msg.ShowAsync();
        //        return null;
        //    }
        //}

    }
}
