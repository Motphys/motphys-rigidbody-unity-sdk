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

    internal class SceneQueryComponentTests : ComponentTestScene
    {
        [UnitySetUp]
        protected override IEnumerator SetUp()
        {
            yield return base.SetUp();
            PhysicsManager.numSubstep = 1;
            PhysicsManager.defaultSolverIterations = 1;
        }

        [UnityTest]
        public IEnumerator TestHitPoint()
        {
            var box = new GameObject("Sphere").AddComponent<SphereCollider3D>();

            yield return new WaitForFixedUpdate();

            var isHit = PhysicsManager.RaycastClosest(new Vector3(0.0f, 0.0f, -10.0f), Vector3.forward, out var hit, 1000.0f);
            Assert.IsTrue(isHit);
            Assert.That(hit.point, Is.EqualTo(new Vector3(0.0f, 0.0f, -0.5f)));

            isHit = PhysicsManager.RaycastAny(new Vector3(0.0f, 0.0f, -10.0f), Vector3.forward, out hit, 1000.0f);
            Assert.IsTrue(isHit);
            Assert.That(hit.point, Is.EqualTo(new Vector3(0.0f, 0.0f, -0.5f)));

            var results = new RaycastHit[1];
            var numHits = PhysicsManager.RaycastAllNonAlloc(new Vector3(0.0f, 0.0f, -10.0f), Vector3.forward, results, 1000.0f);
            Assert.That(numHits, Is.EqualTo(1));
            Assert.That(results[0].point, Is.EqualTo(new Vector3(0.0f, 0.0f, -0.5f)));
        }

        [UnityTest]
        public IEnumerator TestMultiColliders()
        {
            var gameObject = new GameObject("Box");
            gameObject.SetActive(false);

            var boxA = gameObject.AddComponent<BoxCollider3D>();
            var boxB = gameObject.AddComponent<BoxCollider3D>();

            boxA.shapeTranslation = Vector3.left;
            boxB.shapeTranslation = Vector3.right;

            gameObject.SetActive(true);

            yield return new WaitForFixedUpdate();

            var isHit = PhysicsManager.RaycastClosest(new Vector3(-1.0f, 0.0f, -10.0f), Vector3.forward, out var hit, 1000.0f);
            Assert.IsTrue(isHit);
            Assert.That(hit.collider, Is.EqualTo(boxA as BaseCollider));

            isHit = PhysicsManager.RaycastAny(new Vector3(1.0f, 0.0f, -10.0f), Vector3.forward, out hit, 1000.0f);
            Assert.IsTrue(isHit);
            Assert.That(hit.collider, Is.EqualTo(boxB as BaseCollider));
        }

        [UnityTest]
        public IEnumerator TestInsideHits()
        {
            var gameObject = new GameObject("Box");

            var boxA = gameObject.AddComponent<BoxCollider3D>();

            yield return new WaitForFixedUpdate();

            var isHit = PhysicsManager.RaycastClosest(new Vector3(-0.6f, 0.0f, 0.0f), Vector3.right, out var hit, 1000.0f);
            Assert.IsTrue(isHit);
            Assert.That(hit.collider, Is.EqualTo(boxA as BaseCollider));

            isHit = PhysicsManager.RaycastClosest(new Vector3(0.5f, 0.0f, 0.0f), Vector3.right, out hit, 1000.0f);
            Assert.IsFalse(isHit);

            isHit = PhysicsManager.RaycastAny(new Vector3(0.4f, 0.0f, 0.0f), Vector3.right, out hit, 1000.0f);
            Assert.IsFalse(isHit);

            isHit = PhysicsManager.RaycastAny(new Vector3(0.0f, 0.0f, 0.0f), Vector3.right, out hit, 1000.0f, sceneQueryFlags: SceneQueryFlags.Movable | SceneQueryFlags.Static);
            Assert.IsTrue(isHit);
            Assert.AreEqual(hit.distance, 0.0f);
        }
    }
}
