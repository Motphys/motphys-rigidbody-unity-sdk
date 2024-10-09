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

using System.Runtime.CompilerServices;
using Motphys.Rigidbody.Internal;
using UnityEngine;

namespace Motphys.Rigidbody
{
    /// <summary>
    /// Structure used to get information back from a raycast.
    /// </summary>
    public struct RaycastHit
    {
        /// <value>
        /// The distance from the ray's origin to the hit.
        /// </value>
        public float distance;

        /// <value>
        /// The normal of the surface the raycast hit.
        /// </value>
        public Vector3 normal;

        /// <value>
        /// The position of the surface the raycast hit.
        /// </value>
        public Vector3 point;

        /// <value>
        /// The rigidbody of the collider that was hit. If the collider is not bound to a rigidbody, this value is null.
        /// </value>
        public Rigidbody3D rigidbody;

        /// <value>
        /// The collider that was hit.
        /// </value>
        public BaseCollider collider;

        internal RaycastHit(bool isHit, Internal.RaycastHit hitinfo, Vector3 normalizedDirection, Vector3 origin)
        {
            point = origin + normalizedDirection * hitinfo.distance;
            distance = hitinfo.distance;
            normal = hitinfo.normal;
            collider = isHit ? BaseCollider.Get(hitinfo.id) : null;
            rigidbody = collider != null ? collider.attachedRigidbody : null;
        }

        internal RaycastHit(Internal.RaycastHit hitinfo, Vector3 normalizedDirection, Vector3 origin)
        {
            point = origin + normalizedDirection * hitinfo.distance;
            distance = hitinfo.distance;
            normal = hitinfo.normal;
            collider = BaseCollider.Get(hitinfo.id);
            rigidbody = collider.attachedRigidbody;
        }
    }

    public partial class PhysicsManager
    {
        /// <value>
        /// Ignore raycast is layer2 in unity.
        /// That means the layer mask of all layers without layer2 is 0xFFFFFFFB, which is -5 in decimal.
        /// </value>
        public const int DefaultRaycastLayers = SceneQueryContext.DefaultRaycastLayers;

        private const uint MAX_QUERY_RESULTS = 2048;
        private static Internal.RaycastHit[] s_raycastHitResultBuffer = new Internal.RaycastHit[MAX_QUERY_RESULTS];
        private static ColliderId[] s_colliderIdBuffer = new ColliderId[MAX_QUERY_RESULTS];

        /// <summary>
        /// Casts a ray against the scene and return the closest hit.
        /// </summary>
        /// <param name="origin">The starting position of the ray.</param>
        /// <param name="direction">The direction of the ray.</param>
        /// <param name="maxDistance">Hits further than `max_distance` will not be reported.</param>
        /// <param name="layerMask">The target layers to query</param>
        /// <param name="queryTrigger">Whether the trigger should be reported.</param>
        /// <param name="sceneQueryFlags">What type of object should be reported</param>
        /// <param name="hit">>If the ray hit something, <c>hit</c> whill contain details of the closest hit.</param>
        /// <returns>Whether an object was hit by the ray</returns>
        /// <example>
        /// <code>
        /// PhysicsManager.RaycastClosest(new Vector3(0.0f, 0.0,f 0.0f), new Vector3(0.0f, 0.0,f 1.0f), out RaycastHit hit, 100.0f, -1, true, SceneQueryFlags.All);
        /// </code>
        /// </example>
        public static bool RaycastClosest(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance = float.MaxValue, int layerMask = DefaultRaycastLayers, bool queryTrigger = true, SceneQueryFlags sceneQueryFlags = SceneQueryFlags.All)
        {
            direction.Normalize();
            var context = new SceneQueryContext(sceneQueryFlags, queryTrigger, layerMask);

            var result = defaultWorld.RaycastClosest(origin, direction, maxDistance, context);
            hit = new RaycastHit(result.HasValue, result.GetValueOrDefault(), direction, origin);
            return result.HasValue;
        }

