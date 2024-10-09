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
using Assert = UnityEngine.Assertions.Assert;
using Motion = Motphys.Rigidbody.Internal.Motion;

namespace Motphys.Rigidbody.Tests
{
    internal class SceneQueryTests : BasePhysicsEngineTest
    {

        private static ActorOptions CreateStaticActor(Shape shape)
        {
            return CreateStaticActor(new ColliderOptions(shape));
        }

        private static ActorOptions CreateStaticActor(ColliderOptions collider)
        {
            var colliders = ColliderGroupOptions.One(collider);
            var options = new ActorOptions(Motion.Static, colliders);
            return options;
        }

        private static ActorOptions CreateStaticCubeActor(Vector3 size)
        {
            return CreateStaticActor(new Cuboid() { halfExt = size * 0.5f, });
        }

        [Test]
        public void RaycastSingleStaticBody()
        {
            var options = CreateStaticCubeActor(Vector3.one * 100f);
            world.AddRigidbody(options);

            world.Step(0.02f);

            var hit = world.RaycastClosest(
                new Vector3(0.0f, 0.0f, 100.0f), new Vector3(0.0f, 0.0f, -1.0f),
                200.0f,
                SceneQueryContext.Default
            );

            Assert.IsTrue(hit.HasValue);
            Assert.AreEqual(hit.Value.distance, 50.0f);
        }

        [Test]
        public void RaycastClosestBody()
        {

            var options = CreateStaticCubeActor(Vector3.one * 100);
            world.AddRigidbody(options);

            options.transform.position = new Vector3(200.0f, 0.0f, 0.0f);
            world.AddRigidbody(options);

            world.Step(0.02f);

            var hit = world.RaycastClosest(
                new Vector3(0.0f, 0.0f, 100.0f), new Vector3(0.0f, 0.0f, -1.0f),
                1000.0f,
                SceneQueryContext.Default
            );

            Assert.IsTrue(hit.HasValue);
            Assert.AreEqual(hit.Value.distance, 50.0f);
        }

        [Test]
        public void RaycastAll()
        {

            var options = CreateStaticCubeActor(Vector3.one * 100);
            world.AddRigidbody(options);

            options.transform.position = new Vector3(0, 0.0f, -200f);
            world.AddRigidbody(options);

            world.Step(0.02f);

            var results = new Internal.RaycastHit[4];

            var numHits = world.RaycastAllNonAlloc(
                new Vector3(0.0f, 0.0f, 100.0f), new Vector3(0.0f, 0.0f, -1.0f),
                1000.0f,
                2,
                results,
                SceneQueryContext.Default
            );

            Assert.AreEqual(numHits, 2);
        }

        [Test]
        public void RaycastAllWithoutTrigger()
        {
            var options = CreateStaticCubeActor(Vector3.one * 100f);
            var collider = options.colliders[0];
            collider.isTrigger = true;
            options.colliders[0] = collider;
            world.AddRigidbody(options);

            options = CreateStaticCubeActor(Vector3.one * 100);
            options.transform.position = new Vector3(0.0f, 0.0f, -200.0f);
            world.AddRigidbody(options);

            world.Step(0.02f);

            var results = new Internal.RaycastHit[4];

            var numHits = world.RaycastAllNonAlloc(
                new Vector3(0.0f, 0.0f, 100.0f), new Vector3(0.0f, 0.0f, -1.0f),
                1000.0f,
                2,
                results,
                SceneQueryContext.Default.WithQueryTrigger(false)
            );

            Assert.AreEqual(numHits, 1);

            numHits = world.RaycastAllNonAlloc(
                new Vector3(0.0f, 0.0f, 100.0f), new Vector3(0.0f, 0.0f, -1.0f),
                1000.0f,
                2,
                results,
                SceneQueryContext.Default.WithQueryTrigger(true)
            );

            Assert.AreEqual(numHits, 2);
        }

