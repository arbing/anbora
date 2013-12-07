using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Anbora.Home
{
    public class App : Application
    {
        public static bool bOnlyOneInstance = false;

        [STAThread]
        public static void Main()
        {
            Mutex mutex = new Mutex(true, "Anbora.Home", out bOnlyOneInstance);

            if (!bOnlyOneInstance)
            {
                return;
            }
            App app = new App();

            app.Startup += app_Startup;
            app.Run();

        }

        static void app_Startup(object sender, StartupEventArgs e)
        {
            App app = (App)sender;

            MainWindow mainWindow = new MainWindow();
            app.MainWindow = mainWindow;

            mainWindow.DataContext = new MainViewModel();
            mainWindow.Show();

            mainWindow.Closed += mainWindow_Closed;
        }
        static void mainWindow_Closed(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

    }
}