        /// <summary>
        /// Casts a ray against the scene and immediately returns a hit, if any object is hit.
        /// The hit is not guaranteed to be the closest.
        /// </summary>
        /// <param name="origin">The starting position of the ray.</param>
        /// <param name="direction">The direction of the ray.</param>
        /// <param name="hit">If the ray hit something, <c>hit</c> whill contain details of the hit.</param>
        /// <param name="maxDistance">Hits further than `max_distance` will not be reported.</param>
        /// <param name="layerMask">The target layers to query</param>
        /// <param name="queryTrigger">Whether the trigger should be reported.</param>
        /// <param name="sceneQueryFlags">What type of object should be reported</param>
        /// <returns>Whether an object was hit by the ray</returns>
        /// <example>
        /// <code>
        /// PhysicsManager.RaycastAny(new Vector3(0.0f, 0.0,f 0.0f), new Vector3(0.0f, 0.0,f 1.0f), out RaycastHit hit, 100.0f, -1, true, SceneQueryFlags.All);
        /// </code>
        /// </example>
        public static bool RaycastAny(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance = float.MaxValue, int layerMask = DefaultRaycastLayers, bool queryTrigger = true, SceneQueryFlags sceneQueryFlags = SceneQueryFlags.All)
        {
            direction.Normalize();
            var context = new SceneQueryContext(sceneQueryFlags, queryTrigger, layerMask);

            var result = defaultWorld.RaycastAny(origin, direction, maxDistance, context);
            hit = new RaycastHit(result.HasValue, result.GetValueOrDefault(), direction, origin);
            return result.HasValue;
        }

        /// <summary>
        /// Casts a ray against the scene and write the hits to <c>results</c>, and report up to 2048 results
        /// </summary>
        /// <param name="origin">The starting position of the ray.</param>
        /// <param name="direction">The direction of the ray.</param>
        /// <param name="results">If the ray hit something, <c>results</c> whill contain details of hits.</param>
        /// <param name="maxDistance">Hits further than `max_distance` will not be reported.</param>
        /// <param name="layerMask">The target layers to query</param>
        /// <param name="queryTrigger">Whether the trigger should be reported.</param>
        /// <param name="sceneQueryFlags">What type of object should be reported</param>
        /// <returns>Number of hits</returns>
        public static uint RaycastAllNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results, float maxDistance = float.MaxValue, int layerMask = DefaultRaycastLayers, bool queryTrigger = true, SceneQueryFlags sceneQueryFlags = SceneQueryFlags.All)
        {
            direction.Normalize();

            var context = new SceneQueryContext(sceneQueryFlags, queryTrigger, layerMask);

            var maxHits = Mathf.Min(s_raycastHitResultBuffer.Length, results.Length);
            var numHits = defaultWorld.RaycastAllNonAlloc(origin, direction, maxDistance, (uint)maxHits, s_raycastHitResultBuffer, context);

            for (var i = 0; i < numHits; i++)
            {
                var result = s_raycastHitResultBuffer[i];
                results[i] = new RaycastHit(result, direction, origin);
            }

            return numHits;
        }

