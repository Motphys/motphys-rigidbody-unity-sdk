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
using static Motphys.Rigidbody.Editor.HandlesHelper;

namespace Motphys.Rigidbody.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BallJoint3D))]
    internal class BallJoint3DEditor : BaseJointEditor
    {
        private Object[] _joints;
        private BallJoint3D _ballJoint;

        protected override void OnEnable()
        {
            base.OnEnable();
            _joints = targets;
            _ballJoint = target as BallJoint3D;
            serializedObject.FindProperty("_limitSpring").isExpanded = true;

            if (!Application.isPlaying)
            {
                _ballJoint.TryAutoConfigureConnected();
            }
        }

        protected override void LimitEnabledGroup()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_twistLimit"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_swingLimit"));
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
            var joint = _ballJoint;
            EditorGUI.BeginChangeCheck();
            var angularMotor = joint.motor;
            angularMotor.mode = (AngularMotorDriveMode)EditorGUILayout.EnumPopup("Angular Motor Drive Mode", joint.motor.mode);
            var motorPack = joint._d3AngularMotorPack;

            EditorGUILayout.BeginVertical("framebox");
            switch (angularMotor.mode)
            {
                case AngularMotorDriveMode.SLerp or AngularMotorDriveMode.TwistSwing:
                    var rotationMotor = motorPack._rotationMotor;
                    rotationMotor.type = MotorType.Position;
                    rotationMotor.targetRotation = MotphysEditorGUILayout.QuaternionField("Target Rotation", rotationMotor.targetRotation);
                    if (angularMotor.mode == AngularMotorDriveMode.SLerp)
                    {
                        rotationMotor.mode = AngularMotorDriveMode.SLerp;
                        rotationMotor.slerpDrive.spring.stiffness = EditorGUILayout.DelayedFloatField(_d3SlerpRotationStrengthText, rotationMotor.slerpDrive.spring.stiffness);
                        rotationMotor.slerpDrive.spring.damper = EditorGUILayout.DelayedFloatField(_d3SlerpRotationDamperText, rotationMotor.slerpDrive.spring.damper);

                    }
                    else
                    {
                        rotationMotor.mode = AngularMotorDriveMode.TwistSwing;
                        rotationMotor.twistDrive.spring.stiffness = EditorGUILayout.DelayedFloatField(_d3TwistRotationStrengthText, rotationMotor.twistDrive.spring.stiffness);
                        rotationMotor.twistDrive.spring.damper = EditorGUILayout.DelayedFloatField(_d3TwistRotationDamperText, rotationMotor.twistDrive.spring.damper);

                        rotationMotor.swingDrive.spring.stiffness = EditorGUILayout.DelayedFloatField(_d3SwingRotationStrengthText, rotationMotor.swingDrive.spring.stiffness);
                        rotationMotor.swingDrive.spring.damper = EditorGUILayout.DelayedFloatField(_d3SwingRotationDamperText, rotationMotor.swingDrive.spring.damper);

                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        motorPack._rotationMotor = rotationMotor;
                        joint.motor = rotationMotor;
                    }

                    break;
                case AngularMotorDriveMode.Velocity:
                    var velocityMotor = motorPack._velocityMotor;
                    velocityMotor.type = MotorType.Speed;
                    velocityMotor.mode = AngularMotorDriveMode.Velocity;
                    velocityMotor.targetAngularVelocity = EditorGUILayout.Vector3Field("Target Angular Velocity", velocityMotor.targetAngularVelocity);
                    velocityMotor.slerpDrive.spring.damper = EditorGUILayout.DelayedFloatField(_d3VelocityStrengthText, velocityMotor.slerpDrive.spring.damper);

                    if (EditorGUI.EndChangeCheck())
                    {
                        velocityMotor.slerpDrive.spring.stiffness = 0;
                        motorPack._velocityMotor = velocityMotor;
                        joint.motor = velocityMotor;
                    }

                    break;
            }

            EditorGUILayout.EndVertical();
        }

        private void OnSceneGUI()
        {
            var joint = _ballJoint;
            if (joint != null && joint.enabled)
            {
                if (SceneView.currentDrawingSceneView.drawGizmos)
                {
                    DrawLimitArc(joint);
                }
            }
        }

        internal static void DrawLimitArc(BallJoint3D joint)
        {
            var anchorToWorld = HandlesHelper.GetAnchorToWorldMatrix(joint);
            var size = HandleUtility.GetHandleSize(joint.transform.position);
            var twistLimit = joint.twistLimit;
            var low = twistLimit.low;
            var high = twistLimit.high;
            var delta = high - low;
            var from = Quaternion.Euler(low, 0, 0) * Vector3.up;

            //twist limit
            using (new Handles.DrawingScope(BaseJointEditor.s_fadeRed, anchorToWorld))
            {
                Handles.DrawSolidArc(Vector3.zero, Vector3.right, from, delta, size);

                //swing limit
                var angle = Mathf.Abs(joint.swingLimit);
                if (angle > 0)
                {
                    angle = Mathf.Clamp(angle, 1, 179);
                    var swing = new JointAngularLimit(-angle, angle);
                    var config = new EllipsoidHandleConfig(swing, swing)
                    {
                        scale = size,
                        segCount = 100,
                    };
                    HandlesHelper.DrawEllipsoidCone(config);
                }

                //Twist axis
                Handles.ArrowHandleCap(0, Vector3.zero, Quaternion.Euler(0, 90, 0), size * 0.75f, EventType.Repaint);
            }
        }
    }

    [EditorTool("Edit Ball Joint", typeof(BallJoint3D))]
    internal class BallJointEditorTool : JointEditorTool<BallJoint3D>
    {
        public override void OnActivated()
        {
            base.OnActivated();
            BallJoint3DEditor.s_selected = 0;
        }
        public override void OnWillBeDeactivated()
        {
            base.OnWillBeDeactivated();
        }
    }

    /// <summary>
    /// Draw Ball Limit Gizmos
    /// </summary>
    [EditorTool("Edit Ball Limit", typeof(BallJoint3D))]
    [UnityEngine.TestTools.ExcludeFromCoverage]
    internal class BallLimitEditorTool : EditorTool
    {
        private BallJoint3D _joint;
        private static GUIContent s_icon;
        private ConeLimitDrawer _coneLimitDrawer;

        public override GUIContent toolbarIcon => s_icon ??= new GUIContent()
        {
            image = EditorGUIUtility.IconContent("JointAngularLimits").image,
            text = "Edit Ball Limit",
            tooltip = "Edit Ball Limit",
        };
        public override void OnActivated()
        {
            _joint = target as BallJoint3D;
            _coneLimitDrawer = new ConeLimitDrawer();
            BallJoint3DEditor.s_selected = 1;
        }
        public override void OnWillBeDeactivated()
        {
            base.OnWillBeDeactivated();
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (_joint != null)
            {
                var joint = _joint;
                var anchorToWorld = HandlesHelper.GetAnchorToWorldMatrix(joint);
                var size = HandleUtility.GetHandleSize(joint.transform.position);
                var swingLimit = joint.swingLimit;

                if (_coneLimitDrawer.DrawTwist(joint.twistLimit.low, joint.twistLimit.high, size, anchorToWorld))
                {
                    joint.twistLimit = new JointAngularLimit(_coneLimitDrawer.twistLimitLow, _coneLimitDrawer.twistLimitHigh);
                }

                if (_coneLimitDrawer.DrawBallSwing(-swingLimit, swingLimit, size, anchorToWorld))
                {
                    var angle = _coneLimitDrawer.swingLimitHigh_1;
                    joint.swingLimit = angle;
                }

                //Draw limit range always when editing limit.
                if (!SceneView.currentDrawingSceneView.drawGizmos)
                {
                    BallJoint3DEditor.DrawLimitArc(_joint);
                }
            }
        }
    }
}

