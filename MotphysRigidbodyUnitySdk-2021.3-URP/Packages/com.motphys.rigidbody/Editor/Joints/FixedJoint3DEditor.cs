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

namespace Motphys.Rigidbody.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FixedJoint3D))]
    internal class FixedJoint3DEditor : BaseJointEditor
    {
        private FixedJoint3D _fixedJoint;
        protected override bool onlyAnchor => true;
        protected override bool useDetailGUI => false;
        protected override void AnchorFrameGUI(SerializedObject serializedObject) { }
        protected override void ConnectedAnchorFrameGUI(SerializedObject serializedObject) { }

        protected override void OnEnable()
        {
            base.OnEnable();
            _fixedJoint = target as FixedJoint3D;
        }

        public override void OnInspectorGUI()
        {
            if (_fixedJoint == null)
            {
                return;
            }

            base.OnInspectorGUI();
        }

        private void OnSceneGUI()
        {
            if (_fixedJoint != null && _fixedJoint.enabled && SceneView.currentDrawingSceneView.drawGizmos && _fixedJoint.connectedBody != null)
            {
                var head = _fixedJoint.transform.position;
                var tail = _fixedJoint.connectedBody.transform.position;
                Handles.DrawDottedLine(head, tail, 1.2f);
            }
        }
    }

    [EditorTool("Edit Fixed Joint", typeof(FixedJoint3D))]
    internal class FixedJointEditorTool : JointEditorTool<FixedJoint3D>
    {
    }
}
