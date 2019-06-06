using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorApp.Common;
using VisitorApp.Models;
using VisitorApp.ViewModels.Commands;
using Windows.ApplicationModel;

namespace VisitorApp.ViewModels
{
    public class VisitorsList: ObservableCollection<Visitor>
    {
        public ParameterCommand parameterCommand { get; set; }

        public VisitorsList()
        {
            this.parameterCommand = new ParameterCommand(this);

            //public 
            if (DesignMode.DesignModeEnabled)
            {
                for (int i = 0; i <= 10; i++)
                {
                    Add(new Visitor()
                    {
                        VisitorId = "ID " + i,
                        VisitorFullName = "Person " + i,
                        CompanyName = "Lastname " + i,
                        EmailAddress = "@female " + i,
                         PhoneNumber= "@1234 " + i
                    });
                }
                Add(new Visitor()
                {
                    VisitorId = "ID",
                    VisitorFullName = "Person ",
                    CompanyName = "Lastname ",
                    EmailAddress = "@female",
                    PhoneNumber = "@1234"
                });
            }
        }

        //public async void GetVisitor(Visitor visitor)
        //{
        //    RemoteService service = new RemoteService();
        //    VisitorDataPayLoad payload = new VisitorDataPayLoad();

        //    var response = await service.VisitorListControllerService(payload);

        //    foreach (var item in response.VisitorList)
        //    {
        //        Add(new Visitor()
        //        {
        //            VisitorId = item.VisitorId.ToString(),
        //            VisitorFullName = item.VisitorFullName,
        //            CompanyName = item.CompanyName,
        //            EmailAddress=item.emailAddress,
        //            PhoneNumber=item.phoneNumber
        //        });
        //    }
        //}

        public void ParameterMethod(Visitor visitor)
        {
            Add(visitor);
        }
    }
}
