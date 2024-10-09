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
    [AddComponentMenu("Motphys/Colliders/BoxCollider3D")]
    public class BoxCollider3D : BaseCollider
    {
        [Tooltip("The size of the BoxCollider in the local space of the GameObject.")]
        [SerializeField]
        private Vector3 _size = Vector3.one;

        internal override bool OnCreateShape(Vector3 scale, out Shape shape)
        {
            var x = Mathf.Abs(_size.x * scale.x * 0.5f);
            x = Mathf.Approximately(x, 0f) ? 1e-5f : x;
            var y = Mathf.Abs(_size.y * scale.y * 0.5f);
            y = Mathf.Approximately(y, 0f) ? 1e-5f : y;
            var z = Mathf.Abs(_size.z * scale.z * 0.5f);
            z = Mathf.Approximately(z, 0f) ? 1e-5f : z;
            shape = new Cuboid() { halfExt = new Vector3(x, y, z) };
            return true;
        }

        internal override bool CanCreateShape()
        {
            return true;
        }

        /// <value>
        /// The size of the BoxCollider in the local space of the GameObject. The valid range is (0.00001, inf) in every dimension.
        /// </value>
        public Vector3 size
        {
            get
            {
                return _size;
            }
            set
            {
                if (_size == value)
                {
                    return;
                }

                _size = value;
                UpdateNativeShape();
            }
        }

#if UNITY_EDITOR
        private void Reset()
        {
            if (this.gameObject.TryGetComponent<MeshFilter>(out var meshFilter))
            {
                var mesh = meshFilter.sharedMesh;
                if (mesh != null)
                {
                    this.shapeTranslation = mesh.bounds.center;
                    this.size = mesh.bounds.size;
                }
            }
        }
#endif
    }
}