        // **************** Box ******************//
        /// <summary>
        /// Find objects that intersect with the given box and write the overlaps to <c>results</c>, and report up to 2048 results
        /// </summary>
        /// <param name="center">Center of the given box.</param>
        /// <param name="halfExtents">Half the size of the given box.</param>
        /// <param name="results">All objects that intersect the given box.</param>
        /// <param name="orientation">Rotation of the given box.</param>
        /// <param name="layerMask">The target layers to query</param>
        /// <param name="queryTrigger">Whether the trigger should be reported.</param>
        /// <param name="sceneQueryFlags">What type of object should be reported</param>
        /// <returns>The number of objects intersecting the given box</returns>
        public static uint OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, BaseCollider[] results, Quaternion orientation, int layerMask = DefaultRaycastLayers, bool queryTrigger = true, SceneQueryFlags sceneQueryFlags = SceneQueryFlags.All)
        {
            var shape = new Cuboid { halfExt = halfExtents };
            var maxOverlaps = Mathf.Min(s_colliderIdBuffer.Length, results.Length);
            var context = new SceneQueryContext(sceneQueryFlags, queryTrigger, layerMask);

            var numOverlaps = defaultWorld.OverlayTestNonAlloc(shape, new Isometry { position = center, rotation = orientation }, (uint)maxOverlaps, s_colliderIdBuffer, context);

            for (var i = 0; i < numOverlaps; i++)
            {
                var bodyId = s_colliderIdBuffer[i];
                results[i] = BaseCollider.Get(bodyId);
            }

            return numOverlaps;
        }

        /// <summary>
        /// Test whether there is an object intersecting the given box in the scene.
        /// </summary>
        /// <param name="center">Center of the given box.</param>
        /// <param name="halfExtents">Half the size of the given box.</param>
        /// <param name="result">The object that intersect the given box.</param>
        /// <param name="orientation">Rotation of the given box.</param>
        /// <param name="layerMask">The target layers to query</param>
        /// <param name="queryTrigger">Whether the trigger should be reported.</param>
        /// <param name="sceneQueryFlags">What type of object should be reported</param>
        /// <returns>True if an object intersects the given box</returns>
        public static bool CheckBox(Vector3 center, Vector3 halfExtents, out BaseCollider result, Quaternion orientation, int layerMask = DefaultRaycastLayers, bool queryTrigger = true, SceneQueryFlags sceneQueryFlags = SceneQueryFlags.All)
        {
            var shape = new Cuboid { halfExt = halfExtents };
            var context = new SceneQueryContext(sceneQueryFlags, queryTrigger, layerMask);

            var numOverlaps = defaultWorld.OverlayTestNonAlloc(shape, new Isometry { position = center, rotation = orientation }, 1, s_colliderIdBuffer, context);

            result = numOverlaps > 0 ? BaseCollider.Get(s_colliderIdBuffer[0]) : null;

            return numOverlaps > 0;
        }

        // **************** Sphere ******************//

        /// <summary>
        /// Find objects that intersect with the given sphere and write the overlaps to <c>results</c>, and report up to 2048 results
        /// </summary>
        /// <param name="center">Center of the given sphere.</param>
        /// <param name="results">All objects that intersect the given sphere.</param>
        /// <param name="layerMask">The target layers to query</param>
        /// <param name="queryTrigger">Whether the trigger should be reported.</param>
        /// <param name="sceneQueryFlags">What type of object should be reported</param>
        /// <returns>The number of objects intersecting the given sphere</returns>
        public static uint OverlapSphereNonAlloc(Vector3 center, float radius, BaseCollider[] results, int layerMask = DefaultRaycastLayers, bool queryTrigger = true, SceneQueryFlags sceneQueryFlags = SceneQueryFlags.All)
        {
            var shape = new Sphere { radius = radius };
            var maxOverlaps = Mathf.Min(s_colliderIdBuffer.Length, results.Length);
            var context = new SceneQueryContext(sceneQueryFlags, queryTrigger, layerMask);

            var numOverlaps = defaultWorld.OverlayTestNonAlloc(shape, new Isometry(center, Quaternion.identity), (uint)maxOverlaps, s_colliderIdBuffer, context);

            for (var i = 0; i < numOverlaps; i++)
            {
                var bodyId = s_colliderIdBuffer[i];
                results[i] = BaseCollider.Get(bodyId);
            }

            return numOverlaps;
        }

        /// <summary>
        /// Test whether there is an object intersecting the given sphere in the scene.
        /// </summary>
        /// <param name="center">Center of the given sphere.</param>
        /// <param name="result">The object that intersect the given sphere.</param>
        /// <param name="layerMask">The target layers to query</param>
        /// <param name="queryTrigger">Whether the trigger should be reported.</param>
        /// <param name="sceneQueryFlags">What type of object should be reported</param>
        /// <returns>True if an object intersects the given sphere</returns>
        public static bool CheckSphere(Vector3 center, float radius, out BaseCollider result, int layerMask = DefaultRaycastLayers, bool queryTrigger = true, SceneQueryFlags sceneQueryFlags = SceneQueryFlags.All)
        {
            var shape = new Sphere { radius = radius };
            var context = new SceneQueryContext(sceneQueryFlags, queryTrigger, layerMask);

            var numOverlaps = defaultWorld.OverlayTestNonAlloc(shape, new Isometry(center, Quaternion.identity), 1, s_colliderIdBuffer, context);

            result = numOverlaps > 0 ? BaseCollider.Get(s_colliderIdBuffer[0]) : null;

            return numOverlaps > 0;
        }

        // **************** Capsule ******************//
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, SceneQueryContext context, uint maxOverlaps)
        {
            var position = (point0 + point1) * 0.5f;
            var segment = point1 - point0;
            var halfHeight = Vector3.SqrMagnitude(segment) * 0.5f;
            var direction = Vector3.Normalize(segment);
            var rotation = Quaternion.FromToRotation(Vector3.up, direction);
            var shape = new Capsule { halfHeight = halfHeight, radius = radius };

            return defaultWorld.OverlayTestNonAlloc(shape, new Isometry(position, rotation), maxOverlaps, s_colliderIdBuffer, context);
        }

        /// <summary>
        /// Find objects that intersect with the given capsule and write the overlaps to <c>results</c>, and report up to 2048 results
        /// </summary>
        /// <param name="point0">The center of the sphere at the start of the capsule.</param>
        /// <param name="point1">The center of the sphere at the end of the capsule.</param>
        /// <param name="radius">The radius of the capsule.</param>
        /// <param name="results">All objects that intersect the given capsule.</param>
        /// <param name="layerMask">The target layers to query</param>
        /// <param name="queryTrigger">Whether the trigger should be reported.</param>
        /// <param name="sceneQueryFlags">What type of object should be reported</param>
        /// <returns>The number of objects intersecting the given capsule</returns>
        public static uint OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, BaseCollider[] results, int layerMask = DefaultRaycastLayers, bool queryTrigger = true, SceneQueryFlags sceneQueryFlags = SceneQueryFlags.All)
        {
            var maxOverlaps = Mathf.Min(s_colliderIdBuffer.Length, results.Length);

            var context = new SceneQueryContext(sceneQueryFlags, queryTrigger, layerMask);

            var numOverlaps = OverlapCapsuleNonAlloc(point0, point1, radius, context, (uint)maxOverlaps);

            for (var i = 0; i < numOverlaps; i++)
            {
                var bodyId = s_colliderIdBuffer[i];
                results[i] = BaseCollider.Get(bodyId);
            }

            return numOverlaps;
        }

        /// <summary>
        /// Test whether there is an object intersecting the given capsule in the scene.
        /// </summary>
        /// <param name="point0">The center of the sphere at the start of the capsule.</param>
        /// <param name="point1">The center of the sphere at the end of the capsule.</param>
        /// <param name="radius">The radius of the capsule.</param>
        /// <param name="result">The object that intersect the given capsule.</param>
        /// <param name="layerMask">The target layers to query</param>
        /// <param name="queryTrigger">Whether the trigger should be reported.</param>
        /// <param name="sceneQueryFlags">What type of object should be reported</param>
        /// <returns>True if an object intersects the given capsule</returns>
        public static bool CheckCapsule(Vector3 point0, Vector3 point1, float radius, out BaseCollider result, int layerMask = DefaultRaycastLayers, bool queryTrigger = true, SceneQueryFlags sceneQueryFlags = SceneQueryFlags.All)
        {
            var context = new SceneQueryContext(sceneQueryFlags, queryTrigger, layerMask);

            var numOverlaps = OverlapCapsuleNonAlloc(point0, point1, radius, context, 1u);

            result = numOverlaps > 0 ? BaseCollider.Get(s_colliderIdBuffer[0]) : null;
            return numOverlaps > 0;
        }
    }
}
