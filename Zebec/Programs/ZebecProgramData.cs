using Solnet.Programs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zebec.Programs
{
    internal static class ZebecProgramData
    {
        /// <summary>
        /// The offset at which the value which defines the method begins.
        /// </summary>
        internal const int MethodOffset = 0;

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.DepositSol"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeDepositSolData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.DepositSol, MethodOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.WithdrawSol"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeWithdrawSolData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.WithdrawSol, MethodOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.InitializeSolStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeInitializeSolStreamData(ulong startTime, ulong endTime, ulong amount)
        {
            byte[] buffer = new byte[25];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.InitializeSolStream, MethodOffset);
            buffer.WriteU64(startTime, 1);
            buffer.WriteU64(endTime, 9);
            buffer.WriteU64(amount, 17);
            return buffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.CancelSolStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeCancelSolStreamData() => new[] { (byte)ZebecProgramInstructions.Values.CancelSolStream };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.PauseSolStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodePauseSolStreamData() => new[] { (byte)ZebecProgramInstructions.Values.PauseSolStream };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.ResumeSolStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeResumeSolStreamData() => new[] {(byte)ZebecProgramInstructions.Values.ResumeSolStream };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="ZebecProgramInstructions.Values.WithdrawSolStream"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeWithdrawSolStreamData(ulong amount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)ZebecProgramInstructions.Values.WithdrawSolStream, MethodOffset);
            buffer.WriteU64(amount, 1);
            return buffer;
        }
    }
}
