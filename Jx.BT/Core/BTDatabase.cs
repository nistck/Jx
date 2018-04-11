using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    public class BTDatabase
    {
        private readonly Dictionary<string, WeakReference<object>> dataDic = new Dictionary<string, WeakReference<object>>(); 
 
        // Should use dataId as parameter to get data instead of this
        public T GetData<T>(string dataName)
        {
            if (dataName == null)
                return default(T); 

            if( !dataDic.ContainsKey(dataName) )
            {
                BTDebug.Error("BTDatabase: Data for " + dataName + " does not exist!");
                return default(T); 
            }

            WeakReference<object> wr = dataDic[dataName];
            object o = null;
            if (!wr.TryGetTarget(out o))
                return default(T);

            return (T)o; 
        } 
 
        public void SetData<T>(string dataName, T data)
        {
            if (dataName == null)
                return;

            BTDebug.Info("BTDatabase: {0} = {1}", dataName, data); 
            dataDic[dataName] = new WeakReference<object>(data); 
        }
 
        public bool CheckDataNull(string dataName)
        {
            if (dataName == null)
                return true;

            if (!dataDic.ContainsKey(dataName))
                return true; 

            WeakReference<object> wr = dataDic[dataName];
            object o = null;
            if (!wr.TryGetTarget(out o))
                return true;

            return o == null; 
        }
         
 
        public bool ContainsData(string dataName)
        {
            if (dataName == null)
                return false;

            if (!dataDic.ContainsKey(dataName))
                return false;

            WeakReference<object> wr = dataDic[dataName];
            object o = null;
            return wr.TryGetTarget(out o);
        }
    }
}
