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


        public static async Task<RequestResult<ZebecResponse>> InitializeStream(
            Account fromAccount,
            Account toAccount,
            PublicKey token,
            decimal amount,
            ulong startTimeInUnixTimestamp,
            ulong endTimeInUnixTimestamp)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();
            Debug.WriteLineIf(blockHash.WasSuccessful, blockHash.Result.Value.Blockhash, "BlockHash");

            byte[] transaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(ZebecProgram.InitializeTokenStream(
                    fromAccount.PublicKey,
                    toAccount.PublicKey,
                    token,
                    startTimeInUnixTimestamp,
                    endTimeInUnixTimestamp,
                    SolHelper.ConvertToLamports(amount),
                    out Account streamDataAccount)
                )
                .Build(new List<Account>() { fromAccount, streamDataAccount, });

            RequestResult<string> requestResult = await rpcClient.SendTransactionAsync(transaction);
            Debug.WriteLine(requestResult.HttpStatusCode.ToString(), nameof(requestResult.HttpStatusCode));
            Debug.WriteLine(requestResult.WasSuccessful, nameof(requestResult.WasSuccessful));
            Debug.WriteLine(requestResult.Reason, nameof(requestResult.Reason));
            Debug.WriteLine(requestResult.RawRpcResponse, nameof(requestResult.RawRpcResponse));

            return new RequestResult<ZebecResponse>()
            {
                ErrorData = requestResult.ErrorData,
                HttpStatusCode = requestResult.HttpStatusCode,
                WasHttpRequestSuccessful = requestResult.WasHttpRequestSuccessful,
                Reason = requestResult.Reason,
                Result = new ZebecResponse(requestResult.Result, streamDataAccount),
                ServerErrorCode = requestResult.ServerErrorCode,
                WasRequestSuccessfullyHandled = requestResult.WasRequestSuccessfullyHandled,
            };
        }


        public static async Task<RequestResult<ZebecResponse>> WithdrawStream(
            Account fromAccount,
            Account toAccount,
            PublicKey token,
            PublicKey streamDataPda,
            decimal amount)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();
            Debug.WriteLineIf(blockHash.WasSuccessful, blockHash.Result.Value.Blockhash, "BlockHash");

            byte[] transaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(toAccount)
                .AddInstruction(ZebecProgram.WithdrawStreamSol(
                    fromAccount.PublicKey,
                    toAccount.PublicKey,
                    streamDataPda,
                    SolHelper.ConvertToLamports(amount))
                )
                .Build(toAccount);

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
                WasHttpRequestSuccessful = requestResult.WasHttpRequestSuccessful,
                WasRequestSuccessfullyHandled = requestResult.WasRequestSuccessfullyHandled,
            };
        }

        public static async Task<RequestResult<ZebecResponse>> CancelStream(
            Account fromAccount,
            Account toAccount,
            PublicKey token,
            PublicKey streamDataPda)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();
            Debug.WriteLineIf(blockHash.WasSuccessful, blockHash.Result.Value.Blockhash, "BlockHash");

            byte[] transaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(ZebecProgram.CancelTokenStream(
                    fromAccount.PublicKey,
                    toAccount.PublicKey,
                    token,
                    streamDataPda)
                )
                .Build(fromAccount);

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
                WasHttpRequestSuccessful = requestResult.WasHttpRequestSuccessful,
                WasRequestSuccessfullyHandled = requestResult.WasRequestSuccessfullyHandled,
            };
        }


        public static async Task<RequestResult<ZebecResponse>> PauseStream(
            Account fromAccount,
            Account toAccount,
            PublicKey streamDataPda)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();
            Debug.WriteLineIf(blockHash.WasSuccessful, blockHash.Result.Value.Blockhash, "BlockHash");

            byte[] transaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(ZebecProgram.PauseTokenStream(
                    fromAccount.PublicKey,
                    toAccount.PublicKey,
                    streamDataPda)
                )
                .Build(new List<Account>() { fromAccount, toAccount });

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
                WasHttpRequestSuccessful = requestResult.WasHttpRequestSuccessful,
                WasRequestSuccessfullyHandled = requestResult.WasRequestSuccessfullyHandled,
            };
        }


        public static async Task<RequestResult<ZebecResponse>> ResumeStream(
            Account fromAccount,
            Account toAccount,
            PublicKey streamDataPda)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();
            Debug.WriteLineIf(blockHash.WasSuccessful, blockHash.Result.Value.Blockhash, "BlockHash");

            byte[] transaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(ZebecProgram.ResumeTokenStream(
                    fromAccount.PublicKey,
                    toAccount.PublicKey,
                    streamDataPda)
                )
                .Build(fromAccount);

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
                WasHttpRequestSuccessful = requestResult.WasHttpRequestSuccessful,
                WasRequestSuccessfullyHandled = requestResult.WasRequestSuccessfullyHandled,
            };
        }
    }
}
