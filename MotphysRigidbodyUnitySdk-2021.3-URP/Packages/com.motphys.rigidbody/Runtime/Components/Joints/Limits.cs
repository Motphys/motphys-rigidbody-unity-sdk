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
    /// A angular limit.
    /// </summary>
    [System.Serializable]
    public struct JointAngularLimit
    {
        [Range(-180, 180)]
        [SerializeField]
        [Tooltip("The low bound of the limit in degrees.")]
        private float _low;

        [Range(-180, 180)]
        [SerializeField]
        [Tooltip("The high bound of the limit in degrees.")]
        private float _high;

        /// <summary>
        /// Create a limit with the given low and high bounds.
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown if the low bound is less than -180, the high bound is greater than 180, or the low bound is greater than the high bound.</exception>
        public JointAngularLimit(float low, float high)
        {
            if (low < -180 || low > high || high > 180)
            {
                throw new System.ArgumentException($"low must be in [-180, high], high must be in [low, 180]. (low = {low}, high = {high})");
            }

            _low = low;
            _high = high;
        }

        /// <value>
        /// The low bound of the limit in degrees.
        /// </value>
        public float low
        {
            get { return _low; }
        }

        /// <value>
        /// The high bound of the limit in degrees.
        /// </value>
        public float high
        {
            get { return _high; }
        }

        /// <value>
        /// The span of the limit in degrees. Calculated as high - low.
        /// </value>
        public float span
        {
            get { return _high - _low; }
        }

        /// <summary>
        /// Clamp the low and high bounds to valid values.
        /// </summary>
        public void Validate()
        {
            _low = Mathf.Clamp(_low, -180, 180);
            _high = Mathf.Clamp(_high, _low, 180);
        }

        internal MinMax ToMinMaxRad()
        {
            Debug.Assert(_low >= -180 && _low <= _high);
            Debug.Assert(_high >= _low && _high <= 180);
            return new MinMax(_low * Mathf.Deg2Rad, _high * Mathf.Deg2Rad);
        }

        internal void ClampTo180NP()
        {
            _low = Mathf.Clamp(_low, -180, 0);
            _high = Mathf.Clamp(_high, 0, 180);
        }

        internal void ClampTo179()
        {
            _low = Mathf.Clamp(_low, -179, 179);
            _high = Mathf.Clamp(_high, _low, 179);
        }

        /// <value>
        /// A default limit in [-180,180].
        /// </value>
        public static readonly JointAngularLimit Default = new JointAngularLimit(-180f, 180f);
        /// <value>
        /// A zero limit in [0,0].
        /// </value>
        public static readonly JointAngularLimit ZERO = new JointAngularLimit(0, 0);
        /// <value>
        /// An one limit in [-1,1].
        /// </value>
        public static readonly JointAngularLimit One = new JointAngularLimit(-1f, 1f);
    }

    /// <summary>
    /// Linear Translation limit for a joint.
    /// </summary>
    [System.Serializable]
    public struct JointTranslationLimit
    {
        [SerializeField]
        [Tooltip("The low bound of the limit in meters.")]
        private float _low;

        [SerializeField]
        [Tooltip("The high bound of the limit in meters.")]
        private float _high;

        /// <summary>
        /// Create a translation limit with the given low and high bounds.
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown if the low bound is greater than the high bound.</exception>
        public JointTranslationLimit(float low, float high)
        {
            if (low > high)
            {
                throw new System.ArgumentException($"low must be less than or equal to high (low = {low}, high = {high})");
            }

            _low = low;
            _high = high;
        }

        /// <value>
        /// The low bound of the limit in meters.
        /// </value>
        public float low
        {
            get { return _low; }
        }

        /// <value>
        /// The high bound of the limit in meters.
        /// </value>
        public float high
        {
            get { return _high; }
        }

        /// <summary>
        /// Clamp the low and high bounds to valid values.
        /// </summary>
        public void Validate()
        {
            _high = Mathf.Max(_high, _low);
        }
    }
}
