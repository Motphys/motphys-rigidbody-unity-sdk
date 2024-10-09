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

namespace Motphys.Rigidbody.Internal
{
    // Execute after RigidBody3D
    [DefaultExecutionOrder(5)]
    internal class BodyActivationTrack : MonoBehaviour
    {
        private Rigidbody3D _rigidbody;

        internal Rigidbody3D rigidbody3d
        {
            get => _rigidbody;
            set => _rigidbody = value;
        }

        private void Awake()
        {
            hideFlags = HideFlags.DontSave | HideFlags.HideInInspector;
        }

        private void OnEnable()
        {
            // the GameObject is actived
            if (rigidbody3d)
            {
                rigidbody3d.OnGameObjectActivate();
            }
        }

        private void OnDisable()
        {
            // the GameObject is deactivated
            if (rigidbody3d && rigidbody3d.enabled)
            {
                rigidbody3d.OnGameObjectDeactivate();
            }
        }
    }
}
