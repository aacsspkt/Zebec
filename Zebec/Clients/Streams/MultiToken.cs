using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zebec.Models;
using Zebec.Programs;

namespace Zebec.Clients.Streams
{
    public class MultiToken
    {
        /// <summary>
        /// The rpc client that communicates with solana blockchain.
        /// </summary>
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.DevNet);


        public static async Task<RequestResult<ZebecResponse>> Deposit(
            Account account,
            PublicKey token,
            decimal amount)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();
            Debug.WriteLineIf(blockHash.WasSuccessful, blockHash.Result.Value.Blockhash, "BlockHash");

            byte[] transaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(account)
                .AddInstruction(ZebecProgram.DepositToken(
                    account, 
                    token, 
                    SolHelper.ConvertToLamports(amount))
                )
                .Build(account);

            RequestResult<string> requestResult = await rpcClient.SendTransactionAsync(transaction);
            Debug.WriteLine(requestResult.HttpStatusCode.ToString(), nameof(requestResult.HttpStatusCode));
            Debug.WriteLine(requestResult.WasSuccessful, nameof(requestResult.WasSuccessful));
            Debug.WriteLine(requestResult.Reason, nameof(requestResult.Reason));
            Debug.WriteLine(requestResult.RawRpcResponse, nameof(requestResult.RawRpcResponse));
            
            
            return new RequestResult<ZebecResponse>()
            {
                ErrorData = requestResult.ErrorData,
                HttpStatusCode = requestResult.HttpStatusCode,
                Reason = requestResult.Reason,
                Result = new ZebecResponse(requestResult.Result),
                ServerErrorCode = requestResult.ServerErrorCode,
                WasHttpRequestSuccessful = requestResult.WasHttpRequestSuccessful,
                WasRequestSuccessfullyHandled = requestResult.WasRequestSuccessfullyHandled,
            };
        }


        public static async Task<RequestResult<ZebecResponse>> Withdraw(
            Account account,
            PublicKey token,
            ulong amount)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();
            Debug.WriteLineIf(blockHash.WasSuccessful, blockHash.Result.Value.Blockhash, "BlockHash");

            byte[] transaction = new TransactionBuilder().
               SetRecentBlockHash(blockHash.Result.Value.Blockhash)
               .SetFeePayer(account)
               .AddInstruction(ZebecProgram.WithdrawToken(
                   account.PublicKey,
                   token,
                   SolHelper.ConvertToLamports(amount))
               )
               .Build(account);

            RequestResult<string> requestResult = await rpcClient.SendTransactionAsync(transaction);
            Debug.WriteLine(requestResult.HttpStatusCode.ToString(), nameof(requestResult.HttpStatusCode));
            Debug.WriteLine(requestResult.WasSuccessful, nameof(requestResult.WasSuccessful));
            Debug.WriteLine(requestResult.RawRpcResponse, nameof(requestResult.RawRpcResponse));
            Debug.WriteLine(requestResult.Reason, nameof(requestResult.Reason));

            return new RequestResult<ZebecResponse>()
            {
                ErrorData = requestResult.ErrorData,
                HttpStatusCode = requestResult.HttpStatusCode,
                Reason = requestResult.Reason,
                Result = new ZebecResponse(requestResult.Result),
                WasHttpRequestSuccessful = requestResult.WasHttpRequestSuccessful,
                WasRequestSuccessfullyHandled = requestResult.WasRequestSuccessfullyHandled,
            };
        }

    }
}
