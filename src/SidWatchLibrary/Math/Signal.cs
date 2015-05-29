using System;
using System.Collections.Generic;
using System.Numerics;
using SidWatchLibrary.Helpers;
using MathNet.Numerics.IntegralTransforms;
using SidWatchLibrary.Objects;

namespace SidWatchLibrary.Math
{
	public static class Signal
	{
		/// <summary>
		/// Calculates the power spectral density.  Note that this has been tested against SuperSid's output for the specific
		/// need and may not work generally.
		/// </summary>
		/// <returns>The power spectral density.</returns>
		/// <param name="_signalData">Signal data.</param>
		/// <param name="_samplingFrequency">Sampling frequency.</param>
		/// <param name="_nfft">Nfft.</param>
		public static List<PowerDensityReading> CalculatePowerSpectralDensity(
			double[] _signalData, 
			int _samplingFrequency = 96000,
			int _nfft = 1024)
		{
			double sample = Convert.ToDouble (_samplingFrequency);
			int binSize = 128;
			int twoBin = 256;
			int elements = (_nfft / 2) + 1;
			int firstElement = 0;
			int lastElement = elements - 1;
			double nyquist = _samplingFrequency / 2;
			int iterations = Convert.ToInt32(System.Math.Floor (sample / binSize)) - 1;

			double[] window = MathNet.Numerics.Window.Hann(twoBin);

			double[] winSquared = MathNet.Numerics.Window.Hann (twoBin);
			winSquared.Multiply (window);

			double scale = 1.0 / (_samplingFrequency * winSquared.Sum());
			double[] Pxx = new double[elements];
			Complex[] complexData;

			for (int k = 0; k < iterations; k++) {
				int position = k * binSize;

				double[] temp = MathHelper.ExtractSubset (_signalData, position, twoBin);
				temp.Subtract (temp.Mean ());
				temp.Multiply (window);
				temp = temp.ZeroPad (_nfft);

				complexData = MathHelper.FromDoubleArray (temp);
				Fourier.Forward (complexData, FourierOptions.NoScaling);

				if (k == 0) {
					for (int j = 0; j < elements; j++) {
						Pxx [j] = System.Math.Pow(complexData [j].Real, 2);
					}
				} else {
					double factor = k / (k + 1.0);
					Pxx.Multiply (factor);

					for (int j = 0; j < elements; j++) {
						if (j == firstElement
						    || j == lastElement) {
							double value = System.Math.Pow(complexData [j].Real, 2) / (k + 1);

							Pxx [j] += value;
						} else {
							double value = (System.Math.Pow(complexData [j].Real, 2) + 
								System.Math.Pow(complexData [j].Imaginary, 2)) / (k + 1);
							Pxx [j] += value;
						}
					}
				}
			}

			for (int j = 0; j < elements; j++) {
				if (j == firstElement
				    || j == lastElement) {
					Pxx [j] *= scale;
				} else {
					Pxx [j] *= 2 * scale;
				}
			}

			List<PowerDensityReading> output = new List<PowerDensityReading> ();

			for (int i = 0; i < elements; i++) {
				PowerDensityReading reading = new PowerDensityReading ();
				reading.Frequency = (i * nyquist) / (_nfft / 2);
				reading.Value = Pxx [i];

				output.Add (reading);
			}

			return output;
		}
	}
}

