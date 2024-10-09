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
    [AddComponentMenu("Motphys/Colliders/CylinderCollider3D")]
    public class CylinderCollider3D : BaseCollider
    {
        [Tooltip("The height of the cylinder collider in the local space of the GameObject.")]
        [SerializeField]
        private float _height = 2.0f;

        [Tooltip("The top and bottom radius of the cylinder collider in the local space of the GameObject.")]
        [SerializeField]
        private float _radius = 0.5f;

        /// <value>
        /// The height of the cylinder collider in the local space of the GameObject
        /// </value>
        public float height
        {
            get => _height;
            set
            {
                if (_height == value)
                {
                    return;
                }

                _height = value;
                UpdateNativeShape();
            }
        }

        /// <value>
        /// The top and bottom radius of the cylinder collider in the local space of the GameObject
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
            var scale_radius = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));
            var scale_height = Mathf.Abs(scale.y);
            shape = new Cylinder()
            {
                halfHeight = Mathf.Max(0.5f * _height * scale_height, 1e-5f),
                radius = Mathf.Max(_radius * scale_radius, 1e-5f)
            };
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
                    this.height = size.y;
                    this.radius = Mathf.Max(size.x, size.z) * 0.5f;
                }
            }
        }

        // invoke when add component and edit in inspector
        // currently not easy to invoke in test
        [UnityEngine.TestTools.ExcludeFromCoverage]
        protected override void OnValidate()
        {
            if (_radius <= 0)
            {
                _radius = 1e-5f;
            }

            if (_height <= 0)
            {
                _height = 1e-5f;
            }

            base.OnValidate();
        }
#endif
    }
}
