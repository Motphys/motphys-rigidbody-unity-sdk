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
using UnityEngine;

namespace Motphys.Rigidbody
{
    /// <summary>
    /// A static infinite plane collider, n=<0,1,0>, d=0 in plane's local space
    /// </summary>
    [AddComponentMenu("Motphys/Colliders/InfinitePlaneCollider3D")]
    public class InfinitePlaneCollider3D : BaseCollider
    {
        internal override bool OnCreateShape(Vector3 scale, out Shape shape)
        {
            shape = InfinitePlane.Identity;
            return true;
        }

        internal override bool CanCreateShape()
        {
            return true;
        }
    }
}
