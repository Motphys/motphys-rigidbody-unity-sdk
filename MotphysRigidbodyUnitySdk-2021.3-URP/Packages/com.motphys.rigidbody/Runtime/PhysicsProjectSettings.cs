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
using Motphys.Settings;
using UnityEngine;
using UnityEngine.TestTools;

namespace Motphys.Rigidbody
{
    /// <summary>
    /// The physics project setting.
    /// </summary>
    [System.Serializable]
    public class PhysicsProjectSettings : CustomProjectSettings
    {
        /// <value>
        /// The physics setting instance.
        /// </value>
        public static PhysicsProjectSettings Instance
        {
            get
            {
#if UNITY_EDITOR
                return CustomProjectSettings.Get<PhysicsProjectSettings>();
#else
                if (s_instance == null)
                {
                    return CustomProjectSettings.CreateInstance<PhysicsProjectSettings>();
                }
                else
                {
                    return s_instance;
                }
#endif
            }
        }

        private static PhysicsProjectSettings s_instance;
        private void OnEnable()
        {
            s_instance = this;
        }

        [SerializeField]
        private BroadPhaseType _broadPhaseType = BroadPhaseType.GridSAP;

        [SerializeField]
        [Range(1, 20)]
        [Tooltip("The number of substeps per physics step. Improve the numSubstep will improve the stability and accuracy of the simulation but will also increase the computation cost.")]
        private uint _numSubstep = 2;

        [SerializeField]
        [Range(1, 20)]
        [Tooltip("The number of position solver iterations for all constraints in each substep. Increase the number of solver iterations will improve the stability and accuracy of the simulation but will also increase the computation cost.")]
        private uint _numSolverIter = 1;

        [SerializeField]
        [Range(1, 20)]
        [Tooltip("The number of velocity solver iterations for all constraints in each substep. Increase the number of solver iterations will get more accurate simulation about bouncing and dynamic friction.")]
        private uint _numSolverVelocityIter = 1;

        [SerializeField]
        private Vector3 _gravity = new Vector3(0, -9.8f, 0);

        [SerializeField]
        private SimulationMode _simulationMode = SimulationMode.FixedUpdate;

        [SerializeField]
        [Tooltip("If false, overlap test and raycast will not work.")]
        private bool _enableSceneQuery = true;

        [SerializeField]

        [Tooltip("If false, engine will not produce collision events.")]
        private bool _enableContactEvent = true;

        [SerializeField]
        [Tooltip("If true, the engine will expand all colliders' speculative margin to provide a wider range of collision detection.")]
        private bool _allowExpandSpeculativeMargin = false;

        [SerializeField]
        [Tooltip("The maximum level of log information displayed by the engine in the Unity console. You need to restart Unity after each modification for the changes to take effect.")]
        internal LogLevelFilter _logLevelFilter = LogLevelFilter.Off;

        [SerializeField]
        [HideInInspector]
        private uint[] _layerCollidesWith = new uint[MaxLayers];

        internal const int MaxLayers = 32;
        internal const uint FullLayerCollisionMask = 0xFFFFFFFF;

        public PhysicsProjectSettings()
        {
            for (var i = 0; i < MaxLayers; i++)
            {
                _layerCollidesWith[i] = FullLayerCollisionMask;
            }
        }

        /// <summary>
        /// Ignore all collisions between any collider in layerA and any collider in layerB.
        /// It does not affect the generated colliders temporarily.
        /// </summary>
        /// <param name="layerA">Valid layer is in [0, 31]</param>
        /// <param name="layerB">Valid layer is in [0, 31]</param>
        /// <param name="ignore"></param>
        /// <returns>Return false if the input layer is invalid </returns>
        public bool IgnoreLayerCollision(int layerA, int layerB, bool ignore = true)
        {
            if (layerA < 0 || layerA >= MaxLayers || layerB < 0 || layerB >= MaxLayers)
            {
                return false;
            }

            var layerAMask = this.GetCollisionMask(layerA);
            var bitA = 1u << layerB;
            if (ignore)
            {
                layerAMask &= ~bitA;
            }
            else
            {
                layerAMask |= bitA;
            }

            this.SetCollisionMask(layerA, layerAMask);

            if (layerA == layerB)
            {
                return true;
            }

            var layerBMask = this.GetCollisionMask(layerB);
            var bitB = 1u << layerA;

            if (ignore)
            {
                layerBMask &= ~bitB;
            }
            else
            {
                layerBMask |= bitB;
            }

            this.SetCollisionMask(layerB, layerBMask);

            return true;
        }

