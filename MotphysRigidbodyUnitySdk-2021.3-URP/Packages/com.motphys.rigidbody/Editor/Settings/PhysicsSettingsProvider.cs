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

using Motphys.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Motphys.Rigidbody.Editor
{
    [ExcludeFromCoverage]
    public class PhysicsSettingsProvider : SettingsProvider
    {
        private class Styles
        {
            public static GUIContent broadPhaseType = new GUIContent("BroadPhase");
        }

        private PhysicsProjectSettings _settings;
        private UnityEditor.Editor _settingsEditor;
        private bool _expandMatrix = true;

        private PhysicsSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
        }

        public override void OnGUI(string searchContext)
        {
            if (_settings == null)
            {
                _settings = CustomProjectSettings.Get<PhysicsProjectSettings>();
                if (_settings == null)
                {
                    throw new System.Exception("failed to load PhysicsProjectSettings");
                }
            }

            if (_settings == null)
            {
                return;
            }

            if (_settingsEditor == null || _settingsEditor.target == null)
            {
                _settingsEditor = UnityEditor.Editor.CreateEditor(_settings);
            }

            _settingsEditor?.OnInspectorGUI();

            EditorGUILayout.Space(20);
            _expandMatrix = EditorGUILayout.BeginFoldoutHeaderGroup(_expandMatrix, "Collision Matrix");
            if (_expandMatrix)
            {
                Draw(GetValue, SetValue, SetAllValues);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void SetAllValues(bool value)
        {
            _settings.SetAllLayersCollisionMask(value);
            EditorUtility.SetDirty(_settings);
        }

        private bool GetValue(int i, int j)
        {
            if (i < PhysicsProjectSettings.MaxLayers && j < PhysicsProjectSettings.MaxLayers)
            {
                var layerMask = _settings.GetCollisionMask(i);
                var bit = 1 << j;
                return (layerMask & bit) != 0;
            }

            return false;
        }

        private void SetValue(int i, int j, bool value)
        {
            if (_settings.IgnoreLayerCollision(i, j, !value))
            {
                EditorUtility.SetDirty(_settings);
            }
        }

        private delegate bool GetValueFunc(int layerA, int layerB);
        private delegate void SetValueFunc(int layerA, int layerB, bool value);
        private delegate void SetAllValue(bool value);

        private static void Draw(GetValueFunc getValue, SetValueFunc setValue, SetAllValue setAll)
        {
            var dontExpand = GUILayout.ExpandWidth(false);
            var labelWidth = 50.0f;
            var labelInterval = 3.0f;
            var minWidth = GUILayout.MinWidth(labelWidth);
            var maxWidth = GUILayout.MaxWidth(labelWidth);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(labelWidth + 10.0f);

            for (var i = PhysicsProjectSettings.MaxLayers - 1; i >= 0; --i)
            {
                if (LayerMask.LayerToName(i) != string.Empty)
                {
                    EditorGUILayout.LabelField(LayerMask.LayerToName(i), dontExpand, minWidth, maxWidth);
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            var lessWhite = new Color(0.9f, 0.9f, 0.9f, 1.0f);
            for (var i = 0; i < PhysicsProjectSettings.MaxLayers; ++i)
            {
                var layerName = LayerMask.LayerToName(i);
                if (layerName != string.Empty)
                {
                    GUI.color = i % 2 == 0 ? lessWhite : Color.white;
                    EditorGUILayout.BeginHorizontal("frameBox");
                    EditorGUILayout.LabelField(LayerMask.LayerToName(i), dontExpand, minWidth, maxWidth);

                    var rect = EditorGUILayout.GetControlRect();
                    rect.width = labelWidth - 5.0f;
                    rect.height = EditorGUIUtility.singleLineHeight;

                    for (var j = PhysicsProjectSettings.MaxLayers - 1; j >= i; --j)
                    {
                        var anotherName = LayerMask.LayerToName(j);
                        if (anotherName != string.Empty)
                        {
                            var guiContent = new GUIContent("", $"{layerName}/{anotherName}");
                            var flag = getValue(i, j);
                            EditorGUI.BeginChangeCheck();

                            EditorGUI.LabelField(rect, guiContent);
                            var toggle = EditorGUI.ToggleLeft(rect, "", flag);
                            if (EditorGUI.EndChangeCheck())
                            {
                                setValue(i, j, toggle);
                            }

                            rect.x += labelWidth + labelInterval;
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Enable All"))
            {
                setAll(true);
            }

            if (GUILayout.Button("Disable All"))
            {
                setAll(false);
            }

            EditorGUILayout.EndHorizontal();
        }

        [SettingsProvider]
        public static SettingsProvider Create()
        {
            var provider = new PhysicsSettingsProvider("Project/Motphys", SettingsScope.Project)
            {
                // Automatically extract all keywords from the Styles.
                keywords = GetSearchKeywordsFromGUIContentProperties<Styles>()
            };
            return provider;
        }
    }
}
