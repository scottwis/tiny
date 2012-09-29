// ExceptionHeader.cs
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

namespace Tiny.Metadata.Layout
{
    unsafe class ExceptionHeader
    {
        readonly TinyExceptionHeader * m_pTinyHeader;
        readonly FatExceptionHeader * m_pFatHeader;

        public ExceptionHeader(FatExceptionHeader* pFatHeader)
        {
            m_pFatHeader = (FatExceptionHeader *)FluentAsserts.CheckNotNull((void *)pFatHeader, "pFatHeader");
        }

        public ExceptionHeader(TinyExceptionHeader* pTinyHeader)
        {
            m_pTinyHeader = (TinyExceptionHeader *)FluentAsserts.CheckNotNull((void *)pTinyHeader, "pTinyHeader");
        }

        public ExceptionHeaderFlags Flags
        {
            get
            {
                if (m_pTinyHeader != null) {
                    return m_pTinyHeader->Flags;
                }
                return m_pFatHeader->Flags;
            }
        }

        public int DataSize
        {
            get
            {
                if (m_pTinyHeader != null) {
                    return m_pTinyHeader->DataSize;
                }
                return m_pFatHeader->DataSize;
            }
        }

        public int NumberOfExceptionClauses
        {
            get
            {
                if (m_pTinyHeader != null) {
                    return m_pTinyHeader->NumberOfExceptionClauses;
                }
                return m_pFatHeader->NumberOfExceptionClauses;
            }
        }

        public int ExceptionClauseSize
        {
            get
            {
                if (m_pTinyHeader != null) {
                    return 12;
                }
                return 24;
            }
        }

        public ExceptionHandler GetExceptionHandler(int index, Module module)
        {
            index.CheckInRange(0, NumberOfExceptionClauses, "index");
            if (m_pTinyHeader != null) {
                return new ExceptionHandler(
                    (TinyExceptionClause *) ((byte *)m_pTinyHeader + 4 + ExceptionClauseSize  * index), 
                    module
                );
            }
            else {
                return new ExceptionHandler(
                    (FatExceptionClause *)((byte *)m_pFatHeader + 4 + ExceptionClauseSize * index ),
                    module
                );
            }
        }
    }
}
