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

using UnityEditor;
using UnityEngine;

namespace Motphys.Rigidbody.Editor
{
    public class MotphysCreateMenu
    {
        internal static void Place(GameObject go, GameObject parent = null)
        {
            if (parent != null)
            {
                var transform = go.transform;
                Undo.SetTransformParent(transform, parent.transform, "Reparenting");
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
                go.layer = parent.gameObject.layer;
            }
            else
            {
                SceneView.lastActiveSceneView?.MoveToView(go.transform);
            }

            GameObjectUtility.EnsureUniqueNameForSibling(go);
            Undo.SetCurrentGroupName("Create " + go.name);

            Selection.activeGameObject = go;
        }

        public static GameObject CreateAndPlacePrimitive(PrimitiveType type, GameObject parent)
        {
            var primitive = ObjectFactory.CreatePrimitive(type);
            if (primitive.TryGetComponent<Collider>(out var unityCollider))
            {
                MonoBehaviour.DestroyImmediate(unityCollider);
            }

            switch (type)
            {
                case PrimitiveType.Sphere:
                    ObjectFactory.AddComponent<SphereCollider3D>(primitive);
                    break;
                case PrimitiveType.Capsule:
                    ObjectFactory.AddComponent<CapsuleCollider3D>(primitive);
                    break;
                case PrimitiveType.Cylinder:
                    ObjectFactory.AddComponent<CylinderCollider3D>(primitive);
                    break;
                case PrimitiveType.Cube:
                    ObjectFactory.AddComponent<BoxCollider3D>(primitive);
                    break;
                case PrimitiveType.Plane:
                    ObjectFactory.AddComponent<InfinitePlaneCollider3D>(primitive);
                    break;
                case PrimitiveType.Quad:
                    ObjectFactory.AddComponent<BoxCollider3D>(primitive);
                    break;
            }

            primitive.name = type.ToString();
            Place(primitive, parent);
            return primitive;
        }

        [MenuItem("GameObject/Motphys/Cube", priority = 3, validate = false)]
        internal static void CreateCube(MenuCommand menuCommand)
        {
            CreateAndPlacePrimitive(PrimitiveType.Cube, menuCommand?.context as GameObject);
        }

        [MenuItem("GameObject/Motphys/Sphere", priority = 3, validate = false)]
        internal static void CreateSphere(MenuCommand menuCommand)
        {
            CreateAndPlacePrimitive(PrimitiveType.Sphere, menuCommand?.context as GameObject);
        }

        [MenuItem("GameObject/Motphys/Capsule", priority = 3, validate = false)]
        internal static void CreateCapsule(MenuCommand menuCommand)
        {
            CreateAndPlacePrimitive(PrimitiveType.Capsule, menuCommand?.context as GameObject);
        }

        [MenuItem("GameObject/Motphys/Cylinder", priority = 3, validate = false)]
        internal static void CreateCylinder(MenuCommand menuCommand)
        {
            CreateAndPlacePrimitive(PrimitiveType.Cylinder, menuCommand?.context as GameObject);
        }
        [MenuItem("GameObject/Motphys/Plane", priority = 3, validate = false)]
        internal static void CreatePlane(MenuCommand menuCommand)
        {
            CreateAndPlacePrimitive(PrimitiveType.Plane, menuCommand?.context as GameObject);
        }
        [MenuItem("GameObject/Motphys/Quad", priority = 3, validate = false)]
        internal static void CreateQuad(MenuCommand menuCommand)
        {
            CreateAndPlacePrimitive(PrimitiveType.Quad, menuCommand?.context as GameObject);
        }
    }
}