        /// <summary>
        /// Set all layers' collision mask.
        /// True: Enable collisions between all layers.
        /// False: Disable collisions between all layers.
        /// </summary>
        /// <param name="enable"></param>
        public void SetAllLayersCollisionMask(bool enable)
        {
            for (var i = 0; i < MaxLayers; i++)
            {
                if (enable)
                {
                    SetCollisionMask(i, FullLayerCollisionMask);
                }
                else
                {
                    SetCollisionMask(i, 0);
                }
            }
        }

        /// <summary>
        /// Get a collision mask for a specific layer.
        /// </summary>
        /// <param name="layer">Valid layer is in [0, 31]</param>
        /// <returns>Return 0 if the input layer is invalid</returns>
        public uint GetCollisionMask(int layer)
        {
            if (layer >= 0 && layer < MaxLayers)
            {
                return _layerCollidesWith[layer];
            }

            return 0;
        }

        private void SetCollisionMask(int layer, uint layerMask)
        {
            if (layer >= 0 && layer < MaxLayers)
            {
                _layerCollidesWith[layer] = layerMask;
            }
        }

        internal BroadPhaseType broadPhaseType
        {
            get
            {
                return _broadPhaseType;
            }
            set
            {
                _broadPhaseType = value;
            }
        }

        /// <value>
        /// The number of substeps per physics step. Improve the numSubstep will improve the stability and accuracy of the simulation but will also increase the computation cost.
        /// </value>
        public uint numSubstep
        {
            get
            {
                return _numSubstep;
            }
            set
            {
                _numSubstep = System.Math.Max(value, 1);
            }
        }

        /// <value>
        /// The number of position solver iterations for all constraints in each substep. Increase the number of solver iterations will improve the stability and accuracy of the simulation but will also increase the computation cost.
        /// </value>
        public uint numSolverPositionIterations
        {
            get
            {
                return _numSolverIter;
            }
            set
            {
                _numSolverIter = System.Math.Max(value, 1);
            }
        }

        /// <value>
        /// The number of velocity solver iterations for all constraints in each substep. Increase the number of solver iterations will get more accurate simulation about bouncing and dynamic friction.
        /// </value>
        public uint numSolverVelocityIterations
        {
            get
            {
                return _numSolverVelocityIter;
            }
            set
            {
                _numSolverVelocityIter = System.Math.Max(value, 1);
            }
        }

        /// <value>
        /// The gravity of the default physics world.
        /// </value>
        public Vector3 gravity
        {
            get
            {
                return _gravity;
            }
            set
            {
                _gravity = value;
            }
        }

        /// <value>
        /// The simulation mode how the engine updates.
        /// </value>
        public SimulationMode simulationMode
        {
            get
            {
                return _simulationMode;
            }
        }

        /// <value>
        /// If false, overlap test and raycast will not work.
        /// </value>
        public bool enableSceneQuery
        {
            get
            {
                return _enableSceneQuery;
            }
            set
            {
                _enableSceneQuery = value;
            }
        }

        /// <value>
        /// If false, engine will not produce collision events.
        /// </value>
        public bool enableContactEvent
        {
            get
            {
                return _enableContactEvent;
            }
            set
            {
                _enableContactEvent = value;
            }
        }

        /// <value>
        /// If true, the engine will expand all colliders' speculative margin to provide a wider range of collision detection.
        /// </value>
        public bool allowExpandSpeculativeMargin
        {
            get
            {
                return _allowExpandSpeculativeMargin;
            }
            set
            {
                _allowExpandSpeculativeMargin = value;
            }
        }

        [ExcludeFromCoverage]
        public void OnValidate()
        {
            numSolverPositionIterations = numSolverPositionIterations;
            numSubstep = numSubstep;
        }

        [ExcludeFromCoverage]
        protected override void OnLoaded()
        {
        }

        [ExcludeFromCoverage]
        protected override void OnWillSave()
        {
        }
    }
}
