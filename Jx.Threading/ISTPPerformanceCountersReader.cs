using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Threading
{
    public interface ISTPPerformanceCountersReader
    {
        long InUseThreads { get; }
        long ActiveThreads { get; }
        long WorkItemsQueued { get; }
        long WorkItemsProcessed { get; }
    }
}
