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
    [AddComponentMenu("Motphys/Joints/HingeJoint3D")]
    public class HingeJoint3D : BaseJoint
    {
        [SerializeField]
        private bool _useLimit = false;
        [SerializeField]
        private bool _useMotor = false;
        [SerializeField]
        private bool _useSpring = false;

        [SerializeField]
        [Tooltip("The angle limit range of the hinge joint. Valid range is [-180,180].")]
        private JointAngularLimit _angleLimit = JointAngularLimit.ZERO;

        [SerializeField]
        private SpringDamper _limitSpring = SpringDamper.Default;

        [SerializeField]
        private float _angularDamper;

        [SerializeField]
        private AngularMotor _motor = AngularMotor.Default;

        internal override bool TryCreateTypedJointConfig(out TypedJointConfig config)
        {
            config = new HingeJointConfig()
            {
                angleLimitLower = (_useLimit ? _angleLimit.low : -180) * Mathf.Deg2Rad,
                angleLimitHigher = (_useLimit ? _angleLimit.high : 180) * Mathf.Deg2Rad,
                limitSpring = _useSpring ? _limitSpring : SpringDamper.InfiniteStiffness,
                angularDamper = _angularDamper,
                motorDrive = _motor.IntoNative(_useMotor),
            };
            return true;
        }

        /// <value>
        /// The spring used to limit the rotation.
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
        /// Whether enable the limit constraint of the hinge joint.
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
        /// The angle of the hinge joint in degrees.
        /// </value>
        public float angle
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

                var axis = transform.TransformDirection(anchorFrame.axisX);
                var va = transform.TransformDirection(anchorFrame.axisY);
                var vb = connectedBody.transform.TransformDirection(connectedAnchorFrame.axisY);
                return Vector3.SignedAngle(va, vb, axis);
            }
        }

        /// <value>
        /// The configuration of the motor drive.
        /// </value>
        public AngularMotor motor
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
        /// The magnitude of the current motor torque applied to the joint.
        ///
        /// The direction of the torque is always along the hinge axis.
        /// </value>
        public float currentMotorTorque
        {
            get
            {
                if (!hasNativeInitialized)
                {
                    return 0f;
                }

                return PhysicsManager.defaultWorld.GetJointCurrentMotorTorques(id).x;
            }
        }

        /// <value>
        /// The angle limit range of the hinge joint. Only works when <see cref="useLimit"/> is true.
        /// Valid range is [-180,180].
        /// </value>
        public JointAngularLimit angularLimit
        {
            get
            {
                return _angleLimit;
            }
            set
            {
                _angleLimit = value;
                SetDirty();
            }
        }

#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        internal AngularMotor _rotationMotor = AngularMotor.Default;
        [SerializeField]
        [HideInInspector]
        internal AngularMotor _velocityMotor = AngularMotor.Default;
        [SerializeField]
        [HideInInspector]
        internal MotorType _motorType;

        private void UpdateEditor(AngularMotor value)
        {
            switch (value.type)
            {
                case MotorType.Position:
                    Debug.Assert(value.targetSpeed == 0, "position mode target speed should be zero");
                    _rotationMotor = value;
                    break;
                case MotorType.Speed:
                    Debug.Assert(value.targetAngle == 0, "velocity mode target angle should be zero");
                    _velocityMotor = value;
                    break;
            }
        }

        protected override void OnValidate()
        {
            _angleLimit.Validate();
            base.OnValidate();
        }
#endif
    }
}
