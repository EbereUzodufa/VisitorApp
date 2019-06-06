using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VisitorApp.Models;

namespace VisitorApp.ViewModels.Commands
{
    public class ParameterCommand : ICommand
    {
        public VisitorsList visitorList { get; set; }

        public ParameterCommand(VisitorsList viewList)
        {
            this.visitorList = viewList;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.visitorList.ParameterMethod(parameter as Visitor);
        }
    }
}
