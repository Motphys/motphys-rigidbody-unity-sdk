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
using System.Runtime.InteropServices;
using Motphys.Native;
using UnityEngine;

namespace Motphys.Rigidbody
{
    /// <summary>
    /// Stragegy to combine numeric properties of two materials.
    /// </summary>
    public enum MaterialCombine
    {
        /// <summary>
        /// Average mode will compute final value by (A + B) / 2
        /// </summary>
        [Tooltip("Average mode will compute final value by (A + B) / 2")]
        Average,

        /// <summary>
        /// Minimum mode will compute final value by min(A, B)
        /// </summary>
        [Tooltip("Minimum mode will compute final value by min(A, B)")]
        Minimum,

        /// <summary>
        /// Multiply mode will compute final value by A * B
        /// </summary>
        [Tooltip("Multiply mode will compute final value by A * B")]
        Multiply,

        /// <summary>
        /// Maximum mode will compute final value by max(A, B)
        /// </summary>
        [Tooltip("Maximum mode will compute final value by max(A, B)")]
        Maximum,
    }

    /// <summary>
    /// Stragegy to combine boolean properties of two materials.
    /// </summary>
    public enum MaterialBoolCombine
    {
        /// <summary>
        /// Or mode will compute final value by A || B
        /// </summary>
        [Tooltip("Or mode will compute final value by A || B")]
        Or,

        /// <summary>
        /// And mode will compute final value by A&amp;&amp;B
        /// </summary>
        [Tooltip("And mode will compute final value by A && B")]
        And,
    }

    namespace Internal
    {
        internal enum BodyMotionType
        {
            Static,
            Kinematic,
            Dynamic,
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct RigidbodyId : System.IEquatable<RigidbodyId>
        {
            private readonly long _value;

            internal RigidbodyId(long value)
            {
                _value = value;
            }

            internal long value
            {
                get
                {
                    return _value;
                }
            }

            // ToString is used for debugging purposes only. Do not need to be covered.
            [UnityEngine.TestTools.ExcludeFromCoverage]
            public override string ToString()
            {
                return value.ToString();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                if (obj is RigidbodyId id)
                {
                    return Equals(id);
                }
                else
                {
                    return false;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetHashCode()
            {
                return value.GetHashCode();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(RigidbodyId other)
            {
                return this.value == other.value;
            }

            public bool isValid
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return NativeSlotId.IsValid(_value); }
            }

            internal uint version
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return NativeSlotId.GetVersion(_value); }
            }

            internal uint index
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return NativeSlotId.GetIndex(_value); }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator ==(RigidbodyId id1, RigidbodyId id2)
            {
                return id1.value == id2.value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator !=(RigidbodyId id1, RigidbodyId id2)
            {
                return id1.value != id2.value;
            }

            public static readonly RigidbodyId Invalid = new RigidbodyId(NativeSlotId.Invalid);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct ChildColliderKey : System.IEquatable<ChildColliderKey>
        {
            private readonly long _value;

            internal ChildColliderKey(long value)
            {
                _value = value;
            }
            internal long value => _value;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                return Equals((ChildColliderKey)obj);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetHashCode()
            {
                return value.GetHashCode();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(ChildColliderKey other)
            {
                return this.value == other.value;
            }

            public bool isValid
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return NativeSlotId.IsValid(_value); }
            }

            internal uint version
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return NativeSlotId.GetVersion(_value); }
            }

