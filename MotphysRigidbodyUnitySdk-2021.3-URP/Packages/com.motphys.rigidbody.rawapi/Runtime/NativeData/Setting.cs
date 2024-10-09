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

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Motphys.Rigidbody.Internal
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    internal struct CollisionSetting
    {
        [SerializeField]
        [Tooltip("Control how far of the distance between two colliders will be considered a collision")]
        private float _speculativeMargin;

        [SerializeField]
        [Tooltip("Control the actual separation bound of the collider")]
        private float _separationOffset;

        internal float contactOffset
        {
            get { return _speculativeMargin; }
            set
            {
                if (value <= _separationOffset)
                {
                    throw new System.ArgumentException("contactOffset must be greater than separationOffset");
                }

                _speculativeMargin = value;
            }
        }

        internal float separationOffset
        {
            get { return _separationOffset; }
            set
            {
                if (value >= _speculativeMargin)
                {
                    throw new System.ArgumentException("separationOffset must be less than contactOffset");
                }

                _separationOffset = value;
            }
        }

        internal static CollisionSetting Default => new CollisionSetting()
        {
            _speculativeMargin = 0.005f,
            _separationOffset = 0.0f,
        };
    }
}
