using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Ext
{
    public static class SimpleTypesUtils
    {
        private delegate object ParseDelegator(string value);
        private class ParserInfo
        {
            private ParseDelegator parseDelegate;
            private object defaultValue;
            public ParseDelegator ParseDelegate
            {
                get
                {
                    return this.parseDelegate;
                }
            }
            public object DefaultValue
            {
                get
                {
                    return this.defaultValue;
                }
            }
            public ParserInfo(ParseDelegator parseDelegate, object defaultValue)
            {
                this.parseDelegate = parseDelegate;
                this.defaultValue = defaultValue;
            }
        }
        private static Dictionary<Type, ParserInfo> Dv; 
        
        private static ParseDelegator StringParser;
        
        private static ParseDelegator BoolParser;
        
        private static ParseDelegator SbyteParser;
        
        private static ParseDelegator ByteParser;
        
        private static ParseDelegator CharParser;
        
        private static ParseDelegator ShortParser;
        
        private static ParseDelegator UshortParser;
        
        private static ParseDelegator IntParser;
        
        private static ParseDelegator UintParser;
        
        private static ParseDelegator LongParser;
        
        private static ParseDelegator UlongParser;
        
        private static ParseDelegator FloatParser;
        
        private static ParseDelegator DoubleParser; 
 
        static SimpleTypesUtils()
        {
            Dv = new Dictionary<Type, ParserInfo>();

            if (StringParser == null)
                StringParser = new ParseDelegator(GetSimpleTypeValue);
            Dv.Add(typeof(string), new ParserInfo(StringParser, ""));
            
            if (BoolParser == null)
                BoolParser = new ParseDelegator(ToBool);
            Dv.Add(typeof(bool), new ParserInfo(BoolParser, false));

            if (SbyteParser == null)
                SbyteParser = new ParseDelegator(ToSbyte);
            Dv.Add(typeof(sbyte), new ParserInfo(SbyteParser, 0));
             
            if (ByteParser == null)
                ByteParser = new ParseDelegator(ToByte);
            Dv.Add(typeof(byte), new ParserInfo(ByteParser, 0));

            if (CharParser == null)
                CharParser = new ParseDelegator(ToChar);
            Dv.Add(typeof(char), new ParserInfo(CharParser, 0));

            if (ShortParser == null)
                ShortParser = new ParseDelegator(ToShort);
            Dv.Add(typeof(short), new ParserInfo(ShortParser, 0));

            if (UshortParser == null)
                UshortParser = new ParseDelegator(ToUshort);
            Dv.Add(typeof(ushort), new ParserInfo(UshortParser, 0));

            if (IntParser == null)
                IntParser = new ParseDelegator(ToInt);
            Dv.Add(typeof(int), new ParserInfo(IntParser, 0));

            Type arg_220_1 = typeof(uint);
            if (UintParser == null)
                UintParser = new ParseDelegator(ToUint);
            Dv.Add(typeof(uint), new ParserInfo(UintParser, 0u));

            if (LongParser == null)
                LongParser = new ParseDelegator(ToLong);
            Dv.Add(typeof(long), new ParserInfo(LongParser, 0L));

            if (UlongParser == null)
                UlongParser = new ParseDelegator(ToUlong);
            Dv.Add(typeof(ulong), new ParserInfo(UlongParser, 0uL));

            if (FloatParser == null)
                FloatParser = new ParseDelegator(ToFloat);
            Dv.Add(typeof(float), new ParserInfo(FloatParser, 0f));

            if (DoubleParser == null)
                DoubleParser = new ParseDelegator(ToDouble);
            Dv.Add(typeof(double), new ParserInfo(DoubleParser, 0.0));

        }

        public static bool IsSimpleType(Type type)
        {
            return typeof(Enum).IsAssignableFrom(type) || Dv.ContainsKey(type);
        }

        /// <summary>
        /// If null - not simple type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object GetSimpleTypeValue(Type type, string value)
        {
            if (typeof(Enum).IsAssignableFrom(type))
            {
                return Enum.Parse(type, value);
            }
            ParserInfo a;
            if (!Dv.TryGetValue(type, out a))
            {
                return null;
            }
            return a.ParseDelegate(value);
        }

        public static object GetSimpleTypeDefaultValue(Type type)
        {
            if (typeof(Enum).IsAssignableFrom(type))
            {
                Log.Fatal("SimpleTypesUtils: GetSimpleTypeDefaultValue: type is enum: not implemented");
                return null;
            }
            ParserInfo a;
            if (!Dv.TryGetValue(type, out a))
            {
                return null;
            }
            return a.DefaultValue;
        }
        
        private static object GetSimpleTypeValue(string text)
        {
            if (text == null)
            {
                throw new Exception("GetSimpleTypeValue: string type, value = null");
            }
            return text;
        }
        
        private static object ToBool(string text)
        {
            if (text == "1")
            {
                return true;
            }
            if (text == "0")
            {
                return false;
            }
            return bool.Parse(text);
        }
        
        private static object ToSbyte(string s)
        {
            return sbyte.Parse(s);
        }
        
        private static object ToByte(string s)
        {
            return byte.Parse(s);
        }
        
        private static object ToChar(string s)
        {
            return char.Parse(s);
        }
        
        private static object ToShort(string s)
        {
            return short.Parse(s);
        }
        
        private static object ToUshort(string s)
        {
            return ushort.Parse(s);
        }
        
        private static object ToInt(string s)
        {
            return int.Parse(s);
        }
        
        private static object ToUint(string s)
        {
            return uint.Parse(s);
        }
        
        private static object ToLong(string s)
        {
            return long.Parse(s);
        }
        
        private static object ToUlong(string s)
        {
            return ulong.Parse(s);
        }
        
        private static object ToFloat(string text)
        {
            if (string.Compare(text, "infinity", true) == 0)
            {
                return float.PositiveInfinity;
            }
            return float.Parse(text);
        }
        
        private static object ToDouble(string text)
        {
            if (string.Compare(text, "infinity", true) == 0)
            {
                return double.PositiveInfinity;
            }
            return double.Parse(text);
        } 
    }
}
