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
    /// Physics materials, typically used to configure elasticity and friction.
    /// </summary>
    [CreateAssetMenu(menuName = "Motphys/PhysicMaterial")]
    public class PhysicMaterial : ScriptableObject
    {
        [SerializeField]
        private MaterialData _data = MaterialData.Default;

        internal int version
        {
            get; private set;
        }

        internal MaterialData data
        {
            get { return _data; }
        }

        /// <value>
        /// The strategy to combine bounciness of two materials.
        /// </value>
        public MaterialCombine bounceCombine
        {
            get { return _data.bounceCombine; }
            set
            {
                _data.bounceCombine = value;
                version++;
            }
        }

        /// <value>
        /// This value determines how much an object bounces back after a collision.
        /// The valid range of bounciness is [0, 1]. 0 means no bounce, 1 means full bounce.
        /// Note: Value will be automatically clamped to [0, 1] if it is set outside the range.
        /// </value>
        public float bounciness
        {
            get { return _data.bounciness; }
            set
            {
                _data.bounciness = value;
                version++;
            }
        }

        /// <value>
        /// The strategy to combine dynamic and static friction of two materials.
        /// </value>
        /// <seealso cref="MaterialCombine"/>
        public MaterialCombine frictionCombine
        {
            get { return _data.frictionCombine; }
            set
            {
                _data.frictionCombine = value;
                version++;
            }
        }

        /// <value>
        /// More larger value means more difficult to move the body when it is static on a surface.
        ///
        ///  Note: if the set value is smaller than dynamicFriction, dynamicFriction will be set to the same value as staticFriction.
        /// </value>
        ///
        /// <exception cref="System.ArgumentException">Thrown when the set value is negative.</exception>
        public float staticFriction
        {
            get { return _data.staticFriction; }
            set
            {
                _data.staticFriction = value;
            }
        }

        /// <value>
        /// More larger value means more faster to stop the body when it is moving on a surface.
        ///
        /// Note: if the set value is larger than staticFriction, staticFriction will be set to the same value as dynamicFriction.
        /// </value>
        /// <exception cref="System.ArgumentException">Thrown when the set value is negative.</exception>
        public float dynamicFriction
        {
            get { return _data.dynamicFriction; }
            set
            {
                _data.dynamicFriction = value;
            }
        }

        /// <summary>
        /// Update the dynamic and static friction of the material.
        /// </summary>
        ///
        /// <exception cref="System.ArgumentException"> thrown if either dynamicFriction or staticFriction is negative, or dynamicFriction is greater than staticFriction </exception>
        public void SetFrictions(float dynamicFriction, float staticFriction)
        {
            _data.SetFrictions(dynamicFriction, staticFriction);
            version++;
        }

        /// <value>
        /// If true, collision event will be triggered when this body collides with other bodies. Disable it if you don't need collision event for this body to improve performance
        /// </value>
        public bool enableCollisionEvent
        {
            get { return _data.enableCollisionEvent; }
            set { _data.enableCollisionEvent = value; }
        }

        /// <value>
        /// The strategy to combine `enableCollisionEvent` property of two materials.
        /// </value>
        public MaterialBoolCombine collisionEventCombine
        {
            get { return _data.collisionEventCombine; }
            set
            {
                _data.collisionEventCombine = value;
                version++;
            }
        }

        /// <summary>
        /// OnValidate method is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
        /// It's hard to test this method, so we exclude it from coverage.
        /// </summary>
        [UnityEngine.TestTools.ExcludeFromCoverage]
        private void OnValidate()
        {
            _data.Validate();
            version++;
        }

        public PhysicMaterial Clone()
        {
            var material = Instantiate<PhysicMaterial>(this);
            material.version = version;
            return material;
        }
    }
}
