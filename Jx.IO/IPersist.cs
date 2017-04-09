using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.IO
{
    public interface IPersist
    {
        /// <summary>
        /// 从textBlock中读取
        /// </summary>
        /// <param name="textBlock"></param>
        /// <returns></returns>
        bool OnLoad(TextBlock textBlock);
        /// <summary>
        /// 保存到textBlock中
        /// </summary>
        /// <param name="textBlock"></param>
        /// <returns></returns>
        bool OnSave(TextBlock textBlock);
    }
}
