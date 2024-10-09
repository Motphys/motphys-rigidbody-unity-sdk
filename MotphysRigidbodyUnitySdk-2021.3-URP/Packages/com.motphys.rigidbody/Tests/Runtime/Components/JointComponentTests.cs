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

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace Motphys.Rigidbody.Tests
{

    internal class JointComponentTests : ComponentTestScene
    {

        [UnitySetUp]
        protected override IEnumerator SetUp()
        {
            yield return base.SetUp();
            PhysicsManager.numSubstep = 5;
            PhysicsManager.defaultSolverIterations = 1;
        }

        [Test]
        public void TestAutoConfigureConnected()
        {
            var bodyA = CreateRigidBody(new Vector3(-1, 0, 0));
            bodyA.gameObject.AddComponent<BoxCollider3D>();
            var bodyB = CreateRigidBody(new Vector3(1, 0, 0));
            bodyB.gameObject.AddComponent<BoxCollider3D>();
            var joint = bodyA.gameObject.AddComponent<FixedJoint3D>();

            // set to false again, nothing should happen...
            joint.autoConfigureConnected = false;
            Assert.That(!joint.autoConfigureConnected);

            // change local anchor position.
            joint.anchorPosition = new Vector3(-0.5f, 0, 0);
            Assert.That(joint.worldAnchorPosition, Is.EqualTo(new Vector3(-1.5f, 0f, 0f)));

            // test no conected body
            joint.connectedAnchorPosition = new Vector3(0.5f, 0f, 0f);
            // if no conencted body assigned, local is equal to world
            Assert.That(joint.worldConnectedAnchorPosition, Is.EqualTo(joint.connectedAnchorPosition));
            Assert.That(joint.worldConnectedAnchorRotation, Is.EqualTo(joint.connectedAnchorFrame.rotation));

            // test autoConfigureConnected
            joint.connectedBody = bodyB;

            joint.autoConfigureConnected = true;
            Assert.That(joint.autoConfigureConnected);

            Assert.That(joint.worldConnectedAnchorPosition, Is.EqualTo(joint.worldAnchorPosition));
            Assert.That(joint.worldConnectedAnchorRotation, Is.EqualTo(joint.worldAnchorRotation));

            // if autoConfigureConnected is true, manually changing connected anchor do not work.
            var prevConnectedAnchorPosition = joint.connectedAnchorPosition;
            joint.connectedAnchorPosition = Vector3.one;
            Assert.That(joint.connectedAnchorPosition, Is.EqualTo(prevConnectedAnchorPosition));

            var prevConnectedAnchorFrame = joint.connectedAnchorFrame;
            joint.connectedAnchorFrame = new AxisFrame(Vector3.one, Vector3.zero);
            Assert.That(joint.connectedAnchorFrame, Is.EqualTo(prevConnectedAnchorFrame));

        }

        [Test]
        public void TestFixedJoint3D()
        {
            var bodyA = CreateRigidBody(-Vector3.one);
            bodyA.isKinematic = true;
            var bodyB = CreateRigidBody(Vector3.one);
            var joint = bodyA.gameObject.AddComponent<FixedJoint3D>();
            joint.autoConfigureConnected = false;
            joint.connectedBody = bodyB;
            PhysicsManager.Simulate(0.02f);
            Assert.That(Vector3.Distance(bodyA.transform.position, bodyB.transform.position), Is.EqualTo(0f).Using(new FloatEqualityComparer(1e-3f)));
        }

        [UnityTest]
        public IEnumerator TestDistanceJointLimit()
        {
            var bodyA = CreateRigidBody();
            bodyA.isKinematic = true;
            var bodyB = CreateRigidBody(new Vector3(0, 1, 0));
            bodyB.useGravity = false;
            bodyB.enablePostTransformControl = true;
            var joint = bodyA.gameObject.AddComponent<DistanceJoint3D>();

            Assert.That(joint.currentDistance == 0);

            joint.autoConfigureConnected = false;

            joint.useLimit = false;
            joint.useLimit = false;
            joint.useLimit = true;

            joint.motor = LinearMotor.Default;
            joint.motor = new LinearMotor() { spring = new SpringDamper() { compliance = 0.001f, damper = 20 } };
            joint.useMotor = true;
            joint.useMotor = false;
            joint.useMotor = false;

            Assert.That(joint.motor.spring.damper, Is.EqualTo(20.0f));

            Assert.That(joint.useLimit);

            // set limit to invalid range should cause exception
            {
                Assert.That(() =>
                {
                    joint.SetDistanceLimit(1.5f, 0.5f);
                }, Throws.ArgumentException);

                Assert.That(() =>
               {
                   joint.SetDistanceLimit(-0.5f, 0.5f);
               }, Throws.ArgumentException);
            }

            joint.maxDistance = 10;
            joint.minDistance = 2;
            Assert.That(joint.maxDistance, Is.EqualTo(10.0f));
            Assert.That(joint.minDistance, Is.EqualTo(2.0f));

            joint.SetDistanceLimit(0.5f, 1.5f);
            Assert.That(joint.minDistance, Is.EqualTo(0.5f));
            Assert.That(joint.maxDistance, Is.EqualTo(1.5f));

            joint.limitSpring = SpringDamper.InfiniteStiffness;
            Assert.That(joint.limitSpring, Is.EqualTo(SpringDamper.InfiniteStiffness));
            joint.connectedBody = bodyB;
            yield return new WaitForFixedUpdate();
            // body distance is in limit range, so the position should not change
            Assert.That(bodyB.transform.position, Is.EqualTo(new Vector3(0f, 1, 0f)).Using(Vector3EqualityComparer.Instance));

            //move body out of limit range
            bodyB.transform.position = new Vector3(0f, 1.6f, 0f);
            for (var i = 0; i < 5; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.That(bodyB.transform.position.y, Is.InRange(0.5f, 1.5f));
            Assert.That(joint.currentDistance > 0);
#if UNITY_EDITOR
            var newMotor = LinearMotor.TargetSpeedMotor(20, 1000);
            joint.motor = newMotor;

            Assert.That(joint.motor, Is.EqualTo(newMotor));
            Assert.That(joint._motorPack._speedMotor, Is.EqualTo(newMotor));

            newMotor = LinearMotor.TargetPositionMotor(12, SpringDamper.InfiniteStiffness);
            joint.motor = newMotor;

            Assert.That(joint.motor, Is.EqualTo(newMotor));
            Assert.That(joint._motorPack._distanceMotor, Is.EqualTo(newMotor));
#endif
        }

        [UnityTest]
        public IEnumerator TestHingeJoint3D()
        {
            var bodyA = CreateRigidBody();
            bodyA.isKinematic = true;
            var bodyB = CreateRigidBody();
            bodyB.gameObject.AddComponent<BoxCollider3D>();
            bodyB.useGravity = false;
            bodyB.enablePostTransformControl = true;
            bodyB.transform.rotation = Quaternion.AngleAxis(30, Vector3.up);

            var joint = bodyA.gameObject.AddComponent<HingeJoint3D>();
            // hinge joint without connected body should always have angle 0
            Assert.That(joint.angle, Is.EqualTo(0));
            joint.autoConfigureConnected = false;
            joint.connectedBody = bodyB;

            //use limit is false by default
            Assert.That(!joint.useLimit);

            // set use limit to false again, nothing should happen...
            joint.useLimit = false;
            Assert.That(!joint.useLimit);

            yield return new WaitForFixedUpdate();
            // hinge joint should align the x-axis of two bodies.

            var xAxisA = bodyA.transform.right;
            var xAxisB = bodyB.transform.right;

            Assert.That(Vector3.Dot(xAxisA, xAxisB), Is.EqualTo(1f).Using(new FloatEqualityComparer(1e-3f)));

            // revert the rotation of bodyB
            bodyB.transform.rotation = Quaternion.identity;
            yield return new WaitForFixedUpdate();

            // enable limit
            joint.useLimit = true;
            Assert.That(joint.useLimit);
            joint.limitSpring = SpringDamper.InfiniteStiffness;

            joint.angularLimit = new JointAngularLimit(10, 30);
            Assert.That(joint.angularLimit, Is.EqualTo(new JointAngularLimit(10, 30)));

            yield return new WaitForFixedUpdate();

            // then angle of hinge joint should be in range
            Assert.That(joint.angle, Is.InRange(10, 30));

            Assert.That(!joint.useMotor);
            joint.useMotor = true;
            yield return new WaitForFixedUpdate();
            joint.useMotor = true;
            yield return new WaitForFixedUpdate();

            joint.motor = AngularMotor.Default;
            Assert.That(joint.motor.targetAngle, Is.EqualTo(0));

            // after motphys-rigidbody #1971, motor axis from -X to X,
            // the torque from positive to negative
            var motorTorque = joint.currentMotorTorque;
            Assert.Less(motorTorque, 0);

#if UNITY_EDITOR
            var newMotor = AngularMotor.TargetSpeedMotor(12, 1000);
            joint.motor = newMotor;

            Assert.That(joint.motor, Is.EqualTo(newMotor));
            Assert.That(joint._velocityMotor, Is.EqualTo(newMotor));

            newMotor = AngularMotor.TargetAngleMotor(13, SpringDamper.InfiniteStiffness);
            joint.motor = newMotor;

            Assert.That(joint.motor, Is.EqualTo(newMotor));
            Assert.That(joint._rotationMotor, Is.EqualTo(newMotor));
#endif
        }

        [UnityTest]
        public IEnumerator TestSliderJoint()
        {
            var bodyA = CreateRigidBody();
            bodyA.isKinematic = true;
            var bodyB = CreateRigidBody();
            bodyB.gameObject.AddComponent<BoxCollider3D>();
            bodyB.enablePostTransformControl = true;
            //rotation 30 degree around y axis
            bodyB.transform.localRotation = Quaternion.AngleAxis(30, Vector3.up);
            bodyB.transform.localPosition = new Vector3(-2f, 2f, 3f);
            bodyB.useGravity = false;

            var joint = bodyA.gameObject.AddComponent<SliderJoint3D>();

            Assert.That(joint.currentDistance == 0);

            joint.autoConfigureConnected = false;
            joint.connectedBody = bodyB;
            //by default, use limit is false;
            Assert.That(!joint.useLimit);
            // set use limit to false again, nothing should happen...
            joint.useLimit = false;
            // enable use limit
            joint.distanceLimit = new JointTranslationLimit(-10, 20);
            Assert.That(joint.distanceLimit.high == 20);
            Assert.That(joint.distanceLimit.low == -10);

            joint.distanceLimit = new JointTranslationLimit(1f, 5f);

            Assert.That(joint.distanceLimit, Is.EqualTo(new JointTranslationLimit(1f, 5f)));
            joint.limitSpring = SpringDamper.InfiniteStiffness;
            Assert.That(joint.limitSpring, Is.EqualTo(SpringDamper.InfiniteStiffness));

            yield return new WaitForFixedUpdate();

            var position = bodyB.transform.position;
            var floatComparer = new FloatEqualityComparer(1e-6f);
            Assert.That(position.y, Is.EqualTo(0f).Using(floatComparer));
            Assert.That(position.z, Is.EqualTo(0f).Using(floatComparer));
            Assert.That(position.x, Is.EqualTo(-2f).Using(floatComparer));

            var rotation = bodyB.transform.rotation;
            Assert.That(rotation, Is.EqualTo(Quaternion.identity).Using(new QuaternionEqualityComparer(1e-6f)));

            joint.useLimit = true;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            position = bodyB.transform.position;
            Assert.That(position.x, Is.InRange(1f, 5f));

            // test motor
            joint.useLimit = false;
            var targetMotor = LinearMotor.TargetPositionMotor(10f, new SpringDamper(float.PositiveInfinity, 0f));
            var useMotor = joint.useMotor;
            Assert.That(useMotor, Is.EqualTo(false));
            joint.useMotor = true;
            joint.motor = targetMotor;
            joint.useMotor = true;
            Assert.That(joint.motor, Is.EqualTo(targetMotor));

            for (var i = 0; i < 4; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            AssertVector3Eq(bodyB.transform.position, new Vector3(10f, 0f, 0f), 1e-6f);
            Assert.That(joint.currentDistance > 2);

#if UNITY_EDITOR
            var newMotor = LinearMotor.TargetSpeedMotor(12, 1000);
            joint.motor = newMotor;

            Assert.That(joint.motor, Is.EqualTo(newMotor));
            Assert.That(joint._motorPack._speedMotor, Is.EqualTo(newMotor));

            newMotor = LinearMotor.TargetPositionMotor(13, SpringDamper.InfiniteStiffness);
            joint.motor = newMotor;

            Assert.That(joint.motor, Is.EqualTo(newMotor));
            Assert.That(joint._motorPack._distanceMotor, Is.EqualTo(newMotor));
#endif
        }

        [UnityTest]
        public IEnumerator TestSliderJointMotor()
        {
            var bodyA = CreateRigidBody(new Vector3(0, 2, 0));
            var bodyB = CreateRigidBody(new Vector3(3, 2, 0));
            bodyA.isKinematic = true;
            var joint = bodyA.gameObject.AddComponent<SliderJoint3D>();
            joint.useLimit = false;
            joint.useMotor = true;
            var motor = LinearMotor.TargetSpeedMotor(1f, 10f);
            motor.maxForce = float.PositiveInfinity;
            joint.motor = motor;

            joint.connectedBody = bodyB;

            for (var i = 0; i < 4; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            var force = joint.currentMotorForce;
            Assert.That(Mathf.Abs(force) > 0);
        }

        [UnityTest]
        public IEnumerator TestConfigurableJoint()
        {
#if MOTPHYS_RIGIDBODY_EXPERIMENTAL
            var bodyA = CreateRigidBody();
            bodyA.isKinematic = true;
            var bodyB = CreateRigidBody();
            bodyB.gameObject.AddComponent<BoxCollider3D>();
            bodyB.enablePostTransformControl = true;
            bodyB.useGravity = false;
            var joint = bodyA.gameObject.AddComponent<Experimental.ConfigurableJoint3D>();
            joint.autoConfigureConnected = false;
            Assert.That(joint.twistLimit, Is.EqualTo(JointAngularLimit.Default));
            Assert.That(joint.swingLimitY, Is.EqualTo(JointAngularLimit.Default));
            Assert.That(joint.swingLimitZ, Is.EqualTo(JointAngularLimit.Default));
            Assert.That(joint.angularMotor, Is.EqualTo(D3AngularMotor.Default));
            joint.translationMotionX = JointDegreeMotion.Limited;
            joint.translationMotionY = JointDegreeMotion.Limited;
            joint.translationMotionZ = JointDegreeMotion.Limited;
            joint.angularMotionX = JointDegreeMotion.Limited;
            joint.angularMotionY = JointDegreeMotion.Limited;
            joint.angularMotionZ = JointDegreeMotion.Limited;
            joint.connectedBody = bodyB;
            yield return new WaitForFixedUpdate();
            joint.translationLimitX = new JointTranslationLimit(0, 10);
            joint.translationLimitY = new JointTranslationLimit(0, 10);
            joint.translationLimitZ = new JointTranslationLimit(0, 10);
            joint.twistLimit = new JointAngularLimit(-10, 10);
            joint.swingLimitY = new JointAngularLimit(-30, 30);
            joint.swingLimitZ = new JointAngularLimit(-60, 60);
            joint.useSpring = false;
            var newAngularMotor = D3AngularMotor.SlerpRotationMotor(Quaternion.Euler(10, 40, 30), SpringDamper.InfiniteStiffness);
            joint.angularMotor = newAngularMotor;
            yield return new WaitForFixedUpdate();
            Assert.That(joint.translationMotionX, Is.EqualTo(JointDegreeMotion.Limited));
            Assert.That(joint.translationMotionY, Is.EqualTo(JointDegreeMotion.Limited));
            Assert.That(joint.translationMotionZ, Is.EqualTo(JointDegreeMotion.Limited));
            Assert.That(joint.angularMotionX, Is.EqualTo(JointDegreeMotion.Limited));
            Assert.That(joint.angularMotionY, Is.EqualTo(JointDegreeMotion.Limited));
            Assert.That(joint.angularMotionZ, Is.EqualTo(JointDegreeMotion.Limited));
            Assert.That(joint.translationLimitX, Is.EqualTo(new JointTranslationLimit(0, 10)));
            Assert.That(joint.translationLimitY, Is.EqualTo(new JointTranslationLimit(0, 10)));
            Assert.That(joint.translationLimitZ, Is.EqualTo(new JointTranslationLimit(0, 10)));
            Assert.That(joint.twistLimit, Is.EqualTo(new JointAngularLimit(-10, 10)));
            Assert.That(joint.swingLimitY, Is.EqualTo(new JointAngularLimit(-30, 30)));
            Assert.That(joint.swingLimitZ, Is.EqualTo(new JointAngularLimit(-60, 60)));
            Assert.That(joint.useSpring, Is.False);
            Assert.That(joint.angularMotor, Is.EqualTo(newAngularMotor));
            joint.targetRotation = Quaternion.Euler(10, 20, 30);
            Assert.That(joint.targetRotation, Is.EqualTo(Quaternion.Euler(10, 20, 30)));
            joint.useMotor = true;
            Assert.That(joint.useMotor, Is.EqualTo(true));
            joint.translationSpringDamper = new SpringDamper(0.5f, 0.5f);
            Assert.That(joint.translationSpringDamper, Is.EqualTo(new SpringDamper(0.5f, 0.5f)));
            joint.angularSpringDamper = new SpringDamper(0.5f, 0.5f);
            Assert.That(joint.angularSpringDamper, Is.EqualTo(new SpringDamper(0.5f, 0.5f)));
            joint.translationDamper = 0.5f;
            Assert.That(joint.translationDamper, Is.EqualTo(0.5f));
            joint.angularDamper = 0.5f;
            Assert.That(joint.angularDamper, Is.EqualTo(0.5f));
#if UNITY_EDITOR
            newAngularMotor = D3AngularMotor.TargetVelocityMotor(new Vector3(5, 5, 5), 100);
            joint.angularMotor = newAngularMotor;
            Assert.That(joint.angularMotor, Is.EqualTo(newAngularMotor));
            Assert.That(joint._d3AngularMotorPack._velocityMotor, Is.EqualTo(newAngularMotor));

            newAngularMotor = D3AngularMotor.TwistSwingRotationMotor(Quaternion.Euler(5, 7, 9), SpringDamper.InfiniteStiffness, SpringDamper.InfiniteStiffness);
            joint.angularMotor = newAngularMotor;
            yield return new WaitForFixedUpdate();

            Assert.That(joint.angularMotor, Is.EqualTo(newAngularMotor));
            Assert.That(joint._d3AngularMotorPack._rotationMotor, Is.EqualTo(newAngularMotor));
#endif
            joint.useSpring = true;
            yield return new WaitForFixedUpdate();
            Assert.That(joint.useSpring, Is.EqualTo(true));
#endif
            yield return new WaitForFixedUpdate();
        }

        [UnityTest]
        public IEnumerator TestEllipsoidJoint()
        {
            var bodyA = CreateRigidBody();
            bodyA.isKinematic = true;
            var bodyB = CreateRigidBody();
            bodyB.gameObject.AddComponent<BoxCollider3D>();
            bodyB.enablePostTransformControl = true;
            bodyB.useGravity = false;
            bodyB.position = new Vector3(1, 0, 0);
            var joint = bodyA.gameObject.AddComponent<EllipsoidJoint3D>();
            Assert.That(joint.twistLimit, Is.EqualTo(JointAngularLimit.Default));
            Assert.That(joint.swingLimitY, Is.EqualTo(JointAngularLimit.Default));
            Assert.That(joint.swingLimitZ, Is.EqualTo(JointAngularLimit.Default));

            Assert.That(joint.limitSpring, Is.EqualTo(SpringDamper.Default));
            Assert.That(joint.motor, Is.EqualTo(D3AngularMotor.Default));
            joint.connectedBody = bodyB;
            yield return new WaitForFixedUpdate();
            Assert.That(bodyB.position, Is.EqualTo(new Vector3(0, 0, 0)));
            joint.twistLimit = new JointAngularLimit(10, 30);
            joint.swingLimitY = new JointAngularLimit(-10, 30);
            joint.swingLimitZ = new JointAngularLimit(-20, 40);

            joint.limitSpring = SpringDamper.InfiniteStiffness;
            var newMotor = D3AngularMotor.SlerpRotationMotor(Quaternion.Euler(10, 20, 30), SpringDamper.InfiniteStiffness);
            joint.motor = newMotor;

            yield return new WaitForFixedUpdate();

            Assert.That(joint.twistLimit, Is.EqualTo(new JointAngularLimit(10, 30)));
            Assert.That(joint.swingLimitY, Is.EqualTo(new JointAngularLimit(-10, 30)));
            Assert.That(joint.swingLimitZ, Is.EqualTo(new JointAngularLimit(-20, 40)));

            Assert.That(joint.limitSpring, Is.EqualTo(SpringDamper.InfiniteStiffness));
            Assert.That(joint.motor, Is.EqualTo(newMotor));

            joint.useMotor = false;
            Assert.That(!joint.useMotor);
            joint.useMotor = true;
            Assert.That(joint.useMotor);

            newMotor = D3AngularMotor.TargetVelocityMotor(new Vector3(5, 5, 5), 100);
            var nativeVelocityMotor = newMotor.IntoNative();
            joint.motor = newMotor;
            yield return new WaitForFixedUpdate();
            Assert.That(joint.motor.type, Is.EqualTo(MotorType.Speed));
            Assert.That(nativeVelocityMotor.mode, Is.EqualTo(AngularMotorDriveMode.Velocity));

#if UNITY_EDITOR
            newMotor = D3AngularMotor.TargetVelocityMotor(new Vector3(5, 5, 5), 100);
            joint.motor = newMotor;
            Assert.That(joint.motor, Is.EqualTo(newMotor));
            Assert.That(joint._d3AngularMotorPack._velocityMotor, Is.EqualTo(newMotor));

            newMotor = D3AngularMotor.SlerpRotationMotor(Quaternion.Euler(5, 7, 9), SpringDamper.InfiniteStiffness);
            joint.motor = newMotor;
            Assert.That(joint.motor, Is.EqualTo(newMotor));
            Assert.That(joint._d3AngularMotorPack._rotationMotor, Is.EqualTo(newMotor));
#endif
        }

        [UnityTest]
        public IEnumerator TestBallJoint()
        {
            var bodyA = CreateRigidBody();
            bodyA.isKinematic = true;
            var bodyB = CreateRigidBody();
            bodyB.gameObject.AddComponent<BoxCollider3D>();
            bodyB.enablePostTransformControl = true;
            bodyB.useGravity = false;
            bodyB.position = new Vector3(1, 0, 0);
            var joint = bodyA.gameObject.AddComponent<BallJoint3D>();
            Assert.That(joint.twistLimit, Is.EqualTo(JointAngularLimit.Default));
            Assert.That(joint.swingLimit, Is.EqualTo(180));
            Assert.That(joint.motor, Is.EqualTo(D3AngularMotor.Default));

            joint.connectedBody = bodyB;
            yield return new WaitForFixedUpdate();
            Assert.That(bodyB.position, Is.EqualTo(new Vector3(0, 0, 0)));

            joint.twistLimit = new JointAngularLimit(5, 40);

            joint.swingLimit = 30;

            var newMotor = D3AngularMotor.SlerpRotationMotor(Quaternion.Euler(10, 20, 30), SpringDamper.InfiniteStiffness);

            joint.useMotor = false;
            joint.useMotor = false;
            joint.useMotor = true;

            joint.motor = newMotor;
            joint.angularDamper = 2.0f;

            yield return new WaitForFixedUpdate();

            Assert.That(joint.twistLimit, Is.EqualTo(new JointAngularLimit(5, 40)));
            Assert.That(joint.swingLimit, Is.EqualTo(30));
            Assert.That(joint.motor, Is.EqualTo(newMotor));
            Assert.That(joint.angularDamper, Is.EqualTo(2.0f));
#if UNITY_EDITOR
            newMotor.mode = AngularMotorDriveMode.TwistSwing;
            joint.motor = newMotor;
            yield return new WaitForFixedUpdate();

            Assert.That(joint.motor, Is.EqualTo(newMotor));
            Assert.That(joint._d3AngularMotorPack._rotationMotor, Is.EqualTo(newMotor));

            newMotor = D3AngularMotor.TargetVelocityMotor(new Vector3(5, 5, 5), 100);
            joint.motor = newMotor;
            Assert.That(joint.motor, Is.EqualTo(newMotor));
            Assert.That(joint._d3AngularMotorPack._velocityMotor, Is.EqualTo(newMotor));
#endif
        }

        [UnityTest]
        public IEnumerator TestUniversalJoint()
        {
            var bodyA = CreateRigidBody();
            bodyA.isKinematic = true;
            var bodyB = CreateRigidBody();
            bodyB.gameObject.AddComponent<BoxCollider3D>();
            bodyB.enablePostTransformControl = true;
            bodyB.useGravity = false;
            bodyB.position = new Vector3(1, 0, 0);
            var joint = bodyA.gameObject.AddComponent<UniversalJoint3D>();
            joint.angularDamper = 3;
            Assert.That(joint.angularDamper == 3);

            Assert.That(joint.swingLimitY.low, Is.EqualTo(-180));
            Assert.That(joint.swingLimitY.high, Is.EqualTo(180));
            Assert.That(joint.swingLimitZ.low, Is.EqualTo(-180));
            Assert.That(joint.swingLimitZ.high, Is.EqualTo(180));

            joint.connectedBody = bodyB;
            yield return new WaitForFixedUpdate();
            Assert.That(bodyB.position, Is.EqualTo(new Vector3(0, 0, 0)));

            joint.swingLimitY = new JointAngularLimit(-10, 30);
            joint.swingLimitZ = new JointAngularLimit(-30, 30);

            yield return new WaitForFixedUpdate();

            Assert.That(joint.swingLimitY, Is.EqualTo(new JointAngularLimit(-10, 30)));
            Assert.That(joint.swingLimitZ, Is.EqualTo(new JointAngularLimit(-30, 30)));
        }

        [Test]
        public void TestBaseJointProperty()
        {
            var bodyA = CreateRigidBody(Vector3.zero);
            var bodyB = CreateRigidBody(Vector3.one);
            var joint = bodyA.gameObject.AddComponent<FixedJoint3D>();
            joint.autoConfigureConnected = false;
            joint.connectedBody = bodyB;

            joint.ignoreCollision = true;
            Assert.IsTrue(joint.ignoreCollision);

            joint.ignoreCollision = false;
            Assert.IsTrue(!joint.ignoreCollision);

            var worldAnchorPosition = joint.worldAnchorPosition;
            Assert.That(worldAnchorPosition.x, Is.EqualTo(0.0f));
            Assert.That(worldAnchorPosition.y, Is.EqualTo(0.0f));
            Assert.That(worldAnchorPosition.z, Is.EqualTo(0.0f));

            var worldAnchorRotation = joint.worldAnchorRotation;
            Assert.That(worldAnchorRotation.x, Is.EqualTo(0.0f));
            Assert.That(worldAnchorRotation.y, Is.EqualTo(0.0f));
            Assert.That(worldAnchorRotation.z, Is.EqualTo(0.0f));
            Assert.That(worldAnchorRotation.w, Is.EqualTo(1.0f));

            var anchorPos = joint.anchorPosition;
            Assert.That(anchorPos.x, Is.EqualTo(0.0f));
            Assert.That(anchorPos.y, Is.EqualTo(0.0f));
            Assert.That(anchorPos.z, Is.EqualTo(0.0f));

            joint.anchorPosition = new Vector3(1, 0, 0);
            Assert.That(joint.anchorPosition.x, Is.EqualTo(1.0f));

            var frame = joint.anchorFrame;
            Assert.That(frame.anchor.x, Is.EqualTo(1.0f));
            Assert.That(frame.anchor.y, Is.EqualTo(0.0f));
            Assert.That(frame.anchor.z, Is.EqualTo(0.0f));
            Assert.That(frame.axisRotation.x, Is.EqualTo(0.0f));
            Assert.That(frame.axisRotation.y, Is.EqualTo(0.0f));
            Assert.That(frame.axisRotation.z, Is.EqualTo(0.0f));

            joint.anchorFrame = new AxisFrame() { anchor = new Vector3(1, 2, 3), axisRotation = new Vector3(10, 20, 30) };
            frame = joint.anchorFrame;
            Assert.That(frame.anchor.x, Is.EqualTo(1.0f));
            Assert.That(frame.anchor.y, Is.EqualTo(2.0f));
            Assert.That(frame.anchor.z, Is.EqualTo(3.0f));
            Assert.That(frame.axisRotation.x, Is.EqualTo(10.0f));
            Assert.That(frame.axisRotation.y, Is.EqualTo(20.0f));
            Assert.That(frame.axisRotation.z, Is.EqualTo(30.0f));

            //Connected Anchor
            var worldConnectedAnchorPosition = joint.worldConnectedAnchorPosition;
            Assert.That(worldConnectedAnchorPosition.x, Is.EqualTo(1.0f));
            Assert.That(worldConnectedAnchorPosition.y, Is.EqualTo(1.0f));
            Assert.That(worldConnectedAnchorPosition.z, Is.EqualTo(1.0f));

            var worldConnectedAnchorRotation = joint.worldConnectedAnchorRotation;
            Assert.That(worldConnectedAnchorRotation.x, Is.EqualTo(0.0f));
            Assert.That(worldConnectedAnchorRotation.y, Is.EqualTo(0.0f));
            Assert.That(worldConnectedAnchorRotation.z, Is.EqualTo(0.0f));
            Assert.That(worldConnectedAnchorRotation.w, Is.EqualTo(1.0f));

            var connectedAnchorPosition = joint.connectedAnchorPosition;
            Assert.That(connectedAnchorPosition.x, Is.EqualTo(0.0f));
            Assert.That(connectedAnchorPosition.y, Is.EqualTo(0.0f));
            Assert.That(connectedAnchorPosition.z, Is.EqualTo(0.0f));

            joint.connectedAnchorPosition = new Vector3(2, 2, 2);
            connectedAnchorPosition = joint.connectedAnchorPosition;
            Assert.That(connectedAnchorPosition.x, Is.EqualTo(2.0f));
            Assert.That(connectedAnchorPosition.y, Is.EqualTo(2.0f));
            Assert.That(connectedAnchorPosition.z, Is.EqualTo(2.0f));

            var connectedAnchorFrame = joint.connectedAnchorFrame;
            Assert.That(connectedAnchorFrame.anchor.x, Is.EqualTo(2.0f));
            Assert.That(connectedAnchorFrame.anchor.y, Is.EqualTo(2.0f));
            Assert.That(connectedAnchorFrame.anchor.z, Is.EqualTo(2.0f));
            Assert.That(connectedAnchorFrame.axisRotation.x, Is.EqualTo(0.0f));
            Assert.That(connectedAnchorFrame.axisRotation.y, Is.EqualTo(0.0f));
            Assert.That(connectedAnchorFrame.axisRotation.z, Is.EqualTo(0.0f));

            joint.connectedAnchorFrame = new AxisFrame() { anchor = new Vector3(-1, -2, -3), axisRotation = new Vector3(-10, -20, -30) };
            connectedAnchorFrame = joint.connectedAnchorFrame;
            Assert.That(connectedAnchorFrame.anchor.x, Is.EqualTo(-1.0f));
            Assert.That(connectedAnchorFrame.anchor.y, Is.EqualTo(-2.0f));
            Assert.That(connectedAnchorFrame.anchor.z, Is.EqualTo(-3.0f));
            Assert.That(connectedAnchorFrame.axisRotation.x, Is.EqualTo(-10.0f));
            Assert.That(connectedAnchorFrame.axisRotation.y, Is.EqualTo(-20.0f));
            Assert.That(connectedAnchorFrame.axisRotation.z, Is.EqualTo(-30.0f));

            var body = joint.body;
            Assert.IsTrue(body == bodyA);
            var connectedBody = joint.connectedBody;
            Assert.IsTrue(connectedBody == bodyB);

            var dirty = joint.isDirty;
            Assert.IsTrue(dirty);
            PhysicsManager.Simulate(0.02f);

            dirty = joint.isDirty;
            Assert.IsTrue(!dirty);

            var active = joint.active;
            Assert.IsTrue(active);

            joint.breakForce = 2000;
            joint.breakForce = 2000;
            Assert.That(joint.breakForce, Is.EqualTo(2000));

            joint.breakTorque = 1024;
            joint.breakTorque = 1024;
            Assert.That(joint.breakTorque, Is.EqualTo(1024));

            var force = joint.force;
            Assert.That(force != Vector3.zero);
            var torque = joint.torque;
            Assert.That(torque != Vector3.zero);
        }

        [UnityTest]
        public IEnumerator TestJointDisableAndDestroy()
        {
            var bodyA = CreateRigidBody(Vector3.left);
            var bodyB = CreateRigidBody(Vector3.right);

            var joint = bodyA.gameObject.AddComponent<DistanceJoint3D>();
            joint.connectedBody = bodyB;

            yield return new WaitForFixedUpdate();

            joint.enabled = false;
            UnityEngine.Debug.Log("Joint info is " + joint.ToString());

            yield return new WaitForFixedUpdate();

            joint.enabled = true;

            yield return new WaitForFixedUpdate();

            MonoBehaviour.Destroy(joint.gameObject);

            yield return new WaitForFixedUpdate();
        }

        [UnityTest]
        public IEnumerator TestBrokenJoint()
        {
            var bodyA = CreateRigidBody(Vector3.right);
            var bodyB = CreateRigidBody(Vector3.left);

            bodyA.isKinematic = true;
            bodyB.mass = 100;

            var joint = bodyA.gameObject.AddComponent<DistanceJoint3D>();
            joint.SetDistanceLimit(0, 3.0f);
            joint.connectedBody = bodyB;

            var nextUpdate = new WaitForFixedUpdate();

            for (var i = 0; i < 30; i++)
            {
                yield return nextUpdate;
            }

            joint.breakForce = 20;
            joint.breakTorque = 5;

            for (var i = 0; i < 20; i++)
            {
                yield return nextUpdate;
            }

            var value = bodyA.gameObject.TryGetComponent<DistanceJoint3D>(out var res);
            Assert.That(!value);
        }

        [UnityTest]
        public IEnumerator TestUseLimitSpring()
        {
            var nextUpdate = new WaitForFixedUpdate();

            //hinge joint
            //do not use limit
            {
                var bodyA = CreateRigidBody(Vector3.zero);
                var bodyB = CreateRigidBody(Vector3.right);
                bodyA.isKinematic = true;
                bodyA.useGravity = false;

                var joint = bodyA.gameObject.AddComponent<HingeJoint3D>();
                joint.useLimit = false;
                joint.anchorFrame = new AxisFrame(Vector3.zero, new Vector3(0, 90, 0));
                joint.connectedBody = bodyB;
                joint.useSpring = false;

                for (var i = 0; i < 30; i++)
                {
                    yield return nextUpdate;
                }

                Assert.That(bodyB.transform.position.y < -0.5f);

                MonoBehaviour.Destroy(bodyB.gameObject);
                MonoBehaviour.Destroy(bodyA.gameObject);

            }

            for (var i = 0; i < 2; i++)
            {
                yield return nextUpdate;
            }

            //use limit
            {
                var bodyA = CreateRigidBody(Vector3.zero);
                var bodyB = CreateRigidBody(Vector3.right);
                bodyA.isKinematic = true;
                bodyA.useGravity = false;

                var joint = bodyA.gameObject.AddComponent<HingeJoint3D>();
                joint.angularLimit = new JointAngularLimit(0, 10);
                joint.useLimit = true;
                joint.anchorFrame = new AxisFrame(Vector3.zero, new Vector3(0, 90, 0));
                joint.connectedBody = bodyB;
                joint.useSpring = false;

                for (var i = 0; i < 30; i++)
                {
                    yield return nextUpdate;
                }

                Assert.That(bodyB.transform.position.y > -0.5f);

                MonoBehaviour.Destroy(bodyB.gameObject);
                MonoBehaviour.Destroy(bodyA.gameObject);
            }

            for (var i = 0; i < 2; i++)
            {
                yield return nextUpdate;
            }

            //use spring
            {
                var bodyA = CreateRigidBody(Vector3.zero);
                var bodyB = CreateRigidBody(Vector3.right);
                bodyA.isKinematic = true;
                bodyA.useGravity = false;

                var joint = bodyA.gameObject.AddComponent<HingeJoint3D>();
                joint.angularLimit = new JointAngularLimit(0, 10);
                joint.useLimit = true;
                joint.anchorFrame = new AxisFrame(Vector3.zero, new Vector3(0, 90, 0));
                joint.connectedBody = bodyB;
                joint.useSpring = true;
                joint.limitSpring = new SpringDamper(0, 0);

                Assert.That(joint.useSpring);

                for (var i = 0; i < 30; i++)
                {
                    yield return nextUpdate;
                }

                Assert.That(bodyB.transform.position.y < -0.5f);

                MonoBehaviour.Destroy(bodyB.gameObject);
                MonoBehaviour.Destroy(bodyA.gameObject);
            }

            for (var i = 0; i < 2; i++)
            {
                yield return nextUpdate;
            }

            //distance joint
            //do not use limit
            {
                var bodyA = CreateRigidBody(Vector3.zero);
                var bodyB = CreateRigidBody(Vector3.down);
                bodyA.isKinematic = true;
                bodyA.useGravity = false;

                var joint = bodyA.gameObject.AddComponent<DistanceJoint3D>();
                joint.useLimit = false;
                joint.useSpring = false;
                joint.connectedBody = bodyB;

                for (var i = 0; i < 30; i++)
                {
                    yield return nextUpdate;
                }

                Assert.That(bodyB.transform.position.y > -1.1f);

                MonoBehaviour.Destroy(bodyB.gameObject);
                MonoBehaviour.Destroy(bodyA.gameObject);
            }

            for (var i = 0; i < 2; i++)
            {
                yield return nextUpdate;
            }

            //use limit
            {
                var bodyA = CreateRigidBody(Vector3.zero);
                var bodyB = CreateRigidBody(Vector3.down);
                bodyA.isKinematic = true;
                bodyA.useGravity = false;

                var joint = bodyA.gameObject.AddComponent<DistanceJoint3D>();
                joint.useLimit = true;
                joint.useSpring = false;
                joint.SetDistanceLimit(0, 2);
                joint.connectedBody = bodyB;

                for (var i = 0; i < 30; i++)
                {
                    yield return nextUpdate;
                }

                Assert.That(bodyB.transform.position.y < -1.0f);

                MonoBehaviour.Destroy(bodyB.gameObject);
                MonoBehaviour.Destroy(bodyA.gameObject);
            }

            for (var i = 0; i < 2; i++)
            {
                yield return nextUpdate;
            }

            //use spring
            {
                var bodyA = CreateRigidBody(Vector3.zero);
                var bodyB = CreateRigidBody(Vector3.down);
                bodyA.isKinematic = true;
                bodyA.useGravity = false;

                var joint = bodyA.gameObject.AddComponent<DistanceJoint3D>();
                joint.useLimit = true;
                joint.useSpring = true;
                joint.SetDistanceLimit(0, 2);
                joint.connectedBody = bodyB;
                joint.limitSpring = new SpringDamper(0, 0);
                Assert.That(joint.useSpring);

                for (var i = 0; i < 30; i++)
                {
                    yield return nextUpdate;
                }

                Assert.That(bodyB.transform.position.y < -2.0f);

                MonoBehaviour.Destroy(bodyB.gameObject);
                MonoBehaviour.Destroy(bodyA.gameObject);
            }

            for (var i = 0; i < 2; i++)
            {
                yield return nextUpdate;
            }

            //ball joint
            //do not use spring
            {
                var bodyA = CreateRigidBody(Vector3.zero);
                var bodyB = CreateRigidBody(Vector3.right);
                bodyA.isKinematic = true;
                bodyA.useGravity = false;

                var joint = bodyA.gameObject.AddComponent<BallJoint3D>();
                joint.useSpring = false;
                joint.connectedBody = bodyB;

                for (var i = 0; i < 30; i++)
                {
                    yield return nextUpdate;
                }

                Assert.That(bodyB.transform.position.y < -0.1f);

                MonoBehaviour.Destroy(bodyB.gameObject);
                MonoBehaviour.Destroy(bodyA.gameObject);
            }

            for (var i = 0; i < 2; i++)
            {
                yield return nextUpdate;
            }

            //use spring
            {
                var bodyA = CreateRigidBody(Vector3.zero);
                var bodyB = CreateRigidBody(Vector3.right);
                bodyA.isKinematic = true;
                bodyA.useGravity = false;

                var joint = bodyA.gameObject.AddComponent<BallJoint3D>();
                joint.useSpring = true;
                joint.limitSpring = new SpringDamper(0, 0);
                joint.connectedBody = bodyB;

                Assert.That(joint.useSpring);
                Assert.That(joint.limitSpring.stiffness == 0);
                Assert.That(joint.limitSpring.damper == 0);
                for (var i = 0; i < 30; i++)
                {
                    yield return nextUpdate;
                }

                Assert.That(bodyB.transform.position.y < -0.5f);

                MonoBehaviour.Destroy(bodyB.gameObject);
                MonoBehaviour.Destroy(bodyA.gameObject);
            }

            //EllipsoidJoint
            //do not use spring
            {
                var bodyA = CreateRigidBody(Vector3.zero);
                var bodyB = CreateRigidBody(Vector3.right);
                bodyA.isKinematic = true;
                bodyA.useGravity = false;

                var joint = bodyA.gameObject.AddComponent<EllipsoidJoint3D>();
                joint.useSpring = false;
                joint.connectedBody = bodyB;
                joint.swingLimitZ = JointAngularLimit.One;
                for (var i = 0; i < 30; i++)
                {
                    yield return nextUpdate;
                }

                Assert.That(Mathf.Abs(bodyB.transform.position.y) < 0.1f);

                MonoBehaviour.Destroy(bodyB.gameObject);
                MonoBehaviour.Destroy(bodyA.gameObject);
            }

            for (var i = 0; i < 2; i++)
            {
                yield return nextUpdate;
            }

            //use spring
            {
                var bodyA = CreateRigidBody(Vector3.zero);
                var bodyB = CreateRigidBody(Vector3.right);
                bodyA.isKinematic = true;
                bodyA.useGravity = false;

                var joint = bodyA.gameObject.AddComponent<EllipsoidJoint3D>();
                joint.useSpring = true;
                joint.limitSpring = new SpringDamper(0, 0);
                joint.connectedBody = bodyB;
                joint.angularDamper = 3;
                joint.angularDamper = 3;

                Assert.That(joint.useSpring);
                Assert.That(joint.angularDamper == 3);
                for (var i = 0; i < 30; i++)
                {
                    yield return nextUpdate;
                }

                Assert.That(bodyB.transform.position.y < -0.5f);

                MonoBehaviour.Destroy(bodyB.gameObject);
                MonoBehaviour.Destroy(bodyA.gameObject);
            }

            //SliderJoint
            //do not use limit
            {
                var bodyA = CreateRigidBody(Vector3.zero);
                var bodyB = CreateRigidBody(new Vector3(3, 0, 0));
                bodyA.isKinematic = true;
                bodyA.useGravity = false;
                bodyA.enablePostTransformControl = true;

                bodyA.transform.rotation = Quaternion.Euler(0, 0, -10);

                var joint = bodyA.gameObject.AddComponent<SliderJoint3D>();
                joint.useLimit = false;
                joint.useSpring = false;
                joint.connectedBody = bodyB;

                for (var i = 0; i < 180; i++)
                {
                    yield return nextUpdate;
                }

                Assert.That(bodyB.transform.position.y < -1.0f);

                MonoBehaviour.Destroy(bodyB.gameObject);
                MonoBehaviour.Destroy(bodyA.gameObject);
            }

            for (var i = 0; i < 2; i++)
            {
                yield return nextUpdate;
            }

            // use limit
            {
                var bodyA = CreateRigidBody(Vector3.zero);
                var bodyB = CreateRigidBody(new Vector3(3, 0, 0));
                bodyA.isKinematic = true;
                bodyA.useGravity = false;
                bodyA.enablePostTransformControl = true;

                bodyA.transform.rotation = Quaternion.Euler(0, 0, -10);

                var joint = bodyA.gameObject.AddComponent<SliderJoint3D>();
                joint.useLimit = true;
                joint.useSpring = false;
                joint.connectedBody = bodyB;
                joint.distanceLimit = new JointTranslationLimit(0, 1);

                for (var i = 0; i < 180; i++)
                {
                    yield return nextUpdate;
                }

                Assert.That(bodyB.transform.position.y > -1.0f);

                MonoBehaviour.Destroy(bodyB.gameObject);
                MonoBehaviour.Destroy(bodyA.gameObject);
            }

            for (var i = 0; i < 2; i++)
            {
                yield return nextUpdate;
            }

            // use spring
            {
                var bodyA = CreateRigidBody(Vector3.zero);
                var bodyB = CreateRigidBody(new Vector3(3, 0, 0));
                bodyA.isKinematic = true;
                bodyA.useGravity = false;
                bodyA.enablePostTransformControl = true;

                bodyA.transform.rotation = Quaternion.Euler(0, 0, -10);

                var joint = bodyA.gameObject.AddComponent<SliderJoint3D>();
                joint.useLimit = true;
                joint.useSpring = true;
                joint.limitSpring = new SpringDamper(0, 0);
                joint.connectedBody = bodyB;
                joint.distanceLimit = new JointTranslationLimit(0, 1);
                Assert.That(joint.useSpring);

                for (var i = 0; i < 180; i++)
                {
                    yield return nextUpdate;
                }

                Assert.That(bodyB.transform.position.y < -1.0f);

                MonoBehaviour.Destroy(bodyB.gameObject);
                MonoBehaviour.Destroy(bodyA.gameObject);
            }
        }

        [UnityTest]
        public IEnumerator TestDisabledJoint()
        {
            var nextUpdate = new WaitForFixedUpdate();

            {
                var bodyA = CreateRigidBody();
                bodyA.isKinematic = true;
                var bodyB = CreateRigidBody(new Vector3(0, 0, 1));
                bodyB.useGravity = false;

                var joint = bodyA.gameObject.AddComponent<HingeJoint3D>();
                joint.connectedBody = bodyB;
                joint.useLimit = true;
                joint.angularLimit = new JointAngularLimit(10, 30);
                joint.enabled = false;

                yield return nextUpdate;

                Assert.That(bodyB.position.y == 0);

                joint.enabled = true;

                yield return nextUpdate;
                yield return nextUpdate;
                yield return nextUpdate;

                Assert.That(bodyB.position.y < 0);
            }
        }
    }
}
