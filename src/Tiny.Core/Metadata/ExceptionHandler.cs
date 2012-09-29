// ExceptionHandler.cs
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
// THE SOFTWARE.`

using System;
using Tiny.Metadata.Layout;

namespace Tiny.Metadata
{
    public unsafe class ExceptionHandler
    {
        readonly TinyExceptionClause * m_pTinyClause;
        readonly FatExceptionClause * m_pFatClause;
        readonly Module m_module;

        internal ExceptionHandler(TinyExceptionClause * pClause, Module module)
        {
            m_pTinyClause = (TinyExceptionClause *)FluentAsserts.CheckNotNull((void*)pClause, "pClause");
            m_module = module.CheckNotNull("module");
        }

        internal ExceptionHandler(FatExceptionClause * pClause, Module module)
        {
            m_pFatClause = (FatExceptionClause*)FluentAsserts.CheckNotNull((void*)pClause, "pClause");
            m_module = module.CheckNotNull("module");
        }

        public HandlerKind Kind
        {
            get
            {
                CheckDisposed();
                if (m_pTinyClause != null) {
                    return (HandlerKind) (m_pTinyClause->Flags);
                }
                return (HandlerKind) (m_pFatClause->Flags);
            }
        }

        public int TryOffset
        {
            get
            {
                CheckDisposed();
                if (m_pTinyClause != null) {
                    return m_pTinyClause->TryOffset;
                }
                return checked((int)m_pFatClause->TryOffset);
            }
        }

        public int TryLength
        {
            get
            {
                CheckDisposed();
                if (m_pTinyClause != null) {
                    return m_pTinyClause->TryLength;
                }
                return checked((int)m_pFatClause->TryLength);
            }
        }

        public int HandlerOffset
        {
            get
            {
                CheckDisposed();
                if (m_pTinyClause != null) {
                    return m_pTinyClause->HandlerOffset;
                }
                return checked((int) m_pFatClause->HandlerOffset);
            }
        }

        public int HandlerLength
        {
            get
            {
                CheckDisposed();
                if (m_pTinyClause!= null) {
                    return m_pTinyClause->HandlerLength;
                }
                return checked((int) m_pFatClause->HandlerLength);
            }
        }

        public Type ExceptionType
        {
            get
            {
                CheckDisposed();
                IToken token = m_pTinyClause!= null ? m_pTinyClause->ClassToken : m_pFatClause->ClassToken;
                return token.IsNull ? null : new TypeReference(token, m_module);
            }
        }

        public int? FilterOffset
        {
            get
            {
                if (Kind == HandlerKind.Filter) {
                    if (m_pTinyClause != null) {
                        return checked((int)m_pTinyClause->FilterOffset);
                    }
                    return checked((int) m_pFatClause->FilterOffset);
                }
                return null;
            }
        }

        private void CheckDisposed()
        {
            if (m_module.PEFile.IsDisposed) {
                throw new ObjectDisposedException("ExceptionHandler");
            }
        }
    }
}
