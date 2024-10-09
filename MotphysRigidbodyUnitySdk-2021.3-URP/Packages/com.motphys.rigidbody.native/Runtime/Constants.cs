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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a `unified-ci rust pack`.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Motphys.Rigidbody.Native
{
    public static class Constants
    {
#if (!UNITY_EDITOR) && (UNITY_EDITOR || !DEVELOPMENT_BUILD) && !((MOTPHYS_RIGIDBODY_DETERMINISTIC))
        public const string DLL = Motphys.Rigidbody.Native.Standard.Constants.DLL;
#endif
#if (!UNITY_EDITOR) && (UNITY_EDITOR || !DEVELOPMENT_BUILD) && ((MOTPHYS_RIGIDBODY_DETERMINISTIC))
        public const string DLL = Motphys.Rigidbody.Native.Deterministic.Constants.DLL;
#endif
#if (TRUE) && (UNITY_EDITOR || DEVELOPMENT_BUILD) && !((MOTPHYS_RIGIDBODY_DETERMINISTIC))
        public const string DLL = Motphys.Rigidbody.Native.Standard.Development.Constants.DLL;
#endif
#if (TRUE) && (UNITY_EDITOR || DEVELOPMENT_BUILD) && ((MOTPHYS_RIGIDBODY_DETERMINISTIC))
        public const string DLL = Motphys.Rigidbody.Native.Deterministic.Development.Constants.DLL;
#endif
    }
}