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

using System.Linq;
using Motphys.Rigidbody.Internal;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Motphys.Rigidbody.Editor
{
    // Test is not needed for editor gui code.
    [UnityEngine.TestTools.ExcludeFromCoverage]
    internal class BaseColliderEditor : UnityEditor.Editor
    {
        protected static class BaseStyles
        {
            internal static readonly GUIContent s_editCollider = EditorGUIUtility.TrTextContent("Edit Collider", "Press this button to Edit the collider shape");
        }

        protected static Color colliderBoundingEnableColor = new Color(0.4f, 0.8f, 0.4f, 0.6f);
        protected static Color colliderBoundingDisableColor = new Color(0.4f, 0.8f, 0.4f, 0.25f);
        protected static Color backFaceAlphaMultiplier = new Color(1.0f, 1.0f, 1.0f, 0.2f);
        protected bool _editCollider;

        private BaseCollider _baseCollider;
        [SerializeField]
        private MaterialData _actualMaterialData = MaterialData.Default;
        private SerializedObject _editorObject;

        private void OnEnable()
        {
            _baseCollider = target as BaseCollider;
            _editorObject = new SerializedObject(this);

            FindEditorColliderIndex(_baseCollider);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(BaseStyles.s_editCollider, GUILayout.Width(EditorGUIUtility.labelWidth - 4f));
                EditorGUILayout.EditorToolbarForTarget(this);
            }

            EditorGUILayout.Space(5f);
            DrawDefaultInspector();
            DrawDynamicScale();
            if (Application.isPlaying)
            {
                _actualMaterialData = _baseCollider.actualMaterialData;
                _editorObject.Update();

                EditorGUILayout.PropertyField(_editorObject.FindProperty("_actualMaterialData"));
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        protected virtual void DrawDynamicScale()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_supportDynamicScale"));
        }

        /// <summary>
        /// Only assign collider index when the count of colliders on a gameobject is larger than 1.
        /// </summary>
        /// <param name="colldier"></param>
        internal static void FindEditorColliderIndex(BaseCollider colldier)
        {
            if (colldier == null)
            {
                return;
            }

            colldier._editorColliderIndex = -1;
            var colliders = colldier.gameObject.GetComponents<BaseCollider>();
            if (colliders != null && colliders.Length > 1)
            {
                var index = 0;

                for (var i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i] == colldier)
                    {
                        index = i;
                        break;
                    }
                }

                colldier._editorColliderIndex = index;
            }
        }

        public static void DrawEditorColliderIndex(BaseCollider collider)
        {
            var settings = MotphysPreference.GetOrCreateSettings();
            if (!settings._showEditorColliderIndex)
            {
                return;
            }

            if (collider._editorColliderIndex >= 0)
            {
                using (new Handles.DrawingScope(collider.shapeToWorldMatrix))
                {
                    Handles.Label(Vector3.zero, collider._editorColliderIndex.ToString());
                }
            }
        }
    }

    // Editor Tool Code, exclude from test coverage
    [UnityEngine.TestTools.ExcludeFromCoverage]
    internal abstract class Collider3DEditorTool<T> : EditorTool where T : BaseCollider
    {
        public static Color wireFrameEnableColor = new Color(0.0f, 0.9f, 0.0f, 0.6f);
        public static Color wireFrameDisableColor = new Color(0.0f, 0.9f, 0.0f, 0.25f);
        public static Color handleColor = new Color(0.0f, 0.9f, 0.0f, 0.7f);
        public static Color backFaceAlphaMultiplier = new Color(1.0f, 1.0f, 1.0f, 0.4f);
        public float handleSize = 0.03f;
        private static GUIContent s_icon;
        public override GUIContent toolbarIcon => s_icon ??= new GUIContent(EditorGUIUtility.IconContent("EditCollider").image,
             EditorGUIUtility.TrTextContent("Edit Collider3D Tool.").text);

        public override void OnToolGUI(EditorWindow window)
        {
            Undo.RecordObjects(targets.ToArray(), "Motphys:Edit Collider Tool");
            foreach (var target in targets)
            {
                if (!(target is T collider) || Mathf.Approximately(collider.transform.lossyScale.sqrMagnitude, 0f))
                {
                    continue;
                }

                DrawWireframe(collider);
                using (new Handles.DrawingScope(handleColor, Matrix4x4.TRS(collider.transform.position, collider.transform.rotation, Vector3.one)))
                {
                    EditorGUI.BeginChangeCheck();
                    DrawHandles(collider);
                    if (EditorGUI.EndChangeCheck())
                    {
                        UpdateProperties(collider);
                    }
                }
            }
        }

        protected abstract void DrawHandles(T collider);
        protected abstract void DrawWireframe(T collider);
        protected abstract void UpdateProperties(T collider);

        protected Vector3 InvertScaleVector(Vector3 scaleVector)
        {
            for (var i = 0; i < 3; i++)
            {
                scaleVector[i] = (scaleVector[i] == 0f) ? 0f : (1f / scaleVector[i]);
            }

            return scaleVector;
        }
    }

    internal struct MidPointHandle
    {
        public int controlID;
        public Vector3 position;
        public readonly Vector3 Direction;
        public MidPointHandle(Vector3 direction)
        {
            controlID = 0;
            position = Vector3.zero;
            Direction = direction;
        }
    }
}
