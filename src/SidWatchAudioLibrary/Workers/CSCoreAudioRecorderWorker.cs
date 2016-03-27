using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CSCore;
using CSCore.Codecs.WAV;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using SidWatchLibrary.Delegates;
using SidWatchLibrary.Objects;
using TreeGecko.Library.Common.Helpers;

namespace SidWatchAudioLibrary.Workers
{
    public class CSCoreAudioRecorderWorker : AbstractAudioRecorder
    {
	    private WasapiCapture m_AudioSource;
		private MemoryStream m_MemStream;
        private WaveWriter m_WaveWriter;
        private WaveFormat m_WaveFormat = new WaveFormat(96000, 24, 1);

        public CSCoreAudioRecorderWorker(CompletedRecording _complete)
		{
            SamplesPerSecond = 96000;
			RecordForTicks = 1000;
			CompletedRecording = _complete;

	        string desiredAudioDevice = Config.GetSettingValue("AudioDeviceName");

            MMDevice captureDeviceInfo = null;
            var collection = EnumerateWasapiDevices();

	        if (desiredAudioDevice != null)
	        {	            
	            foreach (var device in collection)
                {
	                if (device.FriendlyName == desiredAudioDevice)
	                {
                        Console.WriteLine("Using specified device - {0}", device);
	                    captureDeviceInfo = device;
	                    break;
	                }	                
	            }
	        }

            m_AudioSource = new WasapiCapture(
                eventSync: false,
                shareMode: AudioClientShareMode.Shared,
                latency:100,
                defaultFormat: m_WaveFormat);
            if (captureDeviceInfo != null)
            {
                m_AudioSource.Device = captureDeviceInfo;
            }

            m_AudioSource.DataAvailable += DataAvailable;
            m_AudioSource.Initialize();
		}

        private void DataAvailable(object _sender, DataAvailableEventArgs _e)
        {
            m_WaveWriter.Write(_e.Data, _e.Offset, _e.ByteCount);
        }

        public IEnumerable<MMDevice> EnumerateWasapiDevices()
        {
            using (MMDeviceEnumerator enumerator = new MMDeviceEnumerator())
            {
                return enumerator.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active);
            }
        }

		public CompletedRecording CompletedRecording { get; private set; }

	    public override void DoWork()
	    {
            //Create local memory stream for this recording
	        m_MemStream = new MemoryStream();
	        m_WaveWriter = new WaveWriter(m_MemStream, m_AudioSource.WaveFormat);

            //Start recording
            m_AudioSource.Start();

            //Set the start time and end time
	        StartTime = DateTime.UtcNow;
	        EndTime = StartTime.AddSeconds(1.1);

	        do
	        {
	            Thread.Sleep(10);
	        } while (EndTime > DateTime.UtcNow);

            m_AudioSource.Stop();
            Done();
	    }

		private void Done()
		{
			byte[] data = m_MemStream.ToArray();
            m_MemStream.Dispose();
		    m_MemStream = null;

		    int desiredBytes = Convert.ToInt32((RecordForTicks/1000)*96000) * 4;

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
			if (m_AudioSource != null) {
                m_AudioSource.Dispose();
                m_AudioSource = null;
			}

			if (m_MemStream != null) {
				m_MemStream.Dispose ();
				m_MemStream = null;
			}
		}

        public override void EnumDevicesToConsole()
        {
        }
    }
}
