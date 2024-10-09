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
    public class Tornado : MonoBehaviour
    {
        public BaseCollider tornadoTrigger;
        public GameObject spherePrefab;

        [SerializeField]
        [Range(1, 1000)]
        public int genCount;

        [Range(0, 100)]
        public float generateRange = 20;
        [Range(0, 100)]
        public float triggerMoveSpeed = 0.5f;
        [Range(0, 100)]
        public float tornadoRotationSpeed = 20;
        [Range(0, 100)]
        public float tornadoHorizontalForce = 30;
        [Range(1, 500)]
        public float tornadoVerticalFroce = 1000;

        private List<GameObject> _sceneObjects;
        private Vector3 _tornadoForce;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * generateRange * 2);
            var forceDir = _tornadoForce.normalized;
            Gizmos.DrawLine(tornadoTrigger.transform.position, tornadoTrigger.transform.position + forceDir);
        }

        void Start()
        {
            tornadoTrigger.onTriggerStay += OnTornadoTriggerStay;
            Generation();
        }

        private void Update()
        {
            var time = Time.timeSinceLevelLoad;
            var position = new Vector3(Mathf.Sin(triggerMoveSpeed * time) * 10, 0, Mathf.Cos(triggerMoveSpeed * time) * 10);

            tornadoTrigger.transform.position = position;
            _tornadoForce = new Vector3(Mathf.Sin(tornadoRotationSpeed * time) * tornadoHorizontalForce, tornadoVerticalFroce, Mathf.Cos(tornadoRotationSpeed * time) * tornadoHorizontalForce);
        }
        private void Generation()
        {
            _sceneObjects = new List<GameObject>();
            for (var i = 0; i < genCount; i++)
            {
                var position = new Vector3(Random.Range(-generateRange, generateRange), 0, Random.Range(-generateRange, generateRange));
                var go = Instantiate(spherePrefab, position, Quaternion.identity);
                _sceneObjects.Add(go);
            }
        }
        private void OnTornadoTriggerStay(BaseCollider baseCollider)
        {
            if (baseCollider.TryGetComponent<Rigidbody3D>(out var rigidbody))
            {
                rigidbody.AddForceAtPosition(_tornadoForce, Vector3.zero, Motphys.Rigidbody.ForceMode.Force);
            }
        }
    }
}
