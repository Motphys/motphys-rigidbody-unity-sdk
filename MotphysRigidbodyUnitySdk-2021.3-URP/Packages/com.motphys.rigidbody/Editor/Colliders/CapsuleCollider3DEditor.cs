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
    [CustomEditor(typeof(CapsuleCollider3D))]
    internal class CapsuleCollider3DEditor : BaseColliderEditor
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        private static void DrawGizmo(CapsuleCollider3D collider, GizmoType _)
        {
            DrawWireframe(collider, collider.enabled ? colliderBoundingEnableColor : colliderBoundingDisableColor);
        }

        [DrawGizmo(GizmoType.Selected)]
        private static void DrawEditorColliderIndex(CapsuleCollider3D collider, GizmoType _)
        {
            DrawEditorColliderIndex(collider);
        }

        public static void DrawWireframe(CapsuleCollider3D collider, Color color)
        {
            var lossyScale = collider.transform.lossyScale;
            var maxScaleFactor = Mathf.Max(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.z));
            var bodyMatrix = Matrix4x4.TRS(collider.transform.position, collider.transform.rotation, Vector3.one);
            var shapeMatrix = Matrix4x4.TRS(Vector3.Scale(lossyScale, collider.shapeTranslation), collider.shapeRotation, Vector3.one);
            var halfHeight = Mathf.Abs(0.5f * collider.height * lossyScale.y) - collider.radius * maxScaleFactor;
            var radius = collider.radius * maxScaleFactor;
            var topCenter = halfHeight > 0 ? halfHeight * Vector3.up : Vector3.zero;
            var bottomCenter = halfHeight > 0 ? halfHeight * Vector3.down : Vector3.zero;
            using (new Handles.DrawingScope(color, bodyMatrix * shapeMatrix))
            {
                Handles.DrawWireDisc(topCenter, Vector3.up, radius);
                Handles.DrawWireArc(topCenter, Vector3.left, Vector3.forward, 180f, radius);
                Handles.DrawWireArc(topCenter, Vector3.forward, Vector3.right, 180f, radius);
                Handles.DrawLine(topCenter + radius * Vector3.left, bottomCenter + radius * Vector3.left);
                Handles.DrawLine(topCenter + radius * Vector3.forward, bottomCenter + radius * Vector3.forward);
                Handles.DrawLine(topCenter + radius * Vector3.back, bottomCenter + radius * Vector3.back);
                Handles.DrawLine(topCenter + radius * Vector3.right, bottomCenter + radius * Vector3.right);
                Handles.DrawWireDisc(bottomCenter, Vector3.down, radius);
                Handles.DrawWireArc(bottomCenter, Vector3.left, Vector3.back, 180f, radius);
                Handles.DrawWireArc(bottomCenter, Vector3.forward, Vector3.left, 180f, radius);
            }
        }
    }

    [EditorTool("Edit Capsule Collider3D", typeof(CapsuleCollider3D))]
    internal class CapsuleCollider3DEditorTool : Collider3DEditorTool<CapsuleCollider3D>
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

        protected override void DrawHandles(CapsuleCollider3D collider)
        {
            var localCameraPosition = Handles.inverseMatrix.MultiplyPoint(Camera.current.transform.position);
            var lossyScale = collider.transform.lossyScale;
            var center = Vector3.Scale(collider.shapeTranslation, lossyScale);
            var rotation = collider.shapeRotation;
            var maxScaleFactor = Mathf.Max(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.z));
            var radius = collider.radius * maxScaleFactor;
            var sizeElements = new float[3] { radius, 0.5f * collider.height * lossyScale.y, radius };
            for (var i = 0; i < _midPointHandles.Length; ++i)
            {
                _midPointHandles[i].controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
                var direction = _midPointHandles[i].Direction;
                var position = center + Mathf.Max(sizeElements[i % 3], radius) * (rotation * direction);
                var color = Vector3.Dot(localCameraPosition - position, direction) > 0f ? Handles.color : Handles.color * backFaceAlphaMultiplier;
                using (new Handles.DrawingScope(color))
                {
                    position = Handles.Slider(_midPointHandles[i].controlID, position, rotation * direction, handleSize * HandleUtility.GetHandleSize(position), Handles.DotHandleCap, 0.5f);
                }

                _midPointHandles[i].position = position;
            }
        }

        protected override void DrawWireframe(CapsuleCollider3D collider)
        {
            CapsuleCollider3DEditor.DrawWireframe(collider, handleColor);
        }

        protected override void UpdateProperties(CapsuleCollider3D collider)
        {
            // only allow positive scale
            var rotation = collider.shapeRotation;
            var lossyScale = collider.transform.lossyScale;
            var maxScaleFactor = Mathf.Max(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.z));
            var validDistances = new float[3] { 0f, 2f * collider.radius * maxScaleFactor, 0f };
            for (var i = 0; i < _midPointHandles.Length; ++i)
            {
                if (HandlesHelper.IsControlling(_midPointHandles[i].controlID))
                {
                    var position = _midPointHandles[i].position;
                    var direction = rotation * _midPointHandles[i].Direction;
                    var oppositePosition = _midPointHandles[(i + 3) % 6].position;
                    var distance = Vector3.Dot(position - oppositePosition, direction);
                    var minDistance = validDistances[i % 3];
                    var isValid = distance > minDistance;
                    var translation = isValid ? 0.5f * (position + oppositePosition) : oppositePosition + 0.5f * minDistance * direction;
                    collider.shapeTranslation = Vector3.Scale(translation, InvertScaleVector(lossyScale));
                    // update height
                    if (i == 1 || i == 4)
                    {
                        collider.height = distance / lossyScale.y;
                    }
                    // update radius
                    else
                    {
                        collider.radius = isValid ? 0.5f * distance / maxScaleFactor : 0f;
                    }

                    break;
                }
            }
        }
    }
}
