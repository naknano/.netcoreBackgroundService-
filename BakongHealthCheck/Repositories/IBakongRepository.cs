using BakongHealthCheck.Dto;
using BakongHealthCheck.Dto.MBService;
using BakongHealthCheck.Entities;

namespace BakongHealthCheck.Repository
{
    public interface IBakongRepository
    {
        Task<ResponseV1DTO> getBakongHealth();
        Task<ResponseV1DTO> createBakongHealthCheck(CancellationToken cancellationToken);
        Task<ResponseV1DTO> updateMBService(string status, int regId, string remark);
    }
}
