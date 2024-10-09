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
using Motphys.Rigidbody.Internal;
using UnityEngine;
using Constants = Motphys.Rigidbody.Native.Constants;
using RaycastHit = Motphys.Rigidbody.Internal.RaycastHit;

namespace Motphys.Rigidbody.Api
{
    internal static partial class PhysicsNativeApi
    {

        #region EngineApi
        [DllImport(Constants.DLL)]
        public static extern IntPtr mprCreatePhysicsEngine();
        [DllImport(Constants.DLL)]
        public static extern IntPtr mprCreatePhysicsEngineWithOptions(PhysicsEngineBuilder options);
        [DllImport(Constants.DLL)]
        public static extern void mprDestroyPhysicsEngine(IntPtr engine);
        [DllImport(Constants.DLL)]
        public static extern WorldId mprCreatePhysicsWorld(IntPtr engine, SimulatorDynamicOptions options);
        [DllImport(Constants.DLL)]
        public static extern void mprDestroyPhysicsWorld(IntPtr engine, WorldId worldId);
        [DllImport(Constants.DLL)]
        public static extern WorldId mprGetDefaultWorldId(IntPtr engine);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetWorldSimulatorOptions(this WorldRef world, out SimulatorDynamicOptions options);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprUpdateWorldSimulatorOptions(this WorldRef world, SimulatorDynamicOptions options);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprStepWorld(this WorldRef world, float dt, out StepContinuation continuation);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprQueryWorldEvents(this WorldRef world, out WorldEvents events);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprQueryContactManifold(this WorldRef world, ColliderPair pair, out ContactManifold manifold);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprQueryNumOfAllContactManifolds(this WorldRef world, out Motphys.Native.USize count);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprQueryAllManifolds(this WorldRef world, Motphys.Native.SliceRef<ContactManifold> out_manifolds, out Motphys.Native.USize count);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprBatchUpdateBodyTransforms(this WorldRef world, BodyTransforms batchTransforms);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprFetchBodyTransforms(this WorldRef world, out BodyTransforms batchTransforms);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprFetchSpecifiedBodyTransforms(this WorldRef world, Motphys.Native.SliceRef<RigidbodyId> bodyIds, Motphys.Native.SliceRef<Motphys.Isometry> outTransforms);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprBatchUpdateBodyShape(this WorldRef world, UpdateBodyShapes batchUpdates);

        #endregion

