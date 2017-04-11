using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Jx.Ext
{
    public static class StringUtils
    {
        public static bool IsCorrectIdentifierName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            char c = name[0];
            if ((c < 'A' || c > 'Z') && (c < 'a' || c > 'z') && c != '_' && c != '$' && c != '#')
            {
                return false;
            }
            for (int i = 1; i < name.Length; i++)
            {
                char c2 = name[i];
                if ((c2 < 'A' || c2 > 'Z') && (c2 < 'a' || c2 > 'z') && (c2 < '0' || c2 > '9') && c2 != '_' && c2 != '.' && c2 != '-')
                {
                    return false;
                }
            }
            return true;
        }
        public static string EncodeDelimiterFormatString(string text)
        {
            StringBuilder stringBuilder = new StringBuilder("", text.Length + 2);
            int i = 0;
            while (i < text.Length)
            {
                char c = text[i];
                char c2 = c;
                if (c2 <= '"')
                {
                    switch (c2)
                    {
                        case '\t':
                            stringBuilder.Append("\\t");
                            break;
                        case '\n':
                            stringBuilder.Append("\\n");
                            break;
                        case '\v':
                        case '\f':
                            goto IL_B5;
                        case '\r':
                            stringBuilder.Append("\\r");
                            break;
                        default:
                            if (c2 != '"')
                            {
                                goto IL_B5;
                            }
                            stringBuilder.Append("\\\"");
                            break;
                    }
                }
                else if (c2 != '\'')
                {
                    if (c2 != '\\')
                    {
                        goto IL_B5;
                    }
                    stringBuilder.Append("\\\\");
                }
                else
                {
                    stringBuilder.Append("\\'");
                }
                IL_E9:
                i++;
                continue;
                IL_B5:
                if (c < ' ' || c >= '\u007f')
                {
                    StringBuilder arg_D9_0 = stringBuilder;
                    string arg_D4_0 = "\\x";
                    int num = (int)c;
                    arg_D9_0.Append(arg_D4_0 + num.ToString("x04"));
                    goto IL_E9;
                }
                stringBuilder.Append(c);
                goto IL_E9;
            }
            return stringBuilder.ToString();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void _DecodeDelimiterFormatString(StringBuilder outBuilder, string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\\')
                {
                    i++;
                    char c2 = text[i];
                    char c3 = c2;
                    if (c3 <= '\\')
                    {
                        if (c3 == '"')
                        {
                            outBuilder.Append('"');
                            goto IL_18C;
                        }
                        if (c3 == '\'')
                        {
                            outBuilder.Append('\'');
                            goto IL_18C;
                        }
                        if (c3 == '\\')
                        {
                            outBuilder.Append('\\');
                            goto IL_18C;
                        }
                    }
                    else
                    {
                        if (c3 == 'n')
                        {
                            outBuilder.Append('\n');
                            goto IL_18C;
                        }
                        switch (c3)
                        {
                            case 'r':
                                outBuilder.Append('\r');
                                goto IL_18C;
                            case 's':
                                break;
                            case 't':
                                outBuilder.Append('\t');
                                goto IL_18C;
                            default:
                                if (c3 == 'x')
                                {
                                    if (i + 4 >= text.Length)
                                    {
                                        throw new Exception("Invalid string format");
                                    }
                                    int[] array = new int[4];
                                    for (int j = 0; j < 4; j++)
                                    {
                                        char c4 = text[i + 1 + j];
                                        if (c4 >= '0' && c4 <= '9')
                                        {
                                            array[j] = (int)(c4 - '0');
                                        }
                                        else if (c4 >= 'a' && c4 <= 'f')
                                        {
                                            array[j] = (int)('\n' + c4 - 'a');
                                        }
                                        else
                                        {
                                            if (c4 < 'A' || c4 > 'F')
                                            {
                                                throw new Exception("Invalid string format");
                                            }
                                            array[j] = (int)('\n' + c4 - 'A');
                                        }
                                    }
                                    int num = ((array[0] * 16 + array[1]) * 16 + array[2]) * 16 + array[3];
                                    outBuilder.Append((char)num);
                                    i += 4;
                                    goto IL_18C;
                                }
                                break;
                        }
                    }
                    throw new Exception("Invalid string format");
                }
                outBuilder.Append(c);
                IL_18C:;
            }
        }
        public static string DecodeDelimiterFormatString(string text)
        {
            StringBuilder stringBuilder = new StringBuilder("", text.Length + 2);
            StringUtils._DecodeDelimiterFormatString(stringBuilder, text);
            return stringBuilder.ToString();
        }
        public static string[] TextWordWrap(string text, int charactersPerLine)
        {
            List<string> list = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();
            string[] array = text.Split(new char[]
            {
                ' '
            });
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string text2 = array2[i];
                if (stringBuilder.Length + 1 + text2.Length > charactersPerLine)
                {
                    if (stringBuilder.Length != 0)
                    {
                        list.Add(stringBuilder.ToString());
                    }
                    stringBuilder.Length = 0;
                }
                if (stringBuilder.Length != 0)
                {
                    stringBuilder.Append(" ");
                }
                stringBuilder.Append(text2);
            }
            if (stringBuilder.Length != 0)
            {
                list.Add(stringBuilder.ToString());
            }
            return list.ToArray();
        }
    }
}
