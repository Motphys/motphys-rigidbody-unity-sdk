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

using Motphys.Rigidbody.Api;
using Motphys.Rigidbody.Internal;
using Unity.Collections;

namespace Motphys.Rigidbody
{
    internal static partial class StepContinuationExtensions
    {
        public static VisualizeQuery GetVisualizeQuery(this ref StepContinuation continuation)
        {
            return PhysicsNativeApi.mprGetVisualizeQuery(continuation);
        }
    }

    internal static class VisualizeQueryExtensions
    {

        public static int GetNumPotentialCollisionPairs(this ref VisualizeQuery query)
        {
            return (int)PhysicsNativeApi.mprVisQueryNumPotentialCollisionPairs(query).ToUInt32();
        }

        public static int GetNumActiveColliders(this ref VisualizeQuery query)
        {
            return (int)PhysicsNativeApi.mprVisQueryNumActiveColliders(query).ToUInt32();
        }

        public static int GetNumActiveJoints(this ref VisualizeQuery query)
        {
            return (int)PhysicsNativeApi.mprVisQueryNumActiveJoints(query).ToUInt32();
        }

        public static int ReadPotentialCollisionPositionPairs(this ref VisualizeQuery query, NativeArray<PositionPair> pairs)
        {
            PhysicsNativeApi.mprVisQueryPotentialCollisionPositionPairs(query, new Motphys.Native.SliceRef<PositionPair>(pairs), out var readCount).ThrowExceptionIfNotOk();
            return (int)readCount.ToUInt32();
        }

        public static int ReadActiveAabbs(this ref VisualizeQuery query, NativeArray<Aabb3> aabbs)
        {
            PhysicsNativeApi.mprVisQueryActiveAabbs(query, new Motphys.Native.SliceRef<Aabb3>(aabbs), out var readCount).ThrowExceptionIfNotOk();
            return (int)readCount.ToUInt32();
        }

        public static int ReadActiveJointPositionPairs(this ref VisualizeQuery query, NativeArray<PositionPair> pairs)
        {
            PhysicsNativeApi.mprVisQueryJointPositionPairs(query, new Motphys.Native.SliceRef<PositionPair>(pairs), out var readCount).ThrowExceptionIfNotOk();
            return (int)readCount.ToUInt32();
        }
    }

    /// <summary>
    /// Contains all data for visualization.
    ///
    /// Note:
    ///
    /// Only the data that is enabled by the visualizeDataMask will be collected.
    /// </summary>
    public struct VisualizeData
    {
        /// <value>
        ///  The position of potential collision pairs.
        /// </value>
        public NativeArray<PositionPair> potentialCollisionPositionPairs;

        /// <value>
        /// All active aabbs. Each collider has an aabb.
        /// </value>
        public NativeArray<Aabb3> activeAabbs;

        /// <value>
        /// The position of active joint pairs.
        /// </value>
        public NativeArray<PositionPair> activeJointPositionPairs;
    }

    /// <summary>
    /// Enumerates the data types that can be visualized.
    /// </summary>
    [System.Flags]
    public enum VisualizeDataType
    {
        /// <summary>
        /// No visualize data
        /// </summary>
        None = 0,
        /// <summary>
        /// Query collision pairs
        /// </summary>
        PotentialCollisionPositionPairs = 1,
        /// <summary>
        /// Query active aabbs
        /// </summary>
        ActiveAabbs = 2,
        /// <summary>
        /// Query joint pairs
        /// </summary>
        ActiveJointPositionPairs = 4,
        /// <summary>
        /// Query all data
        /// </summary>
        All = 0xffff,
    }

    public static partial class PhysicsManager
    {
        /// <value>
        /// The visualize data mask, used for drawing gizmos.
        /// </value>
        public static VisualizeDataType visualizeDataMask
        {
            get; set;
        }

        private static event System.Action<VisualizeData> _onReceiveVisualizeData;

        /// <summary>
        /// Register a callback to receive visualize data. The callback will be called once after a step complete. The requested data type should be configured in the dataFilter.
        /// </summary>
        /// <see cref = "VisualizeDataType" />
        public static void RequestVisualizeDataOnce(VisualizeDataType dataFilter, System.Action<VisualizeData> callback)
        {
            visualizeDataMask |= dataFilter;
            _onReceiveVisualizeData += callback;
        }

        private static void OnQueryVisualizeData(StepContinuation continuation)
        {
            var visualizeData = new VisualizeData();

            if (visualizeDataMask != VisualizeDataType.None)
            {
                var query = continuation.GetVisualizeQuery();

                if (visualizeDataMask.HasFlag(VisualizeDataType.PotentialCollisionPositionPairs))
                {
                    var numPotentialCollisionPairs = query.GetNumPotentialCollisionPairs();
                    visualizeData.potentialCollisionPositionPairs = new NativeArray<PositionPair>(numPotentialCollisionPairs, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

                    if (numPotentialCollisionPairs > 0)
                    {
                        var readCount = query.ReadPotentialCollisionPositionPairs(visualizeData.potentialCollisionPositionPairs);
                        UnityEngine.Debug.Assert(readCount == numPotentialCollisionPairs);
                    }
                }

                if (visualizeDataMask.HasFlag(VisualizeDataType.ActiveAabbs))
                {
                    var numActiveAabbs = query.GetNumActiveColliders();
                    visualizeData.activeAabbs = new NativeArray<Aabb3>(numActiveAabbs, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

                    if (numActiveAabbs > 0)
                    {
                        var readCount = query.ReadActiveAabbs(visualizeData.activeAabbs);
                        UnityEngine.Debug.Assert(readCount == numActiveAabbs);
                    }
                }

                if (visualizeDataMask.HasFlag(VisualizeDataType.ActiveJointPositionPairs))
                {
                    var numActiveJointPositionPairs = query.GetNumActiveJoints();
                    visualizeData.activeJointPositionPairs = new NativeArray<PositionPair>(numActiveJointPositionPairs, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

                    if (numActiveJointPositionPairs > 0)
                    {
                        var readCount = query.ReadActiveJointPositionPairs(visualizeData.activeJointPositionPairs);
                        UnityEngine.Debug.Assert(readCount == numActiveJointPositionPairs);
                    }
                }
            }

            try
            {
                var onReceiveVisualizeData = _onReceiveVisualizeData;
                _onReceiveVisualizeData = null;
                visualizeDataMask = 0;

                onReceiveVisualizeData?.Invoke(visualizeData);
            }
            finally
            {

            }
        }
    }
}
