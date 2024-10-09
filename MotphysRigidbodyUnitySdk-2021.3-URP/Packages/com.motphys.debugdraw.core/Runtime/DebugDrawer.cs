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
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.TestTools;

[assembly: InternalsVisibleTo("Motphys.DebugDraw.Editor")]
[assembly: InternalsVisibleTo("Motphys.DebugDraw.Editor.PlayMode")]
[assembly: InternalsVisibleTo("Motphys.DebugDraw.Runtime")]
[assembly: InternalsVisibleTo("Motphys.DebugDraw.Runtime.Tests")]
namespace Motphys.DebugDraw.Core
{
    /// <summary>
    /// Debug Gizmos Drawer, the basic API container
    /// </summary>
    internal partial class DebugDrawer
    {
        private static Material s_aabbMaterial;
        private static Material s_lineMaterial;
        private static Material s_instanceMaterial;
        private static Material s_capsuleMaterial;
        private static Material s_pointMaterial;

        private static ComputeShader s_aabbComputeShader;
        private static ComputeBuffer s_aabbInputBuffer;
        private static ComputeBuffer s_aabbOutputBuffer;
        private static ComputeBuffer s_lineVertexBuffer;
        private static ComputeBuffer s_pointPositionBuffer;

        private static int s_maxCountId;
        private static int s_kernal;
        private static int s_vertexBufferId;
        private static int s_colorId;
        private static int s_radiusId;
        private static int s_heightId;
        private static int s_pointSizeId;
        private static int s_extendId;

        private static Mesh s_cube, s_sphere, s_capsule, s_cylinder;
        private static bool s_initialized;
        private static bool s_ableToDrawAabb;

        private const int VertexPerAabb = 24;

        private static GizmosSettings s_gizmosSettings;

        public static bool EnableComputeShader { get; internal set; } = SystemInfo.supportsComputeShaders;
        /// <summary>
        /// Initialize renderer assets
        /// </summary>
        /// <param name="settings"></param>
        internal static void Initialize(GizmosSettings settings)
        {
            if (s_initialized || settings == null)
            {
                return;
            }

            s_gizmosSettings = settings;
            s_initialized = true;
            LoadAssets(settings);

            s_maxCountId = Shader.PropertyToID("_MaxCount");
            s_vertexBufferId = Shader.PropertyToID("_VertexBuffer");
            s_colorId = Shader.PropertyToID("_Color");
            s_radiusId = Shader.PropertyToID("_Radius");
            s_heightId = Shader.PropertyToID("_Height");
            s_pointSizeId = Shader.PropertyToID("_PointScale");
            s_extendId = Shader.PropertyToID("_Extend");

            CreateRenderResource();
        }

        /// <summary>
        /// Release renderer assets
        /// </summary>
        [ExcludeFromCoverage] // OneTimeTearDown is not included in coverage
        internal static void Dispose()
        {
            s_aabbInputBuffer?.Dispose();
            s_aabbOutputBuffer?.Dispose();
            s_lineVertexBuffer?.Dispose();
            s_pointPositionBuffer?.Dispose();
            s_initialized = false;
        }

        /// <summary>
        /// Draw Cubes
        /// </summary>
        /// <param name="cmd">Command buffer for renderer</param>
        /// <param name="matrices">The shapes' localToWorld matrix</param>
        /// <param name="color">The shapes' color</param>
        internal static void DrawCubes(CommandBuffer cmd, Matrix4x4[] matrices, Color color)
        {
            DrawShapes(ShapeType.Cube, cmd, matrices, color);
        }

        internal static void DrawCube(CommandBuffer cmd, Matrix4x4 matrix, Color color)
        {
            DrawShape(ShapeType.Cube, cmd, matrix, color);
        }
        /// <summary>
        /// Draw Spheres
        /// </summary>
        /// <param name="cmd">Command buffer for renderer</param>
        /// <param name="matrices">The shapes' localToWorld matrix</param>
        /// <param name="color">The shapes' color</param>
        internal static void DrawSpheres(CommandBuffer cmd, Matrix4x4[] matrices, Color color)
        {
            DrawShapes(ShapeType.Sphere, cmd, matrices, color);
        }

