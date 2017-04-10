using System;
namespace Jx.FileSystem.Archives
{
	public abstract class ArchiveFactory : IDisposable
	{
		private string J;
		public string FileExtension
		{
			get
			{
				return this.J;
			}
		}
		protected ArchiveFactory(string fileExtension)
		{
			this.J = fileExtension;
		}
		protected internal abstract bool OnInit();
		public virtual void Dispose()
		{
		}
		protected internal abstract Archive OnLoadArchive(string fileName);
	}
}
