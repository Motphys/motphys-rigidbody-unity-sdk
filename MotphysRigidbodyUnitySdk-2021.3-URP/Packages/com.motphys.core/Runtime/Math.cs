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
using UnityEngine;

namespace Motphys
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Isometry
    {
        public Quaternion rotation;
        public Vector3 position;
        private float _padding;

        public Isometry(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
            _padding = 0;
        }

        public static readonly Isometry Identity = new Isometry()
        {
            rotation = Quaternion.identity,
            position = Vector3.zero,
        };
    }

    /// <summary>
    /// A 3D axis-aligned bounding box (AABB) defined by its minimum and maximum position in each dimension.
    /// This structure represents a box that is aligned with the coordinate axes, and is defined by its 
    /// minimum (bottom-left) and maximum (top-right) corners in 3D space.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Aabb3
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Aabb3"/> struct with the specified minimum and maximum positions.
        /// </summary>
        /// <param name="min">The minimum position of the bounding box in 3D space.</param>
        /// <param name="max">The maximum position of the bounding box in 3D space.</param>
        /// <exception cref="ArgumentException">Thrown if any component of <paramref name="max"/> is less than the corresponding component of <paramref name="min"/>.</exception>
        public Aabb3(Vector3 min, Vector3 max)
        {
            if (max.x < min.x || max.y < min.y || max.z < min.z)
            {
                throw new ArgumentException("The maximum position must not be smaller than the minimum position.");
            }

            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// The minimum position of the bounding box.
        /// </summary>
        public readonly Vector3 min;

        /// <summary>
        /// The maximum position of the bounding box.
        /// </summary>
        public readonly Vector3 max;

        /// <summary>
        /// The center position of the bounding box.
        /// This is calculated as the midpoint between the minimum and maximum positions.
        /// </summary>
        public Vector3 center => (min + max) * 0.5f;

        /// <summary>
        /// The size of the bounding box.
        /// This is calculated as the difference between the maximum and minimum positions.
        /// </summary>
        public Vector3 size => max - min;

        /// <summary>
        /// The half size (extents) of the bounding box.
        /// This is calculated as half the size of the bounding box.
        /// </summary>
        public Vector3 extents => size * 0.5f;

        public override int GetHashCode()
        {
            return center.GetHashCode() ^ (extents.GetHashCode() << 2);
        }

        public override bool Equals(object other)
        {
            if (!(other is Aabb3))
            {
                return false;
            }

            return Equals((Aabb3)other);
        }

        public bool Equals(Aabb3 other)
        {
            return min.Equals(other.min) && max.Equals(other.max);
        }

        public static bool operator ==(Aabb3 lhs, Aabb3 rhs)
        {
            return lhs.center == rhs.center && lhs.extents == rhs.extents;
        }

        public static bool operator !=(Aabb3 lhs, Aabb3 rhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator Bounds(Aabb3 aabb)
        {
            var min = aabb.min;
            var max = aabb.max;
            return new Bounds((min + max) * 0.5f, max - min);
        }

        /// <summary>
        /// A static read-only instance of <see cref="Aabb3"/> representing an empty bounding box.
        /// The minimum and maximum points are both set to the origin (0,0,0), resulting in a size of zero and a center at the origin.
        /// </summary>
        public static readonly Aabb3 Zero = default;
    }
}
