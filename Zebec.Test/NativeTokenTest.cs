using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Wallet;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Zebec.Models;
using Zebec.Clients.Streams;
using Zebec.Utils;
using Solnet.Rpc.Core.Http;

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

            RequestResult<ZebecResponse> response = await NativeToken.Deposit(testAccount, (decimal)1d);

            Assert.AreEqual(response.HttpStatusCode, HttpStatusCode.OK);
            Assert.IsTrue(response.WasSuccessful);
        }

        [TestMethod]
        public async Task TestWithdraw()
        {
            Account testAccount = testWallet.GetAccount(0);

            RequestResult<ZebecResponse> response = await NativeToken.Withdraw(testAccount, (decimal)0.0000001d);

            Assert.AreEqual(response.HttpStatusCode, HttpStatusCode.OK);
            Assert.IsTrue(response.WasSuccessful);
        }

        [TestMethod]
        public async Task TestInitializeStream()
        {
            Account senderTestAccount = testWallet.GetAccount(0);
            Account recieverTestAccount = testWallet.GetAccount(1);

            DateTime now = DateTime.UtcNow;

            RequestResult<ZebecResponse> response = await NativeToken.InitializeStream(
                senderTestAccount,
                recieverTestAccount,
                1,
                (ulong)now.ToUnixTimestamp(),
                (ulong)now.AddMinutes(20).ToUnixTimestamp()
                );

            Assert.AreEqual(response.HttpStatusCode, HttpStatusCode.OK);
            Assert.IsTrue(response.WasSuccessful); 
        }

        [TestMethod]
        public async Task TestPauseStream()
        {
            Account senderTestAccount = testWallet.GetAccount(0);
            Account recieverTestAccount = testWallet.GetAccount(1);

            var streamDataPda = new PublicKey("57WpV7rY9DEgWdmPxSgJ9sXey54ctNgZhEXDAKrBeqAQ");
            RequestResult<ZebecResponse> response = await NativeToken.PauseStream(
                senderTestAccount,
                recieverTestAccount,
                streamDataPda
                );

            Assert.AreEqual(response.HttpStatusCode, HttpStatusCode.OK);
            Assert.IsTrue(response.WasSuccessful);
        }

        [TestMethod]
        public async Task TestResumeStream()
        {
            Account senderTestAccount = testWallet.GetAccount(0);
            Account recieverTestAccount = testWallet.GetAccount(1);

            var streamDataPda = new PublicKey("57WpV7rY9DEgWdmPxSgJ9sXey54ctNgZhEXDAKrBeqAQ");
            RequestResult<ZebecResponse> response = await NativeToken.ResumeStream(
                senderTestAccount,
                recieverTestAccount,
                streamDataPda
                );

            Assert.AreEqual(response.HttpStatusCode, HttpStatusCode.OK);
            Assert.IsTrue(response.WasSuccessful);
        }

        [TestMethod]
        public async Task TestCancelStream()
        {
            Account senderTestAccount = testWallet.GetAccount(0);
            Account recieverTestAccount = testWallet.GetAccount(1);

            var streamDataPda = new PublicKey("57WpV7rY9DEgWdmPxSgJ9sXey54ctNgZhEXDAKrBeqAQ");
            RequestResult<ZebecResponse> response = await NativeToken.CancelStream(
                senderTestAccount,
                recieverTestAccount,
                streamDataPda
                );

            Assert.AreEqual(response.HttpStatusCode, HttpStatusCode.OK);
            Assert.IsTrue(response.WasSuccessful);
        }

        [TestMethod]
        public async Task TestWithdrawFromStream()
        {
            Account senderTestAccount = testWallet.GetAccount(0);
            Account recieverTestAccount = testWallet.GetAccount(1);

            var streamDataPda = new PublicKey("57WpV7rY9DEgWdmPxSgJ9sXey54ctNgZhEXDAKrBeqAQ");
            RequestResult<ZebecResponse> response = await NativeToken.WithdrawStream(
                senderTestAccount,
                recieverTestAccount,
                streamDataPda,
                (decimal)0.000000025d
                );

            Assert.AreEqual(response.HttpStatusCode, HttpStatusCode.OK);
            Assert.IsTrue(response.WasSuccessful);
        }
    }
}