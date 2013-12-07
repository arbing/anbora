using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.SelfHost;

namespace Anbora.Core.Net
{
    public class SelfAssembliesResolver : IAssembliesResolver
    {
        ICollection<Assembly> Controllers = new Collection<Assembly>();

        public SelfAssembliesResolver()
        {
        }

        public ICollection<Assembly> GetAssemblies()
        {
            var fold = @"F:\Projects\Anbora\output\Controller";
            //System.Environment.CurrentDirectory;

            foreach (var item in Directory.GetFiles(fold, "*.dll"))
            {
                var assembly = Assembly.LoadFrom(item);
                if (assembly != null)
                {
                    Controllers.Add(assembly);
                }
            }
            return Controllers;
        }
    }
}
