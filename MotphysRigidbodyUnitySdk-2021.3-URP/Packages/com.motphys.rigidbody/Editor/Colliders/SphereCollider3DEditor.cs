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
    [CustomEditor(typeof(SphereCollider3D))]
    internal class SphereCollider3DEditor : BaseColliderEditor
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        private static void DrawGizmo(SphereCollider3D collider, GizmoType _)
        {
            var color = collider.enabled ? colliderBoundingEnableColor : colliderBoundingDisableColor;
            DrawWireframe(collider, color, true, color * backFaceAlphaMultiplier);
        }

        [DrawGizmo(GizmoType.Selected)]
        private static void DrawEditorColliderIndex(SphereCollider3D collider, GizmoType _)
        {
            DrawEditorColliderIndex(collider);
        }

        public static void DrawWireframe(SphereCollider3D collider, Color color, bool backfaceLighter, Color backfaceColor)
        {
            using (new Handles.DrawingScope(color, Matrix4x4.TRS(collider.transform.position, collider.transform.rotation, Vector3.one)))
            {
                var lossyScale = collider.transform.lossyScale;
                var isOrthographic = Camera.current.orthographic;
                var center = Vector3.Scale(collider.shapeTranslation, lossyScale);
                var rotation = collider.shapeRotation;
                var radius = collider.radius * Mathf.Max(lossyScale.x, lossyScale.y, lossyScale.z);
                var cameraPosition = Handles.inverseMatrix.MultiplyPoint(Camera.current.transform.position);
                var cameraForward = Handles.inverseMatrix.MultiplyVector(Camera.current.transform.forward);
                var lookSphere = center - cameraPosition;
                var distanceToSphere = lookSphere.magnitude;
                var directions = new Vector3[3] { Vector3.right, Vector3.up, Vector3.forward };
                var normals = new Vector3[3] { rotation * directions[0], rotation * directions[1], rotation * directions[2] };

                if (isOrthographic)
                {
                    Handles.DrawWireDisc(center, cameraForward, radius);
                    if (backfaceLighter)
                    {
                        for (var i = 0; i < 3; i++)
                        {
                            var from = Vector3.Cross(normals[i], cameraForward);
                            HandlesHelper.DrawTwoWireArc(center, normals[i], from, 180f, radius, backfaceColor);
                        }
                    }
                }
                else
                {
                    // alpha = angle between lookSphere and tangent line (from camera to sphere)
                    var alpha = Mathf.Asin(radius / distanceToSphere);
                    var lookRadius = distanceToSphere * Mathf.Tan(alpha);
                    Handles.DrawWireDisc(center, lookSphere, lookRadius);
                    if (backfaceLighter)
                    {
                        for (var i = 0; i < 3; i++)
                        {
                            var from = Quaternion.AngleAxis(alpha * Mathf.Rad2Deg, normals[i]) * Vector3.Cross(normals[i], lookSphere);
                            HandlesHelper.DrawTwoWireArc(center, normals[i], from, 2 * (90f - alpha * Mathf.Rad2Deg), radius, backfaceColor);
                        }
                    }
                }

                if (!backfaceLighter)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        Handles.DrawWireDisc(center, normals[i], radius);
                    }
                }
            }
        }
    }

    [EditorTool("Edit Sphere Collider3D", typeof(SphereCollider3D))]
    internal class SphereCollider3DEditorTool : Collider3DEditorTool<SphereCollider3D>
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

        protected override void DrawHandles(SphereCollider3D collider)
        {
            var localCameraPosition = Handles.inverseMatrix.MultiplyPoint(Camera.current.transform.position);
            var lossyScale = collider.transform.lossyScale;
            var center = Vector3.Scale(collider.shapeTranslation, lossyScale);
            var rotation = collider.shapeRotation;
            var maxScale = Mathf.Max(lossyScale.x, lossyScale.y, lossyScale.z);
            for (var i = 0; i < _midPointHandles.Length; ++i)
            {
                _midPointHandles[i].controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
                var direction = _midPointHandles[i].Direction;
                var position = center + collider.radius * maxScale * (rotation * direction);
                var color = Vector3.Dot(localCameraPosition - position, direction) > 0f ? Handles.color : Handles.color * backFaceAlphaMultiplier;
                using (new Handles.DrawingScope(color))
                {
                    position = Handles.Slider(_midPointHandles[i].controlID, position, rotation * direction, handleSize * HandleUtility.GetHandleSize(position), Handles.DotHandleCap, 0.5f);
                }

                _midPointHandles[i].position = position;
            }
        }

        protected override void DrawWireframe(SphereCollider3D collider)
        {
            SphereCollider3DEditor.DrawWireframe(collider, handleColor, false, Color.white);
        }

        protected override void UpdateProperties(SphereCollider3D collider)
        {
            // only allow positive scale
            var rotation = collider.shapeRotation;
            var lossyScale = collider.transform.lossyScale;
            for (var i = 0; i < _midPointHandles.Length; ++i)
            {
                if (HandlesHelper.IsControlling(_midPointHandles[i].controlID))
                {
                    var position = _midPointHandles[i].position;
                    var direction = rotation * _midPointHandles[i].Direction;
                    var oppositePosition = _midPointHandles[(i + 3) % 6].position;
                    var distance = Vector3.Dot(position - oppositePosition, direction);
                    var maxScale = GetRadiusScaleFactor(lossyScale);
                    var isValid = distance > 0f;
                    var translation = isValid ? 0.5f * (position + oppositePosition) : oppositePosition;
                    collider.shapeTranslation = Vector3.Scale(translation, InvertScaleVector(lossyScale));
                    collider.radius = Mathf.Approximately(maxScale, 0f) ? 0f : isValid ? 0.5f * (Vector3.Dot(position - oppositePosition, direction) / maxScale) : 0f;
                    break;
                }
            }
        }

        private float GetRadiusScaleFactor(Vector3 lossyScale)
        {
            var x = Mathf.Abs(lossyScale.x);
            var y = Mathf.Abs(lossyScale.y);
            var z = Mathf.Abs(lossyScale.z);
            return Mathf.Max(x, y, z);
        }
    }
}
