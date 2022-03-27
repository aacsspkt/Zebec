using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zebec.Models;

namespace Zebec.Programs
{
    internal static class ZebecProgramData
    {
        /// <summary>
        /// The offset at which the value which defines the method begins.
        /// </summary>
        internal const int MethodOffset = 0;

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.InitializeSolStream"/> method.
        /// </summary>
        /// <param name="startTime">Start Time to stream represented by 64 bit unsigned integer.</param>
        /// <param name="endTime">End Time to stream represented by 64 bit unsigned integer.</param>
        /// <param name="amount">Amount of Lamports to stream represented by 64 bit unsigned integer.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeSolStreamData(ulong startTime, ulong endTime, ulong amount)
        {
            byte[] buffer = new byte[25];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.InitializeSolStream, MethodOffset);
            buffer.WriteU64(startTime, 1);
            buffer.WriteU64(endTime, 9);
            buffer.WriteU64(amount, 17);
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.WithdrawSolStream"/> method.
        /// </summary>
        /// <param name="amount">Amount of Lamports recieved from stream to withdraw .</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeWithdrawSolStreamData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.WithdrawSolStream, MethodOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.CancelSolStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeCancelSolStreamData() => new[] { (byte)ZebecProgramInstructions.Values.CancelSolStream };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.InitializeTokenStream"/> method.
        /// </summary>
        /// <param name="startTime">Start Time to stream represented by 64 bit unsigned integer.</param>
        /// <param name="endTime">End Time to stream represented by 64 bit unsigned integer.</param>
        /// <param name="amount">Amount of Tokens to stream represented by 64 bit unsigned integer.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeTokenStreamData(ulong startTime, ulong endTime, ulong amount)
        {
            byte[] buffer = new byte[25];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.InitializeTokenStream, MethodOffset);
            buffer.WriteU64(startTime, 1);
            buffer.WriteU64(endTime, 9);
            buffer.WriteU64(amount, 17);
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.PauseSolStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodePauseSolStreamData() => new[] { (byte)ZebecProgramInstructions.Values.PauseSolStream };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.ResumeSolStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeResumeSolStreamData() => new[] { (byte)ZebecProgramInstructions.Values.ResumeSolStream };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.WithdrawTokenStream"/> method.
        /// </summary>
        /// <param name="amount">Amount of Token recieved from stream to withdraw .</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeWithdrawTokenStreamData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.WithdrawTokenStream, MethodOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.DepositSol"/> method.
        /// </summary>
        /// <param name="amount">Amount of Lamports to deposit in Zebec Program</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeDepositSolData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.DepositSol, MethodOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.CancelTokenStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeCancelTokenStreamData() => new[] { (byte)ZebecProgramInstructions.Values.CancelTokenStream };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.PauseTokenStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodePauseTokenStreamData() => new[] { (byte)ZebecProgramInstructions.Values.PauseTokenStream };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.ResumeTokenStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeResumeTokenStreamData() => new[] { (byte)ZebecProgramInstructions.Values.ResumeTokenStream };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.DepositToken"/> method.
        /// </summary>
        /// <param name="amount">Amount of Tokens to deposit in Zebec Program</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeDepositTokenData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.DepositToken, MethodOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.FundSol"/> method.
        /// </summary>
        /// <param name="endTime">End time of stream for funded Sol</param>
        /// <param name="amount">Amount of Lamports to fund to ongoing stream</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeFundSolData(ulong endTime, ulong amount)
        {
            byte[] buffer = new byte[17];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.FundSol, MethodOffset);
            buffer.WriteU64(endTime, 1);
            buffer.WriteU64(amount, 9);
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.FundToken"/> method.
        /// </summary>
        /// <param name="endTime">End time of stream for funded Tokens</param>
        /// <param name="amount">Amount of Tokens to fund to ongoing stream</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeFundTokenData(ulong endTime, ulong amount)
        {
            byte[] buffer = new byte[17];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.FundToken, MethodOffset);
            buffer.WriteU64(endTime, 1);
            buffer.WriteU64(amount, 9);
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.WithdrawSol"/> method.
        /// </summary>
        /// <param name="amount">Amount of Sol to withdraw from Zebec Program.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeWithdrawSolData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.WithdrawSol, MethodOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.WithdrawToken"/> method.
        /// </summary>
        /// <param name="amount">Amount of Tokens to withdraw from Zebec Program.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeWithdrawTokenData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.WithdrawToken, MethodOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.CreateWhitelist"/> method.
        /// </summary>
        /// <param name="whiteLists">Array of <see cref="Zebec.Models.WhiteList"/> to create.</param>
        /// <param name="m">No. of signature required.</param>
        /// <param name="m">Address of Multisig Safe.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeCreateWhitelistData(WhiteList[] whiteLists, byte m, PublicKey multiSigSafeKey)
        {
            // require test
            int listLength = whiteLists.Count() * 33;
            byte[] buffer = new byte[listLength + 33];

            buffer.WriteU8((byte)ZebecProgramInstructions.Values.CreateWhitelist, MethodOffset);

            int listindex = 0;
            foreach (WhiteList whiteList in whiteLists)
            {
                byte[] buf = new byte[33];
                buf.WritePubKey(whiteList.Address, MethodOffset);
                buf.WriteU8(whiteList.Counter, 32);

                // for listindex = [0 .. whiteList.Count()]
                // (listindex * 33) + 1 => [1, 34, 67, 100, .... ((whiteList.Count() * 33) + 1)]
                // listindex is multiplied with 33 because length of a WhiteList is 33 byte and 1 is added after that
                // because whiteList offset begins from 1th index shifting each whiteList span 1 step right.
                buffer.WriteSpan(buf, (listindex * 33) + 1);
                listindex++;
            }

            buffer.WriteU8(m, MethodOffset + listLength);
            buffer.WritePubKey(multiSigSafeKey, MethodOffset + listLength + 1);
            
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.SwapSol"/> method.
        /// </summary>
        /// <param name="amount">Amount of Sol to swap</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeSwapSolData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.SwapSol, MethodOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.SwapToken"/> method.
        /// </summary>
        /// <param name="amount">Amount of Token to swap</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeSwapTokenData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.SwapToken, MethodOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.SignedBy"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeSignedByData() => new[] { (byte)ZebecProgramInstructions.Values.SignedBy };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values. InitializeMultisigSolStream"/> method.
        /// </summary>
        /// <param name="whiteLists">Todo</param>
        /// <param name="sender">Todo</param>
        /// <param name="recipient">Todo</param>
        /// <param name="multisigSafeKey">Todo</param>
        /// <param name="startTime">Todo</param>
        /// <param name="endTime">Todo</param>
        /// <param name="paused">Todo</param>
        /// <param name="withdrawLimit">Todo</param>
        /// <param name="amount">Todo</param>
        /// <param name="canCancel">Todo</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeMultiSigSolStreamData(
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
            // require test
            int listLength = whiteLists.Count() * 33;
            byte[] buffer = new byte[listLength + 137];

            buffer.WriteU8((byte)ZebecProgramInstructions.Values.InitializeMultisigSolStream, MethodOffset);
            buffer.WriteU64(startTime, 1);
            buffer.WriteU64(endTime, 9);
            buffer.WriteU64(paused, 17);
            buffer.WriteU64(withdrawLimit, 25);
            buffer.WriteU64(amount, 33);
            buffer.WritePubKey(sender, 41);
            buffer.WritePubKey(recipient, 73);

            int listindex = 0;
            foreach (WhiteList whiteList in whiteLists)
            {
                byte[] buf = new byte[33];
                buf.WritePubKey(whiteList.Address, MethodOffset);
                buf.WriteU8(whiteList.Counter, 32);

                // for listindex in [0 .. whiteList.Count()]
                // (listindex * 33) + 105 => [105, 138, 171, 204, .... ((whiteList.Count() * 33) + 105)]
                // listindex is multiplied with 33 because length of a WhiteList is 33 byte and 105 is added after that
                // because whiteList offset begins from 105th index shifting each whiteList span 105 step right.
                buffer.WriteSpan(buf, (listindex * 33) + 105);
                listindex++;
            }
            buffer.WritePubKey(multisigSafeKey, 105 + listLength);
            buffer.WriteBool(canCancel, 105 + listLength + 32);
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.WithdrawMultisigSolStream"/> method.
        /// </summary>
        /// <param name="amount">Amount of Sol recieved from Multisig stream to withdraw.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeWithdrawMultiSigSolStreamData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.WithdrawMultisigSolStream, MethodOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.CancelMultisigSolStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeCancelMultiSigSolStreamData() => new byte[] { (byte)ZebecProgramInstructions.Values.CancelMultisigSolStream };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.PauseMultisigSolStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodePauseMultiSigSolStreamData() => new byte[] { (byte)ZebecProgramInstructions.Values.PauseMultisigSolStream };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.ResumeMultisigSolStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeResumeMultiSigSolStreamData() => new byte[] { (byte)ZebecProgramInstructions.Values.ResumeMultisigSolStream };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.RejectMultisigSolStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeRejectMultiSigSolStreamData() => new byte[] { (byte)ZebecProgramInstructions.Values.RejectMultisigSolStream };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values. InitializeMultisigTokenStream"/> method.
        /// </summary>
        /// <param name="whiteLists">Todo</param>
        /// <param name="sender">Todo</param>
        /// <param name="recipient">Todo</param>
        /// <param name="multisigSafeKey">Todo</param>
        /// <param name="startTime">Todo</param>
        /// <param name="endTime">Todo</param>
        /// <param name="paused">Todo</param>
        /// <param name="widthdrawLimit">Todo</param>
        /// <param name="amount">Todo</param>
        /// <param name="canCancel">Todo</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeMultisigTokenStreamData(
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
            // require test
            int listLength = whiteLists.Count() * 33;
            byte[] buffer = new byte[listLength + 137];

            buffer.WriteU8((byte)ZebecProgramInstructions.Values.InitializeMultisigTokenStream, MethodOffset);
            buffer.WriteU64(startTime, 1);
            buffer.WriteU64(endTime, 9);
            buffer.WriteU64(paused, 17);
            buffer.WriteU64(withdrawLimit, 25);
            buffer.WriteU64(amount, 33);
            buffer.WritePubKey(sender, 41);
            buffer.WritePubKey(recipient, 73);
            buffer.WritePubKey(tokenMint, 105);

            int listindex = 0;
            foreach (WhiteList whiteList in whiteLists)
            {
                byte[] buf = new byte[33];
                buf.WritePubKey(whiteList.Address, MethodOffset);
                buf.WriteU8(whiteList.Counter, 32);

                // for listindex in [0 .. whiteList.Count()]
                // (listindex * 33) + 137 => [137, 170, 203, 236, .... ((whiteList.Count() * 33) + 137)]
                // listindex is multiplied with 33 because length of a WhiteList is 33 byte and 137 is added after that
                // because whiteList offset begins from 137th index shifting each whiteList span 137 step right.
                buffer.WriteSpan(buf, (listindex * 33) + 137);
                listindex++;
            }

            buffer.WritePubKey(multisigSafeKey, 137 + listLength);
            buffer.WriteBool(canCancel, 137 + listLength + 32);
            return buffer;
        }

