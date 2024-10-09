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
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace Motphys.Rigidbody.Tests
{

    internal class ComponentTestScene : IPrebuildSetup, IPostBuildCleanup
    {

        private struct PhysicsManagerSetting
        {
            public SimulationMode simulationMode;
            public uint numSubstep;
            public uint numSolverIter;
            public float defaultContactOffset;
            public float defaultSeparationOffset;
        }

        private PhysicsManagerSetting PopulatePhysicsManagerSetting()
        {
            var setting = new PhysicsManagerSetting();
            setting.simulationMode = PhysicsManager.simulationMode;
            setting.numSubstep = PhysicsManager.numSubstep;
            setting.numSolverIter = PhysicsManager.defaultSolverIterations;
            setting.defaultContactOffset = PhysicsManager.defaultContactOffset;
            setting.defaultSeparationOffset = PhysicsManager.defaultSeparationOffset;
            return setting;
        }

        private void RestorePhysicsManagerSetting(PhysicsManagerSetting setting)
        {
            PhysicsManager.simulationMode = setting.simulationMode;
            PhysicsManager.numSubstep = setting.numSubstep;
            PhysicsManager.defaultSolverIterations = setting.numSolverIter;
            PhysicsManager.defaultContactOffset = setting.defaultContactOffset;
            PhysicsManager.defaultSeparationOffset = setting.defaultSeparationOffset;
        }

        private PhysicsManagerSetting _physicsManagerSetting;

        [UnitySetUp]
        protected virtual IEnumerator SetUp()
        {
            PhysicsManager.RestartEngine();
            var scene = SceneManager.CreateScene("PhysicsTestScene");
            SceneManager.SetActiveScene(scene);
            // all unit tests share static variables, so we need to save the original value and restore it after the test.
            _physicsManagerSetting = PopulatePhysicsManagerSetting();
            PhysicsManager.simulationMode = SimulationMode.FixedUpdate;
            PhysicsManager.numSubstep = 1;
            PhysicsManager.defaultSolverIterations = 1;
            PhysicsManager.defaultSolverVelocityIterations = 1;
            PhysicsManager.defaultContactOffset = 0.005f;
            PhysicsManager.defaultSeparationOffset = 0.0f;
            yield return null;
        }

        protected IEnumerator UnloadTestScene()
        {
            yield return SceneManager.UnloadSceneAsync("PhysicsTestScene");
        }

        [UnityTearDown]
        protected virtual IEnumerator TearDown()
        {
            yield return UnloadTestScene();
            RestorePhysicsManagerSetting(_physicsManagerSetting);
        }

        protected Rigidbody3D CreateRigidBody(Vector3 position = default)
        {
            return CreateRigidBody("", position);
        }

        protected Rigidbody3D CreateRigidBody(string name, Vector3 position = default)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            return go.AddComponent<Rigidbody3D>();
        }

        protected static void AssertVector3Eq(Vector3 actual, Vector3 expected, float tolerance = 1e-6f)
        {
            Assert.That(actual, Is.EqualTo(expected).Using(new Vector3EqualityComparer(tolerance)), $"Expected: {expected:0.000000}, Actual: {actual:0.000000}");
        }

#if UNITY_EDITOR
        private const string LinkerSrcPath = "Packages/com.motphys.rigidbody/Tests/Runtime/Components/link.xml";

        private const string LinkerDestPath = "Assets/com.motphys.rigidbody/Tests/Runtime/Components/link.xml";
#endif

        public void Setup()
        {
#if UNITY_EDITOR
            // Copy the linker that preserves all UnityEngine Physics symbols
            // so we can use GameObject.CreatePrimitive in tests
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(LinkerDestPath)))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(LinkerDestPath));
            }

            UnityEditor.AssetDatabase.CopyAsset(LinkerSrcPath, LinkerDestPath);
#endif
        }

        public void Cleanup()
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.DeleteAsset(System.IO.Path.GetDirectoryName(LinkerDestPath));
#endif
        }
    }
}
