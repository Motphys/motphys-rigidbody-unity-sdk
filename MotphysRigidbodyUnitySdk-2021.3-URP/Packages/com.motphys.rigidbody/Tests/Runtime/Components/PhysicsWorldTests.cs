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
    internal class PhysicsWorldTests : ComponentTestScene
    {
        [UnityTest]
        public IEnumerator TestPhysicsWorld()
        {
            var valid = PhysicsManager.defaultWorld.isValid;
            Assert.IsTrue(valid);

            var simulationOptions = PhysicsManager.defaultWorld.simulatorOptions;
            Assert.IsTrue(simulationOptions.gravity.y != 0);

            simulationOptions.gravity = new UnityEngine.Vector3(0, -20, 0);
            var world = PhysicsManager.defaultWorld;
            world.simulatorOptions = simulationOptions;

            var subSteps = world.substeps;
            Assert.IsTrue(subSteps > 0);

            world.substeps = 4;

            var removed = world.RemoveJoint(new Internal.JointId());
            Assert.That(!removed);

            var bodyA = CreateRigidBody(new UnityEngine.Vector3(0, 10, 0));
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            var tramsforms = world.FetchBodyTransforms();
            Assert.That(tramsforms.count > 0);
            var trans = tramsforms.ToSpan()[0];
            Assert.That(trans.transform.position.y < 10);

            simulationOptions.numPositionSolverIterations = 4;
            simulationOptions.numVelocitySolverIterations = 4;
            simulationOptions.numSubSteps = 4;

            world.simulatorOptions = simulationOptions;
            var options = world.simulatorOptions;

            Assert.IsTrue(options.numSubSteps == 4);
            Assert.IsTrue(options.numPositionSolverIterations == 4);
            Assert.IsTrue(options.numVelocitySolverIterations == 4);
        }
    }
}
