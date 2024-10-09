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

using Motphys.Rigidbody.Internal;
using UnityEngine;

namespace Motphys.Rigidbody
{
    [AddComponentMenu("Motphys/Colliders/SphereCollider3D")]
    public class SphereCollider3D : BaseCollider
    {
        [Tooltip("The radius of the sphere collider in the local space of the GameObject.")]
        [Min(1e-5f)]
        [SerializeField]
        private float _radius = 0.5f;

        /// <value>
        /// The radius of the sphere collider in the local space of the GameObject. The valid range is [0.00001, inf).
        /// </value>
        public float radius
        {
            get => _radius;
            set
            {
                if (_radius == value)
                {
                    return;
                }

                _radius = value;
                UpdateNativeShape();
            }
        }

        internal override bool OnCreateShape(Vector3 scale, out Shape shape)
        {
            var radius = Mathf.Max(Mathf.Abs(_radius * scale.x), Mathf.Abs(_radius * scale.y), Mathf.Abs(_radius * scale.z));
            shape = new Sphere() { radius = Mathf.Approximately(radius, 0f) ? 1e-5f : radius };
            return true;
        }

        internal override bool CanCreateShape()
        {
            return true;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            if (this.gameObject.TryGetComponent<MeshFilter>(out var meshFilter))
            {
                var mesh = meshFilter.sharedMesh;
                if (mesh != null)
                {
                    var size = mesh.bounds.size;
                    this.shapeTranslation = mesh.bounds.center;
                    this.radius = Mathf.Max(size.x, size.y, size.z) * 0.5f;
                }
            }
        }
#endif
    }
}
