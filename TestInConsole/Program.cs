using Solnet.Wallet;
using Zebec.Streams;



public class Program
{
    const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";


    public static void Main()
    {
        var w = new Wallet(MnemonicWords);
        Account account = w.GetAccount(0);

        Console.WriteLine(account.ToString());
    }



}
