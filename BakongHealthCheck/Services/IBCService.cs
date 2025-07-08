using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BakongHealthCheck.Repository;
using BakongHealthCheck.Dto.MBService;
using BakongHealthCheck.Dto;

namespace BakongHealthCheck.Services
{
    public interface IBCService 
    {
        Task<ResponseV1DTO> BakongHealthCheck(CancellationToken cancellationToken);
    }
}
