namespace BakongHealthCheck.Configures
{
    public interface IConfigureBakong
    {
        public string BakongBaseUrl { get; set; }
        public string BakongHealthCheck { get; set; }
        public string BakongTimeService { get; set; }

        // Start and Stop
        public string BakongButton { get; set; }
        public string BakongSchedule { get; set; }


    }
}
