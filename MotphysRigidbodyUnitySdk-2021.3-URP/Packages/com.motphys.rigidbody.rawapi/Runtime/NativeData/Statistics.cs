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
    /// Some specfied metrics in the physics world step.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct StepMetrics
    {
        /// <value>
        /// The number of dynamic rigidbodies in the physics world.
        /// </value>
        public uint activeDynamimcCount;

        /// <value>
        /// The number of collision pairs count in broad phase.
        /// </value>
        public uint broadphasePairCount;

        /// <value>
        /// The number of island count in narrow phase;
        /// </value>
        public uint islandCount;

        /// <value>
        /// The number of sleeping rigidbodies in the physics world.
        /// </value>
        public uint sleepingCount;

        /// <value>
        /// The number of rigidbodies which status are wants sleeping in the physics world.
        /// </value>
        public uint wantsSleepingCount;

        /// <value>
        /// The number of rigidbodies which status are awake in the physics world.
        /// </value>
        public uint awakeCount;

        /// <value>
        /// The number of rigidbodies which status are wants awake in the physics world.
        /// </value>
        public uint wantsAwakeCount;

        /// <value>
        ///  The number of collider pairs that enter to collision state in this step.
        /// </value>
        public uint collisionEnterCount;
        /// <value>
        /// The number of collider pairs that exit from collision state in this step.
        /// </value>
        public uint collisionExitCount;

        /// <value>
        /// The number of collision manifold count in the world.
        /// </value>
        public uint manifoldCount;

        /// <value>
        /// The number of collision contacts in the world.
        /// </value>
        public uint contactCount;
    }

    /// <summary>
    /// When two bodys are colliding, one body's contact points will be stored in this struct. (The other body's contact points are ignored. They are very close when two bodies are colliding and not really necessary.)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ContactDebugInfo
    {
        /// <value>
        /// The position of the contact point of one body in world space.
        /// </value>
        public Vector3 position;

        /// <value>
        /// The normal of the contact point of one body in world space.
        /// </value>
        public Vector3 normal;
    }
}
