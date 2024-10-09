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

namespace Motphys.Rigidbody.Demos
{
    public class RagdollMotorUpdate : MonoBehaviour
    {
        public EllipsoidJoint3D leftArm;
        public EllipsoidJoint3D rightArm;
        public EllipsoidJoint3D leftLeg;
        public EllipsoidJoint3D rightLeg;
        private D3AngularMotor _leftArmMotor;
        private D3AngularMotor _rightArmMotor;
        private D3AngularMotor _leftLegMotor;
        private D3AngularMotor _rightLegMotor;

        // a flipping index to set motor's target angle
        private int _index = 0;
        private int index => _index;
        private int prevIndex => 1 - _index;

        private float[] _angles = { -20f, 20f };
        private float _dt = 2f;

        void Start()
        {
            _leftArmMotor = leftArm.motor;
            _rightArmMotor = rightArm.motor;
            _leftLegMotor = leftLeg.motor;
            _rightLegMotor = rightLeg.motor;
        }

        private void FixedUpdate()
        {
            _dt += Time.fixedDeltaTime;
            if (_dt > 2f)
            {
                _index = 1 - _index;
                _leftArmMotor.targetRotation = Quaternion.AngleAxis(_angles[index], Vector3.up);
                _rightArmMotor.targetRotation = Quaternion.AngleAxis(_angles[prevIndex], Vector3.up);
                _leftLegMotor.targetRotation = Quaternion.AngleAxis(_angles[prevIndex], Vector3.up);
                _rightLegMotor.targetRotation = Quaternion.AngleAxis(_angles[index], Vector3.up);
                leftArm.motor = _leftArmMotor;
                rightArm.motor = _rightArmMotor;
                leftLeg.motor = _leftLegMotor;
                rightLeg.motor = _rightLegMotor;
                _dt -= 2f;
            }
        }
    }
}
