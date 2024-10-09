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
    internal class MultipleGameObjectCompositeTests : ComponentTestScene
    {
        [Test]
        public void TestPhysicalComponentCompositer()
        {
            var objectA = new GameObject("A");
            var objectB = new GameObject("B");
            var objectC = new GameObject("C");
            var objectD = new GameObject("D");

            var colliderA = objectA.AddComponent<BoxCollider3D>();
            var rigidBodyB = objectB.AddComponent<Rigidbody3D>();
            var colliderC = objectC.AddComponent<BoxCollider3D>();
            var colliderD = objectD.AddComponent<BoxCollider3D>();

            objectA.transform.parent = objectB.transform;
            objectB.transform.parent = objectC.transform;
            objectC.transform.parent = objectD.transform;

            Assert.AreEqual(colliderA.bodyId, rigidBodyB.rigidbodyId);
            Assert.AreNotEqual(colliderC.bodyId, colliderD.bodyId);
            rigidBodyB.ForeachAttachedCollider((collider) => { Assert.AreEqual(colliderA, collider); });
        }

        [UnityTest]
        public IEnumerator TestOneActorFromTwoGameObject()
        {
            var boxA = new GameObject("A");
            var boxB = new GameObject("B");

            boxA.SetActive(false);
            boxB.SetActive(false);

            var rigidbody = boxA.AddComponent<Rigidbody3D>();
            var colliderA = boxA.AddComponent<BoxCollider3D>();
            var colliderB = boxB.AddComponent<BoxCollider3D>();
            rigidbody.useGravity = false;

            boxB.transform.parent = boxA.transform;

            colliderB.transform.localPosition = Vector3.left;

            boxA.SetActive(true);
            boxB.SetActive(true);

            Assert.AreEqual(colliderA.bodyId, colliderB.bodyId);
            Assert.AreNotEqual(colliderA.id, colliderB.id);

            yield return new WaitForFixedUpdate();

            Assert.AreEqual(new Bounds(Vector3.left, Vector3.one * (1 + 2 * colliderB.contactOffset)), (Bounds)colliderB.motionAABB);
            rigidbody.ForeachAttachedCollider((collider) => { Assert.IsTrue(colliderA == collider || colliderB == collider); });
        }

        [UnityTest]
        public IEnumerator TestColliderChangeParent()
        {
            var emptyGameObjectA = new GameObject("EmptyA");

            var emptyGameObjectB = new GameObject("EmptyB");
            var emptyGameObjectBWorldPosition = new Vector3(5.0f, 0.0f, 0.0f);
            emptyGameObjectB.transform.position = emptyGameObjectBWorldPosition;

            var gameObjectWithNativeA = new GameObject("WithNativeA");
            var gameObjectWithNativeAWorldPosition = new Vector3(10.0f, 0.0f, 0.0f);
            var parentNativeA = gameObjectWithNativeA.AddComponent<Rigidbody3D>();
            gameObjectWithNativeA.transform.position = gameObjectWithNativeAWorldPosition;

            var gameObjectWithNativeB = new GameObject("WithNativeB");
            var gameObjectWithNativeBWorldPosition = new Vector3(15.0f, 0.0f, 0.0f);
            var parentNativeB = gameObjectWithNativeB.AddComponent<Rigidbody3D>();
            gameObjectWithNativeB.transform.position = gameObjectWithNativeBWorldPosition;

            var testObject = new GameObject("TestObject");
            var testObjectWorldPosition = new Vector3(-5.0f, 0.0f, 0.0f);
            testObject.transform.position = testObjectWorldPosition;
            var collider = testObject.AddComponent<BoxCollider3D>();
            var orignalBodyId = collider.bodyId;

            // without parent -> with empty parent
            testObject.transform.parent = emptyGameObjectA.transform;
            Assert.AreEqual(collider.bodyId, orignalBodyId);
            Assert.AreEqual(testObject.transform.position, testObjectWorldPosition);
            Assert.AreEqual(testObject.transform.localPosition, testObjectWorldPosition);

            // with empty parent -> with empty parent
            testObject.transform.parent = emptyGameObjectB.transform;
            Assert.AreEqual(collider.bodyId, orignalBodyId);
            Assert.AreEqual(testObject.transform.position, testObjectWorldPosition);
            Assert.AreEqual(testObject.transform.localPosition, testObjectWorldPosition - emptyGameObjectBWorldPosition);

            // with empty parent -> with a parent which has native
            testObject.transform.parent = gameObjectWithNativeA.transform;
            Assert.AreNotEqual(collider.bodyId, orignalBodyId);
            Assert.AreEqual(testObject.transform.position, testObjectWorldPosition);
            Assert.AreEqual(testObject.transform.localPosition, testObjectWorldPosition - gameObjectWithNativeAWorldPosition);
            Assert.AreEqual(parentNativeA.numShapes, 1);

            parentNativeA.ForeachAttachedCollider((childCollider) => { Assert.AreEqual(collider, childCollider); });

            orignalBodyId = collider.bodyId;

            // with a parent which has native -> with a parent which has native
            testObject.transform.parent = gameObjectWithNativeB.transform;
            Assert.AreNotEqual(collider.bodyId, orignalBodyId);
            Assert.AreEqual(testObject.transform.position, testObjectWorldPosition);
            Assert.AreEqual(testObject.transform.localPosition, testObjectWorldPosition - gameObjectWithNativeBWorldPosition);
            Assert.AreEqual(parentNativeA.numShapes, 0);
            Assert.AreEqual(parentNativeB.numShapes, 1);

            parentNativeA.ForeachAttachedCollider((childCollider) => { Assert.Fail(); });
            parentNativeB.ForeachAttachedCollider((childCollider) => { Assert.AreEqual(collider, childCollider); });

            orignalBodyId = collider.bodyId;

            // with a parent which has native -> with empty parent
            testObject.transform.parent = emptyGameObjectB.transform;
            Assert.AreNotEqual(collider.bodyId, orignalBodyId);
            Assert.AreEqual(collider.attachedRigidbody, null);
            Assert.AreEqual(testObject.transform.position, testObjectWorldPosition);
            Assert.AreEqual(testObject.transform.localPosition, testObjectWorldPosition - emptyGameObjectBWorldPosition);
            Assert.AreEqual(parentNativeB.numShapes, 0);
            parentNativeB.ForeachAttachedCollider((childCollider) => { Assert.Fail(); });

            orignalBodyId = collider.bodyId;

            // with empty parent -> without parent
            testObject.transform.parent = null;
            Assert.AreEqual(collider.bodyId, orignalBodyId);
            Assert.AreEqual(testObject.transform.position, testObjectWorldPosition);

            // without parent -> with a parent which has native
            testObject.transform.parent = gameObjectWithNativeA.transform;
            Assert.AreNotEqual(collider.bodyId, orignalBodyId);
            Assert.AreEqual(testObject.transform.position, testObjectWorldPosition);
            Assert.AreEqual(testObject.transform.localPosition, testObjectWorldPosition - gameObjectWithNativeAWorldPosition);
            Assert.AreEqual(parentNativeA.numShapes, 1);
            parentNativeA.ForeachAttachedCollider((childCollider) => { Assert.AreEqual(collider, childCollider); });

            orignalBodyId = collider.bodyId;

            //with a parent which has native -> without parent
            testObject.transform.parent = null;
            Assert.AreNotEqual(collider.bodyId, orignalBodyId);
            Assert.AreEqual(testObject.transform.position, testObjectWorldPosition);
            Assert.AreEqual(parentNativeA.numShapes, 0);

            parentNativeA.ForeachAttachedCollider((childCollider) => { Assert.Fail(); });

            yield return new WaitForFixedUpdate();
        }

        [UnityTest]
        public IEnumerator TestColliderChangeParent2()
        {
            var emptyGameObjectA = new GameObject("EmptyA");

            var emptyGameObjectB = new GameObject("EmptyB");
            var emptyGameObjectBWorldPosition = new Vector3(5.0f, 0.0f, 0.0f);
            emptyGameObjectB.transform.position = emptyGameObjectBWorldPosition;

            var gameObjectWithNativeA = new GameObject("WithNativeA");
            var gameObjectWithNativeAWorldPosition = new Vector3(10.0f, 0.0f, 0.0f);
            var parentNativeA = gameObjectWithNativeA.AddComponent<Rigidbody3D>();
            gameObjectWithNativeA.transform.position = gameObjectWithNativeAWorldPosition;

            var gameObjectWithNativeB = new GameObject("WithNativeB");
            var gameObjectWithNativeBWorldPosition = new Vector3(15.0f, 0.0f, 0.0f);
            var parentNativeB = gameObjectWithNativeB.AddComponent<Rigidbody3D>();
            gameObjectWithNativeB.transform.position = gameObjectWithNativeBWorldPosition;

            var testObject = new GameObject("TestObject");
            var testObjectWorldPosition = new Vector3(-5.0f, 0.0f, 0.0f);
            testObject.transform.position = testObjectWorldPosition;
            var collider = testObject.AddComponent<MeshCollider3D>();

            Assert.AreEqual(collider.attachedRigidbody, null);
            Assert.IsFalse(collider.isNativeValid);

            // without parent -> with empty parent
            testObject.transform.parent = emptyGameObjectA.transform;
            Assert.AreEqual(collider.attachedRigidbody, null);
            Assert.IsFalse(collider.isNativeValid);

            // with empty parent -> with empty parent
            testObject.transform.parent = emptyGameObjectB.transform;
            Assert.AreEqual(collider.attachedRigidbody, null);
            Assert.IsFalse(collider.isNativeValid);

            // with empty parent -> with a parent which has native
            testObject.transform.parent = gameObjectWithNativeA.transform;
            Assert.AreEqual(collider.attachedRigidbody, null);
            Assert.IsFalse(collider.isNativeValid);

            // with a parent which has native -> with a parent which has native
            testObject.transform.parent = gameObjectWithNativeB.transform;
            Assert.AreEqual(collider.attachedRigidbody, null);
            Assert.IsFalse(collider.isNativeValid);

            // with a parent which has native -> with empty parent
            testObject.transform.parent = emptyGameObjectB.transform;
            Assert.AreEqual(collider.attachedRigidbody, null);
            Assert.IsFalse(collider.isNativeValid);

            // with empty parent -> without parent
            testObject.transform.parent = null;
            Assert.AreEqual(collider.attachedRigidbody, null);
            Assert.IsFalse(collider.isNativeValid);

            // without parent -> with a parent which has native
            testObject.transform.parent = gameObjectWithNativeA.transform;
            Assert.AreEqual(collider.attachedRigidbody, null);
            Assert.IsFalse(collider.isNativeValid);

            //with a parent which has native -> without parent
            testObject.transform.parent = null;
            Assert.AreEqual(collider.attachedRigidbody, null);
            Assert.IsFalse(collider.isNativeValid);

            yield return new WaitForFixedUpdate();
        }

        [UnityTest]
        public IEnumerator TestRigidBody3DChangeParent()
        {
            var emptyGameObjectA = new GameObject("EmptyA");

            var emptyGameObjectB = new GameObject("EmptyB");
            var emptyGameObjectBWorldPosition = new Vector3(5.0f, 0.0f, 0.0f);
            emptyGameObjectB.transform.position = emptyGameObjectBWorldPosition;

            var gameObjectWithNativeA = new GameObject("WithNativeA");
            var gameObjectWithNativeAWorldPosition = new Vector3(10.0f, 0.0f, 0.0f);
            var parentNativeA = gameObjectWithNativeA.AddComponent<Rigidbody3D>();
            gameObjectWithNativeA.transform.position = gameObjectWithNativeAWorldPosition;

            var gameObjectWithNativeB = new GameObject("WithNativeB");
            var gameObjectWithNativeBWorldPosition = new Vector3(15.0f, 0.0f, 0.0f);
            var parentNativeB = gameObjectWithNativeB.AddComponent<Rigidbody3D>();
            gameObjectWithNativeB.transform.position = gameObjectWithNativeBWorldPosition;

            var testObject = new GameObject("TestObject");
            var testObjectWorldPosition = new Vector3(-5.0f, 0.0f, 0.0f);
            testObject.transform.position = testObjectWorldPosition;
            var collider = testObject.AddComponent<BoxCollider3D>();
            var orignalBodyId = collider.bodyId;
            testObject.AddComponent<Rigidbody3D>();
            Assert.AreNotEqual(collider.bodyId, orignalBodyId);
            orignalBodyId = collider.bodyId;

            // without parent -> with empty parent
            testObject.transform.parent = emptyGameObjectA.transform;
            Assert.AreEqual(collider.bodyId, orignalBodyId);
            Assert.AreEqual(testObject.transform.position, testObjectWorldPosition);
            Assert.AreEqual(testObject.transform.localPosition, testObjectWorldPosition);

            // with empty parent -> with empty parent
            testObject.transform.parent = emptyGameObjectB.transform;
            Assert.AreEqual(collider.bodyId, orignalBodyId);
            Assert.AreEqual(testObject.transform.position, testObjectWorldPosition);
            Assert.AreEqual(testObject.transform.localPosition, testObjectWorldPosition - emptyGameObjectBWorldPosition);

            // with empty parent -> with a parent which has native
            testObject.transform.parent = gameObjectWithNativeA.transform;
            Assert.AreEqual(collider.bodyId, orignalBodyId);
            Assert.AreEqual(testObject.transform.position, testObjectWorldPosition);
            Assert.AreEqual(testObject.transform.localPosition, testObjectWorldPosition - gameObjectWithNativeAWorldPosition);
            Assert.AreEqual(parentNativeA.numShapes, 0);
            parentNativeA.ForeachAttachedCollider((childCollider) => { Assert.Fail(); });

            // with a parent which has native -> with a parent which has native
            testObject.transform.parent = gameObjectWithNativeB.transform;
            Assert.AreEqual(collider.bodyId, orignalBodyId);
            Assert.AreEqual(testObject.transform.position, testObjectWorldPosition);
            Assert.AreEqual(testObject.transform.localPosition, testObjectWorldPosition - gameObjectWithNativeBWorldPosition);
            Assert.AreEqual(parentNativeA.numShapes, 0);
            parentNativeA.ForeachAttachedCollider((childCollider) => { Assert.Fail(); });

            // with a parent which has native -> with empty parent
            testObject.transform.parent = emptyGameObjectB.transform;
            Assert.AreEqual(collider.bodyId, orignalBodyId);
            Assert.AreEqual(testObject.transform.position, testObjectWorldPosition);
            Assert.AreEqual(testObject.transform.localPosition, testObjectWorldPosition - emptyGameObjectBWorldPosition);
            Assert.AreEqual(parentNativeB.numShapes, 0);
            parentNativeA.ForeachAttachedCollider((childCollider) => { Assert.Fail(); });

            // with empty parent -> without parent
            testObject.transform.parent = null;
            Assert.AreEqual(collider.bodyId, orignalBodyId);
            Assert.AreEqual(testObject.transform.position, testObjectWorldPosition);

            // without parent -> with a parent which has native
            testObject.transform.parent = gameObjectWithNativeA.transform;
            Assert.AreEqual(collider.bodyId, orignalBodyId);
            Assert.AreEqual(testObject.transform.position, testObjectWorldPosition);
            Assert.AreEqual(testObject.transform.localPosition, testObjectWorldPosition - gameObjectWithNativeAWorldPosition);
            Assert.AreEqual(parentNativeA.numShapes, 0);
            parentNativeA.ForeachAttachedCollider((childCollider) => { Assert.Fail(); });
            //with a parent which has native -> without parent
            testObject.transform.parent = null;
            Assert.AreEqual(collider.bodyId, orignalBodyId);
            Assert.AreEqual(testObject.transform.position, testObjectWorldPosition);
            Assert.AreEqual(parentNativeA.numShapes, 0);
            parentNativeA.ForeachAttachedCollider((childCollider) => { Assert.Fail(); });

            yield return new WaitForFixedUpdate();
        }

        // child (collider) -> parent (collider) -> ancestorA (collider)
        // to
        // child (collider) -> parent (collider) -> ancestorB (empty)
        // to
        // child (collider) -> parent (collider) -> ancestorA (collider)
        [Test]
        public void TestColliderChangeAncestor1()
        {
            var parentObject = new GameObject("parentObject");
            var parentCollider = parentObject.AddComponent<BoxCollider3D>();

            var ancestorObjectA = new GameObject("ancestorA");
            var ancestorColliderA = ancestorObjectA.AddComponent<BoxCollider3D>();
            parentObject.transform.parent = ancestorObjectA.transform;

            var ancestorObjectB = new GameObject("ancestorB");

            var testObject = new GameObject("testObject");
            var collider = testObject.AddComponent<BoxCollider3D>();
            testObject.transform.parent = parentObject.transform;

            Assert.AreNotEqual(collider.bodyId, parentCollider.bodyId);
            Assert.AreNotEqual(collider.bodyId, ancestorColliderA.bodyId);

            var originalId = collider.bodyId;

            parentObject.transform.parent = ancestorObjectB.transform;

            Assert.AreEqual(collider.bodyId, originalId);
            Assert.AreNotEqual(collider.bodyId, ancestorColliderA.bodyId);
            Assert.AreNotEqual(collider.bodyId, parentCollider.bodyId);
            Assert.AreEqual(1, ancestorColliderA.handle.numShapes);
            Assert.AreEqual(1, parentCollider.handle.numShapes);

            parentObject.transform.parent = ancestorObjectA.transform;

            Assert.AreEqual(collider.bodyId, originalId);
        }

        // child (collider) -> parent (collider) -> ancestorA (collider)
        // to
        // child (collider) -> parent (collider) -> ancestorB (collider)
        [Test]
        public void TestColliderChangeAncestor2()
        {
            var parentObject = new GameObject("parentObject");
            var parentCollider = parentObject.AddComponent<BoxCollider3D>();

            var ancestorObjectA = new GameObject("ancestorA");
            var ancestorColliderA = ancestorObjectA.AddComponent<BoxCollider3D>();
            parentObject.transform.parent = ancestorObjectA.transform;

            var ancestorObjectB = new GameObject("ancestorB");
            var ancestorColliderB = ancestorObjectB.AddComponent<BoxCollider3D>();

            var testObject = new GameObject("testObject");
            var collider = testObject.AddComponent<BoxCollider3D>();
            testObject.transform.parent = parentObject.transform;

            Assert.AreNotEqual(collider.bodyId, parentCollider.bodyId);
            Assert.AreNotEqual(collider.bodyId, ancestorColliderA.bodyId);
            Assert.AreEqual(1, ancestorColliderA.handle.numShapes);
            Assert.AreEqual(1, ancestorColliderB.handle.numShapes);

            parentObject.transform.parent = ancestorObjectB.transform;

            Assert.AreNotEqual(collider.bodyId, ancestorColliderA.bodyId);
            Assert.AreNotEqual(collider.bodyId, parentCollider.bodyId);
            Assert.AreNotEqual(collider.bodyId, ancestorColliderB.bodyId);
            Assert.AreEqual(1, ancestorColliderB.handle.numShapes);
            Assert.AreEqual(1, ancestorColliderA.handle.numShapes);
        }

        // child (collider) -> parent (collider) -> ancestorA (collider)
        // to
        // child (collider) -> parent (collider) -> ancestorB (rigidbody)
        [Test]
        public void TestColliderChangeAncestor3()
        {
            var parentObject = new GameObject("parentObject");
            var parentCollider = parentObject.AddComponent<BoxCollider3D>();

            var ancestorObjectA = new GameObject("ancestorA");
            var ancestorColliderA = ancestorObjectA.AddComponent<BoxCollider3D>();
            parentObject.transform.parent = ancestorObjectA.transform;

            var ancestorObjectB = new GameObject("ancestorB");
            var ancestorRigidBody = ancestorObjectB.AddComponent<Rigidbody3D>();

            var testObject = new GameObject("testObject");
            var collider = testObject.AddComponent<BoxCollider3D>();
            testObject.transform.parent = parentObject.transform;

            Assert.AreNotEqual(collider.bodyId, parentCollider.bodyId);
            Assert.AreNotEqual(collider.bodyId, ancestorColliderA.bodyId);
            Assert.AreEqual(1, ancestorColliderA.handle.numShapes);

            parentObject.transform.parent = ancestorObjectB.transform;

            Assert.AreNotEqual(collider.bodyId, ancestorColliderA.bodyId);
            Assert.AreEqual(collider.bodyId, parentCollider.bodyId);
            Assert.AreEqual(collider.bodyId, ancestorRigidBody.rigidbodyId);
            Assert.AreEqual(2, ancestorRigidBody.numShapes);
            Assert.AreEqual(1, ancestorColliderA.handle.numShapes);
        }

        // child (collider) -> parent (rigidbody) -> ancestorA (collider)
        // to
        // child (collider) -> parent (rigidbody) -> ancestorB (collider)
        [Test]
        public void TestColliderChangeAncestor4()
        {
            var parentObject = new GameObject("parentObject");
            var parentRigidBody = parentObject.AddComponent<Rigidbody3D>();

            var ancestorObjectA = new GameObject("ancestorA");
            var ancestorColliderA = ancestorObjectA.AddComponent<BoxCollider3D>();
            parentObject.transform.parent = ancestorObjectA.transform;

            var ancestorObjectB = new GameObject("ancestorB");
            var ancestorColliderB = ancestorObjectB.AddComponent<BoxCollider3D>();

            var testObject = new GameObject("testObject");
            var collider = testObject.AddComponent<BoxCollider3D>();
            testObject.transform.parent = parentObject.transform;
            var originalId = collider.bodyId;

            Assert.AreEqual(collider.bodyId, parentRigidBody.rigidbodyId);
            Assert.AreNotEqual(collider.bodyId, ancestorColliderA.bodyId);

            parentObject.transform.parent = ancestorObjectB.transform;

            Assert.AreNotEqual(collider.bodyId, ancestorColliderA.bodyId);
            Assert.AreNotEqual(collider.bodyId, ancestorColliderB.bodyId);
            Assert.AreEqual(collider.bodyId, parentRigidBody.rigidbodyId);
            Assert.AreEqual(originalId, collider.bodyId);
            Assert.AreEqual(1, ancestorColliderA.handle.numShapes);
            Assert.AreEqual(1, ancestorColliderB.handle.numShapes);
        }

        // collider -> rigidbody
        [Test]
        public void TestDestroyParent1()
        {
            var parentObject = new GameObject("parentObject");
            var rigidBody = parentObject.AddComponent<Rigidbody3D>();

            var testObject = new GameObject("testObject");
            var collider = testObject.AddComponent<BoxCollider3D>();
            testObject.transform.parent = parentObject.transform;
            var originalId = collider.bodyId;

            Assert.AreEqual(collider.bodyId, rigidBody.rigidbodyId);

            GameObject.DestroyImmediate(rigidBody);

            Assert.AreNotEqual(collider.bodyId, originalId);
        }

        // collider -> rigidbody -> rigidbody
        [Test]
        public void TestDestroyParent2()
        {
            var parentObject = new GameObject("parentObject");
            var parentRigidBody = parentObject.AddComponent<Rigidbody3D>();

            var ancestorObject = new GameObject("ancestor");
            var ancestorRigidBody = ancestorObject.AddComponent<Rigidbody3D>();
            parentObject.transform.parent = ancestorObject.transform;

            var testObject = new GameObject("testObject");
            var collider = testObject.AddComponent<BoxCollider3D>();
            testObject.transform.parent = parentObject.transform;
            var originalId = collider.bodyId;

            Assert.AreEqual(collider.bodyId, parentRigidBody.rigidbodyId);

            GameObject.DestroyImmediate(parentRigidBody);

            Assert.AreNotEqual(collider.bodyId, originalId);
            Assert.AreEqual(collider.bodyId, ancestorRigidBody.rigidbodyId);
            Assert.AreEqual(1, ancestorRigidBody.numShapes);

            BaseCollider.EachActiveCollider((c, _) => { Assert.AreEqual(c, collider); }, 0);
        }

        // collider -> rigidbody -> collider
        [Test]
        public void TestDestroyParent3()
        {
            var parentObject = new GameObject("parentObject");
            var parentRigidBody = parentObject.AddComponent<Rigidbody3D>();

            var ancestorObject = new GameObject("ancestor");
            var ancestorCollider = ancestorObject.AddComponent<BoxCollider3D>();
            parentObject.transform.parent = ancestorObject.transform;

            var testObject = new GameObject("testObject");
            var collider = testObject.AddComponent<BoxCollider3D>();
            testObject.transform.parent = parentObject.transform;
            var originalId = collider.bodyId;

            Assert.AreEqual(collider.bodyId, parentRigidBody.rigidbodyId);

            GameObject.DestroyImmediate(parentRigidBody);

            Assert.AreNotEqual(collider.bodyId, originalId);
            Assert.AreNotEqual(collider.bodyId, ancestorCollider.bodyId);
            Assert.AreEqual(collider.attachedRigidbody, null);
        }

        [UnityTest]
        public IEnumerator TestAttachToRigidbodyBeingDestroyed()
        {
            var parentA = new GameObject("parentA");
            var parentRigidBodyA = parentA.AddComponent<Rigidbody3D>();

            var parentB = new GameObject("parentB");
            var parentRigidBodyB = parentB.AddComponent<Rigidbody3D>();

            var testObject = new GameObject("testObject");
            var collider = testObject.AddComponent<BoxCollider3D>();
            testObject.transform.parent = parentA.transform;

            Assert.AreEqual(collider.bodyId, parentRigidBodyA.rigidbodyId);
            var originalId = collider.bodyId;

            GameObject.Destroy(parentRigidBodyB);
            testObject.transform.parent = parentB.transform;

            Assert.AreNotEqual(collider.bodyId, originalId);
            Assert.AreEqual(null, collider.attachedRigidbody);
            yield return new WaitForFixedUpdate();
        }

        [Test]
        public void TestEnableColliderWhenRigidbodyBeingDestroyed()
        {
            var testObject = new GameObject("testObject");
            testObject.SetActive(false);
            var collider = testObject.AddComponent<BoxCollider3D>();
            collider.enabled = false;
            var rigidbody = testObject.AddComponent<Rigidbody3D>();
            testObject.SetActive(true);

            GameObject.Destroy(rigidbody);

            collider.enabled = true;

            Assert.AreEqual(null, collider.attachedRigidbody);
            Assert.IsTrue(collider.isNativeValid);
        }

        [Test]
        public void TestEmptyParentObtainNative1()
        {
            var parentObject = new GameObject("parent");
            var ancestorObject = new GameObject("ancestor");
            var testObject = new GameObject("testObject");
            var ancestorCollider = ancestorObject.AddComponent<BoxCollider3D>();

            parentObject.transform.parent = ancestorObject.transform;

            testObject.transform.parent = parentObject.transform;
            var collider = testObject.AddComponent<BoxCollider3D>();
            Assert.AreNotEqual(collider.bodyId, ancestorCollider.bodyId);

            var parentRigidBody = parentObject.AddComponent<Rigidbody3D>();

            Assert.AreEqual(collider.bodyId, parentRigidBody.rigidbodyId);
        }

        [Test]
        public void TestEmptyParentObtainNative2()
        {
            var parentObject = new GameObject("parent");
            var testObject = new GameObject("testObject");

            testObject.transform.parent = parentObject.transform;
            var collider = testObject.AddComponent<BoxCollider3D>();

            var parentRigidBody = parentObject.AddComponent<Rigidbody3D>();

            Assert.AreEqual(collider.bodyId, parentRigidBody.rigidbodyId);
        }

        [Test]
        public void TestDisableColliderTransformParentChanged()
        {
            var parentObject = new GameObject("parentObject");
            parentObject.SetActive(false);
            var parentRigidBody = parentObject.AddComponent<Rigidbody3D>();
            var collider = parentObject.AddComponent<BoxCollider3D>();
            collider.enabled = false;

            parentObject.SetActive(true);

            var newParentObject = new GameObject("NewParentObject");
            collider.transform.parent = newParentObject.transform;

            Assert.IsFalse(collider.isNativeValid);
            Assert.AreEqual(0, parentRigidBody.numShapes);
        }

        [Test]
        public void TestDisableColliderTransformParentChanged2()
        {
            var parentObject = new GameObject("parentObject");
            parentObject.SetActive(false);
            var rigidbody = parentObject.AddComponent<Rigidbody3D>();

            var childObject = new GameObject("childObject");
            childObject.SetActive(false);

            var collider = childObject.AddComponent<BoxCollider3D>();
            collider.enabled = false;

            parentObject.SetActive(true);
            childObject.SetActive(true);

            childObject.transform.parent = null;

            Assert.IsFalse(collider.isNativeValid);
            Assert.AreEqual(0, rigidbody.numShapes);
        }

        [UnityTest, Ignore("The transform of the child GameObject is affected by the parent. (#690)")]
        public IEnumerator TestAddAndRemovalOnChildCollider()
        {
            var parentObject = new GameObject("parent");
            var testObject = new GameObject("testObject");
            parentObject.SetActive(false);
            testObject.SetActive(false);

            testObject.transform.parent = parentObject.transform;

            testObject.transform.localPosition = new Vector3(1.0f, 1.0f, 0.0f);

            parentObject.transform.position = new Vector3(0.0f, 5.0f, 0.0f);

            var collider = testObject.AddComponent<BoxCollider3D>();

            var parentRigidBody = parentObject.AddComponent<Rigidbody3D>();

            parentObject.SetActive(true);
            testObject.SetActive(true);

            var plane = new GameObject("Plane").AddComponent<BoxCollider3D>();
            plane.size = new Vector3(100.0f, 1.0f, 100.0f);

            for (var i = 0; i < 50; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.IsTrue(collider.transform.position.y > 0.0f);

            var rigidBody = testObject.AddComponent<Rigidbody3D>();

            Assert.AreEqual(collider.bodyId, rigidBody.rigidbodyId);
            Assert.AreNotEqual(collider.bodyId, parentRigidBody.rigidbodyId);
            Assert.AreEqual(1, rigidBody.numShapes);
            Assert.AreEqual(0, parentRigidBody.numShapes);

            for (var i = 0; i < 50; i++)
            {
                yield return new WaitForFixedUpdate();
                UnityEngine.Debug.Log("collider y: " + collider.transform.position.y);
                UnityEngine.Debug.Log("collider aabb.min.y: " + collider.motionAABB.min.y);
                UnityEngine.Debug.Log("collider native y: " + collider.handle.transform.position.y);
            }

            Assert.IsTrue(collider.transform.position.y > 0.0f);

            Object.Destroy(rigidBody);

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            Assert.AreEqual(collider.bodyId, parentRigidBody.rigidbodyId);
            Assert.AreEqual(collider.attachedRigidbody, parentRigidBody);
            Assert.AreEqual(1, parentRigidBody.numShapes);
        }

        [UnityTest]
        public IEnumerator TestAddAndRemovalOnChildCollider2()
        {
            var parentObject = new GameObject("parent");
            var testObject = new GameObject("testObject");
            parentObject.SetActive(false);
            testObject.SetActive(false);

            testObject.transform.parent = parentObject.transform;

            testObject.transform.localPosition = new Vector3(1.0f, 1.0f, 0.0f);

            parentObject.transform.position = new Vector3(0.0f, 5.0f, 0.0f);

            var collider = testObject.AddComponent<BoxCollider3D>();

            var parentRigidBody = parentObject.AddComponent<Rigidbody3D>();
            parentObject.AddComponent<BoxCollider3D>();

            parentObject.SetActive(true);
            testObject.SetActive(true);

            var plane = new GameObject("Plane").AddComponent<BoxCollider3D>();
            plane.size = new Vector3(100.0f, 1.0f, 100.0f);

            for (var i = 0; i < 30; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.IsTrue(collider.transform.position.y > 0.0f);

            var rigidBody = testObject.AddComponent<Rigidbody3D>();

            Assert.AreEqual(collider.bodyId, rigidBody.rigidbodyId);
            Assert.AreNotEqual(collider.bodyId, parentRigidBody.rigidbodyId);
            Assert.AreEqual(1, rigidBody.numShapes);
            Assert.AreEqual(1, parentRigidBody.numShapes);

            for (var i = 0; i < 30; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.IsTrue(collider.transform.position.y > 0.0f);

            Object.Destroy(rigidBody);

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            Assert.AreEqual(collider.bodyId, parentRigidBody.rigidbodyId);
            Assert.AreEqual(collider.attachedRigidbody, parentRigidBody);
            Assert.AreEqual(2, parentRigidBody.numShapes);
        }

        [Test]
        public void TestParentObtainRigidBody()
        {
            var boxA = new GameObject("A");
            var boxB = new GameObject("B");

            boxA.SetActive(false);
            boxB.SetActive(false);

            var colliderA = boxA.AddComponent<BoxCollider3D>();
            var colliderB = boxB.AddComponent<BoxCollider3D>();

            boxB.transform.parent = boxA.transform;

            boxA.SetActive(true);
            boxB.SetActive(true);

            var rigidBody = boxA.AddComponent<Rigidbody3D>();
            Assert.AreEqual(colliderB.attachedRigidbody, rigidBody);
            Assert.AreEqual(colliderA.bodyId, colliderB.bodyId);
            Assert.AreEqual(colliderA.bodyId, rigidBody.rigidbodyId);
        }

        [UnityTest]
        public IEnumerator TestShapeTransform()
        {
            var boxA = new GameObject("A");
            var boxB = new GameObject("B");

            boxA.AddComponent<Rigidbody3D>();
            var colliderA = boxA.AddComponent<BoxCollider3D>();
            var colliderB = boxB.AddComponent<BoxCollider3D>();

            boxB.transform.parent = boxA.transform;

            colliderB.transform.localPosition = new Vector3(1.0f, 1.0f, 0.0f);

            colliderB.shapeRotation = Quaternion.AngleAxis(45.0f, Vector3.up);

            yield return new WaitForFixedUpdate();

            var actualBounds = colliderB.motionAABB;
            var expectedBounds = new Bounds(new Vector3(1.0f, 1.0f, 0.0f), new Vector3(Mathf.Sqrt(2 * (1 + 4 * colliderB.contactOffset)), 1.0f + 2 * colliderB.contactOffset, Mathf.Sqrt(2 * (1 + 4 * colliderB.contactOffset))));

            Assert.AreEqual(expectedBounds.size.x, actualBounds.size.x, 0.001);
            Assert.AreEqual(expectedBounds.size.y, actualBounds.size.y, 0.001);
            Assert.AreEqual(expectedBounds.size.z, actualBounds.size.z, 0.001);
        }

        [UnityTest]
        public IEnumerator TestIterChildRenderer()
        {
            // A (rigidbody and renderer)
            // |\
            // B C    (B: only renderer, C: collider and renderer)
            // | |
            // D E    (D: rigidbody and collider, E: collider and renderer)
            // |
            // F (only renderer)

            var objectA = new GameObject("A");
            var objectB = new GameObject("B");
            var objectC = new GameObject("C");
            var objectD = new GameObject("D");
            var objectE = new GameObject("E");
            var objectF = new GameObject("F");

            var rigidbodyA = objectA.AddComponent<Rigidbody3D>();
            var rigidbodyD = objectD.AddComponent<Rigidbody3D>();

            var rendererA = objectA.AddComponent<MeshRenderer>();
            var rendererB = objectB.AddComponent<MeshRenderer>();
            var rendererC = objectC.AddComponent<MeshRenderer>();
            var rendererE = objectE.AddComponent<MeshRenderer>();
            var rendererF = objectF.AddComponent<MeshRenderer>();

            var colliderC = objectC.AddComponent<BoxCollider3D>();
            var colliderD = objectD.AddComponent<BoxCollider3D>();
            var colliderE = objectE.AddComponent<BoxCollider3D>();

            objectF.transform.parent = objectD.transform;
            objectD.transform.parent = objectB.transform;
            objectB.transform.parent = objectA.transform;
            objectE.transform.parent = objectC.transform;
            objectC.transform.parent = objectA.transform;

            yield return new WaitForFixedUpdate();

            var numRenderer = 0;
            foreach (var renderer in rigidbodyA.IterChildRenderer())
            {
                Assert.IsTrue(renderer == rendererA || renderer == rendererB || renderer == rendererC || renderer == rendererE);
                Assert.AreNotEqual(rendererF, renderer);
                numRenderer++;
            }

            Assert.AreEqual(4, numRenderer);

            foreach (var renderer in rigidbodyD.IterChildRenderer())
            {
                Assert.AreEqual(rendererF, renderer);
            }
        }
    }
}
