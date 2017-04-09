using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using System.Collections.ObjectModel; 

namespace Jx.IO
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
                    nameEncoded = string.Format("\"{0}\"", d.EncodeDelimiterFormatString(this.Name));
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
                        dataEncoded = string.Format("\"{0}\"", d.EncodeDelimiterFormatString(this.Data));
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
                    attrNameEncoded = string.Format("\"{0}\"", d.EncodeDelimiterFormatString(attr.Name));
                }
                else
                {
                    attrNameEncoded = attr.Name;
                }

                string attrValueEncoded;
                if (ShouldEncode(attr.Value, true))
                {
                    attrValueEncoded = string.Format("\"{0}\"", d.EncodeDelimiterFormatString(attr.Value));
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

        public static TextBlock Parse(string str, out string errorString)
        {
            return e.Parse(str, out errorString);
        }
    }
}
