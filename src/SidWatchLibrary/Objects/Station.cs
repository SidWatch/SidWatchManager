namespace SidWatchLibrary.Objects
{
    public class Station
    {
        public string MonitorId { get; set; }
        public string StationName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int UtcOffset { get; set; }
        public string Timezone { get; set; }
    }
}

