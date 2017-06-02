using System;
using System.Runtime.Serialization;

namespace Jx.Threading
{
    /// <summary>
    /// Represents an exception in case IWorkItemResult.GetResult has been timed out
    /// </summary>
    public sealed partial class WorkItemTimeoutException : Exception
    {
        public WorkItemTimeoutException()
        {
        }

        public WorkItemTimeoutException(string message)
            : base(message)
        {
        }

        public WorkItemTimeoutException(string message, Exception e)
            : base(message, e)
        {
        }

        public WorkItemTimeoutException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
