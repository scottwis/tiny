// PosixPlatform.cs
//  
// Author:
//     Scott Wisniewski <scott@scottdw2.com>
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

#if posix

using Mono.Unix.Native;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Tiny.Interop.Posix
{
    sealed unsafe class PosixPlatform : NativePlatform
    {
        [DllImport("libc", EntryPoint="strnlen", CallingConvention=CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.SysUInt)]
        public static extern UIntPtr StrNLen(byte * pData, UIntPtr maxCount);
        
        public override IUnsafeMemoryMap MemoryMapFile(string fileName)
        {
            int file = Syscall.open(fileName, OpenFlags.O_RDONLY);
            Stat fileStat = new Stat();
            void * pMem = null;
            try{
                if (file == -1) {
                    throw new FileLoadException("Unable to load file",fileName, new PosixException(Stdlib.GetLastError()));
                }

                int res = Syscall.fstat(file, out fileStat);

                if (res == -1) {
                    Syscall.close(file);
                    throw new FileLoadException("Unable to load file",fileName, new PosixException(Stdlib.GetLastError()));
                }

                pMem = (void *)Syscall.mmap(
                    IntPtr.Zero,
                    (ulong)fileStat.st_size,
                    MmapProts.PROT_READ,
                    MmapFlags.MAP_PRIVATE,
                    file,
                    0
                );
                
                if (pMem==null) {
                    throw new FileLoadException(
                        "Unable to load file",
                        fileName,
                        new PosixException(Stdlib.GetLastError())
                    );
                }

                return new PosixMemoryMap(pMem, checked((uint)fileStat.st_size));
            }
            catch {
                if (file != -1) {
                    Syscall.close(file);
                }
                if (pMem != null) {
                    Syscall.munmap((IntPtr)pMem,checked((ulong)fileStat.st_size));
                }
                throw;
            }
        }

        public override int StrLen(byte* pName, int maxLength)
        {
            maxLength.CheckGTE(0, "maxLength");
            return checked((int)StrNLen(pName, (UIntPtr)maxLength));
        }
    }
}

#endif