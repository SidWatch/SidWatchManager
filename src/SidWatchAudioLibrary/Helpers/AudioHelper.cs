using System;
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

	    public static void GetMinMax(List<Double> _segmentData, out double _minValue, out double _maxValue)
	    {
	        if (_segmentData != null)
	        {
	            _minValue = double.MaxValue;
	            _maxValue = double.MinValue;

	            foreach (double sample in _segmentData)
	            {
	                if (_minValue > sample)
	                {
	                    _minValue = sample;
	                }

	                if (_maxValue < sample)
	                {
	                    _maxValue = sample;
	                }
	            }

	            return;
	        }

	        _minValue = 0;
	        _maxValue = 0;
	    }
	}
}

