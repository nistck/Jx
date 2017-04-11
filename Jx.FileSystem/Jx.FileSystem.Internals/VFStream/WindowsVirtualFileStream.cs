using System;
using Jx.FileSystem; 
using System.IO;
using System.Runtime.InteropServices;

namespace Jx.FileSystem.Internals.VFStream
{
    internal sealed class WindowsVirtualFileStream : VirtualFileStream
    {
        private const int al = -2147483648;
        private const int aM = 2;
        private const int am = 3;
        private IntPtr fileHandle;
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
                if (this.fileHandle == IntPtr.Zero)
                {
                    throw new ObjectDisposedException(null);
                }
                int num = 0;
                int num2 = GetFileSize(this.fileHandle, out num);
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
                if (this.fileHandle == IntPtr.Zero)
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
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, [In] ref System.Threading.NativeOverlapped lpOverlapped);
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

        public WindowsVirtualFileStream(string realPath)
        {
            this.fileHandle = CreateFile(realPath, -2147483648, FileShare.Read, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            if (!(this.fileHandle == (IntPtr)(-1)))
            {
                return;
            }
            this.fileHandle = IntPtr.Zero;
            int lastWin32Error = Marshal.GetLastWin32Error();
            if (lastWin32Error == 2 || lastWin32Error == 3)
            {
                throw new FileNotFoundException("File not found.", realPath);
            }
            throw new IOException(string.Format("Opening of a file failed \"{0}\".", realPath));
        }
        public override void Close()
        {
            if (this.fileHandle != IntPtr.Zero)
            {
                WindowsVirtualFileStream.CloseHandle(this.fileHandle);
                this.fileHandle = IntPtr.Zero;
            }
            base.Close();
        }
        protected override void Dispose(bool disposing)
        {
            if (this.fileHandle != IntPtr.Zero)
            {
                WindowsVirtualFileStream.CloseHandle(this.fileHandle);
                this.fileHandle = IntPtr.Zero;
            }
            base.Dispose(disposing);
        }
        public override void Flush()
        {
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (this.fileHandle == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }
            int num;
            this.an = (int)WindowsVirtualFileStream.A(this.fileHandle, offset, origin, out num);
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
            if (this.fileHandle == IntPtr.Zero)
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
                IntPtr p = (IntPtr)(ptr + offset);
                result = this.ReadUnmanaged(p, count);
            }
            return result;
        }

        public override int ReadUnmanaged(IntPtr buffer, int count)
        {
            if (this.fileHandle == IntPtr.Zero)
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
            bool flag = ReadFile(this.fileHandle, buffer, count, out num, IntPtr.Zero) == 0;
            if (flag)
            {
                throw new IOException("Reading file failed.");
            }
            this.an += num;
            return num;
        }
        public unsafe override int ReadByte()
        {
            if (this.fileHandle == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }
            byte result;
            int num;
            bool flag = ReadFile(this.fileHandle, (IntPtr)((void*)(&result)), 1, out num, IntPtr.Zero) == 0;
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
