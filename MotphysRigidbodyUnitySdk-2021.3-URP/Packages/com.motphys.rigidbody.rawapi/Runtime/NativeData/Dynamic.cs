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

using System.Runtime.InteropServices;
using UnityEngine;

namespace Motphys.Rigidbody
{
    /// <summary>
    /// Force mode for add force on a rigidbody
    /// </summary>
    public enum ForceMode
    {
        /// <summary>
        /// Add a force to the rigidbody.
        /// </summary>
        Force,
        /// <summary>
        /// Add an acceleration to the rigidbody.
        /// </summary>
        Acceleration,
        /// <summary>
        /// Add an impulse to the rigidbody.
        /// </summary>
        Impulse,
        /// <summary>
        /// Add a velocity change to the rigidbody.
        /// </summary>
        VelocityChange,
    }
}

namespace Motphys.Rigidbody.Internal
{
    /// <summary>
    /// The velocity of rigid body. Include linear and angular.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Velocity
    {
        /// <summary>
        /// The linear velocity. Unit is m/s
        /// </summary>
        public Vector3 linear;

        /// <summary>
        /// The angular velocity. Unit is rad/s
        /// </summary>
        public Vector3 angular;

        public static readonly Velocity Zero = new Velocity()
        {
            linear = Vector3.zero,
            angular = Vector3.zero
        };
    }
    internal unsafe struct Matrix3x3
    {
        public fixed float values[9];
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Mass
    {
        public Vector3 inertiaTensor;
        public Vector3 inertiaTensorInv;
        public float mass;
        public float massInv;
    }

    internal enum SleepStatus
    {
        Awake,
        WantsAwake,
        Sleeping,
        WantsSleeping,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SleepClock
    {
        public SleepStatus status;
        public float sleepingEnergyThreshold;
        public float wakeCounter;
        public float wakeCounterInit;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Freeze
    {
        private byte _data;

        private bool GetBit(int bitIndex)
        {
            var mask = 1 << bitIndex;
            return (_data & mask) == mask;
        }

        private void SetBit(int bitIndex, bool value)
        {
            var mask = 1 << bitIndex;
            if (value)
            {
                _data |= (byte)mask;
            }
            else
            {
                _data &= (byte)~mask;
            }
        }

        public bool freezePositionX
        {
            get { return GetBit(0); }
            set { SetBit(0, value); }
        }
        public bool freezePositionY
        {
            get { return GetBit(1); }
            set { SetBit(1, value); }
        }
        public bool freezePositionZ
        {
            get { return GetBit(2); }
            set { SetBit(2, value); }
        }

        public bool freezeRotationX
        {
            get { return GetBit(3); }
            set { SetBit(3, value); }
        }
        public bool freezeRotationY
        {
            get { return GetBit(4); }
            set { SetBit(4, value); }
        }
        public bool freezeRotationZ
        {
            get { return GetBit(5); }
            set { SetBit(5, value); }
        }

        public static readonly Freeze None = new Freeze() { _data = 0 };
    }
}
