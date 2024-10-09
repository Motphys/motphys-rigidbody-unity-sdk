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
    public class BambooCopter : MonoBehaviour
    {
        [SerializeField]
        public Rigidbody3D[] bambooCopters;
        [Range(1, 500)]
        public float force = 100;
        [Range(1, 500)]
        public float torque = 100;

        void FixedUpdate()
        {
            if (bambooCopters != null)
            {
                foreach (var bambooCopter in bambooCopters)
                {
                    if (bambooCopter.transform.position.y <= 20)
                    {
                        bambooCopter.AddForce(new Vector3(0, force, 0), Rigidbody.ForceMode.Force);
                        bambooCopter.AddTorque(new Vector3(0, torque, 0), Rigidbody.ForceMode.Force);
                    }
                }
            }
        }
    }
}
