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

    [AddComponentMenu("Motphys/Joints/UniversalJoint3D")]
    public class UniversalJoint3D : BaseJoint
    {
        [SerializeField]
        [Tooltip("Used to limit the swing angle. It is defined by the anchor frame's y axis. Valid range is in [-180, 180].")]
        private JointAngularLimit _swingLimitY = JointAngularLimit.Default;

        [SerializeField]
        [Tooltip("Used to limit the swing angle. It is defined by the anchor frame's z axis. Valid range is in [-180, 180].")]
        private JointAngularLimit _swingLimitZ = JointAngularLimit.Default;

        [SerializeField]
        private float _angularDamper;

        /// <value>
        /// Used to limit the swing angle. It is defined by the anchor frame's y axis.
        /// Valid range is in [-180, 180]. 
        /// </value>

        public JointAngularLimit swingLimitY
        {
            get
            {
                return _swingLimitY;
            }
            set
            {
                _swingLimitY = value;
                SetDirty();
            }
        }

        /// <value>
        /// Used to limit the swing angle. It is defined by the anchor frame's z axis.
        /// Valid range is in [-180, 180]. 
        /// </value>
        public JointAngularLimit swingLimitZ
        {
            get
            {
                return _swingLimitZ;
            }
            set
            {
                _swingLimitZ = value;
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
                _angularDamper = value;
                SetDirty();
            }
        }

        internal override bool TryCreateTypedJointConfig(out TypedJointConfig config)
        {
            config = new UniversalJointConfig()
            {
                swingAxisLimitZLow = _swingLimitZ.low * Mathf.Deg2Rad,
                swingAxisLimitZHigh = _swingLimitZ.high * Mathf.Deg2Rad,
                swingAxisLimitYLow = _swingLimitY.low * Mathf.Deg2Rad,
                swingAxisLimitYHigh = _swingLimitY.high * Mathf.Deg2Rad,
                angularDamper = _angularDamper,
            };
            return true;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            _swingLimitY.ClampTo180NP();
            _swingLimitZ.ClampTo180NP();
        }
#endif
    }
}
