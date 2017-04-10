using A;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
namespace Jx.FileSystem
{
	public class TextBlock
	{
		public sealed class Attribute
		{
			internal string aV;
			internal string av;
			public string Name
			{
				get
				{
					return this.aV;
				}
			}
			public string Value
			{
				get
				{
					return this.av;
				}
			}
			internal Attribute()
			{
			}
			public override string ToString()
			{
				return string.Format("Name: \"{0}\", Value \"{1}\"", this.aV, this.av);
			}
		}
		private TextBlock u;
		private string V;
		private string v;
		private List<TextBlock> W = new List<TextBlock>();
		private ReadOnlyCollection<TextBlock> w;
		private List<TextBlock.Attribute> X = new List<TextBlock.Attribute>();
		private ReadOnlyCollection<TextBlock.Attribute> x;
		public TextBlock Parent
		{
			get
			{
				return this.u;
			}
		}
		public string Name
		{
			get
			{
				return this.V;
			}
			set
			{
				if (this.V == value)
				{
					return;
				}
				this.V = value;
				if (string.IsNullOrEmpty(this.V))
				{
					Log.Fatal("TextBlock: set Name: \"name\" is null or empty.");
				}
			}
		}
		public string Data
		{
			get
			{
				return this.v;
			}
			set
			{
				this.v = value;
			}
		}
		public IList<TextBlock> Children
		{
			get
			{
				return this.w;
			}
		}
		public IList<TextBlock.Attribute> Attributes
		{
			get
			{
				return this.x;
			}
		}
		public TextBlock()
		{
			this.w = new ReadOnlyCollection<TextBlock>(this.W);
			this.x = new ReadOnlyCollection<TextBlock.Attribute>(this.X);
		}
		public TextBlock FindChild(string name)
		{
			for (int i = 0; i < this.W.Count; i++)
			{
				TextBlock textBlock = this.W[i];
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
			{
				Log.Fatal("TextBlock: AddChild: \"name\" is null or empty.");
			}
			TextBlock textBlock = new TextBlock();
			textBlock.u = this;
			textBlock.V = name;
			textBlock.v = data;
			this.W.Add(textBlock);
			return textBlock;
		}
		public TextBlock AddChild(string name)
		{
			return this.AddChild(name, "");
		}
		public void DeleteChild(TextBlock child)
		{
			this.W.Remove(child);
			child.u = null;
			child.V = "";
			child.v = "";
			child.W = null;
			child.X = null;
		}
		public void AttachChild(TextBlock child)
		{
			if (child.u != null)
			{
				Log.Fatal("TextBlock: AddChild: Unable to attach. Block is already attached to another block. child.Parent != null.");
			}
			child.u = this;
			this.W.Add(child);
		}
		public void DetachChild(TextBlock child)
		{
			this.W.Remove(child);
			child.u = null;
		}
		public string GetAttribute(string name, string defaultValue)
		{
			for (int i = 0; i < this.X.Count; i++)
			{
				TextBlock.Attribute attribute = this.X[i];
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
			for (int i = 0; i < this.X.Count; i++)
			{
				TextBlock.Attribute attribute = this.X[i];
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
			{
				Log.Fatal("TextBlock: AddChild: \"name\" is null or empty.");
			}
			if (value == null)
			{
				Log.Fatal("TextBlock: AddChild: \"value\" is null.");
			}
			for (int i = 0; i < this.X.Count; i++)
			{
				TextBlock.Attribute attribute = this.X[i];
				if (attribute.Name == name)
				{
					attribute.av = value;
					return;
				}
			}
			TextBlock.Attribute attribute2 = new TextBlock.Attribute();
			attribute2.aV = name;
			attribute2.av = value;
			this.X.Add(attribute2);
		}
		public void DeleteAttribute(string name)
		{
			for (int i = 0; i < this.X.Count; i++)
			{
				if (name == this.X[i].aV)
				{
					TextBlock.Attribute attribute = this.X[i];
					attribute.aV = "";
					attribute.av = "";
					this.X.RemoveAt(i);
					return;
				}
			}
		}
		public void DeleteAllAttributes()
		{
			this.X.Clear();
		}
		private static string A(int num)
		{
			string text = "";
			for (int i = 0; i < num; i++)
			{
				text += "\t";
			}
			return text;
		}
		private static bool A(string text, bool flag)
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
		private void A(StringBuilder stringBuilder, int num)
		{
			string value = TextBlock.A(num);
			if (!string.IsNullOrEmpty(this.Name))
			{
				stringBuilder.Append(value);
				string value2;
				if (TextBlock.A(this.Name, false))
				{
					value2 = string.Format("\"{0}\"", d.EncodeDelimiterFormatString(this.Name));
				}
				else
				{
					value2 = this.Name;
				}
				stringBuilder.Append(value2);
				if (!string.IsNullOrEmpty(this.Data))
				{
					stringBuilder.Append(" ");
					string value3;
					if (TextBlock.A(this.Data, false))
					{
						value3 = string.Format("\"{0}\"", d.EncodeDelimiterFormatString(this.Data));
					}
					else
					{
						value3 = this.Data;
					}
					stringBuilder.Append(value3);
				}
				stringBuilder.Append("\r\n");
				stringBuilder.Append(value);
				stringBuilder.Append("{\r\n");
			}
			foreach (TextBlock.Attribute current in this.X)
			{
				string arg;
				if (TextBlock.A(current.Name, false))
				{
					arg = string.Format("\"{0}\"", d.EncodeDelimiterFormatString(current.Name));
				}
				else
				{
					arg = current.Name;
				}
				string arg2;
				if (TextBlock.A(current.Value, true))
				{
					arg2 = string.Format("\"{0}\"", d.EncodeDelimiterFormatString(current.Value));
				}
				else
				{
					arg2 = current.Value;
				}
				stringBuilder.Append(value);
				stringBuilder.Append((num != -1) ? "\t" : "");
				stringBuilder.AppendFormat("{0} = {1}\r\n", arg, arg2);
			}
			foreach (TextBlock current2 in this.W)
			{
				current2.A(stringBuilder, num + 1);
			}
			if (!string.IsNullOrEmpty(this.Name))
			{
				stringBuilder.Append(value);
				stringBuilder.Append("}\r\n");
			}
		}
		public string DumpToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.A(stringBuilder, -1);
			return stringBuilder.ToString();
		}
		public override string ToString()
		{
			string text = string.Format("Name: \"{0}\"", this.V);
			if (!string.IsNullOrEmpty(this.v))
			{
				text += string.Format(", Data: \"{0}\"", this.v);
			}
			return text;
		}
		public static TextBlock Parse(string str, out string errorString)
		{
			return e.Parse(str, out errorString);
		}
	}
}