        #region RigidbodyApi
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprAddBody(this WorldRef world, ActorOptions options, out RigidbodyId id);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprRemoveBody(this WorldRef world, RigidbodyId id);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetBodyActive(this WorldRef world, RigidbodyId bodyId, bool active);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprWakeUpBody(this WorldRef world, RigidbodyId bodyId);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetBodyMass(this WorldRef world, RigidbodyId bodyId, float mass);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetBodyGravityEnabled(this WorldRef world, RigidbodyId bodyId, bool value);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprIsBodyGravityEnabled(this WorldRef world, RigidbodyId bodyId, out bool value);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetColliderMaterial(this WorldRef world, RigidbodyId bodyId, ChildColliderKey index, MaterialData value);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetCollisionFilter(this WorldRef world, RigidbodyId bodyId, ChildColliderKey index, CollisionMask value);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetNumColliders(this WorldRef world, RigidbodyId bodyId, out ulong size);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetFirstColliderKeyOnBody(this WorldRef world, RigidbodyId bodyId, out ChildColliderKey shapeIndex);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprAttachColliderToBody(this WorldRef world, RigidbodyId bodyId, ColliderOptions options, out ColliderId colliderId);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprDetachColliderFromBody(this WorldRef world, RigidbodyId bodyId, ChildColliderKey shapeIndex);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetColliderShape(this WorldRef world, RigidbodyId bodyId, ChildColliderKey index, Shape shape);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetShapeTransform(this WorldRef world, RigidbodyId bodyId, ChildColliderKey index, Motphys.Isometry transform);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetColliderTrigger(this WorldRef world, RigidbodyId bodyId, ChildColliderKey index, bool enabled);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprIsColliderTrigger(this WorldRef world, RigidbodyId bodyId, ChildColliderKey index, out bool enabled);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetBodyFreeze(this WorldRef world, RigidbodyId bodyId, Freeze value);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetBodyMass(this WorldRef world, RigidbodyId bodyId, out float mass);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetBodyInertia(this WorldRef world, RigidbodyId bodyId, out Vector3 inertiaPrincipal);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetBodyTransform(this WorldRef world, RigidbodyId bodyId, out Motphys.Isometry transform);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetBodyPosition(this WorldRef world, RigidbodyId bodyId, Vector3 position);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetKinematicTarget(this WorldRef world, RigidbodyId bodyId, Isometry transform);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetBodyRotation(this WorldRef world, RigidbodyId bodyId, Quaternion rotation);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetBodyVelocity(this WorldRef world, RigidbodyId bodyId, out Velocity outValue);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetBodyVelocity(this WorldRef world, RigidbodyId bodyId, UnityEngine.Vector3 linear, UnityEngine.Vector3 angular);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetBodyLinearVelocity(this WorldRef world, RigidbodyId bodyId, UnityEngine.Vector3 vel);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetBodyAngularVelocity(this WorldRef world, RigidbodyId bodyId, UnityEngine.Vector3 vel);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetColliderMaterial(this WorldRef world, RigidbodyId bodyId, ChildColliderKey index, out MaterialData outValue);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetCollisionFilter(this WorldRef world, RigidbodyId bodyId, ChildColliderKey index, out CollisionMask outValue);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetCollisionSetting(this WorldRef world, RigidbodyId bodyId, ChildColliderKey index, out CollisionSetting outValue);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetCollisionSetting(this WorldRef world, RigidbodyId bodyId, ChildColliderKey index, CollisionSetting value);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetColliderAabb(this WorldRef world, RigidbodyId bodyId, ChildColliderKey index, out Aabb3 outValue);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetBodyFreeze(this WorldRef world, RigidbodyId bodyId, out Freeze outValue);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprAddForceAtPosition(this WorldRef world, RigidbodyId bodyId, UnityEngine.Vector3 force, UnityEngine.Vector3 position, ForceMode forceMode);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprAddForce(this WorldRef world, RigidbodyId bodyId, UnityEngine.Vector3 force, ForceMode forceMode);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprAddRelativeForce(this WorldRef world, RigidbodyId bodyId, UnityEngine.Vector3 force, ForceMode forceMode);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprAddTorque(this WorldRef world, RigidbodyId bodyId, UnityEngine.Vector3 torque, ForceMode forceMode);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprAddRelativeTorque(this WorldRef world, RigidbodyId bodyId, UnityEngine.Vector3 torque, ForceMode forceMode);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetBodyMaxLinearVelocity(this WorldRef world, RigidbodyId bodyId, out float outValue);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetBodyMaxAngularVelocity(this WorldRef world, RigidbodyId bodyId, out float outValue);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetBodyMaxLinearVelocity(this WorldRef world, RigidbodyId bodyId, float value);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetBodyMaxAngularVelocity(this WorldRef world, RigidbodyId bodyId, float value);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetBodyMotionType(this WorldRef world, RigidbodyId bodyId, BodyMotionType type);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetColliderEnabled(this WorldRef world, RigidbodyId bodyId, ChildColliderKey index, bool enabled);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprIsColliderEnabled(this WorldRef world, RigidbodyId bodyId, ChildColliderKey index, out bool enabled);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetBodyLinearDamping(this WorldRef world, RigidbodyId bodyId, float value);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetBodyAngularDamping(this WorldRef world, RigidbodyId bodyId, float value);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetBodyLinearDamping(this WorldRef world, RigidbodyId bodyId, out float value);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetBodyAngularDamping(this WorldRef world, RigidbodyId bodyId, out float value);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetBodySleepEnergyThreshold(this WorldRef world, RigidbodyId bodyId, float value);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetBodySleepEnergyThreshold(this WorldRef world, RigidbodyId bodyId, out float value);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetBodyWakeCounter(this WorldRef world, RigidbodyId bodyId, out float value);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetBodyClockStatus(this WorldRef world, RigidbodyId bodyId, out SleepStatus value);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetBodyCenterOfMass(this WorldRef world, RigidbodyId bodyId, out Vector3 position);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetBodyCenterOfMass(this WorldRef world, RigidbodyId bodyId, Vector3 position);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprResetBodyCenterOfMass(this WorldRef world, RigidbodyId bodyId);
        #endregion

