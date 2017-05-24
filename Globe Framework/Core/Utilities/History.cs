using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Globe.Core.Utilities
{
    /// <summary>
    /// Simple Undo-Redo mechanism.
    /// </summary> 
    public static class History<State>
        where State : ICloneable
    {
        static int _cursor = 0;
        static Collection<State> _history = new Collection<State>();

        #region Properties

        static int _buffer = 50;
        /// <summary>
        /// Number of states to memorize.
        /// </summary>
        public static int Buffer
        {
            get { return _buffer; }

            set
            {
                _buffer = value;
                if (_buffer <= 10)
                    _buffer = 10;
            }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Controls if the cursor is at the buffer start.
        /// </summary>
        /// <returns></returns>
        public static bool IsAtStart()
        {
            if (_cursor == 0 || _history.Count == 0)
                return true;

            return false;
        }

        /// <summary>
        /// Controls if the cursor is at the buffer end.
        /// </summary>
        /// <returns></returns>
        public static bool IsAtEnd()
        {
            if (_cursor == _history.Count - 1 || _history.Count == 0)
                return true;

            return false;
        }

        /// <summary>
        /// Deletes history states.
        /// </summary>
        public static void Delete()
        {
            _history.Clear();
            _cursor = 0;
        }

        /// <summary>
        /// Memorizes a state.
        /// </summary>
        /// <param name="state">State to memorize.</param>
        public static void Memorize(State state)
        {
            _history.Add((State)state.Clone());
            _cursor = _history.Count - 1;
        }

        /// <summary>
        /// Undo mechanism.
        /// </summary>
        /// <returns></returns>
        public static State Undo()
        {
            if (_cursor > 0)
                _cursor--;

            return (State)_history[_cursor].Clone();
        }

        /// <summary>
        /// Redo memchanism.
        /// </summary>
        /// <returns></returns>
        public static State Redo()
        {
            if (_cursor < _history.Count - 1)
                _cursor++;

            return (State)_history[_cursor].Clone();
        }

        #endregion
    }
}
