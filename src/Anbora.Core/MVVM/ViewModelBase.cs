using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Anbora.Core.MVVM
{
    public class ViewModelBase : NotificationObject
    {
        private CommandMap _Commands = new CommandMap();
        /// <summary>
        /// Commands
        /// </summary>
        public CommandMap Commands
        {
            get
            {
                return _Commands;
            }
            set
            {
                Commands = value;
                RaisePropertyChanged(() => Commands);
            }
        }
    }
}
