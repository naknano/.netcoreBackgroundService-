using System.Net.Http;
using BakongHealthCheck.APIConnection;
using BakongHealthCheck.Configures;
using BakongHealthCheck.Dto.Bakong;
using Newtonsoft.Json;
using Serilog;

namespace BakongHealthCheck.Services.Bakong
{
    public class BakongService : IBakongService
    {
        private readonly IMyHttpRequest myHttpRequest;
        private readonly IConfigureBakong configure;
    
        public BakongService(IMyHttpRequest myHttpRequest, IConfigureBakong configure)
        {
            this.myHttpRequest = myHttpRequest;
            this.configure = configure;
        }

        public Task<ResponseBakongHealthDTO> GetBakongHealth(CancellationToken cancellationToken)
        {
            try
            {
                string a = configure.BakongBaseUrl.ToString();
                var response = myHttpRequest.HttpReqstApiB24Async<ResponseBakongHealthDTO>("", configure.BakongBaseUrl.ToString(), configure.BakongHealthCheck.ToString(), "", cancellationToken);
                return response;
            }
            catch (TaskCanceledException ex)
            {
                // No need add log
                throw new TaskCanceledException(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error("BakongHealthCheck > Direct to Bakong |" + ex.Message);
                throw new Exception(ex.Message);
            }
        }

    }
}

