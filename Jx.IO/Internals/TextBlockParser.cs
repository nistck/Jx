using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.IO
{
    internal class TextBlockParser
    {
        private string input;
        private int currentPosition;
        private string errorMessage;
        private int lineNumber;
        private TextBlock output; 

        private bool EndOfData()
        {
            return currentPosition >= input.Length;
        }

        private bool NextChar(out char ptr)
        {
            if (EndOfData())
            {
                ptr = '\0';
                return false;
            }
            ptr = input[currentPosition];
            currentPosition++;
            return true;
        }

        private void SetCurrentPosition(int pos)
        {
            currentPosition = pos;
        }

        private void RaiseError(string arg)
        {
            if (errorMessage == null)
                errorMessage = string.Format("{0} (line - {1})", arg, lineNumber);
        }

        private string NextToken(bool lineMode, out bool isString)
        {
            isString = false;
            StringBuilder stringBuilder = new StringBuilder();

            char c;
            while (NextChar(out c))
            {
                #region 解析: 注释
                if (c == '/')
                {   // 注释!
                    char c2;
                    if (!NextChar(out c2))
                    {
                        RaiseError("Unexpected end of file");
                        return "";
                    }
                    if (c2 == '/')
                    {   // 单行注释
                        while (NextChar(out c))
                        {
                            if (c == '\n')
                                goto Label_NL;
                        }

                        if (!EndOfData())
                        {
                            RaiseError("Unexpected end of file");
                            return "";
                        }
                        c = '\n';
                    }
                    else if (c2 == '*')
                    {   // 多行注释
                        char c3 = '\0';
                        while (NextChar(out c))
                        {
                            if (c == '\n')
                                lineNumber++;

                            if (c3 == '*' && c == '/')
                            {
                                c = ';';
                                goto Label_NL;
                            }
                            c3 = c;
                        }
                        if (!EndOfData())
                        {
                            RaiseError("Unexpected end of file");
                            return "";
                        }
                        c = ';';
                    }
                    else
                    {
                        SetCurrentPosition(currentPosition - 1);
                    }
                }
                #endregion

                Label_NL:
                if (c == '\n')
                    lineNumber++;
                
                if (c == '=' || c == '{' || c == '}')
                {
                    if (stringBuilder.Length != 0)
                    {
                        SetCurrentPosition(currentPosition - 1);
                        return stringBuilder.ToString().Trim();
                    }
                    return c.ToString();
                }
                else if ((!lineMode && (c <= ' ' || c == ';')) || (lineMode && (c == '\n' || c == '\r' || c == ';')))
                {
                    if (stringBuilder.Length != 0 || lineMode)
                    {
                        return stringBuilder.ToString().Trim();
                    }
                }
                #region 解析: 字符串
                else if (c == '"')
                {
                    if (stringBuilder.Length != 0)
                    {
                        SetCurrentPosition(currentPosition - 1);
                        return stringBuilder.ToString().Trim();
                    }
                    while (NextChar(out c))
                    {
                        if (c == '\n')
                            lineNumber++;
                        
                        if (c == '\\')
                        {
                            char c4;
                            if (!NextChar(out c4))
                            {
                                RaiseError("Unexpected end of file");
                                return "";
                            }
                            string text = "\\" + c4;
                            if (c4 == 'x')
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    if (!NextChar(out c4))
                                    {
                                        RaiseError("Unexpected end of file");
                                        return "";
                                    }
                                    text += c4;
                                }
                            }
                            TextBlockHelper._DecodeDelimiterFormatString(stringBuilder, text);
                        }
                        else
                        {
                            if (c == '"')
                            {
                                isString = true;
                                return stringBuilder.ToString();
                            }
                            stringBuilder.Append(c);
                        }
                    }
                    RaiseError("Unexpected end of file");
                    return "";
                }
                #endregion
                else if (stringBuilder.Length != 0 || (c != ' ' && c != '\t'))
                {
                    stringBuilder.Append(c);
                }
            }
            if (EndOfData())
            {
                return stringBuilder.ToString().Trim();
            }
            RaiseError("Unexpected end of file");
            return "";
        }

        private string NextToken(bool lineMode)
        {
            bool isString;
            return NextToken(lineMode, out isString);
        }

        private bool NextToken(TextBlock textBlock, bool flag)
        {
            while (true)
            {
                bool isString;
                string text = NextToken(false, out isString);
                if (text.Length == 0)
                    break;

                if (text == "}")
                    return true;
                
                string token = NextToken(false);
                if (token.Length == 0)
                {
                    RaiseError("Unexpected end of file");
                    return false;
                }

                if (token == "=")
                {   // ->  NAME = VALUE
                    string value = NextToken(true);
                    textBlock.SetAttribute(text, value);
                }
                else if (token == "{")
                {   // -> OBJECT_NAME {  }
                    TextBlock block = textBlock.AddChild(text);
                    if (!NextToken(block, false))
                    {
                        return false;
                    }
                }
                else
                {
                    string t = NextToken(false);
                    if (t.Length == 0)
                    {
                        RaiseError("Unexpected end of file");
                        return false;
                    }
                    if (!(t == "{"))
                    {
                        RaiseError("Invalid file format");
                        return false;
                    }

                    TextBlock tb = textBlock.AddChild(text, token);
                    if (!NextToken(tb, false))
                        return false;
                }
            }
            if (flag)
                return true;
            
            RaiseError("Unexpected end of file");
            return false; 
        }

        public static TextBlock Parse(string str, out string errorString)
        {
            if (str == null)
                throw new Exception("TextBlock: Parse: \"str\" is null.");

            TextBlockParser parser = new TextBlockParser();

            parser.input = str;
            parser.currentPosition = 0;
            parser.errorMessage = null;
            parser.lineNumber = 1;
            parser.output = new TextBlock();
            if (!parser.NextToken(parser.output, true))
            {
                errorString = parser.errorMessage;
                return null;
            }
            errorString = "";
            return parser.output;
        }
    }
}
