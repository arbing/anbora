using System.Collections.Generic;

namespace ConvertEoL
{
    public static class ByteExtension
    {
        public static int Search(this byte[] all, byte[] s)
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

        public static byte[] Replace(this byte[] all, byte[] s, byte[] t)
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
