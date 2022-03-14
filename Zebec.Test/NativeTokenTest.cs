using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Wallet;
using System;
using System.Threading.Tasks;
using Zebec.Streams;

namespace Zebec.Test
{
    [TestClass]
    public class NativeTokenTest
    {
        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        [TestMethod]
        public async Task TestDepositAsync()
        { 
            var wallet = new Wallet(MnemonicWords);
            Console.WriteLine(wallet.ToString());

            //var testAccount = wallet.GetAccount(10);
            //bool isSuccessful = true;

            //var requestResult = await NativeToken.DepositAsync(testAccount, 1);

            //Assert.IsTrue(isSuccessful == requestResult.WasSuccessful);
        }

        
    }
}