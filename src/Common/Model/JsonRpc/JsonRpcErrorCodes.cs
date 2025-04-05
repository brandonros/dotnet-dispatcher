namespace Common.Model.JsonRpc;

// Define standard JSON-RPC error codes in a separate class for clarity
public static class JsonRpcErrorCodes
{
    public const int ParseError = -32700;
    public const int InvalidRequest = -32600;
    public const int MethodNotFound = -32601;
    public const int InvalidParams = -32602;
    public const int InternalError = -32603;
    public const int ServerError = -32000; // Used for timeouts and other server-specific errors
}
