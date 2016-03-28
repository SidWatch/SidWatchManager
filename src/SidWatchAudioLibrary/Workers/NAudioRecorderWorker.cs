using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using SidWatchLibrary.Delegates;
using SidWatchLibrary.Objects;
using TreeGecko.Library.Common.Helpers;

namespace SidWatchAudioLibrary.Workers
{
	public class NAudioRecorderWorker : AbstractAudioRecorder 
	{
		private IWaveIn m_WaveIn;
		private WaveFileWriter m_WaveWriter;
		private MemoryStream m_MemStream;

        public NAudioRecorderWorker(CompletedRecording _complete)
		{
			CompletedRecording = _complete;
		}
		
		public CompletedRecording CompletedRecording { get; private set; }

		public override void DoWork()
		{
            //Get the desired device
            string deviceName = Config.GetSettingValue("AudioDeviceName");

            //Get all devices
            var deviceEnum = new MMDeviceEnumerator();
            var devices = deviceEnum.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToList();
		    int deviceId = 0;

            //Find right device
		    if (devices.Count > 0)
		    {
		        bool foundDevice = false;
                for (int i = 0; i < WaveIn.DeviceCount; i++)
                {
                    var device = devices[i];
                    var capabilities = WaveIn.GetCapabilities(i);

                    if (deviceName.Equals(device.FriendlyName))
                    {
                        deviceId = i;
                        device.AudioEndpointVolume.Mute = false;
                        foundDevice = true;
                    }
                }

		        if (foundDevice)
		        {
		            Console.WriteLine("Using requested device");
		        }
		        else
		        {
		            Console.WriteLine("Using default device");
		        }

                m_WaveIn = new WaveInEvent
                {
                    WaveFormat = new WaveFormat(SamplesPerSecond, BitsPerSample, 1),
                    BufferMilliseconds = 100,
                    DeviceNumber = deviceId
                };

		        var waveIn = (WaveInEvent) m_WaveIn;
		        waveIn.DeviceNumber = deviceId;

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
            Console.WriteLine("Received {0} bytes", m_MemStream.Length);

		    int desiredSamples = (RecordForTicks/1000)*SamplesPerSecond;

		    List<Double> samples = new List<double>();
		    m_MemStream.Position = 0;
            WaveFileReader reader = new WaveFileReader(m_MemStream);
		    int channels = reader.WaveFormat.Channels;

		    int frames = Convert.ToInt32(reader.SampleCount/channels);

		    double minValue = double.MaxValue;
		    double maxValue = double.MinValue;

            for (int i = 0; i < frames; i++)
            {
                float[] frame = reader.ReadNextSampleFrame();

                double value = frame[0];
                samples.Add(value);

                if (minValue > value)
                {
                    minValue = value;
                }

                if (maxValue < value)
                {
                    maxValue = value;
                }
            }

		    Console.WriteLine("Minimum Value Find - {0}", minValue);
		    Console.WriteLine("Maximum Value Find - {0}", maxValue);

		    Segment = new AudioSegment
		    {
		        StartTime = StartTime,
		        SamplesPerSeconds = SamplesPerSecond,
                Samples = samples
		    };

			FireComplete ();

		    if (CompletedRecording != null)
		    {
		        CompletedRecording(Segment);
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

	    public override void EnumDevicesToConsole()
	    {
            var deviceEnum = new MMDeviceEnumerator();
            var devices = deviceEnum.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToList();

	        int i = 1;
	        foreach (var mmDevice in devices)
	        {
	            Console.WriteLine("{0} - {1}", 1, mmDevice.FriendlyName);
	            i++;
	        }
	    }
	}
}



