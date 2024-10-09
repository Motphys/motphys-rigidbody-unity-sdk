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

using System.IO;
using Motphys.Rigidbody.Api;
using Motphys.Rigidbody.Internal;
using UnityEngine;

namespace Motphys.Rigidbody
{
    public static partial class PhysicsManager
    {
        internal delegate void EngineCreatingEvent(ref PhysicsEngineBuilder builder);

        internal static SimulatorDynamicOptions s_dynamicOptions = SimulatorDynamicOptions.Default;

        private static bool s_tracing_guard = false;

        private static float s_defaultContactOffset = 0.005f;

        private static float s_defaultSeparationOffset = 0.0f;

        internal static event EngineCreatingEvent onEngineCreating;

        internal static void ApplyOptionsRuntime()
        {
            engine?.defaultWorld.ApplySimulatorOptions(s_dynamicOptions);
        }

        /// <value>
        /// The number of substeps per physics step. Improve the numSubstep will improve the stability and accuracy of the simulation but will also increase the computation cost.
        /// </value>
        /// <exception cref="System.ArgumentException">numSubstep must be greater than 0</exception>
        public static uint numSubstep
        {
            get
            {
                return s_dynamicOptions.numSubSteps;
            }
            set
            {
                if (value == 0)
                {
                    throw new System.ArgumentException("numSubstep must be greater than 0");
                }

                s_dynamicOptions.numSubSteps = value;
                ApplyOptionsRuntime();
            }
        }

        /// <value>
        /// The default number of position solver iterations for all constraints in each substep. Increase the number of solver iterations will improve the stability and accuracy of the simulation but will also increase the computation cost.
        /// </value>
        ///
        /// <exception cref="System.ArgumentException">numSolverIter must be greater than 0</exception>
        public static uint defaultSolverIterations
        {
            get
            {
                return s_dynamicOptions.numPositionSolverIterations;
            }
            set
            {
                if (value == 0)
                {
                    throw new System.ArgumentException("numSolverIter must be greater than 0");
                }

                s_dynamicOptions.numPositionSolverIterations = value;
                ApplyOptionsRuntime();
            }
        }

        /// <value>
        ///  The default number of velocity solver iterations for all constraints in each substep. Increase the number of solver iterations will get more accurate simulation about bouncing and dynamic friction.
        /// </value>
        public static uint defaultSolverVelocityIterations
        {
            get
            {
                return s_dynamicOptions.numVelocitySolverIterations;
            }
            set
            {
                if (value == 0)
                {
                    throw new System.ArgumentException("numSolverIter must be greater than 0");
                }

                s_dynamicOptions.numVelocitySolverIterations = value;
                ApplyOptionsRuntime();
            }
        }

        /// <value>
        /// The gravity of the default physics world.
        /// </value>
        public static Vector3 gravity
        {
            get
            {
                return s_dynamicOptions.gravity;
            }
            set
            {
                s_dynamicOptions.gravity = value;
                ApplyOptionsRuntime();
            }
        }

        /// <value>
        /// If false, raycast and overlap test will not work.
        /// </value>
        public static bool isSceneQueryOn
        {
            get
            {
                return s_dynamicOptions.enableSceneQuery;
            }
            set
            {
                s_dynamicOptions.enableSceneQuery = value;
                ApplyOptionsRuntime();
            }
        }

        /// <value>
        /// If false, collision event will not work, such as OnCollisionEnter and so on.
        /// </value>
        public static bool isContactEventOn
        {
            get
            {
                return s_dynamicOptions.enableContactEvent;
            }
            set
            {
                s_dynamicOptions.enableContactEvent = value;
                ApplyOptionsRuntime();
            }
        }

        /// <value>
        /// If true, the engine will use speculative margin to improve the potential collision detection accuracy.
        /// </value>
        /// <value></value>
        public static bool allowExpandSpeculativeMargin
        {
            get
            {
                return s_dynamicOptions.allowExpandSpeculativeMargin;
            }
            set
            {
                s_dynamicOptions.allowExpandSpeculativeMargin = value;
                ApplyOptionsRuntime();
            }
        }

        /// <value>
        /// The default contact offset for newly created colliders.
        /// </value>
        ///
        /// <exception cref="System.ArgumentException">contactOffset must be greater than separationOffset</exception>
        public static float defaultContactOffset
        {
            get
            {
                return s_defaultContactOffset;
            }
            set
            {
                if (value <= s_defaultSeparationOffset)
                {
                    throw new System.ArgumentException("contactOffset must be greater than separationOffset");
                }

                s_defaultContactOffset = value;
            }
        }

