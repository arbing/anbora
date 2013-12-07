using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetFrameworkReplume
{
    class NetFrameworkReplume
    {
        static string OrginFolderPath = @"f:\.NET Framework\RefSrc\Source\";
        static string TargetFolderPath = @"F:\.NET Framework\Source\";
        static int flag = 1;

        static void Main(string[] args)
        {
            DirectoryInfo donetFrameworkDirectry = new DirectoryInfo(OrginFolderPath);
            ReCollectionFrameworkFiles(donetFrameworkDirectry);
            Console.WriteLine("End of collection~~~~~");
            Console.ReadLine();
        }
        static void ReCollectionFrameworkFiles(DirectoryInfo directory)
        {
            foreach (FileSystemInfo f in directory.GetFileSystemInfos())
            {
                if (f is FileInfo)
                {
                    string newPath = RemoveFolderByNameSpace((FileInfo)f);
                    //Console.WriteLine(newPath);
                    if (!string.IsNullOrEmpty(newPath))
                    {
                        string foldername = TargetFolderPath + newPath;
                        if (!Directory.Exists(foldername))
                        {
                            Directory.CreateDirectory(foldername);
                        }
                        string fileFullName = foldername + f.Name;
                        if (File.Exists(fileFullName))
                        {
                            fileFullName = Path.ChangeExtension(fileFullName, "cs" + flag.ToString());
                            //show conflict files
                            Console.WriteLine("Conflict orgin:" + f.FullName + "\t targer:" + fileFullName);
                            flag++;
                        }
                        File.Copy(f.FullName, fileFullName);
                        // Console.WriteLine(f.FullName);
                    }
                    else
                    {
                        //not find the namespace
                        //  Console.WriteLine(f.FullName);
                        File.AppendAllText(@"F:\log.txt", f.FullName + "\r\n");
                    }
                }
                else
                {
                    ReCollectionFrameworkFiles((DirectoryInfo)f);
                }
            }
        }
        static string RemoveFolderByNameSpace(FileInfo file)
        {
            if (file.Name.EndsWith(".cs"))
            {
                string codeInfo = file.OpenText().ReadToEnd();
                Regex reg = new Regex(@"^[ ]*namespace[ ]+(?<namespace>[\w\.]+)", RegexOptions.Multiline);
                var temp = reg.Match(codeInfo).Groups;
                //Console.WriteLine(temp["namespace"]);
                if (temp != null && temp.Count > 1)
                {
                    string strnamespace = temp[1].Value;
                    return GenerationFolderPath(strnamespace);
                }
            }
            return null;
        }
        static string GenerationFolderPath(string strnamespace)
        {
            string[] strs = strnamespace.Split(new char[] { '.' });
            string folder = string.Empty;
            foreach (var s in strs)
            {
                folder += s + @"\";
            }
            return folder;
        }
    }
}