        internal static void DrawSphere(CommandBuffer cmd, Matrix4x4 matrix, Color color)
        {
            DrawShape(ShapeType.Sphere, cmd, matrix, color);
        }

        /// <summary>
        /// Draw Capsules
        /// </summary>
        /// <param name="cmd">Command buffer for renderer</param>
        /// <param name="matrices">The shapes' localToWorld matrix</param>
        /// <param name="color">The shapes' color</param>
        internal static void DrawCapsules(CommandBuffer cmd, Matrix4x4[] matrices, Color color)
        {
            DrawShapes(ShapeType.Capsule, cmd, matrices, color);
        }

        internal static void DrawCapsule(CommandBuffer cmd, Matrix4x4 matrix, float radius, float height, Color color)
        {
            if (cmd == null || !s_initialized)
            {
                return;
            }

            if (s_initialized)
            {
                cmd.SetGlobalFloat(s_radiusId, radius);
                cmd.SetGlobalFloat(s_heightId, height);
                cmd.SetGlobalColor(s_colorId, color);
                cmd.DrawMesh(s_capsule, matrix, s_capsuleMaterial);
            }
        }

        internal static void DrawCylinders(CommandBuffer cmd, Matrix4x4[] matrices, Color color)
        {
            DrawShapes(ShapeType.Cylinder, cmd, matrices, color);
        }

        internal static void DrawCylinder(CommandBuffer cmd, Matrix4x4 matrix, Color color)
        {
            DrawShape(ShapeType.Cylinder, cmd, matrix, color);
        }

        internal static void DrawMeshes(CommandBuffer cmd, Mesh mesh, Matrix4x4[] matrices, Color color)
        {
            if (cmd == null || mesh == null || !s_initialized)
            {
                return;
            }

            var count = matrices?.Length ?? -1;

            if (count <= 0)
            {
                return;
            }

            if (count > Constants.MaxInstanceShapeCount)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[{mesh.name}] Matrices length : [{count}] is larger than [{Constants.MaxInstanceShapeCount}].Can't draw mesh instanced");
#endif
                return;
            }

            if (s_instanceMaterial != null)
            {
                cmd.SetGlobalColor(s_colorId, color);
                cmd.DrawMeshInstanced(mesh, 0, s_instanceMaterial, 0, matrices, count);
            }
        }

        internal static void DrawMesh(CommandBuffer cmd, Mesh mesh, Matrix4x4 matrix, Color color)
        {
            if (cmd == null || mesh == null || s_instanceMaterial == null || !s_initialized)
            {
                return;
            }

            cmd.SetGlobalColor(s_colorId, color);
            cmd.DrawMesh(mesh, matrix, s_instanceMaterial);
        }

        /// <summary>
        /// Draw lines
        /// </summary>
        /// <param name="cmd">Command buffer for renderer</param>
        /// <param name="lines">Array of lines</param>
        internal static void DrawLines(CommandBuffer cmd, NativeArray<Line> lines)
        {
            if (EnableComputeShader)
            {
                DrawProceduralLines(cmd, lines);
            }
            else
            {
                DrawMeshLines(cmd, lines);
            }
        }

        /// <summary>
        /// Draw AABBs(Axis Aligned Bounding Box)
        /// </summary>
        /// <param name="cmd">Command buffer for renderer</param>
        /// <param name="aabbs">Array of AABBs</param>
        internal static void DrawAabbs(CommandBuffer cmd, NativeArray<Motphys.Aabb3> aabbs, Color color)
        {
            if (EnableComputeShader)
            {
                DrawProceduralAabbs(cmd, aabbs, color);
            }
            else
            {
                DrawMeshAabbs(cmd, aabbs, color);
            }
        }

        /// <summary>
        /// Draw Points
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="positions"></param>
        /// <param name="color"></param>
        internal static void DrawPoints(CommandBuffer cmd, NativeArray<float3> positions, Color color, float pointSize)
        {
            if (EnableComputeShader)
            {
                DrawProceduralPoints(cmd, positions, color, pointSize);
            }
            else
            {
                DrawMeshPoints(cmd, positions, color, pointSize);
            }
        }

