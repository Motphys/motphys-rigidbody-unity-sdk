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
using UnityEngine;

namespace Motphys.Rigidbody.Internal
{
    internal enum ShapeType
    {
        Cube,
        SolidSphere,
        Capsule,
        Cylinder,
        ConvexHull,
        Mesh,
        InfinitePlane,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Cuboid
    {
        public Vector3 halfExt;

        public Cuboid(Vector3 halfExt)
        {
            this.halfExt = halfExt;
        }

        public static implicit operator Shape(Cuboid builder)
        {
            return new Shape()
            {
                type = ShapeType.Cube,
                variants = new Shape.Variants { _cube = builder }
            };
        }

        public static readonly Cuboid Identity = new Cuboid() { halfExt = Vector3.one * 0.5f };
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Sphere
    {
        public float radius;

        public static implicit operator Shape(Sphere builder)
        {
            return new Shape()
            {
                type = ShapeType.SolidSphere,
                variants = new Shape.Variants { _sphere = builder }
            };
        }

        public static readonly Sphere Identity = new Sphere() { radius = 1.0f };
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Cylinder
    {
        public float halfHeight;
        public float radius;

        public static implicit operator Shape(Cylinder builder)
        {
            return new Shape()
            {
                type = ShapeType.Cylinder,
                variants = new Shape.Variants { _cylinder = builder }
            };
        }

        public static readonly Cylinder Identity = new Cylinder()
        {
            halfHeight = 1.0f,
            radius = 0.5f
        };
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Capsule
    {
        public float halfHeight;
        public float radius;

        public static implicit operator Shape(Capsule builder)
        {
            return new Shape()
            {
                type = ShapeType.Capsule,
                variants = new Shape.Variants { _capsule = builder }
            };
        }

        public static readonly Capsule Identity = new Capsule()
        {
            halfHeight = 0.5f,
            radius = 0.5f
        };
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct InfinitePlane
    {
        private int _padding;

        public static implicit operator Shape(InfinitePlane builder)
        {
            return new Shape()
            {
                type = ShapeType.InfinitePlane,
                variants = new Shape.Variants { _infinitePlane = builder }
            };
        }

        public static readonly InfinitePlane Identity = new InfinitePlane()
        {
            _padding = 0,
        };
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct ShapeId : System.IEquatable<ShapeId>
    {
        private readonly long _value;

        internal ShapeId(long value)
        {
            _value = value;
        }
        internal long value => _value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return Equals((ShapeId)obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ShapeId other)
        {
            return this.value == other.value;
        }

        public bool isValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return NativeSlotId.IsValid(_value); }
        }

        internal uint version
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return NativeSlotId.GetVersion(_value); }
        }

        internal uint index
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return NativeSlotId.GetIndex(_value); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ShapeId id1, ShapeId id2)
        {
            return id1.value == id2.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ShapeId id1, ShapeId id2)
        {
            return id1.value != id2.value;
        }

        public static readonly ShapeId Invalid = new ShapeId(NativeSlotId.Invalid);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ConvexHullBuilder
    {
        public ShapeId id;
        public static implicit operator Shape(ConvexHullBuilder builder)
        {
            return new Shape()
            {
                type = ShapeType.ConvexHull,
                variants = new Shape.Variants { _convexHull = builder }
            };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MeshBuilder
    {
        public ShapeId id;
        public static implicit operator Shape(MeshBuilder builder)
        {
            return new Shape()
            {
                type = ShapeType.Mesh,
                variants = new Shape.Variants { _mesh = builder }
            };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Shape
    {
        public ShapeType type;

        [StructLayout(LayoutKind.Explicit)]
        public struct Variants
        {
            [FieldOffset(0)]
            internal Cuboid _cube;

            [FieldOffset(0)]
            internal Sphere _sphere;

            [FieldOffset(0)]
            internal Cylinder _cylinder;

            [FieldOffset(0)]
            internal Capsule _capsule;

            [FieldOffset(0)]
            internal InfinitePlane _infinitePlane;

            [FieldOffset(0)]
            internal ConvexHullBuilder _convexHull;

            [FieldOffset(0)]
            internal MeshBuilder _mesh;
        }

        public Variants variants;
    }
}
