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

    // Editor GUI for SpringDamper, exclude from coverage
    [ExcludeFromCoverage]
    [CustomPropertyDrawer(typeof(SpringDamper))]
    internal class SpringDamperDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Generic, label);
            if (property.isExpanded)
            {
                height += EditorGUI.GetPropertyHeight(SerializedPropertyType.Float, label) * 2 + EditorGUIUtility.standardVerticalSpacing * 2;
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Generic, label);
            var rect = new Rect(position.x, position.y, position.width, height);
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);
            rect.y += height + EditorGUIUtility.standardVerticalSpacing;
            if (property.isExpanded)
            {
                var identSpace = 15;
                rect.x += identSpace;
                EditorGUIUtility.labelWidth -= identSpace;
                var compliance = property.FindPropertyRelative("_compliance");
                var damper = property.FindPropertyRelative("_damper");
                var fieldHeight = EditorGUI.GetPropertyHeight(SerializedPropertyType.Float, label);
                rect.height = fieldHeight;
                var stiffness = EditorGUI.FloatField(rect, new GUIContent("Stiffness", "Stiffness is the extent to which an object resists deformation in response to an applied force, which has UNIT of N/m"), 1f / compliance.floatValue);
                stiffness = Mathf.Max(0f, stiffness);
                compliance.floatValue = 1f / stiffness;

                rect.y += fieldHeight;
                rect.y += EditorGUIUtility.standardVerticalSpacing;
                var damperValue = EditorGUI.FloatField(rect, new GUIContent("Damper", damper.tooltip), damper.floatValue);
                damper.floatValue = Mathf.Max(0f, damperValue);

                EditorGUIUtility.labelWidth += identSpace;
            }
        }
    }
}
