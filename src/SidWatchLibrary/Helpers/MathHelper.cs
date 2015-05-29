using System;
using System.Numerics;

namespace SidWatchLibrary.Helpers
{
	public static class MathHelper
	{
		public static Complex[] FromDoubleArray(double[] _input)
		{
			if (_input != null) {
				int length = _input.Length;

				Complex[] output = new Complex[length];

				for (int i = 0; i < length; i++) {
					output [i] = new Complex (_input [i], 0);
				}

				return output;
			}

			return null;
		}

		/// <summary>
		/// returns the magnitudes of the Complex vectors
		/// </summary>
		/// <returns>The complex array.</returns>
		/// <param name="_input">Input.</param>
		public static double[] FromComplexArray(Complex[] _input)
		{
			if (_input != null) {
				int length = _input.Length;

				double[] output = new double[length];

				for (int i = 0; i < length; i++) {
					output [i] = _input [i].Magnitude;
				}

				return output;
			}

			return null;
		}

		public static double[] ApplyWindow(double[] _input, double[] _window)
		{
			int length = _input.Length;

			double[] output = new double[length];

			for (int i = 0; i < length; i++) {
				output [i] = _input [i] * _window [i];
			}

			return output;
		}

		public static double[] ExtractSubset(double[] _input, int _start, int _width)
		{
			double[] output = new double[_width];

			Array.Copy (_input, _start, output, 0, _width);

			return output;
		}



	
	}
}

