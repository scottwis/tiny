// 
// ImageSubsystem.cs
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
using System;

namespace Tiny.Decompiler.Metadata
{
    //# An enum describing the known values of the [OptionalHeader.SubSystem] field
    enum ImageSubsystem : ushort
    {
        IMAGE_SUBSYSTEM_UNKNOWN=0,
        //# Device drivers and native Windows processes
        IMAGE_SUBSYSTEM_NATIVE=1,
        //# The Windows graphical user interface (GUI) subsystem
        IMAGE_SUBSYSTEM_WINDOWS_GUI=2,
        //# The Windows character subsystem
        IMAGE_SUBSYSTEM_WINDOWS_CUI=3,
        //# The Posix character subsystem
        IMAGE_SUBSYSTEM_POSIX_CUI=7,
        IMAGE_SUBSYSTEM_WINDOWS_CE_GUI=9,
        //# An Extensible Firmware Interface (EFI) application
        IMAGE_SUBSYSTEM_EFI_APPLICATION=10,
        //# An EFI driver with boot services
        IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER=11,
        //# An EFI driver with run-time services
        IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER=12,
        //# An EFI ROM image
        IMAGE_SUBSYSTEM_EFI_ROM=13,
        IMAGE_SUBSYSTEM_XBOX=14
    }
}

