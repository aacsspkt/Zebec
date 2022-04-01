using Solnet.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zebec.Clients.Streams
{
    public class MultiToken
    {
        /// <summary>
        /// The rpc client that communicates with solana blockchain.
        /// </summary>
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.DevNet);
    }
}
