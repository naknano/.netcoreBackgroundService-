namespace BakongHealthCheck.APIConnection
{
    public interface IMyHttpRequest
    {
        Task<TResponse> HttpReqstApiB24Async<TResponse>(string jsonRequest, string url, string urlPath, string keyAuth, CancellationToken cancellationToken);
    }
}
