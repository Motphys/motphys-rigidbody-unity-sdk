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
using UnityEngine;

namespace Motphys.Rigidbody
{
    /// <summary>
    /// The type of motion in a specific degree of freedom of a particular joint.
    /// </summary>
    public enum JointDegreeMotion : byte
    {
        /// <summary>
        /// Unconstrained motion
        /// </summary>
        Free,
        /// <summary>
        /// Preventing the freedom of motion
        /// </summary>
        Locked,
        /// <summary>
        /// Partially constrained motion
        /// </summary>
        Limited,
    }

    /// <summary>
    /// The struct contains two coefficients:
    ///
    /// - compliance
    ///     - The inverse of stiffness.
    ///     - Valid range is `[0,infinite]`.  0 means infinite force and lead to absolute rigid
    ///       constraint.
    /// - damper
    ///     - The coefficient used to apply a damper force to dampen the spring.
    ///     - Valid range is `[0,infinite]`. 0 means disable damper.
    /// </summary>
    [System.Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct SpringDamper
    {
        [SerializeField]
        [Tooltip("Compliance is the inverse of stiffness, with UNIT of m/N. 0 means infinitely stiff constraint")]
        private float _compliance;

        [SerializeField]
        [Tooltip("Damper coefficient of the spring. Valid range is [0, inf)")]
        private float _damper;

        /// <summary>
        /// Create a spring damper with given stiffness and damper.
        /// </summary>
        public SpringDamper(float stiffness, float damper)
        {
            if (stiffness < 0f)
            {
                throw new System.ArgumentException($"stiffness must be non-negative: {stiffness}");
            }

            if (damper < 0f)
            {
                throw new System.ArgumentException($"damper must be non-negative: {damper}");
            }

            _compliance = 1f / stiffness;
            _damper = damper;
        }

        /// <value>
        /// Compliance is the inverse of stiffness, with UNIT of m/N. 0 means infinitely stiff constraint.
        /// </value>
        /// <exception cref="System.ArgumentException">Thrown when value is set to negative</exception>
        public float compliance
        {
            get { return _compliance; }
            set
            {
                if (value < 0f)
                {
                    throw new System.ArgumentException($"compliance must be non-negative: {value}");
                }

                _compliance = value;
            }
        }

        /// <value>
        /// Stiffness is the extent to which an object resists deformation in response to an applied force, which has UNIT of N/m.
        ///
        /// Valid range is [0, inf)
        /// </value>
        /// <exception cref="System.ArgumentException">Thrown when value is set to negative</exception>
        public float stiffness
        {
            get { return 1f / _compliance; }
            set
            {
                if (value < 0f)
                {
                    throw new System.ArgumentException($"stiffness must be non-negative: {value}");
                }

                compliance = 1f / value;
            }
        }

        /// <value>
        /// Damping coefficient will provide a force in the opposite direction and proportional to the velocity of the movement.
        /// </value>
        /// <exception cref="System.ArgumentException">Thrown when value is set to negative</exception>
        public float damper
        {
            get { return _damper; }
            set
            {
                if (value < 0f)
                {
                    throw new System.ArgumentException($"Damper must be non-negative: {value}");
                }

                _damper = value;
            }
        }

        /// <summary>
        /// Validate the compliance and damper value and automatically correct them if they are invalid.
        /// </summary>
        public void Validate()
        {
            _compliance = Mathf.Max(_compliance, 0);
            _damper = Mathf.Max(_damper, 0);
        }

        /// <value>
        /// The default spring has 100 stiffness and 10 damping. May change in the future.
        /// </value>
        public static readonly SpringDamper Default = new SpringDamper()
        {
            _compliance = 0.01f,
            _damper = 10,
        };

        /// <value>
        /// A spring with infinite stiffness.
        /// </value>
        public static readonly SpringDamper InfiniteStiffness = new SpringDamper()
        {
            _compliance = 0,
            _damper = 0,
        };

        /// <value>
        /// Inactive spring have 0 stiffness and 0 damping.
        /// </value>
        public static readonly SpringDamper Inactive = new SpringDamper()
        {
            _compliance = float.PositiveInfinity,
            _damper = 0,
        };
    }

    /// <summary>
    /// Represent the joint motion degrees of freedom along the x, y, and z axes.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 1)]
    public struct D3Motion
    {
        [FieldOffset(0)]
        private byte _value;

        public D3Motion(JointDegreeMotion x, JointDegreeMotion y, JointDegreeMotion z)
        {
            var vx = (byte)x;
            var vy = (byte)y;
            var vz = (byte)z;
            _value = (byte)((vz << 4) | (vy << 2) | vx);
        }

        /// <value>
        /// The joint motion degrees of x axis.
        /// </value>
        public JointDegreeMotion x
        {
            get { return (JointDegreeMotion)(_value & 3); }
            set
            {
                var i = (byte)value;
                _value = (byte)((_value & 0b11111100) | i);
            }
        }

        /// <value>
        /// The joint motion degrees of y axis.
        /// </value>
        public JointDegreeMotion y
        {
            get { return (JointDegreeMotion)((_value >> 2) & 3); }
            set
            {
                var i = (byte)value;
                _value = (byte)((_value & 0b11110011) | (i << 2));
            }
        }

        /// <value>
        /// The joint motion degrees of z axis.
        /// </value>
        public JointDegreeMotion z
        {
            get { return (JointDegreeMotion)((_value >> 4) & 3); }
            set
            {
                var i = (byte)value;
                _value = (byte)((_value & 0b11001111) | (i << 4));
            }
        }
    }

    /// <summary>
    /// The translational and rotational degrees of freedom.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct D6Motion
    {
        /// <value>
        /// Translational degrees of freedom along the x, y, and z axes.
        /// </value>
        public D3Motion translation;
        /// <value>
        /// Rotational degrees of freedom around the x, y, and z axes.
        /// </value>
        public D3Motion rotation;
    }

    /// <summary>
    /// Used for applying constraint force.
    /// The anchor can be considered as the point of application of the joint constraint force on the body, typically used in position constraints.
    /// The axisRotation is typically used for the rotational constraints of the joint.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [System.Serializable]
    public struct AxisFrame
    {
        /// <value>
        /// The position of anchor point in local space.
        /// </value>
        public Vector3 anchor;
        /// <value>
        /// The rotation of anchor point in local space.
        /// </value>
        public Vector3 axisRotation;

        /// <value>
        /// The anchor rotation in quaternion.
        /// </value>
        public Quaternion rotation
        {
            get
            {
                var quat = Quaternion.Euler(axisRotation);
                return quat;
            }
        }

        internal Vector3 axisX => rotation * Vector3.right;

        internal Vector3 axisY => rotation * Vector3.up;

        public AxisFrame(Vector3 anchor, Vector3 axisRotation)
        {
            this.anchor = anchor;
            this.axisRotation = axisRotation;
        }

        /// <value>
        /// Identity frame with zero translation and zero rotation.
        /// </value>
        public static readonly AxisFrame Identity = new AxisFrame()
        {
            anchor = Vector3.zero,
            axisRotation = Vector3.zero
        };

        internal Matrix4x4 ToMatrix()
        {
            var isometry = ToIsometry();
            return Matrix4x4.TRS(isometry.position, isometry.rotation, Vector3.one);
        }

        internal Isometry ToIsometry()
        {
            return new Isometry(anchor, rotation);
        }
    }

    /// <summary>
    /// Provide spring and maxTorque settings for a angular motor.
    /// </summary>
    [System.Serializable]
    public struct AngularMotorDrive
    {
        /// <value>
        ///  The spring used to drive the motor.
        /// </value>
        [Tooltip("The spring used to drive the motor")]
        public SpringDamper spring;
        /// <value>
        ///  The maximum force the motor can apply to achieve the target angle and target speed.
        ///
        ///  <para> Non-positive value means disable the motor.</para>
        /// </value>
        [Tooltip("The maximum force the motor can apply to achieve the target angle and target speed")]
        public float maxTorque;

        public AngularMotorDrive(SpringDamper spring, float maxTorque)
        {
            this.spring = spring;
            this.maxTorque = maxTorque;
        }

        /// <value>
        /// By default, the motor is disabled (maxTorque == 0.0).
        /// </value>
        public static readonly AngularMotorDrive Default = new AngularMotorDrive()
        {
            spring = SpringDamper.Default,
            maxTorque = float.PositiveInfinity,
        };
    }

    /// <summary>
    ///  The drive mode of the angular motor.
    /// </summary>
    public enum AngularMotorDriveMode
    {
        /// <summary>
        /// In SLerp mode, the motor will drive the rotation to the target rotation in shortest path in the space of quaternions.
        /// </summary>
        [Tooltip("The motor will drive the rotation to the target rotation in shortest path in the space of quaternions")]
        SLerp,
        /// <summary>
        /// In TwistSwing mode, the motor first decomposes the target rotation into a twist and a swing, and then drives the twist and swing separately.
        /// </summary>
        [Tooltip("the motor first decomposes the target rotation into a twist and a swing, and then drives the twist and swing separately.")]
        TwistSwing,
        /// <summary>
        ///  In Velocity mode, the motor will drive the rotation based on target angular velocity
        /// </summary>
        [Tooltip("the motor will drive the rotation based on target angular velocity")]
        Velocity,
    }

    namespace Internal
    {

        [StructLayout(LayoutKind.Sequential)]
        internal struct JointId : System.IEquatable<JointId>
        {
            private long _value;

            internal JointId(long value)
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

                return Equals((JointId)obj);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetHashCode()
            {
                return value.GetHashCode();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(JointId other)
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
            public static bool operator ==(JointId id1, JointId id2)
            {
                return id1.value == id2.value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator !=(JointId id1, JointId id2)
            {
                return id1.value != id2.value;
            }

            public static readonly JointId Invalid = new JointId(NativeSlotId.Invalid);
        }

        internal enum JointType
        {
            Distance,
            Fixed,
            Hinge,
            Slider,
            Ball,
            Universal,
            D6,
            Ellipsoid
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AnchorFramePair
        {
            public Isometry frameA;
            public Isometry frameB;

            public AnchorFramePair(Isometry frameA, Isometry frameB)
            {
                this.frameA = frameA;
                this.frameB = frameB;
            }

            public static AnchorFramePair FromAnchorPosition(Vector3 anchorPositionA, Vector3 anchorPositionB)
            {
                var pair = new AnchorFramePair();
                pair.frameA.position = anchorPositionA;
                pair.frameB.position = anchorPositionB;
                pair.frameA.rotation = Quaternion.identity;
                pair.frameB.rotation = Quaternion.identity;

                return pair;
            }

            public static AnchorFramePair FromAxisFrame(AxisFrame axisFrameA, AxisFrame axisFrameB)
            {
                return new AnchorFramePair(axisFrameA.ToIsometry(), axisFrameB.ToIsometry());
            }

            public static readonly AnchorFramePair Identity = new AnchorFramePair()
            {
                frameA = Isometry.Identity,
                frameB = Isometry.Identity
            };
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RotationMotorNative
        {
            public Quaternion targetRotation;
            public AngularMotorDrive drive;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RotationVelocityMotorNative
        {
            public Vector3 axis;
            public float speed;
            public AngularMotorDrive drive;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TwistSwingMotorNative
        {
            public Quaternion targetRotation;
            public AngularMotorDrive twistDrive;
            public AngularMotorDrive swingDrive;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct D3AngularMotorNative
        {
            [FieldOffset(0)]
            public AngularMotorDriveMode mode;

            [FieldOffset(4)]
            public RotationMotorNative rotationMotor;

            [FieldOffset(4)]
            public RotationVelocityMotorNative rotationVelocityMotor;

            [FieldOffset(4)]
            public TwistSwingMotorNative twistSwingMotor;

        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AngularMotorNative
        {
            /// <summary>
            /// The target angle of the motor in rad. The motor will try to drive the angle to this value.
            /// </summary>
            public float targetAngle;
            /// <summary>
            /// The target angular speed the motor tries to achieve.
            /// </summary>
            public float targetSpeed;

            /// <summary>
            /// The drive cofniguration of the motor.
            /// </summary>
            public AngularMotorDrive drive;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LinearMotorNative
        {
            public float targetPosition;
            public float targetSpeed;
            public float maxForce;
            public SpringDamper spring;

            public static readonly LinearMotorNative Default = new LinearMotorNative()
            {
                targetPosition = 0f,
                targetSpeed = 0f,
                maxForce = 0f,
                spring = SpringDamper.Default,
            };
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct JointBuilder
        {
            public RigidbodyId bodyA { get; }
            public RigidbodyId bodyB { get; }
            public JointConfig config { get; }

            public JointBuilder(RigidbodyId bodyA, RigidbodyId bodyB, JointConfig config)
            {
                if (bodyA == bodyB)
                {
                    throw new System.ArgumentException("join body with itself is not allowed");
                }

                this.bodyA = bodyA;
                this.bodyB = bodyB;
                this.config = config;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct JointConfig
        {
            [MarshalAs(UnmanagedType.I1)]
            public bool active;
            [MarshalAs(UnmanagedType.I1)]
            public bool ignoreCollision;

            public AnchorFramePair anchorFramePair;
            public float breakForce;
            public float breakTorque;
            public TypedJointConfig type;

            public JointConfig(TypedJointConfig type)
            {
                this.type = type;
                active = true;
                ignoreCollision = true;
                anchorFramePair = AnchorFramePair.Identity;
                breakForce = float.PositiveInfinity;
                breakTorque = float.PositiveInfinity;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct TypedJointConfig
        {
            [FieldOffset(0)]
            public JointType type;

            [FieldOffset(4)]
            internal DistanceJointConfig _distance;

            [FieldOffset(4)]
            internal FixedJointConfig _fix;

            [FieldOffset(4)]
            internal HingeJointConfig _hinge;

            [FieldOffset(4)]
            internal BallJointConfig _ball;

            [FieldOffset(4)]
            internal SliderJointConfig _slider;

            [FieldOffset(4)]
            internal UniversalJointConfig _universal;

            [FieldOffset(4)]
            internal D6JointConfig _d6;

            [FieldOffset(4)]
            internal EllipsoidJointConfig _ellipsoid;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct DistanceJointConfig
        {
            public SpringDamper limitSpring;
            public MinMax distanceLimit;
            public LinearMotorNative motor;
            public float linearDamper;
            public float angularDamper;

            public DistanceJointConfig(MinMax limit)
            {
                distanceLimit = limit;
                limitSpring = SpringDamper.Default;
                motor = LinearMotorNative.Default;
                linearDamper = 0;
                angularDamper = 0;
            }

            public static implicit operator TypedJointConfig(DistanceJointConfig config)
            {
                return new TypedJointConfig() { type = JointType.Distance, _distance = config };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct FixedJointConfig
        {
            private readonly uint _padding;

            public static implicit operator TypedJointConfig(FixedJointConfig config)
            {
                return new TypedJointConfig() { type = JointType.Fixed, _fix = config };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HingeJointConfig
        {
            public float angleLimitLower;
            public float angleLimitHigher;
            public SpringDamper limitSpring;

            public AngularMotorNative motorDrive;

            public float angularDamper;

            public static implicit operator TypedJointConfig(HingeJointConfig config)
            {
                return new TypedJointConfig() { type = JointType.Hinge, _hinge = config };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct BallJointConfig
        {
            public MinMax twistLimit;
            public MinMax swingLimit;
            public SpringDamper limitSpring;
            public D3AngularMotorNative motor;
            public float angularDamper;

            public static implicit operator TypedJointConfig(BallJointConfig config)
            {
                return new TypedJointConfig() { type = JointType.Ball, _ball = config };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct EllipsoidJointConfig
        {
            public MinMax twistLimit;
            public MinMax swingLimitY;
            public MinMax swingLimitZ;
            public SpringDamper limitSpring;
            public D3AngularMotorNative motor;
            public float angularDamper;

            public static implicit operator TypedJointConfig(EllipsoidJointConfig config)
            {
                return new TypedJointConfig() { type = JointType.Ellipsoid, _ellipsoid = config };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SliderJointConfig
        {
            public SpringDamper limitSpring;
            public MinMax distanceLimit;
            public float linearDamper;
            public LinearMotorNative motorDrive;

            public SliderJointConfig(MinMax distanceLimit)
            {
                this.distanceLimit = distanceLimit;
                limitSpring = SpringDamper.Default;
                motorDrive = LinearMotorNative.Default;
                linearDamper = 0;
            }

            public static implicit operator TypedJointConfig(SliderJointConfig builder)
            {
                return new TypedJointConfig() { type = JointType.Slider, _slider = builder };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct UniversalJointConfig
        {
            public float swingAxisLimitZLow;
            public float swingAxisLimitZHigh;
            public float swingAxisLimitYLow;
            public float swingAxisLimitYHigh;
            public float angularDamper;

            public static implicit operator TypedJointConfig(UniversalJointConfig builder)
            {
                return new TypedJointConfig() { type = JointType.Universal, _universal = builder };
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct D6JointConfig
        {
            public D6Motion degreeMotion;
            public float xLimitLow;
            public float xLimitHigh;
            public float yLimitLow;
            public float yLimitHigh;
            public float zLimitLow;
            public float zLimitHigh;
            public MinMax twistLimit;
            public MinMax swingLimitY;
            public MinMax swingLimitZ;
            public D3AngularMotorNative angularMotor;
            public SpringDamper linearSpringDamper;
            public SpringDamper angularSpringDamper;
            public float linearDamper;
            public float angularDamper;

            public static implicit operator TypedJointConfig(D6JointConfig config)
            {
                return new TypedJointConfig() { type = JointType.D6, _d6 = config };
            }
        }
    }
}
