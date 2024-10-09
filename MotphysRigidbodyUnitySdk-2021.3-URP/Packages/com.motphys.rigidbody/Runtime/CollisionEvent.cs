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
    /// Describes a contact point where the collision occurs. ContactPoint is used in CollisionEvent and Collision structure.
    /// </summary>
    ///
    /// <seealso cref="CollisionEvent"/>
    /// <seealso cref="Collision"/>
    public struct ContactPoint
    {
        /// <value>
        /// The first collider's contact position in world space.
        /// </value>
        public Vector3 point;

        /// <value>
        /// The contact normal in world space. Direction is toward outside of the first collider.
        /// </value>
        public Vector3 normal;

        /// <value>
        /// The separation distance of the contact point of two colliders. Always negative when two colliders are colliding.
        /// </value>
        public float separation;

        [UnityEngine.TestTools.ExcludeFromCoverage]
        public override string ToString()
        {
            return $"point = {point:0.000}, normal = {normal:0.000}, separation = {separation}";
        }
    }

    /// <summary>
    /// A Collision event is fired when two colliders collide. The event contains information about the colliders involved in the collision.
    /// </summary>
    public struct CollisionEvent
    {
        internal ColliderId id1
        {
            get; private set;
        }

        internal ColliderId id2
        {
            get; private set;
        }

        /// <value>
        /// The first collider involved in the collision.
        /// </value>
        public BaseCollider collider1;

        /// <value>
        /// The second collider involved in the collision.
        /// </value>
        public BaseCollider collider2;

        private ContactManifold _manifold;
        internal CollisionEvent(
            ColliderId id1,
            ColliderId id2,
            BaseCollider body1,
            BaseCollider body2
        )
        {
            this.id1 = id1;
            this.id2 = id2;
            collider1 = body1;
            collider2 = body2;
            _manifold = new ContactManifold(0);
        }

        /// <summary>
        /// Query Manifold with local pair
        /// </summary>
        internal ContactManifold GetManifold()
        {
            if (_manifold.isInvalid())
            {
                _manifold = PhysicsManager.defaultWorld.QueryCollisionEvent(new ColliderPair(id1, id2));
            }

            return _manifold;
        }

        /// <summary>
        /// Get Contact in ContactManifold by index
        /// </summary>
        internal ContactPoint GetContact(int index)
        {
            ContactManifold manifold = GetManifold();
            var contact = manifold.GetContact(index);
            var separation = -(contact.worldPositionB - contact.worldPositionA).magnitude;
            return new ContactPoint()
            {
                point = contact.worldPositionA,
                normal = contact.normal,
                separation = separation,
            };
        }

        /// <value>
        /// The contact number
        /// </value>
        public int contactCount
        {
            get
            {
                ContactManifold manifold = GetManifold();
                return (int)manifold.numPoints.ToUInt32();
            }
        }
    }

    /// <summary>
    /// Collision information. This struct is passed to the collision event handlers.
    /// </summary>
    public struct Collision
    {
        /// <value>
        /// The collider that was hit. NOTE: this might be null if the rigidbody is not attached to a collider.
        /// </value>
        public BaseCollider collider { get; private set; }

        /// <value>
        /// The rigidbody that was hit. NOTE: this might be null if the collider is not attached to a rigidbody.
        /// </value>
        public Rigidbody3D rigidbody => collider.attachedRigidbody;

        /// <value>
        /// The gameObject that was hit.
        /// </value>
        public GameObject gameObject => collider.gameObject;

        /// <value>
        /// The transform of the gameObject that was hit.
        /// </value>
        public Transform transform => collider.transform;

        private ContactManifold _manifold;

        private ColliderPair _pair;

        /// <summary>
        /// Initializing a collision structure.
        /// </summary>
        internal Collision(BaseCollider collider, ColliderPair pair)
        {
            this.collider = collider;
            _manifold = new ContactManifold(0);
            _pair = pair;
        }

        /// <summary>
        /// Query Manifold with local pair
        /// </summary>
        internal ContactManifold GetManifold()
        {
            if (_manifold.isInvalid())
            {
                _manifold = PhysicsManager.defaultWorld.QueryCollisionEvent(_pair);
            }

            return _manifold;
        }

        /// <summary>
        /// Get Contact in ContactManifold by index
        /// </summary>
        public ContactPoint GetContact(int index)
        {
            ContactManifold manifold = GetManifold();

            var contact = manifold.GetContact(index);
            var separation = -(contact.worldPositionB - contact.worldPositionA).magnitude;
            return new ContactPoint()
            {
                point = contact.worldPositionA,
                normal = contact.normal,
                separation = separation,
            };
        }

        /// <value>
        /// The contact number
        /// </value>
        public int contactCount
        {
            get
            {
                ContactManifold manifold = GetManifold();
                return (int)manifold.numPoints.ToUInt32();
            }
        }
    }

    /// <summary>
    /// A Trigger event is fired when two colliders collide. At least one of the colliders must be marked as a trigger collider.
    /// </summary>
    public struct TriggerEvent
    {
        /// <value>
        /// The first collider involved in the trigger event.
        /// </value>
        public BaseCollider collider1
        {
            get; private set;
        }

        /// <value>
        /// The second collider involved in the trigger event.
        /// </value>
        public BaseCollider collider2
        {
            get; private set;
        }

        //TODO: improve unit test (http://gitlab.mp/motphys/motphys_sdk_remake/-/issues/697)
        internal TriggerEvent(
            BaseCollider body1,
            BaseCollider body2
        )
        {
            this.collider1 = body1;
            this.collider2 = body2;
        }

        internal ColliderId id1 => collider1.id;
        internal ColliderId id2 => collider2.id;
    }
}
