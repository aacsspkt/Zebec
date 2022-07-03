using Solnet.Programs.Utilities;
using Solnet.Wallet;
using Zebec.Models;
using Zebec.Utilities;

namespace Zebec.Program
{
    public static class InstructionData
    {


        internal const int InitialOffset = 0;

        public static byte[] EncodeSolStreamData(ulong startTime, ulong endTime, ulong amount)
        {
            byte[] buffer = new byte[25];
            buffer.WriteU8((byte)InstructionTypes.Values.InitializeSolStream, InitialOffset);
            buffer.WriteU64(startTime, 1);
            buffer.WriteU64(endTime, 9);
            buffer.WriteU64(amount, 17);
            return buffer;
        }

        public static byte[] EncodeWithdrawSolStreamData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)InstructionTypes.Values.WithdrawSolStream, InitialOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        public static byte[] EncodeCancelSolStreamData() => new[] { (byte)InstructionTypes.Values.CancelSolStream };

        public static byte[] EncodeTokenStreamData(ulong startTime, ulong endTime, ulong amount)
        {
            byte[] buffer = new byte[25];
            buffer.WriteU8((byte)InstructionTypes.Values.InitializeTokenStream, InitialOffset);
            buffer.WriteU64(startTime, 1);
            buffer.WriteU64(endTime, 9);
            buffer.WriteU64(amount, 17);
            return buffer;
        }

        public static byte[] EncodePauseSolStreamData() => new[] { (byte)InstructionTypes.Values.PauseSolStream };

        public static byte[] EncodeResumeSolStreamData() => new[] { (byte)InstructionTypes.Values.ResumeSolStream };

