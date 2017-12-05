using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Ext
{
    public sealed class DataWriter
    {
        private byte[] dataBytes = new byte[4];
        private int bitLength;
        public byte[] Data
        {
            get
            {
                return this.dataBytes;
            }
        }
        public int BitLength
        {
            get
            {
                return this.bitLength;
            }
        }
        public void Reset()
        {
            this.bitLength = 0;
        }
        public int GetByteLength()
        {
            return (this.bitLength >> 3) + (((this.bitLength & 7) > 0) ? 1 : 0);
        }
        private void A(int num)
        {
            int num2 = (num >> 3) + (((num & 7) > 0) ? 1 : 0);
            if (this.dataBytes.Length < num2)
            {
                int i;
                for (i = this.dataBytes.Length; i < num2; i *= 2)
                {
                }
                Array.Resize<byte>(ref this.dataBytes, i);
            }
        }
        public void Write(byte[] source, int byteOffset, int byteLength)
        {
            int num = this.bitLength + byteLength * 8;
            this.A(num);
            BytesUtil.WriteBytes(source, byteOffset, byteLength, this.dataBytes, this.bitLength);
            this.bitLength = num;
        }
        public void Write(byte[] source)
        {
            this.Write(source, 0, source.Length);
        }
        public void Write(bool source)
        {
            int num = this.bitLength + 1;
            this.A(num);
            BytesUtil.WriteByte( (byte)(source ? 1 : 0), 1, this.dataBytes, this.bitLength);
            this.bitLength = num;
        }
        public void Write(byte source)
        {
            int num = this.bitLength + 8;
            this.A(num);
            BytesUtil.WriteByte(source, 8, this.dataBytes, this.bitLength);
            this.bitLength = num;
        }
        public void Write(byte source, int numberOfBits)
        {
            int num = this.bitLength + numberOfBits;
            this.A(num);
            BytesUtil.WriteByte(source, numberOfBits, this.dataBytes, this.bitLength);
            this.bitLength = num;
        }
        public void Write(ushort source)
        {
            int num = this.bitLength + 16;
            this.A(num);
            BytesUtil.WriteUInt32((uint)source, 16, this.dataBytes, this.bitLength);
            this.bitLength = num;
        }
        public void Write(ushort source, int numberOfBits)
        {
            int num = this.bitLength + numberOfBits;
            this.A(num);
            BytesUtil.WriteUInt32((uint)source, numberOfBits, this.dataBytes, this.bitLength);
            this.bitLength = num;
        }
        public void Write(short source)
        {
            this.Write((ushort)source);
        }
        public unsafe void Write(int source)
        {
            int num = this.bitLength + 32;
            this.A(num);
            if (this.bitLength % 8 == 0)
            {
                fixed (byte* ptr = &this.dataBytes[this.bitLength / 8])
                {
                    *(int*)ptr = source;
                }
            }
            else
            {
                BytesUtil.WriteUInt32((uint)source, 32, this.dataBytes, this.bitLength);
            }
            this.bitLength = num;
        }
        public void Write(int source, int numberOfBits)
        {
            int num = this.bitLength + numberOfBits;
            this.A(num);
            if (numberOfBits != 32)
            {
                int num2 = 1 << numberOfBits - 1;
                if (source < 0)
                {
                    source = (-source - 1 | num2);
                }
                else
                {
                    source &= ~num2;
                }
            }
            BytesUtil.WriteUInt32((uint)source, numberOfBits, this.dataBytes, this.bitLength);
            this.bitLength = num;
        }
        public unsafe void Write(uint source)
        {
            int num = this.bitLength + 32;
            this.A(num);
            if (this.bitLength % 8 == 0)
            {
                fixed (byte* ptr = &this.dataBytes[this.bitLength / 8])
                {
                    *(int*)ptr = (int)source;
                }
            }
            else
            {
                BytesUtil.WriteUInt32(source, 32, this.dataBytes, this.bitLength);
            }
            this.bitLength = num;
        }
        public void Write(uint source, int numberOfBits)
        {
            int num = this.bitLength + numberOfBits;
            this.A(num);
            BytesUtil.WriteUInt32(source, numberOfBits, this.dataBytes, this.bitLength);
            this.bitLength = num;
        }
        public void Write(long source)
        {
            int num = this.bitLength + 64;
            this.A(num);
            BytesUtil.WriteUInt64((ulong)source, 64, this.dataBytes, this.bitLength);
            this.bitLength = num;
        }
        public void Write(long source, int numberOfBits)
        {
            int num = this.bitLength + numberOfBits;
            this.A(num);
            BytesUtil.WriteUInt64((ulong)source, numberOfBits, this.dataBytes, this.bitLength);
            this.bitLength = num;
        }
        public void Write(ulong source)
        {
            int num = this.bitLength + 64;
            this.A(num);
            BytesUtil.WriteUInt64(source, 64, this.dataBytes, this.bitLength);
            this.bitLength = num;
        }
        public void Write(ulong source, int numberOfBits)
        {
            int num = this.bitLength + numberOfBits;
            this.A(num);
            BytesUtil.WriteUInt64(source, numberOfBits, this.dataBytes, this.bitLength);
            this.bitLength = num;
        }
        public unsafe void Write(float source)
        {
            uint source2 = *(uint*)(&source);
            this.Write(source2);
        }
        public unsafe void Write(double source)
        {
            ulong source2 = (ulong)(*(long*)(&source));
            this.Write(source2);
        }
        
        public void Write(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                this.WriteVariableUInt32(0u);
                return;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(source);
            this.WriteVariableUInt32((uint)bytes.Length);
            this.Write(bytes);
        }
        /// <summary>
        /// Write Base128 encoded variable sized unsigned integer
        /// </summary>
        /// <returns>number of bytes written</returns>
        public int WriteVariableUInt32(uint source)
        {
            int num = 1;
            uint num2 = source;
            while (num2 >= 128u)
            {
                this.Write((byte)(num2 | 128u));
                num2 >>= 7;
                num++;
            }
            this.Write((byte)num2);
            return num;
        }
        /// <summary>
        /// Write Base128 encoded variable sized signed integer
        /// </summary>
        /// <returns>number of bytes written</returns>
        public int WriteVariableInt32(int source)
        {
            int num = 1;
            uint num2 = (uint)(source << 1 ^ source >> 31);
            while (num2 >= 128u)
            {
                this.Write((byte)(num2 | 128u));
                num2 >>= 7;
                num++;
            }
            this.Write((byte)num2);
            return num;
        }
        /// <summary>
        /// Write Base128 encoded variable sized unsigned integer
        /// </summary>
        /// <returns>number of bytes written</returns>
        public int WriteVariableUInt64(ulong source)
        {
            int num = 1;
            ulong num2 = source;
            while (num2 >= 128uL)
            {
                this.Write((byte)(num2 | 128uL));
                num2 >>= 7;
                num++;
            }
            this.Write((byte)num2);
            return num;
        }
        /// <summary>
        /// Compress (lossy) a float in the range -1..1 using numberOfBits bits
        /// </summary>
        public void WriteSignedSingle(float source, int numberOfBits)
        {
            if (source < -1f)
            {
                Log.Fatal("SendDataWriter: WriteRangedInteger: source < -1.");
            }
            if (source > 1f)
            {
                Log.Fatal("SendDataWriter: WriteRangedInteger: source > 1.");
            }
            float num = (source + 1f) * 0.5f;
            int num2 = (1 << numberOfBits) - 1;
            uint source2 = (uint)(num * (float)num2);
            this.Write(source2, numberOfBits);
        }
        /// <summary>
        /// Compress (lossy) a float in the range 0..1 using numberOfBits bits
        /// </summary>
        public void WriteUnitSingle(float source, int numberOfBits)
        {
            if (source < 0f)
            {
                Log.Fatal("SendDataWriter: WriteRangedInteger: source < 0.");
            }
            if (source > 1f)
            {
                Log.Fatal("SendDataWriter: WriteRangedInteger: source > 1.");
            }
            int num = (1 << numberOfBits) - 1;
            uint source2 = (uint)(source * (float)num);
            this.Write(source2, numberOfBits);
        }
        /// <summary>
        /// Compress a float within a specified range using a certain number of bits
        /// </summary>
        public void WriteRangedSingle(float source, float min, float max, int numberOfBits)
        {
            if (source < min)
            {
                Log.Fatal("SendDataWriter: WriteRangedInteger: source < min.");
            }
            if (source > max)
            {
                Log.Fatal("SendDataWriter: WriteRangedInteger: source > max.");
            }
            float num = max - min;
            float num2 = (source - min) / num;
            int num3 = (1 << numberOfBits) - 1;
            this.Write((uint)((float)num3 * num2), numberOfBits);
        }
        private static int A(uint num)
        {
            int num2 = 1;
            while ((num >>= 1) != 0u)
            {
                num2++;
            }
            return num2;
        }
        /// <summary>
        /// Writes an integer with the least amount of bits need for the specified range
        /// </summary>
        /// <returns>number of bits written</returns>
        public int WriteRangedInteger(int source, int min, int max)
        {
            if (source < min)
            {
                Log.Fatal("SendDataWriter: WriteRangedInteger: source < min.");
            }
            if (source > max)
            {
                Log.Fatal("SendDataWriter: WriteRangedInteger: source > max.");
            }
            uint num = (uint)(max - min);
            int num2 = DataWriter.A(num);
            uint source2 = (uint)(source - min);
            this.Write(source2, num2);
            return num2;
        }
        /// <summary>
        /// Pads data with enough bits to reach a full byte. Decreases cpu usage for subsequent byte writes.
        /// </summary>
        public void WritePadBits()
        {
            this.bitLength += (this.bitLength + 7) / 8 * 8;
            this.A(this.bitLength);
        }
        /// <summary>
        /// Pads data with the specified number of bits.
        /// </summary>
        public void WritePadBits(int numberOfBits)
        {
            this.bitLength += numberOfBits;
            this.A(this.bitLength);
        }

    }
}
