namespace BakongHealthCheck.Dto.MBService
{
    public class ResponseMBServiceDTO
    {
        // Response status code and message 
        public int recID { get; set; }
        public string userID { get; set; }
        public string serviceID { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string status { get; set; }
        public string remark { get; set; }
        public string blackListVersion { get; set; }
    }
}
