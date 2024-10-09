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
    [AddComponentMenu("Motphys/Colliders/CapsuleCollider3D")]
    public class CapsuleCollider3D : BaseCollider
    {
        [Tooltip("The top and bottom radius of the capsule collider in the local space of the GameObject.")]
        [SerializeField]
        private float _radius = 0.5f;

        [Tooltip("The height of the capsule collider in the local space of the GameObject, including the radius of the capsule.")]
        [SerializeField]
        private float _height = 2.0f;

        /// <value>
        /// The height of the capsule collider in the local space of the GameObject, including the radius of the capsule.
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
        /// The top and bottom radius of the capsule collider in the local space of the GameObject. The valid range is [0.00001, inf).
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
            var radiusScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));
            var radius = Mathf.Abs(_radius * radiusScale);
            shape = new Capsule()
            {
                halfHeight = Mathf.Max(Mathf.Abs((_height - 2 * _radius) * scale.y * 0.5f), 0f),
                radius = Mathf.Max(radius, 1e-5f)
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

        protected override void OnValidate()
        {
            _radius = Mathf.Max(_radius, 0f);
            _height = Mathf.Max(_height, 0f);
            base.OnValidate();
        }
#endif
    }
}
