using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Ext
{
    public sealed class DataReader
    {
        private byte[] dataBytes;
        private int bitPosition;
        private int startBitPosition;
        private int endBitPosition;
        private bool overflow_;
        private byte[] singleBuf = new byte[4];
        private byte[] doubleBuf = new byte[8];

        public int BitPosition
        {
            get
            {
                return this.bitPosition;
            }
        }
        public int StartBitPosition
        {
            get
            {
                return this.startBitPosition;
            }
        }
        public int EndBitPosition
        {
            get
            {
                return this.endBitPosition;
            }
        }
        public bool Overflow
        {
            get
            {
                return this.overflow_;
            }
        }

        public void Init(byte[] data, int bitOffset, int bitLength)
        {
            this.dataBytes = data;
            this.bitPosition = bitOffset;
            this.startBitPosition = this.bitPosition;
            this.endBitPosition = this.bitPosition + bitLength;
            this.overflow_ = false;
        }

        public bool Complete()
        {
            return this.bitPosition == this.endBitPosition && !this.overflow_;
        }

        public void ReadBuffer(byte[] destination, int byteOffset, int byteLength)
        {
            int num = this.bitPosition + byteLength * 8;
            if (this.overflow_ || num > this.endBitPosition)
            {
                this.overflow_ = true;
                return;
            }
            BytesUtil.ReadBytes(this.dataBytes, byteLength, this.bitPosition, destination, byteOffset);
            this.bitPosition = num;
        }

        public void ReadBuffer(byte[] destination)
        {
            this.ReadBuffer(destination, 0, destination.Length);
        }
        public bool ReadBoolean()
        {
            int num = this.bitPosition + 1;
            if (this.overflow_ || num > this.endBitPosition)
            {
                this.overflow_ = true;
                return false;
            }
            byte b = BytesUtil.ReadByte(this.dataBytes, 1, this.bitPosition);
            this.bitPosition = num;
            return b > 0;
        }
        public byte ReadByte()
        {
            int num = this.bitPosition + 8;
            if (this.overflow_ || num > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0;
            }
            byte result = BytesUtil.ReadByte(this.dataBytes, 8, this.bitPosition);
            this.bitPosition = num;
            return result;
        }
        public byte ReadByte(int numberOfBits)
        {
            int num = this.bitPosition + numberOfBits;
            if (this.overflow_ || num > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0;
            }
            byte result = BytesUtil.ReadByte(this.dataBytes, numberOfBits, this.bitPosition);
            this.bitPosition = num;
            return result;
        }
        public short ReadInt16()
        {
            int num = this.bitPosition + 16;
            if (this.overflow_ || num > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0;
            }
            uint num2 = BytesUtil.ReadUInt32(this.dataBytes, 16, this.bitPosition);
            this.bitPosition = num;
            return (short)num2;
        }
        public ushort ReadUInt16()
        {
            int num = this.bitPosition + 16;
            if (this.overflow_ || num > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0;
            }
            uint num2 = BytesUtil.ReadUInt32(this.dataBytes, 16, this.bitPosition);
            this.bitPosition = num;
            return (ushort)num2;
        }
        public int ReadInt32()
        {
            int num = this.bitPosition + 32;
            if (this.overflow_ || num > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0;
            }
            uint result = BytesUtil.ReadUInt32(this.dataBytes, 32, this.bitPosition);
            this.bitPosition = num;
            return (int)result;
        }
        public int ReadInt32(int numberOfBits)
        {
            int num = this.bitPosition + numberOfBits;
            if (this.overflow_ || num > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0;
            }
            uint num2 = BytesUtil.ReadUInt32(this.dataBytes, numberOfBits, this.bitPosition);
            this.bitPosition += numberOfBits;
            if (numberOfBits == 32)
            {
                return (int)num2;
            }
            int num3 = 1 << numberOfBits - 1;
            if (((ulong)num2 & (ulong)((long)num3)) == 0uL)
            {
                return (int)num2;
            }
            uint num4 = 4294967295u >> 33 - numberOfBits;
            uint num5 = (num2 & num4) + 1u;
            return (int)(-(int)num5);
        }
        public uint ReadUInt32()
        {
            int num = this.bitPosition + 32;
            if (this.overflow_ || num > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0u;
            }
            uint result = BytesUtil.ReadUInt32(this.dataBytes, 32, this.bitPosition);
            this.bitPosition += 32;
            return result;
        }
        public uint ReadUInt32(int numberOfBits)
        {
            int num = this.bitPosition + numberOfBits;
            if (this.overflow_ || num > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0u;
            }
            uint result = BytesUtil.ReadUInt32(this.dataBytes, numberOfBits, this.bitPosition);
            this.bitPosition += numberOfBits;
            return result;
        }
        public long ReadInt64()
        {
            int num = this.bitPosition + 64;
            if (this.overflow_ || num > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0L;
            }
            return (long)this.ReadUInt64();
        }
        public ulong ReadUInt64()
        {
            int num = this.bitPosition + 64;
            if (this.overflow_ || num > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0uL;
            }
            ulong num2 = (ulong)BytesUtil.ReadUInt32(this.dataBytes, 32, this.bitPosition);
            this.bitPosition += 32;
            ulong num3 = (ulong)BytesUtil.ReadUInt32(this.dataBytes, 32, this.bitPosition);
            this.bitPosition += 32;
            return num2 + (num3 << 32);
        }
        public ulong ReadUInt64(int numberOfBits)
        {
            int num = this.bitPosition + numberOfBits;
            if (this.overflow_ || num > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0uL;
            }
            ulong num2;
            if (numberOfBits <= 32)
            {
                num2 = (ulong)BytesUtil.ReadUInt32(this.dataBytes, numberOfBits, this.bitPosition);
            }
            else
            {
                num2 = (ulong)BytesUtil.ReadUInt32(this.dataBytes, 32, this.bitPosition);
                num2 |= (ulong)BytesUtil.ReadUInt32(this.dataBytes, numberOfBits - 32, this.bitPosition);
            }
            this.bitPosition += numberOfBits;
            return num2;
        }
        public long ReadInt64(int numberOfBits)
        {
            int num = this.bitPosition + numberOfBits;
            if (this.overflow_ || num > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0L;
            }
            return (long)this.ReadUInt64(numberOfBits);
        }

        public float ReadSingle()
        {
            int num = this.bitPosition + 32;
            if (this.overflow_ || num > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0f;
            }
            if ((this.bitPosition & 7) == 0)
            {
                float result = BitConverter.ToSingle(this.dataBytes, this.bitPosition >> 3);
                this.bitPosition += 32;
                return result;
            }
            this.ReadBuffer(this.singleBuf);
            if (this.overflow_)
            {
                return 0f;
            }
            return BitConverter.ToSingle(this.singleBuf, 0);
        }

        public double ReadDouble()
        {
            int num = this.bitPosition + 64;
            if (this.overflow_ || num > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0.0;
            }
            if ((this.bitPosition & 7) == 0)
            {
                double result = BitConverter.ToDouble(this.dataBytes, this.bitPosition >> 3);
                this.bitPosition += 64;
                return result;
            }
            this.ReadBuffer(this.doubleBuf);
            if (this.overflow_)
            {
                return 0.0;
            }
            return BitConverter.ToDouble(this.doubleBuf, 0);
        }
        /// <summary>
        /// Reads a UInt32 written using WriteUnsignedVarInt()
        /// </summary>
        public uint ReadVariableUInt32()
        {
            if (this.overflow_)
            {
                return 0u;
            }
            int num = 0;
            int num2 = 0;
            while (num2 != 35)
            {
                byte b = this.ReadByte();
                if (this.overflow_)
                {
                    return 0u;
                }
                num |= (int)(b & 127) << num2;
                num2 += 7;
                if ((b & 128) == 0)
                {
                    return (uint)num;
                }
            }
            this.overflow_ = true;
            return 0u;
        }
        /// <summary>
        /// Reads a Int32 written using WriteSignedVarInt()
        /// </summary>
        public int ReadVariableInt32()
        {
            if (this.overflow_)
            {
                return 0;
            }
            int num = 0;
            int num2 = 0;
            while (num2 != 35)
            {
                byte b = this.ReadByte();
                if (this.overflow_)
                {
                    return 0;
                }
                num |= (int)(b & 127) << num2;
                num2 += 7;
                if ((b & 128) == 0)
                {
                    int num3 = num << 31 >> 31;
                    return num3 ^ num >> 1;
                }
            }
            this.overflow_ = true;
            return 0;
        }
        /// <summary>
        /// Reads a UInt32 written using WriteUnsignedVarInt()
        /// </summary>
        public ulong ReadVariableUInt64()
        {
            if (this.overflow_)
            {
                return 0uL;
            }
            ulong num = 0uL;
            int num2 = 0;
            while (num2 != 119)
            {
                byte b = this.ReadByte();
                if (this.overflow_)
                {
                    return 0uL;
                }
                num |= ((ulong)b & 127uL) << num2;
                num2 += 7;
                if ((b & 128) == 0)
                {
                    return num;
                }
            }
            this.overflow_ = true;
            return 0uL;
        }
        /// <summary>
        /// Reads a float written using WriteSignedSingle()
        /// </summary>
        public float ReadSignedSingle(int numberOfBits)
        {
            if (this.overflow_ || this.bitPosition + numberOfBits > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0f;
            }
            uint num = this.ReadUInt32(numberOfBits);
            int num2 = (1 << numberOfBits) - 1;
            return ((num + 1u) / (float)(num2 + 1) - 0.5f) * 2f;
        }
        /// <summary>
        /// Reads a float written using WriteUnitSingle()
        /// </summary>
        public float ReadUnitSingle(int numberOfBits)
        {
            if (this.overflow_ || this.bitPosition + numberOfBits > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0f;
            }
            uint num = this.ReadUInt32(numberOfBits);
            int num2 = (1 << numberOfBits) - 1;
            return (num + 1u) / (float)(num2 + 1);
        }
        /// <summary>
        /// Reads a float written using WriteRangedSingle() using the same MIN and MAX values
        /// </summary>
        public float ReadRangedSingle(float min, float max, int numberOfBits)
        {
            if (this.overflow_ || this.bitPosition + numberOfBits > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0f;
            }
            float num = max - min;
            int num2 = (1 << numberOfBits) - 1;
            float num3 = this.ReadUInt32(numberOfBits);
            float num4 = num3 / (float)num2;
            return min + num4 * num;
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
        /// Reads an integer written using WriteRangedInteger() using the same min/max values
        /// </summary>
        public int ReadRangedInteger(int min, int max)
        {
            uint num = (uint)(max - min);
            int num2 = DataReader.A(num);
            if (this.overflow_ || this.bitPosition + num2 > this.endBitPosition)
            {
                this.overflow_ = true;
                return 0;
            }
            uint num3 = this.ReadUInt32(num2);
            return (int)((long)min + (long)((ulong)num3));
        }

        public string ReadString()
        {
            int num = (int)this.ReadVariableUInt32();
            if (num == 0)
            {
                return "";
            }
            if (this.overflow_ || this.bitPosition + num * 8 > this.endBitPosition)
            {
                this.overflow_ = true;
                return "";
            }
            if ((this.bitPosition & 7) == 0)
            {
                string @string = Encoding.UTF8.GetString(this.dataBytes, this.bitPosition >> 3, num);
                this.bitPosition += num * 8;
                return @string;
            }
            byte[] array = new byte[num];
            this.ReadBuffer(array);
            if (this.overflow_)
            {
                return "";
            }
            return Encoding.UTF8.GetString(array, 0, array.Length);
        }
        /// <summary>
        /// Pads data with enough bits to reach a full byte. Decreases cpu usage for subsequent byte writes.
        /// </summary>
        public void SkipPadBits()
        {
            this.bitPosition = (this.bitPosition + 7) / 8 * 8;
        }
        /// <summary>
        /// Pads data with the specified number of bits.
        /// </summary>
        public void SkipPadBits(int numberOfBits)
        {
            this.bitPosition += numberOfBits;
        }
        

    }
}
