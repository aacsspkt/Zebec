using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zebec.Programs
{
    internal class InstructionTypes
    {
        /// <summary>
        /// Represents the user-friendly names for the instruction types for the <see cref="ZebecProgram"/>.
        /// </summary>
        internal static readonly Dictionary<Values, string> Names = new()
        {
            { Values.InitializeSolStream, "Initialize Sol Stream" },
            { Values.WithdrawSolStream, "Withdraw Sol Stream" },
            { Values.CancelSolStream, "Cancel Sol Stream" },
            { Values.InitializeTokenStream, "Initialie Token Stream" },
            { Values.PauseSolStream, "Pause Sol Stream" },
            { Values.ResumeSolStream, "Resume Sol Stream" },
            { Values.WithdrawTokenStream, "Withdraw Token Stream" },
            { Values.DepositSol, "Deposit Sol" },
            { Values.CancelTokenStream, "Cancel Token Stream" },
            { Values.PauseTokenStream, "Pause Token Stream" },
            { Values.ResumeTokenStream, "Resume Token Stream" },
            { Values.DepositToken, "Deposit Token" },
            { Values.FundSol, "Fund Sol" },
            { Values.FundToken, "Fund Token" },
            { Values.WithdrawSol, "Withdraw Sol" },
            { Values.WithdrawToken, "Withdraw Token" },
            { Values.CreateWhitelist, "Create Whitelist Account" },
            { Values.SwapSol, "Swap Sol" },
            { Values.SwapToken, "Swap Token" },
            { Values.SignedBy, "Signed By"},
            { Values.InitializeMultisigSolStream, "Initialize Multisig Sol Stream" },
            { Values.WithdrawMultisigSolStream, "Withdraw Multisig Sol Stream" },
            { Values.CancelMultisigSolStream, "Cancel Multisig Sol Stream" },
            { Values.PauseMultisigSolStream, "Pause Multisig Sol Stream" },
            { Values.ResumeMultisigSolStream, "Resume Multisig Sol Stream" },
            { Values.RejectMultisigSolStream, "Reject Multisig Sol Stream" },
            { Values.InitializeMultisigTokenStream, "Initialize Multisig Token Stream"},
            { Values.WithdrawMultisigTokenStream, "Withdraw Multisig Token Stream" },
            { Values.CancelMultisigTokenStream, "Cancel Multisig Token Stream" },
            { Values.PauseMultisigTokenStream, "Pause Multisig Token Stream" },
            { Values.ResumeMultisigTokenStream, "Resume Multisig Token Stream" },
            { Values.RejectMultisigTokenStream, "Reject Multisig Token Stream" },
            { Values.SignedByToken, "Signed By Token" },
            { Values.TransferSol, "Transfer Sol" },
            { Values.SignedByTransferSol, "Signed By Transfer Sol" },
            { Values.TransferToken, "Transfer Token" },
            { Values.SignedByTransferToken,"Signed By Transfer Token" },
            { Values.RejectTransferSol,"Reject Transfer Sol" },
            { Values.RejectTransferToken, "Reject Transfer Token" },
            { Values.Set, "Set" },
            { Values.Execute, "Execute" },
        };

        /// <summary>
        /// Represents the instruction types for the <see cref="ZebecProgram"/>.
        /// </summary>
        internal enum Values : byte
        {
            InitializeSolStream = 0,
            WithdrawSolStream = 1,
            CancelSolStream = 2,
            InitializeTokenStream = 3,
            PauseSolStream = 4,
            ResumeSolStream = 5,
            WithdrawTokenStream = 6,
            DepositSol = 7,
            CancelTokenStream = 8,
            PauseTokenStream = 9,
            ResumeTokenStream = 10,
            DepositToken = 11,
            FundSol = 12,
            FundToken = 13,
            WithdrawSol = 14,
            WithdrawToken = 15,
            CreateWhitelist = 16,
            SwapSol = 17,
            SwapToken = 18,
            SignedBy = 19,
            InitializeMultisigSolStream = 20,
            WithdrawMultisigSolStream = 21,
            CancelMultisigSolStream = 22,
            PauseMultisigSolStream = 23,
            ResumeMultisigSolStream = 24,
            RejectMultisigSolStream = 25,
            InitializeMultisigTokenStream = 26,
            WithdrawMultisigTokenStream = 27,
            CancelMultisigTokenStream = 28,
            PauseMultisigTokenStream = 29,
            ResumeMultisigTokenStream = 30,
            RejectMultisigTokenStream = 31,
            SignedByToken = 32,
            TransferSol = 33,
            SignedByTransferSol = 34,
            TransferToken = 35,
            SignedByTransferToken = 36,
            RejectTransferSol = 37,
            RejectTransferToken = 38,
            Set = 39,
            Execute = 40
        }
    }
}
