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

using UnityEngine.Profiling;

namespace Motphys.Rigidbody.Internal
{

    /// <summary>
    /// Define some profiler samplers for Motphys.
    /// </summary>
    ///
    /// Do not need test coverage for samplers.
    [UnityEngine.TestTools.ExcludeFromCoverage]
    internal static class ProfilerSamplers
    {
        internal static CustomSampler s_fixedUpdate = CustomSampler.Create("Motphys.Rigidbody.FixedUpdate");
        internal static CustomSampler s_advanceStep = CustomSampler.Create("Motphys.Rigidbody.AdvanceStep");
        internal static CustomSampler s_pushTransform = CustomSampler.Create("Motphys.Rigidbody.PushTransforms");
        internal static CustomSampler s_fetchTransform = CustomSampler.Create("Motphys.Rigidbody.FetchTransforms");
        internal static CustomSampler s_fetchTransformFromNative = CustomSampler.Create("Motphys.Rigidbody.FetchTransformsFromNative");
        internal static CustomSampler s_update_native_colliders = CustomSampler.Create("Motphys.Rigidbody.UpdateNativeColliders");
        internal static CustomSampler s_update_native_shape_scales = CustomSampler.Create("Motphys.Rigidbody.UpdateNativeShapeScales");
        internal static CustomSampler s_dispatchEvents = CustomSampler.Create("Motphys.Rigidbody.DispatchEvents");

        internal static class Recorders
        {
            internal static Recorder fixedUpdate
            {
                get => ProfilerSamplers.s_fixedUpdate.GetRecorder();
            }
        }
    }
}
