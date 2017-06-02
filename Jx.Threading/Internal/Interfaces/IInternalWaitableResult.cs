using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Threading.Internal
{
    internal interface IInternalWaitableResult
    {
        /// <summary>
        /// This method is intent for internal use.
        /// </summary>   
        IWorkItemResult GetWorkItemResult();
    }

}
