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

namespace Motphys.Rigidbody.Demos
{
    public class BobbleHead : MonoBehaviour
    {
        [SerializeField]
        private GameObject _runningHead;
        [SerializeField]
        private GameObject _mothphysHead;

        private void Start()
        {
            PhysicsManager.numSubstep = 4;
            PhysicsManager.defaultSolverIterations = 2;
            PhysicsManager.defaultSolverVelocityIterations = 2;
        }

        private void Update()
        {
            if (_runningHead != null)
            {
                var time = Time.timeSinceLevelLoad;
                var x = Mathf.Sin(time * 5) * 10;
                var pos = new Vector3(x, 0.25f, 10);
                _runningHead.transform.position = pos;
            }
        }
    }
}
