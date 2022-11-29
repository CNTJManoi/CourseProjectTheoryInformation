namespace DataReceiver.Models;

internal class DecodeText
{
    public DecodeText(string info, string examination, string s, string syndrome, string comment, string result)
    {
        Info = info;
        Examination = examination;
        S = s;
        Syndrome = syndrome;
        Comment = comment;
        Result = result;
    }

    public string Info { get; set; }
    public string Examination { get; set; }
    public string S { get; set; }
    public string Syndrome { get; set; }
    public string Comment { get; set; }
    public string Result { get; set; }
}