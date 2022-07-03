using Solnet.Rpc.Core.Http;
using Solnet.Wallet;
using Zebec.Models;

namespace Zebec.Utilities
{
    public class ResponseMaker
    {
        public static RequestResult<ZebecResponse> Make(RequestResult<string> requestResult, PublicKey? streamDataAddress = null)
        {
            return new RequestResult<ZebecResponse>()
            {
                ErrorData = requestResult.ErrorData,
                HttpStatusCode = requestResult.HttpStatusCode,
                Reason = requestResult.Reason,
                Result = streamDataAddress != null ?
                    new ZebecResponse(requestResult.Result, streamDataAddress) :
                    new ZebecResponse(requestResult.Result),
                ServerErrorCode = requestResult.ServerErrorCode,
                WasHttpRequestSuccessful = requestResult.WasHttpRequestSuccessful,
                WasRequestSuccessfullyHandled = requestResult.WasRequestSuccessfullyHandled,
            };
        }
    }
}
