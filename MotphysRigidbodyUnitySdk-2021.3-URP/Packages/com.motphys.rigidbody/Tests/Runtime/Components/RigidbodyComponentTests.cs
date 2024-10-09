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

namespace Motphys.Rigidbody.Tests
{
    internal class RigidbodyComponentTests : ComponentTestScene
    {
        [UnityTest]
        public IEnumerator TestSwitchKinematicsFlag()
        {
            var body = CreateRigidBody();
            yield return new WaitForFixedUpdate();
            Assert.That(body.linearVelocity.y < 0f);
            body.isKinematic = true;
            AssertVector3Eq(body.linearVelocity, Vector3.zero);
        }

        [UnityTest]
        public IEnumerator TestApplyForce()
        {
            var body = CreateRigidBody();
            body.useGravity = false;
            body.gameObject.AddComponent<BoxCollider3D>();
            yield return new WaitForFixedUpdate();
            AssertVector3Eq(body.position, Vector3.zero);
            body.AddForceAtPosition(Vector3.up, Vector3.zero, ForceMode.Force);
            yield return new WaitForFixedUpdate();
            var vel = body.linearVelocity;
            Assert.That(vel.y > 0f);
            Assert.That(vel.x, Is.EqualTo(0f));
            Assert.That(vel.z, Is.EqualTo(0f));
            AssertVector3Eq(body.angularVelocity, Vector3.zero);

            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.enablePostTransformControl = true;
            body.transform.position = Vector3.one;
            yield return new WaitForFixedUpdate();

            body.AddForceAtPosition(Vector3.up, Vector3.one, ForceMode.Force);
            yield return new WaitForFixedUpdate();

            vel = body.linearVelocity;
            Assert.That(vel.y > 0f);
            Assert.That(vel.x, Is.EqualTo(0f));
            Assert.That(vel.z, Is.EqualTo(0f));
            AssertVector3Eq(body.angularVelocity, Vector3.zero);
        }

        [UnityTest]
        public IEnumerator TestMaxVelocity()
        {
            var body = CreateRigidBody();
            body.useGravity = false;
            body.gameObject.AddComponent<BoxCollider3D>();
            body.maxLinearVelocity = 1f;
            body.AddForceAtPosition(Vector3.up * 1000000, Vector3.zero, ForceMode.Force);
            yield return new WaitForFixedUpdate();
            AssertVector3Eq(body.linearVelocity, Vector3.up);
        }

        [UnityTest]
        public IEnumerator TestDetectCollisionFlag()
        {
            var body = CreateRigidBody();
            body.gameObject.AddComponent<SphereCollider3D>();
            body.enablePostTransformControl = true;
            body.transform.position = new Vector3(0, 1, 0);
            body.linearVelocity = Vector3.down;

            var collider = new GameObject("Plane").AddComponent<BoxCollider3D>();
            body.detectCollisions = false;

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            Assert.That(body.transform.position.y < 0.95f);
            body.detectCollisions = true;

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            Assert.That(body.detectCollisions);
            Assert.That(collider.detectCollisions);

            Assert.That(body.transform.position.y > 0.95f);
        }

        [UnityTest]
        public IEnumerator TestAngularVelocity()
        {
            var body = CreateRigidBody();
            body.gameObject.AddComponent<BoxCollider3D>();
            body.angularVelocity = new Vector3(1f, 2f, 3f);
            yield return new WaitForFixedUpdate();
            AssertVector3Eq(body.angularVelocity, new Vector3(1f, 2f, 3f), 1e-2f);
        }

        [UnityTest]

        public IEnumerator TestCenterOfMass()
        {
            var nextUpdate = new WaitForFixedUpdate();

            var body = CreateRigidBody(new Vector3(0, 10, 0));
            body.useGravity = false;
            var box0 = body.gameObject.AddComponent<BoxCollider3D>();
            var box1 = body.gameObject.AddComponent<BoxCollider3D>();
            box1.shapeTranslation = new Vector3(2, 0, 0);

            yield return nextUpdate;

            AssertVector3Eq(body.centerOfMass, new Vector3(1, 0, 0));
            var worldCentroid = body.worldCenterOfMass;

            AssertVector3Eq(worldCentroid, new Vector3(1, 10, 0));

            yield return nextUpdate;

            body.centerOfMass = new Vector3(0, -1, 0);
            yield return nextUpdate;
            AssertVector3Eq(body.centerOfMass, new Vector3(0, -1, 0));
            AssertVector3Eq(body.worldCenterOfMass, new Vector3(0, 9, 0));

            body.ResetBodyCenterOfMass();
            yield return nextUpdate;
            AssertVector3Eq(body.centerOfMass, new Vector3(1, 0, 0));
        }

