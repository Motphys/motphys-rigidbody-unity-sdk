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
    [UnityEngine.TestTools.ExcludeFromCoverage]
    internal static class MotphysEditorGUI
    {
        public static Quaternion QuaternionField(Rect rect, string label, Quaternion value)
        {
            const float coordinateLabelWidth = 13.5f;
            var tempWidth = EditorGUIUtility.labelWidth;

            EditorGUI.LabelField(rect, label);

            var elementCount = 3.0f;
            var subSpace = 2.0f;
            var width = (rect.width - EditorGUIUtility.labelWidth - subSpace * (elementCount + 1)) / elementCount;
            var numberRect = new Rect(rect) { width = width };
            numberRect.x += EditorGUIUtility.labelWidth + subSpace;
            EditorGUIUtility.labelWidth = coordinateLabelWidth;

            var euler = value.eulerAngles;

            EditorGUI.BeginChangeCheck();
            var x = EditorGUI.DelayedFloatField(numberRect, "X", euler.x);
            numberRect.x += width + subSpace + 1;
            var y = EditorGUI.DelayedFloatField(numberRect, "Y", euler.y);
            numberRect.x += width + subSpace + 1.5f;
            var z = EditorGUI.DelayedFloatField(numberRect, "Z", euler.z);

            if (EditorGUI.EndChangeCheck())
            {
                value = Quaternion.Euler(x, y, z);
            }

            EditorGUIUtility.labelWidth = tempWidth;
            return value;
        }
    }

    // Editor GUI Code, exclude from test coverage
    [UnityEngine.TestTools.ExcludeFromCoverage]
    internal static class MotphysEditorGUILayout
    {
        public static Quaternion QuaternionField(string label, Quaternion value)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(SerializedPropertyType.Generic, EditorGUIUtility.TrTempContent(label)), EditorStyles.numberField);
            return MotphysEditorGUI.QuaternionField(rect, label, value);
        }
    }

    // Editor GUI Code, exclude from test coverage
    [UnityEngine.TestTools.ExcludeFromCoverage]
    internal class BaseJointEditor : UnityEditor.Editor
    {
        protected string[] _linearMotorTypes = new string[] { "Distance Drive", "Speed Drive" };
        protected string[] _d3angularMotorTypes = new string[] { "Rotation Drive", "Angular Velocity Drive" };
        protected string[] _angularMotorTypes = new string[] { "Angle Drive", "Speed Drive" };

        protected GUIContent _linearDistanceStrengthText = new GUIContent("Strength", "A larger value means that the objects can reach the target distance more powerfully.");
        protected GUIContent _linearDistanceDamperText = new GUIContent("Damper", "A smaller value means that the objects can reach the target distance more quickly.");

        protected GUIContent _angularAngleStrengthText = new GUIContent("Strength", "A larger value means that the objects can reach the target angle more powerfully.");
        protected GUIContent _angularAngleDamperText = new GUIContent("Damper", "A smaller value means that the objects can reach the target angle more quickly.");

        protected GUIContent _linearSpeedStrengthText = new GUIContent("Strength", "A larger value means that the objects can reach the target speed more quickly.");
        protected GUIContent _targetDistanceText = new GUIContent("Target Distance", "The two objects will try to move to satisfy the target distance");
        protected GUIContent _targetSpeedText = new GUIContent("Target Speed", "A positive value indicates that the distance tends to increase, while a negative value indicates that the distance tends to decrease");

        protected GUIContent _d3TwistRotationStrengthText = new GUIContent("Twist Strength", "A larger value means that the objects can reach the target twist rotation more powerfully.");
        protected GUIContent _d3SwingRotationStrengthText = new GUIContent("Swing Strength", "A larger value means that the objects can reach the target swing rotation more powerfully.");
        protected GUIContent _d3SlerpRotationStrengthText = new GUIContent("Slerp Strength", "A larger value means that the objects can reach the target rotation more powerfully.");
        protected GUIContent _d3TwistRotationDamperText = new GUIContent("Twist Damper", "A smaller value means that the objects can reach the target twist rotation more quickly.");
        protected GUIContent _d3SwingRotationDamperText = new GUIContent("Swing Damper", "A smaller value means that the objects can reach the target swing rotation more quickly.");
        protected GUIContent _d3SlerpRotationDamperText = new GUIContent("Slerp Damper", "A smaller value means that the objects can reach the target rotation more quickly.");

        protected GUIContent _d3VelocityStrengthText = new GUIContent("Velocity Strength", "A larger value means that the objects can reach the target velocity more quickly.");

        protected virtual bool onlyAnchor => false;
        protected virtual bool useDetailGUI => true;

        [SerializeField]
        protected SpringDamper _limitSpring = SpringDamper.InfiniteStiffness;
        protected SerializedObject _editorObject;

        protected virtual void OnEnable()
        {
            _editorObject = new SerializedObject(this);
            _editorObject.FindProperty("_limitSpring").isExpanded = true;
        }

        internal enum MenuOption
        {
            Anchor,
            Limit,
            Motor
        }

        internal static int s_selected;

        protected string[] _selectBox = { "Anchor", "Limit", "Motor" };
        protected string[] _selectBox_v2 = { "Anchor", "Limit" };
        protected static class BaseStyles
        {
            public static readonly GUIContent EditAnchor = EditorGUIUtility.TrTextContent("Edit Anchor", "Press this button to Edit the Joint's anchor or limit. " +
                "Adjust the order of the joint component to be the first to activate the button.");
        }
        public static Color s_fadeRed = new Color(1.0f, 0, 0, 0.3f);

        protected static Color _anchorLineColorA = Color.white;
        protected static Color _anchorLineColorB = Color.white;
        protected static Color _twoAnchorLinkLineColor = Color.red;
        protected static float _anchorHandleSizeA = 0.1f;
        protected static float _anchorHandleSizeB = 0.1f;

        private static bool s_debugFoldout = false;
        private bool _showDetails = false;

        protected void DrawToolbarButton(GUIContent buttonName)
        {
            EditorGUI.BeginChangeCheck();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(buttonName, GUILayout.Width(EditorGUIUtility.labelWidth - 4f));
                EditorGUILayout.EditorToolbarForTarget(this);
            }

            EditorGUILayout.Space(5f);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            DrawInspector();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(this.target);
            }

            if (Application.isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);

                s_debugFoldout = EditorGUILayout.Foldout(s_debugFoldout, "Debug");
                if (s_debugFoldout)
                {
                    var joint = target as BaseJoint;
                    EditorGUILayout.Vector3Field("Force (N)", joint.force);
                    EditorGUILayout.Vector3Field("Torque (N * M)", joint.torque);
                }

                EditorGUI.EndDisabledGroup();
            }
        }

        protected virtual void DrawInspector()
        {
            if (onlyAnchor)
            {
                AnchorGUI();
            }
            else
            {
                DrawToolbarButton(BaseStyles.EditAnchor);
                s_selected = GUILayout.SelectionGrid(s_selected, _selectBox, 3);
                var option = (MenuOption)s_selected;
                switch (option)
                {
                    case MenuOption.Anchor:
                        AnchorGUI();
                        break;
                    case MenuOption.Limit:
                        LimitGUI();
                        break;
                    case MenuOption.Motor:
                        MotorGUI();
                        break;
                }
            }
        }

        protected virtual void AnchorFrameGUI(SerializedObject serializedObject)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_anchorFrame"));
        }

        protected virtual void ConnectedAnchorFrameGUI(SerializedObject serializedObject)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_connectedAnchorFrame"));
        }
        protected virtual void AnchorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_connectBody"));

            AnchorFrameGUI(serializedObject);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_ignoreCollision"));

            EditorGUILayout.BeginHorizontal();
            var breakForce = serializedObject.FindProperty("_breakForce");
            EditorGUILayout.PropertyField(breakForce);
            if (GUILayout.Button("Infinity"))
            {
                breakForce.floatValue = float.PositiveInfinity;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            var breakTorque = serializedObject.FindProperty("_breakTorque");
            EditorGUILayout.PropertyField(breakTorque);
            if (GUILayout.Button("Infinity"))
            {
                breakTorque.floatValue = float.PositiveInfinity;
            }

            EditorGUILayout.EndHorizontal();

            if (useDetailGUI)
            {
                EditorGUILayout.Space(10);
                _showDetails = EditorGUILayout.BeginFoldoutHeaderGroup(_showDetails, "Details");
                if (_showDetails)
                {
                    EditorGUI.indentLevel += 1;
                    var useAuto = serializedObject.FindProperty("_autoConfigureConnected");
                    EditorGUILayout.PropertyField(useAuto);

                    EditorGUI.BeginDisabledGroup(useAuto.boolValue);
                    ConnectedAnchorFrameGUI(serializedObject);
                    EditorGUI.EndDisabledGroup();

                    var numPosSolverIter = serializedObject.FindProperty("_numPosSolverIter");
                    EditorGUILayout.PropertyField(numPosSolverIter);

                    var inertiaScaleA = serializedObject.FindProperty("_inertiaScaleA");
                    var inertiaScaleB = serializedObject.FindProperty("_inertiaScaleB");
                    EditorGUILayout.PropertyField(inertiaScaleA);
                    EditorGUILayout.PropertyField(inertiaScaleB);

                    EditorGUI.indentLevel -= 1;
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }

        protected virtual void LimitGUI()
        {
            var useLimit = serializedObject.FindProperty("_useLimit");
            if (useLimit != null)
            {
                EditorGUILayout.PropertyField(useLimit);
                if (useLimit.boolValue)
                {
                    LimitEnabledGroup();
                }
                else
                {
                    EditorGUI.BeginDisabledGroup(!useLimit.boolValue);
                    LimitDisabledGroup();
                    EditorGUI.EndDisabledGroup();
                }
            }
            else
            {
                LimitEnabledGroup();
            }

            LimitDamperGUI();
        }

        protected virtual void MotorGUI()
        {
            var useMotor = serializedObject.FindProperty("_useMotor");
            if (useMotor != null)
            {
                EditorGUILayout.PropertyField(useMotor);
                EditorGUI.BeginDisabledGroup(!useMotor.boolValue);
                MotorDisabledGroup();
                EditorGUI.EndDisabledGroup();
            }
        }
        protected virtual void LimitDamperGUI() { }
        protected virtual void LimitDisabledGroup() { }
        protected virtual void LimitEnabledGroup() { }
        protected virtual void MotorDisabledGroup()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_motor"));
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.InSelectionHierarchy | GizmoType.Active)]
        private static void DrawGizmo(BaseJoint joint, GizmoType _)
        {

        }

        private static void DrawJointAnchor(BaseJoint joint)
        {
            var positionA = joint.transform.position;
            var rotationA = joint.transform.rotation;
            var anchorPositionA = joint.transform.TransformPoint(joint.anchorPosition);
            var anchorRotationA = rotationA * joint.anchorFrame.ToIsometry().rotation;
            using (new Handles.DrawingScope(_anchorLineColorA))
            {
                Handles.SphereHandleCap(0, positionA, rotationA, _anchorHandleSizeA * HandleUtility.GetHandleSize(positionA), EventType.Repaint);
                Handles.DrawDottedLine(positionA, anchorPositionA, 2f);
            }

            if (joint.connectedBody)
            {
                var positionB = joint.connectedBody.transform.position;
                var rotationB = joint.connectedBody.transform.rotation;
                var anchorPositionB = joint.connectedBody.transform.TransformPoint(joint.connectedAnchorPosition);
                var anchorRotationB = rotationB * joint.connectedAnchorFrame.ToIsometry().rotation;
                using (new Handles.DrawingScope(_anchorLineColorB))
                {
                    Handles.SphereHandleCap(0, positionB, rotationB, _anchorHandleSizeB * HandleUtility.GetHandleSize(positionB), EventType.Repaint);
                    Handles.DrawDottedLine(positionB, anchorPositionB, 2f);
                }

                using (new Handles.DrawingScope(_twoAnchorLinkLineColor))
                {
                    Handles.SphereHandleCap(0, anchorPositionA, anchorRotationA, _anchorHandleSizeA * HandleUtility.GetHandleSize(anchorPositionA), EventType.Repaint);
                    Handles.SphereHandleCap(0, anchorPositionB, anchorRotationB, _anchorHandleSizeB * HandleUtility.GetHandleSize(anchorPositionB), EventType.Repaint);
                    Handles.DrawLine(anchorPositionA, anchorPositionB, 1f);
                }
            }
        }
    }

    // Editor Tool Code, exclude from test coverage
    [UnityEngine.TestTools.ExcludeFromCoverage]
    internal abstract class JointEditorTool<T> : EditorTool where T : BaseJoint
    {
        protected Vector3 _anchorHandlePositionA;
        protected Quaternion _anchorHandleRotationA;
        protected Vector3 _anchorHandlePositionB;
        protected Quaternion _anchorHandleRotationB;

        protected virtual bool drawRotationHandles => true;

        private static GUIContent s_icon;
        public override GUIContent toolbarIcon => s_icon ??= new GUIContent()
        {
            image = EditorGUIUtility.IconContent("AvatarPivot").image,
            text = "Edit Joint Anchor",
            tooltip = "Edit Joint Anchor",
        };

        public override void OnToolGUI(EditorWindow window)
        {
            var joint = target as T;

            Undo.RecordObject(joint, "Motphys:Edit Joint Anchor");
            if (joint == null || Mathf.Approximately(joint.transform.lossyScale.sqrMagnitude, 0f))
            {
                return;
            }

            DrawWireframe(joint);

            if (!Application.isPlaying)
            {
                EditorGUI.BeginChangeCheck();

                DrawHandles(joint);

                if (EditorGUI.EndChangeCheck())
                {
                    UpdateProperties(joint);
                }
            }
        }

        protected virtual void DrawHandles(T joint)
        {
            using (new Handles.DrawingScope(Matrix4x4.TRS(joint.transform.position, joint.transform.rotation, Vector3.one)))
            {
                var size = 1.0f * 0.75f;
                var lossyScale = joint.transform.lossyScale;
                var position = Vector3.Scale(joint.anchorPosition, lossyScale);
                var rotation = joint.anchorFrame.ToIsometry().rotation;
                _anchorHandlePositionA = HandlesHelper.PositionHandle(position, rotation, size);
                if (drawRotationHandles)
                {
                    _anchorHandleRotationA = HandlesHelper.RotationHandle(position, rotation, size);
                }
            }
        }

        protected virtual void DrawWireframe(T joint)
        {
        }

        protected virtual void UpdateProperties(T joint)
        {
            joint.anchorFrame = new AxisFrame()
            {
                anchor = Vector3.Scale(_anchorHandlePositionA, InvertScaleVector(joint.transform.lossyScale)),
                axisRotation = _anchorHandleRotationA.eulerAngles,
            };

            joint.TryAutoConfigureConnected();
        }

        protected Vector3 InvertScaleVector(Vector3 scaleVector)
        {
            for (var i = 0; i < 3; i++)
            {
                scaleVector[i] = (scaleVector[i] == 0f) ? 0f : (1f / scaleVector[i]);
            }

            return scaleVector;
        }
    }
}
