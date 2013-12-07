using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Anbora.Controller
{
    public class TransferController : ApiController
    {
        public string PostView([FromBody]string param)
        {
            Queue<string> ViewQueue = new Queue<string>();

            ViewQueue = this.Configuration.Properties["ViewQueue"] as Queue<string>;
            if (ViewQueue != null)
            {
                ViewQueue.Enqueue(param);
            }

            return param;
        }
    }
}
