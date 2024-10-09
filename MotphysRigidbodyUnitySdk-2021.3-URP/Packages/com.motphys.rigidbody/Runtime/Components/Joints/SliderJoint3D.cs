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

namespace Motphys.Rigidbody
{
    [AddComponentMenu("Motphys/Joints/SliderJoint3D")]
    public class SliderJoint3D : BaseJoint
    {
        [SerializeField]
        private bool _useMotor = false;
        [SerializeField]
        private bool _useSpring = false;

        [Tooltip("If enabled, the distance between the two bodies will be restricted to the range [minDistance, maxDistance].")]
        [SerializeField]
        private bool _useLimit = false;

        [Tooltip("The spring used to restrict bodies in limit range")]
        [SerializeField]
        private SpringDamper _limitSpring = SpringDamper.Default;

        [Tooltip("The slide distance limit. Can be negative.")]
        [SerializeField]
        private JointTranslationLimit _distanceLimit;

        [Tooltip("Configure the motor drive of the joint")]
        [SerializeField]
        private LinearMotor _motor = LinearMotor.Default;

        [SerializeField]
        private float _linearDamper;

        /// <value>
        /// If enabled, the distance between the two bodies will be restricted to the range [minDistance, maxDistance].
        /// </value>
        public bool useLimit
        {
            get
            {
                return _useLimit;
            }
            set
            {
                if (_useLimit == value)
                {
                    return;
                }

                _useLimit = value;
                SetDirty();
            }
        }

        /// <value>
        /// Whether this joint uses a spring.
        /// If false, the spring will be infinity stiffness and 0 damper.
        /// </value>
        public bool useSpring
        {
            get { return _useSpring; }
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
        /// The spring used to restrict bodies in limit range. Only works when useLimit is true.
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
        /// Full configuration of the motor drive.
        /// </value>
        public LinearMotor motor
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
        /// The magnitude of the current motor force applied to the joint.
        ///
        /// <para> The direction of the force is always along the slider axis. </para>
        /// </value>
        public float currentMotorForce
        {
            get
            {
                if (!hasNativeInitialized)
                {
                    return 0f;
                }

                return PhysicsManager.defaultWorld.GetJointCurrentMotorForces(id).x;
            }
        }

        /// <value>
        /// The distance limit along the slider axis. Can be negative.
        /// </value>
        public JointTranslationLimit distanceLimit
        {
            get
            {
                return _distanceLimit;
            }
            set
            {
                _distanceLimit = value;
                SetDirty();
            }
        }

        /// <value>
        /// Current distance between two rigidbodies.
        /// </value>
        public float currentDistance
        {
            get
            {
                if (!hasNativeInitialized)
                {
                    return 0;
                }

                if (!connectedBody)
                {
                    return 0;
                }

                var connectedPos = this._connectBody.position;
                var selfPos = this.body.position;
                var distance = Vector3.Distance(connectedPos, selfPos);
                return distance;
            }
        }

        internal override bool TryCreateTypedJointConfig(out TypedJointConfig config)
        {
            config = new SliderJointConfig()
            {
                distanceLimit = _useLimit ? new MinMax(_distanceLimit.low, _distanceLimit.high) : new MinMax(float.MinValue, float.MaxValue),
                limitSpring = _useSpring ? _limitSpring : SpringDamper.InfiniteStiffness,
                linearDamper = _linearDamper,
                motorDrive = _motor.IntoNative(_useMotor),
            };
            return true;
        }

#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        internal MotorType _motorType;
        [SerializeField]
        [HideInInspector]
        internal LinearMotorPack _motorPack = new LinearMotorPack();

        private void UpdateEditor(LinearMotor value)
        {
            switch (value.type)
            {
                case MotorType.Position:
                    Debug.Assert(value.targetSpeed == 0, "position mode target speed should be zero");
                    _motorPack._distanceMotor = value;
                    break;
                case MotorType.Speed:
                    Debug.Assert(value.targetPosition == 0, "velocity mode target angle should be zero");
                    _motorPack._speedMotor = value;
                    break;
            }
        }

        protected override void OnValidate()
        {
            _distanceLimit.Validate();
            base.OnValidate();
        }
#endif
    }
}
