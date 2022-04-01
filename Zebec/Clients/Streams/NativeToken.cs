using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System.Diagnostics;
using Zebec.Models;
using Zebec.Programs;

namespace Zebec.Clients.Streams
{
    public class NativeToken
    {
        /// <summary>
        /// The rpc client that communicates with solana blockchain.
        /// </summary>
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.DevNet);

        /// <summary>
        /// Create and send transaction to deposit sol in Zebec Program.
        /// </summary>
        /// <param name="account">The account who deposits sol.</param>
        /// <param name="amount">The amount of sol to deposit.</param>
        /// <returns>Returns <see cref="RequestResult{T}"/> where T is <see cref="ZebecResponse"/>.</returns>
        public static async Task<RequestResult<ZebecResponse>> Deposit(
            Account account, 
            decimal amount)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();
            Debug.WriteLineIf(blockHash.WasSuccessful, blockHash.Result.Value.Blockhash, "BlockHash");

            byte[] transaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
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

        /// <summary>
        /// Create and send transaction to withdraw deposited sol from Zebec Program.
        /// </summary>
        /// <param name="account">The account who deposited sol.</param>
        /// <param name="amount">The amount sol to withdraw.</param>
        /// <returns>Returns <see cref="RequestResult{T}"/> where T is <see cref="ZebecResponse"/>.</returns>
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

        /// <summary>
        /// Create and send transaction to initialize sol stream.
        /// </summary>
        /// <param name="fromAccount">The account who initializes sol stream.</param>
        /// <param name="toAccount">The account to which sol is streamed.</param>
        /// <param name="amount">The amount of sol to be streamed.</param>
        /// <param name="startTimeInUnixTimestamp">The unix timestamp at which stream initializes.</param>
        /// <param name="endTimeInUnixTimestamp">The unix timestamp at whick stream ends.</param>
        /// <returns>Returns <see cref="RequestResult{T}"/> where T is <see cref="ZebecResponse"/>.</returns>
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

        /// <summary>
        /// Create and send transaction to withdraw streamed sol.
        /// </summary>
        /// <param name="fromAccount">The account who initialized sol stream.</param>
        /// <param name="toAccount">The account to which sol was streamed and withdraws sol.</param>
        /// <param name="streamDataPda">The public key which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <param name="amount">The amount of sol to withdraw.</param>
        /// <returns>Returns <see cref="RequestResult{T}"/> where T is <see cref="ZebecResponse"/>.</returns>
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

        /// <summary>
        /// Create and send transaction to cancel initialized stream.
        /// </summary>
        /// <param name="fromAccount">The account who initialized sol stream.</param>
        /// <param name="toAccount">The account to which sol was streamed and withdraws sol.</param>
        /// <param name="streamDataPda">The public key which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <returns>Returns <see cref="RequestResult{T}"/> where T is <see cref="ZebecResponse"/>.</returns>
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

        /// <summary>
        /// Create and send transaction to pause on going stream.
        /// </summary>
        /// <param name="fromAccount">The account who initialized sol stream.</param>
        /// <param name="toAccount">The account to which sol was streamed and withdraws sol.</param>
        /// <param name="streamDataPda"><param name="streamDataPda">The public key which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <returns>Returns <see cref="RequestResult{T}"/> where T is <see cref="ZebecResponse"/>.</returns>
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

        /// <summary>
        /// Create and send transaction to resume paused stream.
        /// </summary>
        /// <param name="fromAccount">The account who initialized sol stream.</param>
        /// <param name="toAccount">The account to which sol was streamed and withdraws sol.</param>
        /// <param name="streamDataPda">The public key which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <returns>Returns <see cref="RequestResult{T}"/> where T is <see cref="ZebecResponse"/>.</returns>
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

        /// <summary>
        /// Create and send transaction to extends and fund sol an already initialized stream. 
        /// </summary>
        /// <param name="fromAccount">The account who initialized sol stream.</param>
        /// <param name="streamDataPda">The public key which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <param name="endTimeInUnixTimestamp">The unix timestamp at whick stream ends.</param>
        /// <param name="amount">The amount of sol to fund.</param>
        /// <returns></returns>
        public static async Task<RequestResult<ZebecResponse>> FundSol(
            Account fromAccount,
            PublicKey streamDataPda,
            ulong endTimeInUnixTimestamp,
            ulong amount)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();
            Debug.WriteLineIf(blockHash.WasSuccessful, blockHash.Result.Value.Blockhash, "BlockHash");

            byte[] transaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(ZebecProgram.FundSol(
                    fromAccount, 
                    streamDataPda, 
                    endTimeInUnixTimestamp, 
                    amount)
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