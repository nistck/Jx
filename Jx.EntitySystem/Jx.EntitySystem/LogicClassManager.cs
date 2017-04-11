using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace Jx.EntitySystem
{
	public class LogicClassManager : LogicComponent
	{
		private Dictionary<string, LogicClass> aBT;
		public Dictionary<string, LogicClass>.ValueCollection Classes
		{
			get
			{
				this.A();
				return this.aBT.Values;
			}
		}
		private void A()
		{
			if (this.aBT == null)
			{
				this.aBT = new Dictionary<string, LogicClass>();
				foreach (Entity current in base.Children)
				{
					LogicClass logicClass = current as LogicClass;
					if (logicClass != null)
					{
						this.aBT.Add(logicClass.ClassName, logicClass);
					}
				}
			}
		}
		public LogicClass GetByName(string className)
		{
			this.A();
			LogicClass result;
			this.aBT.TryGetValue(className, out result);
			return result;
		}
		protected internal override void OnRemoveChild(Entity entity)
		{
			if (this.aBT != null)
			{
				LogicClass logicClass = entity as LogicClass;
				if (logicClass != null)
				{
					this.aBT.Remove(logicClass.ClassName);
				}
			}
			base.OnRemoveChild(entity);
		}
		public LogicClass CreateStaticClass(string className)
		{
			this.A();
			LogicClass logicClass = this.GetByName(className);
			if (logicClass != null)
			{
				Log.Fatal("LogicClassManager: already create class \"{0}\"", className);
				return null;
			}
			logicClass = (LogicClass)Entities.Instance.Create(EntityTypes.Instance.GetByName("LogicClass"), this);
			logicClass.ClassName = className;
			logicClass.PostCreate();
			this.aBT.Add(className, logicClass);
			return logicClass;
		}
		public LogicEntityClass CreateEntityClass(string className, Type entityClassType)
		{
			this.A();
			LogicClass byName = this.GetByName(className);
			if (byName != null)
			{
				Log.Fatal("LogicClassManager: already create class \"{0}\"", className);
				return null;
			}
			LogicEntityClass logicEntityClass = (LogicEntityClass)Entities.Instance.Create(EntityTypes.Instance.GetByName("LogicEntityClass"), this);
			logicEntityClass.ClassName = className;
			logicEntityClass.EntityClassName = entityClassType.Name;
			logicEntityClass.PostCreate();
			this.aBT.Add(className, logicEntityClass);
			return logicEntityClass;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsExistsScriptsDataForCompiling()
		{
			this.A();
			return this.aBT.Count != 0;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public List<LogicClass.CompileScriptsData> GetCompileScriptsData(string namespacePrefix)
		{
			this.A();
			List<LogicClass.CompileScriptsData> list = new List<LogicClass.CompileScriptsData>();
			foreach (LogicClass current in this.aBT.Values)
			{
				list.Add(current.GetCompileScriptsData(namespacePrefix));
			}
			return list;
		}
	}
}
