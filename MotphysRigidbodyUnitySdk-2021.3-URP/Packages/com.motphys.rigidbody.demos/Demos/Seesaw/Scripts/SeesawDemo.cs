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
    public class SeesawDemo : MonoBehaviour
    {
        [SerializeField]
        private int _totalNum = 5;
        [SerializeField]
        private float _perMass = 1;
        [SerializeField]
        private float _perYDistance = 5;

        [SerializeField]
        public GameObject cubePrefab;

        public void Start()
        {
            PhysicsManager.numSubstep = 2;
            CustomGeneration();
        }

        protected void CustomGeneration()
        {
            for (var i = 0; i < _totalNum; i++)
            {
                var rightLeft = i % 2 == 0 ? 1 : -1;
                var pos = new Vector3(5 * rightLeft, _perYDistance * (i + 1), 0);
                var mass = (i + 1) * _perMass;
                var scale = Vector3.one * mass * 0.5f;

                var cube = Instantiate(cubePrefab, pos, Quaternion.identity);
                cube.transform.localScale = scale;
                cube.GetComponent<Rigidbody3D>().mass = mass;
            }
        }
    }
}
