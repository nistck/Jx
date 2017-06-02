using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Threading
{

    /// <summary>
    /// A delegate that represents the method to run as the work item
    /// </summary>
    /// <param name="state">A state object for the method to run</param>
    public delegate object WorkItemCallback(object state);

    /// <summary>
    /// A delegate to call after the WorkItemCallback completed
    /// </summary>
    /// <param name="wir">The work item result object</param>
    public delegate void PostExecuteWorkItemCallback(IWorkItemResult wir);

    /// <summary>
    /// A delegate to call after the WorkItemCallback completed
    /// </summary>
    /// <param name="wir">The work item result object</param>
    public delegate void PostExecuteWorkItemCallback<TResult>(IWorkItemResult<TResult> wir);

    /// <summary>
    /// A delegate to call when a WorkItemsGroup becomes idle
    /// </summary>
    /// <param name="workItemsGroup">A reference to the WorkItemsGroup that became idle</param>
    public delegate void WorkItemsGroupIdleHandler(IWorkItemsGroup workItemsGroup);

    /// <summary>
    /// A delegate to call after a thread is created, but before 
    /// it's first use.
    /// </summary>
    public delegate void ThreadInitializationHandler();

    /// <summary>
    /// A delegate to call when a thread is about to exit, after 
    /// it is no longer belong to the pool.
    /// </summary>
    public delegate void ThreadTerminationHandler();
}
