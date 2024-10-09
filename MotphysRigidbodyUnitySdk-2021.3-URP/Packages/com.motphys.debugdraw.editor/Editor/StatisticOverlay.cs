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

using Motphys.Rigidbody;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.TestTools;

namespace Motphys.DebugDraw.Editor
{
    [ExcludeFromCoverage]
    [Overlay(typeof(SceneView), "MotphysStatistic", "Motphys Statistics", false)]
    internal class StatisticOverlay : IMGUIOverlay, ITransientOverlay
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        internal static void CreateGameViewPanel()
        {
            if (s_statistics)
            {
                var instance = new GameObject("MotphysStatistic");
                instance.AddComponent<MotphysStatistic>();
                MonoBehaviour.DontDestroyOnLoad(instance);
            }
        }

        private static bool s_statistics = false;

        private const string Statistics = "Window/Analysis/Motphys Stats";

        private StepMetrics _metrics;
        private float _fps;
        private GUIStyle _guiStyle;

        public bool visible => s_statistics;

        internal StatisticOverlay()
        {
            s_statistics = UnityEditor.Menu.GetChecked(Statistics);

            PhysicsManager.requestMetrics += OnReceiveMetrics;
        }

        private void OnReceiveMetrics(StepMetrics metrics)
        {
            _metrics = metrics;
            PhysicsManager.requestMetrics += OnReceiveMetrics;
        }

        [UnityEditor.MenuItem(Statistics, false, 11)]
        internal static void StatisticToggle()
        {
            s_statistics = !s_statistics;
            UnityEditor.Menu.SetChecked(Statistics, s_statistics);
        }

        public override void OnGUI()
        {
            _fps = 1.0f / Time.deltaTime;

            if (_guiStyle == null)
            {
                _guiStyle = new GUIStyle();
                _guiStyle.fontSize = 12;
                _guiStyle.normal.textColor = Color.gray;
            }

            MotphysStatistic.DrawStatistics(_fps, _metrics, _guiStyle, s_statistics);
        }
    }
}

