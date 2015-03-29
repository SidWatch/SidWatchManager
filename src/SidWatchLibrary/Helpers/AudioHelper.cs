using SidWatchLibrary.Delegates;
using SidWatchLibrary.Workers;
using System.Threading;

namespace SidWatchLibrary.Helpers
{
	

	public static class AudioHelper
	{
		public static void RecordAudio(int _durationTicks, CompletedRecording _complete)
		{
			AudioRecorderWorker worker = new AudioRecorderWorker ();
			worker.RecordForTicks = _durationTicks;
			worker.Complete = _complete;

			ThreadStart ts = new ThreadStart (worker.Record);
			Thread t = new Thread (ts);
			t.Start ();
		}
	}
}

