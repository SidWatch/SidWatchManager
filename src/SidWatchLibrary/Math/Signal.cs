﻿using System;
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
		/// Only works on even sized sets of data
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
			double samples = Convert.ToDouble (_signalData.Length); // Double representation of sampling frequency
			int binSize = 512; //Number of samples in bin
			int twoBin = binSize * 2; //Number of samples with overlapping
			int elements = (_nfft / 2) + 1; //Number of output elements
			int firstElement = 0; //Id of the first output element
			int lastElement = elements - 1; //Id of last output element
			double nyquist = _samplingFrequency / 2;  // The maximum frequency that can be output
			int iterations = Convert.ToInt32(System.Math.Floor (samples / binSize)); //Number of loops through processign required

			double[] window = MathNet.Numerics.Window.Hann(twoBin); //Generate a set of windows factors

			double[] winSquared = window.Subset (0, twoBin); //Copy the window
			winSquared.Multiply (window); //Now Square it

			double scale = 1.0 / (_samplingFrequency * winSquared.Sum()); //Calculate Scaling Factor
			double[] Pxx = new double[elements]; //Construct array for output
			Complex[] complexData; //Define Temp array

			//Loop through all of the data 
			for (int k = 0; k < iterations; k++) {
				int position = k * binSize;  //Calculate the current position

				double[] temp = _signalData.Subset(position, twoBin); //Extract the data to process with window
				temp.Subtract (temp.Mean ()); // Calculate the mean and subtract it from the current set of data - "Detrending" from SuperSid
				temp.Multiply (window); // Apply the window to the current set of data
				temp = temp.ZeroPad (_nfft); // Since we only have two times a bin of data going in, but we want NFFT number of frequency devisions
				                             // we may need to zero pad the end of the data so that when the FFT is applied we get the requested number
											 // of sub-divisions desired (For SidWatch we are going to be looking for 513 values out)

				complexData = MathHelper.FromDoubleArray (temp);  //Load the double data into a temporary array of Complex values
				Fourier.Forward (complexData, FourierOptions.NoScaling); // Compute the FFT with no scaling

				if (k == 0) { // If this is the first iteration
					for (int j = 0; j < elements; j++) { // Loop through elements in results
						if (j == firstElement 
						    || j == lastElement) { //For the first and last result, just grab real part of results and assign it to the array
							Pxx [j] = System.Math.Pow (complexData [j].Real, 2);
						} else { // For all middle results sum the squares of the real and imaginary results (to get magnitude)
							Pxx [j] = System.Math.Pow (complexData [j].Real, 2) + System.Math.Pow (complexData [j].Imaginary, 2);
						}
					}
				} else { //Other wise do all this 
					double factor = k / (k + 1.0); // Calculate part of scaling factor
					Pxx.Multiply (factor); // Apply the factor to the array of output data

					for (int j = 0; j < elements; j++) { // Loop through elements in results 
						if (j == firstElement // for the first or last element only 
						    || j == lastElement) {

							Pxx [j] += System.Math.Pow(complexData [j].Real, 2) / (k + 1);  //Calculate and add to output square of real data 
						} else {
							Pxx [j] += (System.Math.Pow(complexData [j].Real, 2) + 
									System.Math.Pow(complexData [j].Imaginary, 2)) / (k + 1); //Calculate the sum of squares of real and imaginary 
						}
					}
				}
			}

			for (int j = 0; j < elements; j++) {
				if (j == firstElement
				    || j == lastElement) {
					Pxx [j] *= scale; //First and last elements are multiplied by scale factor
				} else {
					Pxx [j] *= 2 * scale; //Middle elements are multiplied by 2 times scale factor.
				}
			}

			//Build output objects
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

