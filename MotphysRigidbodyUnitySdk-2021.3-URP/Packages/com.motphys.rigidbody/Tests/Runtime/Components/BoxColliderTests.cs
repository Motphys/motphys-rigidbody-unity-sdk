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
    internal class BoxColliderTests : ComponentTestScene
    {

        [UnityTest]
        public IEnumerator TestBoxColliderScale()
        {
            var planeBody = new GameObject("Plane").AddComponent<Rigidbody3D>();
            planeBody.isKinematic = true;
            var collider = planeBody.gameObject.AddComponent<BoxCollider3D>();
            collider.supportDynamicScale = true;
            planeBody.transform.localScale = new Vector3(100, 1, 100);

            var box = CreateRigidBody("Box");
            var boxColliderB = box.gameObject.AddComponent<BoxCollider3D>();
            box.enablePostTransformControl = true;
            boxColliderB.supportDynamicScale = true;
            box.transform.position = new Vector3(0, 1.5f, 0);
            box.transform.localScale = new Vector3(2, 2, 2);

            for (var i = 0; i < 10; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.That(box.transform.position.y > 1.49f, box.transform.position.ToString("0.0000"));
        }

        [UnityTest]
        public IEnumerator TestShapeTransform()
        {
            var parent = new GameObject("Parent");
            var colliderGo = new GameObject("Child");

            colliderGo.transform.localRotation = Quaternion.Euler(0, 0, 30);
            var collider = colliderGo.AddComponent<BoxCollider3D>();
            collider.supportDynamicScale = true;
            collider.transform.SetParent(parent.transform, true);
            collider.shapeTranslation = new Vector3(1, 0, 0);
            parent.transform.localScale = new Vector3(10, 1, 10);

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            var shapeToWorld = collider.shapeToWorldMatrix;

            var corners = new Vector3[]{
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
            };

            var bounds = new Bounds(shapeToWorld.MultiplyPoint3x4(corners[0]), Vector3.zero);
            foreach (var c in corners)
            {
                var v = shapeToWorld.MultiplyPoint3x4(c);
                bounds.Encapsulate(v);
            }

            yield return new WaitForFixedUpdate();

            var aabb = collider.motionAABB;

            AssertVector3Eq(aabb.center, bounds.center, 1e-3f);
            AssertVector3Eq(aabb.size, bounds.size + Vector3.one * 2 * collider.contactOffset, 1e-3f);
        }
    }
}
