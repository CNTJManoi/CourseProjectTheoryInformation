namespace DataReceiver.Models;

internal class CharInfoOne
{
    public CharInfoOne(char letter, string code)
    {
        Char = letter;
        Code = code;
    }

    public char Char { get; set; }
    public string Code { get; set; }
}