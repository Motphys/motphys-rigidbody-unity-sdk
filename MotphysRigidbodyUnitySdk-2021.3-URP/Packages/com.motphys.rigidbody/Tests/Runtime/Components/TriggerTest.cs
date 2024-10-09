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
    internal class TriggerTest : ComponentTestScene
    {
        [UnityTest]
        public IEnumerator TestEnableAndDisable()
        {
            var collider = new GameObject("Body").AddComponent<BoxCollider3D>();

            yield return new WaitForFixedUpdate();

            Assert.IsFalse(collider.isTrigger);
            Assert.IsFalse(collider.isTriggerInNative);

            collider.isTrigger = true;

            Assert.IsTrue(collider.isTrigger);
            Assert.IsTrue(collider.isTriggerInNative);

            collider.isTrigger = false;

            Assert.IsFalse(collider.isTrigger);
            Assert.IsFalse(collider.isTriggerInNative);

            var go = new GameObject("InitWithTrigger");

            go.SetActive(false);
            collider = go.AddComponent<BoxCollider3D>();
            Assert.IsFalse(collider.isTrigger);
            Assert.IsFalse(collider.isTriggerInNative);
            collider.isTrigger = true;
            Assert.IsTrue(collider.isTrigger);
            Assert.IsFalse(collider.isTriggerInNative);
            go.SetActive(true);

            yield return new WaitForFixedUpdate();

            Assert.IsTrue(collider.isTrigger);
            Assert.IsTrue(collider.isTriggerInNative);
        }

        [UnityTest]
        public IEnumerator TestTriggerEnter()
        {
            var box = new GameObject("Box");
            var sphere = new GameObject("Sphere");
            var boxCollider = box.AddComponent<BoxCollider3D>();
            boxCollider.isTrigger = true;

            sphere.transform.position = new Vector3(0, 2, 0);
            var sphereCollider = sphere.AddComponent<SphereCollider3D>();
            var rigid = sphere.AddComponent<Rigidbody3D>();

            BaseCollider targetCollider = null;

            sphereCollider.onTriggerEnter += (collider) =>
            {
                targetCollider = collider;
            };

            var nextUpdate = new WaitForFixedUpdate();

            for (var i = 0; i < 30; i++)
            {
                yield return nextUpdate;
            }

            Assert.IsTrue(targetCollider == boxCollider);
        }

        [UnityTest]
        public IEnumerator TestTriggerStay()
        {
            var box = new GameObject("Box");
            var sphere = new GameObject("Sphere");
            var boxCollider = box.AddComponent<BoxCollider3D>();
            boxCollider.isTrigger = true;

            sphere.transform.position = new Vector3(0, 2, 0);
            var sphereCollider = sphere.AddComponent<SphereCollider3D>();
            var rigid = sphere.AddComponent<Rigidbody3D>();

            BaseCollider targetCollider = null;

            sphereCollider.onTriggerStay += (collider) =>
            {
                targetCollider = collider;
            };

            var nextUpdate = new WaitForFixedUpdate();

            for (var i = 0; i < 30; i++)
            {
                yield return nextUpdate;
            }

            Assert.IsTrue(targetCollider == boxCollider);
        }

        [UnityTest]
        public IEnumerator TestTriggerExit()
        {
            var box = new GameObject("Box");
            var sphere = new GameObject("Sphere");
            var boxCollider = box.AddComponent<BoxCollider3D>();
            boxCollider.isTrigger = true;

            sphere.transform.position = new Vector3(0, 2, 0);
            var sphereCollider = sphere.AddComponent<SphereCollider3D>();
            var rigid = sphere.AddComponent<Rigidbody3D>();

            BaseCollider targetCollider = null;

            sphereCollider.onTriggerExit += (collider) =>
            {
                targetCollider = collider;
            };

            var nextUpdate = new WaitForFixedUpdate();

            for (var i = 0; i < 60; i++)
            {
                yield return nextUpdate;
            }

            Assert.IsTrue(targetCollider == boxCollider);
        }
    }
}
