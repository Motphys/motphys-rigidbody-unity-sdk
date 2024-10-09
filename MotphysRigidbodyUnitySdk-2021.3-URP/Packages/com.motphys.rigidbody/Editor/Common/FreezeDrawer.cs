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
    [CustomPropertyDrawer(typeof(FreezeOptions))]
    internal class FreezeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.LabelField(position, label);

            var height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Boolean, label);
            var rect = new Rect(position.x, position.y, height * 2, height);

            var identSpace = 45;
            rect.x += EditorGUIUtility.labelWidth + 2;

            var toggleX = property.FindPropertyRelative("x");

            EditorGUI.showMixedValue = toggleX.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var x = EditorGUI.ToggleLeft(rect, "X", toggleX.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                toggleX.boolValue = x;
            }

            rect.x += identSpace;
            var toggleY = property.FindPropertyRelative("y");
            EditorGUI.showMixedValue = toggleY.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var y = EditorGUI.ToggleLeft(rect, "Y", toggleY.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                toggleY.boolValue = y;
            }

            rect.x += identSpace;
            var toggleZ = property.FindPropertyRelative("z");
            EditorGUI.showMixedValue = toggleZ.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var z = EditorGUI.ToggleLeft(rect, "Z", toggleZ.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                toggleZ.boolValue = z;
            }

            EditorGUI.showMixedValue = false;
        }
    }
}

