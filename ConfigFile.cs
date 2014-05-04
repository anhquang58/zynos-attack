using System;
using System.IO;
using System.Text;

namespace ZyXEL_Firmware
{
    public class ConfigFile
    {
        private readonly byte[] bytes;
        public Block[] blocks;

        public ConfigFile(string path)
        {
            bytes = File.ReadAllBytes(path);
            blocks = new Block[2];
            blocks[0] = new Block(bytes, 0x0000);
            blocks[1] = new Block(bytes, 0x2000);
        }
    }

    public class Block
    {
        private readonly byte[] data;
        private readonly int length;
        private int blockNumber;
        private int count;

        private DirEntry[] dirEntries;

        public Block(byte[] file, int offset)
        {
            blockNumber = file[offset];
            count = file[offset + 2] << 8 | file[offset + 3];
            length = file[offset + 4] << 24 | file[offset + 5] << 16 | file[offset + 6] << 8 | file[offset + 7];
            data = new byte[length];
            Array.Copy(file, offset, data, 0, data.Length);
            dirEntries = new DirEntry[count];
            for (int i = 0; i < count; i++)
            {
                var entry = new byte[26];
                Array.Copy(data, 8 + 26 * i, entry, 0, entry.Length);
                dirEntries[i] = new DirEntry(entry);
            }
        }

        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        public DirEntry[] DirEntries
        {
            get { return dirEntries; }
            set { dirEntries = value; }
        }

        public byte[] GetData(DirEntry entry)
        {
            var entryData = new byte[entry.CompressedSize];
            Array.Copy(data, entry.DataOffset, entryData, 0, entryData.Length);
            return entryData;
        }
    }

    public class DirEntry
    {
        private int compressedSize;

        public DirEntry(byte[] bytes)
        {
            var sb = new StringBuilder();
            int i = 0;
            while (bytes[i] != 0)
            {
                sb.Append((char)bytes[i++]);
            }
            Name = sb.ToString();
            UncompressedSize = bytes[14] << 24 | bytes[15] << 16 | bytes[16] << 8 | bytes[17];
            compressedSize = bytes[18] << 24 | bytes[19] << 16 | bytes[20] << 8 | bytes[21];
            Compressed = (compressedSize != 0);
            DataOffset = bytes[22] << 24 | bytes[23] << 16 | bytes[24] << 8 | bytes[25];
        }

        public string Name { get; set; }

        public int UncompressedSize { get; set; }

        public int CompressedSize
        {
            get { return compressedSize; }
            set { compressedSize = value; }
        }

        public bool Compressed { get; set; }

        public int DataOffset { get; set; }
    }
}
