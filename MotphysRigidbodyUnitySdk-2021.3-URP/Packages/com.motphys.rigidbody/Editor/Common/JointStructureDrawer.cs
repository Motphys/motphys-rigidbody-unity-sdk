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
    [CustomPropertyDrawer(typeof(AxisFrame))]
    public class AxisFrameDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var titleHeight = EditorGUI.GetPropertyHeight(SerializedPropertyType.Generic, label);
            var vector3Height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, label);
            return titleHeight + vector3Height * 2 + 4;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Generic, label);
            var vector3Height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, label);
            var rect = new Rect(position.x, position.y, position.width, height);
            var anchorProperty = property.FindPropertyRelative("anchor");
            var axisRotationProperty = property.FindPropertyRelative("axisRotation");

            EditorGUI.LabelField(rect, label);

            EditorGUI.indentLevel++;

            rect.y += vector3Height + 2;
            EditorGUI.PropertyField(rect, anchorProperty);
            rect.y += vector3Height + 2;
            EditorGUI.PropertyField(rect, axisRotationProperty);

            EditorGUI.indentLevel--;
        }
    }

    [ExcludeFromCoverage]
    [CustomPropertyDrawer(typeof(JointAngularLimit))]
    public class JointAngularLimitDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var titleHeight = EditorGUI.GetPropertyHeight(SerializedPropertyType.Generic, label);
            var floatHeight = EditorGUI.GetPropertyHeight(SerializedPropertyType.Float, label);
            return titleHeight + floatHeight * 2 + 4;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Generic, label);
            var floatHeight = EditorGUI.GetPropertyHeight(SerializedPropertyType.Float, label);
            var rect = new Rect(position.x, position.y, position.width, height);
            var lowProperty = property.FindPropertyRelative("_low");
            var highProperty = property.FindPropertyRelative("_high");

            EditorGUI.LabelField(rect, label);

            EditorGUI.indentLevel++;

            rect.y += floatHeight + 2;
            EditorGUI.PropertyField(rect, lowProperty);
            rect.y += floatHeight + 2;
            EditorGUI.PropertyField(rect, highProperty);

            EditorGUI.indentLevel--;
        }
    }

    [ExcludeFromCoverage]
    [CustomPropertyDrawer(typeof(JointTranslationLimit))]
    public class JointTranslationLimitDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var titleHeight = EditorGUI.GetPropertyHeight(SerializedPropertyType.Generic, label);
            var floatHeight = EditorGUI.GetPropertyHeight(SerializedPropertyType.Float, label);
            return titleHeight + floatHeight * 2 + 4;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Generic, label);
            var floatHeight = EditorGUI.GetPropertyHeight(SerializedPropertyType.Float, label);
            var rect = new Rect(position.x, position.y, position.width, height);
            var lowProperty = property.FindPropertyRelative("_low");
            var highProperty = property.FindPropertyRelative("_high");

            EditorGUI.LabelField(rect, label);

            EditorGUI.indentLevel++;

            rect.y += floatHeight + 2;
            EditorGUI.PropertyField(rect, lowProperty);
            rect.y += floatHeight + 2;
            EditorGUI.PropertyField(rect, highProperty);

            EditorGUI.indentLevel--;
        }
    }
}

