using System;

namespace SidWatchLibrary.Helpers
{
	public static class ArrayHelper
	{
		public static void Subtract(this double[] _input, double _value)
		{
			int length = _input.Length;

			for (int i = 0; i < length; i++) {
				_input [i] -= _value;
			}
		}

		public static void Add(this double[] _input, double _value)
		{
			int length = _input.Length;

			for (int i = 0; i < length; i++) {
				_input [i] += _value;
			}
		}

		public static void Multiply(this double[] _input, double _value)
		{
			int length = _input.Length;

			for (int i = 0; i < length; i++) {
				_input [i] *= _value;
			}
		}

		public static void Multiply(this double[] _input, double[] _values)
		{
			int length = _input.Length;

			if (length == _values.Length) {
				for (int i = 0; i < length; i++) {
					_input [i] *= _values[i];
				}
			}
		}

		public static void Divide(this double[] _input, double _value)
		{
			int length = _input.Length;

			for (int i = 0; i < length; i++) {
				_input [i] /= _value;
			}
		}

		public static double Mean(this double[] _input)
		{
			int length = _input.Length;

			double sum = _input.Sum();

			return sum / length;
		}

		public static double Maximum(this double[] _input)
		{
			int length = _input.Length;

			double max = _input [0];

			for (int i = 1; i < length; i++) {
				if (_input [i] > max) {
					max = _input [i];
				}
			}

			return max;
		}

		public static double Minimum(this double[] _input)
		{
			int length = _input.Length;

			double min = _input [0];

			for (int i = 1; i < length; i++) {
				if (_input [i] < min) {
					min = _input [i];
				}
			}

			return min;
		}


		public static double Sum(this double[] _input)
		{
			int length = _input.Length;
			double sum = 0.0;

			for (int i = 0; i < length; i++) {
				sum += _input [i];
			}

			return sum;
		}

		public static double[] ZeroPad(this double[] _input, int _length)
		{
			if (_input.Length < _length) {				
				double[] output = new double[_length];

				Array.Copy (_input, 0, output, 0, _input.Length);

				return output;
			}
			return _input;
		}

		public static double[] Subset(this double[] _input, int _start, int _width)
		{
			double[] output = new double[_width];

			int width = _width;
			int length = _input.Length;

			if (_start + _width > length) {
				width = length - _start;
			}

			Array.Copy (_input, _start, output, 0, width);

			return output;
		}
	}
}

