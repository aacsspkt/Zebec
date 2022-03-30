using Solnet.Rpc.Core.Http;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zebec.Models
{
    public class ZebecResponse
    {
        public string Signature { get; private set; }

        public PublicKey? StreamDataAddress { get; private set; }

        public ZebecResponse(string signature)
        {
            Signature = signature;
        }

        public ZebecResponse(string result, PublicKey streamDepositAccount)
        {
            Signature = result;
            StreamDataAddress = streamDepositAccount;
        }
    }
}
