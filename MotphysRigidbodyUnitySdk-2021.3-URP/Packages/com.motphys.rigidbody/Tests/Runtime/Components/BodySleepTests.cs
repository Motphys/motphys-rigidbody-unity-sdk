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
    internal class BodySleepTests : ComponentTestScene
    {
        [UnityTest]
        public IEnumerator TestTouchToWakeup()
        {
            var plane = new GameObject("Plane").AddComponent<BoxCollider3D>();
            var body = CreateRigidBody("Box", new Vector3(0, 1, 0));
            body.gameObject.AddComponent<BoxCollider3D>();

            var kinematic = CreateRigidBody("Kinematic", new Vector3(-2, 1, 0));
            kinematic.gameObject.AddComponent<BoxCollider3D>();
            kinematic.isKinematic = true;
            kinematic.enablePostTransformControl = true;

            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(1f);
            Assert.That(body.isSleeping);

            kinematic.transform.position = new Vector3(0, 1, 0);
            yield return new WaitForFixedUpdate();
            Assert.That(!body.isSleeping);
            Assert.That(!kinematic.isSleeping);

        }
    }
}
