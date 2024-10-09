// Confidential Information of Motphys. Not for disclosure or distribution without Motphys's prior
// written consent.
//
// This software contains code, techniques and know-how which is confidential and proprietary to
// Motphys.
//
// Product and Trade Secret source code contains trade secrets of Motphys.
//
// Copyright (C) 2020-2024 Motphys Technology Co., Ltd. All Rights Reserved.
//
// This software belongs to the Intellectual Property of Motphys. Use of this software is subject to
// the terms and conditions in the license file accompanying. You may not use this software except
// in compliance with the license file.

using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
namespace Motphys.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal ref struct SliceRef<T> where T : unmanaged
    {
        private System.IntPtr _ptr;
        private System.UIntPtr _len;

        public SliceRef(T[] array, int len)
        {
            len = System.Math.Min(array.Length, len);
            _len = (System.UIntPtr)len;
            if (array.Length > 0)
            {
                unsafe
                {
                    fixed (T* ptr = &array[0])
                    {
                        _ptr = (System.IntPtr)ptr;
                    }
                }
            }
            else
            {
                _ptr = System.IntPtr.Zero;
            }
        }

        public SliceRef(NativeArray<T> array)
        {
            _len = (System.UIntPtr)array.Length;
            unsafe
            {
                _ptr = (System.IntPtr)array.GetUnsafeReadOnlyPtr();
            }
        }

        public SliceRef(NativeSlice<T> array)
        {
            _len = (System.UIntPtr)array.Length;

            unsafe
            {
                _ptr = (System.IntPtr)array.GetUnsafeReadOnlyPtr();
            }
        }

        public System.IntPtr rawPtr
        {
            get { return _ptr; }
        }

        public ulong len
        {
            get { return _len.ToUInt64(); }
        }

        public System.Span<T> ToSpan()
        {
            return this;
        }

        public static unsafe implicit operator System.Span<T>(SliceRef<T> slice)
        {
            return new System.Span<T>((void*)slice._ptr, (int)slice.len);
        }
    }
}
