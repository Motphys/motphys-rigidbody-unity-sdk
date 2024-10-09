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

    internal class DisabledSceneQueryTests : ComponentTestScene
    {
        [UnityTest]
        public IEnumerator TestOnAndDown()
        {
            new GameObject("box").AddComponent<BoxCollider3D>();
            PhysicsManager.isSceneQueryOn = false;

            yield return new WaitForFixedUpdate();

            Assert.IsFalse(PhysicsManager.isSceneQueryOn);

            try
            {
                PhysicsManager.RaycastAny(new Vector3(0.0f, 0.0f, -100.0f), Vector3.forward, out _, 1000.0f);
            }
            catch (System.Exception ex)
            {
                Assert.AreEqual("Physics Engine Exception:SceneQueryIsOff", ex.Message);
            }

            PhysicsManager.isSceneQueryOn = true;
            Assert.IsTrue(PhysicsManager.isSceneQueryOn);

            yield return new WaitForFixedUpdate();
            Assert.IsTrue(PhysicsManager.RaycastAny(new Vector3(0.0f, 0.0f, -100.0f), Vector3.forward, out _, 1000.0f));

            PhysicsManager.isSceneQueryOn = false;
            Assert.IsFalse(PhysicsManager.isSceneQueryOn);
            yield return new WaitForFixedUpdate();

            try
            {
                PhysicsManager.RaycastAny(new Vector3(0.0f, 0.0f, -100.0f), Vector3.forward, out _, 1000.0f);
            }
            catch (System.Exception ex)
            {
                Assert.AreEqual("Physics Engine Exception:SceneQueryIsOff", ex.Message);
            }
        }

        [UnityTest]
        public IEnumerator Test1()
        {
            var box = new GameObject("box").AddComponent<BoxCollider3D>();

            yield return new WaitForFixedUpdate();

            Assert.IsTrue(PhysicsManager.isSceneQueryOn);
            Assert.IsTrue(PhysicsManager.RaycastAny(new Vector3(0.0f, 0.0f, -100.0f), Vector3.forward, out _, 1000.0f));
        }
    }
}
