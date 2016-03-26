using SidWatchLibrary.Delegates;
using SidWatchLibrary.Interfaces;
using SidWatchLibrary.Workers;
using SidWatchLibrary.Objects;
using System.Collections.Generic;
using System.Threading;

namespace SidWatchLibrary.Helpers
{
	

	public static class AudioHelper
	{
		public static void RecordAudio(int _durationTicks, CompletedRecording _complete)
		{
			AudioRecorderWorker worker = new AudioRecorderWorker (_complete);
			worker.RecordForTicks = _durationTicks;
			worker.CompletedWork = CompletedWork;

			worker.Start ();
		}

		private static void CompletedWork(IWorker _worker)
		{
			if (_worker is AudioRecorderWorker) {
				AudioRecorderWorker worker = (AudioRecorderWorker)_worker;

				AudioSample sample = worker.Sample;
			
				if (worker.CompletedRecording != null)
				{
				    worker.CompletedRecording(sample);
				}
			}
		}

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

