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
using UnityEngine;

namespace Motphys.Rigidbody
{

    /// <summary>
    /// The parallel calculation solution for physics engine.
    /// </summary>
    public enum ParallelWorkDispatchType
    {
        /// <summary>
        /// The default parallel calculation solution for physics engine. Currently it is Builtin, may change in the future.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Disable parallel calculation.
        /// </summary>
        Off = 1,

#if MOTPHYS_RIGIDBODY_EXPERIMENTAL
        /// <summary>
        /// Use Unity Job System for parallel calculation.
        /// </summary>
        UnityJobSystem = 2,
#endif

        /// <summary>
        /// Use motphys built-in parallel library for parallel calculation.
        /// </summary>
        Builtin = 3,
    }

    namespace Internal
    {

        /// <summary>
        ///  The pipeline for physics engine. Different pipeline may have different performance and features.
        ///  </summary>
        internal enum PipelineType
        {
            /// <summary>
            ///  The default pipeline for physics engine. It provides the general features and may not have the best performance.
            /// </summary>
            Default = 0,

#if MOTPHYS_RIGIDBODY_EXPERIMENTAL
            /// <summary>
            /// Experimental pipeline is still in development, and may have breaking changes frequently.
            /// </summary>
            Experimental,
#endif
        }

#if MOTPHYS_RIGIDBODY_EXPERIMENTAL
        /// <summary>
        /// stable strategy when stable stack
        /// </summary>
        internal enum StableStrategy
        {
            /// Enable for a global stable stack setting for every scene
            Enable,
            /// Disable for a global unstable stack setting for every scene
            Disable,
            /// Custom for a custom stable stack setting for specific scene
            Custom,
        }
#endif

        [StructLayout(LayoutKind.Sequential)]
        internal struct StableOptions
        {
            /// <summary>
            /// If true, we will use jacobi iteration to solve the contacts in same manifold.
            ///
            /// Note:
            ///
            /// This option is experimental and may have breaking changes in the future. Do not use it in production.
            /// </summary>
            [MarshalAs(UnmanagedType.I1)]
            public bool useJacobiIterationLocally;

            /// <summary>
            /// If true, when solving collision pair, we will ignore the delta correction if it is too small.
            /// </summary>
            [MarshalAs(UnmanagedType.I1)]
            public bool disableSlightChange;

            /// use the average anchor_vec of contact points in a manifold to reduce the amount of rotation
            [MarshalAs(UnmanagedType.I1)]
            public bool allowAveragedAnchor;
            /// sort the collision joint from static object to dynamic object
            [MarshalAs(UnmanagedType.I1)]
            public bool allowContactGraph;
            /// allow the closer an object is to a stationary object, the more difficult it is to move
            [MarshalAs(UnmanagedType.I1)]
            public bool allowShockPropagation;
            /// how difficult it is to move an object closer to static object,
            /// the default value is 1.0f
            public float shockPropagationScale;

#if MOTPHYS_RIGIDBODY_EXPERIMENTAL
            /// <summary>
            ///  Default stable options
            /// </summary>
            public static readonly StableOptions DefaultEnable = new StableOptions()
            {
                useJacobiIterationLocally = false,
                shockPropagationScale = 10.0f,
                disableSlightChange = true,
                allowAveragedAnchor = false,
                allowContactGraph = true,
                allowShockPropagation = true,
            };
#endif

