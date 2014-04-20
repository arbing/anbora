using System;

namespace ConvertEoL
{
    using System.IO;

    public static class FileHelper
    {
        static readonly byte[] Cr = { 0x0D };

        static readonly byte[] Lf = { 0x0A };

        static readonly byte[] CrLf = { 0x0D, 0x0A };

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

        public static int CheckWindowsEoL(string inPath)
        {
            try
            {
                using (var fsSource = File.Open(inPath, FileMode.Open, FileAccess.Read))
                {
                    var bytes = new byte[fsSource.Length];
                    fsSource.Read(bytes, 0, (int)fsSource.Length);

                    // 查找Windows行尾符<CR><LF>以外的换行符，即独立字符<LF>、<CR>
                    var temp = bytes.Replace(CrLf, new byte[] { });
                    var result = temp.Search(Lf) + temp.Search(Cr);

                    return result;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static int CheckUnixEoL(string inPath)
        {
            try
            {
                using (var fsSource = File.Open(inPath, FileMode.Open, FileAccess.Read))
                {
                    var bytes = new byte[fsSource.Length];
                    fsSource.Read(bytes, 0, (int)fsSource.Length);

                    // 查找Unix行尾符<LF>以外的换行符，即<CR><LF>、<CR>
                    var crlfCount = bytes.Search(CrLf);
                    var temp = bytes.Replace(CrLf, new byte[] { });
                    var crCount = temp.Search(Cr);

                    return crlfCount + crCount;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static int CheckMacintoshEoL(string inPath)
        {
            try
            {
                using (var fsSource = File.Open(inPath, FileMode.Open, FileAccess.Read))
                {
                    var bytes = new byte[fsSource.Length];
                    fsSource.Read(bytes, 0, (int)fsSource.Length);

                    // 查找Macintosh行尾符<CR>以外的换行符，即<CR><LF>、<LF>
                    var crlfCount = bytes.Search(CrLf);
                    var temp = bytes.Replace(CrLf, new byte[] { });
                    var lfCount = temp.Search(Lf);

                    return crlfCount + lfCount;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }


        public static bool ConvertToWindowsEoL(string inPath, string outPath)
        {
            try
            {
                using (var fsSource = File.Open(inPath, FileMode.Open, FileAccess.Read))
                {
                    var bytes = new byte[fsSource.Length];
                    fsSource.Read(bytes, 0, (int)fsSource.Length);

                    // 查找Windows行尾符<CR><LF>以外的换行符，即独立字符<LF>、<CR>
                    var temp = bytes.Replace(CrLf, new byte[] { });
                    var result = temp.Search(Lf) + temp.Search(Cr);

                    // 没有独立字符<LF>、<CR>时，不替换
                    if (result <= 0)
                    {
                        return false;
                    }

                    // 字符替换：<LF>、<CR> → <CR><LF>
                    var outBytes = bytes.Replace(CrLf, Lf);
                    outBytes = outBytes.Replace(Cr, Lf);
                    outBytes = outBytes.Replace(Lf, CrLf);

                    // 创建输出目录
                    CheckDirectory(outPath);

                    using (var fsOut = new FileStream(outPath, FileMode.Create, FileAccess.Write))
                    {
                        fsOut.Write(outBytes, 0, outBytes.Length);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ConvertToUnix(string inPath, string outPath)
        {
            try
            {
                using (var fsSource = File.Open(inPath, FileMode.Open, FileAccess.Read))
                {
                    var bytes = new byte[fsSource.Length];
                    fsSource.Read(bytes, 0, (int)fsSource.Length);

                    // 查找Unix行尾符<LF>以外的换行符，即<CR><LF>、<CR>
                    var crlfCount = bytes.Search(CrLf);
                    var temp = bytes.Replace(CrLf, new byte[] { });
                    var crCount = temp.Search(Cr);

                    // 未找到字符<CR><LF>或者没有独立字符<CR>时，不替换
                    if (crlfCount + crCount <= 0)
                    {
                        return false;
                    }

                    // 字符替换<CR><LF>、<CR> → <LF>
                    var outBytes = bytes.Replace(CrLf, Lf);
                    outBytes = outBytes.Replace(Cr, Lf);

                    // 创建输出目录
                    CheckDirectory(outPath);

                    using (var fsOut = new FileStream(outPath, FileMode.Create, FileAccess.Write))
                    {
                        fsOut.Write(outBytes, 0, outBytes.Length);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static bool ConvertToMacintosh(string inPath, string outPath)
        {
            try
            {
                using (var fsSource = File.Open(inPath, FileMode.Open, FileAccess.Read))
                {
                    var bytes = new byte[fsSource.Length];
                    fsSource.Read(bytes, 0, (int)fsSource.Length);

                    // 查找Macintosh行尾符<CR>以外的换行符，即<CR><LF>、<LF>
                    var crlfCount = bytes.Search(CrLf);
                    var temp = bytes.Replace(CrLf, new byte[] { });
                    var lfCount = temp.Search(Lf);

                    // 未找到字符<CR><LF>或者没有独立字符<LF>时，不替换
                    if (crlfCount + lfCount <= 0)
                    {
                        return false;
                    }

                    // 字符替换<CR><LF>、<LF> → <CR>
                    var outBytes = bytes.Replace(CrLf, Cr);
                    outBytes = outBytes.Replace(Lf, Cr);

                    // 创建输出目录
                    CheckDirectory(outPath);

                    using (var fsOut = new FileStream(outPath, FileMode.Create, FileAccess.Write))
                    {
                        fsOut.Write(outBytes, 0, outBytes.Length);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
