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
    public class ConveyorBelt : MonoBehaviour
    {
        public Rigidbody3D moveBelt;
        public BaseCollider trigger;

        [Range(0, 10)]
        public float moveSpeed = 1;
        private Vector3 _initialPos;
        private void Start()
        {
            _initialPos = moveBelt.transform.position;
            trigger.onTriggerEnter += OnMyTriggerEnter;
        }
        void FixedUpdate()
        {
            var moveDir = Vector3.forward * moveSpeed * Time.deltaTime;
            var targetPos = moveBelt.transform.position + moveDir;

            moveBelt.position = _initialPos;
            moveBelt.SetKinematicTarget(targetPos, Quaternion.identity);
            moveBelt.transform.position = _initialPos;
        }

        private void OnMyTriggerEnter(BaseCollider baseCollider)
        {
            baseCollider.transform.position = new Vector3(0, 0.5f, -4);
        }
    }
}

