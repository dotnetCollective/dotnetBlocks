using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        #region DisposeContinuations
        /// <summary>
        /// Disposes resources after task completion.
        /// </summary>
        /// <param name="taskusingdispoable">The <see cref="Task"/>> using the resource</param>
        /// <param name="disposable"><see cref="IDisposable" /> resource to dispose. </param>
        /// <param name="cancelDispose"><see cref="CancellationToken"/> cancel the dispose process.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static Task DisposeAfter(this Task taskusingdispoable, IDisposable disposable, CancellationToken cancelDispose = default)
        {
            ArgumentNullException.ThrowIfNull(disposable);
            ArgumentNullException.ThrowIfNull(taskusingdispoable);
            return taskusingdispoable.ContinueWith((t) => disposable.Dispose(), cancelDispose);
        }

        /// <summary>
        /// Disposes resources after task completion.
        /// </summary>
        /// <param name="taskusingdisposable">The <see cref="Task"/>> using the resource</param>
        /// <param name="disposable"><see cref="IDisposable" /> resource to dispose./></param>
        /// <param name="cancelDispose"><see cref="CancellationToken"/> cancel the dispose process.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async static Task DisposeAfterAsync(this Task taskusingdisposable, IAsyncDisposable disposable, CancellationToken cancelDispose = default)
        {
            ArgumentNullException.ThrowIfNull(disposable);
            ArgumentNullException.ThrowIfNull(taskusingdisposable);
            await taskusingdisposable.ContinueWith(async (t) => await disposable.DisposeAsync(), cancelDispose);
        }
        #endregion

        #region Task Wait

        /// <summary>
        /// Waits asynchronously for all tasks to complete
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="waitTime">The wait time.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="tasks">The tasks.</param>
        /// <returns></returns>
        public async static Task<bool> WaitAll(this Task task, TimeSpan waitTime, CancellationToken cancellationToken = default, params Task[] tasks)
            => await WaitAllAsync(new Task[] { task}.Union(tasks), waitTime, cancellationToken);
        /// <summary>
        /// Waits asynchronously for all tasks to complete
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        /// <param name="waitTime">The wait time.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async static Task<bool> WaitAllAsync(this Task[] tasks, TimeSpan waitTime, CancellationToken cancellationToken = default)
            => await WaitAllAsync(tasks.AsEnumerable(), waitTime, cancellationToken);

        /// <summary>
        /// Waits asynchronously for all tasks to complete
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        /// <param name="waitTime">The wait time.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async static Task<bool> WaitAllAsync(this IEnumerable<Task> tasks, TimeSpan waitTime, CancellationToken cancellationToken = default)
            =>  await Task.FromResult(Task.WaitAll(tasks.ToArray(), (int)waitTime.TotalMilliseconds, cancellationToken));

        /// <summary>
        /// Waits asynchronously for any tasks to complete
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="waitTime">The wait time.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="tasks">The tasks.</param>
        /// <returns></returns>
        public async static Task<int> WaitAnyAsync(this Task task, TimeSpan waitTime, CancellationToken cancellationToken = default, params Task[] tasks)
            => await WaitAnyAsync(new Task[] { task }.Append(task), waitTime, cancellationToken);

        /// <summary>
        /// Waits asynchronously for any tasks to complete
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        /// <param name="waitTime">The wait time.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async static Task<int> WaitAnyAsync(this Task[] tasks, TimeSpan waitTime, CancellationToken cancellationToken = default)
            => await WaitAnyAsync(tasks.AsEnumerable(), waitTime, cancellationToken);

        /// <summary>
        /// Waits asynchronously for all tasks to complete
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        /// <param name="waitTime">The wait time.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async static Task<int> WaitAnyAsync(this IEnumerable<Task> tasks, TimeSpan waitTime, CancellationToken cancellationToken = default)
            =>  await Task.FromResult(Task.WaitAny(tasks.ToArray(), (int)waitTime.TotalMilliseconds, cancellationToken));

        #endregion

    }
}
