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
//		public static List<PowerDensityReading> FFT(double[] _signalData)
//		{
//			int length = _signalData.Length;
//			int halfLength = length / 2;
//
//			Complex[] input = MathHelper.FromDoubleArray (_signalData);
//
//			Fourier.Forward (input);
//
//			List<PowerDensityReading> readings = new List<PowerDensityReading> ();
//
//			for (int i = 0; i < halfLength; i++) {
//				double frequency = i * halfLength / 
//			}
//
//		}

		public static double[] GetWelchWindow(int binSize)
		{
			double[] window = new double[binSize];

			for (int i = 0; i < binSize; i++) {
				window [i] = 1.0 - System.Math.Pow((i - 0.5 * binSize) / (0.5 * binSize),2) ;
			}

			return window;
		}

		public static List<PowerDensityReading> CalculatePowerSpectralDensity2(
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
			int iterations = Convert.ToInt32(System.Math.Floor (sample / binSize)) - 1;

			double[] window = MathNet.Numerics.Window.Hann(twoBin);

			double scale = 1.0 / System.Math.Pow(window.Sum (), 2.0);
			double[] Pxx = new double[elements];

			for (int k = 0; k < iterations; k++) {
				int position = k * binSize;

				double[] temp = MathHelper.ExtractSubset (_signalData, position, twoBin);
				temp.Subtract (temp.Mean ());
				temp = temp.ZeroPad (_nfft);

				alglib.complex[] fftResult;
				alglib.fftr1d (temp, out fftResult);

				if (k == 0) {
					for (int j = 0; j < elements; j++) {
						Pxx [j] = System.Math.Pow(fftResult [j].x, 2);
					}
				} else {
					double factor = k / (k + 1.0);
					Pxx.Multiply (factor);

					for (int j = 0; j < elements; j++) {
						if (j == firstElement
						    || j == lastElement) {
							double value = System.Math.Pow(fftResult [j].x, 2) / (k + 1);

							Pxx [j] += value;
						} else {
							double value = (System.Math.Pow(fftResult [j].x, 2) + 
								System.Math.Pow(fftResult [j].y, 2)) / (k + 1);
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
				reading.Frequency = i * _samplingFrequency / (_nfft / 2);
				reading.Value = Pxx [i];

				output.Add (reading);
			}

			return output;
		}

		public static List<PowerDensityReading> CalculatePowerSpectralDensity(
			double[] _signalData, 
			int _samplingFrequency = 96000,
			int _binSize = 1024)
		{
			//Break down into bins
			int halfBin = _binSize / 2;
			int twoBins = _binSize * 2;
			//int fourBins = _binSize * 4;
			int totalSamples = _signalData.Length;
			double iterationsRequired = Convert.ToDouble(totalSamples) / Convert.ToDouble(halfBin);
			int totalBins = (int)System.Math.Floor(iterationsRequired);

			//double[] window = GetWelchWindow(_binSize);
			double[] window = MathNet.Numerics.Window.Hann(_binSize);
			double sumWindow = 0.0;
			for (int i = 0; i < _binSize; i++) {
				sumWindow += window [i] * window[i];
			}

			double[] powerDensity = new double[halfBin];

			int arrayPosition;
			double[] binData;
			double[] windowedBinData;
			Complex[] complexData;

			for (int i = 0; i < totalBins-1; i++) {
				arrayPosition = i * halfBin;

				binData = new double[_binSize];
				MathHelper.ExtractSubset (_signalData, arrayPosition, _binSize);

				windowedBinData = MathHelper.ApplyWindow (binData, window);

				complexData = MathHelper.FromDoubleArray (windowedBinData);

				Fourier.Forward (complexData,FourierOptions.NoScaling);

				for (int j = 0; j < halfBin; j++) {
					powerDensity [j] += complexData [j].Magnitude;
				}
			}

			//Process last partial bin with zero padding
			arrayPosition = halfBin*(totalBins-1);
			binData = new double[_binSize];
			int remainingData = totalSamples - arrayPosition;
			Array.Copy (_signalData, arrayPosition, binData, 0, remainingData);
			windowedBinData = MathHelper.ApplyWindow (binData, window);

			complexData = MathHelper.FromDoubleArray (windowedBinData);

			Fourier.Forward (complexData);

			for (int j = 0; j < halfBin; j++) {
				powerDensity [j] += complexData [j].Magnitude;
			}


			List<PowerDensityReading> output = new List<PowerDensityReading> ();

			for (int i = 0; i < halfBin; i++) {
				PowerDensityReading reading = new PowerDensityReading ();
				reading.Frequency = i * _samplingFrequency / _binSize;
				//reading.Value = 20 * System.Math.Log10 (powerDensity [i] / (sumWindow));
				reading.Value =  powerDensity [i] / (sumWindow * _binSize);

				output.Add (reading);
			}

			return output;
		}





	}
}

