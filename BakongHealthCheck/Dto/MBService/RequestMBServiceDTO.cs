using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BakongHealthCheck.Dto.MBService
{
    public class RequestMBServiceDTO
    {
        public string recID { get; set; }
        public string userID { get; set; }
        public string serviceID { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string status { get; set; }
        public string remark { get; set; }
        public string blackListVersion { get; set; }
    }

}
