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
    public class FrictionDemo : MonoBehaviour
    {
        [SerializeField]
        private GameObject _cubePrefab;
        [SerializeField]
        private GameObject _boardPrefab;

        [SerializeField]
        private Material[] _materials;
        [SerializeField]
        private int _totalNum = 8;

        [SerializeField]
        [Range(0.1f, 10)]
        private float _boardWidth = 0.65f;
        [SerializeField]
        [Range(0.1f, 10)]
        private float _boardHeight = 0.35f;

        private void Start()
        {
            CustomGeneration();
        }

        protected void CustomGeneration()
        {
            for (var i = 0; i < _totalNum; i++)
            {
                GameObject cube = Instantiate(_cubePrefab, new Vector3(-7 + (i * 2), 2.7f + 7.09f, 1.5f), Quaternion.Euler(-45, 0, 0));
                cube.GetComponent<Rigidbody3D>().freezeRotation = new FreezeOptions() { y = true };

                var index = (i % _materials.Length);
                var material = _materials[index];

                GameObject board = Instantiate(_boardPrefab, new Vector3(-7 + (i * 2), 7.09f, 0.4f), Quaternion.Euler(-45, 0, 0));
                board.GetComponent<MeshRenderer>().sharedMaterial = material;

                cube.transform.localScale = new Vector3(1.1f, 0.6f, 1.1f);
                board.transform.localScale = new Vector3(_boardWidth, _boardHeight, _boardWidth);

                cube.GetComponent<BaseCollider>().material.SetFrictions(i / (_totalNum - 1.0f), i / (_totalNum - 1.0f));
                board.GetComponent<BaseCollider>().material.SetFrictions(i / (_totalNum - 1.0f), i / (_totalNum - 1.0f));
            }
        }
    }
}
