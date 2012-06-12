// 
// DllCharacteristics.cs
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

namespace Tiny.Metadata.Layout
{
    //# A bit flag describing the known flags in the [OptionalHeader.DllCharacteristics][DllCharacteristics] field of the
    //[OptionalHeader][optional PE Header].
    enum DllCharacteristics : ushort
    {
        //# The set of all bits that cannot be set in a managed executable supported by the tiny decompiler.
        RESERVED_BITS = 0x100f,
        //# Indicates that a dll can be relocated at load-time.
        IMAGE_DLL_CHARACTERISTICS_DYNAMIC_BASE=0x0040,
        //# The PE/COFF spec simply says:
        //# > Code Integrity checks are enforced.
        //# I'm not sure what that means.
        IMAGE_DLL_CHARACTERISTICS_FORCE_INTEGRITY=0x0080,
        //# The program is compatible with data execution prevention (DEP).
        IMAGE_DLL_CHARACTERISTICS_NX_COMPAT=0x0100,
        //# Indicates that an image should not be isolated by the windows loader.
        //# This has something to do with Windows Side-by-Side assemblies, but
        //# I'm not sure about the details.
        IMAGE_DLLCHARACTERISTICS_NO_ISOLATION=0x0200,
        //# Indicates that the image does not use structured exception (SE) handling. No SE handler may be called in
        //# an image marked with this flag..
        IMAGE_DLLCHARACTERISTICS_NO_SEH=0x0400,
        //# I'm assuming this bit instructs the loader to not attempt to bind a dll's imported symbols to function
        //# addresses, but was unable to definitively confirm that. In any case, tdc ignores this bit.
        IMAGE_DLLCHARACTERISTICS_NO_BIND=0x0800,
        //# The image is a driver (and uses WDM). The tiny decompiler will conservatively reject any image
        //# that has this bit set to 1.
        IMAGE_DLLCHARACTERISTICS_WDM_DRIVER=0x2000,
        //# Indicates that an image is terminal server aware, and that it does not need to be run under the
        //# "Remote Desktop Services Application Compatability layer". This flag is ignored by tdc.
        IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE=0x8000
    }
}