        [Test]
        public void TestDrag()
        {
            var body = CreateRigidBody();
            // test linear drag.
            // default is 0
            Assert.That(body.drag, Is.EqualTo(0f));
            body.drag = 1.0f;
            Assert.That(body.drag, Is.EqualTo(1.0f));
            Assert.That(body.nativeBridge.linearDamper, Is.EqualTo(1.0f));

            body.drag = -1f;
            Assert.That(body.drag, Is.EqualTo(0f));
            Assert.That(() =>
            {
                body.nativeBridge.linearDamper = -1f;
            }, Throws.ArgumentException);

            Assert.That(body.nativeBridge.linearDamper, Is.EqualTo(0f));

            // test angular drag
            // default is 0.0
            Assert.That(body.angularDrag, Is.EqualTo(0f));
            body.angularDrag = 2.0f;
            Assert.That(body.angularDrag, Is.EqualTo(2.0f));
            Assert.That(body.nativeBridge.angularDamper, Is.EqualTo(2.0f));

            body.angularDrag = -1f;
            Assert.That(body.angularDrag, Is.EqualTo(0f));
            Assert.That(() =>
            {
                body.nativeBridge.angularDamper = -1f;
            }, Throws.ArgumentException);
            Assert.That(body.nativeBridge.angularDamper, Is.EqualTo(0f));
        }

        [Test]
        public void TestSleepThreshold()
        {
            var body = CreateRigidBody();
            body.sleepThreshold = 3.1415f;
            Assert.That(body.sleepThreshold, Is.EqualTo(3.1415f));

            // use nativeBridge to get the value from native directly.
            Assert.That(body.nativeBridge.sleepThreshold, Is.EqualTo(3.1415f));

            // set negative value to native is not allowed.
            Assert.That(() =>
            {
                body.nativeBridge.sleepThreshold = -1f;
            }, Throws.ArgumentException);

            // however, set negative value to component will be clamped to 0.
            body.sleepThreshold = -1f;
            Assert.That(body.sleepThreshold, Is.EqualTo(0f));
        }

        [Test]
        public void TestWakeCounter()
        {
            var body = CreateRigidBody();
            // The default wake counter is 0.4 seconds.
            Assert.That(body.wakeCounter, Is.EqualTo(0.4f));
        }
        [UnityTest]
        public IEnumerator TestTransform()
        {
            var body = CreateRigidBody();
            body.useGravity = false;

            AssertVector3Eq(body.position, Vector3.zero, 1e-6f);
            Assert.That(body.rotation, Is.EqualTo(Quaternion.identity));

            //change the position and rotation of rigidbody will immediately work.

            body.position = Vector3.one;
            AssertVector3Eq(body.position, Vector3.one, 1e-6f);

            body.rotation = Quaternion.Euler(1, 2, 3);
            Assert.That(body.rotation, Is.EqualTo(Quaternion.Euler(1, 2, 3)));

            // however, the change won't sync to transform immediately.
            AssertVector3Eq(body.transform.position, Vector3.zero, 1e-6f);
            Assert.That(body.transform.rotation, Is.EqualTo(Quaternion.identity));

            yield return new WaitForFixedUpdate();

            // sync is done until next fixed update.
            AssertVector3Eq(body.transform.position, Vector3.one, 1e-6f);
            Assert.That(body.transform.rotation, Is.EqualTo(Quaternion.Euler(1, 2, 3)));
        }

        [UnityTest]
        public IEnumerator TestMass()
        {
            PhysicsManager.numSubstep = 1;
            PhysicsManager.defaultSolverIterations = 1;

            var body = CreateRigidBody();
            body.useGravity = false;
            body.maxLinearVelocity = float.MaxValue;

            // auto clamped to 0.
            body.mass = -1.0f;
            Assert.That(body.mass, Is.EqualTo(0.0f));

            body.AddForceAtPosition(Vector3.up * 1000000, Vector3.zero, ForceMode.Force);
            yield return new WaitForFixedUpdate();

            // mass = 0 means infinite mass. so the velocity won't change even if force is applied.
            AssertVector3Eq(body.linearVelocity, Vector3.zero, 1e-2f);

            body.mass = 1.0f;
            body.AddForceAtPosition(Vector3.up * 1000000, Vector3.zero, ForceMode.Force);
            yield return new WaitForFixedUpdate();

            // a = F/m = 1000000/1 = 1000000
            // v = a * t = 1000000 * dt.
            var dt = UnityEngine.Time.fixedDeltaTime;
            AssertVector3Eq(body.linearVelocity, new Vector3(0, 1000000 * dt, 0), 1e-2f);
        }

