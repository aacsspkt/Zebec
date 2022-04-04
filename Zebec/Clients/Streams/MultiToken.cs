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
    public class MultiToken
    {
        /// <summary>
        /// The rpc client that communicates with solana blockchain.
        /// </summary>
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.DevNet);

        /// <summary>
        /// Create and send transaction to deposit token in Zebec Program.
        /// </summary>
        /// <param name="account">The account who deposits token.</param>
        /// <param name="token">The token which is stream.</param>
        /// <param name="amount">The amount of token to deposit.</param>
        /// <returns>Returns <see cref="RequestResult{T}"/> where T is <see cref="ZebecResponse"/>.</returns>
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
            
            
            return ResponseMaker.Make(requestResult);
        }

        /// <summary>
        /// Create and send transaction to withdraw deposited token from Zebec Program.
        /// </summary>
        /// <param name="account">The account who deposited token.</param>
        /// <param name="token">The token which is stream.</param>
        /// <param name="amount">The amount of token to withdraw.</param>
        /// <returns>Returns <see cref="RequestResult{T}"/> where T is <see cref="ZebecResponse"/>.</returns>
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

            return ResponseMaker.Make(requestResult);
        }

        /// <summary>
        /// Create and send transaction to initialize token stream.
        /// </summary>
        /// <param name="fromAccount">The account who initializes token stream.</param>
        /// <param name="toAccount">The account to which token is streamed.</param>
        /// <param name="token">The token which is streamed.</param>
        /// <param name="amount">The amount of token to be streamed.</param>
        /// <param name="startTimeInUnixTimestamp">The unix timestamp at which stream initializes.</param>
        /// <param name="endTimeInUnixTimestamp">The unix timestamp at whick stream ends.</param>
        /// <returns>Returns <see cref="RequestResult{T}"/> where T is <see cref="ZebecResponse"/>.</returns>
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

            return ResponseMaker.Make(requestResult, streamDataAccount.PublicKey);
        }

        /// <summary>
        /// Create and send transaction to withdraw streamed token.
        /// </summary>
        /// <param name="fromAccount">The account who initialized token stream.</param>
        /// <param name="toAccount">The account to which token was streamed.</param>
        /// <param name="token">The token that was stream.</param>
        /// <param name="streamDataPda">The public key which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <param name="amount">The amount of token to withdraw.</param>
        /// <returns>Returns <see cref="RequestResult{T}"/> where T is <see cref="ZebecResponse"/>.</returns>
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

            return ResponseMaker.Make(requestResult);
        }


        /// <summary>
        /// Create and send transaction to cancel initialized stream.
        /// </summary>
        /// <param name="fromAccount">The account who initialized token stream.</param>
        /// <param name="toAccount">The account to which token was streamed and withdraws token.</param>
        /// <param name="token">The token which was streamed.</param>
        /// <param name="streamDataPda">The public key which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <returns>Returns <see cref="RequestResult{T}"/> where T is <see cref="ZebecResponse"/>.</returns>
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

            return ResponseMaker.Make(requestResult);
        }

        /// <summary>
        /// Create and send transaction to pause on going stream.
        /// </summary>
        /// <param name="fromAccount">The account who initialized token stream.</param>
        /// <param name="toAccount">The account to which token was streamed and withdraws token.</param>
        /// <param name="streamDataPda">The public key which was return in 
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
                .AddInstruction(ZebecProgram.PauseTokenStream(
                    fromAccount.PublicKey,
                    toAccount.PublicKey,
                    streamDataPda)
                )
                .Build(new List<Account>() { fromAccount });

            RequestResult<string> requestResult = await rpcClient.SendTransactionAsync(transaction);
            Debug.WriteLine(requestResult.HttpStatusCode.ToString(), nameof(requestResult.HttpStatusCode));
            Debug.WriteLine(requestResult.WasSuccessful, nameof(requestResult.WasSuccessful));
            Debug.WriteLine(requestResult.Reason, nameof(requestResult.Reason));
            Debug.WriteLine(requestResult.RawRpcResponse, nameof(requestResult.RawRpcResponse));

            return ResponseMaker.Make(requestResult);
        }

        /// <summary>
        /// Create and send transaction to resume paused stream.
        /// </summary>
        /// <param name="fromAccount">The account who initialized token stream.</param>
        /// <param name="toAccount">The account to which token was streamed and withdraws token.</param>
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

            return ResponseMaker.Make(requestResult);
        }

        /// <summary>
        /// Create and send transaction to extends and fund token to an already initialized stream. 
        /// </summary>
        /// <param name="fromAccount">The account who initialized token stream.</param>
        /// <param name="streamDataPda">The public key which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <param name="token">The token which is streamed.</param>
        /// <param name="endTimeInUnixTimestamp">The unix timestamp at whick stream ends.</param>
        /// <param name="amount">The amount of token to fund.</param>
        /// <returns>Returns <see cref="RequestResult{T}"/> where T is <see cref="ZebecResponse"/>.</returns>
        public static async Task<RequestResult<ZebecResponse>> FundSol(
            Account fromAccount,
            PublicKey streamDataPda,
            PublicKey token,
            ulong endTimeInUnixTimestamp,
            ulong amount)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();
            Debug.WriteLineIf(blockHash.WasSuccessful, blockHash.Result.Value.Blockhash, "BlockHash");

            byte[] transaction = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(ZebecProgram.FundToken(
                    fromAccount,
                    streamDataPda,
                    token,
                    endTimeInUnixTimestamp,
                    amount)
                )
                .Build(fromAccount);

            RequestResult<string> requestResult = await rpcClient.SendTransactionAsync(transaction);
            Debug.WriteLine(requestResult.HttpStatusCode.ToString(), nameof(requestResult.HttpStatusCode));
            Debug.WriteLine(requestResult.WasSuccessful, nameof(requestResult.WasSuccessful));
            Debug.WriteLine(requestResult.Reason, nameof(requestResult.Reason));
            Debug.WriteLine(requestResult.RawRpcResponse, nameof(requestResult.RawRpcResponse));

            return ResponseMaker.Make(requestResult);
        }
    }
}
