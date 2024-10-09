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

using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Motphys.DebugDraw.Core
{
    internal partial class DebugDrawer
    {

        private static Matrix4x4[] s_instanceMatrix;

        private static Mesh s_webAabbMesh;
        private static Mesh s_lineMesh;
        private static Mesh s_pointMesh;

        private static Material s_webAabbMaterial;
        private static Material s_webPointMaterial;
        private static Material s_webLineMaterial;

        private static MaterialPropertyBlock s_lineProperty;
        private static Vector4[] s_linePrameters;
        private static Vector4[] s_colors;

        private static void DrawMeshAabbs(CommandBuffer cmd, NativeArray<Motphys.Aabb3> aabbs, Color color)
        {
            if (cmd == null || !s_initialized || s_webAabbMesh == null || s_webAabbMaterial == null)
            {
                return;
            }

            var count = aabbs.Length;

            if (count <= 0)
            {
                return;
            }

            var batches = count / Constants.MaxInstanceShapeCount + 1;

            s_webAabbMaterial.SetColor(s_colorId, color);

            for (int batch = 0; batch < batches; batch++)
            {
                var instanceCount = 0;
                for (int i = 0; i < Constants.MaxInstanceShapeCount; i++)
                {
                    var index = batch * Constants.MaxInstanceShapeCount + i;

                    if (index < aabbs.Length)
                    {
                        var aabb3 = aabbs[index];
                        var scale = aabb3.size;
                        var cetner = aabb3.center;

                        ref var trans = ref s_instanceMatrix[i];

                        trans.m00 = scale.x;
                        trans.m11 = scale.y;
                        trans.m22 = scale.z;
                        trans.m03 = cetner.x;
                        trans.m13 = cetner.y;
                        trans.m23 = cetner.z;
                        trans.m33 = 1;

                        instanceCount++;
                    }
                    else
                    {
                        break;
                    }
                }

                cmd.DrawMeshInstanced(s_webAabbMesh, 0, s_webAabbMaterial, 0, s_instanceMatrix, instanceCount);
            }
        }

        private static void DrawMeshLines(CommandBuffer cmd, NativeArray<Line> lines)
        {
            if (cmd == null || !s_initialized)
            {
                return;
            }

            var count = lines.Length;

            if (count <= 0)
            {
                return;
            }

            var batches = count / Constants.MaxInstanceShapeCount + 1;
            for (int batch = 0; batch < batches; batch++)
            {
                var instanceCount = 0;
                for (int i = 0; i < Constants.MaxInstanceShapeCount; i++)
                {
                    var index = batch * Constants.MaxInstanceShapeCount + i;

                    if (index < lines.Length)
                    {
                        var line = lines[index];
                        var begin = line.begin;
                        var end = line.end;

                        ref var trans = ref s_instanceMatrix[i];

                        trans.m00 = 1;
                        trans.m11 = 1;
                        trans.m22 = 1;
                        trans.m33 = 1;

                        trans.m03 = begin.x;
                        trans.m13 = begin.y;
                        trans.m23 = begin.z;

                        ref var endPoint = ref s_linePrameters[i];
                        endPoint.x = end.x;
                        endPoint.y = end.y;
                        endPoint.z = end.z;

                        ref var color = ref s_colors[i];
                        color.x = line.color.x;
                        color.y = line.color.y;
                        color.z = line.color.z;
                        instanceCount++;
                    }
                    else
                    {
                        break;
                    }
                }

                s_lineProperty.SetVectorArray("_End", s_linePrameters);
                s_lineProperty.SetVectorArray("_Color", s_colors);
                cmd.DrawMeshInstanced(s_lineMesh, 0, s_webLineMaterial, 0, s_instanceMatrix, instanceCount, s_lineProperty);
            }
        }

        private static void DrawMeshPoints(CommandBuffer cmd, NativeArray<float3> positions, Color color, float pointSize)
        {
            if (cmd == null || !s_initialized)
            {
                return;
            }

            var count = positions.Length;

            if (count <= 0)
            {
                return;
            }

            var batches = count / Constants.MaxInstanceShapeCount + 1;
            for (int batch = 0; batch < batches; batch++)
            {
                var instanceCount = 0;
                for (int i = 0; i < Constants.MaxInstanceShapeCount; i++)
                {
                    var index = batch * Constants.MaxInstanceShapeCount + i;

                    if (index < positions.Length)
                    {
                        var point = positions[index];

                        ref var trans = ref s_instanceMatrix[i];

                        trans.m00 = pointSize;
                        trans.m11 = pointSize;
                        trans.m22 = pointSize;

                        trans.m03 = point.x;
                        trans.m13 = point.y;
                        trans.m23 = point.z;
                        trans.m33 = 1;

                        instanceCount++;
                    }
                    else
                    {
                        break;
                    }
                }

                s_webPointMaterial.SetColor(s_colorId, color);
                cmd.DrawMeshInstanced(s_pointMesh, 0, s_webPointMaterial, 0, s_instanceMatrix, instanceCount);
            }
        }

        private static void InitialMeshResources(GizmosSettings settings)
        {
            InitialMesh();

            s_webAabbMaterial = new Material(settings._aabbShaderFallback);
            if (s_webAabbMaterial != null)
            {
                s_webAabbMaterial.enableInstancing = true;
            }

            s_webLineMaterial = new Material(settings._lineShaderFallback);
            if (s_webLineMaterial != null)
            {
                s_webLineMaterial.enableInstancing = true;
            }

            s_webPointMaterial = new Material(settings._pointShaderFallback);
            if (s_webPointMaterial != null)
            {
                s_webPointMaterial.enableInstancing = true;
            }

            s_instanceMatrix = new Matrix4x4[Constants.MaxInstanceShapeCount];
            s_linePrameters = new Vector4[Constants.MaxInstanceShapeCount];
            s_colors = new Vector4[Constants.MaxInstanceShapeCount];

            s_lineProperty = new MaterialPropertyBlock();
        }

        private static void InitialMesh()
        {
            s_webAabbMesh = new Mesh();
            var vertices = new List<Vector3>();

            //bottom plane
            vertices.Add(new Vector3(-0.5f, -0.5f, -0.5f));
            vertices.Add(new Vector3(0.5f, -0.5f, -0.5f));
            vertices.Add(new Vector3(0.5f, -0.5f, 0.5f));
            vertices.Add(new Vector3(-0.5f, -0.5f, 0.5f));

            //top plane
            vertices.Add(new Vector3(-0.5f, 0.5f, -0.5f));
            vertices.Add(new Vector3(0.5f, 0.5f, -0.5f));
            vertices.Add(new Vector3(0.5f, 0.5f, 0.5f));
            vertices.Add(new Vector3(-0.5f, 0.5f, 0.5f));

            var indices = new int[24];
            //bottom line
            indices[0] = 0;
            indices[1] = 1;

            indices[2] = 1;
            indices[3] = 2;

            indices[4] = 2;
            indices[5] = 3;

            indices[6] = 3;
            indices[7] = 0;

            //top line
            indices[8] = 4;
            indices[9] = 5;

            indices[10] = 5;
            indices[11] = 6;

            indices[12] = 6;
            indices[13] = 7;

            indices[14] = 7;
            indices[15] = 4;

            // side lines
            indices[16] = 0;
            indices[17] = 4;

            indices[18] = 1;
            indices[19] = 5;

            indices[20] = 2;
            indices[21] = 6;

            indices[22] = 3;
            indices[23] = 7;

            s_webAabbMesh.SetVertices(vertices);
            s_webAabbMesh.SetIndices(indices, MeshTopology.Lines, 0);
            s_webAabbMesh.bounds = new Bounds()
            {
                center = Vector3.zero,
                extents = new Vector3(1000, 1000, 1000)
            };

            s_lineMesh = new Mesh();

            var lineVertices = new List<Vector3>();
            lineVertices.Add(Vector3.zero);
            lineVertices.Add(Vector3.zero);

            s_lineMesh.SetVertices(lineVertices);
            s_lineMesh.SetIndices(new int[] { 0, 1 }, MeshTopology.Lines, 0);
            s_lineMesh.bounds = new Bounds()
            {
                center = Vector3.zero,
                extents = new Vector3(1000, 1000, 1000)
            };

            s_pointMesh = new Mesh();
            var pointVertices = new List<Vector3>();
            pointVertices.Add(new Vector3(-0.5f, 0, 0));
            pointVertices.Add(new Vector3(0, 0, -0.5f));
            pointVertices.Add(new Vector3(0.5f, 0, 0));
            pointVertices.Add(new Vector3(0, 0, 0.5f));

            pointVertices.Add(new Vector3(0, 0.5f, 0));
            pointVertices.Add(new Vector3(0, -0.5f, 0));

            var pointIndices = new int[24];
            pointIndices[0] = 4;
            pointIndices[1] = 1;
            pointIndices[2] = 0;

            pointIndices[3] = 4;
            pointIndices[4] = 2;
            pointIndices[5] = 1;

            pointIndices[6] = 4;
            pointIndices[7] = 3;
            pointIndices[8] = 2;

            pointIndices[9] = 4;
            pointIndices[10] = 0;
            pointIndices[11] = 3;

            pointIndices[12] = 5;
            pointIndices[13] = 0;
            pointIndices[14] = 1;

            pointIndices[15] = 5;
            pointIndices[16] = 1;
            pointIndices[17] = 2;

            pointIndices[18] = 5;
            pointIndices[19] = 2;
            pointIndices[20] = 3;

            pointIndices[21] = 5;
            pointIndices[22] = 3;
            pointIndices[23] = 0;

            s_pointMesh.SetVertices(pointVertices);
            s_pointMesh.SetIndices(pointIndices, MeshTopology.Triangles, 0);
        }
    }
}
