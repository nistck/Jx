using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Threading.Internal
{
    /// <summary>
    /// An internal delegate to call when the WorkItem starts or completes
    /// </summary>
    internal delegate void WorkItemStateCallback(WorkItem workItem);
}
