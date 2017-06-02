using System;
using System.Runtime.Serialization;

namespace Jx.Threading
{
    /// <summary>
    /// Represents an exception in case IWorkItemResult.GetResult has been canceled
    /// </summary>
    public sealed partial class WorkItemCancelException : Exception
    {
        public WorkItemCancelException()
        {
        }

        public WorkItemCancelException(string message)
            : base(message)
        {
        }

        public WorkItemCancelException(string message, Exception e)
            : base(message, e)
        {
        }
        public WorkItemCancelException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
