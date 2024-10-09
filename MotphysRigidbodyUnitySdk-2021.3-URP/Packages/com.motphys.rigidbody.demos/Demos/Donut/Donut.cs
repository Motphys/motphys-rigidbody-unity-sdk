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
    public class Donut : MonoBehaviour
    {
        [SerializeField]
        private int _numOfDonuts = 10;
        [SerializeField]
        private int _numOfCubes = 30;
        [SerializeField]
        private float _initRingRange = 3f;

        [SerializeField]
        private GameObject _cubePrefab;
        [SerializeField]
        private GameObject _ellipsoidRingPrefab;

        [SerializeField]
        private Material[] _materials;

        private void Start()
        {
            PhysicsManager.numSubstep = 4;
            PhysicsManager.defaultSolverIterations = 4;
            PhysicsManager.defaultSolverVelocityIterations = 4;

            CustomGeneration();
        }

        private void GenerateBox()
        {
            var maxSize = 2.5f;
            var initialZ = -5.0f;
            var count = Mathf.CeilToInt(Mathf.Sqrt(_numOfCubes));
            for (var i = 0; i < count; i++)
            {
                var maxZ = float.MinValue;
                var initialX = -5.0f;
                for (var j = 0; j < count; j++)
                {
                    var scale = Random.Range(0.5f, maxSize);
                    var x = initialX + Random.Range(-1.5f, 1.5f);
                    var z = initialZ + Random.Range(-1.5f, 1.5f);

                    GameObject boxObject = Instantiate(_cubePrefab, new Vector3(x, scale * 0.5f + 0.1f, z), Quaternion.identity);

                    boxObject.transform.localScale = new Vector3(scale, scale, scale);
                    boxObject.GetComponent<Rigidbody3D>().mass = scale * scale * scale;

                    initialX = x + scale * 0.5f + maxSize;
                    var intervalZ = z + scale * 0.5f + maxSize;
                    maxZ = Mathf.Max(maxZ, intervalZ);
                }

                initialZ = maxZ;
            }
        }

        protected void CustomGeneration()
        {
            for (var i = 0; i < _numOfDonuts; i++)
            {
                var center = new Vector3(
                            Random.Range(-_initRingRange, _initRingRange),
                            10 + i * 2f,
                            Random.Range(-_initRingRange, _initRingRange));

                GenerateRing(center);
            }

            GenerateBox();
        }

        private void GenerateRing(Vector3 center)
        {
            var go = Instantiate(_ellipsoidRingPrefab, center, Quaternion.identity);
            SetMaterial(go);
        }

        private void SetMaterial(GameObject instance)
        {
            instance.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().sharedMaterial = _materials[Random.Range(0, _materials.Length)];
        }
    }
}