        [Test]
        public void RaycastAllWithoutLayerMask()
        {
            var options = CreateStaticCubeActor(Vector3.one * 100f);
            world.AddRigidbody(options);

            var collider = new ColliderOptions(new Cuboid(Vector3.one * 50));
            collider.collisionMask.group = 2;
            options = CreateStaticActor(collider);
            options.transform.position = new Vector3(0.0f, 0.0f, -200.0f);
            world.AddRigidbody(options);

            collider = new ColliderOptions(new Cuboid(Vector3.one * 50));
            collider.collisionMask.group = UnityEngine.Physics.IgnoreRaycastLayer;
            options = CreateStaticActor(collider);
            options.transform.position = new Vector3(0.0f, 0.0f, -400.0f);
            world.AddRigidbody(options);

            world.Step(0.02f);

            var results = new Internal.RaycastHit[4];

            var origin = new Vector3(0.0f, 0.0f, 100.0f);

            var numHits = world.RaycastAllNonAlloc(
                origin, new Vector3(0.0f, 0.0f, -1.0f),
                1000.0f,
                3,
                results,
                SceneQueryContext.Default
            );

            Assert.AreEqual(numHits, 2);

            numHits = world.RaycastAllNonAlloc(
                origin, new Vector3(0.0f, 0.0f, -1.0f),
                1000.0f,
                3,
                results,
                SceneQueryContext.Default.ReplaceLayerMask(1)
            );

            Assert.AreEqual(numHits, 1);

            numHits = world.RaycastAllNonAlloc(
                origin, new Vector3(0.0f, 0.0f, -1.0f),
                1000.0f,
                3,
                results,
                SceneQueryContext.Default.ReplaceLayerMask(1).WithLayers(2)
            );

            Assert.AreEqual(numHits, 2);

            numHits = world.RaycastAllNonAlloc(
                origin, new Vector3(0.0f, 0.0f, -1.0f),
                1000.0f,
                2,
                results,
                SceneQueryContext.Default.ReplaceLayerMask(1).WithLayers(2 | UnityEngine.Physics.IgnoreRaycastLayer)
            );

            Assert.AreEqual(numHits, 2);

            numHits = world.RaycastAllNonAlloc(
                origin, new Vector3(0.0f, 0.0f, -1.0f),
                1000.0f,
                4,
                results,
                SceneQueryContext.Default.ReplaceLayerMask(1).WithLayers(2 | UnityEngine.Physics.IgnoreRaycastLayer)
            );

            Assert.AreEqual(numHits, 3);

            numHits = world.RaycastAllNonAlloc(
             origin, new Vector3(0.0f, 0.0f, -1.0f),
             1000.0f,
             4,
             results,
             SceneQueryContext.Default.WithoutLayers(LayerMask.GetMask("Default"))
            );

            Assert.AreEqual(numHits, 1);
        }

        [Test]
        public void OverlapTestSingleStaticBody()
        {
            var options = CreateStaticCubeActor(Vector3.one);
            var handler = world.AddRigidbody(options);

            var collider = new ColliderOptions(new Cuboid(Vector3.one * 0.5f));
            collider.collisionMask.group = 2;

            options = CreateStaticActor(collider);
            var handler2 = world.AddRigidbody(options);

            world.Step(0.02f);

            var buffer = new ColliderId[10];

            var numOverlaps = world.OverlayTestNonAlloc(
                new Cuboid() { halfExt = new Vector3(1, 1, 1) * 0.5f, },
                new Isometry { position = new Vector3(0, 0, 0), rotation = Quaternion.identity },
                1,
                buffer,
                SceneQueryContext.Empty.WithFlags(SceneQueryFlags.Movable).WithLayers(1)
            );

            Assert.AreEqual(0, numOverlaps);

            numOverlaps = world.OverlayTestNonAlloc(
                new Cuboid() { halfExt = new Vector3(1, 1, 1) * 0.5f, },
                new Isometry { position = new Vector3(0, 0, 0), rotation = Quaternion.identity },
                1,
                buffer,
                new SceneQueryContext(SceneQueryFlags.Movable, true, 1)
            );

            Assert.AreEqual(0, numOverlaps);

            numOverlaps = world.OverlayTestNonAlloc(
                new Cuboid() { halfExt = new Vector3(1, 1, 1) * 0.5f, },
                new Isometry { position = new Vector3(0, 0, 0), rotation = Quaternion.identity },
                1,
                buffer,
                SceneQueryContext.Empty.WithFlags(SceneQueryFlags.Static).WithLayers(1)
            );

            Assert.AreEqual(1, numOverlaps);
            Assert.AreEqual(handler.id, buffer[0].bodyId);

            numOverlaps = world.OverlayTestNonAlloc(
                new Cuboid() { halfExt = new Vector3(1, 1, 1) * 0.5f, },
                new Isometry { position = new Vector3(0, 0, 0), rotation = Quaternion.identity },
                1,
                buffer,
                new SceneQueryContext(SceneQueryFlags.Static, true, 1)
            );

            Assert.AreEqual(1, numOverlaps);
            Assert.AreEqual(handler.id, buffer[0].bodyId);

            numOverlaps = world.OverlayTestNonAlloc(
                new Cuboid() { halfExt = new Vector3(1, 1, 1) * 0.5f, },
                new Isometry { position = new Vector3(0, 0, 0), rotation = Quaternion.identity },
                1,
                buffer,
                SceneQueryContext.Empty.WithFlags(SceneQueryFlags.Static).WithLayers(2)
            );

            Assert.AreEqual(1, numOverlaps);
            Assert.AreEqual(handler2.id, buffer[0].bodyId);

            numOverlaps = world.OverlayTestNonAlloc(
                new Cuboid() { halfExt = new Vector3(1, 1, 1) * 0.5f, },
                new Isometry { position = new Vector3(0, 0, 0), rotation = Quaternion.identity },
                2,
                buffer,
                new SceneQueryContext(SceneQueryFlags.Static, true, 1).WithLayers(2)
            );

            Assert.AreEqual(2, numOverlaps);
        }
    }
}
