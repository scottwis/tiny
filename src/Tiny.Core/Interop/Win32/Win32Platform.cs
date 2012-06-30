// Win32Platform.cs
//  
// Author:
//       Scott Wisniewski <scott@scottdw2.com>
// 
// Copyright (c) 2012 Scott Wisniewski
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#if WIN32

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Tiny.Interop.Win32
{
    sealed unsafe class Win32Platform : NativePlatform
    {
        static readonly IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1L);
        const uint INVALID_FILE_SIZE = unchecked((uint) -1);

        [DllImport("kernel32", SetLastError = true)]
        public static extern void* MapViewOfFile(
            IntPtr hFileMappingObject,
            FileMapAccess dwDesiredAccess,
            uint dwFileOffsetHigh,
            uint dwFileOffsetLow,
            IntPtr dwNumberOfBytesToMap
        );

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr CreateFileMapping(
            IntPtr hFile,
            IntPtr lpAttributes,
            ProtectionFlags flProtect,
            uint dwMaxiumSizeHigh,
            uint dwMaximumSizeLow,
            [MarshalAs(UnmanagedType.LPTStr)]
            String lpName
        );

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr CreateFile(
            [MarshalAs(UnmanagedType.LPWStr)] String lpFileName,
            FileAccess dwDesiredAccess,
            ShareMode dwShareMode,
            IntPtr lpSecurityAttributes,
            CreationDisposition dwCreationDisposition,
            CreateFileAttributes dwFlagsAndAttributes,
            IntPtr hTemplateFile
        );

        [DllImport("kernel32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnmapViewOfFile(void* lpBaseAddress);

        [DllImport("kernel32", SetLastError = true)]
        public static extern uint GetFileSize(IntPtr hFile, out uint lpFileSizeHigh);

        [DllImport("msvcr110.dll", EntryPoint = "strnlen", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.SysUInt)]
        public static extern UIntPtr StrNLen(
            byte * pData, 
            UIntPtr maxCount
        );

        public override IUnsafeMemoryMap MemoryMapFile(string fileName)
        {
            fileName.AssumeNotNull();
            IntPtr hFile = IntPtr.Zero;
            IntPtr hMapping = IntPtr.Zero;
            void* pData = null;

            try {
                hFile = CreateFile(
                    fileName,
                    FileAccess.GENERIC_READ,
                    ShareMode.FILE_SHARE_READ,
                    IntPtr.Zero,
                    CreationDisposition.OPEN_EXISTING,
                    CreateFileAttributes.FILE_ATTRIBUTE_NORMAL,
                    IntPtr.Zero
                );  

                if (hFile == INVALID_HANDLE_VALUE) {
                    var inner = new Win32Exception(Marshal.GetLastWin32Error());
                    throw new FileLoadException("Unable to load file.", fileName, inner);
                }

                uint fileSizeHigh;
                uint fileSizeLow = GetFileSize(hFile, out fileSizeHigh);

                if (fileSizeHigh != 0) {
                    throw new FileLoadException("File too large.", fileName);
                }

                if (fileSizeLow == INVALID_FILE_SIZE) {
                    var inner = new Win32Exception(Marshal.GetLastWin32Error());
                    throw new FileLoadException("Unable to determine file size.", fileName, inner);
                } 

                hMapping = CreateFileMapping(hFile, IntPtr.Zero, ProtectionFlags.PAGE_READONLY, 0, 0, null);

                if (hMapping == IntPtr.Zero) {
                    var inner = new Win32Exception(Marshal.GetLastWin32Error());
                    throw new FileLoadException("Unable to memory map file.", fileName, inner);
                }

                pData = MapViewOfFile(hMapping, FileMapAccess.FILE_MAP_READ, 0, 0, IntPtr.Zero);

                if (pData == null) {
                    var inner = new Win32Exception(Marshal.GetLastWin32Error());
                    throw new FileLoadException("Unable to memory map file", fileName, inner);
                }

                return new UnsafeWin32MemoryMap(hFile, hMapping, pData, fileSizeLow);
            }
            catch {
                if (pData != null) {
                    UnmapViewOfFile(pData);
                }
                if (hMapping != IntPtr.Zero) {
                    CloseHandle(hMapping);
                }
                if (hFile != IntPtr.Zero) {
                    CloseHandle(hFile);
                }
                throw;
            }
        }

        public override int StrLen(byte* pData, int maxLength)
        {
            maxLength.CheckGTE(0, "maxLength");
            return checked((int)StrNLen(pData, (UIntPtr) maxLength));
        }
    }
}
#endif