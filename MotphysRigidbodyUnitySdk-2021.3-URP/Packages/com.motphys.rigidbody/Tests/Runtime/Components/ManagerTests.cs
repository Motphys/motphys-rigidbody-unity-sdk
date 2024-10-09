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
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace Motphys.Rigidbody.Tests
{
    internal class ManagerTests : ComponentTestScene
    {

        [UnityTest]
        public IEnumerator TestSimulationMode()
        {
            PhysicsManager.numSubstep = 1;
            var gravity = PhysicsManager.gravity;
            // test script simulation mode.
            PhysicsManager.simulationMode = SimulationMode.Script;
            var body = CreateRigidBody();
            var initPosition = body.transform.position;
            yield return new WaitForFixedUpdate();
            yield return null;
            // step won't be executed in script mode. so the position should not change.
            Assert.That(body.position, Is.EqualTo(initPosition));

            // call simulation manually.
            var dt = 0.01f;
            PhysicsManager.Simulate(dt);

            Assert.That(body.linearVelocity, Is.EqualTo(gravity * dt).Using(new Vector3EqualityComparer(1e-3f)));
            Assert.That(body.position, Is.EqualTo(initPosition + gravity * dt * dt).Using(new Vector3EqualityComparer(1e-3f)));

            var prevLinearVelocity = body.linearVelocity;
            var prevPosition = body.position;

            PhysicsManager.simulationMode = SimulationMode.Update;
            yield return null;

            Assert.That(body.linearVelocity, Is.EqualTo(prevLinearVelocity + gravity * Time.deltaTime).Using(new Vector3EqualityComparer(1e-3f)));
            Assert.That(body.position, Is.EqualTo(prevPosition + body.linearVelocity * Time.deltaTime).Using(new Vector3EqualityComparer(1e-3f)));

            prevLinearVelocity = body.linearVelocity;
            prevPosition = body.position;

            PhysicsManager.simulationMode = SimulationMode.FixedUpdate;
            yield return new WaitForFixedUpdate();

            AssertVector3Eq(body.linearVelocity, prevLinearVelocity + gravity * Time.fixedDeltaTime, 1e-3f);
            AssertVector3Eq(body.position, prevPosition + body.linearVelocity * Time.fixedDeltaTime, 1e-3f);
        }

        [Test]
        public void TestInvalidOperations()
        {
            Assert.That(() =>
            {
                PhysicsManager.numSubstep = 0;
            }, Throws.ArgumentException);

            Assert.That(() =>
            {
                PhysicsManager.defaultSolverIterations = 0;
            }, Throws.ArgumentException);

            Assert.That(() =>
            {
                PhysicsManager.defaultSolverVelocityIterations = 0;
            }, Throws.ArgumentException);
        }

        [UnityTest]
        public IEnumerator TestShutdown()
        {
            CreateRigidBody().gameObject.AddComponent<BoxCollider3D>();
            yield return new WaitForFixedUpdate();
            PhysicsManager.ShutDownEngine();
            UnloadTestScene();
            PhysicsManager.LaunchEngine();
        }

        [Test]
        public void TestSetGet()
        {

            // defaultSolverVelocityIterations
            {
                Assert.That(PhysicsManager.defaultSolverVelocityIterations, Is.EqualTo(1));
                PhysicsManager.defaultSolverVelocityIterations = 2;
                Assert.That(PhysicsManager.defaultSolverVelocityIterations, Is.EqualTo(2));
                PhysicsManager.defaultSolverVelocityIterations = 1;
                Assert.That(PhysicsManager.defaultSolverIterations, Is.EqualTo(1));
            }

            // allow speculative margin
            {
                Assert.That(PhysicsManager.allowExpandSpeculativeMargin, Is.EqualTo(false));
                PhysicsManager.allowExpandSpeculativeMargin = true;
                Assert.That(PhysicsManager.allowExpandSpeculativeMargin, Is.EqualTo(true));
            }

            // default contact offset
            {
                Assert.That(PhysicsManager.defaultContactOffset, Is.EqualTo(0.005f));
                PhysicsManager.defaultContactOffset = 0.02f;
                Assert.That(PhysicsManager.defaultContactOffset, Is.EqualTo(0.02f));
                Assert.Throws<System.ArgumentException>(() =>
                {
                    PhysicsManager.defaultContactOffset = 0.0f;
                });
                PhysicsManager.defaultContactOffset = 0.005f;
                Assert.That(PhysicsManager.defaultContactOffset, Is.EqualTo(0.005f));
            }

            // default separation offset
            {
                Assert.That(PhysicsManager.defaultSeparationOffset, Is.EqualTo(0.0f));
                Assert.Throws<System.ArgumentException>(() =>
                {
                    PhysicsManager.defaultSeparationOffset = 0.02f;
                });
                PhysicsManager.defaultSeparationOffset = 0.0f;
                Assert.That(PhysicsManager.defaultSeparationOffset, Is.EqualTo(0.0f));
            }
        }

        [Test]
        [UnityPlatform(
            exclude = new RuntimePlatform[]
            {
                RuntimePlatform.Android,
                RuntimePlatform.IPhonePlayer,
                RuntimePlatform.WebGLPlayer,
#if TUANJIE_1_0_OR_NEWER
                RuntimePlatform.WeixinMiniGamePlayer
#endif
            }
        )]
        public void TestTracing()
        {
            if (!PhysicsManager.IsTracingStarted)
            {
                PhysicsManager.StartTracing();
                Assert.IsTrue(PhysicsManager.IsTracingStarted);
                PhysicsManager.EndTracing(".", "trace_data.json");
                Assert.IsFalse(PhysicsManager.IsTracingStarted);
                if (File.Exists("trace_data.json"))
                {
                    File.Delete("trace_data.json");
                }
            }

            if (!PhysicsManager.IsTracingStarted)
            {
                var folder = "./temp1/temp2";
                Directory.CreateDirectory(folder);
                PhysicsManager.StartTracing();
                Assert.IsTrue(PhysicsManager.IsTracingStarted);
                PhysicsManager.EndTracing(folder, "trace_data.json");
                Assert.IsFalse(PhysicsManager.IsTracingStarted);
                if (File.Exists("./temp1/temp2/trace_data.json"))
                {
                    File.Delete("./temp1/temp2/trace_data.json");
                }

                Directory.Delete(folder);
                Directory.Delete("./temp1");
            }

            if (!PhysicsManager.IsTracingStarted)
            {
                var folder = "./temp1-346tntgf9834489y98gyf9he8/temp2-2353dfg3224tsewsert43w5yt545674356dffrgw34";
                PhysicsManager.StartTracing();
                Assert.IsTrue(PhysicsManager.IsTracingStarted);
                Assert.That(() =>
                {
                    PhysicsManager.EndTracing(folder, "trace_data.json");
                }, Throws.TypeOf<IOException>());

                Directory.CreateDirectory(folder);
                PhysicsManager.EndTracing(folder, "trace_data.json");
                Assert.IsFalse(PhysicsManager.IsTracingStarted);
                if (File.Exists("./temp1-346tntgf9834489y98gyf9he8/temp2-2353dfg3224tsewsert43w5yt545674356dffrgw34/trace_data.json"))
                {
                    File.Delete("./temp1-346tntgf9834489y98gyf9he8/temp2-2353dfg3224tsewsert43w5yt545674356dffrgw34/trace_data.json");
                }

                Directory.Delete(folder);
                Directory.Delete("./temp1-346tntgf9834489y98gyf9he8");
            }
        }
    }
}