        /// <value>
        /// The default separation offset for newly created colliders.
        /// </value>
        ///
        /// <exception cref="System.ArgumentException">separationOffset must be less than contactOffset</exception>
        public static float defaultSeparationOffset
        {
            get
            {
                return s_defaultSeparationOffset;
            }
            set
            {
                if (value >= s_defaultContactOffset)
                {
                    throw new System.ArgumentException("separationOffset must be less than contactOffset");
                }

                s_defaultSeparationOffset = value;
            }
        }

        private static void LoadOptionsFromProjectSettings(PhysicsProjectSettings settings)
        {
            s_dynamicOptions.numPositionSolverIterations = settings.numSolverPositionIterations;
            s_dynamicOptions.numVelocitySolverIterations = settings.numSolverVelocityIterations;
            s_dynamicOptions.numSubSteps = settings.numSubstep;
            s_dynamicOptions.enableSceneQuery = settings.enableSceneQuery;
            s_dynamicOptions.gravity = settings.gravity;
            s_dynamicOptions.enableContactEvent = settings.enableContactEvent;
            simulationMode = settings.simulationMode;
        }

        private static PhysicsEngine s_engine;
        internal static PhysicsEngine engine
        {
            get
            {
                return s_engine;
            }
        }

        internal static PhysicsWorld defaultWorld
        {
            get
            {
                if (engine == null)
                {
                    throw new System.Exception("Engine is not created");
                }

                return engine.defaultWorld;
            }
        }

        internal static bool isEngineCreated
        {
            get
            {
                return engine != null && !engine.isDisposed;
            }
        }

        internal static void ShutDownEngine()
        {
            Rigidbody3D.Dispose();
            BaseCollider.Shutdown();
            if (s_engine != null)
            {
                s_engine.Dispose();
                s_engine = null;
            }
        }

        // LaunchEngine will load PhysicsProjectSettings that configured in the project settings. We can not test it in Unity Test Runner.
        // So exclude it from coverage.
        [UnityEngine.TestTools.ExcludeFromCoverage]
        internal static void LaunchEngine()
        {
            var setting = PhysicsProjectSettings.Instance;
            LoadOptionsFromProjectSettings(setting);
            var builder = PhysicsEngineBuilder.Default;
            builder.worldSimulatorBuilder.setupOptions.broadphaseType = setting.broadPhaseType;
            builder.worldSimulatorBuilder.dynamicOptions = s_dynamicOptions;
            PhysicsNativeApi.mprInitLogger(setting._logLevelFilter, null, null, false);
            // apply plugin defined settings to engine builder
            onEngineCreating?.Invoke(ref builder);
            s_engine = builder.Build();
            Rigidbody3D.Initialize();
        }

        /// <summary>
        /// Destroy the old physics engine and create a new one. This is a dangerous operation, all physics objects will be invalid.
        /// </summary>
        internal static void RestartEngine()
        {
            ShutDownEngine();
            LaunchEngine();
        }

        internal static bool IsTracingStarted
        {
            get => s_tracing_guard == true;
        }

        /// <summary>
        /// <para>
        /// Start tracing in motphys engine, only work if tracing_profiler features of motphys.physics package is on.
        /// Must call <see cref="StartTracing"/> before <see cref="EndTracing"/> or no data will be generated.
        /// </para>
        /// <para>
        /// <b>Caution</b>: once <see cref="StartTracing"/> is called, the trace data collector in native motphys lib will
        /// be set active (which meas some performance loss) until the program is finished and the native motphys dynamic
        /// lib is unloaded, calling <see cref="EndTracing"/> will <b>not</b> terminate this state.
        /// </para>
        /// </summary>
        internal static void StartTracing()
        {
            if (!IsTracingStarted)
            {
                PhysicsNativeApi.mprSetupChromeTracing();
                s_tracing_guard = true;
            }
        }

        /// <summary>
        /// Record tracing data in motphys engine, only work if tracing_profiler features of motphys.physics package is on.
        /// If success, tracing data file with chrome_tracing format can be found in root directory.
        /// Must call <see cref="StartTracing"/> before <see cref="EndTracing"/> or no data will be generated.
        /// </summary>
        /// <exception cref="IOException">Thrown when trace data storage folder not exist.</exception>
        internal static void EndTracing(string traceDataStorageFolder, string traceDataFile)
        {
            if (!Directory.Exists(traceDataStorageFolder))
            {
                throw new IOException(string.Format("Error: trace data storage folder {0} not exist", traceDataStorageFolder));
            }

            if (IsTracingStarted)
            {
                PhysicsNativeApi.mprRecordChromeTracing(Path.Combine(traceDataStorageFolder, traceDataFile));
                s_tracing_guard = false;
            }
        }
    }
}
