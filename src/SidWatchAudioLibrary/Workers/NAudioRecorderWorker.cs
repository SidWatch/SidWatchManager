using System;
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
			byte[] data = m_MemStream.ToArray();
            m_MemStream.Dispose();
		    m_MemStream = null;

		    Console.WriteLine("Received {0} bytes", data.Length);

		    int desiredBytes = Convert.ToInt32((RecordForTicks/1000)*SamplesPerSecond)*(BitsPerSample/8) + 44;

		    byte[] output = new byte[desiredBytes];


		    if (data.Length > desiredBytes)
		    {
		        Console.WriteLine("Copying desired bytes ({0})", desiredBytes);
		        Array.Copy(data, 0, output, 0, desiredBytes);		   
            }
		    else
		    {
                Console.WriteLine("Less than desired bytes ({0})", output.Length);
		        output = data;
		    }

            int countNonZero = 0;
            for (int i = 45; i < output.Length; i++)
            {
                if (output[i] != 0)
                {
                    countNonZero++;
                }
            }

		    Console.WriteLine("Found {0} non-zero bytes", countNonZero);

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

