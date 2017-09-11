using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Runtime.Serialization;

namespace Jx
{
    public class JxException : Exception
    {
        //
        // 摘要:
        //     初始化 System.Exception 类的新实例。
        public JxException()
            : base()
        {

        }

        //
        // 摘要:
        //     使用指定的错误信息初始化 System.Exception 类的新实例。
        //
        // 参数:
        //   message:
        //     描述错误的消息。
        public JxException(string message)
            : base(message)
        {

        }
        //
        // 摘要:
        //     使用指定错误消息和对作为此异常原因的内部异常的引用来初始化 System.Exception 类的新实例。
        //
        // 参数:
        //   message:
        //     解释异常原因的错误信息。
        //
        //   innerException:
        //     导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。
        public JxException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        //
        // 摘要:
        //     用序列化数据初始化 System.Exception 类的新实例。
        //
        // 参数:
        //   info:
        //     System.Runtime.Serialization.SerializationInfo，它存有有关所引发的异常的序列化对象数据。
        //
        //   context:
        //     System.Runtime.Serialization.StreamingContext，它包含有关源或目标的上下文信息。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     info 参数为 null。
        //
        //   T:System.Runtime.Serialization.SerializationException:
        //     类名为 null 或 System.Exception.HResult 为零 (0)。
        [SecuritySafeCritical]
        protected JxException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
