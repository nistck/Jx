using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Ext
{
    public static class SimpleTypesUtils
    {
        private delegate object A(string value);
        private class a
        {
            private SimpleTypesUtils.A ef;
            private object eG;
            public SimpleTypesUtils.A ParseDelegate
            {
                get
                {
                    return this.ef;
                }
            }
            public object DefaultValue
            {
                get
                {
                    return this.eG;
                }
            }
            public a(SimpleTypesUtils.A parseDelegate, object defaultValue)
            {
                this.ef = parseDelegate;
                this.eG = defaultValue;
            }
        }
        private static Dictionary<Type, SimpleTypesUtils.a> Dv;
        private static Type[] DW;
        
        private static SimpleTypesUtils.A Dw;
        
        private static SimpleTypesUtils.A DX;
        
        private static SimpleTypesUtils.A Dx;
        
        private static SimpleTypesUtils.A DY;
        
        private static SimpleTypesUtils.A Dy;
        
        private static SimpleTypesUtils.A DZ;
        
        private static SimpleTypesUtils.A Dz;
        
        private static SimpleTypesUtils.A dA;
        
        private static SimpleTypesUtils.A da;
        
        private static SimpleTypesUtils.A dB;
        
        private static SimpleTypesUtils.A db;
        
        private static SimpleTypesUtils.A dC;
        
        private static SimpleTypesUtils.A dc;
        
        private static SimpleTypesUtils.A dD;
        
        private static SimpleTypesUtils.A dd;
        
        private static SimpleTypesUtils.A dE;
        
        private static SimpleTypesUtils.A de;
        
        private static SimpleTypesUtils.A dF;
        
        private static SimpleTypesUtils.A df;
        
        private static SimpleTypesUtils.A dG;
        
        private static SimpleTypesUtils.A dg;
        
        private static SimpleTypesUtils.A dH;
        
        private static SimpleTypesUtils.A dh;
        
        private static SimpleTypesUtils.A dI;
        
        private static SimpleTypesUtils.A di;
        
        private static SimpleTypesUtils.A dJ;
        
        private static SimpleTypesUtils.A dj;
        
        private static SimpleTypesUtils.A dK;
        
        private static SimpleTypesUtils.A dk;
        
        private static SimpleTypesUtils.A dL;
        
        private static SimpleTypesUtils.A dl;
        
        private static SimpleTypesUtils.A dM;
        
        private static SimpleTypesUtils.A dm;
        
        private static SimpleTypesUtils.A dN;
        
        private static SimpleTypesUtils.A dn;
        
        private static SimpleTypesUtils.A dO;
        
        private static SimpleTypesUtils.A @do;
        
        private static SimpleTypesUtils.A dP;

        public static Type[] SimpleTypes
        {
            get
            {
                return SimpleTypesUtils.DW;
            }
        }
        static SimpleTypesUtils()
        {
            SimpleTypesUtils.Dv = new Dictionary<Type, SimpleTypesUtils.a>();
            Dictionary<Type, SimpleTypesUtils.a> arg_40_0 = SimpleTypesUtils.Dv;
            Type arg_40_1 = typeof(string);
            if (SimpleTypesUtils.Dw == null)
            {
                SimpleTypesUtils.Dw = new SimpleTypesUtils.A(SimpleTypesUtils.GetSimpleTypeValue);
            }
            arg_40_0.Add(arg_40_1, new SimpleTypesUtils.a(SimpleTypesUtils.Dw, ""));
            Dictionary<Type, SimpleTypesUtils.a> arg_7C_0 = SimpleTypesUtils.Dv;
            Type arg_7C_1 = typeof(bool);
            if (SimpleTypesUtils.DX == null)
            {
                SimpleTypesUtils.DX = new SimpleTypesUtils.A(SimpleTypesUtils.ToBool);
            }
            arg_7C_0.Add(arg_7C_1, new SimpleTypesUtils.a(SimpleTypesUtils.DX, false));
            Dictionary<Type, SimpleTypesUtils.a> arg_B8_0 = SimpleTypesUtils.Dv;
            Type arg_B8_1 = typeof(sbyte);
            if (SimpleTypesUtils.Dx == null)
            {
                SimpleTypesUtils.Dx = new SimpleTypesUtils.A(SimpleTypesUtils.B);
            }
            arg_B8_0.Add(arg_B8_1, new SimpleTypesUtils.a(SimpleTypesUtils.Dx, 0));
            Dictionary<Type, SimpleTypesUtils.a> arg_F4_0 = SimpleTypesUtils.Dv;
            Type arg_F4_1 = typeof(byte);
            if (SimpleTypesUtils.DY == null)
            {
                SimpleTypesUtils.DY = new SimpleTypesUtils.A(SimpleTypesUtils.b);
            }
            arg_F4_0.Add(arg_F4_1, new SimpleTypesUtils.a(SimpleTypesUtils.DY, 0));
            Dictionary<Type, SimpleTypesUtils.a> arg_130_0 = SimpleTypesUtils.Dv;
            Type arg_130_1 = typeof(char);
            if (SimpleTypesUtils.Dy == null)
            {
                SimpleTypesUtils.Dy = new SimpleTypesUtils.A(SimpleTypesUtils.C);
            }
            arg_130_0.Add(arg_130_1, new SimpleTypesUtils.a(SimpleTypesUtils.Dy, 0));
            Dictionary<Type, SimpleTypesUtils.a> arg_16C_0 = SimpleTypesUtils.Dv;
            Type arg_16C_1 = typeof(short);
            if (SimpleTypesUtils.DZ == null)
            {
                SimpleTypesUtils.DZ = new SimpleTypesUtils.A(SimpleTypesUtils.c);
            }
            arg_16C_0.Add(arg_16C_1, new SimpleTypesUtils.a(SimpleTypesUtils.DZ, 0));
            Dictionary<Type, SimpleTypesUtils.a> arg_1A8_0 = SimpleTypesUtils.Dv;
            Type arg_1A8_1 = typeof(ushort);
            if (SimpleTypesUtils.Dz == null)
            {
                SimpleTypesUtils.Dz = new SimpleTypesUtils.A(SimpleTypesUtils.D);
            }
            arg_1A8_0.Add(arg_1A8_1, new SimpleTypesUtils.a(SimpleTypesUtils.Dz, 0));
            Dictionary<Type, SimpleTypesUtils.a> arg_1E4_0 = SimpleTypesUtils.Dv;
            Type arg_1E4_1 = typeof(int);
            if (SimpleTypesUtils.dA == null)
            {
                SimpleTypesUtils.dA = new SimpleTypesUtils.A(SimpleTypesUtils.d);
            }
            arg_1E4_0.Add(arg_1E4_1, new SimpleTypesUtils.a(SimpleTypesUtils.dA, 0));
            Dictionary<Type, SimpleTypesUtils.a> arg_220_0 = SimpleTypesUtils.Dv;
            Type arg_220_1 = typeof(uint);
            if (SimpleTypesUtils.da == null)
            {
                SimpleTypesUtils.da = new SimpleTypesUtils.A(SimpleTypesUtils.E);
            }
            arg_220_0.Add(arg_220_1, new SimpleTypesUtils.a(SimpleTypesUtils.da, 0u));
            Dictionary<Type, SimpleTypesUtils.a> arg_25D_0 = SimpleTypesUtils.Dv;
            Type arg_25D_1 = typeof(long);
            if (SimpleTypesUtils.dB == null)
            {
                SimpleTypesUtils.dB = new SimpleTypesUtils.A(SimpleTypesUtils.e);
            }
            arg_25D_0.Add(arg_25D_1, new SimpleTypesUtils.a(SimpleTypesUtils.dB, 0L));
            Dictionary<Type, SimpleTypesUtils.a> arg_29A_0 = SimpleTypesUtils.Dv;
            Type arg_29A_1 = typeof(ulong);
            if (SimpleTypesUtils.db == null)
            {
                SimpleTypesUtils.db = new SimpleTypesUtils.A(SimpleTypesUtils.F);
            }
            arg_29A_0.Add(arg_29A_1, new SimpleTypesUtils.a(SimpleTypesUtils.db, 0uL));
            Dictionary<Type, SimpleTypesUtils.a> arg_2DA_0 = SimpleTypesUtils.Dv;
            Type arg_2DA_1 = typeof(float);
            if (SimpleTypesUtils.dC == null)
            {
                SimpleTypesUtils.dC = new SimpleTypesUtils.A(SimpleTypesUtils.f);
            }
            arg_2DA_0.Add(arg_2DA_1, new SimpleTypesUtils.a(SimpleTypesUtils.dC, 0f));
            Dictionary<Type, SimpleTypesUtils.a> arg_31E_0 = SimpleTypesUtils.Dv;
            Type arg_31E_1 = typeof(double);
            if (SimpleTypesUtils.dc == null)
            {
                SimpleTypesUtils.dc = new SimpleTypesUtils.A(SimpleTypesUtils.G);
            }
            arg_31E_0.Add(arg_31E_1, new SimpleTypesUtils.a(SimpleTypesUtils.dc, 0.0));
            Dictionary<Type, SimpleTypesUtils.a> arg_35E_0 = SimpleTypesUtils.Dv;

        }
        public static bool IsSimpleType(Type type)
        {
            return typeof(Enum).IsAssignableFrom(type) || SimpleTypesUtils.Dv.ContainsKey(type);
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
            SimpleTypesUtils.a a;
            if (!SimpleTypesUtils.Dv.TryGetValue(type, out a))
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
            SimpleTypesUtils.a a;
            if (!SimpleTypesUtils.Dv.TryGetValue(type, out a))
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
        
        private static object B(string s)
        {
            return sbyte.Parse(s);
        }
        
        private static object b(string s)
        {
            return byte.Parse(s);
        }
        
        private static object C(string s)
        {
            return char.Parse(s);
        }
        
        private static object c(string s)
        {
            return short.Parse(s);
        }
        
        private static object D(string s)
        {
            return ushort.Parse(s);
        }
        
        private static object d(string s)
        {
            return int.Parse(s);
        }
        
        private static object E(string s)
        {
            return uint.Parse(s);
        }
        
        private static object e(string s)
        {
            return long.Parse(s);
        }
        
        private static object F(string s)
        {
            return ulong.Parse(s);
        }
        
        private static object f(string text)
        {
            if (string.Compare(text, "infinity", true) == 0)
            {
                return float.PositiveInfinity;
            }
            return float.Parse(text);
        }
        
        private static object G(string text)
        {
            if (string.Compare(text, "infinity", true) == 0)
            {
                return double.PositiveInfinity;
            }
            return double.Parse(text);
        } 
    }
}
