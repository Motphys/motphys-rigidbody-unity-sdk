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
    using Internal;

    [AddComponentMenu("Motphys/Joints/BallJoint3D")]
    public class BallJoint3D : BaseJoint
    {
        [SerializeField]
        private bool _useMotor = false;
        [SerializeField]
        private bool _useSpring = false;

        [SerializeField]
        private JointAngularLimit _twistLimit = JointAngularLimit.Default;

        [SerializeField]
        [Range(0, 180)]
        private float _swingLimit = 180;

        [SerializeField]
        private SpringDamper _limitSpring = SpringDamper.Default;

        [SerializeField]
        private D3AngularMotor _motor = D3AngularMotor.Default;

        [SerializeField]
        private float _angularDamper;

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
        /// Angular damper, higher value means greater deceleration.
        /// </value>
        public float angularDamper
        {
            get
            {
                return _angularDamper;
            }
            set
            {
                if (value < 0)
                {
                    throw new System.ArgumentOutOfRangeException("AngularDamper must be greater than 0");
                }

                _angularDamper = value;
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
                _useSpring = value;
                SetDirty();
            }
        }

        /// <value>
        /// The spring used to limit the twist and swing angles.
        /// </value>
        public SpringDamper limitSpring
        {
            get { return _limitSpring; }
            set
            {
                _limitSpring = value;
                SetDirty();
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
        /// The swing limit of y-axis, z-axis, with range [0,180]
        /// </value>
        public float swingLimit
        {
            get
            {
                return _swingLimit;
            }
            set
            {
                if (value < 0 || value > 180)
                {
                    throw new System.ArgumentOutOfRangeException("SwingLimit must be in [0, 180]");
                }

                _swingLimit = value;
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

        internal override bool TryCreateTypedJointConfig(out TypedJointConfig config)
        {
            var defaultMotor = new D3AngularMotor();
            config = new BallJointConfig()
            {
                twistLimit = _twistLimit.ToMinMaxRad(),
                swingLimit = new JointAngularLimit(-_swingLimit, _swingLimit).ToMinMaxRad(),
                limitSpring = _useSpring ? _limitSpring : SpringDamper.InfiniteStiffness,
                angularDamper = _angularDamper,
                motor = _useMotor ? _motor.IntoNative() : defaultMotor.IntoNative(),
            };
            return true;
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
            base.OnValidate();
        }
#endif
    }
}
