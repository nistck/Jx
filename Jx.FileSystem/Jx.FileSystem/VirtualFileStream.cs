using System;
using System.IO;
namespace Jx.FileSystem
{
	public abstract class VirtualFileStream : Stream
	{
		public abstract int ReadUnmanaged(IntPtr buffer, int count);
	}
}
