using Solnet.Wallet;

namespace Zebec.Models
{
    public struct WhiteList
    {
        public PublicKey Address { get; set; }
        public byte Counter { get; set; }
    }
}