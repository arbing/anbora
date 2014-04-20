using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplaceBytes
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] all = { 0x0A, 0x0D };

            byte[] cr = { 0x0D };
            byte[] lf = { 0x0A };
            byte[] crcrlf = { 0x0D, 0x0D, 0x0A };
            byte[] crlf = { 0x0D, 0x0A };
            byte[] lfcr = { 0x0A, 0x0D };

            var count = Search(all, lf);

            var result = Replace(all, lf, crlf);
        }

        public static int Search(byte[] all, byte[] s)
        {
            if (s.Length == 0 || s.Length > all.Length)
            {
                return -1;
            }

            var count = 0;

            for (var i = 0; i < all.Length - s.Length + 1; i++)
            {
                var catchByte = true;

                for (var j = 0; j < s.Length; j++)
                {
                    if (all[i + j] != s[j])
                    {
                        catchByte = false;
                        break;
                    }
                }

                if (catchByte)
                {
                    count++;
                    i += s.Length - 1;
                }
            }

            return count;
        }

        public static byte[] Replace(byte[] all, byte[] s, byte[] t)
        {
            if (s.Length == 0 || s.Length > all.Length)
            {
                return null;
            }

            if (t.Length == 0)
            {
            }

            var temp = new List<byte>();

            for (var i = 0; i < all.Length - s.Length + 1; i++)
            {
                var catchByte = true;

                for (var j = 0; j < s.Length; j++)
                {
                    if (all[i + j] != s[j])
                    {
                        catchByte = false;
                        break;
                    }
                }

                if (catchByte)
                {
                    temp.AddRange(t);
                    i += s.Length - 1;
                }
                else
                {
                    temp.Add(all[i]);
                }

                if (i == all.Length - s.Length)
                {
                    if (!catchByte)
                    {
                        for (int k = s.Length - 1; k > 0; k--)
                        {
                            temp.Add(all[all.Length - k]);
                        }
                    }
                }
            }

            return temp.ToArray();
        }
    }
}
