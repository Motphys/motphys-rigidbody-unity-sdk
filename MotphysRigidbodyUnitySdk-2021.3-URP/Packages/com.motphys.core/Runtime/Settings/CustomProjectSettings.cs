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
using System.Linq;
using UnityEngine;

namespace Motphys.Settings
{
    /// <summary>
    /// An abstract base class for managing custom project settings within Unity.
    /// This class handles the loading and saving of settings as <see href="https://docs.unity3d.com/Manual/class-ScriptableObject.html">ScriptableObject</see>s,
    /// and ensures they are available at runtime or during editor sessions.
    /// </summary>
    public abstract class CustomProjectSettings : ScriptableObject
    {

        private const string EDITOR_PATH = "Assets/Settings";

        private static Dictionary<System.Type, CustomProjectSettings> s_settings = new Dictionary<System.Type, CustomProjectSettings>();

        /// <summary>
        /// Method to be called after the settings have been loaded.
        /// Implement this method to define custom actions that should take place when the settings are loaded.
        /// </summary>
        protected abstract void OnLoaded();

        /// <summary>
        /// Method to be called before the settings are saved.
        /// Implement this method to define custom actions that should take place right before the settings are saved.
        /// </summary>
        protected abstract void OnWillSave();

        private static void EnsurePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static string GetFileName(System.Type customProjectSettingType)
        {
            return customProjectSettingType.Name + ".asset";
        }

#if UNITY_EDITOR
        private static string GetEditorSavePath(System.Type customProjectSettingType)
        {
            return Path.Combine(EDITOR_PATH, GetFileName(customProjectSettingType));
        }

        private static void SaveAll()
        {
            foreach (var kv in s_settings)
            {
                var setting = kv.Value;
                setting.OnWillSave();
                UnityEditor.AssetDatabase.SaveAssetIfDirty(setting);
            }
        }

        private static T LoadCustomProjectSettingsEditor<T>(string fileName) where T : CustomProjectSettings
        {
            EnsurePath(EDITOR_PATH);

            var filePath = Path.Combine(EDITOR_PATH, fileName);
            var settings = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(filePath);
            if (settings == null)
            {
                settings = CreateInstance<T>();
                UnityEditor.AssetDatabase.CreateAsset(settings, filePath);
            }

            var preloadedAssets = UnityEditor.PlayerSettings.GetPreloadedAssets().ToList();
            if (!preloadedAssets.Contains(settings))
            {
                preloadedAssets.Add(settings);
                UnityEditor.PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            }

            return settings;
        }

        private static void SetPreloadedAssets(UnityEditor.TypeCache.TypeCollection projectSettingTypes)
        {
            EnsurePath(EDITOR_PATH);

            var preloadedAssets = UnityEditor.PlayerSettings.GetPreloadedAssets().ToList();

            foreach (var type in projectSettingTypes)
            {
                var fileName = GetFileName(type);
                var filePath = Path.Combine(EDITOR_PATH, fileName);
                var settings = UnityEditor.AssetDatabase.LoadAssetAtPath(filePath, type);
                if (settings == null)
                {
                    settings = CreateInstance(type);
                    UnityEditor.AssetDatabase.CreateAsset(settings, filePath);
                }

                if (!preloadedAssets.Contains(settings))
                {
                    preloadedAssets.Add(settings);
                }
            }

            UnityEditor.PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
        }
#endif

        private static T LoadCustomProjectSettings<T>(string fileName) where T : CustomProjectSettings
        {
#if UNITY_EDITOR
            return LoadCustomProjectSettingsEditor<T>(fileName);
#else
            return null;
#endif
        }

        /// <summary>
        /// Retrieves the instance of the specified custom project settings type.
        /// If the settings have not been loaded yet, this method loads or creates them.
        /// </summary>
        /// <typeparam name="T">The type of custom project settings to retrieve.</typeparam>
        /// <returns>The instance of the specified custom project settings type.</returns>
        public static T Get<T>() where T : CustomProjectSettings
        {
            var type = typeof(T);
            if (s_settings.TryGetValue(type, out var setting))
            {
                if (setting)
                {
                    return setting as T;
                }

                s_settings.Remove(type);
            }

            var name = GetFileName(type);
            setting = LoadCustomProjectSettings<T>(name);
            if (setting == null)
            {
                setting = CreateInstance<T>();
            }

            setting.OnLoaded();
            s_settings.Add(type, setting);
            return setting as T;
        }
    }
}
