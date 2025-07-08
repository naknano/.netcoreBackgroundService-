namespace BakongHealthCheck.Configures
{
    public class ConfigureBakong : IConfigureBakong
    {
        public string BakongBaseUrl { get; set; }
        public string BakongHealthCheck { get; set; }
        public string BakongTimeService { get; set; }

        public string BakongButton { get; set; }
        public string BakongSchedule { get; set; }
    }
}
