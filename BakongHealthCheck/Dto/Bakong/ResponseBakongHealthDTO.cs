using Newtonsoft.Json;

namespace BakongHealthCheck.Dto.Bakong
{
    public class ResponseBakongHealthDTO
    {
        [JsonProperty("responseCode")]
        public string code { get; set; }

        [JsonProperty("responseMessage")]
        public string message { get; set; }

        [JsonProperty("data")]
        public ResultDto result { get; set; }
    }


    public class ResultDto
    {
        [JsonProperty("status")]
        public string status { get; set; }
    }
}
