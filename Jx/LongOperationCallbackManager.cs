using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx
{
    public delegate void LongOperationNotifyHandler(string text, params object[] args);

    public static class LongOperationCallbackManager
    {
        public static event LongOperationNotifyHandler LongOperationNotify;

        public static void CallCallback(string text, params object[] args)
        {
            if (LongOperationNotify != null)
                LongOperationNotify(text, args);
        }
    }
}
