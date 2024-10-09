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

using UnityEngine;

namespace Motphys.Rigidbody
{
    /// <summary>
    /// The motor's driving mode
    /// </summary>
    public enum MotorType
    {
        /// <summary>
        /// The motor drives to reach a specific position or rotation.
        /// </summary>
        Position,
        /// <summary>
        /// The motor drives to reach a specific speed or velocity.
        /// </summary>
        Speed,
    }

    /// <summary>
    /// Used for configuring the linear motor drive of a joint.
    /// </summary>
    [System.Serializable]
    public struct LinearMotor
    {
        /// <value>
        /// The target position the motor tries to achieve.
        /// </value>
        [Tooltip("The target position the motor tries to achieve")]
        public float targetPosition;

        /// <value>
        /// The target linear speed the motor tries to achieve.
        /// </value>
        [Tooltip("The target linear speed the motor tries to achieve")]
        public float targetSpeed;

        /// <value>
        /// The type of motor, either driven by position or by velocity
        /// </value>
        [Tooltip("The type of motor, either driven by position or by velocity")]
        public MotorType type;

        /// <value>
        /// The spring used to drive the motor.
        /// </value>
        [Tooltip("The spring used to drive the motor")]
        public SpringDamper spring;

        /// <value>
        /// The maximum force the motor can apply to achieve the target position and target speed.
        ///
        /// <para> Non-positive value means disable the motor.</para>
        /// </value>
        [Tooltip("The maximum force the motor can apply to achieve the target position and target speed")]
        public float maxForce;

        internal Internal.LinearMotorNative IntoNative(bool use)
        {
            return new Internal.LinearMotorNative()
            {
                targetPosition = targetPosition,
                targetSpeed = targetSpeed,
                spring = spring,
                maxForce = use ? maxForce : 0,
            };
        }

        /// <value>
        /// The default configuration of the linear motor.
        ///
        /// <para>Note: The use property is false by default </para>
        /// </value>
        public static readonly LinearMotor Default = new LinearMotor()
        {
            targetPosition = 0,
            targetSpeed = 0,
            spring = SpringDamper.Default,
            maxForce = float.MaxValue,
        };

        /// <summary>
        /// Create a motor with a MotorType.Position.
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="spring"></param>
        /// <returns></returns>
        public static LinearMotor TargetPositionMotor(float targetPosition, SpringDamper spring)
        {
            return new LinearMotor()
            {
                type = MotorType.Position,
                targetPosition = targetPosition,
                spring = spring,
                maxForce = float.MaxValue,
            };
        }

        /// <summary>
        /// Create a motor with a MotorType.Speed. 
        /// </summary>
        /// <param name="targetSpeed"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        public static LinearMotor TargetSpeedMotor(float targetSpeed, float strength)
        {
            return new LinearMotor()
            {
                type = MotorType.Speed,
                targetSpeed = targetSpeed,
                spring = new SpringDamper() { stiffness = 0, damper = strength },
                maxForce = float.MaxValue,
            };
        }
    }

    /// <summary>
    /// An angular motor which has three rotional degrees of freedom.
    /// </summary>
    [System.Serializable]
    public struct D3AngularMotor
    {
        /// <value>
        /// The drive mode of the motor.
        /// </value>
        [Tooltip("The drive mode of the motor")]
        public AngularMotorDriveMode mode;

        /// <value>
        /// The type of motor, either driven by position or by velocity
        /// </value>
        [Tooltip("The type of motor, either driven by rotation or by velocity")]
        public MotorType type;

        /// <value>
        /// It is defined as the target anchor rotation of the connected body in first body's local anchor space.
        /// </value>
        [Tooltip("The target anchor rotation of the connected body in first body's local anchor space")]
        public Quaternion targetRotation;

        /// <value>
        /// The target relative angular velocity of the connected body in first body's local anchor space.
        /// </value>
        [Tooltip("The target relative angular velocity of the connected body in first body's local anchor space")]
        public Vector3 targetAngularVelocity;

        /// <value>
        /// The drive used in SLerp and Velocity mode.
        /// </value>
        [Tooltip("The drive used in SLerp mode")]
        public AngularMotorDrive slerpDrive;

        /// <value>
        /// The drive used in twist-swing mode for twist.
        /// </value>
        [Tooltip("The drive used in twist-swing mode for twist")]
        public AngularMotorDrive twistDrive;

        /// <value>
        /// The drive used in twist-swing mode for swing.
        /// </value>
        [Tooltip("The drive used in twist-swing mode for swing")]
        public AngularMotorDrive swingDrive;

        internal Internal.D3AngularMotorNative IntoNative()
        {
            var axis = targetAngularVelocity.normalized;
            var speed = targetAngularVelocity.magnitude;
            if (axis.Equals(Vector3.zero))
            {
                axis = new Vector3(1, 0, 0);
                speed = 0f;
            }

            return mode switch
            {
                AngularMotorDriveMode.SLerp => new Internal.D3AngularMotorNative()
                {
                    mode = AngularMotorDriveMode.SLerp,
                    rotationMotor = new Internal.RotationMotorNative()
                    {
                        targetRotation = targetRotation,
                        drive = slerpDrive,
                    }
                },
                AngularMotorDriveMode.Velocity => new Internal.D3AngularMotorNative()
                {
                    mode = AngularMotorDriveMode.Velocity,
                    rotationVelocityMotor = new Internal.RotationVelocityMotorNative()
                    {
                        axis = axis,
                        speed = speed,
                        drive = slerpDrive,
                    }
                },
                AngularMotorDriveMode.TwistSwing => new Internal.D3AngularMotorNative()
                {
                    mode = AngularMotorDriveMode.TwistSwing,
                    twistSwingMotor = new Internal.TwistSwingMotorNative()
                    {
                        targetRotation = targetRotation,
                        twistDrive = twistDrive,
                        swingDrive = swingDrive,
                    },
                },
                _ => throw new System.NotImplementedException(),
            };
        }

