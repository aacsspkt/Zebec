using Solnet.Programs.Utilities;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Zebec.Models
{
    internal class Stream
    {
        public byte Instruction { get => 0; }
        public ulong AmountInLamport { get; init; }
        public ulong StartTime { get; init; }
        public ulong EndTime { get; init; }

        public byte[] Serialize()
        {
            var byteArray = new byte[25];
            byteArray.WriteU8(Instruction, 0);
            byteArray.WriteU64(AmountInLamport, 1);
            byteArray.WriteU64(StartTime, 2);
            byteArray.WriteU64(EndTime, 3);
            return byteArray;
        }
    }

    [Serializable]
    internal class Deposit : ISerializable 
    {
        public byte Instruction { get => 7; }
        public ulong AmountInLamport { get; init; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Instruction", Instruction, typeof(byte));
            info.AddValue("AmountInLamport", AmountInLamport, typeof(ulong));
        }

        public byte[] Serialize()
        {
            //if(this == null) return null;

            //BinaryFormatter binaryFormatter = new BinaryFormatter();
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    binaryFormatter.Serialize(ms, this);
            //}
            var byteArray = new byte[9];
            byteArray.WriteU8(Instruction, 0);
            byteArray.WriteU64(AmountInLamport, 1);
            return byteArray;
        }
    }

    internal class Cancel
    {
        public byte Instruction { get => 2; }

        public byte[] Serialize()
        {
            var byteArray = new byte[1];
            byteArray.WriteU8(Instruction, 0);
            return byteArray;
        }
    }

    internal class Pause
    {
        public byte Instruction { get => 4; }

        public byte[] Serialize()
        {
            var byteArray = new byte[1];
            byteArray.WriteU8(Instruction, 0);
            return byteArray;
        }
    }

    internal class Resume
    {
        public byte Instruction { get => 5; }

        public byte[] Serialize()
        {
            var byteArray = new byte[1];
            byteArray.WriteU8(Instruction, 0);
            return byteArray;
        }
    }

    internal class WithdrawStreamed
    {
        public byte Instruction { get => 1; }
        public ulong AmountInLamport { get; init; }

        public byte[] Serialize()
        {
            var byteArray = new byte[2];
            byteArray.WriteU8(Instruction, 0);
            byteArray.WriteU64(AmountInLamport, 1);
            return byteArray;
        }
    }

    internal class WithdrawMainWallet
    {
        public byte Instruction { get => 14; }
        public ulong AmountInLamport { get; init; }

        public byte[] Serialize()
        {
            var byteArray = new byte[2];
            byteArray.WriteU8(Instruction, 0);
            byteArray.WriteU64(AmountInLamport, 1);
            return byteArray;
        }
    }
}
