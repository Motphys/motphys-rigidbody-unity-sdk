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
using UnityEditor;
using UnityEngine;

namespace Motphys.Rigidbody.Editor
{
    [CustomEditor(typeof(Rigidbody3D))]
    [CanEditMultipleObjects]
    [UnityEngine.TestTools.ExcludeFromCoverage]
    public class Rigidbody3DEditor : UnityEditor.Editor
    {
        private static bool s_debugFoldout = false;

        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        private static void DrawGizmo(Rigidbody3D rigidbody, GizmoType _)
        {
            if (rigidbody.isNativeValid)
            {
                var settings = MotphysPreference.GetOrCreateSettings();
                if (settings._showCenterOfMass)
                {
                    Gizmos.color = settings._centerOfMassColor;
                    Gizmos.DrawSphere(rigidbody.worldCenterOfMass, settings._centroidRadius);
                }
            }
        }

        private Rigidbody3D _target;

        private void OnEnable()
        {
            _target = target as Rigidbody3D;
        }

        /// <summary>
        /// Extract child colliders to attach to root rigidbody gameobject.
        /// Colliders within child rigidbody will be ignored.
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("CONTEXT/Rigidbody3D/ExtractColldiers")]
        public static void ExtractColldiers(MenuCommand menuCommand)
        {
            var rigidbody = menuCommand.context as Rigidbody3D;
            var gameobject = rigidbody.gameObject;

            var colliders = new List<BaseCollider>();
            IterChildColliders(gameobject.transform, colliders);
            foreach (var col in colliders)
            {
                ConvertCollider(gameobject, col);
            }
        }

        private static void IterChildColliders(Transform root, List<BaseCollider> childColldiers)
        {
            if (childColldiers == null)
            {
                return;
            }

            foreach (Transform child in root)
            {
                if (!child.TryGetComponent<Rigidbody3D>(out var childRigdbody))
                {
                    childColldiers.AddRange(child.GetComponents<BaseCollider>());
                    IterChildColliders(child, childColldiers);
                }
            }
        }

        private static void ConvertCollider(GameObject gameobject, BaseCollider collider)
        {
            var transform = gameobject.transform;

            {
                var oldBox = collider as BoxCollider3D;
                if (oldBox != null)
                {
                    var newBox = UnityEditor.Undo.AddComponent<BoxCollider3D>(gameobject);

                    EditorUtility.CopySerialized(oldBox, newBox);

                    ConvertTransform(transform, oldBox, newBox);

                    newBox.size = Vector3.Scale(oldBox.size, oldBox.transform.localScale);

                    UnityEditor.Undo.DestroyObjectImmediate(oldBox);
                    return;
                }
            }

            {
                var oldSphere = collider as SphereCollider3D;
                if (oldSphere != null)
                {
                    var newSphere = UnityEditor.Undo.AddComponent<SphereCollider3D>(gameobject);

                    EditorUtility.CopySerialized(oldSphere, newSphere);

                    ConvertTransform(transform, oldSphere, newSphere);

                    newSphere.radius = oldSphere.radius * Mathf.Max(oldSphere.transform.localScale.x,
                        oldSphere.transform.localScale.y,
                        oldSphere.transform.localScale.z);

                    UnityEditor.Undo.DestroyObjectImmediate(oldSphere);
                    return;
                }
            }

            {
                var oldCapsule = collider as CapsuleCollider3D;
                if (oldCapsule != null)
                {
                    var newCapsule = UnityEditor.Undo.AddComponent<CapsuleCollider3D>(gameobject);

                    EditorUtility.CopySerialized(oldCapsule, newCapsule);

                    ConvertTransform(transform, oldCapsule, newCapsule);

                    newCapsule.radius = oldCapsule.radius * Mathf.Max(oldCapsule.transform.localScale.x, oldCapsule.transform.localScale.z);
                    newCapsule.height = oldCapsule.height * oldCapsule.transform.localScale.y;

                    UnityEditor.Undo.DestroyObjectImmediate(oldCapsule);
                    return;
                }
            }

            {
                var oldCylinder = collider as CylinderCollider3D;
                if (oldCylinder != null)
                {
                    var newCylinder = UnityEditor.Undo.AddComponent<CylinderCollider3D>(gameobject);

                    EditorUtility.CopySerialized(oldCylinder, newCylinder);

                    ConvertTransform(transform, oldCylinder, newCylinder);

                    newCylinder.radius = oldCylinder.radius * Mathf.Max(oldCylinder.transform.localScale.x, oldCylinder.transform.localScale.z);
                    newCylinder.height = oldCylinder.height * oldCylinder.transform.localScale.y;

                    UnityEditor.Undo.DestroyObjectImmediate(oldCylinder);
                    return;
                }
            }

            {
                var oldPlane = collider as InfinitePlaneCollider3D;
                if (oldPlane != null)
                {
                    var newPlane = UnityEditor.Undo.AddComponent<InfinitePlaneCollider3D>(gameobject);

                    EditorUtility.CopySerialized(oldPlane, newPlane);

                    ConvertTransform(transform, oldPlane, newPlane);

                    UnityEditor.Undo.DestroyObjectImmediate(oldPlane);
                    return;
                }
            }

            {
                var oldMeshCollider = collider as MeshCollider3D;
                if (oldMeshCollider != null)
                {
                    var newMeshCollider = UnityEditor.Undo.AddComponent<MeshCollider3D>(gameobject);

                    EditorUtility.CopySerialized(oldMeshCollider, newMeshCollider);

                    ConvertTransform(transform, oldMeshCollider, newMeshCollider);

                    UnityEditor.Undo.DestroyObjectImmediate(oldMeshCollider);
                    return;
                }
            }

            Debug.LogError($"Unsupport collider: {collider.name}, extract failed.");
            return;
        }

