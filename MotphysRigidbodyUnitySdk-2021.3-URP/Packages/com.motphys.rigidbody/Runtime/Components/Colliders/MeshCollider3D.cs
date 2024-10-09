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

using Motphys.Native;
using Motphys.Rigidbody.Internal;
using Unity.Collections;
using UnityEngine;

namespace Motphys.Rigidbody
{
    /// <summary>
    /// the mesh collider component
    /// </summary>
    [AddComponentMenu("Motphys/Colliders/MeshCollider3D")]
    public class MeshCollider3D : BaseCollider
    {
        [Tooltip("The mesh value of the MeshCollider3D in the local space of the GameObject.")]
        [SerializeField] private Mesh _mesh = null;
        [Tooltip("Whether the mesh needs to be convexed or not")]
        [HideInInspector]
        [SerializeField] private bool _convex = true;
        private ShapeId _shapeId = ShapeId.Invalid;
        private bool _cacheConvex;
        private bool _needRebuildMesh;

        /// <value>
        /// The mesh value to init MeshCollider3D
        /// </value>
        public Mesh mesh
        {
            get => _mesh;
            set
            {
                if (_mesh != value)
                {
                    _needRebuildMesh = true;
                    _mesh = value;
                    UpdateNativeShape();
#if UNITY_EDITOR
                    OnValidate();
#endif
                }
            }
        }

        internal bool convex
        {
            get => _convex;
            set
            {
                if (_convex != value)
                {
                    _needRebuildMesh = true;
                    _convex = value;
                    UpdateNativeShape();
                }
            }
        }

#if UNITY_EDITOR
        [UnityEngine.TestTools.ExcludeFromCoverage]
        internal void Reset()
        {
            if (TryGetComponent<MeshFilter>(out var meshFilter))
            {
                var mesh = meshFilter.sharedMesh;
                if (mesh != null)
                {
                    this._mesh = mesh;
                    this._lastMesh = mesh;
                    _displayMesh = new ConvexMesh(_mesh);
                }
            }
        }

        [HideInInspector]
        internal ConvexMesh _displayMesh;
        internal Mesh _lastMesh;

        /// <value>
        /// Indicates whether the mesh meets the requirements of the MeshCollider.
        /// </value>
        internal bool _isMeshValid => _displayMesh._isValid;
        internal string _errorLog => _displayMesh._errorLog;

        [UnityEngine.TestTools.ExcludeFromCoverage]
        protected override void OnValidate()
        {
            _needRebuildMesh = true;
            base.OnValidate();

            if (_mesh == null)
            {
                return;
            }

            if ((convex && _displayMesh == null) || _lastMesh != _mesh)
            {
                _displayMesh = new ConvexMesh(_mesh);
                _lastMesh = _mesh;
            }
        }
#endif

        private void OnDestroy()
        {
            if (PhysicsManager.engine != null && _shapeId.isValid && !PhysicsManager.engine.isDisposed)
            {
                if (_cacheConvex)
                {
                    PhysicsManager.engine.defaultWorld.DestroyConvexHull(_shapeId);
                }
            }
        }

        private void CreateMeshShape()
        {
            if (_mesh == null)
            {
                return;
            }

            if (_convex)
            {
                using (var dataArray = Mesh.AcquireReadOnlyMeshData(_mesh))
                {
                    var gotVertices = new NativeArray<Vector3>(mesh.vertexCount, Allocator.Temp);
                    dataArray[0].GetVertices(gotVertices);
                    _shapeId = PhysicsManager.engine.defaultWorld.CreateConvexHull(new SliceRef<Vector3>(gotVertices));
                    _cacheConvex = convex;
                    gotVertices.Dispose();
                }
            }
            else
            {
                // create mesh
            }
        }

        internal override bool OnCreateShape(Vector3 scale, out Shape shape)
        {
            if (_needRebuildMesh)
            {
                if (_shapeId.isValid)
                {
                    if (_cacheConvex)
                    {
                        PhysicsManager.engine.defaultWorld.DestroyConvexHull(_shapeId);
                    }
                    else
                    {
                        // destory mesh
                    }

                    _shapeId = ShapeId.Invalid;
                }

                CreateMeshShape();
            }
            else if (!_shapeId.isValid)
            {
                CreateMeshShape();
            }

            _needRebuildMesh = false;
            if (_convex && _shapeId.isValid)
            {
                shape = new ConvexHullBuilder() { id = _shapeId };
                return true;
            }

            // to prevent crash
            shape = new ConvexHullBuilder() { id = ShapeId.Invalid };
            return false;
            //return new MeshBuilder() { id = _shapeId };
        }

        internal override bool CanCreateShape()
        {
            return convex && mesh;
        }
    }
}
