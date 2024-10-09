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
using Motphys.Rigidbody.Api;
using UnityEngine;

namespace Motphys.Rigidbody.Internal
{
    internal struct RigidbodyHandle
    {
        private PhysicsWorld _world;
        private RigidbodyId _id;

        public RigidbodyHandle(PhysicsWorld world, RigidbodyId id)
        {
            _world = world;
            _id = id;
        }

        public RigidbodyId id
        {
            get { return _id; }
        }

        internal bool isEngineDisposed
        {
            get
            {
                return _world.engine.isDisposed;
            }
        }

        internal bool isValid
        {
            get
            {
                return _id.isValid && _world.isValid;
            }
        }

        private WorldRef nativeWorld
        {
            get
            {
                return _world.@ref;
            }
        }

        internal bool RemoveFromWorld()
        {
            var code = PhysicsNativeApi.mprRemoveBody(_world.@ref, _id);
            if (code == ResultCode.BodyNotFound)
            {
                return false;
            }

            code.ThrowExceptionIfNotOk();
            return true;
        }

        internal void SetActive(bool value)
        {
            nativeWorld.mprSetBodyActive(id, value).ThrowExceptionIfNotOk();
        }

        internal void WakeUp()
        {
            nativeWorld.mprWakeUpBody(id).ThrowExceptionIfNotOk();
        }

        public Aabb3 GetColliderAabb(ChildColliderKey index)
        {
            nativeWorld.mprGetColliderAabb(id, index, out var value).ThrowExceptionIfNotOk();
            return value;
        }

