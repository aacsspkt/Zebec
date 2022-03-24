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
        public RequestResult<string> Result { get; private set; }

        public PublicKey? StreamDataAddress { get; private set; }

        public ZebecResponse(RequestResult<string> result)
        {
            Result = result;
        }

        public ZebecResponse(RequestResult<string> result, PublicKey streamDepositAccount)
        {
            Result = result;
            StreamDataAddress = streamDepositAccount;
        }
    }
}