            /// <summary>
            ///  Default unstable options
            /// </summary>
            public static readonly StableOptions DefaultDisable = new StableOptions()
            {
                useJacobiIterationLocally = false,
                shockPropagationScale = 1.0f,
                disableSlightChange = false,
                allowAveragedAnchor = false,
                allowContactGraph = false,
                allowShockPropagation = false,
            };

#if MOTPHYS_RIGIDBODY_EXPERIMENTAL
            /// <summary>
            ///  Custom stable options
            /// </summary>
            public StableOptions(
                bool disableSlightChange, bool allowAveragedAnchor, bool allowContactGraph,
                bool allowShockPropagation, float shockPropagationScale)
            {
                this.useJacobiIterationLocally = false;
                this.disableSlightChange = disableSlightChange;
                this.allowAveragedAnchor = allowAveragedAnchor;
                this.allowContactGraph = allowContactGraph;
                this.allowShockPropagation = allowShockPropagation;
                this.shockPropagationScale = shockPropagationScale;
            }
#endif
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PhysicsEngineBuilder
        {
            public ParallelWorkDispatchType parallelWorkDispatchType;
            public WorldSimulatorBuilder worldSimulatorBuilder;

            public static readonly PhysicsEngineBuilder Default = new PhysicsEngineBuilder()
            {
                parallelWorkDispatchType = ParallelWorkDispatchType.Default,
                worldSimulatorBuilder = WorldSimulatorBuilder.Default,
            };
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WorldSimulatorBuilder
        {
            public PipelineType pipeline;

            public SimulatorSetupOptions setupOptions;
            public SimulatorDynamicOptions dynamicOptions;

            public static readonly WorldSimulatorBuilder Default = new WorldSimulatorBuilder()
            {
                pipeline = PipelineType.Default,
                setupOptions = SimulatorSetupOptions.Default,
                dynamicOptions = SimulatorDynamicOptions.Default,
            };
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SimulatorSetupOptions
        {
            public BroadPhaseType broadphaseType;

            public static readonly SimulatorSetupOptions Default = new SimulatorSetupOptions()
            {
                broadphaseType = BroadPhaseType.GridSAP,
            };
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SimulatorDynamicOptions
        {
            private USize _numSubSteps;
            private USize _numPositionSolverIterations;
            private USize _numVelocitySolverIterations;
            [MarshalAs(UnmanagedType.I1)]
            public bool enableSceneQuery;
            public Vector3 gravity;
            public StableOptions stableOptions;

            [MarshalAs(UnmanagedType.I1)]
            public bool enableContactEvent;
            [MarshalAs(UnmanagedType.I1)]
            public bool allowExpandSpeculativeMargin;
            [MarshalAs(UnmanagedType.I1)]
            public bool usePerSubstepNarrowphase;
            public uint numPositionSolverIterations
            {
                get
                {
                    return _numPositionSolverIterations.ToUInt32();
                }
                set
                {
                    _numPositionSolverIterations = value;
                }
            }

            public uint numVelocitySolverIterations
            {
                get
                {
                    return _numVelocitySolverIterations.ToUInt32();
                }
                set
                {
                    _numVelocitySolverIterations = value;
                }
            }

            public uint numSubSteps
            {
                get
                {
                    return _numSubSteps.ToUInt32();
                }
                set
                {
                    _numSubSteps = value;
                }
            }

            public static readonly SimulatorDynamicOptions Default = new SimulatorDynamicOptions()
            {
                numSubSteps = 5,
                _numPositionSolverIterations = 1,
                enableSceneQuery = true,
                gravity = new Vector3(0, -9.8f, 0),
                stableOptions = StableOptions.DefaultDisable,
                enableContactEvent = true,
                allowExpandSpeculativeMargin = false,
                usePerSubstepNarrowphase = true,
            };
        }

        internal enum ResultCode
        {
            Ok,
            InvalidArguement,
            BodyNotFound,
            JointNotFound,
            ContactNotFound,
            InvalidOperation,
            NotSupported,
            WorldNotFound,
            JointMotorNotSupported,
            ConvexHullNotFound,
            FeatureDisabled,
            VoxelShapeNotFound,
            SceneQueryIsOff,
            CapacityNotEnough,
            ConvexCreatePointCountLessThanFour,
            ConvexCreateVolumeTooSmall,
            ColliderNotFound,
            ConvexCreateDegenerate,
            ConvexCreatePointCountOverflow,
            ConvexCreateFaceCountLimitedHit,
            ParametersOutOfRange,
        }

        internal static class PhysicsInternalExtensions
        {
            public static void ThrowExceptionIfNotOk(this ResultCode code)
            {
                if (code != ResultCode.Ok)
                {
                    throw new System.Exception("Physics Engine Exception:" + code);
                }
            }
        }
    }
}
