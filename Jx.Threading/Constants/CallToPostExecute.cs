using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Threading
{
    [Flags]
    public enum CallToPostExecute
    {
        /// <summary>
        /// Never call to the PostExecute call back
        /// </summary>
		Never = 0x00,

        /// <summary>
        /// Call to the PostExecute only when the work item is cancelled
        /// </summary>
		WhenWorkItemCanceled = 0x01,

        /// <summary>
        /// Call to the PostExecute only when the work item is not cancelled
        /// </summary>
		WhenWorkItemNotCanceled = 0x02,

        /// <summary>
        /// Always call to the PostExecute
        /// </summary>
		Always = WhenWorkItemCanceled | WhenWorkItemNotCanceled,
    }
}
