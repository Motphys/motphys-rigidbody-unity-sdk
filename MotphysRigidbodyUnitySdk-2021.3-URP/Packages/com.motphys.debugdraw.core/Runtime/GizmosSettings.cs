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
using UnityEngine;

namespace Motphys.DebugDraw.Core
{
    [Serializable]
    internal class GizmosSettings : ScriptableObject
    {
        [SerializeField]
        internal Material _instanceMaterial;
        [SerializeField]
        internal ComputeShader _aabbComputeShader;
        [SerializeField]
        internal Shader _aabbShader;
        [SerializeField]
        internal Shader _lineShader;
        [SerializeField]
        internal Shader _capsuleShader;
        [SerializeField]
        internal Shader _pointShader;
        [SerializeField]
        internal Shader _aabbShaderFallback;
        [SerializeField]
        internal Shader _lineShaderFallback;
        [SerializeField]
        internal Shader _pointShaderFallback;
    }
}
