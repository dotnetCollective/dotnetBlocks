
namespace System.IO
{
    public interface IStreamBuffer
    {
        Task? backgroundReadTask { get; }
        Task? backgroundWriteTask { get; }
        bool IsBackgroundReadTaskAssigned { get; }
        bool IsBackgroundWriteTaskAssigned { get; }
        Stream ReadStream { get; }
        Stream WriteStream { get; }

        Task CancelBackgroundAsync(TimeSpan? waitTime = null, CancellationToken cancellationToken = default);
        Task CancelBackgroundReadAsync(TimeSpan? timeout = null, CancellationToken cancelWait = default);
        Task CancelBackgroundWriteAsync(TimeSpan? timeout = null, CancellationToken cancelWait = default);
        void Dispose();
        ValueTask DisposeAsync();
        Task StartBackgroundRead(Action<Stream, CancellationToken> action, CancellationToken cancellationToken = default);
        Task StartBackgroundRead(Action<Stream> action, CancellationToken cancellationToken = default);
        Task StartBackgroundRead(StreamBuffer.BackgroundAction action, TaskCreationOptions? options = null, CancellationToken cancellationToken = default);
        Task StartBackgroundRead(Func<Stream, CancellationToken, Task> func, CancellationToken cancellationToken = default);
        Task StartBackgroundWrite(Action<Stream, CancellationToken> action, CancellationToken cancellationToken = default);
        Task StartBackgroundWrite(Action<Stream> action, CancellationToken cancellationToken = default);
        Task StartBackgroundWrite(StreamBuffer.BackgroundAction action, TaskCreationOptions? options = null, CancellationToken cancellationToken = default);
        Task StartBackgroundWrite(Func<Stream, CancellationToken, Task> func, CancellationToken cancellationToken = default);
        Task WaitForBackgroundAsync(TimeSpan? waitTime = null, CancellationToken cancellationToken = default);
        Task WaitForBackgroundReadAsync(TimeSpan? timeout = null, CancellationToken cancelWait = default);
        Task WaitForBackgroundWriteAsync(TimeSpan? timeout = null, CancellationToken cancelWait = default);
    }
}