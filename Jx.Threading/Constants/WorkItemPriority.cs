using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Threading
{

    /// <summary>
    /// Defines the availeable priorities of a work item.
    /// The higher the priority a work item has, the sooner
    /// it will be executed.
    /// </summary>
	public enum WorkItemPriority
    {
        Lowest,
        BelowNormal,
        Normal,
        AboveNormal,
        Highest,
    }
}
