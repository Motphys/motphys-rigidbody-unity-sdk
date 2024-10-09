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
    // Rendering code exclude from code coverage
    [UnityEngine.TestTools.ExcludeFromCoverage]
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CylinderCollider3D))]
    internal class CylinderCollider3DEditor : BaseColliderEditor
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        private static void DrawGizmo(CylinderCollider3D collider, GizmoType _)
        {
            DrawWireframe(collider, collider.enabled ? colliderBoundingEnableColor : colliderBoundingDisableColor);
        }

        [DrawGizmo(GizmoType.Selected)]
        private static void DrawEditorColliderIndex(CylinderCollider3D collider, GizmoType _)
        {
            DrawEditorColliderIndex(collider);
        }

        public static void DrawWireframe(CylinderCollider3D collider, Color color)
        {
            using (new Handles.DrawingScope(color, Matrix4x4.TRS(collider.transform.position, collider.transform.rotation, Vector3.one)))
            {
                var isOrthographic = Camera.current.orthographic;
                var lossyScale = collider.transform.lossyScale;
                var rotation = collider.shapeRotation;
                var up = rotation * Vector3.up;
                var right = rotation * Vector3.right;
                var forward = rotation * Vector3.forward;
                var maxScaleFactor = Mathf.Max(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.z));
                var center = Vector3.Scale(collider.shapeTranslation, lossyScale);
                var halfHeight = Mathf.Abs(0.5f * collider.height * lossyScale.y);
                var radius = collider.radius * maxScaleFactor;
                var topCenter = halfHeight > 0 ? center + halfHeight * up : center;
                var bottomCenter = halfHeight > 0 ? center + halfHeight * -up : center;
                var lightColor = color * backFaceAlphaMultiplier;

                if (isOrthographic)
                {
                    var localCameraForward = Handles.inverseMatrix.MultiplyVector(Camera.current.transform.forward);
                    var from = Vector3.Cross(up, localCameraForward);
                    var edgeDirection = Vector3.Cross(localCameraForward, bottomCenter - topCenter).normalized;
                    var isFacingTopCenter = Vector3.Dot(localCameraForward, up) < 0;
                    HandlesHelper.DrawTwoWireArc(isFacingTopCenter ? bottomCenter : topCenter, up, from, 180f, radius, color * backFaceAlphaMultiplier);
                    Handles.DrawWireDisc(isFacingTopCenter ? topCenter : bottomCenter, up, radius);
                    Handles.DrawLine(topCenter + radius * -edgeDirection, bottomCenter + radius * -edgeDirection);
                    Handles.DrawLine(topCenter + radius * edgeDirection, bottomCenter + radius * edgeDirection);
                }
                else
                {
                    var localCameraPosition = Handles.inverseMatrix.MultiplyPoint(Camera.current.transform.position);
                    var lookAtTopCenter = topCenter - localCameraPosition;
                    var lookAtBottomCenter = bottomCenter - localCameraPosition;
                    var distanceToTopCenter = lookAtTopCenter.magnitude;
                    var distanceToBottomCenter = lookAtBottomCenter.magnitude;
                    var isFacingTopCenter = Vector3.Dot(lookAtTopCenter, up) < 0;
                    var isFacingBottomCenter = Vector3.Dot(lookAtBottomCenter, -up) < 0;
                    var alpha1 = Mathf.Asin(radius / distanceToTopCenter);
                    var alpha2 = Mathf.Asin(radius / distanceToBottomCenter);
                    var from1 = Quaternion.AngleAxis(alpha1 * Mathf.Rad2Deg, up) * Vector3.Cross(up, lookAtTopCenter).normalized;
                    var from2 = Quaternion.AngleAxis(alpha2 * Mathf.Rad2Deg, -up) * Vector3.Cross(-up, lookAtBottomCenter).normalized;
                    HandlesHelper.DrawTwoWireArc(topCenter, up, from1, isFacingTopCenter ? 360f : 2 * (90f - alpha1 * Mathf.Rad2Deg), radius, lightColor);
                    HandlesHelper.DrawTwoWireArc(bottomCenter, -up, from2, isFacingBottomCenter ? 360f : 2 * (90f - alpha2 * Mathf.Rad2Deg), radius, lightColor);
                    Handles.DrawLine(topCenter + radius * from1, bottomCenter + radius * from1);
                    Handles.DrawLine(topCenter + radius * from2, bottomCenter + radius * from2);
                }

                Handles.color = lightColor;
                Handles.DrawLine(topCenter + radius * -right, bottomCenter + radius * -right);
                Handles.DrawLine(topCenter + radius * forward, bottomCenter + radius * forward);
                Handles.DrawLine(topCenter + radius * -forward, bottomCenter + radius * -forward);
                Handles.DrawLine(topCenter + radius * right, bottomCenter + radius * right);
            }
        }
    }

    // Rendering code exclude from code coverage
    [UnityEngine.TestTools.ExcludeFromCoverage]
    [EditorTool("Edit Cylinder Collider3D", typeof(CylinderCollider3D))]
    internal class CylinderCollider3DEditorTool : Collider3DEditorTool<CylinderCollider3D>
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

        protected override void DrawHandles(CylinderCollider3D collider)
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
                var direction = rotation * _midPointHandles[i].Direction;
                var position = center + sizeElements[i % 3] * direction;
                var color = Vector3.Dot(localCameraPosition - position, direction) > 0f ? Handles.color : Handles.color * backFaceAlphaMultiplier;
                using (new Handles.DrawingScope(color))
                {
                    position = Handles.Slider(_midPointHandles[i].controlID, position, direction, handleSize * HandleUtility.GetHandleSize(position), Handles.DotHandleCap, 0.5f);
                }

                _midPointHandles[i].position = position;
            }
        }

        protected override void DrawWireframe(CylinderCollider3D collider)
        {
            CylinderCollider3DEditor.DrawWireframe(collider, handleColor);
        }

        protected override void UpdateProperties(CylinderCollider3D collider)
        {
            // only allow positive scale
            var rotation = collider.shapeRotation;
            var lossyScale = collider.transform.lossyScale;
            var lossyScales = new float[] { lossyScale.x, lossyScale.y, lossyScale.z };
            var maxScaleFactor = Mathf.Max(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.z));
            for (var i = 0; i < _midPointHandles.Length; ++i)
            {
                if (HandlesHelper.IsControlling(_midPointHandles[i].controlID))
                {
                    var position = _midPointHandles[i].position;
                    var direction = rotation * _midPointHandles[i].Direction;
                    var oppositePosition = _midPointHandles[(i + 3) % 6].position;
                    var distance = Vector3.Dot(position - oppositePosition, direction);
                    var isValid = (i == 1 || i == 4) ? lossyScales[i % 3] > 0 ? distance > 0 : distance < 0 : distance > 0;
                    var translation = isValid ? 0.5f * (position + oppositePosition) : oppositePosition;
                    collider.shapeTranslation = Vector3.Scale(translation, InvertScaleVector(lossyScale));
                    // update height
                    if (i == 1 || i == 4)
                    {
                        collider.height = isValid ? Mathf.Abs(distance / lossyScale.y) : 0;
                    }
                    // update radius
                    else
                    {
                        collider.radius = isValid ? Mathf.Abs(0.5f * distance / maxScaleFactor) : 0;
                    }

                    break;
                }
            }
        }
    }
}