        [UnityTest]
        public IEnumerator TestSetKinematicTarget()
        {
            var body = CreateRigidBody();
            body.isKinematic = true;

            body.SetKinematicTarget(Vector3.one, Quaternion.identity);

            yield return new WaitForFixedUpdate();

            body.SetKinematicTarget(new Vector3(10, 2, 3), Quaternion.Euler(new Vector3(90, 0, 0)));

            yield return new WaitForFixedUpdate();
            AssertVector3Eq(new Vector3(10, 2, 3), body.position);
            AssertVector3Eq(new Vector3(10, 2, 3), body.gameObject.transform.position);
        }

        [UnityTest]
        public IEnumerator TestAddForceAtPosition()
        {
            var spawnPos = new Vector3(1, 0, 0);
            var body = CreateRigidBody(spawnPos);
            body.useGravity = false;
            body.gameObject.AddComponent<BoxCollider3D>();

            var nextFrame = new WaitForFixedUpdate();

            body.AddForceAtPosition(new Vector3(0, 0, 100), Vector3.zero, ForceMode.Force);
            yield return nextFrame;
            yield return nextFrame;
            yield return nextFrame;

            var rotation = body.transform.rotation;
            Assert.That(rotation != Quaternion.identity);

            body.AddForceAtPosition(new Vector3(0, 0, 10), Vector3.zero, ForceMode.Impulse);
            yield return nextFrame;
            yield return nextFrame;
            yield return nextFrame;

            var position = body.transform.position;
            Assert.That(position != spawnPos);
        }

        [UnityTest]
        public IEnumerator TestAddForce()
        {
            var body = CreateRigidBody();
            var nextFrame = new WaitForFixedUpdate();

            body.AddForce(new Vector3(0, 100, 0), ForceMode.Force);
            yield return nextFrame;
            yield return nextFrame;
            yield return nextFrame;

            var position = body.transform.position;
            Assert.That(position.y > 0);

            body.AddForce(new Vector3(10, 0, 0), ForceMode.Impulse);
            yield return nextFrame;
            yield return nextFrame;
            yield return nextFrame;
            position = body.transform.position;
            Assert.That(position.x > 0);
        }

        [UnityTest]
        public IEnumerator TestAddTorque()
        {
            var body = CreateRigidBody();
            var nextFrame = new WaitForFixedUpdate();

            body.AddTorque(new Vector3(0, 200, 0), ForceMode.Force);
            yield return nextFrame;
            yield return nextFrame;
            yield return nextFrame;

            var rotation = body.transform.rotation.eulerAngles;
            Assert.That(rotation.y > 0);

            body.AddTorque(new Vector3(20, 0, 0), ForceMode.Impulse);
            yield return nextFrame;
            yield return nextFrame;
            yield return nextFrame;
            rotation = body.transform.rotation.eulerAngles;
            Assert.That(rotation.x > 0);
        }

        [UnityTest]
        public IEnumerator TestAddRelativeForce()
        {
            var body = CreateRigidBody();
            body.enablePostTransformControl = true;
            body.rotation = Quaternion.Euler(0, 30, 0);
            body.useGravity = false;

            var nextFrame = new WaitForFixedUpdate();

            body.AddRelativeForce(new Vector3(0, 0, 100), ForceMode.Force);

            yield return nextFrame;
            yield return nextFrame;
            yield return nextFrame;

            var position = body.transform.position;
            Assert.That(position.x > 0);

            body.AddRelativeForce(new Vector3(0, 0, 35), ForceMode.VelocityChange);
            yield return nextFrame;
            Assert.That(body.linearVelocity.magnitude > 30);
        }

        [UnityTest]
        public IEnumerator TestAddRelativeTorque()
        {
            var body = CreateRigidBody();
            body.enablePostTransformControl = true;
            body.rotation = Quaternion.Euler(90, 0, 0);
            body.useGravity = false;

            var nextFrame = new WaitForFixedUpdate();

            body.AddRelativeTorque(new Vector3(0, 50, 0), ForceMode.Force);

            yield return nextFrame;
            yield return nextFrame;
            yield return nextFrame;

            var rotation = body.transform.rotation.eulerAngles;
            Assert.That(rotation.z > 0);
        }

