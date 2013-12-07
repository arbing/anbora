using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Anbora.Core.Helper;
using Anbora.Core.MVVM;

namespace Anbora.ViewModel
{

    public class MenuViewModel : ViewModelBase
    {
        HttpClient client = new HttpClient();

        private Dictionary<string, string> _Menu01;
        /// <summary>
        /// Menu01
        /// </summary>
        public Dictionary<string, string> Menu01
        {
            get
            {
                return _Menu01;
            }
            set
            {
                _Menu01 = value;
                RaisePropertyChanged("Menu01");
            }
        }

        public MenuViewModel()
        {
            // 添加命令
            Commands.AddCommand("InitCommand", InitExecute);
            // 添加命令
            Commands.AddCommand("FnCommand", FnExecute);

            if (DesignHelper.IsInDesignMode)
            {
                InitExecute(null);
            }

            Uri _baseAddress = new Uri("http://localhost:1990/");

            client.BaseAddress = _baseAddress;

        }

        /// <summary>
        /// Command处理方法
        /// </summary>
        /// <param name="param"></param>
        private void InitExecute(object param)
        {
            _Menu01 = new Dictionary<string, string>();

            _Menu01["01"] = "中国";
            _Menu01["02"] = "美国";
            _Menu01["03"] = "日本";
            _Menu01["04"] = "韩国";
        }

        /// <summary>
        /// Command处理方法
        /// </summary>
        /// <param name="param"></param>
        private void FnExecute(object param)
        {
            switch (param.ToString())
            {
                case "F01":
                    global::System.Windows.MessageBox.Show("F01");
                    break;
                case "F02":
                    try
                    {
                        string view = "MenuView";
                        client.PostAsJsonAsync("api/Transfer", view);

                    }
                    catch (Exception e)
                    {
                        throw;
                    }

                    break;

                default:
                    break;
            }
        }
    }
}
