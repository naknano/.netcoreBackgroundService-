
using BakongHealthCheck.Dto.MBService;

namespace BakongHealthCheck.Dto
{
    public class ResponseV1DTO
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; } 

        public ResponseMBServiceDTO responseMBServiceDTO { get; set; }
    }
}
