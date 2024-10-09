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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a `unified-ci rust pack`.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

#pragma warning disable CS0162 // Unreachable code detected

namespace Motphys.Rigidbody.Native.Standard.Editor
{
    internal class NativePluginSwitcher : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var folder = $"Packages/{GetPackageId()}";
            Log($"Trying to find native plugin in {folder}");
            var guids = AssetDatabase.FindAssets("*", new string[] { folder });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = AssetImporter.GetAtPath(path) as PluginImporter;
                if (importer != null && importer.isNativePlugin && importer.GetCompatibleWithPlatform(report.summary.platform))
                {
                    Log($"Found native plugin at {path} which is compatible with platform {report.summary.platform}");

                    var shouldInclude = ShouldIncludeInBuild(path, report.summary.options);
                    if (shouldInclude)
                    {
                        Log($"It should be included.");
                    }
                    else
                    {
                        Log($"However it should NOT be included.");
                    }

                    importer.SetIncludeInBuildDelegate((path) => shouldInclude);
                }
            }
        }

        static void Log(string message)
        {
            Debug.Log($"[Motphys Native Plugin Switcher] {message}");
        }
        
        private const bool ForEditorOnly = false;

        private const bool ForDevelopmentBuildOnly = false;

        private const bool ForReleaseBuildOnly = true;

        private string GetPackageId() => "com.motphys.rigidbody.native.standard";

        private bool ShouldIncludeInBuild(string path, BuildOptions buildOptions)
        {
            if (ForEditorOnly)
            {
                return false;
            }

            bool isDevelopmentBuild = (buildOptions & BuildOptions.Development) == BuildOptions.Development;

            if (ForDevelopmentBuildOnly && !isDevelopmentBuild)
            {
                return false;
            }

            if (ForReleaseBuildOnly && isDevelopmentBuild)
            {
                return false;
            }

#if (TRUE) && !((MOTPHYS_RIGIDBODY_DETERMINISTIC))
            return true;
#endif

            return false;
        }
    }
}