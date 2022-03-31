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
        public const string ProgramName = "Zebec Program";

        /// <summary>
        /// The prefix to create seed for withdraw pda.
        /// </summary>
        private const string WITHDRAW_PREFIX = "withdraw_sol";

        /// <summary>
        /// The prefix to create seed for withdraw pda for multi-tokens
        /// </summary>  
        private const string WITHDRAW_TOKEN_PREFIX = "withdraw_token";



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

            PublicKey withdrawDataPda = DeriveSolWithdrawDataAccount(sender);
            Debug.WriteLine(withdrawDataPda.ToString(), nameof(withdrawDataPda));

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

            PublicKey depositPda = DeriveDepositAccount(sender);
            Debug.WriteLine(depositPda.ToString(), nameof(depositPda));

            PublicKey withdrawDataPda = DeriveSolWithdrawDataAccount(sender);
            Debug.WriteLine(withdrawDataPda.ToString(), nameof(withdrawDataPda));

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

            PublicKey depositPda = DeriveDepositAccount(sender);
            Debug.WriteLine(depositPda.ToString(), nameof(depositPda));

            PublicKey withdrawDataPda = DeriveSolWithdrawDataAccount(sender);
            Debug.WriteLine(withdrawDataPda.ToString(), nameof(withdrawDataPda));

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
        /// <param name="sender">The public key of the account to initialize token stream from.</param>
        /// <param name="reciever">The public key of the account to which token is streamed.</param>
        /// <param name="token">The public key of the token which is streamed.</param>
        /// <param name="startTime">The timestamp from which stream is to be initialized.</param>
        /// <param name="endTime">The timestamp at which the stream ends.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <param name="streamDataAccount">The Account which holds the stream data such as
        /// start and end time, is paused or not, sender and recipient address, etc.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeTokenStream(
            PublicKey sender, 
            PublicKey reciever,
            PublicKey token,
            ulong startTime,
            ulong endTime,
            ulong amount,
            out Account streamDataAccount)
        {
            Debug.WriteLine(sender, nameof(sender));
            Debug.WriteLine(reciever, nameof(reciever));
            Debug.WriteLine(token, nameof(token));
            Debug.WriteLine(startTime.ToString(), nameof(startTime));
            Debug.WriteLine(endTime.ToString(), nameof(endTime));

            PublicKey withdrawDataPda = DeriveTokenWithdrawDataAccount(sender, token);
            Debug.WriteLine(withdrawDataPda.ToString(), nameof(withdrawDataPda));

            streamDataAccount = new Account();
            Debug.WriteLine(streamDataAccount, nameof(streamDataAccount));

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sender, true),
                AccountMeta.Writable(reciever, false),
                AccountMeta.Writable(streamDataAccount.PublicKey, false),
                AccountMeta.Writable(withdrawDataPda, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(token, false)
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = ZebecProgramData.EncodeTokenStreamData(startTime, endTime, amount)
            };
        }

        /// <summary>
        /// Initializes an instruction to pause sol stream.
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="streamDataPda">The public key of the stream data account which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
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
        /// <param name="streamDataPda">The public key of the stream data account which was return in 
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
        /// <param name="sender">The public key of the account who streamed tokens.</param>
        /// <param name="recipient">The public key of the account who withdraws tokens.</param>
        /// <param name="token">The public key of the token.</param>
        /// <param name="streamDataPda">The public key of the account which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction WithdrawTokenStream(
            PublicKey sender, 
            PublicKey recipient,
            PublicKey token,
            PublicKey streamDataPda,
            ulong amount)
        {
            Debug.WriteLine(sender, nameof(sender));
            Debug.WriteLine(recipient, nameof(recipient));
            Debug.WriteLine(token, nameof(token));

            PublicKey depositPda = DeriveDepositAccount(sender);
            Debug.WriteLine(depositPda, nameof(depositPda));

            PublicKey recipientAta = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(recipient, token);
            Debug.WriteLine(recipientAta, nameof(recipientAta));

            PublicKey depositAta = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(depositPda, token);
            Debug.WriteLine(depositAta, nameof(depositAta));

            PublicKey feeAta = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(FeeIdKey, token);
            Debug.WriteLine(feeAta, nameof(feeAta));

            PublicKey withdrawDataPda = DeriveTokenWithdrawDataAccount(sender, token);
            Debug.WriteLine(withdrawDataPda, nameof(withdrawDataPda));

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sender, false),
                AccountMeta.Writable(recipient, true),
                AccountMeta.Writable(depositPda, false),
                AccountMeta.Writable(streamDataPda, false),
                AccountMeta.Writable(withdrawDataPda, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
                AccountMeta.Writable(token, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false),
                AccountMeta.Writable(depositAta, false),
                AccountMeta.Writable(recipientAta, false),
                AccountMeta.ReadOnly(AssociatedTokenAccountProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.Writable(FeeIdKey, false),
                AccountMeta.Writable(feeAta, false),
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = ZebecProgramData.EncodeWithdrawTokenStreamData(amount)
            };
        }

        /// <summary>
        /// Initializes an instruction to deposit sol to Zebec Program.
        /// </summary>
        /// <param name="address">The public key of the account who deposits Sol for streaming</param>
        /// <param name="amount">The amount of tokens to deposit.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction DepositSol(PublicKey address, ulong amount)
        {
            Debug.WriteLine(address, nameof(address));

            PublicKey depositPda = DeriveDepositAccount(address);
            Debug.WriteLine(depositPda.ToString(), nameof(depositPda));

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
        /// Initializes an instruction to initialize transfer sol from one account to another via stream. Todo*
        /// </summary>
        /// <param name="sender">The public key of the account to initialize sol stream from.</param>
        /// <param name="recipient">The public key of the account to account to which sol is streamed.</param>
        /// <param name="token">The public key of the account to account to which sol is streamed.</param>
        /// <param name="streamDataAccount">The public key of the account to account to which sol is streamed.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction CancelTokenStream(
            PublicKey sender,
            PublicKey recipient, 
            PublicKey token,
            PublicKey streamDataAccount)
        {
            Debug.WriteLine(sender, nameof(sender));
            Debug.WriteLine(recipient, nameof(recipient));
            Debug.WriteLine(token, nameof(token));
            Debug.WriteLine(streamDataAccount, nameof(streamDataAccount));

            PublicKey depositPda = DeriveDepositAccount(sender);
            Debug.WriteLine(depositPda.ToString(), nameof(depositPda));

            PublicKey withdrawDataPda = DeriveTokenWithdrawDataAccount(sender, token);
            Debug.WriteLine(withdrawDataPda.ToString(), nameof(withdrawDataPda));

            PublicKey recipientAta = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(recipient, token);
            Debug.WriteLine(recipientAta.ToString(), nameof(recipientAta));

            PublicKey depositAta = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(depositPda, token);
            Debug.WriteLine(depositAta.ToString(), nameof(depositAta));

            PublicKey feeAta = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(FeeIdKey, token);
            Debug.WriteLine(feeAta.ToString(), nameof(feeAta));

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sender, true),
                AccountMeta.Writable(recipient, false),
                AccountMeta.Writable(depositPda, true),
                AccountMeta.Writable(streamDataAccount, false),
                AccountMeta.Writable(withdrawDataPda, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
                AccountMeta.Writable(token, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false),
                AccountMeta.Writable(recipientAta, false),
                AccountMeta.Writable(depositAta, false),
                AccountMeta.ReadOnly(AssociatedTokenAccountProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.Writable(FeeIdKey, false),
                AccountMeta.Writable(feeAta, false),
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = ZebecProgramData.EncodeCancelTokenStreamData()
            };
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
        /// <param name="sender">The public key of the account who deposits token for streaming.</param>
        /// <param name="token">The public key of the token which is streamed.</param>
        /// <param name="amount">The amount of tokens to deposit.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction DepositToken(
            PublicKey sender, 
            PublicKey token, 
            ulong amount)
        {
            Debug.WriteLine(sender, nameof(sender));
            Debug.WriteLine(token, nameof(token));

            PublicKey depositPda = DeriveDepositAccount(sender);
            Debug.WriteLine(depositPda.ToString(), nameof(depositPda));

            PublicKey senderAta = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(sender, token);
            Debug.WriteLine(senderAta, nameof(senderAta));

            PublicKey depositAta = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(depositPda, token);
            Debug.WriteLine(depositAta, nameof(depositAta));

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sender, true),
                AccountMeta.ReadOnly(depositPda, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
                AccountMeta.Writable(token, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false),
                AccountMeta.Writable(senderAta, false),
                AccountMeta.Writable(depositAta, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(AssociatedTokenAccountProgram.ProgramIdKey, false),
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = ZebecProgramData.EncodeDepositTokenData(amount)
            };
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

            PublicKey depositPda = DeriveDepositAccount(address);
            Debug.WriteLine(depositPda.ToString(), nameof(depositPda));

            PublicKey withdrawDataPda = DeriveSolWithdrawDataAccount(address);
            Debug.WriteLine(withdrawDataPda.ToString(), nameof(withdrawDataPda));

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
        /// <param name="address">The public key of the account to initialize sol stream from.</param>
        /// <param name="recipient">The public key of the account to account to which sol is streamed.</param>
        /// <param name="token">The public key of the token which is streamed.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction WithdrawToken(
            PublicKey address, 
            PublicKey recipient, 
            PublicKey token, 
            ulong amount)
        {
            PublicKey depositPda = DeriveDepositAccount(address);
            Debug.WriteLine(depositPda.ToString(), nameof(depositPda));

            PublicKey withdrawDataPda = DeriveTokenWithdrawDataAccount(address, token);
            Debug.WriteLine(withdrawDataPda.ToString(), nameof(withdrawDataPda));

            PublicKey senderAta = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(address, token);
            Debug.WriteLine(senderAta, nameof(senderAta));

            PublicKey depositAta = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(depositPda, token);
            Debug.WriteLine(depositAta, nameof(depositAta));

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(address, true),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
                AccountMeta.Writable(token, false),
                AccountMeta.Writable(senderAta, false),
                AccountMeta.Writable(depositPda, false),
                AccountMeta.Writable(withdrawDataPda, false),
                AccountMeta.Writable(depositAta, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false)
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = ZebecProgramData.EncodeWithdrawTokenData(amount)
            };
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

        /// <summary>
        /// Derive a PublicKey 
        /// </summary>
        /// <param name="seeds"></param>
        /// <returns></returns>
        public static PublicKey DeriveZebecProgramAddress(IEnumerable<byte[]> seeds)
        {
            PublicKey.TryFindProgramAddress(seeds, ProgramIdKey, out PublicKey pda, out byte _);
            return pda;
        }

        public static PublicKey DeriveDepositAccount(PublicKey owner)
        {
            PublicKey.TryFindProgramAddress(
                new List<byte[]>(){ owner.KeyBytes }, 
                ProgramIdKey, 
                out PublicKey address, 
                out byte _);
            return address;
        }

        public static PublicKey DeriveSolWithdrawDataAccount(PublicKey owner)
        {
            PublicKey.TryFindProgramAddress(new List<byte[]> 
            { 
                Encoding.UTF8.GetBytes(WITHDRAW_PREFIX), 
                owner.KeyBytes 
            }, 
            ProgramIdKey, 
            out PublicKey address, 
            out byte _);
            return address;
        }
        
        public static PublicKey DeriveTokenWithdrawDataAccount(PublicKey owner, PublicKey token)
        {
            PublicKey.TryFindProgramAddress(
                new List<byte[]> 
                { 
                    Encoding.UTF8.GetBytes(WITHDRAW_TOKEN_PREFIX), 
                    owner.KeyBytes,
                    token.KeyBytes
                }, 
                ProgramIdKey, 
                out PublicKey address, 
                out byte _);
            return address;
        }
    }
}
