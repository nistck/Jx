using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx
{
    public class EngineRandom
    {
        private const double ES = 4.6566128730773926E-10;
        private const double Es = 2.3283064365386963E-10;
        private const float ET = 4.656613E-10f;
        private const uint Et = 842502087u;
        private const uint EU = 3579807591u;
        private const uint Eu = 273326509u;
        private uint EV;
        private uint Ev;
        private uint EW;
        private uint Ew;
        private uint EX;
        private uint Ex = 1u;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Engine.Utils.EngineRandom" /> class, 
        /// using a time-dependent default seed value.
        /// </summary>
        public EngineRandom()
        {
            this.A(Math.Abs(Environment.TickCount * 347865));
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Engine.Utils.EngineRandom" /> class, 
        /// using the specified seed value.
        /// </summary>
        /// <param name="seed">
        /// A number used to calculate a starting value for the pseudo-random number
        /// sequence. If a negative number is specified, the absolute value of the number
        /// is used.
        /// </param>
        public EngineRandom(int seed)
        {
            this.A(seed);
        }
        private void A(int eV)
        {
            this.EV = (uint)eV;
            this.Ev = 842502087u;
            this.EW = 3579807591u;
            this.Ew = 273326509u;
        }
        /// <summary>
        /// Returns a nonnegative random number.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero and less than System.Int32.MaxValue.
        /// </returns>
        public int Next()
        {
            uint num = this.EV ^ this.EV << 11;
            this.EV = this.Ev;
            this.Ev = this.EW;
            this.EW = this.Ew;
            this.Ew = (this.Ew ^ this.Ew >> 19 ^ (num ^ num >> 8));
            uint num2 = this.Ew & 2147483647u;
            if (num2 == 2147483647u)
            {
                return this.Next();
            }
            return (int)num2;
        }
        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number to be generated. 
        /// maxValue must be greater than or equal to zero.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero, and less than maxValue;
        /// that is, the range of return values includes zero but not maxValue.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">maxValue is less than zero.</exception>
        public int Next(int maxValue)
        {
            if (maxValue < 0)
            {
                throw new ArgumentOutOfRangeException("maxValue", maxValue, "maxValue is less than zero.");
            }
            uint num = this.EV ^ this.EV << 11;
            this.EV = this.Ev;
            this.Ev = this.EW;
            this.EW = this.Ew;
            return (int)(4.6566128730773926E-10 * (double)(2147483647u & (this.Ew = (this.Ew ^ this.Ew >> 19 ^ (num ^ num >> 8)))) * (double)maxValue);
        }

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number returned. maxValue must be
        /// greater than or equal to minValue.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to minValue and less than maxValue;
        /// that is, the range of return values includes minValue but not maxValue. If
        /// minValue equals maxValue, minValue is returned.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">minValue is greater than maxValue.</exception>
        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException("maxValue", maxValue, "minValue is greater than maxValue.");
            }
            uint num = this.EV ^ this.EV << 11;
            this.EV = this.Ev;
            this.Ev = this.EW;
            this.EW = this.Ew;
            int num2 = maxValue - minValue;
            if (num2 < 0)
            {
                return minValue + (int)(2.3283064365386963E-10 * (this.Ew = (this.Ew ^ this.Ew >> 19 ^ (num ^ num >> 8))) * (double)((long)maxValue - (long)minValue));
            }
            return minValue + (int)(4.6566128730773926E-10 * (double)(2147483647u & (this.Ew = (this.Ew ^ this.Ew >> 19 ^ (num ^ num >> 8)))) * (double)num2);
        }
        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// </summary>
        /// <returns>A floating point number greater than or equal to 0.0, and less than 1.0.</returns>
        public double NextDouble()
        {
            uint num = this.EV ^ this.EV << 11;
            this.EV = this.Ev;
            this.Ev = this.EW;
            this.EW = this.Ew;
            return 4.6566128730773926E-10 * (double)(2147483647u & (this.Ew = (this.Ew ^ this.Ew >> 19 ^ (num ^ num >> 8))));
        }
        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// </summary>
        /// <returns>A floating point number greater than or equal to 0.0, and less than 1.0.</returns>
        public float NextFloat()
        {
            uint num = this.EV ^ this.EV << 11;
            this.EV = this.Ev;
            this.Ev = this.EW;
            this.EW = this.Ew;
            return 4.656613E-10f * (float)(2147483647u & (this.Ew = (this.Ew ^ this.Ew >> 19 ^ (num ^ num >> 8))));
        }
        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        public void NextBytes(byte[] buffer)
        {
            uint num = this.EV;
            uint num2 = this.Ev;
            uint num3 = this.EW;
            uint num4 = this.Ew;
            int i = 0;
            int num5 = buffer.Length - 3;
            while (i < num5)
            {
                uint num6 = num ^ num << 11;
                num = num2;
                num2 = num3;
                num3 = num4;
                num4 = (num4 ^ num4 >> 19 ^ (num6 ^ num6 >> 8));
                buffer[i++] = (byte)num4;
                buffer[i++] = (byte)(num4 >> 8);
                buffer[i++] = (byte)(num4 >> 16);
                buffer[i++] = (byte)(num4 >> 24);
            }
            if (i < buffer.Length)
            {
                uint num6 = num ^ num << 11;
                num = num2;
                num2 = num3;
                num3 = num4;
                num4 = (num4 ^ num4 >> 19 ^ (num6 ^ num6 >> 8));
                buffer[i++] = (byte)num4;
                if (i < buffer.Length)
                {
                    buffer[i++] = (byte)(num4 >> 8);
                    if (i < buffer.Length)
                    {
                        buffer[i++] = (byte)(num4 >> 16);
                        if (i < buffer.Length)
                        {
                            buffer[i] = (byte)(num4 >> 24);
                        }
                    }
                }
            }
            this.EV = num;
            this.Ev = num2;
            this.EW = num3;
            this.Ew = num4;
        }
        /// <summary>
        /// Returns a single random bit.
        /// </summary>
        /// <returns></returns>
        public bool NextBool()
        {
            if (this.Ex == 1u)
            {
                uint num = this.EV ^ this.EV << 11;
                this.EV = this.Ev;
                this.Ev = this.EW;
                this.EW = this.Ew;
                this.EX = (this.Ew = (this.Ew ^ this.Ew >> 19 ^ (num ^ num >> 8)));
                this.Ex = 2147483648u;
                return (this.EX & this.Ex) == 0u;
            }
            return (this.EX & (this.Ex >>= 1)) == 0u;
        }
        /// <summary>
        /// Returns a random number between -0.5 and 0.5.
        /// </summary>
        /// <returns>A floating point number greater than or equal to -0.5, and less than 0.5.</returns>
        public double NextDoubleCenter()
        {
            return this.NextDouble() - 0.5;
        }
        /// <summary>
        /// Returns a random number between -0.5 and 0.5.
        /// </summary>
        /// <returns>A floating point number greater than or equal to -0.5, and less than 0.5.</returns>
        public float NextFloatCenter()
        {
            return this.NextFloat() - 0.5f;
        }
        /// <summary>
        /// Returns a random number between min and max values.
        /// </summary>
        /// <returns>A floating point number greater than to minValue, and less than to maxValue.</returns>
        public float NextFloat(float minValue, float maxValue)
        {
            return minValue + this.NextFloat() * (maxValue - minValue);
        }
        /// <summary>
        /// Returns a random number between zero and max value.
        /// </summary>
        /// <returns>A floating point number greater than to zero, and less than to maxValue.</returns>
        public float NextFloat(float maxValue)
        {
            return this.NextFloat() * maxValue;
        }
    }

}