        internal static void SetupNormalExtendParameters(CommandBuffer cmd, float value)
        {
            cmd.SetGlobalFloat(s_extendId, value);
        }

        internal static void CreateRenderResource()
        {
            var settings = s_gizmosSettings;
            s_capsuleMaterial = new Material(settings._capsuleShader);
            InitialMeshResources(settings);

            s_ableToDrawAabb = InitializeComputeShader(settings._aabbComputeShader);
            s_aabbMaterial = new Material(settings._aabbShader);
            s_lineMaterial = new Material(settings._lineShader);
            CreateLineBuffer(Constants.BaseBufferCount);

            s_pointMaterial = new Material(settings._pointShader);
            CreatePointBuffer(Constants.BaseBufferCount);
        }

        private static void DrawProceduralLines(CommandBuffer cmd, NativeArray<Line> lines)
        {
            if (cmd == null || !s_initialized || !lines.IsCreated)
            {
                return;
            }

            var count = lines.Length;

            if (count <= 0)
            {
                return;
            }

            var capacity = s_lineVertexBuffer.count;

            if (count > capacity)
            {
                CreateLineBuffer(Mathf.NextPowerOfTwo(count));
            }

            if (s_initialized)
            {
                s_lineVertexBuffer.SetData(lines, 0, 0, count);
                cmd.DrawProcedural(Matrix4x4.identity, s_lineMaterial, 0, MeshTopology.Lines, 2, count);
            }
        }

        private static void DrawProceduralPoints(CommandBuffer cmd, NativeArray<float3> positions, Color color, float pointSize)
        {
            if (cmd == null || !s_initialized || !positions.IsCreated)
            {
                return;
            }

            var count = positions.Length;

            if (count <= 0)
            {
                return;
            }

            var currentCapacity = s_pointPositionBuffer.count;

            if (count > currentCapacity)
            {
                CreatePointBuffer(Mathf.NextPowerOfTwo(count));
            }

            s_pointPositionBuffer.SetData(positions, 0, 0, count);
            s_pointMaterial.SetColor(s_colorId, color);
            s_pointMaterial.SetFloat(s_pointSizeId, pointSize);
            cmd.DrawMeshInstancedProcedural(s_sphere, 0, s_pointMaterial, 0, count);
        }

        private static void DrawProceduralAabbs(CommandBuffer cmd, NativeArray<Motphys.Aabb3> aabbs, Color color)
        {
            if (cmd == null || !s_ableToDrawAabb || !s_initialized || !aabbs.IsCreated)
            {
                return;
            }

            var count = aabbs.Length;

            if (count <= 0)
            {
                return;
            }

            var currentCapacity = s_aabbInputBuffer.count;

            // extends compute buffer
            if (count > currentCapacity)
            {
                CreateAabbComputeBuffer(Mathf.NextPowerOfTwo(count));
            }

            s_aabbInputBuffer.SetData(aabbs, 0, 0, count);
            s_aabbComputeShader.SetInt(s_maxCountId, count);
            s_aabbComputeShader.GetKernelThreadGroupSizes(s_kernal, out var x, out var y, out var z);

            Assert.IsTrue(y == 1);
            Assert.IsTrue(z == 1);

            var groups = (int)((count + x - 1) / x);
            cmd.DispatchCompute(s_aabbComputeShader, s_kernal, groups, 1, 1);
            s_aabbMaterial.SetBuffer(s_vertexBufferId, s_aabbOutputBuffer);
            s_aabbMaterial.SetColor(s_colorId, color);
            cmd.DrawProcedural(Matrix4x4.identity, s_aabbMaterial, 0, MeshTopology.Lines, count * VertexPerAabb, 1);
        }

        private static void DrawShapes(ShapeType shapeType, CommandBuffer cmd, Matrix4x4[] matrices, Color color)
        {
            if (cmd == null || !s_initialized)
            {
                return;
            }

            var count = matrices?.Length ?? -1;

            if (count <= 0)
            {
                return;
            }

            if (count > Constants.MaxInstanceShapeCount)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[{shapeType}] Matrices length : [{count}] is larger than [{Constants.MaxInstanceShapeCount}].Can't draw mesh instanced");
#endif
                return;
            }

