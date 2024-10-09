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
    [CustomEditor(typeof(EllipsoidJoint3D))]
    internal class EllipsoidJoint3DEditor : BaseJointEditor
    {
        private Object[] _joints;
        private EllipsoidJoint3D _ellipsoidJoint;
        protected override void OnEnable()
        {
            base.OnEnable();
            _joints = targets;
            _ellipsoidJoint = target as EllipsoidJoint3D;
            serializedObject.FindProperty("_limitSpring").isExpanded = true;
            if (!Application.isPlaying)
            {
                _ellipsoidJoint.TryAutoConfigureConnected();
            }
        }

        protected override void LimitEnabledGroup()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_twistLimit"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_swingLimitY"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_swingLimitZ"));
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
            var joint = _ellipsoidJoint;
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
            var joint = _ellipsoidJoint;
            if (joint != null && joint.enabled)
            {
                if (SceneView.currentDrawingSceneView.drawGizmos)
                {
                    DrawLimitArc(joint);
                }
            }
        }

        internal static void DrawLimitArc(EllipsoidJoint3D joint)
        {
            var anchorToWorld = HandlesHelper.GetAnchorToWorldMatrix(joint);
            var size = HandleUtility.GetHandleSize(joint.transform.position);
            var twistLimit = joint.twistLimit;
            var low = twistLimit.low;
            var high = twistLimit.high;
            var delta = high - low;
            var from = Quaternion.Euler(low, 0, 0) * Vector3.up;

            using (new Handles.DrawingScope(BaseJointEditor.s_fadeRed, anchorToWorld))
            {
                Handles.DrawSolidArc(Vector3.zero, Vector3.right, from, delta, size);

                var config = new EllipsoidHandleConfig(joint.swingLimitY, joint.swingLimitZ)
                {
                    scale = size,
                    segCount = 100,
                };
                HandlesHelper.DrawEllipsoidCone(config);

                //Twist axis
                Handles.ArrowHandleCap(0, Vector3.zero, Quaternion.Euler(0, 90, 0), size * 0.75f, EventType.Repaint);
            }
        }
    }

    [EditorTool("Edit Ellipsoid Joint", typeof(EllipsoidJoint3D))]
    internal class EllipsoidJointEditorTool : JointEditorTool<EllipsoidJoint3D>
    {
        public override void OnActivated()
        {
            base.OnActivated();
            EllipsoidJoint3DEditor.s_selected = 0;
        }
        public override void OnWillBeDeactivated()
        {
            base.OnWillBeDeactivated();
        }
    }

    /// <summary>
    /// Draw Ellipsoid Limit Gizmos
    /// </summary>
    [EditorTool("Edit Ellipsoid Limit", typeof(EllipsoidJoint3D))]
    [UnityEngine.TestTools.ExcludeFromCoverage]
    internal class EllipsoidLimitEditorTool : EditorTool
    {
        private static GUIContent s_icon;
        private EllipsoidJoint3D _joint;
        private ConeLimitDrawer _coneLimitDrawer;

        public override GUIContent toolbarIcon => s_icon ??= new GUIContent()
        {
            image = EditorGUIUtility.IconContent("JointAngularLimits").image,
            text = "Edit Ellipsoid Limit",
            tooltip = "Edit Ellipsoid Limit",
        };

        public override void OnActivated()
        {
            _joint = target as EllipsoidJoint3D;
            _coneLimitDrawer = new ConeLimitDrawer();
            EllipsoidJoint3DEditor.s_selected = 1;
        }

        public override void OnWillBeDeactivated()
        {
            base.OnWillBeDeactivated();
        }

        public override void OnToolGUI(EditorWindow window)
        {
            var joint = _joint;
            if (joint != null)
            {
                var anchorToWorld = HandlesHelper.GetAnchorToWorldMatrix(joint);
                var twistLimit = joint.twistLimit;

                var swingLimit_Y = joint.swingLimitY;
                var swingLimit_Z = joint.swingLimitZ;
                var size = HandleUtility.GetHandleSize(joint.transform.position);

                if (_coneLimitDrawer.DrawTwist(twistLimit.low, twistLimit.high, size, anchorToWorld))
                {
                    joint.twistLimit = new JointAngularLimit(_coneLimitDrawer.twistLimitLow, _coneLimitDrawer.twistLimitHigh);
                }

                if (_coneLimitDrawer.DrawSwing1(swingLimit_Y.low, swingLimit_Y.high, size, anchorToWorld))
                {
                    joint.swingLimitY = new JointAngularLimit(_coneLimitDrawer.swingLimitLow_1, _coneLimitDrawer.swingLimitHigh_1);
                }

                if (_coneLimitDrawer.DrawSwing2(swingLimit_Z.low, swingLimit_Z.high, size, anchorToWorld))
                {
                    joint.swingLimitZ = new JointAngularLimit(_coneLimitDrawer.swingLimitLow_2, _coneLimitDrawer.swingLimitHigh_2);
                }

                //Draw limit range always when editing limit.
                if (!SceneView.currentDrawingSceneView.drawGizmos)
                {
                    EllipsoidJoint3DEditor.DrawLimitArc(_joint);
                }
            }
        }
    }
}
