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

using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TestTools;

namespace Motphys.DebugDraw.Core
{
    /// <summary>
    /// Debug Renderer Const Parameters
    /// </summary>
    // Constant class, no coverage
    [ExcludeFromCoverage]
    internal static class Constants
    {
        internal const int BaseBufferCount = 1000;
        /// <summary>
        /// The max count of one batch of instance draw call
        /// </summary>
        internal const int MaxInstanceShapeCount = 1023;
        /// <summary>
        /// The maximum count of one compute buffer.
        /// </summary>
        internal const int MaxComputeBufferCount = 16777216; //2 ^ 24;
    }

    /// <summary>
    /// Data structure of runtime rendering axis in cartesian coordinates
    /// </summary>
    public struct Axis
    {
        public float3 center;
        public float3 forward;
        public float3 up;
        public float3 right;
    }

    /// <summary>
    /// Data structure of runtime rendering line
    /// </summary>
    public struct Line
    {
        public float3 begin;
        public float3 end;
        public float3 color;
    }

    /// <summary>
    /// Drawing capsule data
    /// </summary>
    public struct Capsule
    {
        public Matrix4x4 matrix;
        public float radius;
        public float height; // includes hemisphere
        public Color color;
    }

    /// <summary>
    /// Drawing shape data structure
    /// </summary>
    public struct Shape
    {
        public ShapeType shapeType;
        public float4x4 matrix;
        public UnityEngine.Color color;
    }

    /// <summary>
    /// Drawing shape type
    /// </summary>
    public enum ShapeType
    {
        Cube,
        Sphere,
        Capsule,
        Cylinder
    }

    /// <summary>
    /// Drawing type
    /// </summary>
    public enum DrawType
    {
        Wireframe,
        Mesh,
        Both
    }
}
