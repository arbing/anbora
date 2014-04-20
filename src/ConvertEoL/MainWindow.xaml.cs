namespace ConvertEoL
{
    using System.IO;
    using System;
    using System.Windows;
    using System.Windows.Documents;
    using Path = System.IO.Path;

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        //  标志  符号	    十进制Asscii码数     十六进制数                              
        //  CR      /r      13                          0x0D
        //  LF      /n      10                          0x0A

        //  标志  Win/Dos         linux，Unix等     Macintosh
        //  换行  /r/n(<CR><LF>)  /n(<LF>)        /r(<CR>

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnCheck_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Directory.Exists(this.TbInput.Text))
                {
                    MessageBox.Show("Directory does not exist!");
                    return;
                }

                var inputDir = Path.Combine(this.TbInput.Text, string.Empty);
                var pattern = this.TbPattern.Text;
                var convertMode = this.CmbConvertMode.SelectedIndex;

                // 清空Log
                this.FdLog.Blocks.Clear();
                this.FdLog.Blocks.Add(new Paragraph(new Run("The following files contain different line breaks:")));

                var files = Directory.EnumerateFiles(inputDir, pattern, SearchOption.AllDirectories);

                foreach (var f in files)
                {
                    var input = f;
                    var relName = input.Substring(inputDir.Length + 1);

                    int result = 0;
                    switch (convertMode)
                    {
                    case 0:
                        result = FileHelper.CheckWindowsEoL(input);
                        break;
                    case 1:
                        result = FileHelper.CheckUnixEoL(input);
                        break;
                    case 2:
                        result = FileHelper.CheckMacintoshEoL(input);
                        break;
                    }

                    if (result > 0)
                    {
                        var log = relName + ":" + result;
                        this.FdLog.Blocks.Add(new Paragraph(new Run(log)));
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("There was an error!");
            }
        }

        private void BtnRun_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Directory.Exists(this.TbInput.Text))
                {
                    MessageBox.Show("Directory does not exist!");
                    return;
                }

                var inputDir = Path.Combine(this.TbInput.Text, string.Empty);
                var outputDir = this.TbOutput.Text;
                var pattern = this.TbPattern.Text;
                var convertMode = this.CmbConvertMode.SelectedIndex;

                // 删除输出目录
                if (Directory.Exists(outputDir))
                {
                    Directory.Delete(outputDir, true);
                }

                // 清空Log
                this.FdLog.Blocks.Clear();
                this.FdLog.Blocks.Add(new Paragraph(new Run("The following files have been changed:")));

                var files = Directory.EnumerateFiles(inputDir, pattern, SearchOption.AllDirectories);

                foreach (var f in files)
                {
                    var input = f;
                    var relName = input.Substring(inputDir.Length + 1);
                    var output = System.IO.Path.Combine(outputDir, relName);

                    bool result = false;
                    switch (convertMode)
                    {
                    case 0:
                        result = FileHelper.ConvertToWindowsEoL(input, output);
                        break;
                    case 1:
                        result = FileHelper.ConvertToUnix(input, output);
                        break;
                    case 2:
                        result = FileHelper.ConvertToMacintosh(input, output);
                        break;
                    }

                    if (result)
                    {
                        var log = relName;
                        this.FdLog.Blocks.Add(new Paragraph(new Run(log)));
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("There was an error!");
            }
        }
        private void Input_OnDrop(object sender, DragEventArgs e)
        {
            string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            this.TbInput.Text = fileName;
        }

        private void Output_OnDrop(object sender, DragEventArgs e)
        {
            string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            this.TbOutput.Text = fileName;
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
    }
}
