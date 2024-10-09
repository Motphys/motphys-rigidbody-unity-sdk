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

using System.Runtime.InteropServices;
using Motphys.Native;

namespace Motphys.Rigidbody.Internal
{

    /// <summary>
    /// WorldId is used to index a specified world.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct WorldId
    {
        private long _value;

        internal WorldId(long value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public bool isValid
        {
            get { return index != USize.Max; }
        }

        internal uint version
        {
            get { return (uint)(_value >> 32); }
        }

        internal uint index
        {
            get { return (uint)_value; }
        }

        public static readonly WorldId Invalid = new WorldId(unchecked((long)USize.Max.ToUInt64()));
    }

    internal ref struct WorldRef
    {
        private System.IntPtr _engine;
        private WorldId _id;

        public WorldRef(System.IntPtr engine, WorldId id)
        {
            _engine = engine;
            _id = id;
        }

        public WorldId id
        {
            get { return _id; }
        }

        public System.IntPtr engine
        {
            get { return _engine; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct WorldEvents
    {
        private SliceRef<(ColliderId, ColliderId)> _collidingEnters;
        private SliceRef<(ColliderId, ColliderId)> _collidingStays;
        private SliceRef<(ColliderId, ColliderId)> _collidingExits;

        private SliceRef<(ColliderId, ColliderId)> _triggerEnters;
        private SliceRef<(ColliderId, ColliderId)> _triggerStays;
        private SliceRef<(ColliderId, ColliderId)> _triggerExits;

        private SliceRef<RigidbodyId> _wakeUpBodies;
        private SliceRef<RigidbodyId> _sleepDownBodies;
        private SliceRef<JointId> _brokenJoints;

        public System.Span<(ColliderId, ColliderId)> collidingEnters => _collidingEnters;

        public System.Span<(ColliderId, ColliderId)> collidingExits => _collidingExits;

        public System.Span<(ColliderId, ColliderId)> collidingStays => _collidingStays;

        public System.Span<(ColliderId, ColliderId)> triggerEnters => _triggerEnters;

        public System.Span<(ColliderId, ColliderId)> triggerExits => _triggerExits;

        public System.Span<(ColliderId, ColliderId)> triggerStays => _triggerStays;

        public System.Span<RigidbodyId> wakeUpBodies
        {
            get { return _wakeUpBodies; }
        }

        public System.Span<RigidbodyId> sleepDownBodies
        {
            get { return _sleepDownBodies; }
        }

        public System.Span<JointId> brokenJoints
        {
            get { return _brokenJoints; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct VisualizeQuery
    {
        System.IntPtr _world;
    }

    /// <summary>
    /// StepContinuation is returned by PhysicsWorld.Step() and can be used to do further queries. It is designed as a ref struct so that user should use it immediately after it is returned.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal ref struct StepContinuation
    {
        System.IntPtr _world;
    }
}
