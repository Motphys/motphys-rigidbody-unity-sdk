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
using Motphys.Rigidbody.Internal;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace Motphys.Rigidbody.Tests
{
    internal class BodyColliderCompositeTests : ComponentTestScene
    {

        [UnityTest]
        public IEnumerator TestRigidBodyWithoutCollider()
        {
            var body = CreateRigidBody("Body");
            //by default, a simple rigidbody without any collider is dynamic and will be affected by gravity.
            Assert.That(!body.isKinematic);
            Assert.That(body.transform.position.y, Is.EqualTo(0f).Within(1e-3f));
            yield return new WaitForFixedUpdate();

            {
                var dt = Time.fixedDeltaTime;
                var v = PhysicsManager.gravity.y * dt;
                var dropDistance = v * dt;
                Assert.That(body.transform.position.y, Is.EqualTo(dropDistance));
                Assert.That(body.linearVelocity, Is.EqualTo(new Vector3(0f, v, 0f)).Using(Vector3EqualityComparer.Instance));
            }

            var position = body.transform.position;
            body.gameObject.SetActive(false);
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            // inactive rigidbody should not move
            Assert.That(body.transform.position, Is.EqualTo(position).Using(Vector3EqualityComparer.Instance));

            body.gameObject.SetActive(true);
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            Assert.That(body.transform.position.y < position.y);
        }

        [UnityTest]
        public IEnumerator TestAddRemoveColliderOnRigidBody()
        {
            var body = CreateRigidBody("Body", new Vector3(2f, 1.5f, 2f));
            var collider = body.gameObject.AddComponent<BoxCollider3D>();

            var plane = new GameObject("Plane");
            plane.transform.localScale = new Vector3(100, 1, 100);
            var planeCollider = plane.gameObject.AddComponent<BoxCollider3D>();
            Assert.That(planeCollider.attachedRigidbody == null);

            for (var i = 0; i < 50; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            // The body should be hold by the plane
            Assert.That(body.transform.position.y > 0.9f, "body position.y = " + body.transform.position.y);
            Object.Destroy(collider);
            yield return new WaitForFixedUpdate();

            for (var i = 0; i < 50; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.That(body.transform.position.y < 0f);
        }

        [UnityTest]
        public IEnumerator TestAddRemoveRigidbodyOnCollider()
        {
            // static plane
            var plane = new GameObject("Plane");
            plane.transform.localScale = new Vector3(100, 1, 100);
            plane.gameObject.AddComponent<BoxCollider3D>();

            // static cube
            var initBodyPosition = new Vector3(0f, 1.5f, 0f);
            var body = new GameObject("Body");
            body.transform.position = initBodyPosition;
            var bodyCollider = body.AddComponent<BoxCollider3D>();

            Assert.IsFalse(bodyCollider.id.Equals(null));
            Assert.IsTrue(bodyCollider.id.Equals((object)bodyCollider.id));
            Assert.IsFalse(bodyCollider.id.Equals(bodyCollider));

            Assert.IsFalse(bodyCollider.bodyId.Equals(null));
            Assert.IsTrue(bodyCollider.bodyId.Equals((object)bodyCollider.bodyId));
            Assert.IsFalse(bodyCollider.bodyId.Equals(bodyCollider));

            yield return new WaitForFixedUpdate();

            // static cube should not move
            Assert.That(bodyCollider.transform.position, Is.EqualTo(initBodyPosition).Using(Vector3EqualityComparer.Instance));

            // if we add a rigidbody component to it, body type will be changed to dynamic
            var rigidbody = bodyCollider.gameObject.AddComponent<Rigidbody3D>();
            rigidbody.enablePostTransformControl = true;

            for (var i = 0; i < 30; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            // dynamic cube should fall down
            Assert.That(body.transform.position.y >= 1.0 - 1e-3 && body.transform.position.y <= 1.1f, "body position.y = " + body.transform.position.y);
            bodyCollider.transform.position = initBodyPosition;
            yield return new WaitForFixedUpdate();
            // destroy rigidbody component, and body will change back to static
            Object.Destroy(bodyCollider.attachedRigidbody);
            // if Destroy is called in fixed update frame, the body will be simulated first, and then change to static in the next frame.
            // So we need to yield WaitForFixedUpdate to ensure the destroy operation is done.
            yield return new WaitForFixedUpdate();
            initBodyPosition = bodyCollider.transform.position;

            for (var i = 0; i < 30; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.That(bodyCollider.transform.position, Is.EqualTo(initBodyPosition).Using(Vector3EqualityComparer.Instance));
        }

        [UnityTest]
        public IEnumerator TestStaticCollider()
        {
            // static plane
            var plane = new GameObject("Plane");
            plane.transform.localScale = new Vector3(100, 1, 100);
            plane.gameObject.AddComponent<BoxCollider3D>();
            yield return new WaitForFixedUpdate();
            plane.gameObject.SetActive(false);
            yield return new WaitForFixedUpdate();
            plane.gameObject.SetActive(true);
            yield return new WaitForFixedUpdate();
            plane.gameObject.SetActive(false);
            plane.gameObject.SetActive(true);
        }

        [UnityTest]
        public IEnumerator TestColliderEnable()
        {
            // static plane
            var plane = new GameObject("Plane");
            plane.transform.localScale = new Vector3(100, 1, 100);
            plane.gameObject.AddComponent<BoxCollider3D>();

            // cube
            var initBodyPosition = new Vector3(0f, 1.1f, 0f);
            var body = new GameObject("Body");
            body.transform.position = initBodyPosition;
            body.AddComponent<BoxCollider3D>();
            var rigidbody = body.AddComponent<Rigidbody3D>();
            rigidbody.enablePostTransformControl = true;

            for (var i = 0; i < 10; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            // cube hold on the plane
            Assert.That(body.transform.position.y, Is.GreaterThan(1.0 - 2e-3));
            // disable collider
            body.GetComponent<BaseCollider>().enabled = false;
            for (var i = 0; i < 10; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            // cube should pass through the plane
            Assert.That(body.transform.position.y < 0.9);

            // enable again
            body.GetComponent<BaseCollider>().enabled = true;
            body.transform.position = initBodyPosition;

            for (var i = 0; i < 10; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            // cube hold on the plane
            Assert.That(body.transform.position.y > 1.0 - 1e-3);

            body.GetComponent<BaseCollider>().enabled = false;
            body.GetComponent<BaseCollider>().enabled = true;
            //change collider enable state twice, should not crash

            for (var i = 0; i < 2; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            // cube hold on the plane
            Assert.That(body.transform.position.y, Is.GreaterThan(1.0 - 1e-3));
        }

        [UnityTest]
        public IEnumerator TestRigidbodyColliderAwakeTogether()
        {
            var go = new GameObject("Go");
            go.SetActive(false);
            go.AddComponent<Rigidbody3D>();
            go.AddComponent<BoxCollider3D>();
            go.SetActive(true);
            yield return new WaitForFixedUpdate();
            Object.Destroy(go);
            yield return new WaitForFixedUpdate();
        }

        [UnityTest]
        public IEnumerator TestDeactivateDynamic()
        {
            var body = CreateRigidBody();
            body.gameObject.AddComponent<BoxCollider3D>();
            yield return new WaitForFixedUpdate();
            body.gameObject.SetActive(false);

            Assert.IsTrue(body.isNativeValid);

            body.linearVelocity = Vector3.one;
            AssertVector3Eq(body.linearVelocity, Vector3.one);

            yield return new WaitForFixedUpdate();
            AssertVector3Eq(body.linearVelocity, Vector3.one);

            body.gameObject.SetActive(false);
            body.gameObject.SetActive(true);
        }

        [UnityTest]
        public IEnumerator TestMultiCollidersInStaticActor()
        {
            var box = new GameObject("Box");
            box.SetActive(false);

            var colliderA = box.AddComponent<BoxCollider3D>();
            colliderA.shapeTranslation = Vector3.left;
            var colliderB = box.AddComponent<BoxCollider3D>();
            colliderB.shapeTranslation = Vector3.right;

            box.SetActive(true);

            Assert.AreNotEqual(colliderA.id, colliderB.id);
            Assert.IsTrue(colliderA.id != colliderB.id);
            Assert.IsFalse(colliderA.id == colliderB.id);
            Assert.IsFalse(colliderA.id.Equals(colliderB.id));

            yield return new WaitForFixedUpdate();

            Assert.AreEqual((Bounds)colliderA.motionAABB, new Bounds(colliderA.shapeTranslation, Vector3.one * (1 + 2 * colliderA.collisionSetting.contactOffset)));
            Assert.AreEqual((Bounds)colliderB.motionAABB, new Bounds(colliderB.shapeTranslation, Vector3.one * (1 + 2 * colliderB.collisionSetting.contactOffset)));

            var rigidBody3D = box.AddComponent<Rigidbody3D>();
            Assert.AreEqual(colliderA.attachedRigidbody, rigidBody3D);
            Assert.AreEqual(colliderB.attachedRigidbody, rigidBody3D);
        }
        [Test]
        public void TestMultiCollidersInMovableActor2()
        {
            var box = new GameObject("Box");
            box.SetActive(false);
            var colliderA = box.AddComponent<BoxCollider3D>();
            colliderA.shapeTranslation = Vector3.left;
            var rigidBody3D = box.AddComponent<Rigidbody3D>();

            box.SetActive(true);

            rigidBody3D.ForeachAttachedCollider((collider) => { Assert.AreEqual(colliderA, collider); });
        }

        [UnityTest]
        public IEnumerator TestMultiCollidersInMovableActor()
        {
            var box = new GameObject("Box");
            box.SetActive(false);
            var colliderA = box.AddComponent<BoxCollider3D>();
            colliderA.shapeTranslation = Vector3.left;
            var colliderB = box.AddComponent<BoxCollider3D>();
            colliderB.shapeTranslation = Vector3.right;
            var rigidBody3D = box.AddComponent<Rigidbody3D>();

            box.SetActive(true);

            Assert.AreEqual(colliderA.attachedRigidbody, rigidBody3D);
            Assert.AreEqual(colliderB.attachedRigidbody, rigidBody3D);

            Assert.AreNotEqual(colliderA.id, colliderB.id);
            Assert.IsTrue(colliderA.id != colliderB.id);
            Assert.IsFalse(colliderA.id == colliderB.id);
            Assert.IsFalse(colliderA.id.Equals(colliderB.id));

            Assert.AreEqual(rigidBody3D.numShapes, 2);
            rigidBody3D.ForeachAttachedCollider((collider) => { Assert.IsTrue(colliderA == collider || colliderB == collider); });

            yield return new WaitForFixedUpdate();
            //inertiaPrincipal only update in in physics step.
            Assert.AreEqual(rigidBody3D.inertiaPrincipal, new Vector3(0.16666667f, 1.1666666f, 1.1666666f));

            yield return new WaitForFixedUpdate();

            colliderA.enabled = false;
            Assert.AreEqual(rigidBody3D.numShapes, 1);

            colliderA.enabled = true;
            Assert.AreEqual(rigidBody3D.numShapes, 2);

            rigidBody3D.detectCollisions = false;
            Assert.IsFalse(rigidBody3D.detectCollisions);
            Assert.IsFalse(colliderA.detectCollisions);
            Assert.IsFalse(colliderB.detectCollisions);

            rigidBody3D.detectCollisions = true;
            Assert.IsTrue(rigidBody3D.detectCollisions);
            Assert.IsTrue(colliderA.detectCollisions);
            Assert.IsTrue(colliderB.detectCollisions);

            rigidBody3D.detectCollisions = false;
            var colliderC = box.AddComponent<BoxCollider3D>();
            Assert.IsFalse(colliderC.detectCollisions);

            colliderB.enabled = false;
            Assert.IsFalse(colliderB.childIndex.isValid);
            Assert.AreEqual(MaterialData.Default, colliderB.material.data);
            Assert.IsFalse(colliderB.detectCollisions);
            Assert.AreEqual(Aabb3.Zero, colliderB.motionAABB);

            // Make sure not to crash
            colliderB.shapeTranslation = Vector3.right;
            colliderB.detectCollisions = false;
        }

        [UnityTest]
        public IEnumerator TestSetColliderGroup()
        {
            var physicsSettings = PhysicsProjectSettings.Get<PhysicsProjectSettings>();
            var mask0 = physicsSettings.GetCollisionMask(3);
            var mask1 = physicsSettings.GetCollisionMask(4);

            physicsSettings.SetAllLayersCollisionMask(false);
            physicsSettings.IgnoreLayerCollision(3, 3, false);
            physicsSettings.IgnoreLayerCollision(4, 4, false);
            PhysicsManager.RestartEngine();

            {
                var boxA = new GameObject("BoxA");
                boxA.transform.position = new Vector3(0, 1.0f, 0);
                var colliderA = boxA.AddComponent<BoxCollider3D>();
                boxA.AddComponent<Rigidbody3D>();

                boxA.gameObject.layer = 3;

                var boxB = new GameObject("BoxA");
                boxB.transform.position = new Vector3(3, 1.0f, 0);
                var colliderB = boxB.AddComponent<BoxCollider3D>();
                boxB.AddComponent<Rigidbody3D>();

                colliderB.gameObject.layer = 4;

                var plane = new GameObject("Plane");
                var infPlane = plane.AddComponent<InfinitePlaneCollider3D>();
                infPlane.gameObject.layer = 4;

                var nextUpdate = new WaitForFixedUpdate();
                for (var i = 0; i < 60; i++)
                {
                    yield return nextUpdate;
                }

                Assert.Less(boxA.transform.position.y, 0);
                Assert.Greater(boxB.transform.position.y, 0);

                GameObject.Destroy(boxA.gameObject);
                GameObject.Destroy(boxB.gameObject);
                GameObject.Destroy(plane.gameObject);

                for (var i = 0; i < 10; i++)
                {
                    yield return nextUpdate;
                }
            }

            physicsSettings.SetAllLayersCollisionMask(true);
            PhysicsManager.RestartEngine();

            {
                var boxA = new GameObject("BoxA");
                boxA.transform.position = new Vector3(0, 1.0f, 0);
                var colliderA = boxA.AddComponent<BoxCollider3D>();
                boxA.AddComponent<Rigidbody3D>();

                boxA.gameObject.layer = 3;

                var boxB = new GameObject("BoxA");
                boxB.transform.position = new Vector3(3, 1.0f, 0);
                var colliderB = boxB.AddComponent<BoxCollider3D>();
                boxB.AddComponent<Rigidbody3D>();

                colliderB.gameObject.layer = 4;

                var plane = new GameObject("Plane");
                var infPlane = plane.AddComponent<InfinitePlaneCollider3D>();
                infPlane.gameObject.layer = 4;

                var nextUpdate = new WaitForFixedUpdate();
                for (var i = 0; i < 60; i++)
                {
                    yield return nextUpdate;
                }

                Assert.Greater(boxA.transform.position.y, 0);
                Assert.Greater(boxB.transform.position.y, 0);
            }

            var mask = physicsSettings.GetCollisionMask(-1);
            Assert.That(mask == 0);

        }

        [Test]
        public void TestCollisionSetting()
        {
            var body = CreateRigidBody();
            body.gameObject.AddComponent<BoxCollider3D>();
            var collider = body.GetComponent<BoxCollider3D>();
            var collisionSetting = collider.collisionSetting;
            collisionSetting.contactOffset = 0.01f;
            collisionSetting.separationOffset = 0.001f;
            collider.collisionSetting = collisionSetting;

            collider.contactOffset = 0.2f;
            Assert.That(collider.contactOffset == 0.2f);

            collider.separationOffset = 0.1f;
            Assert.That(collider.separationOffset == 0.1f);
        }
    }
}
