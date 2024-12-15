using System;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace StreamBufferTests
{
    internal class RandomStream : MemoryStream
    {
        public Random dataGenerator = new();

        public RandomStream(Int32 totalBytes)
            => Fill(totalBytes);

        public void Fill(Int32 totalBytes)
        {
            byte[] data = new byte[1];
            while (Position < totalBytes)
            {
                dataGenerator.NextBytes(data);
                Write(data);
            }
            Position = 0;
        }

        private readonly Crc32 _crc = new();

        /// <summary>
        /// Gets the CRC.
        /// </summary>
        /// <remarks> Calculates the CRC from the start of the stream to the current position inclusive.</remarks>
        /// <value>
        /// The CRC.
        /// </value>
        public Crc32 CRC
        {
            get
            {
                var position = Position; // Store the stream position.
                _crc.Reset(); // Reset the CRC.

                if (Length == 0) return _crc; // empty set.

                Position = 0; // move to start of stream;
                int readByte = 0;

                while (readByte != -1)
                {
                    readByte = ReadByte();
                    if (readByte == -1) // End of stream so exit;
                        break;
                    _crc.Append(new byte[] { (byte)readByte});

                    if (Position >= position) break; // We are where we started.

                }                
                Position = position; // Ensure we are reset.
                return _crc;
            }
        }

    }
}