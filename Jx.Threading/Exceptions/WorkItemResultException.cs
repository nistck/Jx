using System;
using System.Runtime.Serialization;

namespace Jx.Threading
{

    /// <summary>
    /// Represents an exception in case IWorkItemResult.GetResult has been timed out
    /// </summary>
    public sealed partial class WorkItemResultException : Exception
    {
        public WorkItemResultException()
        {
        }

        public WorkItemResultException(string message)
            : base(message)
        {
        }

        public WorkItemResultException(string message, Exception e)
            : base(message, e)
        {
        }

        public WorkItemResultException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
