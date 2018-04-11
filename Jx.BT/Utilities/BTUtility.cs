using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Jx.BT
{
    public static class BTUtility
    {
        private static readonly RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider(); 

        public static byte[] RandomBytes(int n = 32)
        {
            if (n < 1)
                n = 32;

            byte[] dataBytes = new byte[n];
            cryptoServiceProvider.GetBytes(dataBytes);

            return dataBytes;
        }

        public static int RandomInt()
        {
            byte[] dataBytes = RandomBytes();
            int result = BitConverter.ToInt32(dataBytes, 0);
            return result; 
        }
         
        /// <summary>
        /// 返回 [min, max]
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomInt(int min, int max)
        {
            if (min == max)
                return min;

            if( min > max )
            {
                int tmp = min;
                min = max;
                max = tmp; 
            }
            max = max + 1;

            int generatedValue = Math.Abs(BitConverter.ToInt32(RandomBytes(), 0));

            int diff = max - min;
            int mod = generatedValue % diff;
            int normalizedNumber = min + mod;

            return normalizedNumber; 
        }

        public static long RandomLong()
        {
            byte[] dataBytes = RandomBytes();
            long result = BitConverter.ToInt64(dataBytes, 0);
            return result; 
        }

        public static float RandomFloat()
        {
            byte[] dataBytes = RandomBytes();
            float result = BitConverter.ToSingle(dataBytes, 0);
            return result; 
        }
    }
}
