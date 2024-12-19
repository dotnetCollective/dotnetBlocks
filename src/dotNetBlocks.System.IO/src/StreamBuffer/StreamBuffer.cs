using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{


    /// <summary>
    /// Buffer class controlling flow of data between reader and writer stream, limiting the amount of data in the buffer.
    /// </summary>
    /// <remarks> This class uses the <see cref="_pipe"/> class as the underlying implementation and wraps helper methods for asynchronous functionality.
    /// To prevent becoming blocked or locked, read this article.     https://learn.microsoft.com/en-us/dotnet/standard/io/pipelines#backpressure-and-flow-control
    /// </remarks>
    public partial class StreamBuffer : IAsyncDisposable, IDisposable, IStreamBuffer
    {


        #region Pipe
        private const double DefaultResumePercentBufferUsed = 0.75;
        private const long DefaultBufferSize = 65536; // 64k
        private Pipe? _pipe;
        // Streams
        public Stream ReadStream => _disposed ? throw new ObjectDisposedException(nameof(StreamBuffer)) : _pipe!.Reader.AsStream();
        public Stream WriteStream => _disposed ? throw new ObjectDisposedException(nameof(StreamBuffer)) : _pipe!.Writer.AsStream();
        #endregion


        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamBuffer"/> class.
        /// </summary>
        public StreamBuffer(PipeOptions pipeOptions)
        {
            _pipe = new Pipe(pipeOptions);

            // Initialize the background control structures.

            // Initialize the master background token.
            CancelAllBackgroundTasksTokenSource = new CancellationTokenSource();

            // Initialize Background Read Write tokens and join to the Background Master token;
            CancelBackgroundReadTokenSource = CancelAllBackgroundTasksTokenSource.Token.CreateLinkedTokenSource();
            CancelBackgroundWriteTokenSource = CancelAllBackgroundTasksTokenSource.Token.CreateLinkedTokenSource();

        }

        /// <remarks> The buffer stops writing and connot completely at EXACTLY buffer size bytes.
        /// </remarks>
        /// </summary>
        /// <param name="bufferSize">Size of the buffer.
        ///         /// Default buffer size is 64k

        /// </param>
        public StreamBuffer(long? bufferSize = DefaultBufferSize, double? resumePercentBufferUsed = DefaultResumePercentBufferUsed)
            : this(CalculatePipeOptions(bufferSize, resumePercentBufferUsed))
        {

        }
        // ToDo: Rework this with a buffer taskOptions class.
        private static PipeOptions CalculatePipeOptions(long? bufferSize = DefaultBufferSize, double? resumePercentBufferUsed = DefaultResumePercentBufferUsed)
        {
            // Configure the taskOptions based on the buffer.
            bufferSize ??= DefaultBufferSize;
            resumePercentBufferUsed ??= DefaultResumePercentBufferUsed;

            long pauseWriterThreshold = bufferSize.Value;
            long resumeWriteThreshold = (long)(pauseWriterThreshold * resumePercentBufferUsed);

            // Create the pipe for this buffer;
            return new PipeOptions(pauseWriterThreshold: pauseWriterThreshold, resumeWriterThreshold: resumeWriteThreshold);

        }



        #region IDisposable

        public async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await DisposeAsyncCore().ConfigureAwait(false);

            // Dispose of unmanaged resources.
            Dispose(false);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Over-ridable implemention for async dispose by implementors.
        /// </summary>
        /// <returns></returns>
        protected virtual async ValueTask DisposeAsyncCore()
        {
            // Cancal and wait for all background tasks.
            await CancelBackgroundAsync();
            await ValueTask.CompletedTask;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)

                    // We Assume that DisposeAsync is already called.
                    // We assume that background threads
                    // // had a chance to finish.

                    // Clean up the pipe
                    // Dispose the pipe streams to removing blocking.
                    ReadStream.Dispose();
                    WriteStream.Dispose();

                    // Release a reference to the pipe
                    _pipe = null;

                    // Dispose all token sources.
                    CancelAllBackgroundTasksTokenSource.Dispose();
                    CancelBackgroundReadTokenSource.Dispose();
                    CancelBackgroundWriteTokenSource.Dispose();
                    // Release Tasks.
                    backgroundReadTask?.Dispose();
                    backgroundReadTask = null;

                    backgroundWriteTask?.Dispose();
                    backgroundWriteTask = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposed = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~StreamBuffer()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion


    }
}
