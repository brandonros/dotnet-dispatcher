using Common.Model.JsonRpc;
using Common.Model.Requests;

namespace Common.Model.Requests;

/// <summary>
/// Add GetAccountJsonRpcRequest type
/// </summary>
public class GetAccountJsonRpcRequest : JsonRpcRequest<GetAccountRequest>
{
    public GetAccountJsonRpcRequest()
    {
        Method = JsonRpcMethod.GetAccount;
    }
}
