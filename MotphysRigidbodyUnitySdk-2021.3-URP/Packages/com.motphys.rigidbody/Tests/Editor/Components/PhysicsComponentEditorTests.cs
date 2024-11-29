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

using Motphys.Rigidbody.Internal;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Utils;

namespace Motphys.Rigidbody.Editor
{
    internal class PhysicsComponentTestsEditorTests
    {
        [Test]
        public void TestRigidbodyComponent()
        {
            var body = new GameObject("Body").AddComponent<Rigidbody3D>();
            body.linearVelocity = Vector3.one;
            // set linearVelocity does not work in editor mode.
            Assert.That(body.linearVelocity, Is.EqualTo(Vector3.zero));

            body.angularVelocity = Vector3.one;
            // set angularVelocity does not work in editor mode.
            Assert.That(body.angularVelocity, Is.EqualTo(Vector3.zero));

            body.transform.position = Vector3.one;
            // rigidbody.position is always same with transform.position in editor mode.
            Assert.That(body.position, Is.EqualTo(Vector3.one));

            body.transform.rotation = Quaternion.Euler(30, 40, 50);
            // rigidbody.rotation is always same with transform.rotation in editor mode.
            Assert.That(body.rotation, Is.EqualTo(Quaternion.Euler(30, 40, 50)).Using(new QuaternionEqualityComparer(1e-4f)));

            // set position and rotation for rigidbody in editor mode will change transform immediately.
            body.position = Vector3.up;
            body.rotation = Quaternion.Euler(10, 20, 30);
            Assert.That(body.transform.position, Is.EqualTo(Vector3.up));
            Assert.That(body.transform.rotation, Is.EqualTo(Quaternion.Euler(10, 20, 30)).Using(new QuaternionEqualityComparer(1e-4f)));

            // set drag in editor
            body.drag = 1f;
            Assert.That(body.drag, Is.EqualTo(1f));

            // set angularDrag in editor
            body.angularDrag = 1f;
            Assert.That(body.angularDrag, Is.EqualTo(1f));

            // set sleepThreshold in editor
            body.sleepThreshold = 0.1f;
            Assert.That(body.sleepThreshold, Is.EqualTo(0.1f));

            // native inertiaPrincipal is always zero in editor mode.
            Assert.That(body.inertiaPrincipal, Is.EqualTo(Vector3.zero));

            // native numShapes is always zero in editor mode.
            Assert.That(body.numShapes, Is.EqualTo(0));
        }

        [Test]
        public void TestColliderComponent()
        {
            var collider = new GameObject("Body").AddComponent<BoxCollider3D>();

            // is trigger
            Assert.That(collider.isTrigger, Is.EqualTo(false));
            Assert.That(collider.isTriggerInNative, Is.EqualTo(false));
            collider.isTrigger = true;
            Assert.That(collider.isTrigger, Is.EqualTo(true));
            Assert.That(collider.isTriggerInNative, Is.EqualTo(false));
            collider.isTrigger = false;
            Assert.That(collider.isTrigger, Is.EqualTo(false));
            Assert.That(collider.isTriggerInNative, Is.EqualTo(false));
        }

        [Test]
        public void TestBoxColliderComponent()
        {
            var collider = new GameObject("Body").AddComponent<BoxCollider3D>();
            collider.size = new Vector3(2.0f, 5.0f, 72.0f);
            collider.size = new Vector3(2.0f, 5.0f, 72.0f);

            collider.shapeTranslation = Vector3.right;
            collider.shapeRotation = Quaternion.AngleAxis(30.0f, Vector3.up);

            Assert.That(collider.motionAABB, Is.EqualTo(new Aabb3(Vector3.zero, Vector3.zero)));
        }

        [Test]
        public void TestSphereColliderComponent()
        {
            var collider = new GameObject("Body").AddComponent<SphereCollider3D>();
            collider.radius = 5.0f;
            collider.radius = 5.0f;

            collider.shapeTranslation = Vector3.right;
            collider.shapeRotation = Quaternion.AngleAxis(30.0f, Vector3.up);

            Assert.That(collider.motionAABB, Is.EqualTo(new Aabb3(Vector3.zero, Vector3.zero)));
        }

