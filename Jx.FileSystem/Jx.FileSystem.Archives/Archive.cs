using System;
namespace Jx.FileSystem.Archives
{
	public abstract class Archive : IDisposable
	{
		protected internal struct GetListFileInfo
		{
			private string fileName;
			private long length;
			public string FileName
			{
				get
				{
					return this.fileName;
				}
			}
			public long Length
			{
				get
				{
					return this.length;
				}
			}
			public GetListFileInfo(string fileName, long length)
			{
				this.fileName = fileName;
				this.length = length;
			}
		}
		private ArchiveFactory archiveFactory;
		private string fileName;
		public ArchiveFactory Factory
		{
			get
			{
				return this.archiveFactory;
			}
		}
		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}
		public virtual void Dispose()
		{
		}
		protected Archive(ArchiveFactory factory, string fileName)
		{
			this.archiveFactory = factory;
			this.fileName = fileName;
		}
		protected internal abstract void OnGetDirectoryAndFileList(out string[] directories, out Archive.GetListFileInfo[] files);
		protected internal abstract VirtualFileStream OnFileOpen(string inArchiveFileName);
	}
}
