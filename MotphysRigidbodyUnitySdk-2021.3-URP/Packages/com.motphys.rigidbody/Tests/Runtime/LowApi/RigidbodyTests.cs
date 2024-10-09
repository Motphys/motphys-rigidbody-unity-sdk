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

using System;
using Motphys.Rigidbody.Internal;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Motphys.Rigidbody.Tests
{
    internal class RigidbodyTests : BasePhysicsEngineTest
    {
        [Test]
        public void AddRemoveRigidBody()
        {
            var colliders = ColliderGroupOptions.OneShape(Cuboid.Identity);
            var options = new ActorOptions(DynamicOptions.Dynamic, colliders);
            var pos = new Vector3(1, 2, 3);
            var rot = Quaternion.Euler(60, 100, -60);
            rot.Normalize();
            options.transform.position = pos;
            options.transform.rotation = rot;
            var rigidbody = world.AddRigidbody(options);
            Assert.AreEqual(pos, rigidbody.position);
            Assert.AreEqual(rot, rigidbody.rotation);
            Assert.AreEqual(1.0, rigidbody.mass);
            Assert.IsTrue(rigidbody.isGravityEnabled);

            Assert.IsTrue(rigidbody.isValid);

            rigidbody.SetVelocity(new Vector3(1, 0, 0), new Vector3(10, 0, 0));
            Assert.IsTrue(rigidbody.velocity.linear.x == 1);

            var maxVel = rigidbody.maxLinearVelocity;
            Assert.IsTrue(maxVel > 0);

            var maxAngularVel = rigidbody.maxAngularVelocity;
            Assert.IsTrue(maxAngularVel > 0);

            var freeze = rigidbody.freeze;
            Assert.IsTrue(!freeze.freezePositionX);

            var key = rigidbody.FirstShapeKey();
            Assert.IsTrue(key.isValid);

            var collisionMask = rigidbody.GetCollisionMask(key);
            Assert.IsTrue(collisionMask.group == 1);

            var collisionSetting = CollisionSetting.Default;
            collisionSetting.contactOffset = 0.01f;
            collisionSetting.separationOffset = 0.001f;
            rigidbody.SetCollisionSetting(key, collisionSetting);
            collisionSetting = rigidbody.GetCollisionSetting(key);
            Assert.IsTrue(collisionSetting.contactOffset == 0.01f);
            Assert.IsTrue(collisionSetting.separationOffset == 0.001f);

            NUnit.Framework.Assert.Throws<ArgumentException>(() => { collisionSetting.contactOffset = 0.0f; });
            NUnit.Framework.Assert.Throws<ArgumentException>(() => { collisionSetting.separationOffset = 0.1f; });

            var hasShape = rigidbody.hasShape;
            Assert.IsTrue(hasShape);

            var bodyName = rigidbody.ToString();
            Assert.IsTrue(bodyName != null);

            Assert.IsTrue(rigidbody.RemoveFromWorld());
            Assert.IsFalse(rigidbody.RemoveFromWorld());
        }
    }
}
