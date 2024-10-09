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
    internal class RigidbodyUtilsTests : ComponentTestScene
    {
        [UnityTest]
        public IEnumerator TestCreateColliders()
        {
            var sphere = PhysicsUtils.CreatePrimitive(PrimitiveType.Sphere, new Vector3(-2, 0, 0));
            var cube = PhysicsUtils.CreatePrimitive(PrimitiveType.Cube, new Vector3(2, 2, 0));
            var cylinder = PhysicsUtils.CreatePrimitive(PrimitiveType.Cylinder, new Vector3(0, 0, 2));
            var capsule = PhysicsUtils.CreatePrimitive(PrimitiveType.Capsule, new Vector3(0, 0, -2));
            var plane = PhysicsUtils.CreatePrimitive(PrimitiveType.Plane, new Vector3(0, -2, 0));
            var quad = PhysicsUtils.CreatePrimitive(PrimitiveType.Quad);

            Assert.That(sphere.transform.position.x == -2);
            var flag = sphere.TryGetComponent<SphereCollider3D>(out var sphereCol);
            Assert.IsTrue(flag);

            Assert.That(cube.transform.position.x == 2);
            flag = cube.TryGetComponent<BoxCollider3D>(out var boxCol);
            Assert.IsTrue(flag);

            Assert.That(cylinder.transform.position.z == 2);
            flag = cylinder.TryGetComponent<CylinderCollider3D>(out var cylinderCol);
            Assert.IsTrue(flag);

            Assert.That(capsule.transform.position.z == -2);
            flag = capsule.TryGetComponent<CapsuleCollider3D>(out var capsuleCol);
            Assert.IsTrue(flag);

            Assert.That(plane.transform.position.y == -2);
            flag = plane.TryGetComponent<InfinitePlaneCollider3D>(out var planeCol);
            Assert.IsTrue(flag);

            Assert.That(quad.transform.position.y == 0);
            flag = quad.TryGetComponent<BoxCollider3D>(out var quadCol);
            Assert.IsTrue(flag);

            yield return new WaitForFixedUpdate();
        }
    }
}