        /// <summary>
        ///  Encode the transaction intruction data for the <see cref="ZebecProgramInstructions.Values.WithdrawMultisigTokenStream"/> method.
        /// </summary>
        /// <param name="amount">Amount of Tokens recieved from Multisig stream to withdraw.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] ProcessWithdrawMultisigTokenStreamData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.WithdrawMultisigTokenStream, MethodOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }


        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.CancelMultisigTokenStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeCancelMultiSigTokenStreamData() => new byte[] { (byte)ZebecProgramInstructions.Values.CancelMultisigTokenStream };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.PauseMultisigTokenStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodePauseMultiSigTokenStreamData() => new byte[] { (byte)ZebecProgramInstructions.Values.PauseMultisigTokenStream };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.ResumeMultisigTokenStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeResumeMultiSigTokenStreamData() => new byte[] { (byte)ZebecProgramInstructions.Values.ResumeMultisigTokenStream };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.RejectMultisigTokenStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeRejectMultiSigTokenStreamData() => new byte[] { (byte)ZebecProgramInstructions.Values.RejectMultisigTokenStream };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.SignedByToken"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeSignedByTokenData() => new[] { (byte)ZebecProgramInstructions.Values.SignedByToken };


        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.TransferSol"/> method.
        /// </summary>
        /// <param name="whiteLists"></param>
        /// <param name="sender"></param>
        /// <param name="recipient"></param>
        /// <param name="multisigSafeKey"></param>
        /// <param name="amount"></param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeSolTransferData(
            WhiteList[] whiteLists, 
            PublicKey sender, 
            PublicKey recipient, 
            PublicKey multisigSafeKey, 
            ulong amount)
        {

            // require test
            int listLength = whiteLists.Count() * 33;
            byte[] buffer = new byte[listLength + 104];

            buffer.WriteU8((byte)ZebecProgramInstructions.Values.TransferSol, MethodOffset);
            buffer.WritePubKey(sender, 1);
            buffer.WritePubKey(recipient, 33);

            int listindex = 0;
            foreach (WhiteList whiteList in whiteLists)
            {
                byte[] buf = new byte[33];
                buf.WritePubKey(whiteList.Address, MethodOffset);
                buf.WriteU8(whiteList.Counter, 32);

                // for listindex in [0 .. whiteList.Count()]
                // (listindex * 33) + 65 => [65, 98, 131, 164, .... ((whiteList.Count() * 33) + 65)]
                // listindex is multiplied with 33 because length of a WhiteList is 33 byte and 65 is added after that
                // because whiteList offset begins from 65th index shifting each whiteList span 65 step right.
                buffer.WriteSpan(buf, (listindex * 33) + 65);
                listindex++;
            }

            buffer.WritePubKey(multisigSafeKey, 65 + listLength);
            buffer.WriteU64(amount, 65 + listLength + 32);

            return buffer;
        }


        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.SignedByTransferSol"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeSignedByTransferSolData() => new[] { (byte)ZebecProgramInstructions.Values.SignedByTransferSol };


        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.TransferToken"/> method.
        /// </summary>
        /// <param name="whiteLists"></param>
        /// <param name="sender"></param>
        /// <param name="recipient"></param>
        /// <param name="multisigSafeKey"></param>
        /// <param name="amount"></param>
        /// <param name="tokenMint"></param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeTokenTransferData(
            WhiteList[] whiteLists, 
            PublicKey sender, 
            PublicKey recipient, 
            PublicKey multisigSafeKey, 
            ulong amount, 
            PublicKey tokenMint)
        {
            // require test
            int listLength = whiteLists.Count() * 33;
            byte[] buffer = new byte[listLength + 104];

            buffer.WriteU8((byte)ZebecProgramInstructions.Values.TransferToken, MethodOffset);
            buffer.WritePubKey(sender, 1);
            buffer.WritePubKey(recipient, 33);

            int listindex = 0;
            foreach (WhiteList whiteList in whiteLists)
            {
                byte[] buf = new byte[33];
                buf.WritePubKey(whiteList.Address, MethodOffset);
                buf.WriteU8(whiteList.Counter, 32);

                // for listindex in [0 .. whiteList.Count()]
                // (listindex * 33) + 65 => [65, 98, 131, 164, .... ((whiteList.Count() * 33) + 65)]
                // listindex is multiplied with 33 because length of a WhiteList is 33 byte and 65 is added after that
                // because whiteList offset begins from 65th index shifting each whiteList span 65 step right.
                buffer.WriteSpan(buf, (listindex * 33) + 65);
                listindex++;
            }

            buffer.WritePubKey(multisigSafeKey, 65 + listLength);
            buffer.WriteU64(amount, 65 + listLength + 32);
            buffer.WritePubKey(tokenMint, 65 + listLength + 40);

            return buffer;
        }


        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.SignedByTransferToken"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeSignedByTransferTokenData() => new [] { (byte)ZebecProgramInstructions.Values.SignedByTransferToken };


        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.RejectTransferSol"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeRejectTransferSolData() => new [] { (byte)ZebecProgramInstructions.Values.RejectTransferSol };


        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.RejectTransferToken"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeRejectTransferTokenData() => new [] { (byte)ZebecProgramInstructions.Values.RejectTransferToken };
    }
}
