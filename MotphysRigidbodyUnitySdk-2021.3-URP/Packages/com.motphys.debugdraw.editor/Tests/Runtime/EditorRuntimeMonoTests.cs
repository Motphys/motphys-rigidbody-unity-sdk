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

using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Motphys.DebugDraw.Editor.PlayMode.Tests
{
    internal class EditorRuntimeMonoTests
    {
        [UnityTest]
        public IEnumerator PhysicsStatisticTest()
        {
            var go = new GameObject("EditorRuntimeTest");
            var statisc = go.AddComponent<MotphysStatistic>();
            var nextFrame = new WaitForFixedUpdate();

            yield return nextFrame;
            yield return nextFrame;
            yield return nextFrame;
        }

        [UnityTest]
        public IEnumerator RuntimeGizmosTest()
        {
            var nextFrame = new WaitForFixedUpdate();

            var go_0 = new GameObject("RuntimeGizmosTest");
            var gizmos_0 = go_0.AddComponent<RuntimeGizmosEditorUse>();
            gizmos_0.drawContacts = true;
            gizmos_0.drawAabb = true;
            gizmos_0.drawCollisionPair = true;
            gizmos_0.drawJointPair = true;

            yield return nextFrame;

            gizmos_0.DrawRuntimeGizmos();

            var go = new GameObject("RuntimeGizmosTest");
            var gizmos = go.AddComponent<RuntimeGizmosEditorUse>();

            gizmos.drawContacts = true;
            gizmos.drawAabb = true;
            gizmos.drawCollisionPair = true;
            gizmos.drawJointPair = true;

            yield return nextFrame;
            yield return nextFrame;
            yield return nextFrame;

            gizmos.DrawRuntimeGizmos();

            yield return nextFrame;
            yield return nextFrame;
            yield return nextFrame;

            gizmos.DrawRuntimeGizmos();
        }
    }
}

