
using Newtonsoft.Json;
using Serilog;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;

namespace BakongHealthCheck.APIConnection
{
    public class MyHttpRequest : BaseHttpClientWithFactory, IMyHttpRequest
    {
        private readonly HttpClient _httpClient;

        public MyHttpRequest(IHttpClientFactory factory, HttpClient httpClient) : base(factory)
        {
            _httpClient = httpClient;
        }

        public async Task<TResponse> HttpReqstApiB24Async<TResponse>(string jsonRequest, string url, string urlPath, string keyAuth , CancellationToken cancellationToken)
        {
            try
            {
                var message = new HttpRequestBuilder(url)
                                  .SetPath(urlPath)
                                  .HttpMethod(HttpMethod.Get)
                                  .GetHttpMessage();
                message.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                string result = await SendRequestStringAsync(message, cancellationToken);
                return JsonConvert.DeserializeObject<TResponse>(result);
            }
            catch (TaskCanceledException ex)
            {
                if (cancellationToken.IsCancellationRequested) Log.Information("Service request api call was explicitly cancelled (new request initiated) : " + ex.Message);
                else Log.Information("Service request api call timed out : " + ex.Message);
                throw new TaskCanceledException(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Information("Service request api catch error : " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> SendRequestStringAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }


    }
}
