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
    [AddComponentMenu("Motphys/Joints/DistanceJoint3D")]
    public class DistanceJoint3D : BaseJoint
    {
        [SerializeField]
        private bool _useLimit = true;

        [SerializeField]
        private bool _useMotor = false;

        [SerializeField]
        private bool _useSpring = false;

        [SerializeField]
        [Min(0)]
        private float _minDistance = 0f;

        [SerializeField]
        [Min(0)]
        private float _maxDistance = 0f;

        [SerializeField]
        private SpringDamper _limitSpring = SpringDamper.Default;

        [SerializeField]
        private LinearMotor _motor = LinearMotor.Default;

        [SerializeField]
        private float _linearDamper;

        [SerializeField]
        private float _angularDamper;

        /// <value>
        /// The spring used to limit the distance.
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
        /// If false, the default limit is [0,0].
        /// </value>
        public bool useLimit
        {
            get { return _useLimit; }
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
        /// The motor this joint uses.
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

        /// <summary>
        /// Set the distance limit of the joint.
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown when min > max</exception>
        ///
        public void SetDistanceLimit(float min, float max)
        {
            if (min > max || min < 0 || max < 0)
            {
                throw new System.ArgumentException($"min should greater than 0 and less than max (min = {min}, max = {max})");
            }

            _minDistance = min;
            _maxDistance = max;
            SetDirty();
        }

        /// <value>
        /// The min distance limit of the distance joint. It must be greater than 0.
        /// </value>
        public float minDistance
        {
            get
            {
                return _minDistance;
            }
            set
            {
                var min = value;
                var max = _maxDistance;
                if (min > max || min < 0)
                {
                    throw new System.ArgumentOutOfRangeException($"min should greater than 0 and less than max (min = {min}, max = {max})");
                }

                _minDistance = min;
                SetDirty();
            }
        }

        /// <value>
        /// The max distance limit of the distance joint. It must be greater than 0.
        /// </value>
        public float maxDistance
        {
            get
            {
                return _maxDistance;
            }
            set
            {
                var min = _minDistance;
                var max = value;
                if (min > max || max < 0)
                {
                    throw new System.ArgumentOutOfRangeException($"min should greater than 0 and less than max (min = {min}, max = {max})");
                }

                _maxDistance = max;
                SetDirty();
            }
        }

        /// <value>
        /// Current distance between two rigidbodies
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

        private void ValidateDistance()
        {
            _minDistance = Mathf.Min(_minDistance, _maxDistance);
        }

        internal override bool TryCreateTypedJointConfig(out TypedJointConfig config)
        {
            var minMax = _useLimit ? new MinMax(_minDistance, _maxDistance) : new MinMax(0, 0);
            config = new DistanceJointConfig(minMax)
            {
                limitSpring = _useSpring ? _limitSpring : SpringDamper.InfiniteStiffness,
                motor = _motor.IntoNative(_useMotor),
                linearDamper = _linearDamper,
                angularDamper = _angularDamper,
            };
            return true;
        }

        protected override void Awake()
        {
            autoConfigureConnected = false;
            base.Awake();
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

        [UnityEngine.TestTools.ExcludeFromCoverage]
        private void Reset()
        {
            autoConfigureConnected = false;
        }

        private void ValidateMotor()
        {
            _motor.targetPosition = Mathf.Max(_motor.targetPosition, 0f);
            _motor.spring.Validate();
        }

        protected override void OnValidate()
        {
            ValidateDistance();
            _limitSpring.Validate();
            ValidateMotor();
            base.OnValidate();
        }
#endif
    }
}
