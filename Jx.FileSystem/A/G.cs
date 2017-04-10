using Jx.FileSystem;
using System;
using System.IO;
using System.Runtime.InteropServices;
namespace A
{
	internal sealed class G : VirtualFileStream
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
		public G(string realPath)
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
	internal sealed class g : VirtualFileStream
	{
		private const int al = -2147483648;
		private const int aM = 2;
		private const int am = 3;
		private IntPtr aN;
		private int an;
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
				if (this.aN == IntPtr.Zero)
				{
					throw new ObjectDisposedException(null);
				}
				int num = 0;
				int num2 = g.GetFileSize(this.aN, out num);
				if (num2 == -1)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error != 0)
					{
						throw new IOException("Getting file length failed.");
					}
				}
				return (long)((ulong)num | (ulong)num2);
			}
		}
		public override long Position
		{
			get
			{
				if (this.aN == IntPtr.Zero)
				{
					throw new ObjectDisposedException(null);
				}
				return (long)this.an;
			}
			set
			{
				this.Seek(value, SeekOrigin.Begin);
			}
		}
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "CreateFile", SetLastError = true)]
		private static extern IntPtr CreateFile(string p1, int p2, FileShare p3, IntPtr p4, FileMode p5, int p6, IntPtr p7);
		[DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true)]
		private static extern bool CloseHandle(IntPtr h);
		[DllImport("kernel32.dll", EntryPoint = "ReadFile", SetLastError = true)]
		private static extern int ReadFile(IntPtr p1, IntPtr p2, int p3, out int p4, IntPtr p5);
		[DllImport("kernel32.dll", EntryPoint = "GetFileSize", SetLastError = true)]
		private static extern int GetFileSize(IntPtr p1, out int p2);
		[DllImport("kernel32.dll", EntryPoint = "SetFilePointer", SetLastError = true)]
		private static extern int SetFilePointer(IntPtr p1, int p2, ref int p3, int p4);

		private static long A(IntPtr intPtr, long num, SeekOrigin seekOrigin, out int ptr)
		{
			ptr = 0;
			int num2 = (int)num;
			int num3 = (int)(num >> 32);
			num2 = SetFilePointer(intPtr, num2, ref num3, (int)seekOrigin);
			if (num2 == -1 && (ptr = Marshal.GetLastWin32Error()) != 0)
			{
				return -1L;
			}
			return (long)((ulong)(num3 | num2));
		}

		public g(string realPath)
		{
			this.aN = CreateFile(realPath, -2147483648, FileShare.Read, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
			if (!(this.aN == (IntPtr)(-1)))
			{
				return;
			}
			this.aN = IntPtr.Zero;
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error == 2 || lastWin32Error == 3)
			{
				throw new FileNotFoundException("File not found.", realPath);
			}
			throw new IOException(string.Format("Opening of a file failed \"{0}\".", realPath));
		}
		public override void Close()
		{
			if (this.aN != IntPtr.Zero)
			{
				g.CloseHandle(this.aN);
				this.aN = IntPtr.Zero;
			}
			base.Close();
		}
		protected override void Dispose(bool disposing)
		{
			if (this.aN != IntPtr.Zero)
			{
				g.CloseHandle(this.aN);
				this.aN = IntPtr.Zero;
			}
			base.Dispose(disposing);
		}
		public override void Flush()
		{
		}
		public override long Seek(long offset, SeekOrigin origin)
		{
			if (this.aN == IntPtr.Zero)
			{
				throw new ObjectDisposedException(null);
			}
			int num;
			this.an = (int)g.A(this.aN, offset, origin, out num);
			if (this.an == -1)
			{
				throw new IOException("Seeking file length failed.");
			}
			return (long)this.an;
		}
		public override void SetLength(long value)
		{
			throw new NotSupportedException("The method is not supported.");
		}
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("The method is not supported.");
		}
		public unsafe override int Read(byte[] buffer, int offset, int count)
		{
			if (this.aN == IntPtr.Zero)
			{
				throw new ObjectDisposedException(null);
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("Invalid offset length.");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			int result;
			fixed (byte* ptr = buffer)
			{
                //result = this.ReadUnmanaged((IntPtr)((void*)ptr + offset / sizeof(void)), count);
                result = this.ReadUnmanaged((IntPtr)((void*)ptr + offset / sizeof(void)), count);
            }
			return result;
		}
		public override int ReadUnmanaged(IntPtr buffer, int count)
		{
			if (this.aN == IntPtr.Zero)
			{
				throw new ObjectDisposedException(null);
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (count == 0)
			{
				return 0;
			}
			int num;
			bool flag = g.ReadFile(this.aN, buffer, count, out num, IntPtr.Zero) == 0;
			if (flag)
			{
				throw new IOException("Reading file failed.");
			}
			this.an += num;
			return num;
		}
		public unsafe override int ReadByte()
		{
			if (this.aN == IntPtr.Zero)
			{
				throw new ObjectDisposedException(null);
			}
			byte result;
			int num;
			bool flag = g.ReadFile(this.aN, (IntPtr)((void*)(&result)), 1, out num, IntPtr.Zero) == 0;
			if (flag)
			{
				throw new IOException("Reading file failed.");
			}
			if (num == 0)
			{
				return -1;
			}
			this.an++;
			return (int)result;
		}
	}
}
