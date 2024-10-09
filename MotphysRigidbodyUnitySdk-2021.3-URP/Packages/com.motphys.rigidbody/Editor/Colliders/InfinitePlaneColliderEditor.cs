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
    // Rendering code exclude from test coverage
    [UnityEngine.TestTools.ExcludeFromCoverage]
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InfinitePlaneCollider3D))]
    internal class InfinitePlaneColliderEditor : BaseColliderEditor
    {
        private static Color s_backfaceEnableColor = new Color(0f, 0f, 1f, 0.4f);
        private static float s_maxGridCount = 200f;
        private static Color s_backfaceDisableColor = new Color(0f, 0f, 1f, 0.2f);
        private static float s_maxSmallGridCount = 100f;

        protected override void DrawDynamicScale() { }

        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        private static void DrawGizmo(InfinitePlaneCollider3D collider, GizmoType _)
        {
            DrawWireframe(collider, collider.enabled ? colliderBoundingEnableColor : colliderBoundingDisableColor, collider.enabled ? s_backfaceEnableColor : s_backfaceDisableColor);
        }

        [DrawGizmo(GizmoType.Selected)]
        private static void DrawEditorColliderIndex(InfinitePlaneCollider3D collider, GizmoType _)
        {
            DrawEditorColliderIndex(collider);
        }

        public static void DrawWireframe(InfinitePlaneCollider3D collider, Color frontColor, Color backfaceColor)
        {
            using (new Handles.DrawingScope(Matrix4x4.TRS(collider.transform.position, collider.transform.rotation, Vector3.one)))
            {
                var zTestFunc = Handles.zTest;
                Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

                var center = Vector3.Scale(collider.shapeTranslation, collider.transform.lossyScale);
                var rotation = collider.shapeRotation;
                var lookAt = center - Handles.inverseMatrix.MultiplyPoint(Camera.current.transform.position);
                var color = Vector3.Dot(lookAt, rotation * Vector3.up) < 0 ? frontColor : backfaceColor;
                var distance = lookAt.magnitude;
                var maxDrawDistance = distance < 10 ? 100 : 10f * distance;
                Handles.color = color;
                Handles.DrawLine(center + rotation * new Vector3(-maxDrawDistance, 0, 0), center + rotation * new Vector3(maxDrawDistance, 0, 0));
                Handles.DrawLine(center + rotation * new Vector3(0, 0, -maxDrawDistance), center + rotation * new Vector3(0, 0, maxDrawDistance));
                var scale = Mathf.Log10(distance * 7);
                var gridSize = Mathf.Max(1f, Mathf.Pow(10, Mathf.Floor(scale) - 1));
                var bigGridSize = 10f * gridSize;
                var colorAlpha = scale > 1 ? 1 - scale + Mathf.Floor(scale) : 1;
                var offset = 0f;
                var currentGridCount = 0;
                var currentMaxDrawDistance = maxDrawDistance;
                float currentColorAlpha;
                float distanceColorAlpha;
                while (currentGridCount < s_maxGridCount && offset < maxDrawDistance)
                {
                    offset += currentGridCount < s_maxSmallGridCount ? gridSize : bigGridSize;

                    distanceColorAlpha = 1 - offset / currentMaxDrawDistance;
                    if (distanceColorAlpha < 0.05f)
                    {
                        break;
                    }

                    currentColorAlpha = color.a * (offset % bigGridSize == 0 ? 1 : colorAlpha * distanceColorAlpha);

                    Handles.color = new Color(color.r, color.g, color.b, currentColorAlpha);
                    Handles.DrawLine(center + rotation * new Vector3(-currentMaxDrawDistance, 0, offset), center + rotation * new Vector3(currentMaxDrawDistance, 0, offset));
                    Handles.DrawLine(center + rotation * new Vector3(-currentMaxDrawDistance, 0, -offset), center + rotation * new Vector3(currentMaxDrawDistance, 0, -offset));
                    Handles.DrawLine(center + rotation * new Vector3(offset, 0, -currentMaxDrawDistance), center + rotation * new Vector3(offset, 0, currentMaxDrawDistance));
                    Handles.DrawLine(center + rotation * new Vector3(-offset, 0, -currentMaxDrawDistance), center + rotation * new Vector3(-offset, 0, currentMaxDrawDistance));

                    currentGridCount++;
                }

                Handles.zTest = zTestFunc;
            }
        }
    }

    // Rendering code exclude from test coverage
    [UnityEngine.TestTools.ExcludeFromCoverage]
    [EditorTool("Edit Infinite Plane Collider 3D", typeof(InfinitePlaneCollider3D))]
    internal class InfinitePlaneCollider3DEditorTool : Collider3DEditorTool<InfinitePlaneCollider3D>
    {
        private const float ArrowHandleSize = 1f;
        private const float SphereCapSize = 0.2f;

        private MidPointHandle[] _midPointHandles = new MidPointHandle[2]
        {
            new MidPointHandle(Vector3.up),
            new MidPointHandle(Vector3.down),
        };

        protected override void DrawHandles(InfinitePlaneCollider3D collider)
        {
            var lossyScale = collider.transform.lossyScale;
            var center = Vector3.Scale(lossyScale, collider.shapeTranslation);
            var rotation = collider.shapeRotation;
            for (var i = 0; i < _midPointHandles.Length; ++i)
            {
                _midPointHandles[i].controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
                var direction = rotation * _midPointHandles[i].Direction;
                _midPointHandles[i].position = Handles.Slider(_midPointHandles[i].controlID, center, direction, HandleUtility.GetHandleSize(center) * ArrowHandleSize, Handles.ArrowHandleCap, 0f);
            }
        }

        protected override void DrawWireframe(InfinitePlaneCollider3D collider)
        {
            using (new Handles.DrawingScope(Matrix4x4.TRS(collider.transform.position, collider.transform.rotation, Vector3.one)))
            {
                var center = Vector3.Scale(collider.shapeTranslation, collider.transform.lossyScale);
                Handles.SphereHandleCap(0, center, Quaternion.identity, HandleUtility.GetHandleSize(center) * SphereCapSize, EventType.Repaint);
            }
        }

        protected override void UpdateProperties(InfinitePlaneCollider3D collider)
        {
            for (var i = 0; i < _midPointHandles.Length; ++i)
            {
                if (HandlesHelper.IsControlling(_midPointHandles[i].controlID))
                {
                    collider.shapeTranslation = Vector3.Scale(_midPointHandles[i].position, InvertScaleVector(collider.transform.lossyScale));
                    break;
                }
            }
        }
    }
}
