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
using Motphys.Native;
using Motphys.Rigidbody.Internal;

namespace Motphys.Rigidbody.Api
{
    internal static partial class PhysicsNativeApi
    {
        [DllImport(Constants.DLL)]
        public static extern VisualizeQuery mprGetVisualizeQuery(StepContinuation continuation);

        /// <summary>
        /// Get Number of Potential Collision Pairs
        /// </summary>
        [DllImport(Constants.DLL)]
        public static extern USize mprVisQueryNumPotentialCollisionPairs(VisualizeQuery query);

        /// <summary>
        /// Read Potential Collision Pair Positions
        /// </summary>
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprVisQueryPotentialCollisionPositionPairs(VisualizeQuery query, Motphys.Native.SliceRef<PositionPair> pairs, out USize readCount);

        /// <summary>
        /// Get Number of Active Colliders
        /// </summary>
        [DllImport(Constants.DLL)]
        public static extern USize mprVisQueryNumActiveColliders(VisualizeQuery query);

        /// <summary>
        ///  Read Active Collider Aabbs
        /// </summary>
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprVisQueryActiveAabbs(VisualizeQuery query, Motphys.Native.SliceRef<Aabb3> aabbs, out USize readCount);

        /// <summary>
        /// Get Number of Active Joints
        /// </summary>
        [DllImport(Constants.DLL)]
        public static extern USize mprVisQueryNumActiveJoints(VisualizeQuery query);

        /// <summary>
        /// Read Potential Collision Pair Positions
        /// </summary>
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprVisQueryJointPositionPairs(VisualizeQuery query, Motphys.Native.SliceRef<PositionPair> pairs, out USize readCount);

    }
}
