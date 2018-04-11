using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    [BTProperty("打印", BTConstants.GROUP_ACTION)]
    public class BTPrintAction : BTAction
    {
        private BTLogLevel level;
        private string message; 

        public BTLogLevel Level
        {
            get { return level; }
            set { this.level = value; }
        }

        public string Message
        {
            get { return message; }
            set { this.message = value; }
        }

        public BTPrintAction()
            : base()
        {
            this.Name = "Print"; 
        }

        protected override BTResult OnTick(BTContext context)
        {
            string text = Message ?? "";
            switch (level)
            {
                case BTLogLevel.Info:
                    BTDebug.Info(text);
                    break;
                case BTLogLevel.Warning:
                    BTDebug.Warning(text);
                    break;
                case BTLogLevel.Error:
                    BTDebug.Error(text);
                    break;
            }

            return BTResult.Success.Create(Message == null? "消息为空" : null);
        }

        public override string ToString()
        {
            string text = string.Format("级别: {0}, 内容: {1}", level, Message);
            return text;
        }
    }

    public enum BTLogLevel
    {
        Info, Warning, Error
    }
}
