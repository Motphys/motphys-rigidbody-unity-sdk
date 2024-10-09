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
    internal class StatisticsTests : ComponentTestScene
    {
        [UnitySetUp]
        protected override IEnumerator SetUp()
        {
            yield return base.SetUp();
            PhysicsManager.numSubstep = 1;
            PhysicsManager.defaultSolverIterations = 1;
        }

        [UnityTest]
        public IEnumerator TestAllStatistics()
        {
            var box = new GameObject("Box");
            box.AddComponent<BoxCollider3D>();
            box.AddComponent<Rigidbody3D>();
            box.GetComponent<Rigidbody3D>().useGravity = true;
            box.GetComponent<Rigidbody3D>().enablePostTransformControl = true;
            box.transform.position = new Vector3(0, 0.5f, 0);

            var ground = new GameObject("Ground");
            ground.AddComponent<BoxCollider3D>();
            ground.transform.position = new Vector3(0, 0, 0);
            ground.transform.localScale = new Vector3(10, 1, 10);
            ground.GetComponent<BoxCollider3D>().supportDynamicScale = true;

            var bodyA = new GameObject("bodyA");
            bodyA.AddComponent<BoxCollider3D>();
            bodyA.AddComponent<Rigidbody3D>();
            bodyA.GetComponent<Rigidbody3D>().enablePostTransformControl = true;
            bodyA.transform.position = new Vector3(0, 100f, 0);

            var bodyB = new GameObject("bodyB");
            bodyB.AddComponent<BoxCollider3D>();
            bodyB.AddComponent<Rigidbody3D>();
            bodyB.GetComponent<Rigidbody3D>().enablePostTransformControl = true;
            bodyB.transform.position = new Vector3(0, 101f, 0);

            bodyA.AddComponent<FixedJoint3D>();
            bodyA.GetComponent<FixedJoint3D>().connectedBody = bodyB.GetComponent<Rigidbody3D>();
            bodyA.GetComponent<FixedJoint3D>().ignoreCollision = true;

            StepMetrics metrics = default;
            System.Action<StepMetrics> onReceiveMetrics = (pMetrics) =>
            {
                metrics = pMetrics;
            };

            VisualizeData visualizeData = default;
            System.Action<VisualizeData> onReceiveVisualizeData = (pVisualizeData) =>
            {
                visualizeData = pVisualizeData;

            };

            PhysicsManager.requestMetrics += onReceiveMetrics;
            PhysicsManager.RequestVisualizeDataOnce(VisualizeDataType.All, onReceiveVisualizeData);

            yield return new WaitForFixedUpdate();

            Assert.AreEqual(3, metrics.awakeCount);

            Assert.AreEqual(4, visualizeData.activeAabbs.Length);
            Assert.AreEqual(1, visualizeData.activeJointPositionPairs.Length);
            Assert.AreEqual(1, visualizeData.potentialCollisionPositionPairs.Length);

            var contact_debug_infos = PhysicsManager.Statistics.contacts;
            Assert.AreEqual(4, contact_debug_infos.Length);
        }
    }
}
