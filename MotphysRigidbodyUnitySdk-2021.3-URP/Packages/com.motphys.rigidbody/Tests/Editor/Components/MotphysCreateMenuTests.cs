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

using NUnit.Framework;
using UnityEngine;

namespace Motphys.Rigidbody.Editor
{
    internal class MotphysCreateMenuTests
    {
        [Test]
        public void TestCreateCube()
        {
            {
                var go = MotphysCreateMenu.CreateAndPlacePrimitive(PrimitiveType.Cube, null);
                Assert.IsTrue(go.TryGetComponent<BoxCollider3D>(out var component));
            }

            {
                var parent = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var go = MotphysCreateMenu.CreateAndPlacePrimitive(PrimitiveType.Cube, parent);
                Assert.IsTrue(go.TryGetComponent<BoxCollider3D>(out var component));

                Assert.IsTrue(parent.transform.childCount > 0);
                Assert.IsTrue(parent.transform.GetChild(0).gameObject == go);
            }
        }
        [Test]
        public void TestCreateSphere()
        {
            var go = MotphysCreateMenu.CreateAndPlacePrimitive(PrimitiveType.Sphere, null);
            Assert.IsTrue(go.TryGetComponent<SphereCollider3D>(out var component));
        }
        [Test]
        public void TestCreateCapsule()
        {
            var go = MotphysCreateMenu.CreateAndPlacePrimitive(PrimitiveType.Capsule, null);
            Assert.IsTrue(go.TryGetComponent<CapsuleCollider3D>(out var component));
        }
        [Test]
        public void TestCreateCylinder()
        {
            var go = MotphysCreateMenu.CreateAndPlacePrimitive(PrimitiveType.Cylinder, null);
            Assert.IsTrue(go.TryGetComponent<CylinderCollider3D>(out var component));
        }
        [Test]
        public void TestCreatePlane()
        {
            var go = MotphysCreateMenu.CreateAndPlacePrimitive(PrimitiveType.Plane, null);
            Assert.IsTrue(go.TryGetComponent<InfinitePlaneCollider3D>(out var component));
        }
        [Test]
        public void TestCreateQuad()
        {
            var go = MotphysCreateMenu.CreateAndPlacePrimitive(PrimitiveType.Quad, null);
            Assert.IsTrue(go.TryGetComponent<BoxCollider3D>(out var component));
        }

        [Test]
        public void TestCreateGameObjectsByMenu()
        {
            MotphysCreateMenu.CreatePlane(null);
            MotphysCreateMenu.CreateCube(null);
            MotphysCreateMenu.CreateSphere(null);
            MotphysCreateMenu.CreateCapsule(null);
            MotphysCreateMenu.CreateCylinder(null);
            MotphysCreateMenu.CreateQuad(null);
        }
    }
}
