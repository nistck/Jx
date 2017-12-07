using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using System.Collections.ObjectModel; 

namespace Jx.FileSystem
{
    public class TextBlock
    {
        public sealed class Attribute
        {
            internal string name;
            internal string value;
            public string Name
            { 
                get
                {
                    return this.name;
                }
            }
            public string Value
            {
                get
                {
                    return this.value;
                }
            }
            internal Attribute()
            {
            }
            public override string ToString()
            {
                return string.Format("Name: \"{0}\", Value \"{1}\"", this.name, this.value);
            }
        }
        private TextBlock parent;
        private string name;
        private string data;
        private List<TextBlock> children = new List<TextBlock>();
        private ReadOnlyCollection<TextBlock> readOnlyChildren;

        private List<Attribute> attributes = new List<Attribute>();
        private ReadOnlyCollection<Attribute> readOnlyAttributes;

        public TextBlock Parent
        {
            get
            {
                return this.parent;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name == value)
                    return;
                this.name = value;
                if (string.IsNullOrEmpty(this.name))
                    throw new Exception("TextBlock: set Name: \"name\" is null or empty.");
            }
        }

        public string Data
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
            }
        }

        public IList<TextBlock> Children
        {
            get
            {
                return this.readOnlyChildren;
            }
        }

        public IList<Attribute> Attributes
        {
            get
            {
                return this.readOnlyAttributes;
            }
        }

        public TextBlock()
        {
            this.readOnlyChildren = new ReadOnlyCollection<TextBlock>(this.children);
            this.readOnlyAttributes = new ReadOnlyCollection<TextBlock.Attribute>(this.attributes);
        }

        public TextBlock FindChild(string name)
        {
            for (int i = 0; i < this.children.Count; i++)
            {
                TextBlock textBlock = this.children[i];
                if (textBlock.Name == name)
                {
                    return textBlock;
                }
            }
            return null;
        }
        public TextBlock AddChild(string name, string data)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("TextBlock: AddChild: \"name\" is null or empty.");

            TextBlock textBlock = new TextBlock();
            textBlock.parent = this;
            textBlock.name = name;
            textBlock.data = data;
            this.children.Add(textBlock);
            return textBlock;
        }

        public TextBlock AddChild(string name)
        {
            return this.AddChild(name, "");
        }

        public void DeleteChild(TextBlock child)
        {
            this.children.Remove(child);
            child.parent = null;
            child.name = "";
            child.data = "";
            child.children = null;
            child.attributes = null;
        }

        public void AttachChild(TextBlock child)
        {
            if (child.parent != null)
            {
                throw new Exception("TextBlock: AddChild: Unable to attach. Block is already attached to another block. child.Parent != null.");
            }
            child.parent = this;
            this.children.Add(child);
        }

        public void DetachChild(TextBlock child)
        {
            this.children.Remove(child);
            child.parent = null;
        }

        public string GetAttribute(string name, string defaultValue)
        {
            for (int i = 0; i < this.attributes.Count; i++)
            {
                TextBlock.Attribute attribute = this.attributes[i];
                if (attribute.Name == name)
                {
                    return attribute.Value;
                }
            }
            return defaultValue;
        }

        public string GetAttribute(string name)
        {
            return this.GetAttribute(name, "");
        }

        public bool IsAttributeExist(string name)
        {
            for (int i = 0; i < this.attributes.Count; i++)
            {
                TextBlock.Attribute attribute = this.attributes[i];
                if (attribute.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetAttribute(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("TextBlock: AddChild: \"name\" is null or empty.");

            if (value == null)
                throw new Exception("TextBlock: AddChild: \"value\" is null.");            

            for (int i = 0; i < this.attributes.Count; i++)
            {
                TextBlock.Attribute attribute = this.attributes[i];
                if (attribute.Name == name)
                {
                    attribute.value = value;
                    return;
                }
            }
            TextBlock.Attribute attr = new TextBlock.Attribute();
            attr.name = name;
            attr.value = value;
            this.attributes.Add(attr);
        }

        public void DeleteAttribute(string name)
        {
            for (int i = 0; i < this.attributes.Count; i++)
            {
                if (name == this.attributes[i].name)
                {
                    TextBlock.Attribute attribute = this.attributes[i];
                    attribute.name = "";
                    attribute.value = "";
                    this.attributes.RemoveAt(i);
                    return;
                }
            }
        }

        public void DeleteAllAttributes()
        {
            this.attributes.Clear();
        }

        private static string Tabs(int n)
        {
            string text = "";
            for (int i = 0; i < n; i++)
            {
                text += "\t";
            }
            return text;
        }

        private static bool ShouldEncode(string text, bool flag)
        {
            if (!flag)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];
                    if ((c < 'A' || c > 'Z') && (c < 'a' || c > 'z') && (c < '0' || c > '9') && c != '_' && c != '#' && c != '$')
                    {
                        bool result = true;
                        return result;
                    }
                }
                return false;
            }

            if (text.Length > 0 && (text[0] == ' ' || text[text.Length - 1] == ' '))
            {
                return true;
            }
            for (int j = 0; j < text.Length; j++)
            {
                char c2 = text[j];
                if ((c2 < 'A' || c2 > 'Z') && (c2 < 'a' || c2 > 'z') && (c2 < '0' || c2 > '9') && c2 != '_' && c2 != '#' && c2 != '$' && c2 != '.' && c2 != ',' && c2 != '-' && c2 != '!' && c2 != '%' && c2 != '&' && c2 != '(' && c2 != ')' && c2 != '*' && c2 != '+' && c2 != '?' && c2 != '[' && c2 != ']' && c2 != '^' && c2 != '|' && c2 != '~' && c2 != ' ')
                {
                    bool result = true;
                    return result;
                }
            }
            return false;
        }

        private void DumpToString(StringBuilder stringBuilder, int numTabs)
        {
            string tabsPrefix = Tabs(numTabs);

            if (!string.IsNullOrEmpty(this.Name))
            {
                stringBuilder.Append(tabsPrefix);
                string nameEncoded;
                if (ShouldEncode(this.Name, false))
                {
                    nameEncoded = string.Format("\"{0}\"", EncodeDelimiterFormatString(this.Name));
                }
                else
                {
                    nameEncoded = this.Name;
                }
                stringBuilder.Append(nameEncoded);

                if (!string.IsNullOrEmpty(this.Data))
                {
                    stringBuilder.Append(" ");
                    string dataEncoded;
                    if (ShouldEncode(this.Data, false))
                    {
                        dataEncoded = string.Format("\"{0}\"", EncodeDelimiterFormatString(this.Data));
                    }
                    else
                    {
                        dataEncoded = this.Data;
                    }
                    stringBuilder.Append(dataEncoded);
                }
                stringBuilder.Append("\r\n");
                stringBuilder.Append(tabsPrefix);
                stringBuilder.Append("{\r\n");
            }

            foreach (Attribute attr in this.attributes)
            {
                string attrNameEncoded;
                if (ShouldEncode(attr.Name, false))
                {
                    attrNameEncoded = string.Format("\"{0}\"", EncodeDelimiterFormatString(attr.Name));
                }
                else
                {
                    attrNameEncoded = attr.Name;
                }

                string attrValueEncoded;
                if (ShouldEncode(attr.Value, true))
                {
                    attrValueEncoded = string.Format("\"{0}\"", EncodeDelimiterFormatString(attr.Value));
                }
                else
                {
                    attrValueEncoded = attr.Value;
                }

                stringBuilder.Append(tabsPrefix);
                stringBuilder.Append((numTabs != -1) ? "\t" : "");
                stringBuilder.AppendFormat("{0} = {1}\r\n", attrNameEncoded, attrValueEncoded);
            }

            foreach (TextBlock child in this.children)
            {
                child.DumpToString(stringBuilder, numTabs + 1);
            }

            if (!string.IsNullOrEmpty(this.Name))
            {
                stringBuilder.Append(tabsPrefix);
                stringBuilder.Append("}\r\n");
            }
        }

        public string DumpToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            this.DumpToString(stringBuilder, -1);
            return stringBuilder.ToString();
        }

        public override string ToString()
        {
            string text = string.Format("Name: \"{0}\"", this.name);
            if (!string.IsNullOrEmpty(this.data))
            {
                text += string.Format(", Data: \"{0}\"", this.data);
            }
            return text;
        }

        #region Encode & Decode

        public static string EncodeDelimiterFormatString(string text)
        {
            StringBuilder stringBuilder = new StringBuilder("", text.Length + 2);
            int i = 0;
            while (i < text.Length)
            {
                char c = text[i];
                if (c <= '"')
                {
                    switch (c)
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
                            if (c != '"')
                            {
                                goto IL_B5;
                            }
                            stringBuilder.Append("\\\"");
                            break;
                    }
                }
                else if (c != '\'')
                {
                    if (c != '\\')
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
                    string arg_D4_0 = "\\x";
                    int num = (int)c;
                    stringBuilder.Append(arg_D4_0 + num.ToString("x04"));
                    goto IL_E9;
                }
                stringBuilder.Append(c);
                goto IL_E9;
            }
            return stringBuilder.ToString();
        }

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
            _DecodeDelimiterFormatString(stringBuilder, text);
            return stringBuilder.ToString();
        }
        #endregion

        public static TextBlock Parse(string str, out string errorString)
        {
            return TextBlockParser.Parse(str, out errorString);
        }
    }

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
                            TextBlock._DecodeDelimiterFormatString(stringBuilder, text);
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
