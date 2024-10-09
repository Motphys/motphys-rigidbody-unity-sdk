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

using System.Collections.Generic;
using UnityEngine;

namespace Motphys.Rigidbody.Internal
{

    // TODO: improve test coverage.
    // http://gitlab.mp/motphys/motphys_sdk_remake/-/issues/587
    internal class BodyNativeBridge
    {
        private RigidbodyHandle _handle = RigidbodyHandle.Invalid;
        private Rigidbody3D _rigidbody;

        // Assigned only on creation.
        private GameObject _gameObject;

        internal GameObject gameObject => _gameObject;

        internal Rigidbody3D rigidbody => _rigidbody;

        internal BodyNativeBridge(Rigidbody3D rigidbody3D)
        {
            _rigidbody = rigidbody3D;
            _gameObject = rigidbody3D.gameObject;
            _handle = CreateRigidbodyNative(_rigidbody, null);
        }

        internal BodyNativeBridge(BaseCollider collider)
        {
            _gameObject = collider.gameObject;
            _handle = CreateRigidbodyNative(null, collider);
            // Debug.Assert(hasShape);
        }

        internal void DestroyNativeImmendiate()
        {
            if (_handle.isEngineDisposed)
            {
                return;
            }

            var exist = _handle.RemoveFromWorld();
            Debug.Assert(exist);
        }

        internal RigidbodyHandle handle => _handle;

        internal bool isEngineDisposed
        {
            get
            {
                return _handle.isEngineDisposed;
            }
        }

        internal RigidbodyId id
        {
            get
            {
                return _handle.id;
            }
        }

        internal ColliderId AttachCollider(BaseCollider collider)
        {
            Debug.Assert(_rigidbody != null);

            var success = collider.TryExtractOptions(out var options);

            Debug.Assert(success);

            var id = handle.AttachCollider(options);

            // This property of all colliders of the same body should have the same value.
            SetCollisionEnabled(id.childId, _rigidbody.detectCollisions);

            return id;
        }

        internal void DetachCollider(ChildColliderKey ChildIndex)
        {
            Debug.Assert(_rigidbody);

            handle.DeatchCollider(ChildIndex);
        }

        internal bool useGravity
        {
            set
            {
                _handle.isGravityEnabled = value;
            }
        }

        internal float mass
        {
            set
            {
                _handle.mass = value;
            }
        }

        internal Vector3 position
        {
            get => _handle.position;
            set
            {
                _handle.position = value;
            }
        }

        internal Quaternion rotation
        {
            get => _handle.rotation;
            set
            {
                _handle.rotation = value;
            }
        }

        internal Vector3 linearVelocity
        {
            get => _handle.velocity.linear;
            set
            {
                _handle.SetLinearVelocity(value);
            }
        }

        internal Vector3 angularVelocity
        {
            get => _handle.velocity.angular;
            set
            {
                _handle.SetAngularVelocity(value);
            }
        }

        internal bool isKinematic
        {
            set
            {
                _handle.SetBodyMotionType(value ? BodyMotionType.Kinematic : BodyMotionType.Dynamic);

                if (!value)
                {
                    // change type to dynamic. need to push dynamic options to native.
                    UpdateNativeDynamicProperties();
                }
            }
        }

        internal void SetKinematicTarget(Vector3 position, Quaternion rotation)
        {
            _handle.SetKinematicTarget(new Isometry(position, rotation));
        }

        internal SleepStatus sleepStatus => _handle.sleepStatus;

        internal float wakeCounter => _handle.wakeCounter;

        internal float maxLinearVelocity
        {
            set
            {
                if (value < 0.0f)
                {
                    throw new System.ArgumentException("maxLinearVelocity must be greater than or equal to 0.0f");
                }

                _handle.maxLinearVelocity = value;
            }
        }

        internal float maxAngularVelocity
        {
            set
            {
                if (value < 0.0f)
                {
                    throw new System.ArgumentException("maxAngularVelocity must be greater than or equal to 0.0f");
                }

                _handle.maxAngularVelocity = value;
            }
        }

        internal void WakeUp()
        {
            _handle.WakeUp();
        }

        internal bool IsCollisionEnabled(ChildColliderKey index)
        {
            return _handle.IsCollisionEnabled(index);
        }

        internal void SetCollisionEnabled(ChildColliderKey index, bool enabled)
        {
            _handle.SetCollisionEnabled(index, enabled);
        }

        internal void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode forceMode)
        {
            _handle.AddForceAtPosition(force, position, forceMode);
        }

        internal void AddForce(Vector3 force, ForceMode forceMode)
        {
            _handle.AddForce(force, forceMode);
        }

        internal void AddRelativeForce(Vector3 force, ForceMode forceMode)
        {
            _handle.AddRelativeForce(force, forceMode);
        }

        internal void AddTorque(Vector3 torque, ForceMode forceMode)
        {
            _handle.AddTorque(torque, forceMode);
        }

        internal void AddRelativeTorque(Vector3 torque, ForceMode forceMode)
        {
            _handle.AddRelativeTorque(torque, forceMode);
        }

        internal void SetCollisionMask(ChildColliderKey index, CollisionMask mask)
        {
            _handle.SetCollisionMask(index, mask);
        }

        internal void SetCollisionSetting(ChildColliderKey index, CollisionSetting setting)
        {
            _handle.SetCollisionSetting(index, setting);
        }

