using System;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamBufferTests
{
    internal class RandomStream : MemoryStream
    {
        public Random dataGenerator = new();
        public Crc32 CRC = new();

        public RandomStream(Int32 totalBytes)
            => Fill(totalBytes);

        public void Fill(Int32 totalBytes)
        {
            byte[] data = new byte[1];
            while(totalBytes > 0)
            {
                dataGenerator.NextBytes(data);
                Write(data);
                CRC.Append(data);
                totalBytes-= data.Length;
            }
            Position = 0;
        }

    }
}
