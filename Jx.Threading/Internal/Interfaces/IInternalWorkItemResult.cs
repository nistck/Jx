using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Threading.Internal
{
    internal interface IInternalWorkItemResult
    {
        event WorkItemStateCallback OnWorkItemStarted;
        event WorkItemStateCallback OnWorkItemCompleted;
    }
}
