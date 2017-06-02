using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jx.Threading
{
    /// <summary>
    /// IWorkItemResult interface.
    /// Created when a WorkItemCallback work item is queued.
    /// </summary>
    public interface IWorkItemResult : IWorkItemResult<object>
    {
    }


    /// <summary>
    /// IWorkItemResult&lt;TResult&gt; interface.
    /// Created when a Func&lt;TResult&gt; work item is queued.
    /// </summary>
    public interface IWorkItemResult<TResult> : IWaitableResult
    {
        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits.
        /// </summary>
        /// <returns>The result of the work item</returns>
        TResult GetResult();

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits until timeout.
        /// </summary>
        /// <returns>The result of the work item</returns>
        /// On timeout throws WorkItemTimeoutException
        TResult GetResult(
            int millisecondsTimeout,
            bool exitContext);

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits until timeout.
        /// </summary>
        /// <returns>The result of the work item</returns>
        /// On timeout throws WorkItemTimeoutException
        TResult GetResult(
            TimeSpan timeout,
            bool exitContext);

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits until timeout or until the cancelWaitHandle is signaled.
        /// </summary>
        /// <param name="millisecondsTimeout">Timeout in milliseconds, or -1 for infinite</param>
        /// <param name="exitContext">
        /// true to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it; otherwise, false. 
        /// </param>
        /// <param name="cancelWaitHandle">A cancel wait handle to interrupt the blocking if needed</param>
        /// <returns>The result of the work item</returns>
        /// On timeout throws WorkItemTimeoutException
        /// On cancel throws WorkItemCancelException
        TResult GetResult(
            int millisecondsTimeout,
            bool exitContext,
            WaitHandle cancelWaitHandle);

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits until timeout or until the cancelWaitHandle is signaled.
        /// </summary>
        /// <returns>The result of the work item</returns>
        /// On timeout throws WorkItemTimeoutException
        /// On cancel throws WorkItemCancelException
        TResult GetResult(
            TimeSpan timeout,
            bool exitContext,
            WaitHandle cancelWaitHandle);

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits.
        /// </summary>
        /// <param name="e">Filled with the exception if one was thrown</param>
        /// <returns>The result of the work item</returns>
        TResult GetResult(out Exception e);

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits until timeout.
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <param name="exitContext"></param>
        /// <param name="e">Filled with the exception if one was thrown</param>
        /// <returns>The result of the work item</returns>
        /// On timeout throws WorkItemTimeoutException
        TResult GetResult(
            int millisecondsTimeout,
            bool exitContext,
            out Exception e);

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits until timeout.
        /// </summary>
        /// <param name="exitContext"></param>
        /// <param name="e">Filled with the exception if one was thrown</param>
        /// <param name="timeout"></param>
        /// <returns>The result of the work item</returns>
        /// On timeout throws WorkItemTimeoutException
        TResult GetResult(
            TimeSpan timeout,
            bool exitContext,
            out Exception e);

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits until timeout or until the cancelWaitHandle is signaled.
        /// </summary>
        /// <param name="millisecondsTimeout">Timeout in milliseconds, or -1 for infinite</param>
        /// <param name="exitContext">
        /// true to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it; otherwise, false. 
        /// </param>
        /// <param name="cancelWaitHandle">A cancel wait handle to interrupt the blocking if needed</param>
        /// <param name="e">Filled with the exception if one was thrown</param>
        /// <returns>The result of the work item</returns>
        /// On timeout throws WorkItemTimeoutException
        /// On cancel throws WorkItemCancelException
        TResult GetResult(
            int millisecondsTimeout,
            bool exitContext,
            WaitHandle cancelWaitHandle,
            out Exception e);

        /// <summary>
        /// Get the result of the work item.
        /// If the work item didn't run yet then the caller waits until timeout or until the cancelWaitHandle is signaled.
        /// </summary>
        /// <returns>The result of the work item</returns>
        /// <param name="cancelWaitHandle"></param>
        /// <param name="e">Filled with the exception if one was thrown</param>
        /// <param name="timeout"></param>
        /// <param name="exitContext"></param>
        /// On timeout throws WorkItemTimeoutException
        /// On cancel throws WorkItemCancelException
        TResult GetResult(
            TimeSpan timeout,
            bool exitContext,
            WaitHandle cancelWaitHandle,
            out Exception e);

        /// <summary>
        /// Gets an indication whether the asynchronous operation has completed.
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Gets an indication whether the asynchronous operation has been canceled.
        /// </summary>
        bool IsCanceled { get; }

        /// <summary>
        /// Gets the user-defined object that contains context data 
        /// for the work item method.
        /// </summary>
        object State { get; }

        /// <summary>
        /// Same as Cancel(false).
        /// </summary>
        bool Cancel();

        /// <summary>
        /// Cancel the work item execution.
        /// If the work item is in the queue then it won't execute
        /// If the work item is completed, it will remain completed
        /// If the work item is in progress then the user can check the SmartThreadPool.IsWorkItemCanceled
        ///   property to check if the work item has been cancelled. If the abortExecution is set to true then
        ///   the Smart Thread Pool will send an AbortException to the running thread to stop the execution 
        ///   of the work item. When an in progress work item is canceled its GetResult will throw WorkItemCancelException.
        /// If the work item is already cancelled it will remain cancelled
        /// </summary>
        /// <param name="abortExecution">When true send an AbortException to the executing thread.</param>
        /// <returns>Returns true if the work item was not completed, otherwise false.</returns>
        bool Cancel(bool abortExecution);

        /// <summary>
        /// Get the work item's priority
        /// </summary>
        WorkItemPriority WorkItemPriority { get; }

        /// <summary>
        /// Return the result, same as GetResult()
        /// </summary>
        TResult Result { get; }

        /// <summary>
        /// Returns the exception if occured otherwise returns null.
        /// </summary>
        object Exception { get; }
    }
}
