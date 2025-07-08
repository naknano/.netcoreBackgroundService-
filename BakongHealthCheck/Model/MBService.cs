using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace BakongHealthCheck.Entities
{
    [Table("MB_SERVICE")]
    public class MBService 
    {
        [Key]
        [JsonProperty("RECID")]
        [Column("RECID")]
        public int recID { get; set; }


        [JsonProperty("USER_ID")]
        [Column("USER_ID")]
        public string userID { get; set; }


        [JsonProperty("SERVICE_ID")]
        [Column("SERVICE_ID")]
        public string serviceID { get; set; }


        [JsonProperty("START_DATE")]
        [Column("START_DATE")]
        public DateTime startDate { get; set; }


        [JsonProperty("END_DATE")]
        [Column("END_DATE")]
        public DateTime endDate { get; set; }


        [JsonProperty("STATUS")]
        [Column("STATUS")]
        public string status { get; set; }


        [JsonProperty("REMARK")]
        [Column("REMARK")]
        public string? remark { get; set; }


        [JsonProperty("BLACK_LIST_VERSION")]
        [Column("BLACK_LIST_VERSION")]
        public string? blackListVersion { get; set; }
    }
}

