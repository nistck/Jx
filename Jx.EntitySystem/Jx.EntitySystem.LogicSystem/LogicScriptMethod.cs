using System;
namespace Jx.EntitySystem.LogicSystem
{
	public class LogicScriptMethod : LogicMethod
	{
		[Entity.FieldSerializeAttribute("code")]
		private string abi;
		public string Code
		{
			get
			{
				return this.abi;
			}
			set
			{
				this.abi = value;
			}
		}
		protected override string[] GetCompileScriptsBody(string namespaceName)
		{
			if (this.abi == null)
			{
				return new string[0];
			}
			string[] array = this.abi.Split(new char[]
			{
				'\n'
			}, StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Replace("\r", "");
			}
			return array;
		}
	}
}
