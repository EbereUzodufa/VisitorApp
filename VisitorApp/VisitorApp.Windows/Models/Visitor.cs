using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorApp.Common;
using Windows.UI.Popups;

namespace VisitorApp.Models
{
    public class Visitor
    {
        private string visitorId;
        private string visitorFullname;
        private string companyName;
        private string emailAddress;
        private string phoneNumber;

        public string VisitorId {
            get { return visitorId; }
            set
            {
                visitorId = value;
            }
        }

        public string VisitorFullName
        {
            get { return visitorFullname; }
            set
            {
                visitorFullname = value;
            }
        }

        public string CompanyName
        {
            get { return companyName; }
            set
            {
                companyName = value;
            }
        }

        public string EmailAddress
        {
            get { return emailAddress; }
            set
            {
                emailAddress = value;
            }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set
            {
                phoneNumber = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void tester()
        {
            try
            {
                //this.Frame.Navigate(typeof(PhotoUpdatePage));

                //This helps get user detail while data is filled out.
                RemoteService service = new RemoteService();
                VisitorDataPayLoad payload = new VisitorDataPayLoad();

                var response = await service.VisitorListControllerService(payload);

                foreach (var item in response.VisitorList)
                {
                    visitorId = item.VisitorId.ToString();
                    visitorFullname = item.VisitorFullName;
                    companyName = item.CompanyName;
                    emailAddress = item.emailAddress;
                    phoneNumber = item.phoneNumber;
                }

            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message);
                await msg.ShowAsync();
            }
        }
    }
}
