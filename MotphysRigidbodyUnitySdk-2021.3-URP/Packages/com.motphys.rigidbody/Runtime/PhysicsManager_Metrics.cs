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
        /// <summary>
        /// Get phyics metrics data.
        /// </summary>
        /// <param name="continuation"></param>
        /// <returns></returns>
        public static StepMetrics GetMetrics(this ref StepContinuation continuation)
        {
            return PhysicsNativeApi.mprGetAllMetrics(continuation);
        }
    }

    public partial class PhysicsManager
    {

        /// <summary>
        /// Register a callback to receive metrics data once after step.
        /// </summary>
        public static event System.Action<StepMetrics> requestMetrics;

        private static void OnReceiveMetrics(StepContinuation continuation)
        {
            try
            {
                if (requestMetrics != null)
                {
                    var metrics = continuation.GetMetrics();
                    requestMetrics(metrics);
                }
            }
            finally
            {
                requestMetrics = null;
            }
        }

        public class Statistics
        {

            /// <value>
            /// All contact debug infos produced during last step frame.
            /// </value>
            public static NativeArray<ContactDebugInfo> contacts => defaultWorld.statistics.contactDebugInfos;
        }
    }
}
