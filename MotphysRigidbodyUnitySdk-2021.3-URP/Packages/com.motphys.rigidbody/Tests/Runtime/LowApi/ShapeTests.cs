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

namespace Motphys.Rigidbody.Tests
{
    internal class ShapeTests : BasePhysicsEngineTest
    {
        [Test]
        public void SphereTest()
        {
            var options = new ActorOptions(DynamicOptions.Dynamic, ColliderGroupOptions.OneShape(Sphere.Identity));
            var rigidbody = world.AddRigidbody(options);
            //TODO: Assert shape datas
            Assert.IsTrue(rigidbody.RemoveFromWorld());
        }

        [Test]
        public void CylinderTest()
        {
            var options = new ActorOptions(DynamicOptions.Dynamic, ColliderGroupOptions.OneShape(Cylinder.Identity));
            var rigidbody = world.AddRigidbody(options);
            //TODO: Assert shape datas
            Assert.IsTrue(rigidbody.RemoveFromWorld());
        }

        [Test]
        public void CapsuleTest()
        {
            var options = new ActorOptions(DynamicOptions.Dynamic, ColliderGroupOptions.OneShape(Capsule.Identity));
            var rigidbody = world.AddRigidbody(options);
            //TODO: Assert shape datas
            Assert.IsTrue(rigidbody.RemoveFromWorld());
        }

        [Test]
        public void InfinitePlaneTest()
        {
            var options = new ActorOptions(Motion.Static, ColliderGroupOptions.OneShape(InfinitePlane.Identity));
            var rigidbody = world.AddRigidbody(options);
            //TODO: Assert shape datas
            Assert.IsTrue(rigidbody.RemoveFromWorld());
        }
    }
}
