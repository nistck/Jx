using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Jx.UI.Model
{
    public interface IViewModel
    {
        Image Icon { get; }
        Size IconSize { get; }

        Color TextColor { get; }
    }
}
