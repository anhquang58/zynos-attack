using System;
using System.Collections;

namespace ZyXEL_Firmware
{
    public class BitReader
    {
        private readonly BitArray bits;
        private int index;

        public BitReader(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                byte a = bytes[i];
                byte b = 0;
                for (int j = 0; j < 8; j++)
                {
                    b <<= 1;
                    b += (byte)((a >> j) & 1);
                }
                bytes[i] = b;
            }
            bits = new BitArray(bytes);
            index = 0;
        }

        public bool ReadBit()
        {
            return bits[index++];
        }

        public byte ReadByte()
        {
            return Convert.ToByte(ReadBits(8));
        }

        public int ReadBits(int count)
        {
            int result = 0;
            for (int i = 0; i < count; i++)
            {
                result <<= 1;
                result += Convert.ToByte(ReadBit());
            }
            return result;
        }
    }
}
