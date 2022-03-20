using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zebec.Models
{
    public class ZebecResult
    {
        public string Result { get; private set; }

        public PublicKey? StreamDepositAccount { get; private set; }

        public ZebecResult(string transactionHash, PublicKey streamDepositAccount = null)
        {
            Result = transactionHash;
            if (StreamDepositAccount != null)
            {
                StreamDepositAccount = streamDepositAccount;
            }
        }
    }
}
