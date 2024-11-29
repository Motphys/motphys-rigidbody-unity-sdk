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
    internal class MeshCollierTests : ComponentTestScene
    {

        [UnityTest]
        public IEnumerator TestMeshCollierAdd()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = Vector3.zero;
            var mesh = cube.GetComponent<MeshFilter>().mesh;
            var planeBody = cube.AddComponent<Rigidbody3D>();
            planeBody.isKinematic = true;
            var collider = planeBody.gameObject.AddComponent<MeshCollider3D>();
            collider.supportDynamicScale = true;
            collider.convex = true;
            collider.mesh = mesh;

            var box = CreateRigidBody("Box");
            var mesh_collider_box = box.gameObject.AddComponent<MeshCollider3D>();

            box.enablePostTransformControl = true;
            mesh_collider_box.supportDynamicScale = true;
            mesh_collider_box.convex = true;
            mesh_collider_box.mesh = mesh;
            box.transform.position = new Vector3(0, 1.0f, 0);

            Assert.IsTrue(mesh_collider_box.childIndex.isValid);

            for (var i = 0; i < 10; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.That(box.transform.position.y > 0.98f, box.transform.position.ToString("0.0000"));
            mesh_collider_box.mesh = null;
            Assert.IsFalse(mesh_collider_box.childIndex.isValid);

            for (var i = 0; i < 10; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.That(box.transform.position.y < 0.98f, box.transform.position.ToString("0.0000"));

            mesh_collider_box.mesh = mesh;
            box.transform.position = new Vector3(0, 1.5f, 0);
            box.transform.localScale = new Vector3(2, 2, 2);

            Assert.IsTrue(mesh_collider_box.childIndex.isValid);

            for (var i = 0; i < 10; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.That(box.transform.position.y > 0.98f, box.transform.position.ToString("0.0000"));
        }

        [UnityTest, Ignore("adapt to the new convex hull log (#1326)")]
        public IEnumerator TestMeshCollierBake()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = Vector3.zero;
            var mesh = cube.GetComponent<MeshFilter>().mesh;
            var newMesh = new ConvexMesh(mesh);

            Assert.NotNull(newMesh);
            Assert.NotNull(newMesh.Mesh);

            yield return new WaitForFixedUpdate();

            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.position = Vector3.zero;
            var planeMesh = plane.GetComponent<MeshFilter>().mesh;
            var planeNewMesh = new ConvexMesh(planeMesh);

            Assert.NotNull(newMesh);
            LogAssert.Expect(LogType.Error, "Failed to create a convex mesh from the source mesh 'Plane Instance', because the volume of source mesh is too small.");
        }
    }
}
