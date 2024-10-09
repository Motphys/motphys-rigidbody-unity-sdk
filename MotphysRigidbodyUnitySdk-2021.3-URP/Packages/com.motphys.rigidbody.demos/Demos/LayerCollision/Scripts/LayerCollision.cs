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
using UnityEngine;

namespace Motphys.Rigidbody.Demos
{
    public class LayerCollision : MonoBehaviour
    {
        [SerializeField]
        private GameObject _spherePrefab;
        [SerializeField]
        private GameObject _cubePrefab;
        [SerializeField]
        private GameObject _capsulePrefab;

        [SerializeField]
        private int _totalNum = 180;
        [SerializeField]
        private float _initRange = 8f;
        [SerializeField]
        private float _initHeight = 0.5f;

        private List<GameObject> _resetObjectList = new List<GameObject>();

        private void Start()
        {
            var capsuleLayer = _capsulePrefab.layer;
            var sphereLayer = _spherePrefab.layer;
            var cubeLayer = _cubePrefab.layer;

            var capsuleLayerMask = PhysicsProjectSettings.Instance.GetCollisionMask(capsuleLayer);
            var sphereLayerMask = PhysicsProjectSettings.Instance.GetCollisionMask(sphereLayer);
            var boxLayerMask = PhysicsProjectSettings.Instance.GetCollisionMask(cubeLayer);

            //no collisions occur between these three layers.
            if ((capsuleLayerMask & (1 << _spherePrefab.layer)) != 0 || (capsuleLayerMask & (1 << _cubePrefab.layer)) != 0 ||
                (sphereLayerMask & (1 << _capsulePrefab.layer)) != 0 || (sphereLayerMask & (1 << _cubePrefab.layer)) != 0 ||
                (boxLayerMask & (1 << _capsulePrefab.layer)) != 0 || (boxLayerMask & (1 << _spherePrefab.layer)) != 0)
            {
                //Ignore layers.
                PhysicsProjectSettings.Instance.IgnoreLayerCollision(capsuleLayer, sphereLayer);
                PhysicsProjectSettings.Instance.IgnoreLayerCollision(capsuleLayer, cubeLayer);
                PhysicsProjectSettings.Instance.IgnoreLayerCollision(sphereLayer, cubeLayer);
                Debug.LogWarning("Wrong collision layer of LayerCapsule, LayerBox and LayerSphere. Set layers by script now. Open Edit/ProjectSettings/Motphys to set collision matrix usually");
            }

            GenerateShapes();
        }

        private void Update()
        {
            ResetObjects();
        }

        private void GenerateShapes()
        {
            var defaultSize = 3.0f;
            var initialY = 30.0f;
            var layers = 10.0f;
            var shapeCount = _totalNum / layers;
            var count = Mathf.CeilToInt(Mathf.Sqrt(shapeCount));
            for (var layer = 0; layer < layers; layer++)
            {
                var initialZ = -_initRange;
                var maxY = -9999.0f;
                for (var i = 0; i < count; i++)
                {
                    var maxZ = -9999.0f;
                    var initialX = -_initRange;
                    for (var j = 0; j < count; j++)
                    {
                        var x = initialX + Random.Range(-2.3f, 2.3f);
                        var y = initialY + Random.Range(-2.3f, 2.3f) + _initHeight + 3.5f;
                        var z = initialZ + Random.Range(-2.3f, 2.3f);
                        var position = new Vector3(x, y, z);

                        GameObject gameObject = null;
                        switch (Random.Range(0, 3))
                        {
                            case 0:
                                gameObject = Instantiate(_spherePrefab, position, Random.rotation);
                                break;
                            case 1:
                                gameObject = Instantiate(_cubePrefab, position, Random.rotation);
                                break;
                            case 2:
                                gameObject = Instantiate(_capsulePrefab, position, Random.rotation);
                                break;
                        }

                        if (gameObject != null)
                        {
                            _resetObjectList.Add(gameObject);
                        }

                        initialX = x + defaultSize * 2.2f;
                        var intervalZ = z + defaultSize * 2.2f;
                        var intervalY = y + defaultSize * 1.5f;
                        maxZ = Mathf.Max(maxZ, intervalZ);
                        maxY = Mathf.Max(maxY, intervalY);
                    }

                    initialZ = maxZ;
                }

                initialY = maxY;
            }
        }

        private void ResetObjects()
        {
            for (var i = 0; i < _resetObjectList.Count; ++i)
            {
                Vector3 positon = _resetObjectList[i].transform.position;
                var rigidbody = _resetObjectList[i].GetComponent<Rigidbody3D>();
                Vector3 velocity = rigidbody.linearVelocity;

                if (positon.x > 9.4 || positon.x < -9.4)
                {
                    rigidbody.linearVelocity = new Vector3(-velocity.x, velocity.y, velocity.z);
                }

                if (positon.z > 9.4 || positon.z < -9.4)
                {
                    rigidbody.linearVelocity = new Vector3(velocity.x, velocity.y, -velocity.z);
                }

                if (positon.y < -4.6f)
                {
                    rigidbody.transform.position = new Vector3(Random.Range(-1.0f, 1.0f), 40, Random.Range(-1.0f, 1.0f));
                }
            }
        }
    }
}

