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

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Motphys.Rigidbody.Editor
{
    [UnityEngine.TestTools.ExcludeFromCoverage]
    internal class MotphysPreference : ScriptableObject
    {
        [SerializeField]
        internal bool _showEditorColliderIndex;
        [SerializeField]
        internal bool _showCenterOfMass;
        [SerializeField]
        internal Color _centerOfMassColor = Color.cyan;
        [SerializeField]
        [Range(0.01f, 0.5f)]
        internal float _centroidRadius = 0.07f;

        internal const string SavePath = "ProjectSettings/Motphys";
        internal const string FileName = "MotphysPreference.json";

        private static MotphysPreference s_motphysPreference;

        internal static MotphysPreference LoadFileFromPath()
        {
            var filePath = Path.Combine(SavePath, FileName);
            if (File.Exists(filePath))
            {
                try
                {
                    s_motphysPreference = ScriptableObject.CreateInstance<MotphysPreference>();
                    var text = File.ReadAllText(filePath);
                    JsonUtility.FromJsonOverwrite(text, s_motphysPreference);
                    return s_motphysPreference;
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                    return default;
                }
            }

            return default;
        }

        internal static MotphysPreference GetOrCreateSettings()
        {
            if (s_motphysPreference != null)
            {
                return s_motphysPreference;
            }

            s_motphysPreference = LoadFileFromPath();
            if (s_motphysPreference == null)
            {
                s_motphysPreference = CreateInstance<MotphysPreference>();
            }

            return s_motphysPreference;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        internal static void SavePreference()
        {
            if (s_motphysPreference == null)
            {
                return;
            }

            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }

            var path = Path.Combine(SavePath, FileName);
            try
            {
                var json = JsonUtility.ToJson(s_motphysPreference);
                File.WriteAllText(path, json);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    [UnityEngine.TestTools.ExcludeFromCoverage]
    internal static class MotphysPreferenceRegister
    {
        private static readonly GUIContent s_showColliderIndexText = new GUIContent("Show Editor Collider Index", "Whether to draw collider index on a game object");
        private static readonly GUIContent s_showCenterOfMassText = new GUIContent("Show Center Of Mass", "Whether to draw the center of mass on a rigidbody");
        private static readonly GUIContent s_centerOfMassColorText = new GUIContent("Center Of Mass Color");
        private static readonly GUIContent s_centroidRadiusText = new GUIContent("The Radius Of Centroid");

        [SettingsProvider]
        private static SettingsProvider MotphysPreferenceProvider()
        {
            var provider = new SettingsProvider("Preferences/Motphys Preference", SettingsScope.User)
            {
                label = "Motphys Preference",
                guiHandler = (searchContext) =>
                {
                    var settings = MotphysPreference.GetSerializedSettings();
                    var labelWidth = EditorGUIUtility.labelWidth;
                    var changed = false;

                    EditorGUIUtility.labelWidth += 10;
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(settings.FindProperty("_showEditorColliderIndex"), s_showColliderIndexText);
                    EditorGUILayout.PropertyField(settings.FindProperty("_showCenterOfMass"), s_showCenterOfMassText);
                    EditorGUILayout.PropertyField(settings.FindProperty("_centerOfMassColor"), s_centerOfMassColorText);
                    EditorGUILayout.PropertyField(settings.FindProperty("_centroidRadius"), s_centroidRadiusText);
                    changed = EditorGUI.EndChangeCheck();

                    EditorGUIUtility.labelWidth = labelWidth;

                    if (changed)
                    {
                        settings.ApplyModifiedPropertiesWithoutUndo();
                        MotphysPreference.SavePreference();
                    }
                },
                keywords = new HashSet<string>(new[] { "Collider", "CenterOfMass" })
            };

            return provider;
        }
    }
}
