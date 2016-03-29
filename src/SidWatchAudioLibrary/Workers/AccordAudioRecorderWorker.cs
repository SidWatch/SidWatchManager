using System;
using System.Collections.Generic;
using System.IO;
using Accord.Audio;
using Accord.Audio.Formats;
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
	        : base(_complete)
		{
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
	                    break;
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

	    public override void DoWork()
	    {
            //Create local memory stream for this recording
	        m_MemStream = new MemoryStream();

	        FirstData = true;
            
            //Start recording
            m_AudioSource.Start();
	    }

        private void AudioSourceError(object _sender, AudioSourceErrorEventArgs _e)
        {
            
        }

        private void NewAudioFrame(object _sender, NewFrameEventArgs _e)
	    {
            if (FirstData)
            {
                SetStartEnd();
            }
            else
            {
                //Append data to the stream
                byte[] data = _e.Signal.RawData;
                m_MemStream.Write(data, 0, data.Length);

                //Check if done
                if (DateTime.UtcNow > EndTime)
                {
                    //stop if done
                    m_AudioSource.Stop();
                    Done();
                }                
            }
	    }

		private void Done()
		{
		    WaveDecoder decoder = new WaveDecoder(m_MemStream);
		    LogFormat(decoder.SampleRate, decoder.BitsPerSample, decoder.Channels);

            int desiredSamples = (RecordForMilliseconds / 1000) * SamplesPerSecond;

		    Signal signal = decoder.Decode();
            
		    int frames = signal.Samples;

		    TraceFileHelper.Verbose(string.Format("Found {0} samples, using {1}", frames, desiredSamples));

		    if (frames > desiredSamples)
		    {
		        frames = desiredSamples;
		    }

            List<Double> samples = new List<double>();
		    for (int i = 0; i < frames; i++)
		    {
		        samples.Add(signal.GetSample(0, i));
		    }

		    Segment = new AudioSegment
		    {
		        StartTime = StartTime,
		        SamplesPerSeconds = SamplesPerSecond,
                Channel1 = samples
		    };

		    SendData(Segment);
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

