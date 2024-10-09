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

namespace Motphys.Core.Tests
{
    internal class MathTests
    {
        [Test]
        public void TestAabb3()
        {
            var aabb = new Aabb3(new Vector3(), new Vector3());
            Assert.That(aabb.min, Is.EqualTo(aabb.max));
            Assert.That(aabb.center, Is.EqualTo(Vector3.zero));
            Assert.That(aabb.size, Is.EqualTo(Vector3.zero));
            Assert.That(aabb.extents, Is.EqualTo(Vector3.zero));
            Assert.That(aabb.GetHashCode(), Is.EqualTo(0));
            Assert.That(aabb.Equals(aabb), Is.True);
            Assert.That(aabb.Equals(new object()), Is.False);
            Assert.That(aabb.Equals(new Aabb3(new Vector3(1, 2, 3), new Vector3(4, 5, 6))), Is.False);
            Assert.That(aabb == aabb, Is.True);
            Assert.That(aabb != aabb, Is.False);
            Assert.That((Bounds)aabb, Is.EqualTo(new Bounds(Vector3.zero, Vector3.zero)));
        }
    }
}
