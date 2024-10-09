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

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Motphys.Native;
using UnityEngine;
using UnityEngine.Assertions;

namespace Motphys.Rigidbody
{
    /// <summary>
    /// A pair of positions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PositionPair
    {
        /// <value>
        /// The position of the bodyA in world space.
        /// </value>
        public Vector3 positionA;

        /// <value>
        /// The position of the bodyB in world space.
        /// </value>
        public Vector3 positionB;
    }
}

namespace Motphys.Rigidbody.Internal
{
    internal static class Constants
    {
        public const string DLL = Motphys.Rigidbody.Native.Constants.DLL;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct BodyPair
    {
        public RigidbodyId body1;
        public RigidbodyId body2;
    }

    /// <summary>
    /// Composited of a RigidbodyId and a Isometry transform
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct BodyTransform
    {
        public BodyTransform(RigidbodyId id, Isometry transform)
        {
            rigidbodyId = id;
            this.transform = transform;
        }

        public RigidbodyId rigidbodyId
        {
            get;
        }

        public Isometry transform
        {
            get;
        }

        static BodyTransform()
        {
            Assert.AreEqual(Marshal.SizeOf<BodyTransform>(), 40);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct BodyTransforms
    {

        private SliceRef<BodyTransform> _transforms;

        public BodyTransforms(BodyTransform[] bodyTransforms, int len)
        {
            _transforms = new SliceRef<BodyTransform>(bodyTransforms, len);
        }

        public ulong count
        {
            get { return _transforms.len; }
        }

        public System.IntPtr rawPtr
        {
            get { return _transforms.rawPtr; }
        }

        public System.Span<BodyTransform> ToSpan()
        {
            return this;
        }

        public static implicit operator System.Span<BodyTransform>(BodyTransforms transforms)
        {
            return transforms._transforms.ToSpan();
        }
    }

    [StructLayout(LayoutKind.Sequential)]

    internal struct UpdateBodyShape
    {
        public ColliderId colliderId;
        public Shape shape;
        public Vector3 shapeTranslation;
    }

    internal readonly ref struct UpdateBodyShapes
    {
        private readonly SliceRef<UpdateBodyShape> _shapes;

        public UpdateBodyShapes(UpdateBodyShape[] shapes, int len)
        {
            _shapes = new SliceRef<UpdateBodyShape>(shapes, len);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct TripleUint
    {
        internal USize _a;
        internal USize _b;
        internal USize _c;

        public TripleUint(USize a, USize b, USize c)
        {
            this._a = a;
            this._b = b;
            this._c = c;
        }
    }

    internal struct NativeSlotId
    {
        public static readonly long Invalid = (1 << 32) | uint.MaxValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(long value)
        {
            return value != Invalid;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetVersion(long value)
        {
            return (uint)(value >> 32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetIndex(long value)
        {
            return (uint)value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]

    internal struct MinMax
    {
        private float _min;

        private float _max;

        public MinMax(float min, float max)
        {
            Debug.Assert(max >= min);
            _min = min;
            _max = max;
        }

        /// <value>
        /// The min value.
        ///
        /// Note: if the setted value is larger than max, it will be automatically clamped.
        /// </value>
        public float min
        {
            get
            {
                return _min;
            }
            set
            {
                _min = Mathf.Min(value, _max);
            }
        }

        /// <value>
        /// The max value.
        ///
        /// Note: if the setted value is lesser than min, it will be automatically clamped.
        /// </value>
        public float max
        {
            get
            {
                return _max;
            }
            set
            {
                _max = Mathf.Max(value, _min);
            }
        }
    }
}
