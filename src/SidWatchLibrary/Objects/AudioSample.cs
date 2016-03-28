using System;
using System.Collections.Generic;

namespace SidWatchLibrary.Objects
{
    public class AudioSegment
    {
        public DateTime StartTime { get; set; }
        public int SamplesPerSeconds { get; set; }
        public List<double> Samples { get; set; }
    }
}
