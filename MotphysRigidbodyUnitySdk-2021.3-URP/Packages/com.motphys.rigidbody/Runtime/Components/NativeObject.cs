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

using System.Collections.Generic;
using UnityEngine;

namespace Motphys.Rigidbody
{
    /// <summary>
    /// The motphys native object.
    /// </summary>
    public abstract class NativeObject : MonoBehaviour
    {
        internal enum InitStatus : byte
        {
            NotIntialized,
            Initializing,
            Initizlized,
            Destroying,
            Destroyed,
        }

        private bool _hasAwake = false;
        private InitStatus _initStatus = InitStatus.NotIntialized;

        protected virtual void Awake()
        {
            _hasAwake = true;
            if (_initStatus != InitStatus.NotIntialized)
            {
                s_nonAwakeNativeObjects.Remove(this);
            }
            else
            {
                CreateNative();
            }
        }

        protected virtual void OnDestroy()
        {
            Debug.Assert(_hasAwake);
            Debug.Assert(!s_nonAwakeNativeObjects.Contains(this));
            DestroyNativeIfCan();
        }

        /// <value>
        /// Will be true after OnCreateNative has been called
        /// </value>
        internal bool hasNativeInitialized
        {
            get { return _initStatus == InitStatus.Initizlized; }
        }

        protected void CreateNativeIfNot()
        {
            if (_initStatus == InitStatus.NotIntialized)
            {
                CreateNative();
            }
        }

        private void DestroyNativeIfCan()
        {
            Debug.Assert(_initStatus == InitStatus.Initizlized);
            _initStatus = InitStatus.Destroying;
            OnDestroyNative();
            _initStatus = InitStatus.Destroyed;
        }

        private void CreateNative()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new System.Exception("Api currently only support in runtime");
            }
#endif
            Debug.Assert(_initStatus == InitStatus.NotIntialized);
            _initStatus = InitStatus.Initializing;
            try
            {
                OnCreateNative();
                _initStatus = InitStatus.Initizlized;
                if (!_hasAwake)
                {
                    s_nonAwakeNativeObjects.Add(this);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        protected abstract void OnCreateNative();
        protected abstract void OnDestroyNative();

        private static HashSet<NativeObject> s_nonAwakeNativeObjects = new HashSet<NativeObject>();
        private static List<NativeObject> s_nativeDestroyed = new List<NativeObject>();

        internal static void CleanUntrackedDestroy()
        {
            foreach (var o in s_nonAwakeNativeObjects)
            {
                if (!o)
                {
                    o.DestroyNativeIfCan();
                    s_nativeDestroyed.Add(o);
                }
            }

            foreach (var o in s_nativeDestroyed)
            {
                s_nonAwakeNativeObjects.Remove(o);
            }

            s_nativeDestroyed.Clear();
        }
    }
}
