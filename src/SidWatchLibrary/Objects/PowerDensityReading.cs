using System;

namespace SidWatchLibrary.Objects
{
	public class PowerDensityReading
	{
		public PowerDensityReading ()
		{
		}

		public double Frequency { get; set;}
		public double Value { get; set; }

		public Double dBValue {
			get {
				return 10 * System.Math.Log10 (Value); 
				// The data was squared but square root was not applied so 10 rather than 20
			}	
		}
	}
}

