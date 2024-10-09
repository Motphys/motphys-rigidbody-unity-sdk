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
    [CustomEditor(typeof(SliderJoint3D))]
    internal class SliderJoint3DEditor : BaseJointEditor
    {
        private Object[] _joints;
        private SliderJoint3D _sliderJoint;

        [SerializeField]
        private JointTranslationLimit _distanceLimit = new JointTranslationLimit(float.MinValue, float.MaxValue);

        protected override void OnEnable()
        {
            base.OnEnable();
            _joints = targets;
            _sliderJoint = target as SliderJoint3D;
            serializedObject.FindProperty("_limitSpring").isExpanded = true;
            if (!Application.isPlaying)
            {
                _sliderJoint.TryAutoConfigureConnected();
            }
        }

        protected override void ConnectedAnchorFrameGUI(SerializedObject serializedObject)
        {
            var frame = serializedObject.FindProperty("_connectedAnchorFrame");
            var anchor = frame.FindPropertyRelative("anchor");
            anchor.vector3Value = EditorGUILayout.Vector3Field("Connected Anchor", anchor.vector3Value);
        }

        protected override void LimitDisabledGroup()
        {
            EditorGUILayout.PropertyField(_editorObject.FindProperty("_distanceLimit"));
        }

        protected override void LimitEnabledGroup()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_distanceLimit"));
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_linearDamper"));
        }

        protected override void MotorDisabledGroup()
        {
            var joint = this._sliderJoint;

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

                //Target speed mode
                case MotorType.Speed:
                    var speedMotor = motorPack._speedMotor;
                    speedMotor.type = MotorType.Speed;

                    speedMotor.targetSpeed = EditorGUILayout.DelayedFloatField(_targetSpeedText, speedMotor.targetSpeed);
                    speedMotor.spring.damper = EditorGUILayout.DelayedFloatField(_linearSpeedStrengthText, speedMotor.spring.damper);
                    if (EditorGUI.EndChangeCheck())
                    {
                        speedMotor.targetPosition = 0;
                        speedMotor.spring.stiffness = 0;
                        motorPack._speedMotor = speedMotor;
                        joint.motor = speedMotor;
                    }

                    break;
            }

            EditorGUILayout.EndVertical();
        }

        private void OnSceneGUI()
        {
            var joint = _sliderJoint;
            if (joint != null && joint.enabled)
            {
                if (SceneView.currentDrawingSceneView.drawGizmos)
                {
                    DrawLimitLine(joint);
                }
            }
        }

        internal static void DrawLimitLine(SliderJoint3D joint)
        {
            var anchorToWorld = HandlesHelper.GetAnchorToWorldMatrix(joint);
            var size = HandleUtility.GetHandleSize(joint.transform.position);

            //main anchor
            using (new Handles.DrawingScope(Color.red, anchorToWorld))
            {
                Handles.SphereHandleCap(0, Vector3.zero, Quaternion.identity, size * 0.2f, EventType.Repaint);
            }

            //connected anchor
            if (Application.isPlaying && joint.connectedBody != null)
            {
                using (new Handles.DrawingScope(Color.red, HandlesHelper.ExtractTRMatrix(joint.connectedBody.transform.localToWorldMatrix)))
                {
                    var connectedAnchor = Vector3.Scale(joint.connectedAnchorPosition, joint.connectedBody.transform.lossyScale);
                    Handles.SphereHandleCap(0, connectedAnchor, Quaternion.identity, size * 0.2f, EventType.Repaint);
                }
            }

            using (new Handles.DrawingScope(Color.red, anchorToWorld))
            {
                var cubeSize = new Vector3(0, size * 0.75f, size * 0.75f);
                if (joint.useLimit)
                {
                    var head = Vector3.right * joint.distanceLimit.low;
                    var tail = Vector3.right * joint.distanceLimit.high;
                    Handles.DrawWireCube(head, cubeSize);
                    Handles.DrawWireCube(tail, cubeSize);
                    Handles.DrawLine(head, tail, 1.2f);
                }
                else
                {
                    Handles.ArrowHandleCap(-1, Vector3.zero, Quaternion.Euler(0, 90, 0), size * 0.75f, EventType.Repaint);
                }
            }
        }
    }

    [EditorTool("Edit Slider Joint", typeof(SliderJoint3D))]
    internal class SliderJointEditorTool : JointEditorTool<SliderJoint3D>
    {
        public override void OnActivated()
        {
            SliderJoint3DEditor.s_selected = 0;
        }
        public override void OnWillBeDeactivated()
        {
            base.OnWillBeDeactivated();
        }
    }

    /// <summary>
    /// Draw Slider Limit Gizmos
    /// </summary>
    [EditorTool("Edit Slider Limit", typeof(SliderJoint3D))]
    [UnityEngine.TestTools.ExcludeFromCoverage]
    internal class SliderLimitEditorTool : EditorTool
    {
        private SliderJoint3D _joint;
        private static GUIContent s_icon;
        public override GUIContent toolbarIcon => s_icon ??= new GUIContent()
        {
            image = EditorGUIUtility.IconContent("JointAngularLimits").image,
            text = "Edit Slider Limit",
            tooltip = "Edit Slider Limit",
        };
        public override void OnActivated()
        {
            _joint = target as SliderJoint3D;
            SliderJoint3DEditor.s_selected = 1;
        }
        public override void OnWillBeDeactivated()
        {
            base.OnWillBeDeactivated();
        }

        public static void DefaultDistanceHandleDrawFunction(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            Handles.CubeHandleCap(controlID, position, rotation, size * 0.3f, eventType);
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (_joint != null && _joint.useLimit)
            {
                var anchorToWorld = HandlesHelper.GetAnchorToWorldMatrix(_joint);
                using (new Handles.DrawingScope(BaseJointEditor.s_fadeRed, anchorToWorld))
                {
                    var position = Vector3.zero;
                    var limitLow = _joint.distanceLimit.low;
                    var limitHigh = _joint.distanceLimit.high;
                    var head = Vector3.right * limitLow;
                    var tail = Vector3.right * limitHigh;
                    var controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
                    var size = HandleUtility.GetHandleSize(position);
                    var modified = false;

                    Handles.SphereHandleCap(-1, position, Quaternion.identity, size * 0.25f, EventType.Repaint);

                    EditorGUI.BeginChangeCheck();
                    var headPos = Handles.Slider(1, head, Vector3.right, size, DefaultDistanceHandleDrawFunction, EditorSnapSettings.move.z);
                    if (EditorGUI.EndChangeCheck())
                    {
                        limitLow = headPos.x - position.x;
                        modified = true;
                    }

                    EditorGUI.BeginChangeCheck();
                    var tailPos = Handles.Slider(2, tail, Vector3.right, size, DefaultDistanceHandleDrawFunction, EditorSnapSettings.move.z);
                    if (EditorGUI.EndChangeCheck())
                    {
                        limitHigh = tailPos.x - position.x;
                        modified = true;
                    }

                    if (modified)
                    {
                        limitLow = Mathf.Min(limitLow, limitHigh);
                        _joint.distanceLimit = new JointTranslationLimit(limitLow, limitHigh);
                    }

                    Handles.DrawLine(head, tail);
                }

                //Draw limit range always when editing limit.
                if (!SceneView.currentDrawingSceneView.drawGizmos)
                {
                    SliderJoint3DEditor.DrawLimitLine(_joint);
                }
            }
        }
    }
}
