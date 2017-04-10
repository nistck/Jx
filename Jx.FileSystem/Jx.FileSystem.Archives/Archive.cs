using System;
namespace Jx.FileSystem.Archives
{
	public abstract class Archive : IDisposable
	{
		protected internal struct GetListFileInfo
		{
			private string aW;
			private long aw;
			public string FileName
			{
				get
				{
					return this.aW;
				}
			}
			public long Length
			{
				get
				{
					return this.aw;
				}
			}
			public GetListFileInfo(string fileName, long length)
			{
				this.aW = fileName;
				this.aw = length;
			}
		}
		private ArchiveFactory aB;
		private string ab;
		public ArchiveFactory Factory
		{
			get
			{
				return this.aB;
			}
		}
		public string FileName
		{
			get
			{
				return this.ab;
			}
		}
		public virtual void Dispose()
		{
		}
		protected Archive(ArchiveFactory factory, string fileName)
		{
			this.aB = factory;
			this.ab = fileName;
		}
		protected internal abstract void OnGetDirectoryAndFileList(out string[] directories, out Archive.GetListFileInfo[] files);
		protected internal abstract VirtualFileStream OnFileOpen(string inArchiveFileName);
	}
}
