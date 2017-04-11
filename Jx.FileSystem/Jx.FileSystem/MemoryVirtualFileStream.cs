using System;
using System.IO;
using System.Runtime.InteropServices;
namespace Jx.FileSystem
{
	public sealed class MemoryVirtualFileStream : VirtualFileStream
	{
		private byte[] bytesBuffer;
		private int currentPosition;
		private bool disposed;

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
				return (long)bytesBuffer.Length;
			}
		}
		public override long Position
		{
			get
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException(null);
				}
				return currentPosition;
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
			this.bytesBuffer = buffer;
		}

		public override void Close()
		{
			this.disposed = true;
			base.Close();
		}

		protected override void Dispose(bool disposing)
		{
			this.disposed = true;
			base.Dispose(disposing);
		}

		public override void Flush()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (this.disposed)
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
				this.currentPosition = (int)offset;
				break;
			case SeekOrigin.Current:
				if ((long)this.currentPosition + offset < 0L)
				{
					throw new IOException("Seek before begin.");
				}
				this.currentPosition += (int)offset;
				break;
			case SeekOrigin.End:
				if ((long)this.bytesBuffer.Length + offset < 0L)
				{
					throw new IOException("Seek before begin.");
				}
				this.currentPosition = this.bytesBuffer.Length + (int)offset;
				break;
			default:
				throw new ArgumentException("Invalid seek origin.");
			}
			return (long)this.currentPosition;
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
			if (this.disposed)
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
			int num = this.bytesBuffer.Length - this.currentPosition;
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
					buffer[offset + num2] = this.bytesBuffer[this.currentPosition + num2];
				}
			}
			else
			{
				Buffer.BlockCopy(this.bytesBuffer, this.currentPosition, buffer, offset, num);
			}
			this.currentPosition += num;
			return num;
		}

		public override int ReadUnmanaged(IntPtr buffer, int count)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			int num = this.bytesBuffer.Length - this.currentPosition;
			if (num > count)
			{
				num = count;
			}
			if (num <= 0)
			{
				return 0;
			}
			Marshal.Copy(this.bytesBuffer, this.currentPosition, buffer, num);
			this.currentPosition += num;
			return num;
		}

		public override int ReadByte()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			if (this.currentPosition >= this.bytesBuffer.Length)
			{
				return -1;
			}
			return (int)this.bytesBuffer[this.currentPosition++];
		}
	}
}
