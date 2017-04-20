using System;
using System.ComponentModel;
using System.Reflection;
namespace Jx.EntitySystem
{
	public class LogicEntityClass : LogicClass
	{
		[Entity.FieldSerializeAttribute("entityClassName")]
		private string abe;
		private EntityTypes.ClassInfo abF;
		public string EntityClassName
		{
			get
			{
				return this.abe;
			}
			set
			{
				this.abe = value;
				this.abF = null;
			}
		}
		public EntityTypes.ClassInfo EntityClassInfo
		{
			get
			{
				if (this.abF == null)
				{
					this.abF = EntityTypes.Instance.GetClassInfoByEntityClassName(this.abe);
				}
				return this.abF;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected override void GetCompileScriptsClassBody(string namespaceName, LogicClass.CompileScriptsData data)
		{
			string text = this.abe;
			if (this.EntityClassInfo != null)
			{
				text = Jx.Ext.CJ.TypeToCSharpString(this.EntityClassInfo.entityClassType);
			}
			data.Strings.Add(string.Format("\t\t{0} __ownerEntity;", text));
			data.Strings.Add("\t\t");
			data.Strings.Add(string.Format("\t\tpublic {0}( {1} ownerEntity )", base.ClassName, text));
			data.Strings.Add("\t\t\t: base( ownerEntity )");
			data.Strings.Add("\t\t{");
			data.Strings.Add("\t\t\tthis.__ownerEntity = ownerEntity;");
			foreach (LogicMethod current in base.Methods)
			{
				if (current.IsEntityEventMethod)
				{
					string text2 = "\t\t\townerEntity." + current.MethodName + " += ";
					EventInfo entityEventInfo = current.EntityEventInfo;
					if (entityEventInfo == null)
					{
						Log.Warning("LogicEntityClass: Compilation error: Method \"{0}\" EntityEventInfo = null", current.ToString());
						return;
					}
					MethodInfo method = entityEventInfo.EventHandlerType.GetMethod("Invoke");
					string arg = Jx.Ext.CJ.TypeToCSharpString(method.GetParameters()[0].ParameterType);
					text2 += string.Format("delegate( {0} __entity", arg);
					foreach (LogicParameter current2 in current.Parameters)
					{
						string text3 = text2;
						text2 = string.Concat(new string[]
						{
							text3,
							", ",
							Jx.Ext.CJ.TypeToCSharpString(current2.ParameterType),
							" ",
							current2.ParameterName
						});
					}
					text2 += " ) { ";
					text2 += "if( Engine.EntitySystem.LogicSystemManager.Instance != null )";
					text2 = text2 + current.MethodName + "( ";
					for (int i = 0; i < current.Parameters.Count; i++)
					{
						if (i != 0)
						{
							text2 += ", ";
						}
						text2 += current.Parameters[i].ParameterName;
					}
					text2 += " ); };";
					data.Strings.Add(text2);
				}
			}
			data.Strings.Add("\t\t}");
			data.Strings.Add("\t\t");
			data.Strings.Add(string.Format("\t\tpublic {0} Owner", text));
			data.Strings.Add("\t\t{");
			data.Strings.Add("\t\t\tget { return __ownerEntity; }");
			data.Strings.Add("\t\t}");
			data.Strings.Add("\t\t");
			base.GetCompileScriptsClassBody(namespaceName, data);
		}
		public LogicMethod CreateEntityEventMethod(LogicMethodType methodType, string methodName)
		{
			if (this.EntityClassInfo == null)
			{
				Log.Error("LogicEntityClass: CreateEntityEventMethod: class not defined \"{0}\"", this.abe);
				return null;
			}
			LogicMethod logicMethod = base.GetMethodByName(methodName);
			if (logicMethod != null)
			{
				Log.Fatal("LogicClass: already create method \"{0}\"", methodName);
				return null;
			}
			logicMethod = (LogicMethod)Entities.Instance.Create(methodType, this);
			logicMethod.MethodName = methodName;
			logicMethod.isEntityEventMethod = true;
			logicMethod.PostCreate();
			base.Add(logicMethod);
			MethodInfo method = logicMethod.EntityEventInfo.EventHandlerType.GetMethod("Invoke");
			ParameterInfo[] parameters = method.GetParameters();
			for (int i = 1; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				logicMethod.CreateParameter(parameterInfo.ParameterType, parameterInfo.Name);
			}
			return logicMethod;
		}
		public override string ToString()
		{
			return base.ToString() + "( " + this.EntityClassName + " )";
		}
	}
}
