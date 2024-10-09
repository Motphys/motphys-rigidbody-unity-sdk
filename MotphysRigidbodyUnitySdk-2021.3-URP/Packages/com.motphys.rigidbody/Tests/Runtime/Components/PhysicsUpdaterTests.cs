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
namespace Motphys.Rigidbody.Tests
{
    internal class PhysicsUpdaterTests
    {
        [UnityTest]
        public IEnumerator TestPhysicsUpdater()
        {
            yield return new WaitForSeconds(2.0f);

            var go = GameObject.Find("PhysicsUpdater");
            if (go != null)
            {
                MonoBehaviour.Destroy(go);
            }

            yield return new WaitForSeconds(2.0f);

            PhysicsUpdater.Launch();

            yield return new WaitForSeconds(2.0f);
        }

        [Test]
        public void TestPhysicsSettings()
        {
            var setting = PhysicsProjectSettings.CreateInstance<PhysicsProjectSettings>();
            setting.numSubstep = 4;
            Assert.That(setting.numSubstep == 4);
            setting.enableSceneQuery = false;
            Assert.That(!setting.enableSceneQuery);
            setting.enableContactEvent = true;
            Assert.That(setting.enableContactEvent);

            setting.broadPhaseType = BroadPhaseType.GridSAP;
            Assert.That(setting.broadPhaseType.algorithm == BroadPhaseAlgorithm.GridSAP);
            setting.gravity = new Vector3(0, -20.0f, 0);
            Assert.That(setting.gravity == new Vector3(0, -20.0f, 0));
            setting.numSolverPositionIterations = 4;
            Assert.That(setting.numSolverPositionIterations == 4);
            setting.numSolverVelocityIterations = 2;
            Assert.That(setting.numSolverVelocityIterations == 2);

            setting.enableSceneQuery = true;
            Assert.That(setting.enableSceneQuery);
            setting.allowExpandSpeculativeMargin = false;
            Assert.That(setting.allowExpandSpeculativeMargin == false);

            setting.SetAllLayersCollisionMask(true);
            Assert.That(setting.GetCollisionMask(0) == PhysicsProjectSettings.FullLayerCollisionMask);

            var layerA = 3;
            var layerB = 6;
            var flag = setting.IgnoreLayerCollision(layerA, layerB, true);
            var layerAMask = PhysicsProjectSettings.FullLayerCollisionMask & ~(1u << layerB);
            Assert.That(setting.GetCollisionMask(layerA) == layerAMask);
            var layerBMask = PhysicsProjectSettings.FullLayerCollisionMask & ~(1u << layerA);
            Assert.That(setting.GetCollisionMask(layerB) == layerBMask);
            Assert.That(flag);

            flag = setting.IgnoreLayerCollision(98, 1, true);
            Assert.That(!flag);
        }
    }
}
