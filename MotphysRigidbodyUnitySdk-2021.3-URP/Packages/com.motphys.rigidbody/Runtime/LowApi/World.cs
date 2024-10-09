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

using System.Runtime.CompilerServices;
using Motphys.Native;
using Motphys.Rigidbody.Api;
using Unity.Collections;
using UnityEngine;

namespace Motphys.Rigidbody.Internal
{
    internal struct PhysicsWorld
    {
        public PhysicsEngine engine
        {
            get;
        }
        public WorldId id
        {
            get;
        }

        public PhysicsWorld(PhysicsEngine engine, WorldId id)
        {
            this.engine = engine;
            this.id = id;
        }

        internal System.IntPtr enginePtr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return engine.ptr; }
        }

        /// <value>
        /// A world is valid if it has a valid world id and the referenced engine is not disposed.
        ///
        /// <para>NOTE: A world removed from the engine maybe still valid</para>
        /// </value>
        internal bool isValid
        {
            get
            {
                return id.isValid && engine != null && !engine.isDisposed;
            }
        }

        /// <value>
        /// Get the reference to the native world.
        /// </value>
        internal WorldRef @ref => new WorldRef(enginePtr, id);

        /// <value>
        /// Use this to get query some statistics data about the world.
        /// Note: Only available in package with feature: 'statistics' on.
        /// </value>
        public Statistics statistics => new Statistics(@ref);

        public SimulatorDynamicOptions simulatorOptions
        {
            get
            {
                @ref.mprGetWorldSimulatorOptions(out var options)
                    .ThrowExceptionIfNotOk();
                return options;
            }
            set { ApplySimulatorOptions(value); }
        }

        public void ApplySimulatorOptions(SimulatorDynamicOptions options)
        {
            @ref.mprUpdateWorldSimulatorOptions(options).ThrowExceptionIfNotOk();
        }

        public uint substeps
        {
            set
            {
                var options = simulatorOptions;
                options.numSubSteps = value;
                ApplySimulatorOptions(options);
            }
            get { return simulatorOptions.numSubSteps; }
        }

        internal RigidbodyHandle AddRigidbody(ActorOptions options)
        {
            @ref.mprAddBody(options, out var bodyId).ThrowExceptionIfNotOk();
            return new RigidbodyHandle(this, bodyId);
        }

        internal JointId AddJoint(JointBuilder joint)
        {
            @ref.mprAddJoint(joint, out var jointId).ThrowExceptionIfNotOk();
            return jointId;
        }

        internal bool RemoveJoint(JointId jointId)
        {
            var code = @ref.mprRemoveJoint(jointId);
            if (code == ResultCode.JointNotFound)
            {
                return false;
            }

            code.ThrowExceptionIfNotOk();
            return true;
        }

        internal void SetJointActive(JointId jointId, bool active)
        {
            @ref.mprSetJointActive(jointId, active).ThrowExceptionIfNotOk();
        }

        internal void UpdateJointProperties(JointId jointId, JointBuilder properties)
        {
            @ref.mprUpdateJoint(jointId, properties).ThrowExceptionIfNotOk();
        }

        internal Vector3 GetJointForce(JointId jointId)
        {
            @ref.mprGetJointForce(jointId, out var force).ThrowExceptionIfNotOk();
            return force;
        }

        internal Vector3 GetJointTorque(JointId jointId)
        {
            @ref.mprGetJointTorque(jointId, out var torque).ThrowExceptionIfNotOk();
            return torque;
        }

        internal Vector3 GetJointCurrentMotorForces(JointId jointId)
        {
            @ref.mprGetJointMotorForces(jointId, out var forces).ThrowExceptionIfNotOk();
            return forces;
        }

        internal Vector3 GetJointCurrentMotorTorques(JointId jointId)
        {
            @ref.mprGetJointMotorTorques(jointId, out var torques).ThrowExceptionIfNotOk();
            return torques;
        }

        public StepContinuation Step(float dt)
        {
            @ref.mprStepWorld(dt, out var continuation).ThrowExceptionIfNotOk();
            return continuation;
        }

        public void BatchUpdateBodyTransforms(BodyTransforms transforms)
        {
            @ref.mprBatchUpdateBodyTransforms(transforms).ThrowExceptionIfNotOk();
        }

        public BodyTransforms FetchBodyTransforms()
        {
            @ref.mprFetchBodyTransforms(out var bodyTransforms).ThrowExceptionIfNotOk();
            return bodyTransforms;
        }

        public NativeArray<Isometry> FetchSpecifiedBodyTransforms(NativeSlice<RigidbodyId> requiredBodyIds)
        {
            var bodyIds = new SliceRef<RigidbodyId>(requiredBodyIds);
            var outTransforms = new NativeArray<Isometry>((int)bodyIds.len, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            @ref.mprFetchSpecifiedBodyTransforms(bodyIds, new SliceRef<Isometry>(outTransforms)).ThrowExceptionIfNotOk();
            return outTransforms;
        }

        public WorldEvents QueryWorldEvents()
        {
            @ref.mprQueryWorldEvents(out var events).ThrowExceptionIfNotOk();
            return events;
        }

        public ContactManifold QueryCollisionEvent(ColliderPair pair)
        {
            @ref.mprQueryContactManifold(pair, out var manifold).ThrowExceptionIfNotOk();
            return manifold;
        }

        public uint QueryAllManifoldsCount()
        {
            @ref.mprQueryNumOfAllContactManifolds(out var count).ThrowExceptionIfNotOk();
            return (uint)count.ToUInt32();
        }

        public uint QueryAllManifolds(NativeArray<ContactManifold> allContactManifolds)
        {
            @ref.mprQueryAllManifolds(new SliceRef<ContactManifold>(allContactManifolds), out var count).ThrowExceptionIfNotOk();
            return (uint)count.ToUInt32();
        }

        public void BatchUpdateBodyShapes(UpdateBodyShapes updates)
        {
            @ref.mprBatchUpdateBodyShape(updates).ThrowExceptionIfNotOk();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RaycastHit? RaycastClosest(Vector3 origin, Vector3 direction, float maxDistance, SceneQueryContext context)
        {
            direction.Normalize();
            @ref.mprRaycastClosest(new Ray { origin = origin, direction = direction }, maxDistance, context, out var result).ThrowExceptionIfNotOk();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RaycastHit? RaycastAny(Vector3 origin, Vector3 direction, float maxDistance, SceneQueryContext context)
        {
            direction.Normalize();
            @ref.mprRaycastAny(new Ray { origin = origin, direction = direction }, maxDistance, context, out var result).ThrowExceptionIfNotOk();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint RaycastAllNonAlloc(Vector3 origin, Vector3 direction, float maxDistance, uint maxHits, RaycastHit[] hitResults, SceneQueryContext context)
        {
            direction.Normalize();
            var slice = new SliceRef<RaycastHit>(hitResults, (int)maxHits);
            @ref.mprRaycastAll(
                new Ray { origin = origin, direction = direction },
                maxDistance,
                slice,
                context,
                out var numHits
            ).ThrowExceptionIfNotOk();

            return numHits;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint OverlayTestNonAlloc(Shape shape, Isometry transform, uint maxOverlaps, ColliderId[] rigidbodyIdBuffer, SceneQueryContext context)
        {
            @ref.mprOverlapTest(shape, transform, new SliceRef<ColliderId>(rigidbodyIdBuffer, (int)maxOverlaps), context, out var numOverlaps).ThrowExceptionIfNotOk();
            return numOverlaps;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ShapeId CreateConvexHull(SliceRef<Vector3> points)
        {
            @ref.mprCreateConvexHull(points, out var shapeId).ThrowExceptionIfNotOk();
            return shapeId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ShapeId DestroyConvexHull(ShapeId shapeId)
        {
            @ref.mprDestroyConvexHull(shapeId).ThrowExceptionIfNotOk();
            return shapeId;
        }
    }
}
