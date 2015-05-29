using System;
using SidWatchLibrary.Delegates;
using NAudio.Wave;
using System.Threading;
using System.IO;

namespace SidWatchLibrary.Workers
{
	public class AudioRecorderWorker : AbstractWorker 
	{
		private WaveInEvent m_WaveIn;
		private WaveFileWriter m_WaveWriter;
		private MemoryStream m_MemStream;

		public AudioRecorderWorker(CompletedRecording _complete)
		{
			SamplesPerSecond = 96000;
			RecordForTicks = 1000;
			CompletedRecording = _complete;
		}
		
		public int RecordForTicks { get; set;}
		public int SamplesPerSecond {get;set;}
		public CompletedRecording CompletedRecording { get; private set; }

		public override void DoWork()
		{
			m_WaveIn = new WaveInEvent();
			m_WaveIn.WaveFormat = new WaveFormat(96000, 1);
			m_WaveIn.BufferMilliseconds = 1000;
			m_WaveIn.RecordingStopped += RecordingStopped;
			m_MemStream = new MemoryStream();
			m_WaveWriter = new WaveFileWriter(m_MemStream, m_WaveIn.WaveFormat);

			m_WaveIn.StartRecording();

			long endTick = DateTime.Now.Ticks + RecordForTicks;

			do {
				Thread.Sleep(50);
			} while (DateTime.Now.Ticks < endTick);

			m_WaveIn.StopRecording ();


		}

		private void RecordingStopped(object sender, StoppedEventArgs e)
		{
			byte[] data = m_MemStream.ToArray();

			FireComplete ();
		}

		public override void Dispose ()
		{
			if (m_WaveIn != null) {
				m_WaveIn.Dispose ();
				m_WaveIn = null;
			}

			if (m_MemStream != null) {
				m_MemStream.Dispose ();
				m_MemStream = null;
			}
		}
	}
}

