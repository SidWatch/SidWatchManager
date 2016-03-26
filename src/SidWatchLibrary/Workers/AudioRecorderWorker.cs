using System;
using System.Collections.Generic;
using SidWatchLibrary.Objects;
using SidWatchLibrary.Delegates;
using NAudio.Wave;
using System.Threading;
using System.IO;
using System.Linq;
using NAudio.CoreAudioApi;

namespace SidWatchLibrary.Workers
{
	public class AudioRecorderWorker : AbstractWorker 
	{
		private IWaveIn m_WaveIn;
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
        public DateTime StartTime { get; private set;}
        public DateTime EndTime { get; private set; }
		public AudioSample Sample { get; set; }

		public override void DoWork()
		{
            var deviceEnum = new MMDeviceEnumerator();
            var devices = deviceEnum.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToList();

		    if (devices.Count > 0)
		    {
                var device = (MMDevice)devices[0];
                device.AudioEndpointVolume.Mute = false;

                var capabilities = WaveIn.GetCapabilities(0);

                m_WaveIn = new WaveInEvent
                {
                    WaveFormat = new WaveFormat(96000, 24, 1),
                    BufferMilliseconds = 100,
                    DeviceNumber = 0
                };

                //m_WaveIn = new WasapiCapture(device);

                m_WaveIn.RecordingStopped += RecordingStopped;
                m_WaveIn.DataAvailable += DataAvailable;

                m_MemStream = new MemoryStream { Capacity = 1000000 };
                m_WaveWriter = new WaveFileWriter(m_MemStream, m_WaveIn.WaveFormat);

		        m_WaveIn.StartRecording();
                StartTime = DateTime.Now;

                EndTime = StartTime.AddSeconds(1.1);		        
		    }
		}

        private void DataAvailable(object sender, WaveInEventArgs e)
        {
            m_WaveWriter.Write(e.Buffer, 0, e.BytesRecorded);
            m_WaveWriter.Flush();

            if (DateTime.Now > EndTime)
            {
                m_WaveIn.StopRecording();
            }
        }

		private void RecordingStopped(object sender, StoppedEventArgs e)
		{
			byte[] data = m_MemStream.ToArray();

		    int desiredBytes = Convert.ToInt32((RecordForTicks/1000)*96000);

		    byte[] output = new byte[desiredBytes];

		    if (data.Length > desiredBytes)
		    {
		        Array.Copy(data, 0, output, 0, desiredBytes);		   
            }
		    else
		    {
		        output = data;
		    }

		    Sample = new AudioSample
		    {
		        StartTime = StartTime,
		        SamplesPerSeconds = SamplesPerSecond,
		        Data = output
		    };

			FireComplete ();

		    if (CompletedRecording != null)
		    {
		        CompletedRecording(Sample);
		    }
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