        private static void ConvertTransform(Transform newTransform, BaseCollider oldCollider, BaseCollider newCollider)
        {
            var oldPosition = oldCollider.transform.TransformPoint(oldCollider.shapeTranslation);
            var oldRotation = oldCollider.transform.rotation * oldCollider.shapeRotation;
            var worldToActor = Matrix4x4.TRS(newTransform.position, newTransform.rotation, newTransform.lossyScale).inverse;

            var newlocalPosition = worldToActor * new Vector4(oldPosition.x, oldPosition.y, oldPosition.z, 1.0f);
            var newlocalRotation = worldToActor.rotation * oldRotation;

            newCollider.shapeTranslation = newlocalPosition;
            newCollider.shapeRotation = newlocalRotation;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Application.isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);
                s_debugFoldout = EditorGUILayout.Foldout(s_debugFoldout, "Debug");
                if (s_debugFoldout)
                {
                    var rigidbody = target as Rigidbody3D;
                    var nativeBridge = rigidbody.nativeBridge;
                    if (rigidbody.isNativeValid)
                    {
                        // var material = nativeBridge.handle.material;
                        var handle = nativeBridge.handle;
                        var status = handle.sleepStatus;
                        var wakeCounter = handle.wakeCounter;
                        EditorGUILayout.Vector3Field("Center Of Mass (Local Space)", rigidbody.centerOfMass);
                        // EditorGUILayout.FloatField("static friction", material.staticFriction);
                        EditorGUILayout.LabelField("Sleep Status", status.ToString());
                        EditorGUILayout.FloatField("Wake Counter", wakeCounter);
                        EditorGUILayout.Vector3Field("Linear Velocity", rigidbody.linearVelocity);
                        EditorGUILayout.Vector3Field("Angular Velocity", rigidbody.angularVelocity);
                        EditorGUILayout.IntField("Num Shapes", (int)rigidbody.numShapes);
                        var inertiaPrincipal = nativeBridge.handle.inertiaPrincipal;
                        EditorGUILayout.Vector3Field("Inertia", inertiaPrincipal);
                    }
                }

                EditorGUI.EndDisabledGroup();
            }
        }
    }
}
