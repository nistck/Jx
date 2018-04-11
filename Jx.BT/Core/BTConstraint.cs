using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    /// <summary>
    /// 前置约束
    /// </summary>
    public abstract class BTConstraint
    {
        private string id; 

        public BTConstraint()
        {
            this.id = Guid.NewGuid().ToString().Replace("-", ""); 
        }

        public string Id
        {
            get { return id; }
        }

        public bool Execute(BTContext context)
        {
            bool result = Evaluate(context);
            return result; 
        }

        protected virtual bool Evaluate(BTContext context)
        {
            return false; 
        }
    }
}
