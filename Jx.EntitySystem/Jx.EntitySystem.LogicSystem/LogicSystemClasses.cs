using Jx.Ext;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

using Jx.FileSystem;

namespace Jx.EntitySystem.LogicSystem
{
	public class LogicSystemClasses
	{
		private static LogicSystemClasses instance;
		private Dictionary<string, LogicSystemClass> logicSystemClassNameDic = new Dictionary<string, LogicSystemClass>();
		private Dictionary<string, LogicSystemClass> logicSystemClassTypeNameDic = 
                    new Dictionary<string, LogicSystemClass>();
		private Dictionary<Type, LogicSystemClass> logicSystemClassTypeDic = new Dictionary<Type, LogicSystemClass>();

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
				return this.logicSystemClassNameDic.Values;
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
			LoadLogicClasses();
		}

		private LogicSystemClass DefineLogicClass(Type classType)
		{
            if (classType == null)
                return null; 

			LogicSystemClass logicSystemClass = new LogicSystemClass(classType);
            if (logicSystemClassNameDic.ContainsKey(logicSystemClass.Name))
                return logicSystemClassNameDic[logicSystemClass.Name];

			logicSystemClassNameDic.Add(logicSystemClass.Name, logicSystemClass);
			logicSystemClassTypeNameDic.Add(logicSystemClass.ClassType.FullName, logicSystemClass);
			logicSystemClassTypeDic.Add(logicSystemClass.ClassType, logicSystemClass);
			return logicSystemClass;
		}

		private void DefineBaseObject()
		{
			LogicSystemClass logicSystemClass = DefineLogicClass(typeof(object));
			logicSystemClass.DefineMethod(logicSystemClass.ClassType.GetMethod("ToString"));
			logicSystemClass = DefineLogicClass(typeof(ValueType));
			logicSystemClass = DefineLogicClass(typeof(string));

			logicSystemClass = DefineLogicClass(typeof(bool));
			logicSystemClass.DefineMethod(logicSystemClass.ClassType.GetMethod("Parse", new Type[]
			{
				typeof(string)
			}));

			logicSystemClass = DefineLogicClass(typeof(int));
			logicSystemClass.DefineMethod(logicSystemClass.ClassType.GetMethod("ToString", new Type[]
			{
				typeof(string)
			}));
			logicSystemClass.DefineMethod(logicSystemClass.ClassType.GetMethod("Parse", new Type[]
			{
				typeof(string)
			}));

			logicSystemClass = DefineLogicClass(typeof(float));
			logicSystemClass.DefineMethod(logicSystemClass.ClassType.GetMethod("ToString", new Type[]
			{
				typeof(string)
			}));
			logicSystemClass.DefineMethod(logicSystemClass.ClassType.GetMethod("Parse", new Type[]
			{
				typeof(string)
			}));

			logicSystemClass = DefineLogicClass(typeof(Log));
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

		private void LoadLogicClasses()
		{
			List<Assembly> list = new List<Assembly>();
			foreach (string current in EntitySystemWorld.Instance.LogicSystemSystemClassesAssemblies)
			{
				Assembly item = AssemblyUtils.LoadAssemblyByRealFileName(current, false);
				list.Add(item);
			}

			Dictionary<Type, Type> typeDic = new Dictionary<Type, Type>(); 
			foreach (Assembly current in list)
			{
                string assemblyMessage = string.Format("搜索LogicClass, 程序集: {0}", current.GetName().Name);

				Type[] types = current.GetTypes().Where(_type => !typeDic.ContainsKey(_type)).ToArray(); 
				for (int i = 0; i < types.Length; i++)
				{
					Type type = types[i];

                    List<Type> typeList = new List<Type>();
                    Type typeCurrent = type;
                    while (typeCurrent != null)
                    {
                        typeList.Add(typeCurrent);
                        typeCurrent = typeCurrent.BaseType;
                    }
                    var typeQuery = typeList.Where(_type => !typeDic.ContainsKey(_type))
                        .Where(_type => {
                            LogicSystemBrowsableAttribute[] _rAttrs = (LogicSystemBrowsableAttribute[])_type.GetCustomAttributes(typeof(LogicSystemBrowsableAttribute), true);
                            bool _typeBrowsable = _rAttrs.Where(_attr => !_attr.Browsable).Count() == 0;
                            return _typeBrowsable;
                        })
                        ;
                    typeQuery.Any(_type => {
                        DefineLogicClass(_type);
                        return false;
                    });

                    LongOperationNotifier.Notify("{0}, {1}/{2}", assemblyMessage, i + 1, types.Length);
                }
			} 
		}

		private void _Shutdown()
		{
		}

		public LogicSystemClass GetByName(string name)
		{
			LogicSystemClass result;
			logicSystemClassNameDic.TryGetValue(name, out result);
			return result;
		}

		public LogicSystemClass GetByFullName(string fullName)
		{
			LogicSystemClass result;
			logicSystemClassTypeNameDic.TryGetValue(fullName, out result);
			return result;
		}

		public LogicSystemClass GetByType(Type type)
		{
			LogicSystemClass result;
			this.logicSystemClassTypeDic.TryGetValue(type, out result);
			return result;
		}
	}
}
