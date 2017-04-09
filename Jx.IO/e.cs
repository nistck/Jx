using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.IO
{
    internal static class e
    {
        private static string Y;
        private static int y;
        private static string Z;
        private static int z;
        private static TextBlock aA;
        private static StringBuilder aa = new StringBuilder();
        private static bool A()
        {
            return e.y >= e.Y.Length;
        }
        private static bool A(out char ptr)
        {
            if (e.A())
            {
                ptr = '\0';
                return false;
            }
            ptr = e.Y[e.y];
            e.y++;
            return true;
        }
        private static void A(int num)
        {
            e.y = num;
        }
        private static void A(string arg)
        {
            if (e.Z == null)
            {
                e.Z = string.Format("{0} (line - {1})", arg, e.z);
            }
        }
        private static string A(bool flag, out bool ptr)
        {
            ptr = false;
            StringBuilder stringBuilder = e.aa;
            stringBuilder.Length = 0;
            char c;
            while (e.A(out c))
            {
                if (c == '/')
                {
                    char c2;
                    if (!e.A(out c2))
                    {
                        e.A("Unexpected end of file");
                        return "";
                    }
                    if (c2 == '/')
                    {
                        while (e.A(out c))
                        {
                            if (c == '\n')
                            {
                                goto IL_EA;
                            }
                        }
                        if (!e.A())
                        {
                            e.A("Unexpected end of file");
                            return "";
                        }
                        c = '\n';
                    }
                    else if (c2 == '*')
                    {
                        char c3 = '\0';
                        while (e.A(out c))
                        {
                            if (c == '\n')
                            {
                                e.z++;
                            }
                            if (c3 == '*' && c == '/')
                            {
                                c = ';';
                                goto IL_EA;
                            }
                            c3 = c;
                        }
                        if (!e.A())
                        {
                            e.A("Unexpected end of file");
                            return "";
                        }
                        c = ';';
                    }
                    else
                    {
                        e.A(e.y - 1);
                    }
                }
                IL_EA:
                if (c == '\n')
                {
                    e.z++;
                }
                if (c == '=' || c == '{' || c == '}')
                {
                    if (stringBuilder.Length != 0)
                    {
                        e.A(e.y - 1);
                        return stringBuilder.ToString().Trim();
                    }
                    return c.ToString();
                }
                else if ((!flag && (c <= ' ' || c == ';')) || (flag && (c == '\n' || c == '\r' || c == ';')))
                {
                    if (stringBuilder.Length != 0 || flag)
                    {
                        return stringBuilder.ToString().Trim();
                    }
                }
                else if (c == '"')
                {
                    if (stringBuilder.Length != 0)
                    {
                        e.A(e.y - 1);
                        return stringBuilder.ToString().Trim();
                    }
                    while (e.A(out c))
                    {
                        if (c == '\n')
                        {
                            e.z++;
                        }
                        if (c == '\\')
                        {
                            char c4;
                            if (!e.A(out c4))
                            {
                                e.A("Unexpected end of file");
                                return "";
                            }
                            string text = "\\" + c4;
                            if (c4 == 'x')
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    if (!e.A(out c4))
                                    {
                                        e.A("Unexpected end of file");
                                        return "";
                                    }
                                    text += c4;
                                }
                            }
                            d._DecodeDelimiterFormatString(stringBuilder, text);
                        }
                        else
                        {
                            if (c == '"')
                            {
                                ptr = true;
                                return stringBuilder.ToString();
                            }
                            stringBuilder.Append(c);
                        }
                    }
                    e.A("Unexpected end of file");
                    return "";
                }
                else if (stringBuilder.Length != 0 || (c != ' ' && c != '\t'))
                {
                    stringBuilder.Append(c);
                }
            }
            if (e.A())
            {
                return stringBuilder.ToString().Trim();
            }
            e.A("Unexpected end of file");
            return "";
        }
        private static string A(bool flag)
        {
            bool flag2;
            return e.A(flag, out flag2);
        }
        private static bool A(TextBlock textBlock, bool flag)
        {
            while (true)
            {
                bool flag2;
                string text = e.A(false, out flag2);
                if (text.Length == 0)
                {
                    break;
                }
                if (text == "}")
                {
                    return true;
                }
                string text2 = e.A(false);
                if (text2.Length == 0)
                {
                    goto Block_3;
                }
                if (text2 == "=")
                {
                    string value = e.A(true);
                    textBlock.SetAttribute(text, value);
                }
                else if (text2 == "{")
                {
                    TextBlock textBlock2 = textBlock.AddChild(text);
                    if (!e.A(textBlock2, false))
                    {
                        return false;
                    }
                }
                else
                {
                    string text3 = e.A(false);
                    if (text3.Length == 0)
                    {
                        goto Block_7;
                    }
                    if (!(text3 == "{"))
                    {
                        goto IL_D3;
                    }
                    TextBlock textBlock3 = textBlock.AddChild(text, text2);
                    if (!e.A(textBlock3, false))
                    {
                        return false;
                    }
                }
            }
            if (flag)
            {
                return true;
            }
            e.A("Unexpected end of file");
            return false;
            Block_3:
            e.A("Unexpected end of file");
            return false;
            Block_7:
            e.A("Unexpected end of file");
            return false;
            IL_D3:
            e.A("Invalid file format");
            return false;
        }
        public static TextBlock Parse(string str, out string errorString)
        {
            if (str == null)
                throw new Exception("TextBlock: Parse: \"str\" is null.");
            
            e.Y = str;
            e.y = 0;
            e.Z = null;
            e.z = 1;
            e.aA = new TextBlock();
            if (!e.A(e.aA, true))
            {
                errorString = e.Z;
                return null;
            }
            errorString = "";
            return e.aA;
        }
    }
}
