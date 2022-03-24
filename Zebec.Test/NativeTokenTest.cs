using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Wallet;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Zebec.Models;
using Zebec.Streams;
using Zebec.Utils;

namespace Zebec.Test
{
    [TestClass]
    public class NativeTokenTest
    {
        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        private readonly Wallet testWallet = new Wallet(MnemonicWords);

        [TestMethod]
        public async Task TestDeposit()
        {
            Account testAccount = testWallet.GetAccount(0);

            ZebecResponse response = await NativeToken.Deposit(testAccount, (decimal)0.00000001d);

            Assert.AreEqual(response.Result.HttpStatusCode, HttpStatusCode.OK);
            Assert.IsTrue(response.Result.WasSuccessful);
        }

        [TestMethod]
        public async Task TestWithdraw()
        {
            Account testAccount = testWallet.GetAccount(0);

            ZebecResponse response = await NativeToken.Withdraw(testAccount, (decimal)0.1d);

            Assert.AreEqual(response.Result.HttpStatusCode, HttpStatusCode.OK);
            Assert.IsTrue(response.Result.WasSuccessful);
        }

        [TestMethod]
        public async Task TestInitializeStream()
        {
            Account senderTestAccount = testWallet.GetAccount(0);
            Account recieverTestAccount = testWallet.GetAccount(1);

            DateTime now = DateTime.UtcNow;

            ZebecResponse response = await NativeToken.InitializeStream(
                senderTestAccount,
                recieverTestAccount,
                1,
                (ulong)now.ToUnixTimestamp(),
                (ulong)now.AddMinutes(20).ToUnixTimestamp()
                );

            Assert.AreEqual(response.Result.HttpStatusCode, HttpStatusCode.OK);
            Assert.IsTrue(response.Result.WasSuccessful);
        }

        [TestMethod]
        public async Task TestPauseStream()
        {
            Account senderTestAccount = testWallet.GetAccount(0);
            Account recieverTestAccount = testWallet.GetAccount(1);

            var streamDataPda = new PublicKey("57WpV7rY9DEgWdmPxSgJ9sXey54ctNgZhEXDAKrBeqAQ");
            ZebecResponse response = await NativeToken.PauseStream(
                senderTestAccount,
                recieverTestAccount,
                streamDataPda
                );

            Assert.AreEqual(response.Result.HttpStatusCode, HttpStatusCode.OK);
            Assert.IsTrue(response.Result.WasSuccessful);
        }

        [TestMethod]
        public async Task TestResumeStream()
        {
            Account senderTestAccount = testWallet.GetAccount(0);
            Account recieverTestAccount = testWallet.GetAccount(1);

            var streamDataPda = new PublicKey("57WpV7rY9DEgWdmPxSgJ9sXey54ctNgZhEXDAKrBeqAQ");
            ZebecResponse response = await NativeToken.ResumeStream(
                senderTestAccount,
                recieverTestAccount,
                streamDataPda
                );

            Assert.AreEqual(response.Result.HttpStatusCode, HttpStatusCode.OK);
            Assert.IsTrue(response.Result.WasSuccessful);
        }

        [TestMethod]
        public async Task TestCancelStream()
        {
            Account senderTestAccount = testWallet.GetAccount(0);
            Account recieverTestAccount = testWallet.GetAccount(1);

            var streamDataPda = new PublicKey("57WpV7rY9DEgWdmPxSgJ9sXey54ctNgZhEXDAKrBeqAQ");
            ZebecResponse response = await NativeToken.CancelStream(
                senderTestAccount,
                recieverTestAccount,
                streamDataPda
                );

            Assert.AreEqual(response.Result.HttpStatusCode, HttpStatusCode.OK);
            Assert.IsTrue(response.Result.WasSuccessful);
        }

        [TestMethod]
        public async Task TestWithdrawFromStream()
        {
            Account senderTestAccount = testWallet.GetAccount(0);
            Account recieverTestAccount = testWallet.GetAccount(1);

            var streamDataPda = new PublicKey("57WpV7rY9DEgWdmPxSgJ9sXey54ctNgZhEXDAKrBeqAQ");
            ZebecResponse response = await NativeToken.WithdrawStream(
                senderTestAccount,
                recieverTestAccount,
                streamDataPda,
                (decimal)0.000000025d
                );

            Assert.AreEqual(response.Result.HttpStatusCode, HttpStatusCode.OK);
            Assert.IsTrue(response.Result.WasSuccessful);
        }
    }
}