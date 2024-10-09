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

using System;
using System.Runtime.InteropServices;

namespace Motphys.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct USize
    {
        private UIntPtr _inner;

        public ulong ToUInt64()
        {
            return _inner.ToUInt64();
        }

        public uint ToUInt32()
        {
            return _inner.ToUInt32();
        }

        private static UIntPtr MaxUIntPtr()
        {
            if (UIntPtr.Size == 4)
            {
                return (UIntPtr)uint.MaxValue;
            }
            else
            {
                return (UIntPtr)ulong.MaxValue;
            }
        }

        public static explicit operator uint(USize value)
        {
            return value._inner.ToUInt32();
        }

        public static implicit operator USize(uint value)
        {
            return new USize() { _inner = (UIntPtr)value };
        }

        public static implicit operator ulong(USize value)
        {
            return value._inner.ToUInt64();
        }

        public static explicit operator USize(ulong value)
        {
            return new USize() { _inner = (UIntPtr)value };
        }

        public static explicit operator USize(int value)
        {
            return new USize() { _inner = (UIntPtr)value };
        }

        public static readonly USize Max = new USize() { _inner = MaxUIntPtr() };

        public static readonly USize Min = new USize() { _inner = (UIntPtr)0 };
    }
}
