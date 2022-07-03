using Solnet.Programs;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System.Diagnostics;
using System.Text;
using Zebec.Models;

namespace Zebec.Program
{
    public static class Instructions
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
        /// The prefix to create seed for multisig safe pda
        /// </summary>
        private const string MULTISIG_SAFE_PREFIX = "multisig_safe";

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account who initialize sol stream.</param>
        /// <param name="reciever">The public key of the account to account to which sol is streamed.</param>
        /// <param name="startTime">The timestamp from which stream is initialized.</param>
        /// <param name="endTime">The timestamp at which the stream ends.</param>
        /// <param name="amount">The amount of lamports to transfer via stream.</param>
        /// <param name="streamDataAccount">The Account which holds the stream data such as
        /// start and end time, is paused or not, sender and recipient address, etc.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeSolStream(
            PublicKey sender,
            PublicKey reciever,
            ulong startTime,
            ulong endTime,
            ulong amount,
            out Account streamDataAccount)
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
                Data = InstructionData.EncodeSolStreamData(startTime, endTime, amount)
            };
        }

        /// <summary>
        /// Initializes an instruction to initialize withdraw sol recieved from stream. Here the reciever is the signer.
        /// </summary>
        /// <param name="sender">The public key of the account who initialized sol stream.</param>
        /// <param name="reciever">The public key of the account to whom sol was streamed.</param>
        /// <param name="streamDataPda">The public key of the account which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <param name="amount">The amount of lamports to transfer via stream.</param>
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
                Data = InstructionData.EncodeWithdrawSolStreamData(amount)
            };
        }

        /// <summary>
        /// Initializes an instruction to cancel sol stream.
        /// </summary>
        /// <param name="sender">The public key of the account who initialized sol stream.</param>
        /// <param name="reciever">The public key of the account to which sol was streamed.</param>
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
                Data = InstructionData.EncodeCancelSolStreamData()
            };
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer token from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account who initialized token stream.</param>
        /// <param name="reciever">The public key of the account to which token is streamed.</param>
        /// <param name="token">The public key of the token which is streamed.</param>
        /// <param name="startTime">The timestamp from which stream is initialized.</param>
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
                AccountMeta.Writable(streamDataAccount.PublicKey, true),
                AccountMeta.Writable(withdrawDataPda, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(token, false)
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = InstructionData.EncodeTokenStreamData(startTime, endTime, amount)
            };
        }

        /// <summary>
        /// Initializes an instruction to pause sol stream.
        /// </summary>
        /// <param name="sender">The public key of the account who initialized sol stream from.</param>
        /// <param name="reciever">The public key of the account to which sol was streamed.</param>
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
                Data = InstructionData.EncodePauseSolStreamData()
            };
        }

        /// <summary>
        /// Initializes an instruction to resume sol stream.
        /// </summary>
        /// <param name="sender">The public key of the account who initialized sol stream from.</param>
        /// <param name="reciever">The public key of the account to which sol is streamed.</param>
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
                Data = InstructionData.EncodeResumeSolStreamData()
            };
        }

        /// <summary>
        /// Initializes an instruction to initialize withdraw tokens recieved from stream. Here the recipient
        /// is the signer.
        /// </summary>
        /// <param name="sender">The public key of the account from which token stream was initialized.</param>
        /// <param name="recipient">The public key of the account to which tokens was streamed.</param>
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
                Data = InstructionData.EncodeWithdrawTokenStreamData(amount)
            };
        }

        /// <summary>
        /// Initializes an instruction to deposit sol in Zebec Program.
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
                Data = InstructionData.EncodeDepositSolData(amount)
            };
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream. Todo*
        /// </summary>
        /// <param name="sender">The public key of the account who initialized token stream.</param>
        /// <param name="recipient">The public key of the account to account to which token was streamed.</param>
        /// <param name="token">The public key of the token.</param>
        /// <param name="streamDataPda">The public key of the account which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction CancelTokenStream(
            PublicKey sender,
            PublicKey recipient,
            PublicKey token,
            PublicKey streamDataPda)
        {
            Debug.WriteLine(sender, nameof(sender));
            Debug.WriteLine(recipient, nameof(recipient));
            Debug.WriteLine(token, nameof(token));
            Debug.WriteLine(streamDataPda, nameof(streamDataPda));

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
                AccountMeta.Writable(streamDataPda, false),
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
                Data = InstructionData.EncodeCancelTokenStreamData()
            };
        }

        /// <summary>
        /// Initializes an instruction to pause token stream.
        /// </summary>
        /// <param name="sender">The public key of the account who initialized token stream.</param>
        /// <param name="recipient">The public key of the account to which token was streamed.</param>
        /// <param name="streamDataPda">The public key of the account which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction PauseTokenStream(
            PublicKey sender,
            PublicKey recipient,
            PublicKey streamDataPda)
        {
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = new List<AccountMeta>
                {
                    AccountMeta.Writable(sender, true),
                    AccountMeta.Writable(recipient, false),
                    AccountMeta.Writable(streamDataPda, false),
                    AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false)
                },
                Data = InstructionData.EncodePauseTokenStreamData()
            };
        }

        /// <summary>
        /// Initializes an instruction to resume token stream.
        /// </summary>
        /// <param name="sender">The public key of the account who initialized sol stream.</param>
        /// <param name="recipient">The public key of the account to which sol was streamed.</param>
        /// <param name="streamDataPda">The public key of the account which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction ResumeTokenStream(
            PublicKey sender,
            PublicKey recipient,
            PublicKey streamDataPda)
        {
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = new List<AccountMeta>
                {
                    AccountMeta.Writable(sender, true),
                    AccountMeta.Writable(recipient, false),
                    AccountMeta.Writable(streamDataPda, false),
                    AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false)
                },
                Data = InstructionData.EncodeResumeTokenStreamData()
            };
        }

        /// <summary>
        /// Initializes an instruction to deposit tokens in Zebec Program.
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
                Data = InstructionData.EncodeDepositTokenData(amount)
            };
        }

        /// <summary>
        /// Initializes an instruction to fund sol to existing stream and extends its time.
        /// </summary>
        /// <param name="sender">The public key of the account who initialized sol stream.</param>
        /// <param name="streamDataPda">The public key of the account which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <param name="endTime">The timestamp at which the stream ends.</param>
        /// <param name="amount">The amount of lamports to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction FundSol(
            PublicKey sender,
            PublicKey streamDataPda,
            ulong endTime,
            ulong amount)
        {
            PublicKey withdrawDataPda = DeriveSolWithdrawDataAccount(sender);
            Debug.WriteLine(withdrawDataPda, nameof(withdrawDataPda));

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = new List<AccountMeta>
                {
                    AccountMeta.Writable(sender, true),
                    AccountMeta.Writable(streamDataPda, false),
                    AccountMeta.Writable(withdrawDataPda, false)
                },
                Data = InstructionData.EncodeFundSolData(endTime, amount)
            };
        }

        /// <summary>
        /// Initializes an instruction to initialize transfer sol from one account to another via stream.
        /// </summary>
        /// <param name="sender">The public key of the account who initialized token stream.</param>
        /// <param name="streamDataPda">The public key of the account which was return in 
        /// <see cref="ZebecResponse.StreamDataAddress"/> after stream was initialized.</param>
        /// <param name="token">The public key of the token which is streamed.</param>
        /// <param name="endTime">The timestamps at which the stream ends.</param>
        /// <param name="amount">The amount of tokens to transfer via stream.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction FundToken(
            PublicKey sender,
            PublicKey streamDataPda,
            PublicKey token,
            ulong endTime,
            ulong amount)
        {
            PublicKey withdrawDataPda = DeriveTokenWithdrawDataAccount(sender, token);
            Debug.WriteLine(withdrawDataPda, nameof(withdrawDataPda));

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = new List<AccountMeta>
                {
                    AccountMeta.Writable(sender, true),
                    AccountMeta.Writable(streamDataPda, false),
                    AccountMeta.Writable(withdrawDataPda, false)
                },
                Data = InstructionData.EncodeFundTokenData(endTime, amount)
            };
        }

        /// <summary>
        /// Initializes an instruction to withdraw sol that had been deposited in Zebec Program.
        /// </summary>
        /// <param name="address">The public key of the account who deposited in Zebec Program.</param>
        /// <param name="amount">The amount of lamports to withdraw.</param>
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
                Data = InstructionData.EncodeWithdrawSolData(amount)
            };
        }

        /// <summary>
        /// Initializes an instruction to withdraw token that had been deposited in Zebec Program..
        /// </summary>
        /// <param name="address">The public key of the account who deposited in Zebec Program.</param>
        /// <param name="token">The public key of the token which is streamed.</param>
        /// <param name="amount">The amount of tokens to withdraw.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction WithdrawToken(
            PublicKey address,
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
                Data = InstructionData.EncodeWithdrawTokenData(amount)
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
        public static TransactionInstruction SwapSol(
            PublicKey sender,
            ulong endTime)
        {
            Debug.WriteLine(sender, nameof(sender));

            PublicKey depositPda = DeriveDepositAccount(sender);
            Debug.WriteLine(depositPda, nameof(depositPda));

            PublicKey withdrawDataPda = DeriveSolWithdrawDataAccount(sender);
            Debug.WriteLine(withdrawDataPda, nameof(withdrawDataPda));

            // todo
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
        /// Derive a PublicKey owned by Zebec
        /// </summary>
        /// <param name="seeds"></param>
        /// <returns></returns>
        public static PublicKey DeriveZebecProgramAddress(IEnumerable<byte[]> seeds)
        {
            PublicKey.TryFindProgramAddress(seeds, ProgramIdKey, out PublicKey pda, out byte _);
            return pda;
        }

        /// <summary>
        /// Derive deposit account from an address owned by Zebec Program.
        /// </summary>
        /// <param name="owner">The PublicKey of the owner who deposits amount.</param>
        /// <returns></returns>
        public static PublicKey DeriveDepositAccount(PublicKey owner)
        {
            return DeriveZebecProgramAddress(new List<byte[]>() { owner.KeyBytes });
        }


        /// <summary>
        /// Derive withdraw data account from an address ouwned by Zebec Program.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static PublicKey DeriveSolWithdrawDataAccount(PublicKey owner)
        {
            return DeriveZebecProgramAddress(new List<byte[]>
            {
                Encoding.UTF8.GetBytes(WITHDRAW_PREFIX),
                owner.KeyBytes
            });
        }


        /// <summary>
        /// Derive withdraw data account of a token from an address owned by Zebec Program.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static PublicKey DeriveTokenWithdrawDataAccount(PublicKey owner, PublicKey token)
        {
            return DeriveZebecProgramAddress(new List<byte[]>
                {
                    Encoding.UTF8.GetBytes(WITHDRAW_TOKEN_PREFIX),
                    owner.KeyBytes,
                    token.KeyBytes
                });
        }

        /// <summary>
        /// Derive multisig data account from an address owned by Zebec Program.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static PublicKey DeriveMultisigDataAccount(PublicKey owner)
        {
            return DeriveZebecProgramAddress(new List<byte[]>
                {
                    Encoding.UTF8.GetBytes(MULTISIG_SAFE_PREFIX),
                    owner.KeyBytes
                });
        }
    }
}
