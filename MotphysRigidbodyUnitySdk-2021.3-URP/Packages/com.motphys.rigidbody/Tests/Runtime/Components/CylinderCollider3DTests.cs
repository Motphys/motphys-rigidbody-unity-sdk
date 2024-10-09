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

    internal class CylinderCollider3DTests : ComponentTestScene
    {
        // Start is called before the first frame update
        [UnitySetUp]
        protected override IEnumerator SetUp()
        {
            yield return base.SetUp();
            PhysicsManager.numSubstep = 5;
            PhysicsManager.defaultSolverIterations = 1;
        }

        [Test]
        public void TestSetCylinderCollider3DParam()
        {
            var rigidBody = CreateRigidBody("Cylinder", new Vector3(0f, 5f, 0f));
            var cylinderCollider = rigidBody.gameObject.AddComponent<CylinderCollider3D>();

            cylinderCollider.height = 1f;
            cylinderCollider.radius = 2f;
            Assert.That(Mathf.Abs(cylinderCollider.height - 1f) < Mathf.Epsilon);
            Assert.That(Mathf.Abs(cylinderCollider.radius - 2f) < Mathf.Epsilon);
        }

        private IEnumerator TestCylinderCollider3D(System.Type collider)
        {
            // create a falldown cylinder
            var rigidBody = CreateRigidBody("FalldownCylinder", new Vector3(0f, 5f, 0f));
            rigidBody.useGravity = true;
            var cylinderCollider = rigidBody.gameObject.AddComponent<CylinderCollider3D>();
            // create a fixed collider
            var fixedToHit = new GameObject("FixedCollider");
            fixedToHit.transform.localPosition = new Vector3(0.2f, 0f, 0.3f);
            var fixedRotation = Quaternion.Euler(45f, 45f, 45f);
            fixedToHit.transform.localRotation = fixedRotation;
            var _fixedCollider = fixedToHit.AddComponent(collider);
            for (var i = 0; i < 50; i++)
            {
                // wait cylinder falldown
                yield return new WaitForFixedUpdate();
            }

            Assert.That(rigidBody.position.y > 0f);
            Assert.That(cylinderCollider.transform.position.y > 0f);

            for (var i = 0; i < 50; i++)
            {
                // wait cylinder collide
                yield return new WaitForFixedUpdate();
            }

            Assert.That(rigidBody.transform.position.y < 0f);
            Assert.That(cylinderCollider.transform.position.y < 0f);
            Assert.That(Mathf.Abs(cylinderCollider.transform.position.x) > 1e-3);
            Assert.That(Mathf.Abs(cylinderCollider.transform.position.z) > 1e-3);
            Assert.That(cylinderCollider.transform.rotation != Quaternion.identity);
        }

        // Update is called once per frame
        [UnityTest]
        public IEnumerator TestCylinderCollider3DWithCylinder()
        {
            yield return TestCylinderCollider3D(typeof(CylinderCollider3D));
        }

        [UnityTest]
        public IEnumerator TestCylinderCollider3DWithBox()
        {
            yield return TestCylinderCollider3D(typeof(BoxCollider3D));
        }

        [UnityTest]
        public IEnumerator TestCylinderCollider3DWithCapsule()
        {
            yield return TestCylinderCollider3D(typeof(CapsuleCollider3D));
        }

        [UnityTest]
        public IEnumerator TestCylinderCollider3DWithSphere()
        {
            yield return TestCylinderCollider3D(typeof(SphereCollider3D));
        }
    }
}
