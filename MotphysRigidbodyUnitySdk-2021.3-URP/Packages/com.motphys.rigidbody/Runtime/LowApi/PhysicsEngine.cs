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

using System.Runtime.CompilerServices;
using Motphys.Rigidbody.Api;

namespace Motphys.Rigidbody.Internal
{

    internal class PhysicsEngine : System.IDisposable
    {
        private System.IntPtr _ptr;
        private PhysicsWorld _defaultWorld;

        public PhysicsEngine()
        {
            _ptr = PhysicsNativeApi.mprCreatePhysicsEngine();
            Initialize();
        }

        public PhysicsEngine(PhysicsEngineBuilder options)
        {
            _ptr = PhysicsNativeApi.mprCreatePhysicsEngineWithOptions(options);
            Initialize();
        }

        private void Initialize()
        {
            var defaultWorldId = PhysicsNativeApi.mprGetDefaultWorldId(_ptr);
            _defaultWorld = new PhysicsWorld(this, defaultWorldId);
        }

        public PhysicsWorld defaultWorld
        {
            get
            {
                ThrowExceptionIfDisposed();
                return _defaultWorld;
            }
        }

        internal System.IntPtr ptr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ThrowExceptionIfDisposed();
                return _ptr;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal System.IntPtr GetPtrUnchecked()
        {
            return _ptr;
        }

        public bool isDisposed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _ptr == System.IntPtr.Zero; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowExceptionIfDisposed()
        {
            if (isDisposed)
            {
                throw new System.ObjectDisposedException(this.GetType().Name);
            }
        }

        private void DoDispose()
        {
            if (isDisposed)
            {
                return;
            }

            PhysicsNativeApi.mprDestroyPhysicsEngine(_ptr);
            _ptr = System.IntPtr.Zero;
        }

        public void Dispose()
        {
            DoDispose();
            System.GC.SuppressFinalize(this);
        }

        ~PhysicsEngine()
        {
            DoDispose();
        }
    }

    internal static class PhysicsEngineBuilderExt
    {
        public static PhysicsEngine Build(this PhysicsEngineBuilder builder)
        {
            return new PhysicsEngine(builder);
        }
    }
}
