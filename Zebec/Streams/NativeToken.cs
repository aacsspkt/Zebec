using Solnet.Programs;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System.Text;
using Zebec.Models;


namespace Zebec.Streams
{


    public class NativeToken
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.DevNet);

        private static readonly PublicKey Base58PublicKey = new PublicKey("AknC341xog56SrnoK6j3mUvaD1Y7tYayx1sxUGpeYWdX");

        private static readonly PublicKey ZebecProgramIdKey = new PublicKey("AknC341xog56SrnoK6j3mUvaD1Y7tYayx1sxUGpeYWdX");

        private static readonly PublicKey FeeAddressIdKey = new PublicKey("EsDV3m3xUZ7g8QKa1kFdbZT18nNz8ddGJRcTK84WDQ7k");

        private const string STRING_OF_WITHDRAW = "withdraw_sol";

        private Wallet wallet { get; set; }

        public NativeToken(Wallet wallet)
        {
            this.wallet = wallet;
        }

        public async Task<ZebecResult> InitStreamAsync(Account fromAccount, Account toAccount, decimal ammountInSol, ulong startTime, ulong endTime)
        {
            bool success = PublicKey.TryFindProgramAddress(
                new List<byte[]> { Encoding.ASCII.GetBytes(STRING_OF_WITHDRAW), fromAccount.PublicKey.KeyBytes },
                ZebecProgramIdKey,
                out PublicKey validProgramPublicKey,
                out byte bump
                );

            var streamDepositAccount = new Account();

            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();

            TransactionInstruction instruction = new()
            {
                Keys = new List<AccountMeta>()
                {
                    AccountMeta.Writable(fromAccount.PublicKey, true),
                    AccountMeta.Writable(toAccount.PublicKey, false),
                    AccountMeta.Writable(streamDepositAccount.PublicKey, true),
                    AccountMeta.Writable(validProgramPublicKey, false),
                    AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                },
                ProgramId = ZebecProgramIdKey.KeyBytes,
                Data = new InitStream()
                {
                    AmountInLamport = SolHelper.ConvertToLamports(ammountInSol),
                    StartTime = startTime,
                    EndTime = endTime
                }.Serialize()
            };

            Transaction transaction = new()
            {
                RecentBlockHash = blockHash.Result.Value.Blockhash,
                FeePayer = fromAccount,
                Instructions = new List<TransactionInstruction>() { instruction },
            };

            transaction.PartialSign(streamDepositAccount);

            byte[] compiledTransaction = transaction.CompileMessage();

            byte[] signedTransaction = wallet.Sign(compiledTransaction);

            RequestResult<string> requestResult = await rpcClient.SendTransactionAsync(signedTransaction);

            return new ZebecResult(requestResult.Result, streamDepositAccount);
        }


        public async Task<ZebecResult> DepositAsync(Account fromAccount, decimal amountInSOL)
        {
            bool success = PublicKey.TryFindProgramAddress(
                new List<byte[]>() { fromAccount.PublicKey.KeyBytes },
                ZebecProgramIdKey,
                out PublicKey validProgramPublicKey,
                out byte bump
                );

            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();

            TransactionInstruction instruction = new()
            {
                Keys = new List<AccountMeta>()
                {
                    AccountMeta.Writable(fromAccount, true),
                    AccountMeta.Writable(validProgramPublicKey, false),
                    AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                },
                ProgramId = ZebecProgramIdKey.KeyBytes,
                Data = new Deposit()
                {
                    AmountInLamport = SolHelper.ConvertToLamports(amountInSOL)
                }.Serialize()
            };

            Transaction transaction = new()
            {
                RecentBlockHash = blockHash.Result.Value.Blockhash,
                FeePayer = fromAccount.PublicKey,
                Instructions = new List<TransactionInstruction>() { instruction }
            };

            byte[] compiledTransaction = transaction.CompileMessage();

            byte[] signedTransaction = wallet.Sign(compiledTransaction);

            RequestResult<string> requestResult = await rpcClient.SendTransactionAsync(signedTransaction);

            return new ZebecResult(requestResult.Result);
        }


        public async Task<ZebecResult> WithdrawDeposited(Account fromAccount, decimal amountInSOL)
        {
            bool success = PublicKey.TryFindProgramAddress(
                new List<byte[]>() { fromAccount.PublicKey.KeyBytes },
                ZebecProgramIdKey,
                out PublicKey validProgramPublicKey,
                out byte bump
                );

            bool success1 = PublicKey.TryFindProgramAddress(
                new List<byte[]>() { Encoding.ASCII.GetBytes(STRING_OF_WITHDRAW), fromAccount.PublicKey.KeyBytes, },
                ZebecProgramIdKey,
                out PublicKey validProgramPublicKeyOfWithdraw,
                out byte bumpOfWithdraw
                );

            TransactionInstruction instruction = new()
            {
                Keys = new List<AccountMeta>()
                {
                    AccountMeta.Writable(fromAccount.PublicKey, true),
                    AccountMeta.Writable(validProgramPublicKey, false),
                    AccountMeta.Writable(validProgramPublicKeyOfWithdraw, false),
                    AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                },
                ProgramId = ZebecProgramIdKey.KeyBytes,
                Data = new WithdrawDeposit()
                {
                    AmountInLamport = SolHelper.ConvertToLamports(amountInSOL)
                }.Serialize(),
            };

            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();

            Transaction transaction = new()
            {
                RecentBlockHash = blockHash.Result.Value.Blockhash,
                FeePayer = fromAccount.PublicKey,
                Instructions = new List<TransactionInstruction>() { instruction }
            };

            byte[] compiledTransaction = transaction.CompileMessage();

            byte[] signedTransaction = wallet.Sign(compiledTransaction);

            RequestResult<string> requestResult = await rpcClient.SendTransactionAsync(signedTransaction);

            return new ZebecResult(requestResult.Result);
        }

        public async Task<ZebecResult> WithdrawStreamed(Account fromAccount, Account toAccount, Account tempStreamDepositAccount, decimal amountInSOL)
        {
            bool success = PublicKey.TryFindProgramAddress(
                new List<byte[]>() { fromAccount.PublicKey.KeyBytes },
                ZebecProgramIdKey,
                out PublicKey validProgramKey,
                out byte bump
                );

            bool success1 = PublicKey.TryFindProgramAddress(
                new List<byte[]>() { Encoding.ASCII.GetBytes(STRING_OF_WITHDRAW), fromAccount.PublicKey.KeyBytes, },
                ZebecProgramIdKey,
                out PublicKey validProgramKeyOfWithdraw,
                out byte bumpOfWithdraw
                );

            TransactionInstruction instruction = new()
            {
                Keys = new List<AccountMeta>()
                {
                    AccountMeta.Writable(fromAccount.PublicKey, false),
                    AccountMeta.Writable(toAccount.PublicKey, true),
                    AccountMeta.Writable(validProgramKey, false),
                    AccountMeta.Writable(tempStreamDepositAccount.PublicKey, false),
                    AccountMeta.Writable(validProgramKeyOfWithdraw, false),
                    AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                    AccountMeta.Writable(FeeAddressIdKey, false),
                },
                ProgramId = ZebecProgramIdKey.KeyBytes,
                Data = new WithdrawStreamed()
                {
                    AmountInLamport = SolHelper.ConvertToLamports(amountInSOL)
                }.Serialize(),
            };

            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();

            Transaction transaction = new()
            {
                RecentBlockHash = blockHash.Result.Value.Blockhash,
                FeePayer = fromAccount.PublicKey,
                Instructions = new List<TransactionInstruction>() { instruction }
            };

            byte[] compiledTransaction = transaction.CompileMessage();

            byte[] signedTransaction = wallet.Sign(compiledTransaction);

            RequestResult<string> requestResult = await rpcClient.SendTransactionAsync(signedTransaction);

            return new ZebecResult(requestResult.Result);
        }

        public async Task<ZebecResult> CancelStream(Account fromAccount, Account toAccount, Account tempStreamDepositAccount)
        {
            bool success = PublicKey.TryFindProgramAddress(
                new List<byte[]>() { fromAccount.PublicKey.KeyBytes },
                ZebecProgramIdKey,
                out PublicKey validProgramKey,
                out byte bump
                );

            bool success1 = PublicKey.TryFindProgramAddress(
                new List<byte[]>() { Encoding.ASCII.GetBytes(STRING_OF_WITHDRAW), fromAccount.PublicKey.KeyBytes, },
                ZebecProgramIdKey,
                out PublicKey validProgramKeyOfWithdraw,
                out byte bumpOfWithdraw
                );

            TransactionInstruction instruction = new()
            {
                Keys = new List<AccountMeta>()
                {
                    AccountMeta.Writable(fromAccount.PublicKey, true),
                    AccountMeta.Writable(toAccount.PublicKey, false),
                    AccountMeta.Writable(validProgramKey, false),
                    AccountMeta.Writable(tempStreamDepositAccount.PublicKey, false),
                    AccountMeta.Writable(validProgramKeyOfWithdraw, false),
                    AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                    AccountMeta.Writable(FeeAddressIdKey, false),
                },
                ProgramId = ZebecProgramIdKey.KeyBytes,
                Data = new CancelStream().Serialize(),
            };

            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();

            Transaction transaction = new()
            {
                RecentBlockHash = blockHash.Result.Value.Blockhash,
                FeePayer = fromAccount.PublicKey,
                Instructions = new List<TransactionInstruction>() { instruction }
            };

            byte[] compiledTransaction = transaction.CompileMessage();

            byte[] signedTransaction = wallet.Sign(compiledTransaction);

            RequestResult<string> requestResult = await rpcClient.SendTransactionAsync(signedTransaction);
            
            return new ZebecResult(requestResult.Result);

        }

        public async Task<ZebecResult> PauseStream(Account fromAccount, Account toAccount, Account tempStreamDepositAccount)
        {
            TransactionInstruction instruction = new()
            {
                Keys = new List<AccountMeta>()
                {
                    AccountMeta.Writable(fromAccount.PublicKey, true),
                    AccountMeta.Writable(toAccount.PublicKey, false),
                    AccountMeta.Writable(tempStreamDepositAccount.PublicKey, false),
                    AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                },
                ProgramId = ZebecProgramIdKey.KeyBytes,
                Data = new PauseStream().Serialize(),
            };

            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();

            Transaction transaction = new()
            {
                RecentBlockHash = blockHash.Result.Value.Blockhash,
                FeePayer = fromAccount.PublicKey,
                Instructions = new List<TransactionInstruction>() { instruction }
            };

            byte[] compiledTransaction = transaction.CompileMessage();

            byte[] signedTransaction = wallet.Sign(compiledTransaction);

            RequestResult<string> requestResult = await rpcClient.SendTransactionAsync(signedTransaction);

            return new ZebecResult(requestResult.Result);
        }
        
        public async Task<ZebecResult> ResumeStream(Account fromAccount, Account toAccount, Account tempStreamDepositAccount)
        {
            TransactionInstruction instruction = new()
            {
                Keys = new List<AccountMeta>()
                {
                    AccountMeta.Writable(fromAccount.PublicKey, true),
                    AccountMeta.Writable(toAccount.PublicKey, false),
                    AccountMeta.Writable(tempStreamDepositAccount.PublicKey, false),
                    AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                },
                ProgramId = ZebecProgramIdKey.KeyBytes,
                Data = new ResumeStream().Serialize(),
            };

            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();

            Transaction transaction = new()
            {
                RecentBlockHash = blockHash.Result.Value.Blockhash,
                FeePayer = fromAccount.PublicKey,
                Instructions = new List<TransactionInstruction>() { instruction }
            };

            byte[] compiledTransaction = transaction.CompileMessage();

            byte[] signedTransaction = wallet.Sign(compiledTransaction);

            RequestResult<string> requestResult = await rpcClient.SendTransactionAsync(signedTransaction);

            return new ZebecResult(requestResult.Result);
        }
    }
}
