using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSender.Models
{
    internal static class TextUtils
    {
        public static int Nabiv { get; set; }
        public static string CharToBytes(string bin)
        {
            Nabiv = 0;
            while (bin.Length % 4 != 0)
            {
                Nabiv++;
                bin = bin.Insert(0, "0");
            }

            return bin;
        }

        public static string BytesToChar(string binaryStr)
        {
            var byteArray = Enumerable.Range(0, int.MaxValue / 8).Select(i => i * 8)
                .TakeWhile(i => i < binaryStr.Length)
                .Select(i => binaryStr.Substring(i, 8))
                .Select(s => Convert.ToByte(s, 2))
                .ToArray();
            byte[] bytes = { byteArray[1], byteArray[0] };
            return Encoding.Unicode.GetString(bytes);
        }

        public static IEnumerable<string> Split(TextReader sr, int size, bool fixedSize = true)
        {
            while (sr.Peek() >= 0)
            {
                var buffer = new char[size];
                var c = sr.ReadBlock(buffer, 0, size);
                yield return fixedSize ? new string(buffer) : new string(buffer, 0, c);
            }
        }

        public static IEnumerable<string> Split(string s, int size, bool fixedSize = true)
        {
            var sr = new StringReader(s);
            return Split(sr, size, fixedSize);
        }
    }
}
