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
    internal class PhysicMaterialTests : ComponentTestScene
    {

        [Test]
        public void TestPhysicMaterialReadWrite()
        {
            var material = ScriptableObject.CreateInstance<PhysicMaterial>();
            Assert.That(material.version, Is.EqualTo(0));
            material.bounciness = -1;
            // auto clamped to 0
            Assert.That(material.bounciness, Is.EqualTo(0));
            material.bounciness = 2;
            // auto clamped to 1
            Assert.That(material.bounciness, Is.EqualTo(1));

            // negative friction will throw an exception
            Assert.That(() =>
            {
                material.SetFrictions(-1f, 1f);
            }, Throws.ArgumentException);

            // if set dynamicFriction to be greater than staticFriction, it will throw an exception
            Assert.That(() =>
           {
               material.SetFrictions(0.5f, 0.1f);
           }, Throws.ArgumentException);

            material.SetFrictions(0.1f, 0.2f);

            Assert.That(material.dynamicFriction, Is.EqualTo(0.1f));
            Assert.That(material.staticFriction, Is.EqualTo(0.2f));

            material.bounceCombine = MaterialCombine.Multiply;
            Assert.That(material.bounceCombine, Is.EqualTo(MaterialCombine.Multiply));

            material.frictionCombine = MaterialCombine.Average;
            Assert.That(material.frictionCombine, Is.EqualTo(MaterialCombine.Average));

            material.collisionEventCombine = MaterialBoolCombine.Or;
            Assert.That(material.collisionEventCombine, Is.EqualTo(MaterialBoolCombine.Or));

            material.enableCollisionEvent = false;
            Assert.That(material.enableCollisionEvent, Is.False);

            Assert.That(() =>
            {
                material.dynamicFriction = -0.1f;
            }, Throws.ArgumentException);

            Assert.That(() =>
            {
                material.staticFriction = -0.1f;
            }, Throws.ArgumentException);

            material.dynamicFriction = 0.3f;
            Assert.That(material.dynamicFriction, Is.EqualTo(0.3f));
            Assert.That(material.staticFriction, Is.EqualTo(0.3f));

            material.staticFriction = 0.2f;
            Assert.That(material.dynamicFriction, Is.EqualTo(0.2f));
            Assert.That(material.staticFriction, Is.EqualTo(0.2f));
        }

        [UnityTest]
        public IEnumerator TestPhysicMaterialOnCollider()
        {
            var sharedMaterial = ScriptableObject.CreateInstance<PhysicMaterial>();
            sharedMaterial.bounciness = 0.1f;

            var plane = new GameObject("Plane").AddComponent<BoxCollider3D>();
            plane.sharedMaterial = sharedMaterial;
            Assert.That(plane.sharedMaterial, Is.EqualTo(sharedMaterial));
            Assert.That(plane.actualMaterial, Is.EqualTo(sharedMaterial));

            var ball = CreateRigidBody(new Vector3(0, 5, 0));
            var ballCollider = ball.gameObject.AddComponent<SphereCollider3D>();
            ballCollider.sharedMaterial = sharedMaterial;

            Assert.That(ballCollider.sharedMaterial, Is.EqualTo(sharedMaterial));

            // test duplicate assignment
            ballCollider.sharedMaterial = sharedMaterial;
            // nothing changed..
            Assert.That(ballCollider.sharedMaterial, Is.EqualTo(sharedMaterial));

            yield return new WaitForFixedUpdate();

            // the modification of shared material should be synced to all the colliders that use it.
            Assert.That(ballCollider.nativeMaterial, Is.EqualTo(sharedMaterial.data));
            Assert.That(plane.nativeMaterial, Is.EqualTo(sharedMaterial.data));

            // call material property will create a new material instance and assign it to the collider.
            ballCollider.material.SetFrictions(0.1f, 0.2f);
            Assert.That(ballCollider.actualMaterial, Is.EqualTo(ballCollider.material));

            yield return new WaitForFixedUpdate();

            Assert.That(ballCollider.nativeMaterial, Is.EqualTo(ballCollider.material.data));

            // if both sharedMaterial and material are assigned to null, the default material will be used for native.
            ballCollider.sharedMaterial = null;
            ballCollider.material = null;
            yield return new WaitForFixedUpdate();
            Assert.That(ballCollider.nativeMaterial, Is.EqualTo(Internal.MaterialData.Default));

            // call material property will create a new material instance that it's data is equal to the default material.
            var instantiatedMaterial = ballCollider.material;
            Assert.That(instantiatedMaterial.data, Is.EqualTo(Internal.MaterialData.Default));
            instantiatedMaterial.bounciness = 0.1f;
            yield return new WaitForFixedUpdate();
            Assert.That(ballCollider.nativeMaterial, Is.EqualTo(instantiatedMaterial.data));

            ballCollider.material = instantiatedMaterial;
            Assert.That(ballCollider.material, Is.EqualTo(instantiatedMaterial));

            // test later awake case
            var go = new GameObject("Inactive");
            go.SetActive(false);
            var collider = go.AddComponent<BoxCollider3D>();
            collider.sharedMaterial = sharedMaterial;
            go.SetActive(true);

            Assert.That(collider.nativeMaterial, Is.EqualTo(sharedMaterial.data));
            Assert.That(collider.nativeMaterialVersion, Is.EqualTo(sharedMaterial.version));
            yield return new WaitForFixedUpdate();
        }

        [UnityTest]
        public IEnumerator TestColliderOnlyWithSharedMaterial1()
        {
            var sharedMaterial = ScriptableObject.CreateInstance<PhysicMaterial>();
            sharedMaterial.bounciness = 0.1f;

            var testObject = new GameObject("Plane");
            testObject.SetActive(false);
            var collider = testObject.AddComponent<BoxCollider3D>();
            collider.sharedMaterial = sharedMaterial;
            testObject.SetActive(true);

            yield return new WaitForFixedUpdate();

            Assert.That(collider.nativeMaterial, Is.EqualTo(sharedMaterial.data));
        }

        [UnityTest]
        public IEnumerator TestColliderOnlyWithSharedMaterial2()
        {
            var sharedMaterial = ScriptableObject.CreateInstance<PhysicMaterial>();
            sharedMaterial.bounciness = 0.1f;

            var testObject = new GameObject("Plane");
            testObject.SetActive(false);
            testObject.AddComponent<Rigidbody3D>();
            var collider = testObject.AddComponent<BoxCollider3D>();
            collider.sharedMaterial = sharedMaterial;
            testObject.SetActive(true);

            yield return new WaitForFixedUpdate();

            Assert.That(collider.nativeMaterial, Is.EqualTo(sharedMaterial.data));
        }
    }
}
