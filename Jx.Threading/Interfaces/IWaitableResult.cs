using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Threading
{
    /// <summary>
    /// The common interface of IWorkItemResult and IWorkItemResult&lt;T&gt;
    /// </summary>
    public interface IWaitableResult
    {
        /// <summary>
        /// This method intent is for internal use.
        /// </summary>
        /// <returns></returns>
        IWorkItemResult GetWorkItemResult();

        /// <summary>
        /// This method intent is for internal use.
        /// </summary>
        /// <returns></returns>
        IWorkItemResult<TResult> GetWorkItemResultT<TResult>();
    }
}
