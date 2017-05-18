using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace Jx.Editors
{
    public class CursorKeeper : IDisposable
    {
        private Cursor originalCursor;
        private bool isDisposed;
        public CursorKeeper(Cursor newCursor)
        {
            this.originalCursor = Cursor.Current;
            Cursor.Current = newCursor;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed && disposing)
            {
                Cursor.Current = this.originalCursor;
            }
            this.isDisposed = true;
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
