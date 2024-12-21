using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;

namespace System.IO
{

    public record StreamBufferOptions(long BufferSize = 65536, double? ResumePercentBufferUsed = 0.75)
    {
        public const long DefaultBufferSize = 65536; // Default buffer size is 64k
        public const double DefaultResumePercentBufferUsed = 0.75;

        //public long BufferSize { get; init; } = DefaultBufferSize;
        //double? ResumePercentBufferUsed { get ; init; } = DefaultResumePercentBufferUsed; // Default resume percent buffer used


        private PipeOptions? _pipeOptions = null;

        [NotNull()]
        public PipeOptions? PipeOptions { get => _pipeOptions ?? BuildPipeOptions(); set => _pipeOptions = value; }


        private PipeOptions BuildPipeOptions()
        {
            // Configure the taskOptions based on the buffer sizes.

            long pauseWriterThreshold = BufferSize; // Block when you hit the buffer max,
            long resumeWriteThreshold = (long)(pauseWriterThreshold * DefaultResumePercentBufferUsed);

            // Create the pipe for this buffer;
            return new PipeOptions(pauseWriterThreshold: pauseWriterThreshold, resumeWriterThreshold: resumeWriteThreshold);
        }
    }

}