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
using UnityEditor.EditorTools;
using UnityEngine;

namespace Motphys.Rigidbody.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BoxCollider3D))]
    // Test is not needed for editor gui code.
    [UnityEngine.TestTools.ExcludeFromCoverage]
    internal class BoxCollider3DEditor : BaseColliderEditor
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        private static void DrawGizmo(BoxCollider3D collider, GizmoType _)
        {
            DrawWireframe(collider, collider.enabled ? colliderBoundingEnableColor : colliderBoundingDisableColor);
        }

        [DrawGizmo(GizmoType.Selected)]
        private static void DrawEditorColliderIndex(BoxCollider3D collider, GizmoType _)
        {
            DrawEditorColliderIndex(collider);
        }

        public static void DrawWireframe(BoxCollider3D collider, Color color)
        {
            using (new Handles.DrawingScope(color, collider.shapeToWorldMatrix))
            {
                Handles.DrawWireCube(Vector3.zero, collider.size);
            }
        }
    }

    [EditorTool("Edit Box Collider3D", typeof(BoxCollider3D))]
    internal class BoxCollider3DEditorTool : Collider3DEditorTool<BoxCollider3D>
    {
        private MidPointHandle[] _midPointHandles = new MidPointHandle[6]
        {
            new MidPointHandle(Vector3.right),
            new MidPointHandle(Vector3.up),
            new MidPointHandle(Vector3.forward),
            new MidPointHandle(Vector3.left),
            new MidPointHandle(Vector3.down),
            new MidPointHandle(Vector3.back),
        };

        protected override void DrawHandles(BoxCollider3D collider)
        {
            var localCameraPosition = Handles.inverseMatrix.MultiplyPoint(Camera.current.transform.position);
            var lossyScale = collider.transform.lossyScale;
            var center = Vector3.Scale(lossyScale, collider.shapeTranslation);
            var rotation = collider.shapeRotation;
            var sizeElements = new float[3] { collider.size.x * lossyScale.x, collider.size.y * lossyScale.y, collider.size.z * lossyScale.z };
            for (var i = 0; i < _midPointHandles.Length; ++i)
            {
                _midPointHandles[i].controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
                var direction = _midPointHandles[i].Direction;
                var position = center + 0.5f * sizeElements[i % 3] * (rotation * direction);
                var color = Vector3.Dot(localCameraPosition - position, direction) > 0f ? Handles.color : Handles.color * backFaceAlphaMultiplier;
                using (new Handles.DrawingScope(color))
                {
                    position = Handles.Slider(_midPointHandles[i].controlID, position, rotation * direction, handleSize * HandleUtility.GetHandleSize(position), Handles.DotHandleCap, 0.5f);
                }

                _midPointHandles[i].position = position;
            }
        }

        protected override void DrawWireframe(BoxCollider3D collider)
        {
            BoxCollider3DEditor.DrawWireframe(collider, handleColor);
        }

        protected override void UpdateProperties(BoxCollider3D collider)
        {
            // only allow positive scale
            var rotation = collider.shapeRotation;
            var lossyScale = collider.transform.lossyScale;
            var scales = new float[3] { Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.y), Mathf.Abs(lossyScale.z) };
            var size = collider.size;
            for (var i = 0; i < _midPointHandles.Length; ++i)
            {
                if (HandlesHelper.IsControlling(_midPointHandles[i].controlID))
                {
                    var position = _midPointHandles[i].position;
                    var direction = rotation * _midPointHandles[i].Direction;
                    var oppositePosition = _midPointHandles[(i + 3) % 6].position;
                    var distance = Vector3.Dot(position - oppositePosition, direction);
                    var scale = scales[i % 3];
                    var isValid = distance > 0f;
                    var translation = isValid ? 0.5f * (position + oppositePosition) : oppositePosition;
                    collider.shapeTranslation = Vector3.Scale(translation, InvertScaleVector(lossyScale));
                    size[i % 3] = isValid ? distance / scale : 0f;
                    collider.size = size;
                    break;
                }
            }
        }
    }
}
