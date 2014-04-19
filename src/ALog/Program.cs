using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALog
{
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                    return;

                var path = "log.txt";
                string content;

                if (args.Length == 1)
                {
                    content = args[0];
                }
                else
                {
                    path = args[0];
                    content = args[1];
                }

                CheckDirectory(path);

                Log(path, content);

                //using (StreamWriter w = File.AppendText(path))
                //{
                //    Log(w, content);
                //}
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void Log(string path, string content)
        {
            var log = string.Format("{0}  {1} \r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), content);

            using (FileStream fs = File.Open(path, FileMode.Append, FileAccess.Write))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(log);
                fs.Write(info, 0, info.Length);
            }
        }

        public static void Log(TextWriter w, string content)
        {
            var log = string.Format("{0}  {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), content);
            w.WriteLine(log);
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
