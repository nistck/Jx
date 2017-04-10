using System;
using System.IO;
using System.Runtime.InteropServices;
namespace Jx.FileSystem
{
	public sealed class MemoryVirtualFileStream : VirtualFileStream
	{
		private byte[] aO;
		private int ao;
		private bool aP;
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}
		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}
		public override long Length
		{
			get
			{
				return (long)this.aO.Length;
			}
		}
		public override long Position
		{
			get
			{
				if (this.aP)
				{
					throw new ObjectDisposedException(null);
				}
				return (long)this.ao;
			}
			set
			{
				this.Seek(value, SeekOrigin.Begin);
			}
		}
		public MemoryVirtualFileStream(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			this.aO = buffer;
		}
		public override void Close()
		{
			this.aP = true;
			base.Close();
		}
		protected override void Dispose(bool disposing)
		{
			this.aP = true;
			base.Dispose(disposing);
		}
		public override void Flush()
		{
		}
		public override long Seek(long offset, SeekOrigin origin)
		{
			if (this.aP)
			{
				throw new ObjectDisposedException(null);
			}
			switch (origin)
			{
			case SeekOrigin.Begin:
				if (offset < 0L)
				{
					throw new IOException("Seek before begin.");
				}
				this.ao = (int)offset;
				break;
			case SeekOrigin.Current:
				if ((long)this.ao + offset < 0L)
				{
					throw new IOException("Seek before begin.");
				}
				this.ao += (int)offset;
				break;
			case SeekOrigin.End:
				if ((long)this.aO.Length + offset < 0L)
				{
					throw new IOException("Seek before begin.");
				}
				this.ao = this.aO.Length + (int)offset;
				break;
			default:
				throw new ArgumentException("Invalid seek origin.");
			}
			return (long)this.ao;
		}
		public override void SetLength(long value)
		{
			throw new NotSupportedException("The method is not supported.");
		}
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("The method is not supported.");
		}
		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.aP)
			{
				throw new ObjectDisposedException(null);
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("Invalid offset length.");
			}
			int num = this.aO.Length - this.ao;
			if (num > count)
			{
				num = count;
			}
			if (num <= 0)
			{
				return 0;
			}
			if (num <= 8)
			{
				int num2 = num;
				while (--num2 >= 0)
				{
					buffer[offset + num2] = this.aO[this.ao + num2];
				}
			}
			else
			{
				Buffer.BlockCopy(this.aO, this.ao, buffer, offset, num);
			}
			this.ao += num;
			return num;
		}
		public override int ReadUnmanaged(IntPtr buffer, int count)
		{
			if (this.aP)
			{
				throw new ObjectDisposedException(null);
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			int num = this.aO.Length - this.ao;
			if (num > count)
			{
				num = count;
			}
			if (num <= 0)
			{
				return 0;
			}
			Marshal.Copy(this.aO, this.ao, buffer, num);
			this.ao += num;
			return num;
		}
		public override int ReadByte()
		{
			if (this.aP)
			{
				throw new ObjectDisposedException(null);
			}
			if (this.ao >= this.aO.Length)
			{
				return -1;
			}
			return (int)this.aO[this.ao++];
		}
	}
}
