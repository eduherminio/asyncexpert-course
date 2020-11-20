using System;
using System.Threading;

namespace ThreadPoolExercises.Core
{
    public class ThreadingHelpers
    {
        /// <summary>
        /// * Create a thread and execute there `action` given number of `repeats` - waiting for the execution!
        ///   HINT: you may use `Join` to wait until created Thread finishes
        /// * In a loop, check whether `token` is not cancelled
        /// * If an `action` throws and exception (or token has been cancelled) - `errorAction` should be invoked (if provided)
        /// </summary>
        /// <param name="action"></param>
        /// <param name="repeats"></param>
        /// <param name="token"></param>
        /// <param name="errorAction"></param>
        public static void ExecuteOnThread(Action action, int repeats, CancellationToken token = default, Action<Exception>? errorAction = null)
        {
            var thread = new Thread(() => MyAction(action, repeats, token, errorAction, null))
            {
                IsBackground = true
            };

            thread.Start();
            thread.Join();
        }


        /// <summary>
        /// * Queue work item to a thread pool that executes `action` given number of `repeats` - waiting for the execution!
        ///   HINT: you may use `AutoResetEvent` to wait until the queued work item finishes
        /// * In a loop, check whether `token` is not cancelled
        /// * If an `action` throws and exception (or token has been cancelled) - `errorAction` should be invoked (if provided)
        /// </summary>
        /// <param name="action"></param>
        /// <param name="repeats"></param>
        /// <param name="token"></param>
        /// <param name="errorAction"></param>
        public static void ExecuteOnThreadPool(Action action, int repeats, CancellationToken token = default, Action<Exception>? errorAction = null)
        {
            using var autoresetEvent = new AutoResetEvent(false);

            ThreadPool.QueueUserWorkItem((_) => MyAction(action, repeats, token, errorAction, autoresetEvent));

            autoresetEvent.WaitOne();
        }

        private static void MyAction(Action action, int repeats, CancellationToken token, Action<Exception>? errorAction, AutoResetEvent? autoresetEvent)
        {
            try
            {
                for (int i = 0; i < repeats; ++i)
                {
                    token.ThrowIfCancellationRequested();
                    action.Invoke();
                }
            }
            catch (Exception e)
            {
                errorAction?.Invoke(e);
            }
            finally
            {
                autoresetEvent?.Set();
            }
        }
    }
}