        public static byte[] EncodeWithdrawTokenStreamData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)InstructionTypes.Values.WithdrawTokenStream, InitialOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        public static byte[] EncodeDepositSolData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)InstructionTypes.Values.DepositSol, InitialOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        public static byte[] EncodeCancelTokenStreamData() => new[] { (byte)InstructionTypes.Values.CancelTokenStream };

        public static byte[] EncodePauseTokenStreamData() => new[] { (byte)InstructionTypes.Values.PauseTokenStream };

        public static byte[] EncodeResumeTokenStreamData() => new[] { (byte)InstructionTypes.Values.ResumeTokenStream };

        public static byte[] EncodeDepositTokenData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)InstructionTypes.Values.DepositToken, InitialOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        public static byte[] EncodeFundSolData(ulong endTime, ulong amount)
        {
            byte[] buffer = new byte[17];
            buffer.WriteU8((byte)InstructionTypes.Values.FundSol, InitialOffset);
            buffer.WriteU64(endTime, 1);
            buffer.WriteU64(amount, 9);
            return buffer;
        }

        public static byte[] EncodeFundTokenData(ulong endTime, ulong amount)
        {
            byte[] buffer = new byte[17];
            buffer.WriteU8((byte)InstructionTypes.Values.FundToken, InitialOffset);
            buffer.WriteU64(endTime, 1);
            buffer.WriteU64(amount, 9);
            return buffer;
        }

        public static byte[] EncodeWithdrawSolData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)InstructionTypes.Values.WithdrawSol, InitialOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        public static byte[] EncodeWithdrawTokenData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)InstructionTypes.Values.WithdrawToken, InitialOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        public static byte[] EncodeCreateWhitelistData(WhiteList[] whiteLists, byte m, PublicKey multiSigSafeKey, ulong withdrawal)
        {
            byte[] buffer = new byte[(whiteLists.Length * 33) + 46];
            buffer.WriteU8((byte)InstructionTypes.Values.CreateWhitelist, InitialOffset);
            buffer.WriteWhiteLists(whiteLists, 1, out int endOffset);
            buffer.WriteU8(m, endOffset);
            buffer.WritePubKey(multiSigSafeKey, endOffset + 1);
            buffer.WriteU64(withdrawal, endOffset + 33);
            return buffer;
        }

        public static byte[] EncodeSwapSolData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)InstructionTypes.Values.SwapSol, InitialOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        public static byte[] EncodeSwapTokenData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)InstructionTypes.Values.SwapToken, InitialOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        public static byte[] EncodeSignedByData() => new[] { (byte)InstructionTypes.Values.SignedBy };

        public static byte[] EncodeMultiSigSolStreamData(
            WhiteList[] whiteLists,
            PublicKey sender,
            PublicKey recipient,
            PublicKey multisigSafeKey,
            ulong startTime,
            ulong endTime,
            ulong paused,
            ulong withdrawLimit,
            ulong amount,
            bool canCancel)
        {
            byte[] buffer = new byte[(whiteLists.Length * 33) + 142];
            buffer.WriteU8((byte)InstructionTypes.Values.InitializeMultisigSolStream, InitialOffset);
            buffer.WriteU64(startTime, 1);
            buffer.WriteU64(endTime, 9);
            buffer.WriteU64(paused, 17);
            buffer.WriteU64(withdrawLimit, 25);
            buffer.WriteU64(amount, 33);
            buffer.WritePubKey(sender, 41);
            buffer.WritePubKey(recipient, 73);
            buffer.WriteWhiteLists(whiteLists, 105, out int endOffset);
            buffer.WritePubKey(multisigSafeKey, endOffset);
            buffer.WriteBool(canCancel, endOffset + 32);
            return buffer;
        }

        public static byte[] EncodeWithdrawMultiSigSolStreamData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)InstructionTypes.Values.WithdrawMultisigSolStream, InitialOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        public static byte[] EncodeCancelMultiSigSolStreamData() => new byte[] { (byte)InstructionTypes.Values.CancelMultisigSolStream };

        public static byte[] EncodePauseMultiSigSolStreamData() => new byte[] { (byte)InstructionTypes.Values.PauseMultisigSolStream };

        public static byte[] EncodeResumeMultiSigSolStreamData() => new byte[] { (byte)InstructionTypes.Values.ResumeMultisigSolStream };

        public static byte[] EncodeRejectMultiSigSolStreamData() => new byte[] { (byte)InstructionTypes.Values.RejectMultisigSolStream };

        public static byte[] EncodeMultisigTokenStreamData(
            WhiteList[] whiteLists,
            PublicKey sender,
            PublicKey recipient,
            PublicKey tokenMint,
            ulong startTime,
            ulong endTime,
            ulong paused,
            ulong withdrawLimit,
            ulong amount,
            PublicKey multisigSafeKey,
            bool canCancel)
        {
            byte[] buffer = new byte[(whiteLists.Count() * 33) + 174];
            buffer.WriteU8((byte)InstructionTypes.Values.InitializeMultisigTokenStream, InitialOffset);
            buffer.WriteU64(startTime, 1);
            buffer.WriteU64(endTime, 9);
            buffer.WriteU64(paused, 17);
            buffer.WriteU64(withdrawLimit, 25);
            buffer.WriteU64(amount, 33);
            buffer.WritePubKey(sender, 41);
            buffer.WritePubKey(recipient, 73);
            buffer.WritePubKey(tokenMint, 105);
            buffer.WriteWhiteLists(whiteLists, 137, out int endOffset);
            buffer.WritePubKey(multisigSafeKey, endOffset);
            buffer.WriteBool(canCancel, endOffset + 32);
            return buffer;
        }

        public static byte[] ProcessWithdrawMultisigTokenStreamData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)InstructionTypes.Values.WithdrawMultisigTokenStream, InitialOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        public static byte[] EncodeCancelMultiSigTokenStreamData() => new byte[] { (byte)InstructionTypes.Values.CancelMultisigTokenStream };

        public static byte[] EncodePauseMultiSigTokenStreamData() => new byte[] { (byte)InstructionTypes.Values.PauseMultisigTokenStream };

        public static byte[] EncodeResumeMultiSigTokenStreamData() => new byte[] { (byte)InstructionTypes.Values.ResumeMultisigTokenStream };

        public static byte[] EncodeRejectMultiSigTokenStreamData() => new byte[] { (byte)InstructionTypes.Values.RejectMultisigTokenStream };

        public static byte[] EncodeSignedByTokenData() => new[] { (byte)InstructionTypes.Values.SignedByToken };

        public static byte[] EncodeSolTransferData(
            WhiteList[] whiteLists,
            PublicKey sender,
            PublicKey recipient,
            PublicKey multisigSafeKey,
            ulong amount)
        {
            byte[] buffer = new byte[(whiteLists.Count() * 33) + 109];
            buffer.WriteU8((byte)InstructionTypes.Values.TransferSol, InitialOffset);
            buffer.WritePubKey(sender, 1);
            buffer.WritePubKey(recipient, 33);
            buffer.WriteWhiteLists(whiteLists, 65, out int endOffset);
            buffer.WritePubKey(multisigSafeKey, endOffset);
            buffer.WriteU64(amount, endOffset + 32);
            return buffer;
        }

        public static byte[] EncodeSignedByTransferSolData() => new[] { (byte)InstructionTypes.Values.SignedByTransferSol };

        public static byte[] EncodeTokenTransferData(
            WhiteList[] whiteLists,
            PublicKey sender,
            PublicKey recipient,
            PublicKey multisigSafeKey,
            ulong amount,
            PublicKey tokenMint)
        {
            byte[] buffer = new byte[(whiteLists.Count() * 33) + 141];
            buffer.WriteU8((byte)InstructionTypes.Values.TransferToken, InitialOffset);
            buffer.WritePubKey(sender, 1);
            buffer.WritePubKey(recipient, 33);
            buffer.WriteWhiteLists(whiteLists, 65, out int endOffset);
            buffer.WritePubKey(multisigSafeKey, endOffset);
            buffer.WriteU64(amount, endOffset + 8);
            buffer.WritePubKey(tokenMint, endOffset + 40);
            return buffer;
        }

        public static byte[] EncodeSignedByTransferTokenData() => new[] { (byte)InstructionTypes.Values.SignedByTransferToken };

        public static byte[] EncodeRejectTransferSolData() => new[] { (byte)InstructionTypes.Values.RejectTransferSol };

        public static byte[] EncodeRejectTransferTokenData() => new[] { (byte)InstructionTypes.Values.RejectTransferToken };

        public static byte[] EndcodeSetData(ulong number)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)InstructionTypes.Values.Set, InitialOffset);
            buffer.WriteU64(number, 1);
            return buffer;
        }

        public static byte[] EncodeExecuteData() => new[] { (byte)InstructionTypes.Values.Execute };
    }
}
