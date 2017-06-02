using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Threading
{
    /// <summary>
    /// IWorkItemsGroup interface
    /// Created by SmartThreadPool.CreateWorkItemsGroup()
    /// </summary>
    public interface IWorkItemsGroup
    {
        /// <summary>
        /// Get/Set the name of the WorkItemsGroup
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Get/Set the maximum number of workitem that execute cocurrency on the thread pool
        /// </summary>
        int Concurrency { get; set; }

        /// <summary>
        /// Get the number of work items waiting in the queue.
        /// </summary>
        int WaitingCallbacks { get; }

        /// <summary>
        /// Get an array with all the state objects of the currently running items.
        /// The array represents a snap shot and impact performance.
        /// </summary>
        object[] GetStates();

        /// <summary>
        /// Get the WorkItemsGroup start information
        /// </summary>
        WIGStartInfo WIGStartInfo { get; }

        /// <summary>
        /// Starts to execute work items
        /// </summary>
        void Start();

        /// <summary>
        /// Cancel all the work items.
        /// Same as Cancel(false)
        /// </summary>
        void Cancel();

        /// <summary>
        /// Cancel all work items using thread abortion
        /// </summary>
        /// <param name="abortExecution">True to stop work items by raising ThreadAbortException</param>
        void Cancel(bool abortExecution);

        /// <summary>
        /// Wait for all work item to complete.
        /// </summary>
		void WaitForIdle();

        /// <summary>
        /// Wait for all work item to complete, until timeout expired
        /// </summary>
        /// <param name="timeout">How long to wait for the work items to complete</param>
        /// <returns>Returns true if work items completed within the timeout, otherwise false.</returns>
		bool WaitForIdle(TimeSpan timeout);

        /// <summary>
        /// Wait for all work item to complete, until timeout expired
        /// </summary>
        /// <param name="millisecondsTimeout">How long to wait for the work items to complete in milliseconds</param>
        /// <returns>Returns true if work items completed within the timeout, otherwise false.</returns>
        bool WaitForIdle(int millisecondsTimeout);

        /// <summary>
        /// IsIdle is true when there are no work items running or queued.
        /// </summary>
        bool IsIdle { get; }

        /// <summary>
        /// This event is fired when all work items are completed.
        /// (When IsIdle changes to true)
        /// This event only work on WorkItemsGroup. On SmartThreadPool
        /// it throws the NotImplementedException.
        /// </summary>
        event WorkItemsGroupIdleHandler OnIdle;

        #region QueueWorkItem

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="callback">A callback to execute</param>
        /// <returns>Returns a work item result</returns>        
        IWorkItemResult QueueWorkItem(WorkItemCallback callback);

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="callback">A callback to execute</param>
        /// <param name="workItemPriority">The priority of the work item</param>
        /// <returns>Returns a work item result</returns>
        IWorkItemResult QueueWorkItem(WorkItemCallback callback, WorkItemPriority workItemPriority);

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="callback">A callback to execute</param>
        /// <param name="state">
        /// The context object of the work item. Used for passing arguments to the work item. 
        /// </param>
        /// <returns>Returns a work item result</returns>
        IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state);

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="callback">A callback to execute</param>
        /// <param name="state">
        /// The context object of the work item. Used for passing arguments to the work item. 
        /// </param>
        /// <param name="workItemPriority">The work item priority</param>
        /// <returns>Returns a work item result</returns>
        IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, WorkItemPriority workItemPriority);

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="callback">A callback to execute</param>
        /// <param name="state">
        /// The context object of the work item. Used for passing arguments to the work item. 
        /// </param>
        /// <param name="postExecuteWorkItemCallback">
        /// A delegate to call after the callback completion
        /// </param>
        /// <returns>Returns a work item result</returns>
        IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback);

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="callback">A callback to execute</param>
        /// <param name="state">
        /// The context object of the work item. Used for passing arguments to the work item. 
        /// </param>
        /// <param name="postExecuteWorkItemCallback">
        /// A delegate to call after the callback completion
        /// </param>
        /// <param name="workItemPriority">The work item priority</param>
        /// <returns>Returns a work item result</returns>
        IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, WorkItemPriority workItemPriority);

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="callback">A callback to execute</param>
        /// <param name="state">
        /// The context object of the work item. Used for passing arguments to the work item. 
        /// </param>
        /// <param name="postExecuteWorkItemCallback">
        /// A delegate to call after the callback completion
        /// </param>
        /// <param name="callToPostExecute">Indicates on which cases to call to the post execute callback</param>
        /// <returns>Returns a work item result</returns>
        IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, CallToPostExecute callToPostExecute);

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="callback">A callback to execute</param>
        /// <param name="state">
        /// The context object of the work item. Used for passing arguments to the work item. 
        /// </param>
        /// <param name="postExecuteWorkItemCallback">
        /// A delegate to call after the callback completion
        /// </param>
        /// <param name="callToPostExecute">Indicates on which cases to call to the post execute callback</param>
        /// <param name="workItemPriority">The work item priority</param>
        /// <returns>Returns a work item result</returns>
        IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, CallToPostExecute callToPostExecute, WorkItemPriority workItemPriority);

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="workItemInfo">Work item info</param>
        /// <param name="callback">A callback to execute</param>
        /// <returns>Returns a work item result</returns>
        IWorkItemResult QueueWorkItem(WorkItemInfo workItemInfo, WorkItemCallback callback);

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="workItemInfo">Work item information</param>
        /// <param name="callback">A callback to execute</param>
        /// <param name="state">
        /// The context object of the work item. Used for passing arguments to the work item. 
        /// </param>
        /// <returns>Returns a work item result</returns>
        IWorkItemResult QueueWorkItem(WorkItemInfo workItemInfo, WorkItemCallback callback, object state);

        #endregion

        #region QueueWorkItem(Action<...>)

        /// <summary>
        /// Queue a work item.
        /// </summary>
        /// <returns>Returns a IWorkItemResult object, but its GetResult() will always return null</returns>
        IWorkItemResult QueueWorkItem(Action action);

        /// <summary>
        /// Queue a work item.
        /// </summary>
        /// <returns>Returns a IWorkItemResult object, but its GetResult() will always return null</returns>
        IWorkItemResult QueueWorkItem(Action action, WorkItemPriority priority);

        /// <summary>
        /// Queue a work item.
        /// </summary>
        /// <returns>Returns a IWorkItemResult object, but its GetResult() will always return null</returns>
        IWorkItemResult QueueWorkItem<T>(Action<T> action, T arg, WorkItemPriority priority);

        /// <summary>
        /// Queue a work item.
        /// </summary>
        /// <returns>Returns a IWorkItemResult object, but its GetResult() will always return null</returns>
        IWorkItemResult QueueWorkItem<T>(Action<T> action, T arg);

        /// <summary>
        /// Queue a work item.
        /// </summary>
        /// <returns>Returns a IWorkItemResult object, but its GetResult() will always return null</returns>
        IWorkItemResult QueueWorkItem<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2);

        /// <summary>
        /// Queue a work item.
        /// </summary>
        /// <returns>Returns a IWorkItemResult object, but its GetResult() will always return null</returns>
        IWorkItemResult QueueWorkItem<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, WorkItemPriority priority);

        /// <summary>
        /// Queue a work item.
        /// </summary>
        /// <returns>Returns a IWorkItemResult object, but its GetResult() will always return null</returns>
        IWorkItemResult QueueWorkItem<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3);

        /// <summary>
        /// Queue a work item.
        /// </summary>
        /// <returns>Returns a IWorkItemResult object, but its GetResult() will always return null</returns>
        IWorkItemResult QueueWorkItem<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, WorkItemPriority priority);

        /// <summary>
        /// Queue a work item.
        /// </summary>
        /// <returns>Returns a IWorkItemResult object, but its GetResult() will always return null</returns>
        IWorkItemResult QueueWorkItem<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

        /// <summary>
        /// Queue a work item.
        /// </summary>
        /// <returns>Returns a IWorkItemResult object, but its GetResult() will always return null</returns>
        IWorkItemResult QueueWorkItem<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, WorkItemPriority priority);

        #endregion

        #region QueueWorkItem(Func<...>)

        /// <summary>
        /// Queue a work item.
        /// </summary>
        /// <returns>Returns a IWorkItemResult&lt;TResult&gt; object. 
        /// its GetResult() returns a TResult object</returns>
        IWorkItemResult<TResult> QueueWorkItem<TResult>(Func<TResult> func);

        /// <summary>
        /// Queue a work item.
        /// </summary>
        /// <returns>Returns a IWorkItemResult&lt;TResult&gt; object. 
        /// its GetResult() returns a TResult object</returns>
        IWorkItemResult<TResult> QueueWorkItem<T, TResult>(Func<T, TResult> func, T arg);

        /// <summary>
        /// Queue a work item.
        /// </summary>
        /// <returns>Returns a IWorkItemResult&lt;TResult&gt; object. 
        /// its GetResult() returns a TResult object</returns>
        IWorkItemResult<TResult> QueueWorkItem<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2);

        /// <summary>
        /// Queue a work item.
        /// </summary>
        /// <returns>Returns a IWorkItemResult&lt;TResult&gt; object. 
        /// its GetResult() returns a TResult object</returns>
        IWorkItemResult<TResult> QueueWorkItem<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3);

        /// <summary>
        /// Queue a work item.
        /// </summary>
        /// <returns>Returns a IWorkItemResult&lt;TResult&gt; object. 
        /// its GetResult() returns a TResult object</returns>
        IWorkItemResult<TResult> QueueWorkItem<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

        #endregion
    }
}
