namespace DataReceiver.Models;

internal class CharInfo
{
    public CharInfo(string info, string examination, string countability)
    {
        Info = info;
        Examination = examination;
        Countability = countability;
    }

    public string Info { get; set; }
    public string Examination { get; set; }
    public string Countability { get; set; }
}