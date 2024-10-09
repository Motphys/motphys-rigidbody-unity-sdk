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

using UnityEngine;

namespace Motphys.Rigidbody
{
    public static class PhysicsUtils
    {
        /// <summary>
        /// Create gameobject with motphys collider by primitive type.
        /// </summary>
        /// <param name="primitiveType"></param>
        /// <returns></returns>
        public static GameObject CreatePrimitive(PrimitiveType primitiveType)
        {
            return CreatePrimitive(primitiveType, Vector3.zero);
        }

        /// <summary>
        /// Create gameobject with motphys collider by primitive type.
        /// </summary>
        /// <param name="primitiveType"></param>
        /// <param name="position"></param>
        /// <returns></returns>

        public static GameObject CreatePrimitive(PrimitiveType primitiveType, Vector3 position)
        {
            var go = GameObject.CreatePrimitive(primitiveType);
            if (go.TryGetComponent<UnityEngine.Collider>(out var unityCollider))
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    MonoBehaviour.DestroyImmediate(unityCollider);
                }
                else
                {
                    MonoBehaviour.Destroy(unityCollider);
                }
#else
                MonoBehaviour.Destroy(unityCollider);
#endif
            }

            go.transform.position = position;
            AddCollider(go, primitiveType);
            return go;
        }

        private static void AddCollider(GameObject go, PrimitiveType primitiveType)
        {
            switch (primitiveType)
            {
                case PrimitiveType.Sphere:
                    go.AddComponent<SphereCollider3D>();
                    break;
                case PrimitiveType.Capsule:
                    go.AddComponent<CapsuleCollider3D>();
                    break;
                case PrimitiveType.Cylinder:
                    go.AddComponent<CylinderCollider3D>();
                    break;
                case PrimitiveType.Cube:
                    go.AddComponent<BoxCollider3D>();
                    break;
                case PrimitiveType.Plane:
                    go.AddComponent<InfinitePlaneCollider3D>();
                    break;
                case PrimitiveType.Quad:
                    go.AddComponent<BoxCollider3D>();
                    break;
            }
        }
    }
}

