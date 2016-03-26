using System;

namespace SidWatchLibrary.Objects
{
    public class AudioSample
    {
        public DateTime StartTime { get; set; }
        public int SamplesPerSeconds { get; set; }
        public byte[] Data { get; set; }
    }
}
