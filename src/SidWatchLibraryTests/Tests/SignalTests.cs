using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SidWatch.Library.Calculation;


namespace SidWatch.Library.Tests
{
    [TestFixture]
    public class SignalTests
    {
        [Test]
        public void TestBucket1()
        {
            int bucket = 1;
            int maxBucket = Calculate(bucket);

            Assert.AreEqual(bucket, maxBucket);
        }

        [Test]
        public void TestBuckets()
        {
            for (int i = 1; i < 512; i++)
            {
                Console.WriteLine("Calculating bucket {0}", i);
                int maxBucket = Calculate(i);
                Assert.AreEqual(i, maxBucket);                            
            }
        }

        public int Calculate(int _bucket)
        {
            var bucket = Convert.ToSingle(_bucket);
            float frequency = 93.75f * bucket;
            float[] array = new float[96000];

            for (int i = 0; i < 96000; i++)
            {
                float t = frequency * (Convert.ToSingle(i) / 96000);

                float value = (float)Math.Sin(2f * Math.PI * t);
                array[i] = value;
            }

            var output = Signal.CalculatePowerSpectralDensity(array);

            int maxBucket = GetMaxBucket(output);

            return maxBucket;
        }

        public int GetMaxBucket(float[,] _psd)
        {
            int elements = _psd.GetLength(1);
            float maxValue = Single.MinValue;
            int maxBucket = -1;

            for (int i = 0; i < elements; i++)
            {
                float value = _psd[1, i];

                if (value > maxValue)
                {
                    maxValue = value;
                    maxBucket = i;
                }
            }

            return maxBucket;
        }


    }
}
