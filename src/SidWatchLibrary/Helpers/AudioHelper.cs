using SidWatchLibrary.Delegates;
using SidWatchLibrary.Interfaces;
using SidWatchLibrary.Workers;
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

		public void CompletedWork(IWorker _worker)
		{
			if (IWorker is AudioRecorderWorker) {
				AudioRecorderWorker worker = (AudioRecorderWorker)_worker;

				List<AudioReading> readings = worker.
			
			}
		}
	}
}

