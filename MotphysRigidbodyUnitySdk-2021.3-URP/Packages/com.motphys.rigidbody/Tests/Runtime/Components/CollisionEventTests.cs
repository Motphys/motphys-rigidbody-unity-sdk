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
using Unity.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace Motphys.Rigidbody.Tests
{
    internal class CollisionEventTests : ComponentTestScene
    {

        [UnityTest]
        public IEnumerator TestCollisionEvent()
        {
            static void OnCollisionEnter(CollisionEvent evt)
            {
                PhysicsManager.onCollisionEnter -= OnCollisionEnter;
                Assert.That(evt.contactCount, Is.EqualTo(4));
                Assert.That(evt.GetContact(0).separation, Is.EqualTo(-0.8f).Using(new FloatEqualityComparer(0.1f)));
            }

            PhysicsManager.onCollisionEnter += OnCollisionEnter;
            var bodyA = CreateRigidBody(new Vector3(-2f, 0, 0));
            bodyA.enablePostTransformControl = true;
            var colliderA = bodyA.gameObject.AddComponent<BoxCollider3D>();
            var receiveOnCollisionEnterA = false;
            var receiveOnCollisionEnterB = false;
            var receiveOnCollisionExitA = false;
            var receiveOnCollisionExitB = false;

            colliderA.onCollisionEnter += (evt) =>
            {
                receiveOnCollisionEnterA = true;
                Assert.That(evt.contactCount, Is.EqualTo(4));
                Assert.That(evt.GetContact(0).separation, Is.EqualTo(-0.8f).Using(new FloatEqualityComparer(1e-3f)));
                Assert.That(() =>
                {
                    evt.GetContact(4);
                }, Throws.TypeOf<System.ArgumentOutOfRangeException>());
                Assert.That(() =>
                {
                    evt.GetContact(-1);
                }, Throws.TypeOf<System.ArgumentOutOfRangeException>());
            };
            colliderA.onCollisionExit += (evt) =>
            {
                receiveOnCollisionExitA = true;
            };

            var bodyB = CreateRigidBody(new Vector3(2f, 0, 0));
            bodyB.enablePostTransformControl = true;
            var colliderB = bodyB.gameObject.AddComponent<BoxCollider3D>();
            colliderB.size = new Vector3(1f, 5f, 5f);
            colliderB.onCollisionEnter += (evt) =>
            {
                receiveOnCollisionEnterB = true;
                Assert.That(evt.collider, Is.EqualTo(colliderA));
                Assert.That(evt.transform, Is.EqualTo(colliderA.transform));
                Assert.That(evt.gameObject, Is.EqualTo(colliderA.gameObject));
                Assert.That(evt.rigidbody, Is.EqualTo(bodyA));
            };
            colliderB.onCollisionExit += (evt) =>
            {
                receiveOnCollisionExitB = true;
            };
            yield return new WaitForFixedUpdate();

            bodyA.transform.position = new Vector3(-0.1f, 0, 0);
            bodyB.transform.position = new Vector3(0.1f, 0, 0);

            yield return new WaitForFixedUpdate();

            Assert.IsTrue(receiveOnCollisionEnterA);
            Assert.IsTrue(receiveOnCollisionEnterB);
            Assert.That(PhysicsManager.defaultWorld.QueryAllManifoldsCount(), Is.EqualTo(1));
            var manifolds = new NativeArray<ContactManifold>(1, Allocator.Temp);
            PhysicsManager.defaultWorld.QueryAllManifolds(manifolds);
            Assert.That(manifolds[0].colliderA == colliderA.id);
            Assert.That(manifolds[0].colliderB == colliderB.id);

            bodyA.transform.position = new Vector3(-2f, 0, 0);
            bodyB.transform.position = new Vector3(2f, 0, 0);

            yield return new WaitForFixedUpdate();

            Assert.IsTrue(receiveOnCollisionExitA);
            Assert.IsTrue(receiveOnCollisionExitB);
            PhysicsManager.onCollisionEnter -= OnCollisionEnter;
            Assert.That(PhysicsManager.defaultWorld.QueryAllManifoldsCount(), Is.EqualTo(0));
        }

        [UnityTest]
        public IEnumerator TestCollisionEnterMultiSubstep()
        {
            PhysicsManager.numSubstep = 4;

            var bodyA = CreateRigidBody(new Vector3(-2f, 0, 0));
            bodyA.enablePostTransformControl = true;
            var colliderA = bodyA.gameObject.AddComponent<BoxCollider3D>();
            var receiveOnCollisionEnterA = false;
            var receiveOnCollisionEnterB = false;
            var receiveOnCollisionExitA = false;
            var receiveOnCollisionExitB = false;

            colliderA.onCollisionEnter += (evt) =>
            {
                receiveOnCollisionEnterA = true;
            };
            colliderA.onCollisionExit += (evt) =>
            {
                receiveOnCollisionExitA = true;
            };

            var bodyB = CreateRigidBody(new Vector3(2f, 0, 0));
            bodyB.enablePostTransformControl = true;
            var colliderB = bodyB.gameObject.AddComponent<BoxCollider3D>();
            colliderB.size = new Vector3(1f, 5f, 5f);
            colliderB.onCollisionEnter += (evt) =>
            {
                receiveOnCollisionEnterB = true;
            };
            colliderB.onCollisionExit += (evt) =>
            {
                receiveOnCollisionExitB = true;
            };
            yield return new WaitForFixedUpdate();

            bodyA.transform.position = new Vector3(-0.1f, 0, 0);
            bodyB.transform.position = new Vector3(0.1f, 0, 0);

            yield return new WaitForFixedUpdate();

            Assert.IsTrue(receiveOnCollisionEnterA);
            Assert.IsTrue(receiveOnCollisionEnterB);
            bodyA.transform.position = new Vector3(-2f, 0, 0);
            bodyB.transform.position = new Vector3(2f, 0, 0);

            yield return new WaitForFixedUpdate();

            Assert.IsTrue(receiveOnCollisionExitA);
            Assert.IsTrue(receiveOnCollisionExitB);
        }

        [UnityTest]
        public IEnumerator TestDeactivateOnCollisionEnter()
        {
            var staticBox = new GameObject("Box").AddComponent<BoxCollider3D>();
            var onCollisionEnterCount = 0;
            staticBox.onCollisionEnter += (evt) =>
            {
                staticBox.gameObject.SetActive(false);
                onCollisionEnterCount++;
            };

            var bodyA = CreateRigidBody(new Vector3(0, 0, 0)).gameObject.AddComponent<BoxCollider3D>();
            var bodyB = CreateRigidBody(new Vector3(0, 0, 0)).gameObject.AddComponent<BoxCollider3D>();
            yield return new WaitForFixedUpdate();
            Assert.That(onCollisionEnterCount, Is.EqualTo(2));
        }

        [UnityTest]
        public IEnumerator TestDisableOnTriggerEnter()
        {
            var sphere = new GameObject("sphere");
            sphere.transform.position = new Vector3(0.0f, 2.0f, 0.0f);
            sphere.SetActive(false);
            sphere.AddComponent<Rigidbody3D>();
            var collider = sphere.AddComponent<SphereCollider3D>();
            collider.isTrigger = true;
            sphere.SetActive(true);

            var onTriggerEnterCount = 0;
            collider.onTriggerEnter += (evt) =>
            {
                onTriggerEnterCount++;
                collider.enabled = false;
            };

            var box = new GameObject("Box").AddComponent<BoxCollider3D>();

            for (var i = 0; i < 30; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.That(onTriggerEnterCount, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator TestIgnoreLayerCollision()
        {
            var nextFrame = new WaitForFixedUpdate();

            PhysicsManager.IgnoreLayerCollision(0, 5, false);
            PhysicsManager.IgnoreLayerCollision(0, 6, false);

            PhysicsManager.IgnoreLayerCollision(5, 6, false);

            var plane = new GameObject("Plane");
            plane.AddComponent<InfinitePlaneCollider3D>();
            plane.layer = 0;

            var box0 = new GameObject("Box0");
            box0.layer = 5;

            box0.transform.position = new Vector3(0, 4, 0);
            box0.AddComponent<BoxCollider3D>();
            box0.AddComponent<Rigidbody3D>();

            var box1 = new GameObject("Box1");
            box1.layer = 6;

            box1.transform.position = new Vector3(0, 6, 0);
            box1.AddComponent<BoxCollider3D>();
            box1.AddComponent<Rigidbody3D>();

            for (var i = 0; i < 60; i++)
            {
                yield return nextFrame;
            }

            var finalY = box1.transform.position.y;
            Assert.That(finalY > 1.4f);

            yield return nextFrame;
            PhysicsManager.IgnoreLayerCollision(5, 6, true);

            var box3 = new GameObject("Box3");
            box3.layer = 5;

            box3.transform.position = new Vector3(3, 4, 0);
            box3.AddComponent<BoxCollider3D>();
            box3.AddComponent<Rigidbody3D>();

            var box4 = new GameObject("Box4");
            box4.layer = 6;

            box4.transform.position = new Vector3(3, 6, 0);
            box4.AddComponent<BoxCollider3D>();
            box4.AddComponent<Rigidbody3D>();

            for (var i = 0; i < 60; i++)
            {
                yield return nextFrame;
            }

            finalY = box4.transform.position.y;
            Assert.That(finalY < 0.6f);

            var flag = PhysicsManager.IgnoreLayerCollision(-1, 458, true);
            Assert.That(!flag);

            yield return nextFrame;
        }
    }
}
