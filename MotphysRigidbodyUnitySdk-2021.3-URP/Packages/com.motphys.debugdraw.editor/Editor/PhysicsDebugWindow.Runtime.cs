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
using UnityEngine;

namespace Motphys.DebugDraw.Editor
{
    internal partial class PhysicsDebugWindow
    {
        private bool _showRuntimeColors;

        private void RuntimePanel()
        {
            var flag = Application.isPlaying;

            if (!flag)
            {
                EditorGUILayout.HelpBox("Only available in runtime", MessageType.Warning);
            }

            using (new EditorGUILayout.VerticalScope("frameBox"))
            {
                EditorDebugSettings.showCollisionPair = EditorGUILayout.Toggle("Show Collision Pair", EditorDebugSettings.showCollisionPair);
                EditorDebugSettings.showJointPair = EditorGUILayout.Toggle("Show Joint Pair", EditorDebugSettings.showJointPair);
                EditorDebugSettings.showAabb = EditorGUILayout.Toggle("Show Aabb", EditorDebugSettings.showAabb);
                EditorDebugSettings.showContacts = EditorGUILayout.Toggle("Show Contacts", EditorDebugSettings.showContacts);

                EditorGUILayout.Space(2);

                _showRuntimeColors = EditorGUILayout.BeginFoldoutHeaderGroup(_showRuntimeColors, "Colors");
                if (_showRuntimeColors)
                {
                    EditorDebugSettings.aabbColor = EditorGUILayout.ColorField(EditorGUIUtility.TrTempContent("Aabb Color"), EditorDebugSettings.aabbColor, true, false, false);
                    EditorDebugSettings.collisionPairColor = EditorGUILayout.ColorField(EditorGUIUtility.TrTempContent("Collision Pair Color"), EditorDebugSettings.collisionPairColor, true, false, false);
                    EditorDebugSettings.contactPointColor = EditorGUILayout.ColorField(EditorGUIUtility.TrTempContent("Contact Point Color"), EditorDebugSettings.contactPointColor, true, false, false);
                    EditorDebugSettings.contactLineColor = EditorGUILayout.ColorField(EditorGUIUtility.TrTempContent("Contact Line Color"), EditorDebugSettings.contactLineColor, true, false, false);
                    EditorDebugSettings.jointPairColor = EditorGUILayout.ColorField(EditorGUIUtility.TrTempContent("Joint Pair Color"), EditorDebugSettings.jointPairColor, true, false, false);
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }

        private void RuntimeDraw()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (_drawCollisionInfo && RuntimeGizmosEditorUse.Instance != null)
            {
                var instance = RuntimeGizmosEditorUse.Instance;

                instance.drawAabb = EditorDebugSettings.showAabb;
                instance.drawCollisionPair = EditorDebugSettings.showCollisionPair;
                instance.drawJointPair = EditorDebugSettings.showJointPair;
                instance.drawContacts = EditorDebugSettings.showContacts;
                instance.aabbColor = EditorDebugSettings.aabbColor;

                instance.collisionPairColor = EditorDebugSettings.collisionPairColor;
                instance.contactLineColor = EditorDebugSettings.contactLineColor;
                instance.contactPointColor = EditorDebugSettings.contactPointColor;
                instance.jointPairColor = EditorDebugSettings.jointPairColor;

                instance.DrawRuntimeGizmos();
            }
        }
    }
}
