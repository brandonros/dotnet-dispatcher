using Common.Model.JsonRpc;
using Common.Model.Requests;

namespace Common.Model.Requests;

/// <summary>
/// Method-specific request types for better type safety and discoverability
/// </summary>
public class GetUserJsonRpcRequest : JsonRpcRequest<GetUserRequest>
{
    public GetUserJsonRpcRequest()
    {
        Method = JsonRpcMethod.GetUser;
    }
}
