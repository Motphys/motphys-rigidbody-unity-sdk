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
    [CustomEditor(typeof(DistanceJoint3D))]
    internal class DistanceJoint3DEditor : BaseJointEditor
    {
        private Object[] _joints;
        private DistanceJoint3D _distancejoint;

        private bool _showVelocityDamper = true;

        protected override void OnEnable()
        {
            base.OnEnable();
            _joints = targets;
            _distancejoint = target as DistanceJoint3D;
            serializedObject.FindProperty("_limitSpring").isExpanded = true;
        }

        protected override void AnchorFrameGUI(SerializedObject serializedObject)
        {
            var frame = serializedObject.FindProperty("_anchorFrame");
            var anchor = frame.FindPropertyRelative("anchor");
            EditorGUILayout.PropertyField(anchor);
        }
        protected override void ConnectedAnchorFrameGUI(SerializedObject serializedObject)
        {
            var frame = serializedObject.FindProperty("_connectedAnchorFrame");
            var anchor = frame.FindPropertyRelative("anchor");
            anchor.vector3Value = EditorGUILayout.Vector3Field("Connected Anchor", anchor.vector3Value);
        }

        protected override void LimitEnabledGroup()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_minDistance"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxDistance"));
        }

        protected override void LimitDisabledGroup()
        {
            EditorGUILayout.FloatField("Min Distance", 0);
            EditorGUILayout.FloatField("Max Distance", 0);
        }

        protected override void LimitDamperGUI()
        {
            EditorGUILayout.Space(10);
            var useSpring = serializedObject.FindProperty("_useSpring");
            EditorGUILayout.PropertyField(useSpring);
            if (useSpring.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_limitSpring"));
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(_editorObject.FindProperty("_limitSpring"));
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.Space(10);
            _showVelocityDamper = EditorGUILayout.Foldout(_showVelocityDamper, "Velocity Damper");
            EditorGUI.indentLevel += 1;
            if (_showVelocityDamper)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_linearDamper"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_angularDamper"));
            }

            EditorGUI.indentLevel -= 1;
        }

        protected override void MotorDisabledGroup()
        {
            var joint = this._distancejoint;

            EditorGUI.BeginChangeCheck();
            joint._motorType = (MotorType)EditorGUILayout.Popup("Motor Type", (int)joint._motorType, _linearMotorTypes);
            var motorPack = joint._motorPack;
            EditorGUILayout.BeginVertical("framebox");
            switch (joint._motorType)
            {
                //Target distance mode
                case MotorType.Position:
                    var distanceMotor = motorPack._distanceMotor;
                    distanceMotor.type = MotorType.Position;

                    distanceMotor.targetPosition = EditorGUILayout.DelayedFloatField(_targetDistanceText, distanceMotor.targetPosition);
                    distanceMotor.spring.stiffness = EditorGUILayout.DelayedFloatField(_linearDistanceStrengthText, distanceMotor.spring.stiffness);
                    distanceMotor.spring.damper = EditorGUILayout.DelayedFloatField(_linearDistanceDamperText, distanceMotor.spring.damper);

                    if (EditorGUI.EndChangeCheck())
                    {
                        distanceMotor.targetSpeed = 0;
                        motorPack._distanceMotor = distanceMotor;
                        joint.motor = distanceMotor;
                    }

                    break;
                //Target distance mode
                case MotorType.Speed:
                    var speedMotor = motorPack._speedMotor;
                    speedMotor.type = MotorType.Speed;

                    speedMotor.targetSpeed = EditorGUILayout.DelayedFloatField(_targetSpeedText, speedMotor.targetSpeed);
                    speedMotor.spring.damper = EditorGUILayout.DelayedFloatField(_linearSpeedStrengthText, speedMotor.spring.damper);
                    if (EditorGUI.EndChangeCheck())
                    {
                        speedMotor.targetPosition = 0;
                        speedMotor.spring.stiffness = 0.5f;
                        motorPack._speedMotor = speedMotor;
                        joint.motor = speedMotor;
                    }

                    break;
            }

            EditorGUILayout.EndVertical();
        }

        private void OnSceneGUI()
        {
            var joint = _distancejoint;
            if (joint != null && joint.enabled)
            {
                if (SceneView.currentDrawingSceneView.drawGizmos)
                {
                    DrawLimitLine(joint);

                    //Connected line
                    if (joint.connectedBody != null)
                    {
                        using (new Handles.DrawingScope(Color.red))
                        {
                            var anchor = joint.transform.TransformPoint(joint.anchorPosition);
                            var connectedAnchor = joint.connectedBody.transform.TransformPoint(joint.connectedAnchorPosition);
                            var anchorSize = HandleUtility.GetHandleSize(connectedAnchor) * 0.15f;
                            Handles.SphereHandleCap(0, connectedAnchor, Quaternion.identity, anchorSize, EventType.Repaint);
                            Handles.DrawLine(anchor, connectedAnchor, 1f);
                        }
                    }
                }
            }
        }

        internal static void DrawLimitLine(DistanceJoint3D joint)
        {
            var anchorToWorld = HandlesHelper.GetAnchorToWorldMatrix(joint);
            var size = HandleUtility.GetHandleSize(joint.transform.position);
            var ciecleSize = size * 0.5f;

            //anchor
            using (new Handles.DrawingScope(Color.red, anchorToWorld))
            {
                var anchorSize = HandleUtility.GetHandleSize(Vector3.zero) * 0.15f;
                Handles.SphereHandleCap(0, Vector3.zero, Quaternion.identity, anchorSize, EventType.Repaint);
            }

            //distance
            using (new Handles.DrawingScope(Color.red, anchorToWorld))
            {
                if (joint.useLimit)
                {
                    var distance = new Vector2(joint.minDistance, joint.maxDistance);
                    var head_0 = Vector3.right * Mathf.Abs(distance.x);
                    var tail_0 = Vector3.right * Mathf.Abs(distance.y);

                    var head_1 = Vector3.left * Mathf.Abs(distance.x);
                    var tail_1 = Vector3.left * Mathf.Abs(distance.y);

                    Handles.DrawWireDisc(head_0, Vector3.right, ciecleSize);
                    Handles.DrawWireDisc(tail_0, Vector3.right, ciecleSize);

                    Handles.DrawWireDisc(head_1, Vector3.right, ciecleSize);
                    Handles.DrawWireDisc(tail_1, Vector3.right, ciecleSize);

                    Handles.DrawLine(head_0, tail_0, 2.0f);
                    Handles.DrawLine(head_1, tail_1, 2.0f);

                }
                else
                {
                    Handles.ArrowHandleCap(-1, Vector3.zero, Quaternion.Euler(0, 90, 0), size * 0.5f, EventType.Repaint);
                }
            }
        }
    }

    [EditorTool("Edit Distance Joint", typeof(DistanceJoint3D))]
    internal class DistanceJointEditorTool : JointEditorTool<DistanceJoint3D>
    {
        protected override bool drawRotationHandles => false;

        public override void OnActivated()
        {
            DistanceJoint3DEditor.s_selected = 0;
        }

        public override void OnWillBeDeactivated()
        {
            base.OnWillBeDeactivated();
        }

        protected override void DrawHandles(DistanceJoint3D joint3D)
        {
            base.DrawHandles(joint3D);

            //Draw connected anchor handles.
            if (!joint3D.autoConfigureConnected && joint3D.connectedBody != null)
            {
                var matrix = Matrix4x4.TRS(joint3D.connectedBody.transform.position, joint3D.connectedBody.transform.rotation, Vector3.one);

                using (new Handles.DrawingScope(matrix))
                {
                    var size = 1.0f;
                    var lossyScale = joint3D.connectedBody.transform.lossyScale;
                    var position = Vector3.Scale(joint3D.connectedAnchorPosition, lossyScale);
                    var rotation = joint3D.connectedAnchorFrame.rotation;
                    _anchorHandlePositionB = HandlesHelper.PositionHandle(position, rotation, size);
                }
            }
        }

        protected override void UpdateProperties(DistanceJoint3D joint3D)
        {
            base.UpdateProperties(joint3D);
            if (!joint3D.autoConfigureConnected && joint3D.connectedBody != null)
            {
                joint3D.connectedAnchorFrame = new AxisFrame()
                {
                    anchor = Vector3.Scale(_anchorHandlePositionB, InvertScaleVector(joint3D.connectedBody.transform.lossyScale)),
                    axisRotation = _anchorHandleRotationA.eulerAngles,
                };
            }
        }
    }

    /// <summary>
    /// Draw Distance Limit Gizmos
    /// </summary>
    [EditorTool("Edit Distance Limit", typeof(DistanceJoint3D))]
    [UnityEngine.TestTools.ExcludeFromCoverage]
    internal class DistanceLimitEditorTool : EditorTool
    {
        private DistanceJoint3D _joint;
        private static GUIContent s_icon;
        public override GUIContent toolbarIcon => s_icon ??= new GUIContent()
        {
            image = EditorGUIUtility.IconContent("JointAngularLimits").image,
            text = "Edit Distance Limit",
            tooltip = "Edit Distance Limit",
        };
        public override void OnActivated()
        {
            _joint = target as DistanceJoint3D;
            DistanceJoint3DEditor.s_selected = 1;
        }
        public override void OnWillBeDeactivated()
        {
            base.OnWillBeDeactivated();
        }

        public static void DefaultDistanceHandleDrawFunction(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            Handles.DrawSolidDisc(position, Vector3.right, size);
            Handles.CircleHandleCap(controlID, position, rotation, size, eventType);
        }
        public override void OnToolGUI(EditorWindow window)
        {
            if (_joint != null && _joint.useLimit)
            {
                //Draw limit range always when editing limit.
                if (!SceneView.currentDrawingSceneView.drawGizmos)
                {
                    DistanceJoint3DEditor.DrawLimitLine(_joint);
                }

                var anchorToWorld = HandlesHelper.GetAnchorToWorldMatrix(_joint);
                using (new Handles.DrawingScope(BaseJointEditor.s_fadeRed, anchorToWorld))
                {
                    var position = Vector3.zero;
                    var head = Vector3.right * _joint.minDistance;
                    var tail = Vector3.right * _joint.maxDistance;
                    var controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
                    var size = HandleUtility.GetHandleSize(position) * 0.5f;

                    EditorGUI.BeginChangeCheck();
                    var headPos = Handles.Slider(1, head, Vector3.right, size, DefaultDistanceHandleDrawFunction, EditorSnapSettings.move.z);
                    var tailPos = Handles.Slider(2, tail, Vector3.right, size, DefaultDistanceHandleDrawFunction, EditorSnapSettings.move.z);

                    if (EditorGUI.EndChangeCheck())
                    {
                        var min = Vector3.Distance(position, headPos);
                        var max = Vector3.Distance(position, tailPos);
                        if (max < min)
                        {
                            max = min;
                        }

                        _joint.SetDistanceLimit(min, max);
                    }
                }
            }
        }
    }
}
