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
using Motphys.Native;
using Motphys.Rigidbody.Api;
using Motphys.Rigidbody.Internal;
using Unity.Collections;
using UnityEngine;

namespace Motphys.Rigidbody
{
    internal class ConvexMesh
    {
        private UnityEngine.Mesh _mesh;
        internal UnityEngine.Mesh Mesh => _mesh;
        internal bool _isValid => _mesh != null;

        private const int MAX_POINT_COUNT = 65536;

        internal string _errorLog;

        internal ConvexMesh(UnityEngine.Mesh rawMesh)
        {
            if (rawMesh == null)
            {
                return;
            }

            var rawVertices = rawMesh.vertices;
            var rawVerticesData = new NativeArray<Vector3>(rawVertices, Allocator.Temp);
            var slice = new Motphys.Native.SliceRef<Vector3>(rawVerticesData);
            var resultCode = ConvexHullUtils.CreateConvexMesh(slice, out var convexMeshPtr);

            switch (resultCode)
            {
                case ResultCode.Ok:
                    var triangleCount = ConvexHullUtils.GetConvexTrianglesCount(convexMeshPtr);
                    var vertexCount = ConvexHullUtils.GetConvexVerticesCount(convexMeshPtr);

                    var triangles = new NativeArray<int>(triangleCount * 3, Allocator.Temp);
                    var vertices = new NativeArray<Vector3>(vertexCount, Allocator.Temp);

                    var trianglesSlice = new Motphys.Native.SliceRef<int>(triangles);
                    var verticesSlice = new Motphys.Native.SliceRef<Vector3>(vertices);

                    var real_index = ConvexHullUtils.GetConvexTriangles(convexMeshPtr, trianglesSlice);
                    var real_vertice = ConvexHullUtils.GetConvexVertices(convexMeshPtr, verticesSlice);

                    if (real_index == triangleCount * 3 && real_vertice == vertexCount)
                    {
                        var convexMesh = new Mesh();
                        convexMesh.vertices = verticesSlice.ToSpan().ToArray();
                        convexMesh.triangles = trianglesSlice.ToSpan().ToArray();

                        if (convexMesh.vertices.Length > 0 && convexMesh.triangles.Length > 0)
                        {
                            convexMesh.RecalculateBounds();
                            convexMesh.RecalculateNormals();
                            _mesh = convexMesh;
                        }
                    }

                    ConvexHullUtils.DestroyConvexMesh(convexMeshPtr);
                    break;
                case ResultCode.ConvexCreatePointCountLessThanFour:
                    _errorLog = $"Failed to create a convex mesh from the source mesh '{rawMesh.name}', because the number of vertices from source mesh is less than 4.";
                    Debug.LogError(_errorLog);
                    break;
                case ResultCode.ConvexCreateVolumeTooSmall:
                    _errorLog = $"Failed to create a convex mesh from the source mesh '{rawMesh.name}', because the volume of source mesh is too small.";
                    Debug.LogError(_errorLog);
                    break;
                case ResultCode.ConvexCreateDegenerate:
                    _errorLog = $"Failed to create a convex mesh from the source mesh '{rawMesh.name}', because the number of triangles is less than 4, or the triangles are too small and too close together.";
                    Debug.LogError(_errorLog);
                    break;
                case ResultCode.ConvexCreatePointCountOverflow:
                    _errorLog = $"Failed to create a convex mesh from the source mesh '{rawMesh.name}', because the number of vertices from source mesh is greater than {MAX_POINT_COUNT}.";
                    Debug.LogError(_errorLog);
                    break;
                case ResultCode.ConvexCreateFaceCountLimitedHit:
                    _errorLog = $"Failed to create a convex mesh from the source mesh '{rawMesh.name}', because the number of generated polygons is greater than limit (255). Consider simplifying your mesh";
                    Debug.LogError(_errorLog);
                    break;
            }
        }
    }

    internal static class ConvexHullUtils
    {
        internal static ResultCode CreateConvexMesh(Motphys.Native.SliceRef<Vector3> vertices, out IntPtr value)
        {
            return PhysicsNativeApi.mprCreateConvexMesh(vertices, out value);
        }

        internal static void DestroyConvexMesh(IntPtr convexMesh)
        {
            PhysicsNativeApi.mprDestroyConvexMesh(convexMesh);
        }

        internal static int GetConvexTrianglesCount(IntPtr convexMesh)
        {
            return PhysicsNativeApi.mprGetConvexTrianglesCount(convexMesh);
        }

        internal static int GetConvexVerticesCount(IntPtr convexMesh)
        {
            return PhysicsNativeApi.mprGetConvexVerticesCount(convexMesh);
        }

        internal static int GetConvexTriangles(IntPtr convexMesh, SliceRef<int> triangles)
        {
            return PhysicsNativeApi.mprGetConvexTriangles(convexMesh, triangles);
        }

        internal static int GetConvexVertices(IntPtr convexMesh, SliceRef<Vector3> vertices)
        {
            return PhysicsNativeApi.mprGetConvexVertices(convexMesh, vertices);
        }
    }
}
