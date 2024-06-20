using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{


    /// <summary>
    /// Buffer class controlling flow of data between reader and writer stream, limiting the amount of data in the buffer.
    /// </summary>
    /// <remarks> This class uses the <see cref="_pipe"/> class as the underlying implementation and wraps helper methods for asynchronous functionality.</remarks>
    public class StreamBuffer
    {
        // ActionDelegateDefinitions.
        public delegate void bufferAction(Stream stream, CancellationToken cancellationToken);
        public delegate Task bufferActionAsync(Stream stream, CancellationToken cancellationToken);


        private const long DefaultBufferSize = 65536; // 64k
        private readonly Pipe _pipe;

        /// <summary>
        /// Default buffer size is 64k
        /// </summary>
        /// <param name="bufferSize"></param>
        public StreamBuffer(long? bufferSize = DefaultBufferSize)
        {
            // Configure the options based on the buffer.
            long pauseWriterThreshold = bufferSize ?? DefaultBufferSize;
            long resumeWriterThreshold = pauseWriterThreshold / 2;

            // Default resume writing at half-buffer size.

            // Create the pipe for this buffer;
            PipeOptions options = new PipeOptions(pauseWriterThreshold: pauseWriterThreshold, resumeWriterThreshold: resumeWriterThreshold);
            _pipe = new Pipe(options);
        }

        // Streams
        public Stream ReadStream => _pipe.Reader.AsStream();
        public Stream WriteStream => _pipe.Writer.AsStream();

        // Background processes

        #region Background Write
        public Task backgroundWriteTask { get; private set; } = Task.CompletedTask;
        public void StartBackgroundWrite(bufferAction? writeAction = default, bufferActionAsync? writeActionAsync = default, CancellationToken cancellationToken = default)
        {
            // Start the async action.
            if (writeActionAsync is not null)
            {
                backgroundWriteTask = RunAsyncAction(writeActionAsync, WriteStream, cancellationToken);
                return;
            }
            // Start the sync action

            // can't supply two actions.
            if (writeAction is null) throw new ArgumentException(nameof(writeAction));
            backgroundWriteTask = RunAction(writeAction!, WriteStream, cancellationToken);
        }
        #endregion

        #region helper methods
        private static Task RunAction(bufferAction action, Stream stream, CancellationToken cancellationToken)
        {
            return Task.Run(
                () => 
                    { action(stream, cancellationToken);
                    }
                    ,cancellationToken);
        }

        private async static Task RunAsyncAction(bufferActionAsync action, Stream stream, CancellationToken cancellationToken)
        => await action(stream, cancellationToken);

        #endregion


    }
}
