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
using UnityEngine;

namespace Motphys.DebugDraw.Editor
{
    public class MotphysStatistic : MonoBehaviour
    {
        private float _fps;
        private StepMetrics _metrics;

        private GUIStyle _gameViewStyle;

        private void Start()
        {
            _gameViewStyle = new GUIStyle();
            _gameViewStyle.fontSize = 25;
            _gameViewStyle.normal.textColor = Color.green;
        }
        private void Update()
        {
            _fps = 1.0f / Time.deltaTime;
        }
        private void OnReceiveMetrics(StepMetrics metrics)
        {
            _metrics = metrics;
        }

        private void FixedUpdate()
        {
            if (PhysicsManager.isEngineCreated)
            {
                PhysicsManager.requestMetrics += OnReceiveMetrics;
            }
        }

        private void OnGUI()
        {
            DrawStatistics(_fps, _metrics, _gameViewStyle, true, true);
        }

        internal static void DrawStatistics(float fps, StepMetrics metrics, GUIStyle style, bool showStatistics = true, bool inGameview = false)
        {
            var totalHeight = 300.0f;
            var totalWidth = 350.0f;

            var rect = inGameview ? new Rect(20, 20, totalWidth, totalHeight) : new Rect(0, 0, totalWidth, totalHeight);

            if (inGameview)
            {
                GUI.Box(rect, "");
                rect = new Rect(30, 30, totalWidth, totalHeight);
            }

            if (showStatistics)
            {
                GUI.BeginGroup(rect);

                GUILayout.Label($"Motphys 1.0.0", style);

                GUILayout.Space(10);
                GUILayout.Label($"FPS:{fps.ToString("f2")}", style);

                GUILayout.Space(10);

                GUILayout.Label($"Active Dynamimc Count: {metrics.activeDynamimcCount}", style);
                GUILayout.Label($"Broadphase Pair Count: {metrics.broadphasePairCount}", style);
                GUILayout.Label($"Island Count: {metrics.islandCount}", style);
                GUILayout.Label($"Sleeping Count: {metrics.sleepingCount}", style);
                GUILayout.Label($"Wants Sleeping Count: {metrics.wantsSleepingCount}", style);
                GUILayout.Label($"Awake Count: {metrics.awakeCount}", style);
                GUILayout.Label($"Wants Awake Count: {metrics.wantsAwakeCount}", style);

                GUI.EndGroup();
            }
        }
    }
}
