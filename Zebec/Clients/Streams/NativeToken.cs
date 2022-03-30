﻿using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Solnet.Programs;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System.Text;
using Zebec.Models;
using Solnet.Rpc.Builders;
using Zebec.Programs;
using Solnet.Rpc.Types;

namespace Zebec.Clients.Streams
{


    public class NativeToken
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.DevNet);

        public static async Task<RequestResult<ZebecResponse>> Deposit(
            Account account, 
            decimal amount)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();
            Debug.WriteLineIf(blockHash.WasSuccessful, blockHash.Result.Value.Blockhash, "BlockHash");


            byte[] transaction = new TransactionBuilder().
                SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(account)
                .AddInstruction(ZebecProgram.DepositSol(
                    account.PublicKey, 
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
            decimal amount)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();
            Debug.WriteLineIf(blockHash.WasSuccessful, blockHash.Result.Value.Blockhash, "BlockHash");

            byte[] transaction = new TransactionBuilder().
               SetRecentBlockHash(blockHash.Result.Value.Blockhash)
               .SetFeePayer(account)
               .AddInstruction(ZebecProgram.WithdrawSol(
                   account.PublicKey, 
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
            decimal amount, 
            ulong startTimeInUnixTimestamp,
            ulong endTimeInUnixTimestamp)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();
            Debug.WriteLineIf(blockHash.WasSuccessful, blockHash.Result.Value.Blockhash, "BlockHash");

            byte[] transaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(ZebecProgram.InitializeSolStream(
                    fromAccount.PublicKey, 
                    toAccount.PublicKey, 
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
            PublicKey streamDataPda)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();
            Debug.WriteLineIf(blockHash.WasSuccessful, blockHash.Result.Value.Blockhash, "BlockHash");

            byte[] transaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(ZebecProgram.CancelSolStream(
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
                .AddInstruction(ZebecProgram.PauseSolStream(
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
                Result = new ZebecResponse (requestResult.Result),
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
                .AddInstruction(ZebecProgram.ResumeSolStream(
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