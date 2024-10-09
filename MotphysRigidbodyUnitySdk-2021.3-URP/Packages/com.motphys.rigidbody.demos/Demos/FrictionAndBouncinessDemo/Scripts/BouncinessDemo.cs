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
    public class BouncinessDemo : MonoBehaviour
    {
        [SerializeField]
        private GameObject _spherePrefab;
        [SerializeField]
        private GameObject _blackBoard;
        [SerializeField]
        private GameObject _whiteBoard;

        [SerializeField]
        [Range(1, 10)]
        private int _totalNum = 8;
        [SerializeField]
        [Range(0.1f, 10)]
        private float _sphereRadius = 1.2f;

        private void Start()
        {
            CustomGeneration();
        }
        protected void CustomGeneration()
        {
            for (var i = 0; i < _totalNum; i++)
            {
                var prefab = i % 2 == 0 ? _whiteBoard : _blackBoard;

                GameObject sphere = Instantiate(_spherePrefab, new Vector3(-5 + (i * 2), 6.6f, 0), Quaternion.identity);

                GameObject board = Instantiate(prefab, new Vector3(-5 + (i * 2), 1.31f, 0), Quaternion.identity);
                sphere.transform.localScale = Vector3.one * _sphereRadius;

                sphere.GetComponent<BaseCollider>().material.bounciness = i / (_totalNum - 1.0f);
                board.GetComponent<BaseCollider>().material.bounciness = i / (_totalNum - 1.0f);
            }
        }
    }
}
