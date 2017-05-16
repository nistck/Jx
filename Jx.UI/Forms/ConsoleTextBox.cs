using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Jx.UI.Controls.FCTB;

namespace Jx.UI.Forms
{

    /// <summary>
    /// Console emulator.
    /// </summary>
    public class ConsoleTextBox : FastColoredTextBox
    {
        private volatile bool isReadLineMode;
        private volatile bool isUpdating;

        private uint timeout = 0;
        private long timeStart = 0;

        public Place StartReadPlace { get; private set; }
        public Place EndReadPlace { get; private set; }

        /// <summary>
        /// Control is waiting for line entering. 
        /// </summary>
        public bool IsReadLineMode
        {
            get { return isReadLineMode; }
            set { isReadLineMode = value; }
        }

        /// <summary>
        /// Seconds
        /// </summary>
        public uint Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        /// <summary>
        /// Append line to end of text.
        /// </summary>
        /// <param name="text"></param>
        public void WriteLine(string text, params object[] args)
        {
            string _text = string.Format(text, args);
            IsReadLineMode = false;
            isUpdating = true;
            try
            {
                AppendText(_text);
                GoEnd();
            }
            finally
            {
                isUpdating = false;
                ClearUndo();
            }
        }

        /// <summary>
        /// Append line to end of text.
        /// </summary>
        /// <param name="text"></param>
        public void WriteLine(string text, Style style, params object[] args)
        {
            string _text = string.Format(text, args);
            IsReadLineMode = false;
            isUpdating = true;
            try
            {
                AppendText(_text, style);
                GoEnd();
            }
            finally
            {
                isUpdating = false;
                ClearUndo();
            }
        }

        /// <summary>
        /// Wait for line entering.
        /// Set IsReadLineMode to false for break of waiting.
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
        {
            Range range = ReadLineRange();
            return range.Text.TrimEnd('\r', '\n');
        }

        public Range ReadLineRange()
        {
            GoEnd();
            StartReadPlace = Range.End;
            IsReadLineMode = true;
            try
            {
                timeStart = DateTime.Now.Ticks;
                while (IsReadLineMode)
                {
                    Application.DoEvents();
                    Thread.Sleep(5);

                    if (Timeout > 0)
                    {
                        long ts1 = (DateTime.Now.Ticks - timeStart) / 10000 / 1000;
                        if (ts1 > Timeout)
                            break;
                    }
                }
            }
            finally
            {
                IsReadLineMode = false;
                timeStart = 0;
                EndReadPlace = Range.End;
                ClearUndo();
            }

            return new Range(this, StartReadPlace, Range.End);
        }

        public override void OnTextChanging(ref string text)
        {
            if (!IsReadLineMode && !isUpdating)
            {
                text = ""; //cancel changing
                return;
            }

            if (IsReadLineMode)
            {
                timeStart = DateTime.Now.Ticks;
                if (Selection.Start < StartReadPlace || Selection.End < StartReadPlace)
                    GoEnd();//move caret to entering position

                if (Selection.Start == StartReadPlace || Selection.End == StartReadPlace)
                    if (text == "\b") //backspace
                    {
                        text = ""; //cancel deleting of last char of readonly text
                        return;
                    }

                if (text != null && text.Contains('\n'))
                {
                    text = text.Substring(0, text.IndexOf('\n') + 1);
                    IsReadLineMode = false;
                }
            }

            base.OnTextChanging(ref text);
        }

        public override void Clear()
        {
            var oldIsReadMode = isReadLineMode;

            isReadLineMode = false;
            isUpdating = true;

            base.Clear();

            isUpdating = false;
            isReadLineMode = oldIsReadMode;

            StartReadPlace = Place.Empty;
            EndReadPlace = Place.Empty;
        }
    }

    /// <summary>
    /// This style will drawing ellipse around of the word
    /// </summary>
    internal class EllipseStyle : Style
    {
        public override void Draw(Graphics gr, Point position, Range range)
        {
            //get size of rectangle
            Size size = GetSizeOfRange(range);
            //create rectangle
            Rectangle rect = new Rectangle(position, size);
            //inflate it
            rect.Inflate(2, 2);
            //get rounded rectangle
            var path = GetRoundedRectangle(rect, 7);
            //draw rounded rectangle
            gr.DrawPath(Pens.Red, path);
        }
    }
}
