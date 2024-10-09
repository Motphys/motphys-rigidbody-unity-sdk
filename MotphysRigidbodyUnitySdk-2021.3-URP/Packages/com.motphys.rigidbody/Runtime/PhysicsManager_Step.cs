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

using Motphys.Rigidbody.Internal;
using UnityEngine;
using UnityEngine.Jobs;

namespace Motphys.Rigidbody
{

    public static partial class PhysicsManager
    {

        private static readonly ExpandableArray<BodyTransform> s_syncInBodyTransforms = new ExpandableArray<BodyTransform>();
        private static readonly ExpandableArray<UpdateBodyShape> s_bodyShapesToUpdate = new ExpandableArray<UpdateBodyShape>();
        private static readonly System.Action<BaseCollider, ExpandableArray<UpdateBodyShape>> s_onCollectColliderScale = OnCollectColliderScale;

        /// <value>
        /// The simulation mode of the default physics world.
        /// </value>
        ///
        /// <see cref="SimulationMode"/>
        public static SimulationMode simulationMode { get; set; } = SimulationMode.FixedUpdate;

        private static void SyncTransforms()
        {
            ProfilerSamplers.s_pushTransform.Begin();
            s_syncInBodyTransforms.Clear();

            var enumerator = Rigidbody3D.GetPostControlEnumerator();
            while (enumerator.MoveNext())
            {
                var body = enumerator.Current;
                //We believe that changes on transform take the highest priority, overriding any position set by the rigidbody.
                //Otherwise, setting the position on a rigidbody will be effective.
                if (body.IsTransformChanged(out var position, out var rotation))
                {
                    var bodyTransform = new Isometry()
                    {
                        position = position,
                        rotation = rotation
                    };
                    s_syncInBodyTransforms.Push(
                        new BodyTransform(body.rigidbodyId, bodyTransform)
                    );
                }
            }

            if (s_syncInBodyTransforms.length > 0)
            {
                var bodyTransforms = new BodyTransforms(
                    s_syncInBodyTransforms.rawArray,
                    s_syncInBodyTransforms.length
                );
                defaultWorld.BatchUpdateBodyTransforms(bodyTransforms);
            }

            ProfilerSamplers.s_pushTransform.End();
        }

        private static void OnCollectColliderScale(BaseCollider collider, ExpandableArray<UpdateBodyShape> commands)
        {
            if (collider.supportDynamicScale && collider.hasScaleChanged)
            {
                // when the scale of the collider changed, the size and translation of native shape will both be affected.
                if (collider.TryCreateShape(out var builder))
                {
                    commands.Push(new UpdateBodyShape()
                    {
                        colliderId = collider.id,
                        shape = builder,
                        shapeTranslation = collider.expectNativeShapeTranslation,
                    });
                }
            }
        }

        private static void SyncColliderShapeScales()
        {
            ProfilerSamplers.s_update_native_shape_scales.Begin();
            s_bodyShapesToUpdate.Clear();
            BaseCollider.EachActiveCollider(s_onCollectColliderScale, s_bodyShapesToUpdate);
            if (s_bodyShapesToUpdate.length > 0)
            {
                defaultWorld.BatchUpdateBodyShapes(new UpdateBodyShapes(s_bodyShapesToUpdate.rawArray, s_bodyShapesToUpdate.length));
            }

            ProfilerSamplers.s_update_native_shape_scales.End();
        }

        private static void FetchBodyTransforms()
        {
            ProfilerSamplers.s_fetchTransform.Begin();
            var transformsFetcher = Rigidbody3D.transformAccessArrayBuilder;
            ProfilerSamplers.s_fetchTransformFromNative.Begin();
            using var transforms = defaultWorld.FetchSpecifiedBodyTransforms(transformsFetcher.rigidbodyIds);
            ProfilerSamplers.s_fetchTransformFromNative.End();
            Debug.Assert(transforms.Length == transformsFetcher.transformAccessArray.length);
            var job = new TransformSyncJob()
            {
                transformDatas = transforms
            };
            var handle = job.Schedule(transformsFetcher.transformAccessArray);
            handle.Complete();
            ProfilerSamplers.s_fetchTransform.End();
        }

        /// <summary>
        /// Manually step the physics engine with a given time step
        /// </summary>
        /// <param name="deltaTimeSeconds"> the advanced delta time</param>
        public static void Simulate(float deltaTimeSeconds)
        {
            if (deltaTimeSeconds <= 0)
            {
                return;
            }

            ProfilerSamplers.s_fixedUpdate.Begin();
            SyncTransforms();
            SyncColliderShapeScales();
            BaseJoint.CleanDirtyJoints();
            BaseCollider.UpdateAllNative();

            NativeStep(deltaTimeSeconds);
            FetchBodyTransforms();
            Rigidbody3D.RecordLastTransform();

            DispatchEvents();
            NativeObject.CleanUntrackedDestroy();
            BaseCollider.ProcessUnregisters();
            ProfilerSamplers.s_fixedUpdate.End();
        }

        private static void NativeStep(float dt)
        {
            ProfilerSamplers.s_advanceStep.Begin();
            var stepContinue = defaultWorld.Step(dt);
            OnPostStep(stepContinue);
            ProfilerSamplers.s_advanceStep.End();
        }

        private static void OnPostStep(StepContinuation continuation)
        {
            OnQueryVisualizeData(continuation);
            OnReceiveMetrics(continuation);
        }
    }

    /// <summary>
    /// Simulation mode of the default physics world. Used to control when the physics engine will be stepped.
    /// </summary>
    public enum SimulationMode
    {
        /// <summary>
        /// The physics engine will be stepped in FixedUpdate. Choose this mode if you want a more deterministic simulation result.
        /// </summary>
        FixedUpdate,
        /// <summary>
        /// The physics engine will be stepped in Update.
        /// </summary>
        Update,
        /// <summary>
        /// The physics engine will not be stepped automatically. You need to call PhysicsManager.Simulate() manually.
        /// </summary>
        Script,
    }
}
