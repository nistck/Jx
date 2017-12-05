using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Ext
{
    internal static class BytesUtil
    {
        public static byte ReadByte(byte[] fromBuffer, int numberOfBits, int readBitOffset)
        {
            int num = readBitOffset >> 3;
            int num2 = readBitOffset - num * 8;
            if (num2 == 0 && numberOfBits == 8)
            {
                return fromBuffer[num];
            }
            byte b = (byte)(fromBuffer[num] >> num2);
            int num3 = numberOfBits - (8 - num2);
            if (num3 < 1)
            {
                return (byte)((int)b & 255 >> 8 - numberOfBits);
            }
            byte b2 = fromBuffer[num + 1];
            b2 &= (byte)(255 >> 8 - num3);
            return (byte) (b | (byte)(b2 << numberOfBits - num3));
        }

        public static void ReadBytes(byte[] fromBuffer, int numberOfBytes, int readBitOffset, byte[] destination, int destinationByteOffset)
        {
            int num = readBitOffset >> 3;
            int num2 = readBitOffset - num * 8;
            if (num2 == 0)
            {
                for (int i = 0; i < numberOfBytes; i++)
                {
                    destination[destinationByteOffset++] = fromBuffer[num++];
                }
                return;
            }
            int num3 = 8 - num2;
            int num4 = 255 >> num3;
            for (int j = 0; j < numberOfBytes; j++)
            {
                int num5 = fromBuffer[num] >> num2;
                num++;
                int num6 = (int)fromBuffer[num] & num4;
                destination[destinationByteOffset++] = (byte)(num5 | num6 << num3);
            }
        }
        public static void WriteByte(byte source, int numberOfBits, byte[] destination, int destBitOffset)
        {
            uint num = (uint)source & 4294967295u >> 8 - numberOfBits;
            int num2 = destBitOffset >> 3;
            int num3 = destBitOffset % 8;
            if (num3 == 0)
            {
                destination[num2] = (byte)num;
                return;
            }
            destination[num2] = (byte)(((int)destination[num2] & 255 >> 8 - num3) | (int)((byte)(num << num3)));
            if (num3 + numberOfBits > 8)
            {
                destination[num2 + 1] = (byte)(((int)destination[num2 + 1] & 255 << num3) | (int)((byte)(num >> 8 - num3)));
            }
        }
        public static void WriteBytes(byte[] source, int sourceByteOffset, int numberOfBytes, byte[] destination, int destBitOffset)
        {
            int num = destBitOffset >> 3;
            int num2 = destBitOffset % 8;
            if (num2 == 0)
            {
                for (int i = 0; i < numberOfBytes; i++)
                {
                    destination[num++] = source[sourceByteOffset + i];
                }
                return;
            }
            int num3 = 8 - num2;
            for (int j = 0; j < numberOfBytes; j++)
            {
                byte b = source[sourceByteOffset + j];
                int expr_41_cp_1 = num;
                destination[expr_41_cp_1] &= (byte)(255 >> num3);
                int expr_60_cp_1 = num;
                destination[expr_60_cp_1] |= (byte)(b << num2);
                num++;
                int expr_80_cp_1 = num;
                destination[expr_80_cp_1] &= (byte)(255 << num2);
                int expr_9F_cp_1 = num;
                destination[expr_9F_cp_1] |= (byte)(b >> num3);
            }
        }
        public unsafe static uint ReadUInt32(byte[] fromBuffer, int numberOfBits, int readBitOffset)
        {
            if (numberOfBits == 32 && readBitOffset % 8 == 0)
            {
                return *(uint*)(fromBuffer[readBitOffset / 8]);
            }
            if (numberOfBits <= 8)
            {
                return (uint)BytesUtil.ReadByte(fromBuffer, numberOfBits, readBitOffset);
            }
            uint num = (uint)BytesUtil.ReadByte(fromBuffer, 8, readBitOffset);
            numberOfBits -= 8;
            readBitOffset += 8;
            if (numberOfBits <= 8)
            {
                return num | (uint)((uint)BytesUtil.ReadByte(fromBuffer, numberOfBits, readBitOffset) << 8);
            }
            num |= (uint)((uint)BytesUtil.ReadByte(fromBuffer, 8, readBitOffset) << 8);
            numberOfBits -= 8;
            readBitOffset += 8;
            if (numberOfBits <= 8)
            {
                uint num2 = (uint)BytesUtil.ReadByte(fromBuffer, numberOfBits, readBitOffset);
                num2 <<= 16;
                return num | num2;
            }
            num |= (uint)((uint)BytesUtil.ReadByte(fromBuffer, 8, readBitOffset) << 16);
            numberOfBits -= 8;
            readBitOffset += 8;
            return num | (uint)((uint)BytesUtil.ReadByte(fromBuffer, numberOfBits, readBitOffset) << 24);
        }
        public static ulong ReadUInt64(byte[] fromBuffer, int numberOfBits, int readBitOffset)
        {
            throw new NotImplementedException();
        }
        public static int WriteUInt32(uint source, int numberOfBits, byte[] destination, int destinationBitOffset)
        {
            int result = destinationBitOffset + numberOfBits;
            if (numberOfBits <= 8)
            {
                BytesUtil.WriteByte((byte)source, numberOfBits, destination, destinationBitOffset);
                return result;
            }
            BytesUtil.WriteByte((byte)source, 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;
            if (numberOfBits <= 8)
            {
                BytesUtil.WriteByte((byte)(source >> 8), numberOfBits, destination, destinationBitOffset);
                return result;
            }
            BytesUtil.WriteByte((byte)(source >> 8), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;
            if (numberOfBits <= 8)
            {
                BytesUtil.WriteByte((byte)(source >> 16), numberOfBits, destination, destinationBitOffset);
                return result;
            }
            BytesUtil.WriteByte((byte)(source >> 16), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;
            BytesUtil.WriteByte((byte)(source >> 24), numberOfBits, destination, destinationBitOffset);
            return result;
        }
        public static int WriteUInt64(ulong source, int numberOfBits, byte[] destination, int destinationBitOffset)
        {
            int result = destinationBitOffset + numberOfBits;
            if (numberOfBits <= 8)
            {
                BytesUtil.WriteByte((byte)source, numberOfBits, destination, destinationBitOffset);
                return result;
            }
            BytesUtil.WriteByte((byte)source, 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;
            if (numberOfBits <= 8)
            {
                BytesUtil.WriteByte((byte)(source >> 8), numberOfBits, destination, destinationBitOffset);
                return result;
            }
            BytesUtil.WriteByte((byte)(source >> 8), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;
            if (numberOfBits <= 8)
            {
                BytesUtil.WriteByte((byte)(source >> 16), numberOfBits, destination, destinationBitOffset);
                return result;
            }
            BytesUtil.WriteByte((byte)(source >> 16), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;
            if (numberOfBits <= 8)
            {
                BytesUtil.WriteByte((byte)(source >> 24), numberOfBits, destination, destinationBitOffset);
                return result;
            }
            BytesUtil.WriteByte((byte)(source >> 24), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;
            if (numberOfBits <= 8)
            {
                BytesUtil.WriteByte((byte)(source >> 32), numberOfBits, destination, destinationBitOffset);
                return result;
            }
            BytesUtil.WriteByte((byte)(source >> 32), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;
            if (numberOfBits <= 8)
            {
                BytesUtil.WriteByte((byte)(source >> 40), numberOfBits, destination, destinationBitOffset);
                return result;
            }
            BytesUtil.WriteByte((byte)(source >> 40), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;
            if (numberOfBits <= 8)
            {
                BytesUtil.WriteByte((byte)(source >> 48), numberOfBits, destination, destinationBitOffset);
                return result;
            }
            BytesUtil.WriteByte((byte)(source >> 48), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;
            if (numberOfBits <= 8)
            {
                BytesUtil.WriteByte((byte)(source >> 56), numberOfBits, destination, destinationBitOffset);
                return result;
            }
            BytesUtil.WriteByte((byte)(source >> 56), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;
            return result;
        }
        public static int WriteVariableUInt32(byte[] destination, int offset, uint value)
        {
            int num = 0;
            uint num2 = value;
            while (num2 >= 128u)
            {
                destination[offset + num] = (byte)(num2 | 128u);
                num2 >>= 7;
                num++;
            }
            destination[offset + num] = (byte)num2;
            return num + 1;
        }
        public static uint ReadVariableUInt32(byte[] buffer, ref int offset)
        {
            int num = 0;
            int num2 = 0;
            while (num2 != 35)
            {
                byte b = buffer[offset++];
                num |= (int)(b & 127) << num2;
                num2 += 7;
                if ((b & 128) == 0)
                {
                    return (uint)num;
                }
            }
            throw new FormatException("Bad 7-bit encoded integer");
        }
    }
}