            Mesh mesh = null;

            switch (shapeType)
            {
                case ShapeType.Capsule:
                    mesh = s_capsule;
                    break;
                case ShapeType.Cube:
                    mesh = s_cube;
                    break;
                case ShapeType.Sphere:
                    mesh = s_sphere;
                    break;
                case ShapeType.Cylinder:
                    mesh = s_cylinder;
                    break;
            }

            if (s_instanceMaterial != null && mesh != null)
            {
                cmd.SetGlobalColor(s_colorId, color);
                cmd.DrawMeshInstanced(mesh, 0, s_instanceMaterial, 0, matrices, count);
            }
        }

        private static void DrawShape(ShapeType shapeType, CommandBuffer cmd, Matrix4x4 matrix, Color color)
        {
            if (cmd == null || !s_initialized)
            {
                return;
            }

            Mesh mesh = null;

            switch (shapeType)
            {
                case ShapeType.Cube:
                    mesh = s_cube;
                    break;
                case ShapeType.Sphere:
                    mesh = s_sphere;
                    break;
                case ShapeType.Cylinder:
                    mesh = s_cylinder;
                    break;
            }

            if (s_instanceMaterial != null && mesh != null)
            {
                cmd.SetGlobalColor(s_colorId, color);
                cmd.DrawMesh(mesh, matrix, s_instanceMaterial);
            }
        }

        private static void LoadAssets(GizmosSettings assets)
        {
            if (assets != null)
            {
                s_cube = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
                s_sphere = Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");
                s_capsule = Resources.GetBuiltinResource<Mesh>("New-Capsule.fbx");
                s_cylinder = Resources.GetBuiltinResource<Mesh>("New-Cylinder.fbx");

                s_instanceMaterial = assets._instanceMaterial;
            }
        }

        private static bool InitializeComputeShader(ComputeShader computeShader)
        {
            bool flag = false;
            if (computeShader != null && EnableComputeShader)
            {
                s_aabbComputeShader = computeShader;
                s_kernal = s_aabbComputeShader.FindKernel("CSMain");

                CreateAabbComputeBuffer(Constants.BaseBufferCount);

                flag = true;
            }

            return flag;
        }

        private static void CreateAabbComputeBuffer(int count)
        {
            s_aabbInputBuffer?.Dispose();
            s_aabbOutputBuffer?.Dispose();

            int aabbSize = Marshal.SizeOf(typeof(Aabb3));
            int positionSize = Marshal.SizeOf(typeof(float3));

            count = Mathf.Min(count, Constants.MaxComputeBufferCount);

            s_aabbInputBuffer = new ComputeBuffer(count, aabbSize); // center + size
            s_aabbOutputBuffer = new ComputeBuffer(count * VertexPerAabb, positionSize); // 24 vertices

            s_aabbComputeShader.SetBuffer(s_kernal, "_Input", s_aabbInputBuffer);
            s_aabbComputeShader.SetBuffer(s_kernal, "_Aabbs", s_aabbOutputBuffer);
        }

        private static void CreateLineBuffer(int count)
        {
            if (EnableComputeShader)
            {
                count = Mathf.Min(count, Constants.MaxComputeBufferCount);

                s_lineVertexBuffer?.Dispose();
                int size = Marshal.SizeOf(typeof(Line));
                s_lineVertexBuffer = new ComputeBuffer(count, size);
                s_lineMaterial.SetBuffer("_LineBuffer", s_lineVertexBuffer);
            }
        }

        private static void CreatePointBuffer(int count)
        {
            if (EnableComputeShader)
            {
                count = Mathf.Min(count, Constants.MaxComputeBufferCount);

                s_pointPositionBuffer?.Dispose();
                int size = Marshal.SizeOf<float3>();
                s_pointPositionBuffer = new ComputeBuffer(count, size);
                s_pointMaterial.SetBuffer("_WorldPos", s_pointPositionBuffer);
            }
        }
    }
}