        internal void SetShapeTransform(ChildColliderKey index, Vector3 translation, Quaternion rotation)
        {
            handle.SetShapeTransform(index, translation, rotation);
        }

        internal bool TryUpdateNativeShape(ChildColliderKey index, BaseCollider collider)
        {
            if (collider.TryCreateShape(out var builder))
            {
                handle.UpdateShape(index, builder);
                return true;
            }

            return false;
        }

        internal Aabb3 GetColliderAabb(ChildColliderKey index)
        {
            return handle.GetColliderAabb(index);
        }

        internal MaterialData GetMaterial(ChildColliderKey index)
        {
            return _handle.GetMaterial(index);
        }

        internal void SetMaterial(ChildColliderKey index, MaterialData value)
        {
            _handle.SetMaterial(index, value);
        }

        internal bool IsTrigger(ChildColliderKey index)
        {
            return _handle.IsTrigger(index);
        }

        internal void SetIsTrigger(ChildColliderKey index, bool isTrigger)
        {
            _handle.SetIsTrigger(index, isTrigger);
        }

        internal Vector3 GetBodyCenterOfMass()
        {
            return _handle.GetBodyCenterOfMass();
        }

        internal void SetBodyCenterOfMass(Vector3 position)
        {
            _handle.SetBodyCenterOfMass(position);
        }

        internal void ResetBodyCenterOfMass()
        {
            _handle.ResetBodyCenterOfMass();
        }

        internal float linearDamper
        {
            get => _handle.linearDamping;
            set
            {
                _handle.linearDamping = value;
            }
        }

        internal float angularDamper
        {
            get => _handle.angularDamping;
            set
            {
                _handle.angularDamping = value;
            }
        }

        internal float sleepThreshold
        {
            get => _handle.sleepThreshold;
            set
            {
                _handle.sleepThreshold = value;
            }
        }

        private void UpdateNativeDynamicProperties()
        {
            _handle.freeze = _rigidbody.ExtractFreeze();
            _handle.maxAngularVelocity = _rigidbody.maxAngularVelocity;
            _handle.maxLinearVelocity = _rigidbody.maxLinearVelocity;
            _handle.sleepThreshold = _rigidbody.sleepThreshold;
        }

        internal void UpdateFreezeOptions()
        {
            _handle.freeze = _rigidbody.ExtractFreeze();
        }

        internal void UpdateNativeRigidbodyProperties()
        {
            _handle.SetBodyMotionType(_rigidbody.isKinematic ? BodyMotionType.Kinematic : BodyMotionType.Dynamic);
            _handle.isGravityEnabled = _rigidbody.useGravity;
            _handle.mass = _rigidbody.mass;
            _handle.linearDamping = _rigidbody.drag;
            _handle.angularDamping = _rigidbody.angularDrag;
            if (!_rigidbody.isKinematic)
            {
                UpdateNativeDynamicProperties();
            }
        }

        internal static ActorOptions GetActorOptions(Rigidbody3D rigidbody, BaseCollider[] colliders)
        {
            Transform transform;
            if (rigidbody)
            {
                transform = rigidbody.transform;
            }
            else if (colliders != null)
            {
                transform = colliders[0].transform;

                for (int i = 1; i < colliders.Length; i++)
                {
                    if (colliders[i].transform != transform)
                    {
                        throw new System.ArgumentException($"when rigidbody is null, all colliders should have the same transform, but {colliders[i].transform} and {transform} are not the same");
                    }
                }
            }
            else
            {
                throw new System.ArgumentException("rigidbody and collider are both null");
            }

            var colliderOptions = new List<ColliderOptions>();

            if (colliders != null)
            {
                foreach (var collider in colliders)
                {
                    if (collider != null && collider.TryCreateShape(out var builder))
                    {
                        var options = new ColliderOptions(builder)
                        {
                            shapeToActorTransform = collider.expectNativeShapeTransform,
                            material = collider.actualMaterial == null ? MaterialData.Default : collider.actualMaterial.data,
                            collisionMask = collider.collisionMask,
                            isTrigger = collider.isTrigger,
                            collisionSetting = collider.collisionSetting,
                        };
                        colliderOptions.Add(options);
                    }
                }
            }

            Motion motion;
            if (rigidbody == null)
            {
                motion = Motion.Static;
            }
            else
            {
                var dynamicOptions = new DynamicOptions(rigidbody.isKinematic)
                {
                    mass = rigidbody.mass,
                    useGravity = rigidbody.useGravity,
                    linearDamper = rigidbody.drag,
                    angularDamper = rigidbody.angularDrag,
                    freeze = rigidbody.ExtractFreeze(),
                    sleepingEnergyThreshold = rigidbody.sleepThreshold,
                    maxAngularVelocity = rigidbody.maxAngularVelocity,
                    maxLinearVelocity = rigidbody.maxLinearVelocity,
                };
                motion = Motion.CreateDynamic(dynamicOptions);
            }

            var transformData = new Isometry()
            {
                position = transform.position,
                rotation = transform.rotation,
            };

            return new ActorOptions(motion, transformData, colliderOptions.ToArray());
        }

        internal static RigidbodyHandle CreateRigidbodyNative(Rigidbody3D rigidbody, BaseCollider collider)
        {
            var options = GetActorOptions(rigidbody, collider != null ? new BaseCollider[] { collider } : null);
            return PhysicsManager.engine.defaultWorld.AddRigidbody(
                options
            );
        }
    }
}