        #region Joint

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprAddJoint(this WorldRef world, Internal.JointBuilder joint, out JointId id);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprRemoveJoint(this WorldRef world, JointId jointId);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprUpdateJoint(this WorldRef world, JointId jointId, Internal.JointBuilder properties);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprSetJointActive(this WorldRef world, JointId jointId, bool active);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetJointForce(this WorldRef world, JointId jointId, out UnityEngine.Vector3 outValue);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetJointTorque(this WorldRef world, JointId jointId, out UnityEngine.Vector3 outValue);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetJointMotorTorques(this WorldRef world, JointId jointId, out UnityEngine.Vector3 outTorques);
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprGetJointMotorForces(this WorldRef world, JointId jointId, out UnityEngine.Vector3 outForces);
        #endregion

        #region Scene Query
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprRaycastClosest(this WorldRef world, Ray ray, float maxDistance, SceneQueryContext context, out OptionalRaycastHit raycastHit);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprRaycastAny(this WorldRef world, Ray ray, float maxDistance, SceneQueryContext context, out OptionalRaycastHit raycastHit);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprRaycastAll(this WorldRef world, Ray ray, float maxDistance, Motphys.Native.SliceRef<RaycastHit> outHitResults, SceneQueryContext context, out uint numHits);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprOverlapTest(this WorldRef world, Shape shape, Motphys.Isometry transform, Motphys.Native.SliceRef<ColliderId> outRigidbodyIndices, SceneQueryContext context, out uint numOverlaps);

        #endregion

        #region Tracing
        [DllImport(Constants.DLL)]
        public static extern void mprSetupChromeTracing();
        [DllImport(Constants.DLL)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool mprRecordChromeTracing([MarshalAs(UnmanagedType.LPStr)] string traceFilePath);

        #endregion

        #region Collider

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprCreateConvexHull(this WorldRef world, Motphys.Native.SliceRef<Vector3> vertices, out ShapeId id);

        [DllImport(Constants.DLL)]
        public static extern ResultCode mprDestroyConvexHull(this WorldRef world, ShapeId id);

        #endregion

        #region Statistics

        [DllImport(Constants.DLL)]
        public static extern StepMetrics mprGetAllMetrics(StepContinuation continuation);

        #endregion

        #region Utils
        [DllImport(Constants.DLL)]
        public static extern ResultCode mprCreateConvexMesh(Motphys.Native.SliceRef<Vector3> vertices, out IntPtr meshPtr);

        [DllImport(Constants.DLL)]
        public static extern int mprGetConvexTrianglesCount(IntPtr convexMesh);
        [DllImport(Constants.DLL)]
        public static extern int mprGetConvexVerticesCount(IntPtr convexMesh);

        [DllImport(Constants.DLL)]
        public static extern int mprGetConvexTriangles(IntPtr convexMesh, Motphys.Native.SliceRef<int> triangles);
        [DllImport(Constants.DLL)]
        public static extern int mprGetConvexVertices(IntPtr convexMesh, Motphys.Native.SliceRef<Vector3> vertices);
        [DllImport(Constants.DLL)]
        public static extern void mprDestroyConvexMesh(IntPtr convexMesh);
        #endregion

        #region Others
        [DllImport(Constants.DLL)]
        public static extern void mprInitLogger(LogLevelFilter level, [MarshalAs(UnmanagedType.LPStr)] string tag, [MarshalAs(UnmanagedType.LPStr)] string filters, bool isTest);
        #endregion
    }
}
