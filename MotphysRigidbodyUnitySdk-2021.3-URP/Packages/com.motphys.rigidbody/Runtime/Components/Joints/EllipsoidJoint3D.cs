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
using UnityEngine.Serialization;

namespace Motphys.Rigidbody
{
    [AddComponentMenu("Motphys/Joints/EllipsoidJoint3D")]
    public class EllipsoidJoint3D : BaseJoint
    {
        [SerializeField]
        private bool _useMotor = false;
        [SerializeField]
        private bool _useSpring = false;

        [SerializeField]
        [Tooltip("Used to limit the twist angle. The twist axis is defined by the anchor frame's x axis.")]
        private JointAngularLimit _twistLimit = JointAngularLimit.Default;

        [SerializeField]
        [Tooltip("Used to limit the swing angle. It is defined by the anchor frame's y axis. Low value must be in [-180,0], and high value must be in [0,180] for swing limit of ellipsoid joint.")]
        private JointAngularLimit _swingLimitY = JointAngularLimit.Default;

        [SerializeField]
        [Tooltip("Used to limit the swing angle. It is defined by the anchor frame's z axis. Low value must be in [-180,0], and high value must be in [0,180] for swing limit of ellipsoid joint.")]
        private JointAngularLimit _swingLimitZ = JointAngularLimit.Default;

        [SerializeField]
        [FormerlySerializedAs("_angularSpringDamper")]
        [Tooltip("The spring used to limit the twist and swing angles.")]
        private SpringDamper _limitSpring = SpringDamper.Default;

        [SerializeField]
        [Tooltip("The motor used to drive the twist and swing angles.")]
        private D3AngularMotor _motor = D3AngularMotor.Default;

        [SerializeField]
        private float _angularDamper;
        internal override bool TryCreateTypedJointConfig(out TypedJointConfig typedConfig)
        {
            var defaultMotor = new D3AngularMotor();
            typedConfig = new EllipsoidJointConfig()
            {
                twistLimit = _twistLimit.ToMinMaxRad(),
                swingLimitY = _swingLimitY.ToMinMaxRad(),
                swingLimitZ = _swingLimitZ.ToMinMaxRad(),
                limitSpring = _useSpring ? _limitSpring : SpringDamper.InfiniteStiffness,
                motor = _useMotor ? _motor.IntoNative() : defaultMotor.IntoNative(),
                angularDamper = _angularDamper,
            };
            return true;
        }

        /// <value>
        /// The spring used to limit the twist and swing angles.
        /// </value>
        public SpringDamper limitSpring
        {
            get
            {
                return _limitSpring;
            }
            set
            {
                _limitSpring = value;
                SetDirty();
            }
        }

        /// <value>
        /// The motor used to drive the twist and swing angles.
        /// </value>
        public D3AngularMotor motor
        {
            get
            {
                return _motor;
            }
            set
            {
                _motor = value;
                SetDirty();
#if UNITY_EDITOR
                UpdateEditor(value);
#endif
            }
        }

        /// <value>
        /// Whether to use a motor.
        /// </value>
        public bool useMotor
        {
            get
            {
                return _useMotor;
            }
            set
            {
                if (_useMotor == value)
                {
                    return;
                }

                _useMotor = value;
                SetDirty();
            }
        }

        /// <value>
        /// Whether this joint uses a spring.
        /// If false, the spring will be infinity stiffness and 0 damper.
        /// </value>
        public bool useSpring
        {
            get
            {
                return _useSpring;
            }
            set
            {
                if (_useSpring == value)
                {
                    return;
                }

                _useSpring = value;
                SetDirty();
            }
        }

        /// <value>
        /// Angular damper, higher value means greater deceleration.
        /// </value>
        public float angularDamper
        {
            get { return _angularDamper; }
            set
            {
                if (_angularDamper == value)
                {
                    return;
                }

                _angularDamper = value;
                SetDirty();
            }
        }

        /// <value>
        /// Used to limit the twist angle. The twist axis is defined by the anchor frame's x axis.
        /// </value>
        public JointAngularLimit twistLimit
        {
            get
            {
                return _twistLimit;
            }
            set
            {
                _twistLimit = value;
                SetDirty();
            }
        }

        /// <value>
        /// Used to limit the swing angle. It is defined by the anchor frame's y axis.
        /// Low value must be in [-180,0], and high value must be in [0,180] for swing limit of ellipsoid joint.
        /// </value>
        public JointAngularLimit swingLimitY
        {
            get
            {
                return _swingLimitY;
            }
            set
            {
                var low = value.low;
                var high = value.high;
                if (-180 <= low && low <= 0 && 0 <= high && high <= 180)
                {
                    _swingLimitY = value;
                    SetDirty();
                }
                else
                {
                    throw new System.ArgumentException($"Low value must be in [-180,0], and high value must be in [0,180] for swing limit of ellipsoid joint. (low = {low}, high = {high})");
                }
            }
        }

        /// <value>
        /// Used to limit the swing angle. It is defined by the anchor frame's z axis.
        /// Low value must be in [-180,0], and high value must be in [0,180] for swing limit of ellipsoid joint.
        /// </value>
        public JointAngularLimit swingLimitZ
        {
            get
            {
                return _swingLimitZ;
            }
            set
            {
                var low = value.low;
                var high = value.high;
                if (-180 <= low && low <= 0 && 0 <= high && high <= 180)
                {
                    _swingLimitZ = value;
                    SetDirty();
                }
                else
                {
                    throw new System.ArgumentException($"Low value must be in [-180,0], and high value must be in [0,180] for swing limit of ellipsoid joint. (low = {low}, high = {high})");
                }
            }
        }

#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        internal D3AngularMotorPack _d3AngularMotorPack = new D3AngularMotorPack();

        private void UpdateEditor(D3AngularMotor value)
        {
            switch (value.type)
            {
                case MotorType.Position:
                    _d3AngularMotorPack._rotationMotor = value;
                    break;
                case MotorType.Speed:
                    _d3AngularMotorPack._velocityMotor = value;
                    break;
            }
        }

        protected override void OnValidate()
        {
            _twistLimit.Validate();
            _swingLimitY.ClampTo180NP();
            _swingLimitZ.ClampTo180NP();
            base.OnValidate();
        }
#endif
    }
}