        public Isometry transform
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                nativeWorld.mprGetBodyTransform(id, out var value).ThrowExceptionIfNotOk();
                return value;
            }
        }

        public Vector3 position
        {
            get { return transform.position; }
            set
            {
                nativeWorld.mprSetBodyPosition(id, value).ThrowExceptionIfNotOk();
            }
        }

        public Quaternion rotation
        {
            get { return transform.rotation; }
            set
            {
                nativeWorld.mprSetBodyRotation(id, value).ThrowExceptionIfNotOk();
            }
        }

        public Velocity velocity
        {
            get
            {
                nativeWorld.mprGetBodyVelocity(id, out var value).ThrowExceptionIfNotOk();
                return value;
            }
        }

        public void SetKinematicTarget(Isometry transform)
        {
            nativeWorld.mprSetKinematicTarget(id, transform).ThrowExceptionIfNotOk();
        }

        public void SetVelocity(Vector3 linear, Vector3 angular)
        {
            nativeWorld.mprSetBodyVelocity(id, linear, angular).ThrowExceptionIfNotOk();
        }

        public void SetLinearVelocity(Vector3 linearVel)
        {
            nativeWorld.mprSetBodyLinearVelocity(id, linearVel).ThrowExceptionIfNotOk();
        }

        public void SetAngularVelocity(Vector3 angularVel)
        {
            nativeWorld.mprSetBodyAngularVelocity(id, angularVel).ThrowExceptionIfNotOk();
        }

        public float maxLinearVelocity
        {
            get
            {
                nativeWorld.mprGetBodyMaxLinearVelocity(id, out var value).ThrowExceptionIfNotOk();
                return value;
            }
            set
            {
                nativeWorld.mprSetBodyMaxLinearVelocity(id, value).ThrowExceptionIfNotOk();
            }
        }

        public float maxAngularVelocity
        {
            get
            {
                nativeWorld.mprGetBodyMaxAngularVelocity(id, out var value).ThrowExceptionIfNotOk();
                return value;
            }
            set
            {
                nativeWorld.mprSetBodyMaxAngularVelocity(id, value).ThrowExceptionIfNotOk();
            }
        }

        public void SetBodyMotionType(BodyMotionType type)
        {
            nativeWorld.mprSetBodyMotionType(id, type).ThrowExceptionIfNotOk();
        }

        public bool IsCollisionEnabled(ChildColliderKey index)
        {
            nativeWorld.mprIsColliderEnabled(id, index, out var value).ThrowExceptionIfNotOk();
            return value;
        }

        public void SetCollisionEnabled(ChildColliderKey index, bool enabled)
        {
            nativeWorld.mprSetColliderEnabled(id, index, enabled).ThrowExceptionIfNotOk();
        }

        public void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode forceMode)
        {
            nativeWorld.mprAddForceAtPosition(id, force, position, forceMode).ThrowExceptionIfNotOk();
        }

        public void AddForce(Vector3 force, ForceMode forceMode)
        {
            nativeWorld.mprAddForce(id, force, forceMode).ThrowExceptionIfNotOk();
        }

        public void AddRelativeForce(Vector3 force, ForceMode forceMode)
        {
            nativeWorld.mprAddRelativeForce(id, force, forceMode).ThrowExceptionIfNotOk();
        }

        public void AddTorque(Vector3 torque, ForceMode forceMode)
        {
            nativeWorld.mprAddTorque(id, torque, forceMode).ThrowExceptionIfNotOk();
        }

        public void AddRelativeTorque(Vector3 torque, ForceMode forceMode)
        {
            nativeWorld.mprAddRelativeTorque(id, torque, forceMode).ThrowExceptionIfNotOk();
        }

        public bool isGravityEnabled
        {
            get
            {
                nativeWorld.mprIsBodyGravityEnabled(id, out var value).ThrowExceptionIfNotOk();
                return value;
            }
            set
            {
                nativeWorld.mprSetBodyGravityEnabled(id, value).ThrowExceptionIfNotOk();
            }
        }

        public Freeze freeze
        {
            get
            {
                nativeWorld.mprGetBodyFreeze(id, out var value).ThrowExceptionIfNotOk();
                return value;
            }
            set
            {
                nativeWorld.mprSetBodyFreeze(id, value).ThrowExceptionIfNotOk();
            }
        }

        public float mass
        {
            get
            {
                nativeWorld.mprGetBodyMass(id, out var value).ThrowExceptionIfNotOk();
                return value;
            }
            set
            {
                nativeWorld.mprSetBodyMass(id, value).ThrowExceptionIfNotOk();
            }
        }

        public Vector3 inertiaPrincipal
        {
            get
            {
                nativeWorld.mprGetBodyInertia(id, out var value).ThrowExceptionIfNotOk();
                return value;
            }
        }

        internal MaterialData GetMaterial(ChildColliderKey index)
        {
            nativeWorld.mprGetColliderMaterial(id, index, out var value).ThrowExceptionIfNotOk();
            return value;
        }

        internal void SetMaterial(ChildColliderKey index, MaterialData value)
        {
            nativeWorld.mprSetColliderMaterial(id, index, value).ThrowExceptionIfNotOk();
        }

        internal CollisionMask GetCollisionMask(ChildColliderKey index)
        {
            nativeWorld.mprGetCollisionFilter(id, index, out var value).ThrowExceptionIfNotOk();
            return value;
        }

        internal void SetCollisionMask(ChildColliderKey index, CollisionMask mask)
        {
            nativeWorld.mprSetCollisionFilter(id, index, mask).ThrowExceptionIfNotOk();
        }

        internal CollisionSetting GetCollisionSetting(ChildColliderKey index)
        {
            nativeWorld.mprGetCollisionSetting(id, index, out var value).ThrowExceptionIfNotOk();
            return value;
        }

        internal void SetCollisionSetting(ChildColliderKey index, CollisionSetting value)
        {
            nativeWorld.mprSetCollisionSetting(id, index, value).ThrowExceptionIfNotOk();
        }

        public bool IsTrigger(ChildColliderKey index)
        {
            nativeWorld.mprIsColliderTrigger(id, index, out var value).ThrowExceptionIfNotOk();
            return value;
        }

        public void SetIsTrigger(ChildColliderKey index, bool isTrigger)
        {
            nativeWorld.mprSetColliderTrigger(id, index, isTrigger).ThrowExceptionIfNotOk();
        }

        public Vector3 GetBodyCenterOfMass()
        {
            nativeWorld.mprGetBodyCenterOfMass(id, out var position).ThrowExceptionIfNotOk();
            return position;
        }

        public void SetBodyCenterOfMass(Vector3 position)
        {
            nativeWorld.mprSetBodyCenterOfMass(id, position).ThrowExceptionIfNotOk();
        }

        public void ResetBodyCenterOfMass()
        {
            nativeWorld.mprResetBodyCenterOfMass(id).ThrowExceptionIfNotOk();
        }

        internal bool hasShape => numShapes != 0;

        internal ulong numShapes
        {
            get
            {
                nativeWorld.mprGetNumColliders(id, out var num).ThrowExceptionIfNotOk();
                return num;
            }
        }

        internal ChildColliderKey FirstShapeKey()
        {
            nativeWorld.mprGetFirstColliderKeyOnBody(id, out var key).ThrowExceptionIfNotOk();
            return key;
        }

        internal ColliderId AttachCollider(ColliderOptions options)
        {
            nativeWorld.mprAttachColliderToBody(id, options, out var key).ThrowExceptionIfNotOk();
            return key;
        }

        internal void DeatchCollider(ChildColliderKey key)
        {
            nativeWorld.mprDetachColliderFromBody(id, key).ThrowExceptionIfNotOk();
        }

        internal void UpdateShape(ChildColliderKey key, Shape shape)
        {
            nativeWorld.mprSetColliderShape(id, key, shape).ThrowExceptionIfNotOk();
        }

        internal void SetShapeTransform(ChildColliderKey key, Vector3 position, Quaternion rotation)
        {
            nativeWorld.mprSetShapeTransform(id, key, new Isometry(position, rotation)).ThrowExceptionIfNotOk();
        }

        /// <exception cref="System.ArgumentException">Negative linear damper is not allowed</exception>
        internal float linearDamping
        {
            set
            {
                if (value < 0)
                {
                    throw new System.ArgumentException("Negative linear damper is not allowed");
                }

                nativeWorld.mprSetBodyLinearDamping(id, value).ThrowExceptionIfNotOk();
            }
            get
            {
                nativeWorld.mprGetBodyLinearDamping(id, out var value).ThrowExceptionIfNotOk();
                return value;
            }
        }

        /// <exception cref="System.ArgumentException">Negative angular damper is not allowed</exception>
        internal float angularDamping
        {
            set
            {
                if (value < 0)
                {
                    throw new System.ArgumentException("Negative angular damper is not allowed");
                }

                nativeWorld.mprSetBodyAngularDamping(id, value).ThrowExceptionIfNotOk();
            }
            get
            {
                nativeWorld.mprGetBodyAngularDamping(id, out var value).ThrowExceptionIfNotOk();
                return value;
            }
        }

        internal float sleepThreshold
        {
            get
            {
                nativeWorld.mprGetBodySleepEnergyThreshold(id, out var value).ThrowExceptionIfNotOk();
                return value;
            }
            set
            {
                if (value < 0)
                {
                    throw new System.ArgumentException("Negative sleep threshold is not allowed");
                }

                nativeWorld.mprSetBodySleepEnergyThreshold(id, value).ThrowExceptionIfNotOk();
            }
        }

        internal float wakeCounter
        {
            get
            {
                nativeWorld.mprGetBodyWakeCounter(id, out var value).ThrowExceptionIfNotOk();
                return value;
            }
        }

        internal SleepStatus sleepStatus
        {
            get
            {
                nativeWorld.mprGetBodyClockStatus(id, out var value).ThrowExceptionIfNotOk();
                return value;
            }
        }

        public override string ToString()
        {
            return string.Format("World = {0}, Body = {1}", _world.id, _id);
        }

        public static readonly RigidbodyHandle Invalid = new RigidbodyHandle(default, RigidbodyId.Invalid);
    }
}
