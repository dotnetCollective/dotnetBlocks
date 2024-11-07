using System;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace StreamBufferTests
{
    static internal class StreamBufferTestHelper
    {
        /// <summary>
        /// Reads the data and calculates the CRC.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="totalBytes">The total bytes to read.</param>
        /// <param name="crc">The CRC.</param>
        public static void ReadAndCalculateCRC(this Stream stream, Int32 totalBytes, Crc32 crc)
        {
            byte[] data = new byte[1];
            while (totalBytes > 0)
            {
                stream.Read(data);
                crc.Append(data);
                totalBytes -= data.Length;
            }
        }


        /// <summary>
        /// Executes the action with a timeout
        /// </summary>
        /// 
        /// <param name="action">Action tp execute</param>
        /// <param name="timeout">timespan to timeout.</param>
        /// <returns>true if timed out otherwise false</returns>
        public static async Task<bool> ExecuteWithTimeoutAsync(Action action, TimeSpan timeout)
        {
            var actionTask = Task.Run(action);
            var x = Task.Delay(timeout);
            var delayTask = Task.Run(async () => { await Task.Delay(timeout); });
            // Wait for both tasks and if we time out then assume the action is blocked.
            await Task.WhenAny( actionTask, delayTask);

            return delayTask.IsCompleted && !actionTask.IsCompleted;
        }
    }
}