            internal uint index
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return NativeSlotId.GetIndex(_value); }
            }

            // ToString is used for debugging purposes only. Do not need to be covered.
            [UnityEngine.TestTools.ExcludeFromCoverage]
            public override string ToString()
            {
                return value.ToString();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator ==(ChildColliderKey id1, ChildColliderKey id2)
            {
                return id1.value == id2.value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator !=(ChildColliderKey id1, ChildColliderKey id2)
            {
                return id1.value != id2.value;
            }

            public static readonly ChildColliderKey Invalid = new ChildColliderKey(NativeSlotId.Invalid);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ColliderId : System.IEquatable<ColliderId>
        {
            private readonly RigidbodyId _bodyId;
            private readonly ChildColliderKey _childId;

            public RigidbodyId bodyId => _bodyId;
            public ChildColliderKey childId => _childId;

            public static readonly ColliderId Invalid = new ColliderId(RigidbodyId.Invalid, ChildColliderKey.Invalid);

            public ColliderId(RigidbodyId bodyid, ChildColliderKey childId)
            {
                _bodyId = bodyid;
                _childId = childId;
            }

            // ToString is used for debugging purposes only. Do not need to be covered.
            [UnityEngine.TestTools.ExcludeFromCoverage]
            public override string ToString()
            {
                return string.Format("body = {0}, child = {1}", _bodyId, _childId);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                if (obj is ColliderId id)
                {
                    return Equals(id);
                }
                else
                {
                    return false;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(ColliderId other)
            {
                return this.bodyId == other.bodyId && this.childId == other.childId;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator ==(ColliderId id1, ColliderId id2)
            {
                return id1.bodyId == id2.bodyId && id1.childId == id2.childId;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator !=(ColliderId id1, ColliderId id2)
            {
                return id1.bodyId != id2.bodyId || id1.childId != id2.childId;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetHashCode()
            {
                return (bodyId, childId).GetHashCode();
            }
        }

        /// <summary>
        /// The physical material of a rigid body.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        [System.Serializable]
        internal struct MaterialData
        {
            [SerializeField]
            [Range(0f, 1f)]
            [Tooltip("0 means no bounce, 1 means full bounce.")]
            private float _bounciness;

            [SerializeField]
            [Tooltip("More larger value means more faster to stop the body when it is moving on a surface")]
            private float _dynamicFriction;

            [SerializeField]
            [Tooltip("More larger value means more difficult to move the body when it is static on a surface")]
            private float _staticFriction;

            /// <summary>
            /// If true, collision event will be triggered when this body collides with other bodies. Disable it if you don't need collision event for this body to improve performance
            /// </summary>
            [MarshalAs(UnmanagedType.I1)]
            [Tooltip("If true, collision event will be triggered when this body collides with other bodies. Disable it if you don't need collision event for this body to improve performance")]
            public bool enableCollisionEvent;

            /// <summary>
            /// The strategy to combine bounciness of two materials.
            /// </summary>
            [Tooltip("The strategy to combine bounciness of two materials")]
            public MaterialCombine bounceCombine;
            /// <summary>
            /// The strategy to combine dynamic and static friction of two materials.
            /// </summary>
            [Tooltip("The strategy to combine dynamic and static friction of two materials")]
            public MaterialCombine frictionCombine;
            /// <summary>
            /// The strategy to combine `enableCollisionEvent` property of two materials.
            /// </summary>
            [Tooltip("The strategy to combine `enableCollisionEvent` property of two materials")]
            public MaterialBoolCombine collisionEventCombine;

            /// <value>
            /// This value determines how much an object bounces back after a collision.
            /// The valid range of bounciness is [0, 1]. 0 means no bounce, 1 means full bounce.
            /// Note: Value will be automatically clamped to [0, 1] if it is set outside the range.
            /// </value>
            public float bounciness
            {
                get { return _bounciness; }
                set { _bounciness = Mathf.Clamp(value, 0f, 1f); }
            }

            /// <value>
            /// The valid range of static friction is [dynamicFriction, +inf). More larger value means more difficult to move the body when it is static on a surface.
            ///
            /// Note: if the set value is smaller than dynamicFriction, dynamicFriction will be set to the same value as staticFriction.
            /// </value>
            ///
            /// <exception cref="System.ArgumentException">Thrown when the set value is negative.</exception>
            public float staticFriction
            {
                get { return _staticFriction; }
                set
                {
                    if (value < 0)
                    {
                        throw new System.ArgumentException("staticFriction can not be negative");
                    }

                    _staticFriction = value;
                    _dynamicFriction = Mathf.Min(_staticFriction, _dynamicFriction);
                }
            }

            /// <value>
            /// The valid range of dynamic friction is [0, +inf). More larger value means more faster to stop the body when it is moving on a surface.
            ///
            /// Note: if the set value is larger than staticFriction, staticFriction will be set to the same value as dynamicFriction.
            /// </value>
            ///
            /// <exception cref="System.ArgumentException">Thrown when the set value is negative.</exception>
            public float dynamicFriction
            {
                get { return _dynamicFriction; }
                set
                {
                    if (value < 0)
                    {
                        throw new System.ArgumentException("dynamicFriction can not be negative");
                    }

                    _dynamicFriction = value;
                    _staticFriction = Mathf.Max(_staticFriction, _dynamicFriction);
                }
            }

            public void SetFrictions(float dynamicFriction, float staticFriction)
            {
                if (dynamicFriction < 0f)
                {
                    throw new System.ArgumentException("dynamicFriction must be greater than or equal to 0");
                }

                if (staticFriction < dynamicFriction)
                {
                    throw new System.ArgumentException("staticFriction must be greater than or equal to dynamicFriction");
                }

                _dynamicFriction = dynamicFriction;
                _staticFriction = staticFriction;
            }

            /// <summary>
            /// The material data may be invalid when they are set from editor inspector. Call this method to clamp all values to valid range.
            /// </summary>
            /// Only data set from editor inspector may be invalid, so exclude from coverage.
            [UnityEngine.TestTools.ExcludeFromCoverage]
            public void Validate()
            {
                _bounciness = Mathf.Clamp(_bounciness, 0f, 1f);
                _dynamicFriction = Mathf.Max(_dynamicFriction, 0f);
                _staticFriction = Mathf.Max(_staticFriction, _dynamicFriction);
            }

            /// for debug only, exclude from coverage
            [UnityEngine.TestTools.ExcludeFromCoverage]
            public override string ToString()
            {
                return string.Format("bounciness = {0}, dynamicFriction = {1}, staticFriction = {2}, enableCollisionEvent = {3}, bounceCombine = {4}, frictionCombine = {5}, collisionEventCombine = {6}",
                    _bounciness, _dynamicFriction, _staticFriction, enableCollisionEvent, bounceCombine, frictionCombine, collisionEventCombine);
            }

            /// <summary>
            /// The default physical material.
            /// </summary>
            public static readonly MaterialData Default = new MaterialData()
            {
                _bounciness = 0f,
                _dynamicFriction = 0.5f,
                _staticFriction = 0.5f,
                enableCollisionEvent = true,
                collisionEventCombine = MaterialBoolCombine.Or,
            };
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Motion
        {
            public enum Tag
            {
                Static,
                Dynamic,
            }

            public Tag tag;
            private DynamicOptions _dynamicOptions;

            /// <summary>
            /// Get the dynamic options if the motion is dynamic.
            /// </summary>
            public bool TryGetDynamicOptions(out DynamicOptions options)
            {
                if (tag == Tag.Dynamic)
                {
                    options = _dynamicOptions;
                    return true;
                }
                else
                {
                    options = default;
                    return false;
                }
            }

            public static Motion Static
            {
                get
                {
                    return new Motion()
                    {
                        tag = Tag.Static,
                        _dynamicOptions = default,
                    };
                }
            }

            public static Motion CreateDynamic(DynamicOptions options)
            {
                return new Motion()
                {
                    tag = Tag.Dynamic,
                    _dynamicOptions = options,
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DynamicOptions
        {
            internal enum MassType
            {
                Automatic,
                Custom,
            }

            private MassType _massType;
            private float _mass;

            [MarshalAs(UnmanagedType.I1)]
            public bool isKinematic;

            public Velocity velocity;

            [MarshalAs(UnmanagedType.I1)]
            public bool useGravity;

            public float linearDamper;
            public float angularDamper;
            public Freeze freeze;
            public float sleepingEnergyThreshold;
            public float maxLinearVelocity;
            public float maxAngularVelocity;
            public CenterOfMass centerOfMass;
            public Inertia inertia;

            public DynamicOptions(bool isKinematic)
            {
                this.isKinematic = isKinematic;
                _massType = MassType.Custom;
                _mass = 1.0f;
                velocity = Velocity.Zero;
                useGravity = true;
                linearDamper = 0f;
                angularDamper = 0f;
                freeze = Freeze.None;
                sleepingEnergyThreshold = 0.04f;
                maxLinearVelocity = float.PositiveInfinity;
                maxAngularVelocity = float.PositiveInfinity;
                centerOfMass = CenterOfMass.Automatic;
                inertia = Inertia.Automatic;
            }

            public float mass
            {
                get
                {
                    return _mass;
                }
                set
                {
                    if (value < 0.0)
                    {
                        throw new System.ArgumentException($"Negative mass ({value}) is not allowed");
                    }

                    if (!float.IsFinite(value))
                    {
                        throw new System.ArgumentException($"mass = {value} is not finite.");
                    }

                    _massType = MassType.Custom;
                    _mass = value;
                }
            }

            /// <value>
            /// Create a dynamic options with kinematic set to false.
            /// </value>
            public static DynamicOptions Dynamic => new DynamicOptions(false);
        }

        internal enum CenterOfMassType
        {
            Automatic,
            Custom
        }

        internal struct CenterOfMass
        {
            public CenterOfMassType type;
            public Vector3 position;

            public static CenterOfMass Automatic => new CenterOfMass()
            {
                type = CenterOfMassType.Automatic,
                position = Vector3.zero,
            };

            public static CenterOfMass Custom(Vector3 position) => new CenterOfMass()
            {
                type = CenterOfMassType.Custom,
                position = position,
            };
        }

        internal enum InertiaType
        {
            Automatic,
            Custom
        }

        internal struct Inertia
        {
            public InertiaType type;

            public Quaternion rotation;
            public Vector3 diagInertia;

            public static Inertia Automatic => new Inertia()
            {
                type = InertiaType.Automatic,
                rotation = Quaternion.identity,
                diagInertia = Vector3.zero,
            };

            public static Inertia Custom(Quaternion rotation, Vector3 diagInertia) => new Inertia()
            {
                type = InertiaType.Custom,
                rotation = rotation,
                diagInertia = diagInertia,
            };
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ColliderOptions
        {
            public Shape shape;
            /// <summary>
            /// The shape to actor transform
            /// </summary>
            public Isometry shapeToActorTransform;
            public CollisionMask collisionMask;

            [MarshalAs(UnmanagedType.I1)]
            public bool isTrigger;

            [MarshalAs(UnmanagedType.I1)]
            public bool enableSimulation;

            public MaterialData material;

            public CollisionSetting collisionSetting;

            public enum MassOrDensity
            {
                Mass,
                Density,
            }

            public MassOrDensity massOrDensity;
            public float massOrDensityValue;

            public ColliderOptions(Shape shape)
            {
                this.shape = shape;
                shapeToActorTransform = Isometry.Identity;
                collisionMask = CollisionMask.Default;
                isTrigger = false;
                enableSimulation = true;
                material = MaterialData.Default;
                collisionSetting = CollisionSetting.Default;
                massOrDensity = MassOrDensity.Density;
                massOrDensityValue = 1.0f;
            }
        }

        internal struct ColliderGroupOptions
        {
            private ColliderOptions[] _colliders;

            public ColliderGroupOptions(ColliderOptions[] colliders)
            {
                _colliders = colliders;
            }

            public ColliderOptions[] colliders
            {
                get
                {
                    return _colliders;
                }
            }

            public static ColliderGroupOptions OneShape(Shape shape)
            {
                return new ColliderGroupOptions(new ColliderOptions[] { new ColliderOptions(shape) });
            }

            public static ColliderGroupOptions One(ColliderOptions collider)
            {
                return new ColliderGroupOptions(new ColliderOptions[] { collider });
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal ref struct ActorOptions
        {
            public Motion motion;
            /// <summary>
            /// The actor transform in world space.
            /// </summary>
            public Isometry transform;
            private SliceRef<ColliderOptions> _colliders;

            public ActorOptions(DynamicOptions dynamicOptions, ColliderOptions[] colliders)
            {
                this.motion = Motion.CreateDynamic(dynamicOptions);
                transform = Isometry.Identity;
                _colliders = new SliceRef<ColliderOptions>(colliders, colliders.Length);
            }

            public ActorOptions(DynamicOptions dynamicOptions, ColliderGroupOptions colliders) : this(dynamicOptions, colliders.colliders)
            {
            }

            public ActorOptions(Motion motion, Isometry transform, ColliderOptions[] colliders)
            {
                this.motion = motion;
                this.transform = transform;
                _colliders = new SliceRef<ColliderOptions>(colliders, colliders.Length);
            }

            public ActorOptions(Motion motion, ColliderOptions[] colliders) : this(motion, Isometry.Identity, colliders)
            {
            }

            public ActorOptions(Motion motion, ColliderGroupOptions colliders) : this(motion, colliders.colliders)
            {
            }

            public System.Span<ColliderOptions> colliders
            {
                get
                {
                    return _colliders;
                }
            }
        }
    }
}
