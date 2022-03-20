using Solnet.Programs.Utilities;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Zebec.Models
{
    [Serializable]
    internal class InitStream: ISerializable
    {
        public byte Instruction { get => 0; }
        public ulong AmountInLamport { get; init; }
        public ulong StartTime { get; init; }
        public ulong EndTime { get; init; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Instruction), Instruction);
            info.AddValue(nameof(AmountInLamport), AmountInLamport);
            info.AddValue(nameof(StartTime), StartTime);
            info.AddValue(nameof(EndTime), EndTime);
        }

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
            info.AddValue(nameof(Instruction), Instruction);
            info.AddValue(nameof(AmountInLamport), AmountInLamport);
        }

        public byte[] Serialize()
        {
            var byteArray = new byte[9];
            byteArray.WriteU8(Instruction, 0);
            byteArray.WriteU64(AmountInLamport, 1);
            return byteArray;
        }
    }

    [Serializable]
    internal class CancelStream : ISerializable
    {
        public byte Instruction { get => 2; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Instruction), Instruction);
        }

        public byte[] Serialize()
        {
            var byteArray = new byte[1];
            byteArray.WriteU8(Instruction, 0);
            return byteArray;
        }
    }

    [Serializable]
    internal class PauseStream: ISerializable
    {
        public byte Instruction { get => 4; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Instruction), Instruction);
        }

        public byte[] Serialize()
        {
            var byteArray = new byte[1];
            byteArray.WriteU8(Instruction, 0);
            return byteArray;
        }
    }

    [Serializable]
    internal class ResumeStream: ISerializable
    {
        public byte Instruction { get => 5; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Instruction), Instruction);
        }

        public byte[] Serialize()
        {
            var byteArray = new byte[1];
            byteArray.WriteU8(Instruction, 0);
            return byteArray;
        }
    }

    [Serializable]
    internal class WithdrawStreamed: ISerializable
    {
        public byte Instruction { get => 1; }
        public ulong AmountInLamport { get; init; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Instruction), Instruction);
            info.AddValue(nameof(AmountInLamport), AmountInLamport);
        }

        public byte[] Serialize()
        {
            var byteArray = new byte[9];
            byteArray.WriteU8(Instruction, 0);
            byteArray.WriteU64(AmountInLamport, 1);
            return byteArray;
        }
    }

    [Serializable]
    internal class WithdrawDeposit: ISerializable
    {
        public byte Instruction { get => 14; }
        public ulong AmountInLamport { get; init; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Instruction), Instruction);
            info.AddValue(nameof(AmountInLamport), AmountInLamport);
        }

        public byte[] Serialize()
        {
            var byteArray = new byte[9];
            byteArray.WriteU8(Instruction, 0);
            byteArray.WriteU64(AmountInLamport, 1);
            return byteArray;
        }
    }
}
