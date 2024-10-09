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
using Motphys.Rigidbody.Api;
using Unity.Collections;

namespace Motphys.Rigidbody.Internal
{
    internal readonly ref struct Statistics
    {
        private readonly WorldRef _world;

        public Statistics(WorldRef world)
        {
            _world = world;
        }

        public NativeArray<ContactDebugInfo> contactDebugInfos
        {
            get
            {
                _world.mprQueryWorldEvents(out var events).ThrowExceptionIfNotOk();
                var collidingEnters = events.collidingEnters;
                var collidingStays = events.collidingStays;
                var maxNumContactPoints = (collidingEnters.Length + collidingStays.Length) * ContactManifold.maxContactPoints;
                var contactDebugInfos = new NativeArray<ContactDebugInfo>(maxNumContactPoints, Allocator.Temp);
                var index = 0;
                FillUpContactDebugInfos(contactDebugInfos, collidingEnters, ref index);
                FillUpContactDebugInfos(contactDebugInfos, collidingStays, ref index);
                return contactDebugInfos.GetSubArray(0, index);
            }
        }

        private void FillUpContactDebugInfos(NativeArray<ContactDebugInfo> contactDebugInfos, Span<(ColliderId, ColliderId)> pairs, ref int index)
        {
            for (var i = 0; i < pairs.Length; ++i)
            {
                var pair = pairs[i];
                var colliderPair = new ColliderPair(pair.Item1, pair.Item2);
                _world.mprQueryContactManifold(colliderPair, out var contactManifold).ThrowExceptionIfNotOk();
                for (var j = 0; j < contactManifold.contactCount; ++j)
                {
                    contactDebugInfos[index++] = contactManifold.GetContactDebugInfo(j);
                }
            }
        }
    }
}
