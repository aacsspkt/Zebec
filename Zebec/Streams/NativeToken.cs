using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solnet.Programs;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using Zebec.Models;

namespace Zebec.Streams
{


    public class NativeToken
    {
        // named as connection in javascript sdk
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.DevNet);

        //public async void InitAsync(PublicKey senderAddress, PublicKey receiverAddress, float ammountToSend, long startTime, long endTime)
        //{
            // Note: Wallet is also required here.
            // Don't know which is better, whether to ask it as parameter in this method or during constructor intialization!!!
            //_ = new NotImplementedException();
        //}


        public static async Task<RequestResult<string>> DepositAsync(Account senderAccount, decimal amountInSOL)
        {
            List<byte[]> seeds = new List<byte[]>();
            seeds.Add(senderAccount.PublicKey.KeyBytes);

            bool success = PublicKey.TryFindProgramAddress(seeds, Constants.PROGRAM_ID, out PublicKey validProgramAddress, out byte bump);

            List<AccountMeta> keys = new();
            keys.Add(AccountMeta.Writable(senderAccount, true));
            keys.Add(AccountMeta.Writable(validProgramAddress, false));
            keys.Add(AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false));

            var deposit = new Deposit()
            {
                AmountInLamport = SolHelper.ConvertToLamports(amountInSOL)
            };

            var instruction = TransactionInstructionFactory.Create(Constants.PROGRAM_ID, keys, deposit.Serialize());

            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();

            byte[] signedTransaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(senderAccount.PublicKey)
                .AddInstruction(instruction)
                .Build(senderAccount);

            RequestResult<string> signature = await rpcClient.SendTransactionAsync(signedTransaction);

            return signature;
        }
    }
}
