namespace HammingAlgorithm;

public class Hamming
{
    public static string CodeText(string Text, List<List<int>> GenerativeMatr)
    {
        var result = "";
        var ch = "";
        for (var i = 0; i < Text.Length; i++)
        {
            var Binary = TextUtils.CharToBytes(Text[i]);
            var Bytes = TextUtils.Split(Binary, 4).ToList();
            for (var m = 0; m < Bytes.Count; m++)
            for (var j = 0; j < GenerativeMatr[0].Count; j++)
            {
                var tempRes = "";
                for (var k = 0; k < Bytes[m].Count(); k++)
                    tempRes += byte.Parse(Bytes[m][k].ToString()) & GenerativeMatr[k][j];
                var r = 0;
                foreach (var byteS in tempRes) r += int.Parse(byteS.ToString());
                r = r % 2;
                result += r;
                ch += r;
                if (ch.Length == 7)
                {
                    var sum = 0;
                    foreach (var byt in ch) sum += int.Parse(byt.ToString());
                    sum = sum % 2;
                    result += sum;
                    ch = "";
                }
            }
        }

        return result;
    }
}