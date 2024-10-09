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
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Motphys.Rigidbody.Editor
{
    internal class HandlesHelper
    {
        private struct PositionHandleID
        {
            public readonly int XAxis;
            public readonly int YAxis;
            public readonly int ZAxis;
            public readonly int XYPlane;
            public readonly int YZPlane;
            public readonly int XZPlane;
            public static PositionHandleID defaultValue => new PositionHandleID(
                GUIUtility.GetControlID("XAxis".GetHashCode(), FocusType.Passive),
                GUIUtility.GetControlID("YAxis".GetHashCode(), FocusType.Passive),
                GUIUtility.GetControlID("ZAxis".GetHashCode(), FocusType.Passive),
                GUIUtility.GetControlID("XYPlane".GetHashCode(), FocusType.Passive),
                GUIUtility.GetControlID("YZPlane".GetHashCode(), FocusType.Passive),
                GUIUtility.GetControlID("XZPlane".GetHashCode(), FocusType.Passive)
            );
            public PositionHandleID(int xAxis, int yAxis, int zAxis, int xyPlane, int yzPlane, int xzPlane)
            {
                XAxis = xAxis;
                YAxis = yAxis;
                ZAxis = zAxis;
                XYPlane = xyPlane;
                YZPlane = yzPlane;
                XZPlane = xzPlane;
            }
            public int[] all => new int[] { XAxis, YAxis, ZAxis, XYPlane, YZPlane, XZPlane };
            public int[] xRelated => new int[] { XAxis, XYPlane, XZPlane };
            public int[] yRelated => new int[] { YAxis, XYPlane, YZPlane };
            public int[] zRelated => new int[] { ZAxis, XZPlane, YZPlane };
        }
        private struct RotationHandleID
        {
            public readonly int X;
            public readonly int Y;
            public readonly int Z;
            public readonly int XYZ;
            public static RotationHandleID defaultValue => new RotationHandleID(
                GUIUtility.GetControlID("XRotation".GetHashCode(), FocusType.Passive),
                GUIUtility.GetControlID("YRotation".GetHashCode(), FocusType.Passive),
                GUIUtility.GetControlID("ZRotation".GetHashCode(), FocusType.Passive),
                GUIUtility.GetControlID("XYZRotation".GetHashCode(), FocusType.Passive)
            );
            public RotationHandleID(int x, int y, int z, int xyz)
            {
                X = x;
                Y = y;
                Z = z;
                XYZ = xyz;
            }
            public int[] all => new int[] { X, Y, Z, XYZ };
        }
        private static Vector3 s_rotateStartPosition;
        private static float s_rotationDistance;
        private static Quaternion s_rotateStartRotation;
        private static Vector3 s_rotateStartAxis;
        private static Vector2 s_currentMousePosition;
        private static Vector2 s_startMousePosition;
        private static Vector3[] s_verts = new Vector3[4]
        {
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero
        };
        private static Color s_axisXColor = new Color(1f, 0.6f, 0.6f, 1f);
        private static Color s_axisYColor = new Color(0.6f, 1f, 0.6f, 1f);
        private static Color s_axisZColor = new Color(0.6f, 0.6f, 1f, 1f);
        private static Color s_uncontrollableAxisXColor = new Color(1f, 0.6f, 0.6f, 0.4f);
        private static Color s_uncontrollableAxisYColor = new Color(0.6f, 1f, 0.6f, 0.4f);
        private static Color s_uncontrollableAxisZColor = new Color(0.6f, 0.6f, 1f, 0.4f);
        private static Color s_disableHandleColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        private static Color s_selectedColor = new Color(82f / 85f, 242f / 255f, 10f / 51f, 0.89f);
        private static Color s_unselectedColor = new Color(0f, 0f, 0f, 0.3f);
        private static float s_hoverExtraThickness = 1f;

        internal static Vector3 PositionHandle(Vector3 position, Quaternion rotation, float axisSize = 0.5f, float planeSize = 0.1f)
        {
            var handleID = PositionHandleID.defaultValue;

            position = AxesPositionHandle(handleID, position, rotation, axisSize);

            position = PlanesPositionHandle(handleID, position, rotation, planeSize);

            return position;
        }

        internal static void UncontrollablePositionHandle(Vector3 position, Quaternion rotation, float axisSize = 0.5f)
        {
            var handleID = PositionHandleID.defaultValue;
            AxesUncontrollablePositionHandle(handleID, position, rotation, axisSize);
        }

        internal static Quaternion RotationHandle(Vector3 position, Quaternion rotation, float size = 0.5f)
        {
            var handleID = RotationHandleID.defaultValue;

            rotation = CirclesRotationHandle(handleID, position, rotation, size);

            return rotation;
        }

        private static Vector3 AxesPositionHandle(PositionHandleID handleID, Vector3 position, Quaternion rotation, float size)
        {
            var preColor = Handles.color;

            size *= HandleUtility.GetHandleSize(position);

            Handles.color = IsControlling(handleID.all) ? s_disableHandleColor : s_axisXColor;
            Handles.color = IsControlling(handleID.xRelated) ? s_selectedColor : Handles.color;
            Handles.color = ToActiveColorSpace(Handles.color);
            position = SliderHandle(handleID.XAxis, position, rotation * Vector3.right, size);

            Handles.color = IsControlling(handleID.all) ? s_disableHandleColor : s_axisYColor;
            Handles.color = IsControlling(handleID.yRelated) ? s_selectedColor : Handles.color;
            Handles.color = ToActiveColorSpace(Handles.color);
            position = SliderHandle(handleID.YAxis, position, rotation * Vector3.up, size);

            Handles.color = IsControlling(handleID.all) ? s_disableHandleColor : s_axisZColor;
            Handles.color = IsControlling(handleID.zRelated) ? s_selectedColor : Handles.color;
            Handles.color = ToActiveColorSpace(Handles.color);
            position = SliderHandle(handleID.ZAxis, position, rotation * Vector3.forward, size);

            Handles.color = preColor;

            return position;
        }

        private static void AxesUncontrollablePositionHandle(PositionHandleID handleID, Vector3 position, Quaternion rotation, float size)
        {
            var preColor = Handles.color;

            size *= HandleUtility.GetHandleSize(position);

            Handles.color = s_uncontrollableAxisXColor;
            Handles.ArrowHandleCap(handleID.XAxis, position, Quaternion.LookRotation(rotation * Vector3.right), size, EventType.Repaint);

            Handles.color = s_uncontrollableAxisYColor;
            Handles.ArrowHandleCap(handleID.YAxis, position, Quaternion.LookRotation(rotation * Vector3.up), size, EventType.Repaint);

            Handles.color = s_uncontrollableAxisZColor;
            Handles.ArrowHandleCap(handleID.ZAxis, position, Quaternion.LookRotation(rotation * Vector3.forward), size, EventType.Repaint);

            Handles.color = preColor;
        }
        private static Vector3 PlanesPositionHandle(PositionHandleID handleID, Vector3 position, Quaternion rotation, float size)
        {
            var hoveringAlpha = 0.5f;
            var notHoveringAlpha = 0.1f;

            var preColor = Handles.color;
            size *= HandleUtility.GetHandleSize(position);

            // YOZ Plane
            Handles.color = IsControlling(handleID.all) ? Color.clear : Handles.xAxisColor;
            var offsetX = rotation * new Vector3(0, size, size);
            position = Slider2DHandle(handleID.YZPlane, position, offsetX, rotation * Vector3.right, rotation * Vector3.forward, rotation * Vector3.up, size);
            s_verts[0] = position;
            s_verts[1] = position + rotation * new Vector3(0, size * 2, 0);
            s_verts[2] = position + offsetX * 2;
            s_verts[3] = position + rotation * new Vector3(0, 0, size * 2);
            var yOzColor = Handles.xAxisColor;
            yOzColor.a = IsHovering(handleID.YZPlane) ? hoveringAlpha : notHoveringAlpha;
            yOzColor = IsControlling(handleID.YZPlane) ? s_selectedColor : IsControllingState() ? Color.clear : yOzColor;
            yOzColor = ToActiveColorSpace(yOzColor);
            Handles.color = yOzColor;
            Handles.DrawSolidRectangleWithOutline(s_verts, yOzColor, Color.clear);

            // XOZ Plane
            Handles.color = IsControlling(handleID.all) ? Color.clear : Handles.yAxisColor;
            var offsetY = rotation * new Vector3(size, 0, size);
            position = Slider2DHandle(handleID.XZPlane, position, offsetY, rotation * Vector3.up, rotation * Vector3.forward, rotation * Vector3.right, size);
            s_verts[0] = position;
            s_verts[1] = position + rotation * new Vector3(size * 2, 0, 0);
            s_verts[2] = position + offsetY * 2;
            s_verts[3] = position + rotation * new Vector3(0, 0, size * 2);
            var xOzColor = Handles.yAxisColor;
            xOzColor.a = IsHovering(handleID.XZPlane) ? hoveringAlpha : notHoveringAlpha;
            xOzColor = IsControlling(handleID.XZPlane) ? s_selectedColor : IsControllingState() ? Color.clear : xOzColor;
            xOzColor = ToActiveColorSpace(xOzColor);
            Handles.color = xOzColor;
            Handles.DrawSolidRectangleWithOutline(s_verts, xOzColor, Color.clear);

            // XOY Plane
            Handles.color = IsControlling(handleID.all) ? Color.clear : Handles.zAxisColor;
            var offsetZ = rotation * new Vector3(size, size, 0);
            position = Slider2DHandle(handleID.XYPlane, position, offsetZ, rotation * Vector3.forward, rotation * Vector3.right, rotation * Vector3.up, size);
            s_verts[0] = position;
            s_verts[1] = position + rotation * new Vector3(size * 2, 0, 0);
            s_verts[2] = position + offsetZ * 2;
            s_verts[3] = position + rotation * new Vector3(0, size * 2, 0);
            var xOyColor = Handles.zAxisColor;
            xOyColor.a = IsHovering(handleID.XYPlane) ? hoveringAlpha : notHoveringAlpha;
            xOyColor = IsControlling(handleID.XYPlane) ? s_selectedColor : IsControllingState() ? Color.clear : xOyColor;
            xOyColor = ToActiveColorSpace(xOyColor);
            Handles.color = xOyColor;
            Handles.DrawSolidRectangleWithOutline(s_verts, xOyColor, Color.clear);

            Handles.color = preColor;
            return position;
        }

        private static Quaternion CirclesRotationHandle(RotationHandleID handleID, Vector3 position, Quaternion rotation, float size)
        {
            size *= HandleUtility.GetHandleSize(position);
            var preColor = Handles.color;

            Handles.color = IsControlling(handleID.XYZ) ? s_selectedColor : s_unselectedColor;
            rotation = Handles.FreeRotateHandle(handleID.XYZ, rotation, position, size);

            var snapCount = 24f;

            Handles.color = ToActiveColorSpace(s_axisXColor);
            rotation = CircleRotationHandle(handleID.X, rotation, position, rotation * Vector3.right, size, snapCount);

            Handles.color = ToActiveColorSpace(s_axisYColor);
            rotation = CircleRotationHandle(handleID.Y, rotation, position, rotation * Vector3.up, size, snapCount);

            Handles.color = ToActiveColorSpace(s_axisZColor);
            rotation = CircleRotationHandle(handleID.Z, rotation, position, rotation * Vector3.forward, size, snapCount);

            Handles.color = preColor;
            return rotation;
        }
        private static Vector3 SliderHandle(int controlID, Vector3 position, Vector3 direction, float size)
        {
            position = Handles.Slider(controlID, position, direction, size, Handles.ArrowHandleCap, -1);
            return position;
        }

        private static Vector3 Slider2DHandle(int controlID, Vector3 position, Vector3 offset, Vector3 handleDir, Vector3 sliderDir1, Vector3 slideDir2, float size)
        {
            position = Handles.Slider2D(controlID, position, offset, handleDir, sliderDir1, slideDir2, size, Handles.RectangleHandleCap, Vector2.zero);
            return position;
        }
        private static bool IsHovering(int controlID)
        {
            return controlID == HandleUtility.nearestControl && GUIUtility.hotControl == 0;
        }
        private static Color ToActiveColorSpace(Color color)
        {
            return (QualitySettings.activeColorSpace == ColorSpace.Linear) ? color.linear : color;
        }
        public static bool IsControllingState()
        {
            return !IsControlling(0);
        }
        public static bool IsControlling(params int[] controlIDs)
        {
            foreach (var id in controlIDs)
            {
                if (id == GUIUtility.hotControl)
                {
                    return true;
                }
            }

            return false;
        }

        public static Quaternion CircleRotationHandle(int id, Quaternion rotation, Vector3 position, Vector3 axis, float size, float snapCount)
        {
            var preColor = Handles.color;
            var vector = Handles.inverseMatrix.MultiplyVector(Camera.current.transform.forward);
            var rotateStartDirection = (s_rotateStartPosition - position).normalized;
            var current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.Layout:
                {
                    var distance = HandleUtility.DistanceToArc(position, axis, Vector3.Cross(axis, vector).normalized, 180f, size) * 0.3f;
                    HandleUtility.AddControl(id, distance);
                    break;
                }
                case EventType.MouseDown:
                    if (HandleUtility.nearestControl == id && current.button == 0 && !current.alt)
                    {
                        GUIUtility.hotControl = id;
                        s_rotateStartPosition = HandleUtility.ClosestPointToArc(position, axis, Vector3.Cross(axis, vector).normalized, 180f, size);

                        s_rotationDistance = 0f;
                        s_rotateStartRotation = rotation;
                        s_rotateStartAxis = axis;
                        s_startMousePosition = Event.current.mousePosition;
                        s_currentMousePosition = Event.current.mousePosition;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }

                    break;
                case EventType.MouseDrag:
                    if (!IsControlling(id) || current.alt)
                    {
                        break;
                    }

                    s_currentMousePosition += current.delta;
                    s_rotationDistance = HandleUtility.CalcLineTranslation(s_startMousePosition, s_currentMousePosition, s_rotateStartPosition, Vector3.Cross(rotateStartDirection, axis).normalized) / size * 30f;
                    s_rotationDistance = Handles.SnapValue(s_rotationDistance, 360f / snapCount);
                    rotation = Quaternion.AngleAxis(-s_rotationDistance, s_rotateStartAxis) * s_rotateStartRotation;

                    GUI.changed = true;
                    current.Use();
                    break;
                case EventType.Repaint:
                {
                    if (IsControlling(id))
                    {
                        Handles.color = s_selectedColor;
                        Handles.DrawLine(position, position + rotateStartDirection * size);
                        var angle = -Mathf.Sign(s_rotationDistance) * Mathf.Repeat(Mathf.Abs(s_rotationDistance), 360f);
                        var rotateEndDirection = Quaternion.AngleAxis(angle, axis) * rotateStartDirection;
                        Handles.DrawLine(position, position + rotateEndDirection * size);
                        Handles.color = s_selectedColor * new Color(1f, 1f, 1f, 0.2f);
                        var fullCircleCount = (int)Mathf.Abs(s_rotationDistance / 360f);
                        for (var i = 0; i < fullCircleCount; i++)
                        {
                            Handles.DrawSolidDisc(position, axis, size);
                        }

                        Handles.DrawSolidArc(position, axis, rotateStartDirection, angle, size);
                        if (EditorGUI.actionKey)
                        {
                            DrawRotationUnitSnapMarkers(position, axis, size, 0.1f, snapCount, rotateStartDirection);
                            DrawRotationUnitSnapMarkers(position, axis, size, 0.2f, 8, rotateStartDirection);
                        }

                        Handles.color = s_selectedColor;
                    }

                    var thickness = IsHovering(id) ? Handles.lineThickness + s_hoverExtraThickness : Handles.lineThickness;

                    if (IsControlling(id))
                    {
                        Handles.DrawWireDisc(position, axis, size, thickness);
                    }
                    else
                    {
                        Handles.DrawWireArc(position, axis, Vector3.Cross(axis, vector).normalized, 180f, size, thickness);
                    }

                    break;
                }
            }

            Handles.color = preColor;
            return rotation;
        }
        private static void DrawRotationUnitSnapMarkers(Vector3 position, Vector3 axis, float handleSize, float markerSize, float snapCount, Vector3 from)
        {
            var snapDegree = 360f / snapCount;
            for (var i = 0; i < snapCount; i++)
            {
                var quaternion = Quaternion.AngleAxis((float)i * snapDegree, axis);
                var vector = quaternion * from;
                var p = position + (1f - markerSize) * handleSize * vector;
                var p2 = position + 1f * handleSize * vector;
                Handles.color = s_selectedColor;
                Handles.DrawLine(p, p2);
            }
        }

        internal static void DrawTwoWireArc(Vector3 position, Vector3 normal, Vector3 from, float firstDegree, float radius, Color lightColor)
        {
            Handles.DrawWireArc(position, normal, from, firstDegree, radius);
            using (new Handles.DrawingScope(lightColor))
            {
                Handles.DrawWireArc(position, normal, from, firstDegree - 360f, radius);
            }
        }

        public static Matrix4x4 ExtractTRMatrix(Matrix4x4 matrix)
        {
            var position = matrix.GetPosition();
            var rotation = matrix.rotation;
            return Matrix4x4.TRS(position, rotation, Vector3.one);
        }

        public static Matrix4x4 GetAnchorToWorldMatrix(BaseJoint joint)
        {
            if (joint == null)
            {
                return Matrix4x4.identity;
            }

            var localToWorld = HandlesHelper.ExtractTRMatrix(joint.transform.localToWorldMatrix);
            var lossyScale = joint.transform.lossyScale;
            var localAnchorPos = Vector3.Scale(joint.anchorPosition, lossyScale);
            var anchorToLocal = Matrix4x4.TRS(localAnchorPos, joint.anchorFrame.rotation, Vector3.one);
            return localToWorld * anchorToLocal;
        }

        public static void DrawEllipsoidCone(EllipsoidHandleConfig config)
        {

            var preColor = Handles.color;
            var color = Color.cyan;
            color.a = 0.2f;
            Handles.color = color;
            for (var i = 0; i < config.segCount; i++)
            {
                var p0 = config.GetPoint(i);
                var p1 = config.GetPoint(i + 1);

                Handles.DrawAAConvexPolygon(Vector3.zero, p0, p1);

                Handles.color = Color.black;
                Handles.DrawLine(p0, p1, 2.0f);
                Handles.color = color;
            }

            Handles.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);

            var yColor = Handles.yAxisColor;
            var zColor = Handles.zAxisColor;
            zColor.a = 0.3f;
            yColor.a = 0.3f;
            Handles.color = yColor;
            Handles.DrawSolidArc(Vector3.zero, Vector3.up, config.GetPoint(0f), -config.angleYLimit.span, config.scale);
            Handles.color = zColor;
            Handles.DrawSolidArc(Vector3.zero, Vector3.forward, config.GetPoint(0.5f * Mathf.PI), -config.angleZLimit.span, config.scale);
            Handles.color = preColor;
        }

        public struct EllipsoidHandleConfig
        {
            public JointAngularLimit angleYLimit;
            public JointAngularLimit angleZLimit;
            private int _segCount;
            private float _scale;

            public EllipsoidHandleConfig(JointAngularLimit angleYLimit, JointAngularLimit angleZLimit)
            {
                this.angleYLimit = angleYLimit;
                this.angleZLimit = angleZLimit;
                _segCount = 100;
                _scale = 1.0f;
            }

            public int segCount
            {
                get
                {
                    return _segCount;
                }
                set
                {
                    _segCount = Mathf.Max(3, value);
                }
            }

            private Vector2 CalculateSemi(float theta)
            {
                if (theta < Mathf.PI * 0.5)
                {
                    return new Vector2(angleYLimit.high, angleZLimit.high) * Mathf.Deg2Rad;
                }
                else if (theta < Mathf.PI)
                {
                    return new Vector2(-angleYLimit.low, angleZLimit.high) * Mathf.Deg2Rad;
                }
                else if (theta < Mathf.PI * 1.5)
                {
                    return new Vector2(-angleYLimit.low, -angleZLimit.low) * Mathf.Deg2Rad;
                }
                else
                {
                    return new Vector2(angleYLimit.high, -angleZLimit.low) * Mathf.Deg2Rad;
                }
            }
            public Vector3 GetPoint(int index)
            {
                var deltaTheta = 2 * Mathf.PI / segCount;
                var theta = index * deltaTheta;
                return GetPoint(theta);
            }

            public Vector3 GetPoint(float theta)
            {
                var semiRadius = CalculateSemi(theta);
                var a0 = Mathf.Cos(theta) * semiRadius.x;
                var b0 = Mathf.Sin(theta) * semiRadius.y;
                var angle = Mathf.Sqrt(a0 * a0 + b0 * b0);
                var rot0 = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, new Vector3(0f, a0, b0));
                return rot0 * Vector3.right * _scale;
            }

            public float scale
            {
                get
                {
                    return _scale;
                }
                set
                {
                    _scale = Mathf.Max(0.01f, value);
                }
            }
        }
    }

    internal class ConeLimitDrawer
    {
        private static readonly Matrix4x4 s_rotationOffset_0 =
           Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.up), Vector3.one);

        private static readonly Matrix4x4 s_rotationOffset_1 =
            Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.forward), Vector3.one);

        private static readonly Matrix4x4 s_rotationOffset_2 =
            Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.right), Vector3.one);

        private static readonly Matrix4x4 s_rotationOffset_3 =
            Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(180, Vector3.right), Vector3.one);

        //twist handle
        private ArcHandle _twistHandle_min;
        private ArcHandle _twistHandle_max;

        //swing 1 handle
        private ArcHandle _swingHandle_min_1;
        private ArcHandle _swingHandle_max_1;

        //swing 2 handle
        private ArcHandle _swingHandle_min_2;
        private ArcHandle _swingHandle_max_2;

        public float twistLimitLow => _twistHandle_min.angle;
        public float twistLimitHigh => _twistHandle_max.angle;

        public float swingLimitLow_1 => _swingHandle_min_1.angle;
        public float swingLimitHigh_1 => _swingHandle_max_1.angle;

        public float swingLimitLow_2 => _swingHandle_min_2.angle;
        public float swingLimitHigh_2 => _swingHandle_max_2.angle;

        public ConeLimitDrawer()
        {
            _swingHandle_min_1 = new ArcHandle();
            _swingHandle_max_1 = new ArcHandle();
            _swingHandle_min_2 = new ArcHandle();
            _swingHandle_max_2 = new ArcHandle();
            _twistHandle_min = new ArcHandle();
            _twistHandle_max = new ArcHandle();

            _swingHandle_min_1.fillColor = Color.clear;
            _swingHandle_max_1.fillColor = Color.clear;
            _swingHandle_min_1.wireframeColor = Color.clear;
            _swingHandle_max_1.wireframeColor = Color.clear;

            _swingHandle_min_2.fillColor = Color.clear;
            _swingHandle_max_2.fillColor = Color.clear;
            _swingHandle_min_2.wireframeColor = Color.clear;
            _swingHandle_max_2.wireframeColor = Color.clear;

            _twistHandle_min.fillColor = Color.clear;
            _twistHandle_max.fillColor = Color.clear;
            _twistHandle_min.wireframeColor = Color.clear;
            _twistHandle_max.wireframeColor = Color.clear;
        }

        public bool DrawTwist(float low, float high, float radius, Matrix4x4 anchorToWorld)
        {
            var color = Handles.xAxisColor;
            using (new Handles.DrawingScope(color, anchorToWorld * s_rotationOffset_1 * s_rotationOffset_3 * s_rotationOffset_0))
            {
                _twistHandle_min.angle = low;
                _twistHandle_max.angle = high;
                _twistHandle_max.radius = radius;
                _twistHandle_min.radius = radius;

                EditorGUI.BeginChangeCheck();
                _twistHandle_min.DrawHandle();
                _twistHandle_max.DrawHandle();
                if (EditorGUI.EndChangeCheck())
                {
                    _twistHandle_min.angle = Mathf.Clamp(_twistHandle_min.angle, -180, _twistHandle_max.angle);
                    _twistHandle_max.angle = Mathf.Clamp(_twistHandle_max.angle, _twistHandle_min.angle, 180);

                    return true;
                }

                return false;
            }
        }

        public bool DrawSwing1(float low, float high, float radius, Matrix4x4 anchorToWorld)
        {
            var color = Handles.yAxisColor;
            using (new Handles.DrawingScope(color, anchorToWorld * s_rotationOffset_0))
            {
                _swingHandle_min_1.angle = low;
                _swingHandle_max_1.angle = high;

                _swingHandle_min_1.radius = radius;
                _swingHandle_max_1.radius = radius;

                EditorGUI.BeginChangeCheck();
                _swingHandle_min_1.DrawHandle();
                _swingHandle_max_1.DrawHandle();
                if (EditorGUI.EndChangeCheck())
                {
                    _swingHandle_min_1.angle = Mathf.Clamp(_swingHandle_min_1.angle, -180, 0);
                    _swingHandle_max_1.angle = Mathf.Clamp(_swingHandle_max_1.angle, 0, 180);

                    return true;
                }

                return false;
            }
        }

        public bool DrawSwing2(float low, float high, float radius, Matrix4x4 anchorToWorld)
        {
            var color = Handles.zAxisColor;
            using (new Handles.DrawingScope(color, anchorToWorld * s_rotationOffset_2 * s_rotationOffset_0))
            {
                _swingHandle_min_2.angle = low;
                _swingHandle_max_2.angle = high;

                _swingHandle_min_2.radius = radius;
                _swingHandle_max_2.radius = radius;

                EditorGUI.BeginChangeCheck();
                _swingHandle_min_2.DrawHandle();
                _swingHandle_max_2.DrawHandle();
                if (EditorGUI.EndChangeCheck())
                {
                    _swingHandle_min_2.angle = Mathf.Clamp(_swingHandle_min_2.angle, -180, 0);
                    _swingHandle_max_2.angle = Mathf.Clamp(_swingHandle_max_2.angle, 0, 180);

                    return true;
                }

                return false;
            }
        }

        public bool DrawBallSwing(float low, float high, float radius, Matrix4x4 anchorToWorld)
        {
            var color = Handles.yAxisColor;
            using (new Handles.DrawingScope(color, anchorToWorld * s_rotationOffset_0))
            {
                _swingHandle_min_1.angle = low;
                _swingHandle_max_1.angle = high;

                _swingHandle_min_1.radius = radius;
                _swingHandle_max_1.radius = radius;

                var modified = false;

                EditorGUI.BeginChangeCheck();
                _swingHandle_min_1.DrawHandle();
                if (EditorGUI.EndChangeCheck())
                {
                    var currentLow = Mathf.Abs(Mathf.Clamp(_swingHandle_min_1.angle, -180, 0));
                    _swingHandle_min_1.angle = -currentLow;
                    _swingHandle_max_1.angle = currentLow;
                    modified = true;
                }

                EditorGUI.BeginChangeCheck();
                _swingHandle_max_1.DrawHandle();
                if (EditorGUI.EndChangeCheck())
                {
                    var currentHigh = Mathf.Abs(Mathf.Clamp(_swingHandle_max_1.angle, 0, 180));
                    _swingHandle_min_1.angle = -currentHigh;
                    _swingHandle_max_1.angle = currentHigh;
                    modified = true;
                }

                return modified;
            }
        }
    }

    internal class ArcLimitDrawer
    {
        public float arcLow => _arcHandle_min.angle;
        public float arcHigh => _arcHandle_max.angle;

        private ArcHandle _arcHandle_min;
        private ArcHandle _arcHandle_max;

        private static readonly Matrix4x4 s_rotationOffset_0 =
            Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.up), Vector3.one);
        private static readonly Matrix4x4 s_rotationOffset_1 =
            Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(180, Vector3.right), Vector3.one);
        private static readonly Matrix4x4 s_rotationOffset_2 =
            Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.forward), Vector3.one);

        public ArcLimitDrawer()
        {
            _arcHandle_min = new ArcHandle();
            _arcHandle_max = new ArcHandle();

            _arcHandle_min.fillColor = Color.clear;
            _arcHandle_max.fillColor = Color.clear;

            _arcHandle_max.wireframeColor = Color.clear;
            _arcHandle_min.wireframeColor = Color.clear;
        }

        public bool DrawArcLimit(float low, float high, float radius, Matrix4x4 anchorToWorld)
        {
            using (new Handles.DrawingScope(Handles.xAxisColor, anchorToWorld * s_rotationOffset_2 * s_rotationOffset_1 * s_rotationOffset_0))
            {
                _arcHandle_max.radius = radius;
                _arcHandle_min.radius = radius;

                _arcHandle_min.angle = low;
                _arcHandle_max.angle = high;

                EditorGUI.BeginChangeCheck();
                _arcHandle_min.DrawHandle();
                _arcHandle_max.DrawHandle();
                if (EditorGUI.EndChangeCheck())
                {
                    _arcHandle_min.angle = Mathf.Clamp(_arcHandle_min.angle, -180, _arcHandle_max.angle);
                    _arcHandle_max.angle = Mathf.Clamp(_arcHandle_max.angle, _arcHandle_min.angle, 180);
                    return true;
                }

                return false;
            }
        }
    }
}
