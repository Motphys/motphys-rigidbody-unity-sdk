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
    [CustomEditor(typeof(HingeJoint3D))]
    internal class HingeJoint3DEditor : BaseJointEditor
    {
        private Object[] _joints;
        private HingeJoint3D _hingeJoint;

        [SerializeField]
        private JointAngularLimit _angleLimit = new JointAngularLimit(-180, 180);
        protected override void OnEnable()
        {
            base.OnEnable();
            _joints = targets;
            _hingeJoint = target as HingeJoint3D;

            serializedObject.FindProperty("_limitSpring").isExpanded = true;
        }

        protected override void LimitDisabledGroup()
        {
            EditorGUILayout.PropertyField(_editorObject.FindProperty("_angleLimit"));
        }

        protected override void LimitEnabledGroup()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_angleLimit"));
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_angularDamper"));
        }

        protected override void MotorDisabledGroup()
        {
            var joint = this._hingeJoint;

            EditorGUI.BeginChangeCheck();

            joint._motorType = (MotorType)EditorGUILayout.Popup("Motor Type", (int)joint._motorType, _angularMotorTypes);

            EditorGUILayout.BeginVertical("framebox");
            switch (joint._motorType)
            {
                case MotorType.Position:
                    var rotationMotor = joint._rotationMotor;
                    rotationMotor.type = MotorType.Position;

                    rotationMotor.targetAngle = EditorGUILayout.DelayedFloatField("Target Angle", rotationMotor.targetAngle);

                    rotationMotor.spring.stiffness = EditorGUILayout.DelayedFloatField(_angularAngleStrengthText, rotationMotor.spring.stiffness);
                    rotationMotor.spring.damper = EditorGUILayout.DelayedFloatField(_angularAngleDamperText, rotationMotor.spring.damper);

                    if (EditorGUI.EndChangeCheck())
                    {
                        rotationMotor.targetSpeed = 0;
                        joint._rotationMotor = rotationMotor;
                        joint.motor = rotationMotor;
                    }

                    break;
                case MotorType.Speed:
                    var velocityMotor = joint._velocityMotor;
                    velocityMotor.type = MotorType.Speed;

                    velocityMotor.targetSpeed = EditorGUILayout.DelayedFloatField("Target Speed", velocityMotor.targetSpeed);
                    velocityMotor.spring.damper = EditorGUILayout.DelayedFloatField(_linearSpeedStrengthText, velocityMotor.spring.damper);

                    if (EditorGUI.EndChangeCheck())
                    {
                        velocityMotor.targetAngle = 0;
                        velocityMotor.spring.stiffness = 0;
                        joint._velocityMotor = velocityMotor;
                        joint.motor = velocityMotor;
                    }

                    break;
            }

            EditorGUILayout.EndVertical();
        }

        private void OnSceneGUI()
        {
            if (_joints.Length > 1)
            {
                return;
            }

            var joint = _hingeJoint;
            if (joint != null && joint.enabled)
            {
                if (SceneView.currentDrawingSceneView.drawGizmos)
                {
                    DrawLimitArc(joint);
                }
            }
        }

        internal static void DrawLimitArc(HingeJoint3D joint)
        {
            var anchorToWorld = HandlesHelper.GetAnchorToWorldMatrix(joint);
            var size = HandleUtility.GetHandleSize(joint.transform.position);
            var low = joint.angularLimit.low;
            var high = joint.angularLimit.high;

            //draw arc range;
            using (new Handles.DrawingScope(BaseJointEditor.s_fadeRed, anchorToWorld))
            {
                var delta = high - low;
                var from = Quaternion.Euler(low, 0, 0) * Vector3.up;
                Handles.DrawSolidArc(Vector3.zero, Vector3.right, from, joint.useLimit ? delta : 360, size);
                Handles.ArrowHandleCap(-1, Vector3.zero, Quaternion.Euler(0, 90, 0), size * 0.75f, EventType.Repaint);
            }
        }
    }

    /// <summary>
    /// Draw Anchor Gizmos
    /// </summary>
    [EditorTool("Edit Joint Anchor", typeof(HingeJoint3D))]
    internal class HingeJointAnchorEditorTool : JointEditorTool<HingeJoint3D>
    {
        public override void OnActivated()
        {
            HingeJoint3DEditor.s_selected = 0;
        }

        public override void OnWillBeDeactivated()
        {
            base.OnWillBeDeactivated();
        }
    }

    /// <summary>
    /// Draw Rotation Limit Gizmos
    /// </summary>
    [EditorTool("Edit Angular Limit", typeof(HingeJoint3D))]
    [UnityEngine.TestTools.ExcludeFromCoverage]
    internal class AngularLimitEditorTool : EditorTool
    {
        private ArcLimitDrawer _arcLimitDrawer;

        private HingeJoint3D _joint;
        private static GUIContent s_icon;
        public override GUIContent toolbarIcon => s_icon ??= new GUIContent()
        {
            image = EditorGUIUtility.IconContent("JointAngularLimits").image,
            text = "Edit Angular Limit",
            tooltip = "Edit Angular Limit",
        };

        public override void OnActivated()
        {
            _arcLimitDrawer = new ArcLimitDrawer();
            _joint = target as HingeJoint3D;

            HingeJoint3DEditor.s_selected = 1;
        }

        public override void OnWillBeDeactivated()
        {
            base.OnWillBeDeactivated();
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (_joint != null)
            {
                //draw arc handles
                if (_joint.useLimit)
                {
                    var size = HandleUtility.GetHandleSize(_joint.transform.position);
                    var low = _joint.angularLimit.low;
                    var high = _joint.angularLimit.high;
                    var anchorToWorld = HandlesHelper.GetAnchorToWorldMatrix(_joint);
                    if (_arcLimitDrawer.DrawArcLimit(low, high, size, anchorToWorld))
                    {
                        _joint.angularLimit = new JointAngularLimit(_arcLimitDrawer.arcLow, _arcLimitDrawer.arcHigh);
                    }
                }

                //Draw limit range always when editing limit.
                if (!SceneView.currentDrawingSceneView.drawGizmos)
                {
                    HingeJoint3DEditor.DrawLimitArc(_joint);
                }
            }
        }
    }
}
