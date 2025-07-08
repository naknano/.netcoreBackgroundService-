using Serilog;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;

namespace BakongHealthCheck.APIConnection
{
    public abstract class BaseHttpClientWithFactory
    {
        private readonly IHttpClientFactory _factory;
        public Uri BaseAddress { get; set; }
        public string BasePath { get; set; }
        public BaseHttpClientWithFactory(IHttpClientFactory factory)
        => _factory = factory;
        private HttpClient GetHttpClient()
        {
            return _factory.CreateClient();
        }
    
        public virtual async Task<T> SendRequest<T>(HttpRequestMessage request)
        where T : class
        {
            var client = GetHttpClient();
            T result = null;
            try
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    Log.Debug("Get or Post API Success ," + response);
                    result = await response.Content.ReadFromJsonAsync<T>();
                }
            }
            catch (HttpRequestException ex)
            {
                Log.Information(ex, "API Request Failed: {Message}", ex.Message);
                throw;
            }
            catch (System.Text.Json.JsonException ex)
            {
                Log.Information("JSON Deserialization Failed: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Log.Information(ex, "An unexpected error occurred during API request: {Message}", ex.Message);
                throw;
            }
            return result;
        }
        public virtual async Task<string> SendRequestStringAsync(HttpRequestMessage request)
        {
            string result = null;
            var client = GetHttpClient();
            try
            {
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    Log.Information("API Request Succeeded. Request: {RequestMessage}", response.RequestMessage?.ToString());
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    result = "EWB-" + errorContent;
                    Log.Information("API Request Failed. Response: {Response}, Status Code: {StatusCode}, Content: {Content}",
                                       response.ToString(), response.StatusCode, errorContent);
                }
            }
            catch (HttpRequestException ex)
            {
                Log.Error(ex, "HTTP Request Exception: {Message}", ex.Message);
                result = "EWB-" + ex.Message;
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error occurred during string request: {Message}", ex.Message);
                result = "EWB-" + ex.Message;
                throw;
            }

            return result;
        }
        public virtual async Task<string> PostRequestAsync(HttpContent content, HttpRequestMessage message)
        {
            string result = null;
            var client = GetHttpClient();
            var postRequest = new HttpRequestMessage(HttpMethod.Post, message.RequestUri);
            postRequest.Content = content;
            if (message.Headers.Authorization != null)
            {
                postRequest.Headers.Authorization = message.Headers.Authorization;
            }
            try
            {
                var response = await client.SendAsync(postRequest);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    Log.Information("API POST Request Succeeded. Request: {RequestUri}", postRequest.RequestUri);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    result = "EWB-" + errorContent;

                    Log.Information("API POST Request Failed. Response Status: {StatusCode}, Content: {Content}, Request: {RequestUri}",
                                      response.StatusCode, errorContent, postRequest.RequestUri);
                }
            }
            catch (HttpRequestException ex)
            {
                Log.Error(ex, "HTTP Request Exception during POST to {RequestUri}: {Message}",
                                postRequest.RequestUri, ex.Message);
                result = "EWB-" + (string.IsNullOrWhiteSpace(ex.Message) ? "Network or API Error" : ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error occurred during POST to {RequestUri}: {Message}",
                                postRequest.RequestUri, ex.Message);
                result = "EWB-" + ex.Message;
                throw;
            }
            return result;
        }

    }
}
