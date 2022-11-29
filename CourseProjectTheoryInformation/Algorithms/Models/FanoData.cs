namespace DataSender.Algorithms.Models;

public class FanoData
{
    public FanoData(char letter, float px, int lm, string code)
    {
        Char = letter;
        Px = px;
        Lm = lm;
        Code = code;
    }

    public char Char { get; set; }
    public float Px { get; set; }
    public string Code { get; set; }
    public int Lm { get; set; }
}