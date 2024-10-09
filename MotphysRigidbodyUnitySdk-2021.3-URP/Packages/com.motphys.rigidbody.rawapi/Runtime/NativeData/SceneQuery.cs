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
using UnityEngine;

namespace Motphys.Rigidbody
{
    /// <summary>
    /// Represent a ray.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Ray
    {
        /// <value>
        /// The origin of a ray.
        /// </value>
        public Vector3 origin;

        /// <value>
        /// The direction of a ray.
        /// </value>
        public Vector3 direction;
    }

    /// <summary>
    /// Some flags used to enable some built-in functions, such as whether to discard static objects.
    /// </summary>
    [System.Flags]
    public enum SceneQueryFlags : uint
    {
        /// <summary>
        /// Discard all results.
        /// </summary>
        None = 0,
        /// <summary>
        /// Preserve static object result.
        /// </summary>
        Static = 1,
        /// <summary>
        /// Preserve movable object result.
        /// </summary>
        Movable = Static << 1,
        /// <summary>
        /// Discard inside hit.
        /// </summary>
        DiscardInsideHits = Static << 3,
        /// <summary>
        /// Preserve static and movable object result, and discard inside hit.
        /// </summary>
        All = Static | Movable | DiscardInsideHits,
    }

    namespace Internal
    {
        /// <summary>
        /// Some flags used to enable some built-in functions, such as whether to query trigger.
        /// </summary>
        [System.Flags]
        internal enum SceneQueryExtensionFlags : uint
        {
            None = 0,
            DiscardTrigger = 1,
        };

        /// <summary>
        /// Some settings used to enable some built-in functions, such as whether to discard static objects.
        /// </summary>
        /// <example>
        /// <code>
        /// new SceneQueryContext(SceneQueryFlags.Movable, false, -1);
        /// </code>
        /// </example>
        [StructLayout(LayoutKind.Sequential)]
        internal struct SceneQueryContext
        {
            public const int DefaultRaycastLayers = ~(1 << 2);
            /// <summary>
            /// Constructing a context to enable some built-in functions.
            /// </summary>
            /// <param name="flags">Some flags used to enable some built-in functions, such as whether to discard static objects.</param>
            /// <param name="queryTrigger">Whether to query triggers.</param>
            public SceneQueryContext(SceneQueryFlags flags, bool queryTrigger, int layerMask)
            {
                _flags = flags;
                _physicsFlags = queryTrigger ? SceneQueryExtensionFlags.None : SceneQueryExtensionFlags.DiscardTrigger;
                _layerMask = layerMask;
            }

            /// <value>
            /// Context with default settings.
            /// </value>
            public static readonly SceneQueryContext Default = new SceneQueryContext { _flags = SceneQueryFlags.Static | SceneQueryFlags.Movable | DISCARD_INSIDE_HITS, _physicsFlags = SceneQueryExtensionFlags.None, _layerMask = DefaultRaycastLayers };

            /// <value>
            /// No active flags.
            /// </value>
            public static readonly SceneQueryContext Empty = new SceneQueryContext { _flags = DISCARD_INSIDE_HITS, _physicsFlags = SceneQueryExtensionFlags.None, _layerMask = 0 };
            internal const SceneQueryFlags DISCARD_INSIDE_HITS = (SceneQueryFlags)((uint)SceneQueryFlags.Static << 3);

            private SceneQueryFlags _flags;
            private SceneQueryExtensionFlags _physicsFlags;
            private LayerMask _layerMask;

            /// <summary>
            /// Add some flags based on the current context.
            /// </summary>
            /// <param name="flags">Some flags used to enable some built-in functions, such as whether to discard static objects.</param>
            /// <returns>Modified context</returns>
            public SceneQueryContext WithFlags(SceneQueryFlags flags)
            {
                _flags |= flags;

                return this;
            }

            /// <summary>
            /// Exclude the given flags from current context.
            /// </summary>
            public SceneQueryContext WithoutFlags(SceneQueryFlags flags)
            {
                _flags &= ~flags;

                return this;
            }

            /// <summary>
            /// Modify the current context to decide whether to query the trigger.
            /// </summary>
            /// <param name="with">Whether to query triggers.</param>
            /// <returns>Modified context</returns>
            public SceneQueryContext WithQueryTrigger(bool with)
            {
                if (with)
                {
                    _physicsFlags &= ~SceneQueryExtensionFlags.DiscardTrigger;
                }
                else
                {
                    _physicsFlags |= SceneQueryExtensionFlags.DiscardTrigger;
                }

                return this;
            }

            /// <summary>
            /// Include the given layers in the query.
            /// </summary>
            public SceneQueryContext WithLayers(LayerMask mask)
            {
                _layerMask |= mask;
                return this;
            }

            /// <summary>
            /// Exclude the given layers from the query.
            /// </summary>
            public SceneQueryContext WithoutLayers(LayerMask mask)
            {
                _layerMask &= ~mask;
                return this;
            }

            /// <summary>
            /// Replace given layer mask to the mask of the current context.
            /// </summary>
            /// <param name="mask">A layer mask that is used to selectively ignore colliders.</param>
            /// <returns>Modified context</returns>
            public SceneQueryContext ReplaceLayerMask(LayerMask mask)
            {
                _layerMask = mask;
                return this;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RaycastHit
        {
            public float distance;
            public Vector3 normal;
            public ColliderId id;
        }

        /// <summary>
        /// map the OptionalRaycastHit to `COptional<RaycastHit>` in rust ffi.
        /// </summary>
        internal struct OptionalRaycastHit
        {
            private bool _isMissing;
            private RaycastHit _hit;

            /// <value>
            ///  Whether the ray hits something.
            /// </value>
            public bool isHit => !_isMissing;

            /// implicit conversion from Optional<T> to Nullable<T>
            public static implicit operator RaycastHit?(OptionalRaycastHit optional)
            {
                return optional.isHit ? optional._hit : (RaycastHit?)null;
            }
        }
    }
}
