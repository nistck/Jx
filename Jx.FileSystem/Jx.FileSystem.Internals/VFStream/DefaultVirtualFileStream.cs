using Jx.FileSystem;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Jx.FileSystem.Internals.VFStream
{
	internal sealed class DefaultVirtualFileStream : VirtualFileStream
	{
		private FileStream aI;
		private byte[] ai;
		public override bool CanRead
		{
			get
			{
				if (this.aI == null)
				{
					throw new ObjectDisposedException(null);
				}
				return this.aI.CanRead;
			}
		}
		public override bool CanSeek
		{
			get
			{
				if (this.aI == null)
				{
					throw new ObjectDisposedException(null);
				}
				return this.aI.CanSeek;
			}
		}
		public override bool CanWrite
		{
			get
			{
				if (this.aI == null)
				{
					throw new ObjectDisposedException(null);
				}
				return this.aI.CanWrite;
			}
		}
		public override long Length
		{
			get
			{
				if (this.aI == null)
				{
					throw new ObjectDisposedException(null);
				}
				return this.aI.Length;
			}
		}
		public override long Position
		{
			get
			{
				if (this.aI == null)
				{
					throw new ObjectDisposedException(null);
				}
				return this.aI.Position;
			}
			set
			{
				if (this.aI == null)
				{
					throw new ObjectDisposedException(null);
				}
				this.aI.Position = value;
			}
		}
		public DefaultVirtualFileStream(string realPath)
		{
			this.aI = new FileStream(realPath, FileMode.Open, FileAccess.Read);
		}
		public override void Close()
		{
			if (this.aI != null)
			{
				this.aI.Close();
			}
			base.Close();
		}
		protected override void Dispose(bool disposing)
		{
			if (this.aI != null)
			{
				this.aI.Dispose();
				this.aI = null;
			}
			base.Dispose(disposing);
		}
		public override void Flush()
		{
			if (this.aI == null)
			{
				throw new ObjectDisposedException(null);
			}
			this.aI.Flush();
		}
		public override long Seek(long offset, SeekOrigin origin)
		{
			if (this.aI == null)
			{
				throw new ObjectDisposedException(null);
			}
			return this.aI.Seek(offset, origin);
		}
		public override void SetLength(long value)
		{
			if (this.aI == null)
			{
				throw new ObjectDisposedException(null);
			}
			this.aI.SetLength(value);
		}
		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.aI == null)
			{
				throw new ObjectDisposedException(null);
			}
			this.aI.Write(buffer, offset, count);
		}
		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.aI == null)
			{
				throw new ObjectDisposedException(null);
			}
			return this.aI.Read(buffer, offset, count);
		}
		public override int ReadUnmanaged(IntPtr buffer, int count)
		{
			if (this.aI == null)
			{
				throw new ObjectDisposedException(null);
			}
			if (this.ai == null || this.ai.Length < count)
			{
				this.ai = new byte[count];
			}
			int num = this.aI.Read(this.ai, 0, count);
			if (num > 0)
			{
				Marshal.Copy(this.ai, 0, buffer, num);
			}
			return num;
		}
		public override int ReadByte()
		{
			if (this.aI == null)
			{
				throw new ObjectDisposedException(null);
			}
			return this.aI.ReadByte();
		}
	}
	
}