        [UnityTest]
        public IEnumerator TestEnablePostTransformControl()
        {
            var nextFrame = new WaitForFixedUpdate();
            {
                var body = CreateRigidBody();
                body.enablePostTransformControl = false;
                yield return nextFrame;
                body.enablePostTransformControl = false;
                yield return nextFrame;
                body.enablePostTransformControl = true;
                yield return nextFrame;
                body.enablePostTransformControl = true;
                yield return nextFrame;
                body.enablePostTransformControl = false;
                yield return nextFrame;
                body.enablePostTransformControl = true;
                yield return nextFrame;
            }

            {
                //dynamic use transform
                var body = CreateRigidBody();
                body.enablePostTransformControl = false;
                body.useGravity = false;

                var position = new Vector3(2, 3, 4);
                var rotation = Quaternion.Euler(30, 30, 0);
                yield return nextFrame;

                body.transform.position = position;
                yield return nextFrame;
                Assert.That(body.position != position);

                body.transform.rotation = rotation;
                yield return nextFrame;
                Assert.That(body.rotation != rotation);

                body.enablePostTransformControl = true;

                body.transform.position = position;
                yield return nextFrame;
                Assert.That(body.position == position);

                body.transform.rotation = rotation;
                yield return nextFrame;
                Assert.That(body.rotation == rotation);
            }

            {
                //dynamic use rigidbody 
                var body = CreateRigidBody();
                body.enablePostTransformControl = false;
                body.useGravity = false;

                var position = new Vector3(2, 3, 4);
                var rotation = Quaternion.Euler(30, 30, 0);
                yield return nextFrame;

                body.position = position;
                yield return nextFrame;
                Assert.That(body.transform.position == position);

                body.rotation = rotation;
                yield return nextFrame;
                Assert.That(body.transform.rotation == rotation);

                body.enablePostTransformControl = true;

                var newPos = new Vector3(0, 2, 1);
                body.position = newPos;
                yield return nextFrame;
                Assert.That(body.transform.position == newPos);

                var newRot = Quaternion.Euler(10, 30, 40);
                body.rotation = newRot;
                yield return nextFrame;
                Assert.That(body.transform.rotation == newRot);
            }

            {
                //kinematic use transform
                var body = CreateRigidBody();
                body.enablePostTransformControl = false;
                body.useGravity = false;
                body.isKinematic = true;

                var position = new Vector3(2, 3, 4);
                var rotation = Quaternion.Euler(30, 30, 0);
                yield return nextFrame;

                body.transform.position = position;
                yield return nextFrame;
                Assert.That(body.position != position);

                body.transform.rotation = rotation;
                yield return nextFrame;
                Assert.That(body.rotation != rotation);

                body.enablePostTransformControl = true;

                var newPos = new Vector3(0, 2, 1);
                var newRot = Quaternion.Euler(10, 30, 40);
                body.transform.position = newPos;
                yield return nextFrame;
                Assert.That(body.position == newPos);

                body.transform.rotation = newRot;
                yield return nextFrame;
                Assert.That(body.rotation == newRot);
            }

            {
                //kinematic use rigidbody 
                var body = CreateRigidBody();
                body.enablePostTransformControl = false;
                body.useGravity = false;
                body.isKinematic = true;

                var position = new Vector3(2, 3, 4);
                var rotation = Quaternion.Euler(30, 30, 0);
                yield return nextFrame;

                body.position = position;
                yield return nextFrame;
                Assert.That(body.transform.position == position);

                body.rotation = rotation;
                yield return nextFrame;
                Assert.That(body.transform.rotation == rotation);

                body.enablePostTransformControl = true;

                var newPos = new Vector3(0, 2, 1);
                body.position = newPos;
                yield return nextFrame;
                Assert.That(body.transform.position == newPos);

                var newRot = Quaternion.Euler(10, 30, 40);
                body.rotation = newRot;
                yield return nextFrame;
                Assert.That(body.transform.rotation == newRot);
            }

            {
                //destroy body.
                var bodyA = CreateRigidBody();
                var bodyB = CreateRigidBody();

                bodyA.useGravity = false;
                bodyB.useGravity = false;

                bodyA.enablePostTransformControl = true;
                bodyB.enablePostTransformControl = true;

                var position = new Vector3(2, 3, 4);
                var rotation = Quaternion.Euler(30, 30, 0);
                var newPos = new Vector3(0, 2, 1);
                var newRot = Quaternion.Euler(10, 30, 40);

                bodyA.transform.position = position;
                bodyA.transform.rotation = rotation;

                bodyB.transform.position = position;
                bodyB.transform.rotation = rotation;

                yield return nextFrame;

                Assert.That(bodyA.transform.position == position);
                Assert.That(bodyA.transform.rotation == rotation);

                MonoBehaviour.Destroy(bodyA);

                bodyB.transform.position = newPos;
                bodyB.transform.rotation = newRot;

                yield return nextFrame;

                Assert.That(bodyB.transform.position == newPos);
                Assert.That(bodyB.transform.rotation == newRot);
            }
        }
    }
}
