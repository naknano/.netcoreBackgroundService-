using System.Net.Http;
using BakongHealthCheck.APIConnection;
using BakongHealthCheck.Configures;
using BakongHealthCheck.Dto.Bakong;
using Newtonsoft.Json;
using Serilog;

namespace BakongHealthCheck.Services.Bakong
{
    public interface IBakongService 
    {
        Task<ResponseBakongHealthDTO>  GetBakongHealth(CancellationToken cancellationToken);
    }
}

