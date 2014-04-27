using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace SolutionGenerater
{
    using System.IO;
    using System.Windows;

    using Path = System.IO.Path;

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        class Item
        {
            public int Type
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }

            public string FullName
            {
                get;
                set;
            }

            public string Parent
            {
                get;
                set;
            }

            public Guid Guid
            {
                get;
                set;
            }

            public override string ToString()
            {
                return FullName;
            }
        }

        private static Guid dirTypeGuid = Guid.Parse("{2150E333-8FDC-42A3-9474-1A3956D46DE8}");
        private static Guid projTypeGuid = Guid.Parse("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}");

        public MainWindow()
        {
            InitializeComponent();
        }

        private static string GetRelativePath(string fullPath, string sourcePath)
        {
            return fullPath.Length == sourcePath.Length ? string.Empty : fullPath.Substring(sourcePath.Length + 1);
        }

        private static IEnumerable<Item> GenerateItems(string path)
        {
            var directories = Directory.EnumerateDirectories(path, "? *", SearchOption.AllDirectories);

            var dirs = from directory in directories
                       let parent = Path.GetDirectoryName(directory)
                       let name = GetRelativePath(directory, parent)
                       select new Item()
                                  {
                                      Guid = Guid.NewGuid(),
                                      Type = 0,
                                      Name = name,
                                      Parent = GetRelativePath(parent, path),
                                      FullName = GetRelativePath(directory, path)
                                  };

            var files = Directory.EnumerateFiles(path, "*.csproj", SearchOption.AllDirectories);

            var projs = from file in files
                        let parent = Path.GetDirectoryName(Path.GetDirectoryName(file))
                        let name = Path.GetFileNameWithoutExtension(file)
                        select new Item()
                                   {
                                       Guid = Guid.NewGuid(),
                                       Type = 1,
                                       Name = name,
                                       Parent = GetRelativePath(parent, path),
                                       FullName = GetRelativePath(file, path)
                                   };

            return dirs.Union(projs).OrderBy(p => p.FullName);
        }

        private static IEnumerable<string> GenerateProjectItems(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                if (item.Type == 0)
                {
                    yield return string.Format(
                        "Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"\nEndProject\n"
                        , dirTypeGuid.ToString("B").ToUpper()
                        , item.Name
                        , item.Name
                        , item.Guid.ToString("B").ToUpper());
                }
                else if (item.Type == 1)
                {
                    yield return string.Format(
                        "Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"\nEndProject\n"
                        , projTypeGuid.ToString("B").ToUpper()
                        , item.Name
                        , item.FullName
                        , item.Guid.ToString("B").ToUpper());
                }
            }
        }

        private static IEnumerable<string> GenerateProjectConfigs(IEnumerable<Item> items)
        {
            return from item in items
                   where item.Type == 1
                   select string.Format(
                              "\t\t{0}.Debug|Any CPU.ActiveCfg = Debug|Any CPU\n"
                            + "\t\t{0}.Debug|Any CPU.Build.0 = Debug|Any CPU\n"
                            + "\t\t{0}.Debug|x64.ActiveCfg = Debug|x64\n"
                            + "\t\t{0}.Debug|x64.Build.0 = Debug|x64\n"
                            + "\t\t{0}.Release|Any CPU.ActiveCfg = Release|Any CPU\n"
                            + "\t\t{0}.Release|Any CPU.Build.0 = Release|Any CPU\n"
                            + "\t\t{0}.Release|x64.ActiveCfg = Release|x64\n"
                            + "\t\t{0}.Release|x64.Build.0 = Release|x64\n"
                        , item.Guid.ToString("B").ToUpper());
        }

        private static IEnumerable<string> GenerateProjectNesteds(IEnumerable<Item> items)
        {
            return from item in items
                   let parent = items.FirstOrDefault(p => p.FullName == item.Parent)
                   where !string.IsNullOrEmpty(item.Parent)
                   where parent != default(Item)
                   select string.Format(
                           "\t\t{0} = {1}\n",
                           item.Guid.ToString("B").ToUpper(),
                           parent.Guid.ToString("B").ToUpper());
        }

        private void BtnRun_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentDirectory = Directory.GetCurrentDirectory();

                var inputDir = Path.Combine(this.TbInput.Text, string.Empty);

                if (!Directory.Exists(inputDir))
                {
                    MessageBox.Show("Directory does not exist!");
                    return;
                }

                if (!this.TbOutput.Text.EndsWith(".sln",StringComparison.CurrentCultureIgnoreCase))
                {
                    this.TbOutput.Text = string.Format("{0}.sln", GetRelativePath(inputDir, Path.GetDirectoryName(inputDir)));
                }

                var outputDir = inputDir;

                var slnName = this.TbOutput.Text;
                var slnPath = Path.Combine(outputDir, slnName);

                var slnTplPath = Directory.EnumerateFiles(currentDirectory,"*.tpl").FirstOrDefault();

                // 读入.csproj
                var items = GenerateItems(inputDir).ToList();

                if (File.Exists(slnPath))
                {
                    File.Move(slnPath, Path.Combine(outputDir, string.Format("{0}.{1}", slnPath, DateTime.Now.ToString("yyyyMMddHHmmss"))));
                }

                // 输出.sln
                var content = new StringBuilder();

                if (string.IsNullOrEmpty(slnTplPath) || !File.Exists(slnTplPath))
                {
                    MessageBox.Show("The solution template file does not exist!");
                    return;
                }

                using (var sr = new StreamReader(slnTplPath, Encoding.UTF8))
                {
                    content.Append(sr.ReadToEnd());
                    sr.Close();
                }

                // Projects
                var projectsb = new StringBuilder();
                var projects = GenerateProjectItems(items);
                foreach (var p in projects)
                {
                    projectsb.Append(p);
                }

                content.Replace(@"${Projects}", projectsb.ToString());

                // ProjectConfigurationPlatforms
                var projectConfigsb = new StringBuilder();
                var projectConfigs = GenerateProjectConfigs(items);
                foreach (var p in projectConfigs)
                {
                    projectConfigsb.Append(p);
                }

                content.Replace(@"${ProjectConfigurationPlatforms}", projectConfigsb.ToString());

                // NestedProjects
                var projectNestedsb = new StringBuilder();
                var projectNesteds = GenerateProjectNesteds(items);
                foreach (var p in projectNesteds)
                {
                    projectNestedsb.Append(p);
                }

                content.Replace(@"${NestedProjects}", projectNestedsb.ToString());

                using (var outfile = new StreamWriter(slnPath, true, Encoding.UTF8))
                {
                    outfile.Write(content.ToString());
                }

                MessageBox.Show("Process completed successfully!");
            }
            catch (Exception)
            {
                MessageBox.Show("There was an error!");
            }
        }

        private void Input_OnDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            this.TbInput.Text = path;
            this.TbOutput.Text = string.Format("{0}.sln", GetRelativePath(path, Path.GetDirectoryName(path)));
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
