using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    public class BTCompareData<T> : BTConditional
    {
        private string _dataName; 
        private T _rhs; 

        public BTCompareData(string readDataName, T rhs)
        {
            _dataName = readDataName;
            _rhs = rhs;
        }
 
        protected override bool Check(BTContext context)
        {
            if (context.Database == null)
                return false; 

            if (_rhs == null)
            {
                return context.Database.CheckDataNull(_dataName);
            }
            return context.Database.GetData<T>(_dataName).Equals(_rhs);
        }
    }
}
