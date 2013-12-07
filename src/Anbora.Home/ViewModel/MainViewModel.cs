using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.SelfHost;
using System.Windows.Controls;
using Anbora.Core.MVVM;
using Anbora.Core.Net;
using Anbora.View;
using Anbora.ViewModel;

namespace Anbora.Home
{
    class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            CurrentView = new MenuView();
            CurrentViewModel = new MenuViewModel();


            Thread aThread = new Thread(new ThreadStart(SetupServer));
            aThread.Start();
        }

        private string _Title;
        /// <summary>
        /// Title
        /// </summary>
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        private UserControl _CurrentView;
        /// <summary>
        /// CurrentView
        /// </summary>
        public UserControl CurrentView
        {
            get
            {
                return _CurrentView;
            }
            set
            {
                _CurrentView = value;
                RaisePropertyChanged("CurrentView");
            }
        }

        private ViewModelBase _CurrentViewModel;
        /// <summary>
        /// CurrentViewModel
        /// </summary>
        public ViewModelBase CurrentViewModel
        {
            get
            {
                return _CurrentViewModel;
            }
            set
            {
                _CurrentViewModel = value;
                RaisePropertyChanged("CurrentViewModel");
            }
        }

        private void SetupServer()
        {
            Uri _baseAddress = new Uri("http://localhost:1990/");
            HttpSelfHostServer server = null;

            try
            {
                // Set up server configuration
                HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(_baseAddress);

                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new
                    {
                        id = RouteParameter.Optional
                    }
                );

                config.Services.Replace(typeof(IAssembliesResolver), new SelfAssembliesResolver());

                // Create server
                server = new HttpSelfHostServer(config);

                // Start listening
                server.OpenAsync();

                while (true)
                {
                    Queue<string> ViewQueue;
                    if (!server.Configuration.Properties.ContainsKey("ViewQueue"))
                    {
                        ViewQueue = new Queue<string>();
                        server.Configuration.Properties["ViewQueue"] = ViewQueue;
                    }
                    else
                    {
                        ViewQueue = server.Configuration.Properties["ViewQueue"] as Queue<string>;
                    }

                    if (ViewQueue != null && ViewQueue.Count > 0)
                    {
                        string viewName = ViewQueue.Dequeue();
                        Title = Title + viewName;
                    }

                    Thread.Sleep(100);
                }

            }
            catch (Exception e)
            {
                if (server != null)
                {
                    // Stop listening
                    server.CloseAsync().Wait();
                }
            }
            finally
            {
            }
        }
    }
}
