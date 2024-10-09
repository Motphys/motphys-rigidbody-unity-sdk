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
    [CustomEditor(typeof(UniversalJoint3D))]
    internal class UniversalJoint3DEditor : BaseJointEditor
    {
        private Object[] _joints;
        private UniversalJoint3D _universalJoint;

        protected override void OnEnable()
        {
            base.OnEnable();
            _joints = targets;
            _universalJoint = target as UniversalJoint3D;
            if (!Application.isPlaying)
            {
                _universalJoint.TryAutoConfigureConnected();
            }
        }

        protected override void DrawInspector()
        {
            DrawToolbarButton(BaseStyles.EditAnchor);
            s_selected = GUILayout.SelectionGrid(s_selected, _selectBox_v2, 2);
            var option = (MenuOption)s_selected;
            switch (option)
            {
                case MenuOption.Anchor:
                    AnchorGUI();
                    break;
                case MenuOption.Limit:
                    LimitGUI();
                    break;
            }
        }
        protected override void LimitEnabledGroup()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_swingLimitY"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_swingLimitZ"));

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_angularDamper"));
        }

        private void OnSceneGUI()
        {
            var joint = _universalJoint;
            if (joint != null && joint.enabled)
            {
                if (SceneView.currentDrawingSceneView.drawGizmos)
                {
                    DrawLimitArc(joint);
                }
            }
        }

        internal static void DrawLimitArc(UniversalJoint3D joint)
        {
            var anchorToWorld = HandlesHelper.GetAnchorToWorldMatrix(joint);
            var size = HandleUtility.GetHandleSize(joint.transform.position);
            using (new Handles.DrawingScope(BaseJointEditor.s_fadeRed, anchorToWorld))
            {
                var xlow = Mathf.Clamp(joint.swingLimitY.low, -180, 0);
                var xhigh = Mathf.Clamp(joint.swingLimitY.high, 0, 180);
                var ylow = Mathf.Clamp(joint.swingLimitZ.low, -180, 0);
                var yhigh = Mathf.Clamp(joint.swingLimitZ.high, 0, 180);
                var x = new JointAngularLimit(xlow, xhigh);
                var y = new JointAngularLimit(ylow, yhigh);
                var config = new EllipsoidHandleConfig(x, y)
                {
                    scale = size,
                    segCount = 100,
                };
                HandlesHelper.DrawEllipsoidCone(config);

                //Twist axis
                Handles.ArrowHandleCap(0, Vector3.zero, Quaternion.Euler(0, 90, 0), size * 0.75f, EventType.Repaint);
            }
        }

        [EditorTool("Edit Universal Joint", typeof(UniversalJoint3D))]
        internal class UniversalJointEditorTool : JointEditorTool<UniversalJoint3D>
        {
            public override void OnActivated()
            {
                base.OnActivated();
                UniversalJoint3DEditor.s_selected = 0;
            }
            public override void OnWillBeDeactivated()
            {
                base.OnWillBeDeactivated();
            }
        }

        /// <summary>
        /// Draw Universal Limit Gizmos
        /// </summary>
        [EditorTool("Edit Universal Limit", typeof(UniversalJoint3D))]
        [UnityEngine.TestTools.ExcludeFromCoverage]
        internal class UniversalLimitEditorTool : EditorTool
        {
            private static GUIContent s_icon;
            private UniversalJoint3D _joint;
            private ConeLimitDrawer _coneLimitDrawer;

            public override GUIContent toolbarIcon => s_icon ??= new GUIContent()
            {
                image = EditorGUIUtility.IconContent("JointAngularLimits").image,
                text = "Edit Universal Limit",
                tooltip = "Edit Universal Limit",
            };

            public override void OnActivated()
            {
                _joint = target as UniversalJoint3D;
                _coneLimitDrawer = new ConeLimitDrawer();
                UniversalJoint3DEditor.s_selected = 1;
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
                    var swingLimitY = joint.swingLimitY;
                    var swingLimitZ = joint.swingLimitZ;
                    var size = HandleUtility.GetHandleSize(joint.transform.position);

                    if (_coneLimitDrawer.DrawSwing1(swingLimitY.low, swingLimitY.high, size, anchorToWorld))
                    {
                        joint.swingLimitY = new Motphys.Rigidbody.JointAngularLimit(_coneLimitDrawer.swingLimitLow_1, _coneLimitDrawer.swingLimitHigh_1);
                    }

                    if (_coneLimitDrawer.DrawSwing2(swingLimitZ.low, swingLimitZ.high, size, anchorToWorld))
                    {
                        joint.swingLimitZ = new Motphys.Rigidbody.JointAngularLimit(_coneLimitDrawer.swingLimitLow_2, _coneLimitDrawer.swingLimitHigh_2);
                    }

                    //Draw limit range always when editing limit.
                    if (!SceneView.currentDrawingSceneView.drawGizmos)
                    {
                        UniversalJoint3DEditor.DrawLimitArc(joint);
                    }
                }
            }
        }
    }
}
