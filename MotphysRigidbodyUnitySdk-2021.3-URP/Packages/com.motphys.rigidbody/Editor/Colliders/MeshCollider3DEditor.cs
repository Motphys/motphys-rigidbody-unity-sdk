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
    [UnityEngine.TestTools.ExcludeFromCoverage]
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MeshCollider3D))]
    public class MeshCollider3DEditor : UnityEditor.Editor
    {
        private MeshCollider3D _target;
        private GUIContent _convexText = new GUIContent("Convex", "Whether the mesh needs to be convexed or not. Mesh must be convex currently");
        private void OnEnable()
        {
            _target = target as MeshCollider3D;

            BaseColliderEditor.FindEditorColliderIndex(_target);
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        private static void DrawGizmo(MeshCollider3D collider, GizmoType _)
        {
            if (collider._displayMesh?.Mesh != null)
            {
                var matrix = Gizmos.matrix;

                var localToWorld = collider.transform.localToWorldMatrix;
                var position = localToWorld.MultiplyPoint3x4(collider.shapeTranslation);
                var rotation = collider.transform.rotation * collider.shapeRotation;
                var shapeMatrix = Matrix4x4.TRS(position, rotation, Vector3.one);

                Gizmos.matrix = shapeMatrix;
                Gizmos.color = collider.enabled ? new Color(0.4f, 0.8f, 0.4f, 0.6f) : new Color(0.4f, 0.8f, 0.4f, 0.25f);
                Gizmos.DrawWireMesh(collider._displayMesh.Mesh);
                Gizmos.matrix = matrix;
            }
        }

        [DrawGizmo(GizmoType.Selected)]
        private static void DrawEditorColliderIndex(MeshCollider3D collider, GizmoType _)
        {
            BaseColliderEditor.DrawEditorColliderIndex(collider);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle(_convexText, _target.convex);
            EditorGUI.EndDisabledGroup();
            if (!_target._isMeshValid)
            {
                EditorGUILayout.HelpBox($"No valid mesh set, MeshCollider3D will not work. \n{_target._errorLog}", MessageType.Error);
            }
        }
    }
}
