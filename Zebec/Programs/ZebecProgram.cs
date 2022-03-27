using Solnet.Programs;
using Solnet.Programs.Utilities;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using Zebec.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zebec.Programs
{
    public static class ZebecProgram
    {
        /// <summary>
        /// The public key of the Zebec Program.
        /// </summary>
        public static readonly PublicKey ProgramIdKey = new("AknC341xog56SrnoK6j3mUvaD1Y7tYayx1sxUGpeYWdX");

        /// <summary>
        /// The public key of the Fee Account.
        /// </summary>
        public static readonly PublicKey FeeIdKey = new("EsDV3m3xUZ7g8QKa1kFdbZT18nNz8ddGJRcTK84WDQ7k");

        /// <summary>
        /// The program's name.
        /// </summary>
        private const string ProgramName = "Zebec Program";

        /// <summary>
        /// The prefix to create seed for withdraw pda.
        /// </summary>
        private const string WITHDRAW_PREFIX = "withdraw_sol";


        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account who initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="startTime">The timestamp from which stream is to be initialized.</param>
        /// <param name="endTime">The timestamp at which the stream ends.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <param name="streamDataAccount">The Account which holds the stream data such as
        /// start and end time, is paused or not, sender and recipient address, etc.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeSolStream(
            PublicKey sender, 
            PublicKey reciever, 
            ulong startTime, 
            ulong endTime, 
            ulong amount, 
            out Account streamDataAccount )
        {
            Debug.WriteLine(sender, nameof(sender));
            Debug.WriteLine(reciever, nameof(reciever));
            Debug.WriteLine(startTime.ToString(), nameof(startTime));
            Debug.WriteLine(endTime.ToString(), nameof(endTime));

            bool success = PublicKey.TryFindProgramAddress(
                new List<byte[]>() { Encoding.UTF8.GetBytes(WITHDRAW_PREFIX), sender.KeyBytes }, 
                ProgramIdKey, 
                out PublicKey withdrawDataPda, 
                out byte withdrawBump
                );
            Debug.WriteLineIf(success, withdrawDataPda.ToString(), nameof(withdrawDataPda));

            streamDataAccount = new Account();
            Debug.WriteLine(streamDataAccount, nameof(streamDataAccount));

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sender, true),
                AccountMeta.Writable(reciever, false),
                AccountMeta.Writable(streamDataAccount.PublicKey, true),
                AccountMeta.Writable(withdrawDataPda, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false)
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = ZebecProgramData.EncodeSolStreamData(startTime, endTime, amount)
            };
        }

        /// <summary>
        /// Initializes an instruction to initialize withdraw sol recieved from stream. Here the reciever is the signer.
        /// </summary>
        /// <param name="sender">The public key of the account from which sol stream was initialized.</param>
        /// <param name="reciever">The public key of the account to whom sol was streamed and who can withdraw streamed sol.</param>
        /// <param name="streamDataPda">The public key of the account which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction WithdrawStreamSol( 
            PublicKey sender, 
            PublicKey reciever, 
            PublicKey streamDataPda, 
            ulong amount)
        {
            Debug.WriteLine(sender, nameof(sender));
            Debug.WriteLine(reciever, nameof(reciever));
            Debug.WriteLine(streamDataPda, nameof(streamDataPda));

            bool success = PublicKey.TryFindProgramAddress(
                new List<byte[]>() { sender.KeyBytes },
                ProgramIdKey,
                out PublicKey depositPda,
                out byte depositBump
                );
            Debug.WriteLineIf(success, depositPda.ToString(), nameof(depositPda));

            bool anotherSuccess = PublicKey.TryFindProgramAddress(
                new List<byte[]>() { Encoding.UTF8.GetBytes(WITHDRAW_PREFIX), sender.KeyBytes },
                ProgramIdKey,
                out PublicKey withdrawDataPda,
                out byte withdrawBump
                );
            Debug.WriteLineIf(anotherSuccess, withdrawDataPda.ToString(), nameof(withdrawDataPda));

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sender, false),
                AccountMeta.Writable(reciever, true),
                AccountMeta.Writable(depositPda, false),
                AccountMeta.Writable(streamDataPda, false),
                AccountMeta.Writable(withdrawDataPda, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.Writable(FeeIdKey, false),
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = ZebecProgramData.EncodeWithdrawSolStreamData(amount)
            };
        }

        /// <summary>
        /// Initializes an instruction to cancel sol stream.
        /// </summary>
        /// <param name="sender">The public key of the account initialized sol stream.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="streamDataPda">The public key of the account which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction CancelSolStream(
            PublicKey sender,
            PublicKey reciever,
            PublicKey streamDataPda)
        {
            Debug.WriteLine(sender, nameof(sender));
            Debug.WriteLine(reciever, nameof(reciever));
            Debug.WriteLine(streamDataPda, nameof(streamDataPda));

            bool success = PublicKey.TryFindProgramAddress(
                new List<byte[]>() { sender.KeyBytes },
                ProgramIdKey,
                out PublicKey depositPda,
                out byte depositBump
                );
            Debug.WriteLineIf(success, depositPda.ToString(), nameof(depositPda));

            bool anotherSuccess = PublicKey.TryFindProgramAddress(
                new List<byte[]>() { Encoding.UTF8.GetBytes(WITHDRAW_PREFIX), sender.KeyBytes },
                ProgramIdKey,
                out PublicKey withdrawDataPda,
                out byte withdrawBump
                );
            Debug.WriteLineIf(anotherSuccess, withdrawDataPda.ToString(), nameof(withdrawDataPda));

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sender, true),
                AccountMeta.Writable(reciever, false),
                AccountMeta.Writable(depositPda, false),
                AccountMeta.Writable(streamDataPda, false),
                AccountMeta.Writable(withdrawDataPda, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.Writable(FeeIdKey, false),
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = ZebecProgramData.EncodeCancelSolStreamData()
            };
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="streamDataPda">The public key of the account which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeTokenStream(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to pause sol stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="streamDataPda">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction PauseSolStream(
            PublicKey sender,
            PublicKey reciever,
            PublicKey streamDataPda
            )
        {
            Debug.WriteLine(sender, nameof(sender));
            Debug.WriteLine(reciever, nameof(reciever));
            Debug.WriteLine(streamDataPda, nameof(streamDataPda));

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sender, true),
                AccountMeta.Writable(reciever, false),
                AccountMeta.Writable(streamDataPda, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = ZebecProgramData.EncodePauseSolStreamData()
            };
        }

        /// <summary>
        /// Initializes an instruction to resume sol stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="streamDataPda">The public key of the account which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction ResumeSolStream(
            PublicKey sender,
            PublicKey reciever,
            PublicKey streamDataPda
            )
        {
            Debug.WriteLine(sender, nameof(sender));
            Debug.WriteLine(reciever, nameof(reciever));
            Debug.WriteLine(streamDataPda, nameof(streamDataPda));

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sender, true),
                AccountMeta.Writable(reciever, false),
                AccountMeta.Writable(streamDataPda, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = ZebecProgramData.EncodeResumeSolStreamData()
            };
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction WithdrawTokenStream(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to deposit sol to Zebec Program.
        /// </summary>
        /// <param name="address">The public key of the account who is going to deposit Sol for streaming</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction DepositSol(PublicKey address, ulong amount)
        {
            Debug.WriteLine(address, nameof(address));

            bool success = PublicKey.TryFindProgramAddress(
                new List<byte[]>() { address.KeyBytes },
                ProgramIdKey,
                out PublicKey depositPda,
                out byte depositBump
                );
            Debug.WriteLineIf(success, depositPda.ToString(), nameof(depositPda));

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(address, true),
                AccountMeta.Writable(depositPda, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = ZebecProgramData.EncodeDepositSolData(amount)
            };
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction CancelTokenStream(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction PauseTokenStream(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction ResumeTokenStream(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction DepositToken(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction FundSol(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction FundToken(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to  withdraw sol that have beeen deposited in Zebec Program.
        /// </summary>
        /// <param name="address">The public key of the account who deposited in Zebec Program.</param>
        /// <param name="amount">The amount of tokens to transfer withdraw.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction WithdrawSol(PublicKey address, ulong amount)
        {
            Debug.WriteLine(address, nameof(address));

            bool success = PublicKey.TryFindProgramAddress(
                new List<byte[]>() { address.KeyBytes },
                ProgramIdKey,
                out PublicKey depositPda,
                out byte depositBump
                );
            Debug.WriteLineIf(success, depositPda.ToString(), nameof(depositPda));

            bool anotherSuccess = PublicKey.TryFindProgramAddress(
                new List<byte[]>() { Encoding.UTF8.GetBytes(WITHDRAW_PREFIX), address.KeyBytes },
                ProgramIdKey,
                out PublicKey withdrawDataPda,
                out byte withdrawBump
                );

            Debug.WriteLineIf(anotherSuccess, withdrawDataPda.ToString(), nameof(withdrawDataPda));

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(address, true),
                AccountMeta.Writable(depositPda, false),
                AccountMeta.Writable(withdrawDataPda, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = ZebecProgramData.EncodeWithdrawSolData(amount)
            };
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction WithdrawToken(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction CreateWhitelist(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction SwapSol(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction SwapToken(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction SignedBy(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeMultisigSolStream(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction WithdrawMultisigSolStream(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction CancelMultisigSolStream(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction PauseMultisigSolStream(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction ResumeMultisigSolStream(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction RejectMultisigSolStream(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeMultisigTokenStream(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction WithdrawMultisigTokenStream(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction CancelMultisigTokenStream(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction PauseMultisigTokenStream(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction ResumeMultisigTokenStream(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction RejectMultisigTokenStream(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction SignedByToken(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction TransferSol(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction SignedByTransferSol(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction TransferToken(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction SignedByTransferToken(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction RejectTransferSol(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction RejectTransferToken(PublicKey sender, ulong startTime, ulong endTime)
        {
            return new TransactionInstruction();
        }
    }
}
