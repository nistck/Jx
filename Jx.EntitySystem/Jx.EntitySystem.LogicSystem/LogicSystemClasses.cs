using Jx.Ext;
using System;
using System.Collections.Generic;
using System.Reflection;

using Jx.FileSystem;

namespace Jx.EntitySystem.LogicSystem
{
	public class LogicSystemClasses
	{
		private static LogicSystemClasses instance;
		private Dictionary<string, LogicSystemClass> aAT = new Dictionary<string, LogicSystemClass>();
		private Dictionary<string, LogicSystemClass> logicSystemClassNameDic = 
                    new Dictionary<string, LogicSystemClass>();
		private Dictionary<Type, LogicSystemClass> aAU = new Dictionary<Type, LogicSystemClass>();

		public static LogicSystemClasses Instance
		{
			get
			{
				return instance;
			}
		}

		public Dictionary<string, LogicSystemClass>.ValueCollection Classes
		{
			get
			{
				return this.aAT.Values;
			}
		}

		internal static void Init()
		{
			if (instance != null)
			{
				Log.Fatal("LogicSystemClasses: instance already created");
			}

            Log.Info(">> 初始化 逻辑系统...");
			instance = new LogicSystemClasses();
			instance.Startup();
		}

		internal static void Shutdown()
		{
			if (instance != null)
			{
				instance._Shutdown();
				instance = null;
			}
		}

		private void Startup()
		{
			DefineBaseObject();
			this.C();
		}

		private LogicSystemClass A(Type classType)
		{
			LogicSystemClass logicSystemClass = new LogicSystemClass(classType);
			this.aAT.Add(logicSystemClass.Name, logicSystemClass);
			this.logicSystemClassNameDic.Add(logicSystemClass.ClassType.FullName, logicSystemClass);
			this.aAU.Add(logicSystemClass.ClassType, logicSystemClass);
			return logicSystemClass;
		}

		private void DefineBaseObject()
		{
			LogicSystemClass logicSystemClass = this.A(typeof(object));
			logicSystemClass.DefineMethod(logicSystemClass.ClassType.GetMethod("ToString"));
			logicSystemClass = this.A(typeof(ValueType));
			logicSystemClass = this.A(typeof(string));
			logicSystemClass = this.A(typeof(bool));
			logicSystemClass.DefineMethod(logicSystemClass.ClassType.GetMethod("Parse", new Type[]
			{
				typeof(string)
			}));
			logicSystemClass = this.A(typeof(int));
			logicSystemClass.DefineMethod(logicSystemClass.ClassType.GetMethod("ToString", new Type[]
			{
				typeof(string)
			}));
			logicSystemClass.DefineMethod(logicSystemClass.ClassType.GetMethod("Parse", new Type[]
			{
				typeof(string)
			}));
			logicSystemClass = this.A(typeof(float));
			logicSystemClass.DefineMethod(logicSystemClass.ClassType.GetMethod("ToString", new Type[]
			{
				typeof(string)
			}));
			logicSystemClass.DefineMethod(logicSystemClass.ClassType.GetMethod("Parse", new Type[]
			{
				typeof(string)
			}));
			logicSystemClass = this.A(typeof(Log));
			logicSystemClass.DefineMethod(logicSystemClass.ClassType.GetMethod("Info", new Type[]
			{
				typeof(string)
			}));
			logicSystemClass.DefineMethod(logicSystemClass.ClassType.GetMethod("Warning", new Type[]
			{
				typeof(string)
			}));
			logicSystemClass.DefineMethod(logicSystemClass.ClassType.GetMethod("Error", new Type[]
			{
				typeof(string)
			}));
			logicSystemClass.DefineMethod(logicSystemClass.ClassType.GetMethod("Fatal", new Type[]
			{
				typeof(string)
			}));
		}

		private void C()
		{
			List<Assembly> list = new List<Assembly>();
			foreach (string current in EntitySystemWorld.Instance.LogicSystemSystemClassesAssemblies)
			{
				Assembly item = AssemblyUtils.LoadAssemblyByRealFileName(current, false);
				list.Add(item);
			}
			float time = EngineApp.Instance.Time;
			Dictionary<Type, Type> dictionary = new Dictionary<Type, Type>();
			List<Type> list2 = new List<Type>();
			foreach (Assembly current2 in list)
			{
				Type[] types = current2.GetTypes();
				for (int i = 0; i < types.Length; i++)
				{
					Type type = types[i];
					if (!dictionary.ContainsKey(type))
					{
						Type type2 = type;
						while (type2 != null)
						{
							list2.Add(type2);
							type2 = type2.BaseType;
						}
						for (int j = list2.Count - 1; j >= 0; j--)
						{
							Type type3 = list2[j];
							if (!dictionary.ContainsKey(type3))
							{
								dictionary.Add(type3, type3);
								bool flag = false;
								LogicSystemBrowsableAttribute[] array = (LogicSystemBrowsableAttribute[])type3.GetCustomAttributes(typeof(LogicSystemBrowsableAttribute), true);
								LogicSystemBrowsableAttribute[] array2 = array;
								for (int k = 0; k < array2.Length; k++)
								{
									LogicSystemBrowsableAttribute logicSystemBrowsableAttribute = array2[k];
									if (!logicSystemBrowsableAttribute.Browsable)
									{
										flag = false;
										break;
									}
									flag = true;
								}
								if (flag)
								{
									this.A(type3);
								}
							}
						}
						list2.Clear();
					}
				}
			}
			float arg_197_0 = EngineApp.Instance.Time;
		}

		private void _Shutdown()
		{
		}

		public LogicSystemClass GetByName(string name)
		{
			LogicSystemClass result;
			aAT.TryGetValue(name, out result);
			return result;
		}

		public LogicSystemClass GetByFullName(string fullName)
		{
			LogicSystemClass result;
			logicSystemClassNameDic.TryGetValue(fullName, out result);
			return result;
		}

		public LogicSystemClass GetByType(Type type)
		{
			LogicSystemClass result;
			this.aAU.TryGetValue(type, out result);
			return result;
		}
	}
}
