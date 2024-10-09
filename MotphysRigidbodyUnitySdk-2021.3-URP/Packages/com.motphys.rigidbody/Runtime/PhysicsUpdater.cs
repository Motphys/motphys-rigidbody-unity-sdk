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

using UnityEngine;

namespace Motphys.Rigidbody
{
    [DefaultExecutionOrder(-1)]
    internal class PhysicsUpdater : MonoBehaviour
    {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        internal static void Launch()
        {
            var updater = new GameObject("PhysicsUpdater").AddComponent<PhysicsUpdater>();
            GameObject.DontDestroyOnLoad(updater.gameObject);
        }

        private static PhysicsUpdater s_instance;

        internal static PhysicsUpdater Instance => s_instance;

        private void Awake()
        {
            if (s_instance != this && s_instance != null)
            {
                Destroy(gameObject);
                Debug.LogError("Only one PhysicsUpdater allowed in scene");
                return;
            }

            s_instance = this;
            PhysicsManager.LaunchEngine();
        }

        private void FixedUpdate()
        {
            if (PhysicsManager.simulationMode == SimulationMode.FixedUpdate)
            {
                PhysicsManager.Simulate(Time.fixedDeltaTime);
            }
        }

        private void Update()
        {
            if (PhysicsManager.simulationMode == SimulationMode.Update)
            {
                PhysicsManager.Simulate(Time.deltaTime);
            }
        }

        private void OnDestroy()
        {
            if (s_instance == this)
            {
                PhysicsManager.ShutDownEngine();
                s_instance = null;
            }
        }
    }
}