        [Test]
        public void TestCapsuleColliderComponent()
        {
            var collider = new GameObject("Body").AddComponent<CapsuleCollider3D>();
            collider.radius = 5.0f;
            collider.radius = 5.0f;
            collider.height = 5.0f;
            collider.height = 5.0f;

            Assert.That(collider.height, Is.EqualTo(5.0f));
            Assert.That(collider.radius, Is.EqualTo(5.0f));

            collider.shapeTranslation = Vector3.right;
            collider.shapeRotation = Quaternion.AngleAxis(30.0f, Vector3.up);

            Assert.That(collider.motionAABB, Is.EqualTo(new Aabb3(Vector3.zero, Vector3.zero)));
        }

        [Test]
        public void TestCylinderColliderComponent()
        {
            var collider = new GameObject("Body").AddComponent<CylinderCollider3D>();
            collider.radius = 5.0f;
            collider.radius = 5.0f;

            collider.height = 5.0f;
            collider.height = 5.0f;

            Assert.That(collider.height, Is.EqualTo(5.0f));
            Assert.That(collider.radius, Is.EqualTo(5.0f));

            collider.shapeTranslation = Vector3.right;
            collider.shapeRotation = Quaternion.AngleAxis(30.0f, Vector3.up);

            Assert.That(collider.motionAABB, Is.EqualTo(new Aabb3(Vector3.zero, Vector3.zero)));
        }

        [Test]
        public void TestMeshColliderComponent()
        {
            var collider = new GameObject("Body").AddComponent<MeshCollider3D>();
            collider.convex = true;
            collider.shapeTranslation = Vector3.right;
            collider.shapeRotation = Quaternion.AngleAxis(30.0f, Vector3.up);

            Assert.That(collider.motionAABB, Is.EqualTo(new Aabb3(Vector3.zero, Vector3.zero)));
        }

        [Test]
        public void TestInfinitePlaneColliderComponent()
        {
            var collider = new GameObject("Body").AddComponent<InfinitePlaneCollider3D>();
        }

        [Test]
        public void TestColliderResetSize()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var collider = go.AddComponent<BoxCollider3D>();
            Assert.That(collider.size, Is.EqualTo(Vector3.one));

            var cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            var cylinderCollider = cylinder.AddComponent<CylinderCollider3D>();
            Assert.That(cylinderCollider.height, Is.EqualTo(2));
            Assert.That(cylinderCollider.radius, Is.EqualTo(0.5f).Using(new FloatEqualityComparer(1e-4f)));

            var sphereCollider = cylinder.AddComponent<SphereCollider3D>();
            Assert.That(sphereCollider.radius, Is.EqualTo(1.0f));

            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var capsuleCollider = sphere.AddComponent<CapsuleCollider3D>();
            Assert.That(capsuleCollider.height, Is.EqualTo(1.0f));
        }

        [Test]
        public void TestColliderCollisionSetting()
        {
            var collider = new GameObject("Body").AddComponent<SphereCollider3D>();
            var collisionSetting = CollisionSetting.Default;
            collisionSetting.contactOffset = 0.1f;
            collisionSetting.separationOffset = 0.05f;
            collider.collisionSetting = collisionSetting;
            Assert.That(collider.collisionSetting, Is.EqualTo(collisionSetting));
        }

        [Test]
        public void TestRigidbodyCenterOfMass()
        {
            var body = new GameObject("Body").AddComponent<Rigidbody3D>();
            var box0 = body.gameObject.AddComponent<BoxCollider3D>();
            var box1 = body.gameObject.AddComponent<BoxCollider3D>();
            box1.shapeTranslation = new Vector3(2, 0, 0);
            var centroid = body.centerOfMass;
            Assert.That(centroid == Vector3.zero);
        }

        [Test]
        public void TestInvalidMeshCollierAdd_Triangle()
        {
            var mesh = new Mesh();
            mesh.SetVertices(new Vector3[] { new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f) });
            mesh.SetTriangles(new int[] { 0, 1, 2 }, 0);
            mesh.name = "Triangle";

            var collider = new GameObject("Body").AddComponent<MeshCollider3D>();
            collider.convex = true;
            collider.mesh = mesh;

            Assert.IsTrue(collider._isMeshValid);
        }
    }
}
