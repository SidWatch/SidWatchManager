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
	}
}

