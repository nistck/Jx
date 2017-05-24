using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Jx.Drawing.Utilities
{
    /// <summary>
    /// Simple Clipboard mechanism.
    /// </summary> 
    public static class Clipboard<State>
        where State : ICloneable
    {
        #region Properties

        static ICloneable _clip = null;
        /// <summary>
        /// Gets or sets the clipboard state.
        /// </summary>
        static public State Clip
        {
            get { return (State)_clip.Clone(); }
            set { _clip = (State)value.Clone(); }
        }

        #endregion
    }
}
