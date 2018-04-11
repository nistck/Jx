using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{

    public class BTResult
    {
        private BTResultCode code = BTResultCode.Running;

        private BTResult() { }

        public BTResultCode Code
        {
            get { return code; }
        }

        private WeakReference<object> data = null; 

        /// <summary>
        /// 相关数据
        /// </summary>
        public object Data
        {
            get {
                if (data == null)
                    return null;
                object o = null;
                if (data.TryGetTarget(out o))
                    return o;
                return null;
            }
            private set { 
                this.data = new WeakReference<object>(value);
            }
        }

        /// <summary>
        /// 是否有相关数据
        /// </summary>
        public bool HasData
        {
            get {
                if (this.data == null)
                    return false;

                object o = null;
                return this.data.TryGetTarget(out o);  
            }
        }

        public BTResult Create(object data) 
        {
            this.Data = data;
            return this; 
        }
        
        public override bool Equals(object obj)
        {
            BTResult r = obj as BTResult;
            if (r == null)
                return false;
            return Code == r.Code;
        }

        public override int GetHashCode()
        {
            return (int)Code;
        }

        public override string ToString()
        {
            string text = string.Format("{0}", Code);
            if (HasData)
                text = string.Format("{0}, 数据: {1}", text, Data);
            return text;
        }


        public static implicit operator BTResultCode(BTResult result)
        {
            return result == null ? BTResultCode.Running : result.code;
        }

        public static BTResult Success
        {
            get {
                BTResult r = new BTResult()
                {
                    code = BTResultCode.Success
                };
                return r;
            }
        }

        public static BTResult Failed
        {
            get {
                BTResult r = new BTResult()
                {
                    code = BTResultCode.Failed
                };
                return r;
            }
        }

        public static BTResult Running
        {
            get {
                BTResult r = new BTResult()
                {
                    code = BTResultCode.Running
                };
                return r;
            }
        }
    }

    /// <summary>
    /// 节点运行结果
    /// </summary>
	public enum BTResultCode
    {
        /// <summary>
        /// 成功
        /// </summary>
		Success = 1,
        /// <summary>
        /// 失败
        /// </summary>
		Failed = 2,
        /// <summary>
        /// 运行中
        /// </summary>
		Running = 3,
    }
}
