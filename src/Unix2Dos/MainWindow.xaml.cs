namespace Unix2Dos
{
    using System.IO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    using Path = System.IO.Path;

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnRun_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Directory.Exists(this.TbInput.Text))
                {
                    System.Windows.MessageBox.Show("Directory does not exist!");
                    return;
                }
                var inputDir = Path.Combine(this.TbInput.Text, string.Empty);
                var outputDir = this.TbOutput.Text;
                var pattern = this.TbPattern.Text;
                var convertMode = this.RbtnConvertMode.IsChecked ?? true;

                // 删除输出目录
                if (Directory.Exists(outputDir))
                {
                    Directory.Delete(outputDir, true);
                }

                // 清空Log
                var logs = new List<string>();
                this.LbLog.ItemsSource = logs;

                //  标志  符号	    十进制Asscii码数     十六进制数                              
                //  CR      /r      13                          0x0D
                //  LF      /n      10                          0x0A
                //  标志          Win/Dos                 linux，Unix等
                //  换行          /r/n(<CR><LF>)      /n(<LF>)   

                var files = Directory.EnumerateFiles(inputDir, pattern, SearchOption.AllDirectories);

                foreach (var f in files)
                {
                    var input = f;
                    var relName = input.Substring(inputDir.Length + 1);
                    var output = System.IO.Path.Combine(outputDir, relName);

                    // 创建输出目录
                    CheckDirectory(output);

                    bool result = convertMode ? Unix2Dos(input, output) : Dos2Unix(input, output);

                    logs.Add(relName + (result ? " conversion success!" : " conversion failed!"));
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("There was an error!");
                return;
            }
        }

        public static bool Unix2Dos(string input, string output)
        {
            try
            {
                using (FileStream fsSource = File.Open(input, FileMode.Open, FileAccess.Read))
                {
                    // Read the source file into a byte array.
                    byte[] bytes = new byte[fsSource.Length];
                    int numBytesToRead = (int)fsSource.Length;
                    int numBytesRead = 0;
                    while (numBytesToRead > 0)
                    {
                        // Read may return anything from 0 to numBytesToRead.
                        int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

                        // Break when the end of the file is reached.
                        if (n == 0)
                            break;

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }
                    numBytesToRead = bytes.Length;

                    // Write the byte array to the other FileStream.
                    using (FileStream fsNew = new FileStream(output,
                        FileMode.Create, FileAccess.Write))
                    {
                        foreach (var b in bytes)
                        {
                            if (b == 0x0A)
                            {
                                fsNew.WriteByte(0x0D);
                            }
                            fsNew.WriteByte(b);
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool Dos2Unix(string input, string output)
        {
            try
            {
                using (FileStream fsSource = File.Open(input, FileMode.Open, FileAccess.Read))
                {
                    // Read the source file into a byte array.
                    byte[] bytes = new byte[fsSource.Length];
                    int numBytesToRead = (int)fsSource.Length;
                    int numBytesRead = 0;
                    while (numBytesToRead > 0)
                    {
                        // Read may return anything from 0 to numBytesToRead.
                        int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

                        // Break when the end of the file is reached.
                        if (n == 0)
                            break;

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }
                    numBytesToRead = bytes.Length;

                    // Write the byte array to the other FileStream.
                    using (FileStream fsNew = new FileStream(output,
                        FileMode.Create, FileAccess.Write))
                    {
                        // No CR encountered yet
                        var crFlag = false;

                        foreach (var b in bytes)
                        {
                            if (crFlag && b != 0x0A)
                            {
                                // This CR did not preceed LF 
                                fsNew.WriteByte(0x0D);
                            }

                            if (!(crFlag = b == 0x0D))
                            {
                                fsNew.WriteByte(b);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static void CheckDirectory(string path)
        {
            if (path.LastIndexOf('\\') > 0)
            {
                var dir = path.Substring(0, path.LastIndexOf('\\'));

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
        }

    }
}
