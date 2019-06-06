using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using VisitorApp.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VisitorApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class sample : Page
    {
        public List<GuestGlobal> Guests;
        public sample()
        {
            this.InitializeComponent();

            Guests = new List<GuestGlobal>
            {
                new GuestGlobal
                {
                     GuestName = "Donald Odiachi",
                      HostName =  "Ebere Uzodufa"
                },
                 new GuestGlobal
                {
                     GuestName = "Donald Odiachi",
                      HostName =  "Ebere Uzodufa"
                },
                  new GuestGlobal
                {
                     GuestName = "Donald Odiachi",
                      HostName =  "Ebere Uzodufa"
                },
                   new GuestGlobal
                {
                     GuestName = "Donald Odiachi",
                      HostName =  "Ebere Uzodufa"
                },
                    new GuestGlobal
                {
                     GuestName = "Donald Odiachi",
                      HostName =  "Ebere Uzodufa"
                },
            };

           /* RemoteService service = new RemoteService();
            VisitorDataPayLoad payload = new VisitorDataPayLoad();

            var response = service.VisitorListControllerService(payload).Result;
            Guests = response.GuestList;*/
        }
    }
}
