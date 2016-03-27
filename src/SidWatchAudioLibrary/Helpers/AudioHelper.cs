using System.Collections.Generic;
using SidWatchAudioLibrary.Workers;
using SidWatchLibrary.Delegates;
using SidWatchLibrary.Interfaces;
using SidWatchLibrary.Objects;

namespace SidWatchAudioLibrary.Helpers
{
	public static class AudioHelper
	{
		public static double[] GetSignal(List<AudioReading> _readings)
		{
			int length = _readings.Count;

			double[] output = new double[length];

			int pos = 0;

			foreach (AudioReading reading in _readings) {
				output [pos] = reading.Value;
				pos++;
			}

			return output;
		}

		public static double[] GetPower(List<PowerDensityReading> _readings)
		{
			int length = _readings.Count;

			double[] output = new double[length];

			int pos = 0;

			foreach (PowerDensityReading reading in _readings) {
				output [pos] = reading.Value;
				pos++;
			}

			return output;
		}
	}
}

