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
using UnityEngine.TestTools;
namespace Motphys.Rigidbody.Editor
{
    [ExcludeFromCoverage]
    [CustomPropertyDrawer(typeof(D3AngularMotor))]
    internal class D3AngularMotorDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = 0.0f;
            height += EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, label);
            height += EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, label);
            height += EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, label);

            var mode = property.FindPropertyRelative("mode");
            if (mode.intValue == 0)
            {
                height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("slerpDrive"));
            }
            else if (mode.intValue == 1)
            {
                height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("twistDrive"));
                height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("swingDrive"));
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Generic, label);
            var rect = new Rect(position.x, position.y, position.width, height);
            var mode = property.FindPropertyRelative("mode");
            EditorGUI.PropertyField(rect, mode);
            rect.y += height + EditorGUIUtility.standardVerticalSpacing;

            var rotationProp = property.FindPropertyRelative("targetRotation");
            rotationProp.quaternionValue = MotphysEditorGUI.QuaternionField(rect, "Target Rotation", rotationProp.quaternionValue);

            rect.y += height + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("targetAngularVelocity"));

            var index = mode.enumValueIndex;
            if (index == 0)
            {
                rect.y += height + EditorGUIUtility.standardVerticalSpacing;
                var drive = property.FindPropertyRelative("slerpDrive");
                EditorGUI.PropertyField(rect, drive);
            }
            else if (index == 1)
            {
                rect.y += height + EditorGUIUtility.standardVerticalSpacing;
                var twistDrive = property.FindPropertyRelative("twistDrive");
                EditorGUI.PropertyField(rect, twistDrive);

                var swingDrive = property.FindPropertyRelative("swingDrive");
                rect.y += EditorGUI.GetPropertyHeight(twistDrive);
                EditorGUI.PropertyField(rect, swingDrive);
            }
        }
    }

    [ExcludeFromCoverage]
    [CustomPropertyDrawer(typeof(AngularMotorDrive))]
    internal class AngularMotorDriveDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Generic, label);
            if (property.isExpanded)
            {
                var spring = property.FindPropertyRelative("spring");
                height += EditorGUI.GetPropertyHeight(spring);
                height += EditorGUI.GetPropertyHeight(SerializedPropertyType.Float, label) + EditorGUIUtility.standardVerticalSpacing * 2;
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Generic, label);
            var rect = new Rect(position.x, position.y, position.width, height);
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);
            if (property.isExpanded)
            {
                var identSpace = 15;
                rect.x += identSpace;

                var spring = property.FindPropertyRelative("spring");
                rect.y += height + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(rect, spring);
                if (spring.isExpanded)
                {
                    rect.y += (height + EditorGUIUtility.standardVerticalSpacing) * 3;
                }
                else
                {
                    rect.y += height + EditorGUIUtility.standardVerticalSpacing;
                }

                EditorGUI.PropertyField(rect, property.FindPropertyRelative("maxTorque"));
            }
        }
    }
}
