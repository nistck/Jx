using Jx.FileSystem;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace A
{
	internal sealed class f : VirtualFileStream
	{
		private IntPtr aH;
		private int ah;
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
				if (this.aH == IntPtr.Zero)
				{
					throw new ObjectDisposedException(null);
				}
				return (long)f.VirtualFileStream_Length(this.aH);
			}
		}
		public override long Position
		{
			get
			{
				if (this.aH == IntPtr.Zero)
				{
					throw new ObjectDisposedException(null);
				}
				return (long)this.ah;
			}
			set
			{
				this.Seek(value, SeekOrigin.Begin);
			}
		}
		[DllImport("MacAppNativeWrapper", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "MacAppNativeWrapper_VirtualFileStream_Open")]
		public static extern IntPtr VirtualFileStream_Open(string realPath);
		[DllImport("MacAppNativeWrapper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MacAppNativeWrapper_VirtualFileStream_Close")]
		public static extern void VirtualFileStream_Close(IntPtr handle);
		[DllImport("MacAppNativeWrapper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MacAppNativeWrapper_VirtualFileStream_Length")]
		public static extern int VirtualFileStream_Length(IntPtr handle);
		[DllImport("MacAppNativeWrapper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MacAppNativeWrapper_VirtualFileStream_Read")]
		public static extern int VirtualFileStream_Read(IntPtr handle, IntPtr buffer, int count);
		[DllImport("MacAppNativeWrapper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MacAppNativeWrapper_VirtualFileStream_Seek")]
		public static extern int VirtualFileStream_Seek(IntPtr handle, int offset, SeekOrigin origin);
		public f(string realPath)
		{
			this.aH = f.VirtualFileStream_Open(realPath);
			if (!(this.aH == IntPtr.Zero))
			{
				return;
			}
			if (!File.Exists(realPath))
			{
				throw new FileNotFoundException("File not found.", realPath);
			}
			throw new IOException(string.Format("Opening of a file failed \"{0}\".", realPath));
		}
		public override void Close()
		{
			if (this.aH != IntPtr.Zero)
			{
				f.VirtualFileStream_Close(this.aH);
				this.aH = IntPtr.Zero;
			}
			base.Close();
		}
		protected override void Dispose(bool disposing)
		{
			if (this.aH != IntPtr.Zero)
			{
				f.VirtualFileStream_Close(this.aH);
				this.aH = IntPtr.Zero;
			}
			base.Dispose(disposing);
		}
		public override void Flush()
		{
		}
		public override long Seek(long offset, SeekOrigin origin)
		{
			if (this.aH == IntPtr.Zero)
			{
				throw new ObjectDisposedException(null);
			}
			int num = f.VirtualFileStream_Seek(this.aH, (int)offset, origin);
			if (num != 0)
			{
				throw new IOException("Seeking file length failed.");
			}
			switch (origin)
			{
			case SeekOrigin.Begin:
				this.ah = (int)offset;
				break;
			case SeekOrigin.Current:
				this.ah += (int)offset;
				break;
			case SeekOrigin.End:
				this.ah = (int)this.Length + (int)offset;
				break;
			}
			return (long)this.ah;
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
			if (this.aH == IntPtr.Zero)
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
				result = this.ReadUnmanaged((IntPtr)((void*)ptr + offset / sizeof(void)), count);
			}
			return result;
		}

		public override int ReadUnmanaged(IntPtr buffer, int count)
		{
			if (this.aH == IntPtr.Zero)
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
			int num = f.VirtualFileStream_Read(this.aH, buffer, count);
			if (num < 0)
			{
				throw new IOException("Reading file failed.");
			}
			this.ah += num;
			return num;
		}
		public unsafe override int ReadByte()
		{
			if (this.aH == IntPtr.Zero)
			{
				throw new ObjectDisposedException(null);
			}
			byte result;
			int num = f.VirtualFileStream_Read(this.aH, (IntPtr)((void*)(&result)), 1);
			if (num < 0)
			{
				throw new IOException("Reading file failed.");
			}
			if (num == 0)
			{
				return -1;
			}
			this.ah++;
			return (int)result;
		}
	}
	[CompilerGenerated]
	internal class F
	{
		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 6)]
        internal struct A
		{
		}
		internal static F.A aC;
		internal static F.A ac;
	}
}
