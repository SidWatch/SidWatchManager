using System;
using System.IO;
using Accord.Audio;
using Accord.DirectSound;
using SidWatchLibrary.Delegates;
using SidWatchLibrary.Objects;
using TreeGecko.Library.Common.Helpers;

namespace SidWatchAudioLibrary.Workers
{
	public class AccordAudioRecorderWorker : AbstractAudioRecorder
	{
	    private AudioCaptureDevice m_AudioSource;
		private MemoryStream m_MemStream;

	    public AccordAudioRecorderWorker(CompletedRecording _complete)
		{
            SamplesPerSecond = 96000;
			RecordForTicks = 1000;
			CompletedRecording = _complete;

	        string desiredAudioDevice = Config.GetSettingValue("AudioDeviceName");

	        AudioDeviceInfo captureDeviceInfo = null;
            var collection = new AudioDeviceCollection(AudioDeviceCategory.Capture);

	        if (desiredAudioDevice != null)
	        {	            
	            foreach (var device in collection)
	            {
	                if (device.Description == desiredAudioDevice)
	                {
                        Console.WriteLine("Using specified device - {0}", device);
	                    captureDeviceInfo = device;
	                }	                
	            }
	        }

	        if (captureDeviceInfo == null)
	        {
	            captureDeviceInfo = collection.Default;
	            Console.WriteLine("Using Default Capture Device- {0}", captureDeviceInfo);
	        }

	        m_AudioSource = new AudioCaptureDevice(captureDeviceInfo)
            {
                DesiredFrameSize = 4096,
                SampleRate = SamplesPerSecond,
                Format = SampleFormat.Format32Bit
            };

            // Specify capturing options
            m_AudioSource.NewFrame += NewAudioFrame;
            m_AudioSource.AudioSourceError += AudioSourceError;
		}
		

		public CompletedRecording CompletedRecording { get; private set; }

	    public override void DoWork()
	    {
            //Create local memory stream for this recording
	        m_MemStream = new MemoryStream();

            //Start recording
            m_AudioSource.Start();

            //Set the start time and end time
	        StartTime = DateTime.Now;
	        EndTime = StartTime.AddSeconds(1.1);
	    }

        private void AudioSourceError(object _sender, AudioSourceErrorEventArgs _e)
        {
            
        }

        private void NewAudioFrame(object _sender, NewFrameEventArgs _e)
	    {
            //Append data to the stream
	        byte[] data = _e.Signal.RawData;
	        m_MemStream.Write(data, 0, data.Length);

            //Check if done
            if (DateTime.Now > EndTime)
            {
                //stop if done
                m_AudioSource.Stop();
                Done();
            }
	    }

		private void Done()
		{
			byte[] data = m_MemStream.ToArray();
            m_MemStream.Dispose();
		    m_MemStream = null;

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
			if (m_AudioSource != null) {
                m_AudioSource.Dispose();
                m_AudioSource = null;
			}

			if (m_MemStream != null) {
				m_MemStream.Dispose ();
				m_MemStream = null;
			}
		}
	}
}