        /// <summary>
        /// Create a target rotation motor in slerp mode.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="targetRotation"></param>
        /// <param name="spring"></param>
        /// <returns></returns>
        public static D3AngularMotor SlerpRotationMotor(Quaternion targetRotation, SpringDamper spring)
        {
            var motor = D3AngularMotor.Default;
            motor.targetRotation = targetRotation;
            motor.type = MotorType.Position;
            motor.mode = AngularMotorDriveMode.SLerp;
            motor.slerpDrive.spring = spring;

            return motor;
        }

        /// <summary>
        /// Create a target rotation motor in twist-swing mode.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="targetRotation"></param>
        /// <param name="spring"></param>
        /// <returns></returns>
        public static D3AngularMotor TwistSwingRotationMotor(Quaternion targetRotation, SpringDamper twistSpring, SpringDamper swingSpring)
        {
            var motor = D3AngularMotor.Default;
            motor.targetRotation = targetRotation;
            motor.type = MotorType.Position;
            motor.mode = AngularMotorDriveMode.TwistSwing;
            motor.twistDrive.spring = twistSpring;
            motor.swingDrive.spring = swingSpring;

            return motor;
        }

        /// <summary>
        /// Create a target angular velocity motor.
        /// </summary>
        /// <param name="targetVelocity"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        public static D3AngularMotor TargetVelocityMotor(Vector3 targetVelocity, float strength)
        {
            var motor = D3AngularMotor.Default;
            motor.mode = AngularMotorDriveMode.Velocity;
            motor.type = MotorType.Speed;
            motor.targetAngularVelocity = targetVelocity;
            motor.slerpDrive.spring = new SpringDamper() { damper = strength };

            return motor;
        }

        /// <value>
        /// Get the default configuration of the D3AngularMotor.
        /// </value>
        public static readonly D3AngularMotor Default = new D3AngularMotor()
        {
            mode = AngularMotorDriveMode.SLerp,
            targetRotation = Quaternion.identity,
            targetAngularVelocity = Vector3.zero,
            slerpDrive = AngularMotorDrive.Default,
            twistDrive = AngularMotorDrive.Default,
            swingDrive = AngularMotorDrive.Default,
        };
    }

    /// <summary>
    /// Used for configuring the angular motor drive of a joint in one-dimensional mode.
    /// </summary>
    [System.Serializable]
    public struct AngularMotor
    {
        /// <value>
        /// The target drive angle in degrees.
        /// </value>
        [Tooltip("The target drive angle in degrees.")]
        public float targetAngle;
        /// <value>
        /// The target angular speed the motor tries to achieve.
        /// <para> Unit is rad/s </para>
        /// </value>
        [Tooltip("The target angular speed the motor tries to achieve. Unit is rad/s")]
        public float targetSpeed;

        /// <value>
        /// The type of motor, either driven by rotation or by velocity
        /// </value>
        [Tooltip("The type of motor, either driven by rotation or by velocity")]
        public MotorType type;

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

        /// <summary>
        /// Create a motor with a MotorType.Speed.
        /// </summary>
        /// <param name="targetSpeed"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        public static AngularMotor TargetSpeedMotor(float targetSpeed, float strength)
        {
            return new AngularMotor()
            {
                type = MotorType.Speed,
                targetSpeed = targetSpeed,
                spring = new SpringDamper() { stiffness = 0, damper = strength },
                maxTorque = float.MaxValue,
            };
        }

        /// <summary>
        /// Create a motor with a MotorType.Position.
        /// </summary>
        /// <param name="targetAngle"></param>
        /// <param name="spring"></param>
        /// <returns></returns>
        public static AngularMotor TargetAngleMotor(float targetAngle, SpringDamper spring)
        {
            return new AngularMotor()
            {
                type = MotorType.Position,
                targetAngle = targetAngle,
                spring = spring,
                maxTorque = float.MaxValue,
            };
        }

        internal Internal.AngularMotorNative IntoNative(bool use)
        {
            return new Internal.AngularMotorNative()
            {
                targetAngle = targetAngle * Mathf.Deg2Rad,
                targetSpeed = targetSpeed,
                drive = new AngularMotorDrive()
                {
                    spring = spring,
                    maxTorque = use ? maxTorque : 0,
                }
            };
        }

        /// <value>
        /// The default configuration of the angular motor.
        /// </value>
        public static readonly AngularMotor Default = new AngularMotor()
        {
            targetAngle = 0,
            targetSpeed = 0,
            spring = SpringDamper.Default,
            maxTorque = float.PositiveInfinity,
        };
    }
}
