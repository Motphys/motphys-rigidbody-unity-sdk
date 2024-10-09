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
using System.Runtime.InteropServices;
using Motphys.Native;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
namespace Motphys.Rigidbody
{
    /// <summary>
    /// Collision mask for collision filtering.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CollisionMask
    {
        /// <value>
        /// Each bit represents a group. If the bit is set, the object belongs to the group.
        /// </value>
        public uint group;

        /// <value>
        /// Each bit represents a group. If the bit is set, the object can collide with the group.
        /// </value>
        public uint collideMask;

        /// <value>
        /// Default collision mask. Belongs to group 1 and can collide with group 1.
        /// </value>
        public static readonly CollisionMask Default = new CollisionMask()
        {
            group = 1,
            collideMask = 1,
        };
    }

    /// <summary>
    /// The broad phase algorithms that supported by the engine.
    /// </summary>
    public enum BroadPhaseAlgorithm
    {
#if MOTPHYS_RIGIDBODY_EXPERIMENTAL
        /// <summary>
        /// Sweep and Prune algorithm will sort the objects along the axis and use a sweep and prune algorithm to accelerate the collision detection.
        /// </summary>
        SAP = 0,
        /// <summary>
        /// Incremental Sweep and Prune algorithm is similar to SAP but it will only sort the objects that are moved.
        /// </summary>
        ISAP = 1,
        /// <summary>
        /// Axis Aligned Bounding Box Tree algorithm will build a tree of AABBs to accelerate the collision detection.
        /// </summary>
        BVH4 = 2,
#endif
        /// <summary>
        /// Grid Sweep and Prune algorithm will split the world into a grid and use SAP for each cell.
        /// </summary>
        GridSAP = 3,
#if MOTPHYS_RIGIDBODY_EXPERIMENTAL
        /// <summary>
        /// Incremental Grid Sweep and Prune algorithm will split the world into a grid and use ISAP for each cell.
        /// </summary>
        IncrementalGridSAP = 4,
#endif
    }
    namespace Internal
    {
        [System.Serializable]
        [StructLayout(LayoutKind.Sequential)]
        internal struct Grid2DConfig
        {
            [MarshalAs(UnmanagedType.I1)]
            public bool autoWorldAabb;
            public Vector3 worldAabbMin;
            public Vector3 worldAabbMax;
            public uint numCellsInX;
            public uint numCellsInZ;

            public static readonly Grid2DConfig Default = new Grid2DConfig()
            {
                autoWorldAabb = false,
                worldAabbMin = new Vector3(-500, -500, -500),
                worldAabbMax = new Vector3(500, 500, 500),
                numCellsInX = 4,
                numCellsInZ = 4,
            };
        }

        [System.Serializable]
        [StructLayout(LayoutKind.Explicit)]
        internal struct BroadPhaseType
        {
            [FieldOffset(0)]
            [System.Obsolete("This enum value is experimental and may change in future releases.")]
            [HideInInspector]
            public BroadPhaseAlgorithm algorithm;

            [FieldOffset(4)]
            public Grid2DConfig gridConfig;

            public BroadPhaseType(BroadPhaseAlgorithm inAlgorithm)
            {
                algorithm = inAlgorithm;
                gridConfig = Grid2DConfig.Default;
            }

            public static readonly BroadPhaseType GridSAP = new BroadPhaseType(BroadPhaseAlgorithm.GridSAP);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Contact
        {
            public Vector3 normal;
            public Vector3 localOffsetA;
            public Vector3 localOffsetB;
            public Vector3 worldPositionA;
            public Vector3 worldPositionB;

            public Contact Flip()
            {
                return new Contact()
                {
                    normal = -normal,
                    localOffsetA = localOffsetB,
                    localOffsetB = localOffsetA,
                    worldPositionA = worldPositionB,
                    worldPositionB = worldPositionA,
                };
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct ColliderPair
        {
            internal ColliderId _id1;
            internal ColliderId _id2;

            internal ColliderPair(ColliderId id1, ColliderId id2)
            {
                _id1 = id1;
                _id2 = id2;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ContactManifold
        {
            public ColliderId colliderA;
            public ColliderId colliderB;
            public Contact contact0;
            public Contact contact1;
            public Contact contact2;
            public Contact contact3;
            public USize numPoints;

            public static int maxContactPoints => 4;

            public ContactManifold(USize num)
            {
                colliderA = ColliderId.Invalid;
                colliderB = ColliderId.Invalid;
                contact0 = default;
                contact1 = default;
                contact2 = default;
                contact3 = default;
                numPoints = num;
            }

            public bool isInvalid()
            {
                return colliderA == ColliderId.Invalid;
            }
            public ContactManifold Flip()
            {
                return new ContactManifold()
                {
                    colliderA = colliderB,
                    colliderB = colliderA,
                    contact0 = contact0.Flip(),
                    contact1 = contact1.Flip(),
                    contact2 = contact2.Flip(),
                    contact3 = contact3.Flip(),
                    numPoints = numPoints,
                };
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Contact GetContact(int index)
            {
                if (index < 0 || index >= numPoints.ToUInt32())
                {
                    throw new System.ArgumentOutOfRangeException(nameof(index));
                }

                unsafe
                {
                    return UnsafeUtility.ReadArrayElement<Contact>(UnsafeUtility.AddressOf(ref this.contact0), index);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ContactDebugInfo GetContactDebugInfo(int contactIndex)
            {
                var contact = GetContact(contactIndex);

                return new ContactDebugInfo()
                {
                    position = contact.worldPositionA,
                    normal = contact.normal,
                };
            }

            /// <value>
            /// The contact number
            /// </value>
            public int contactCount => (int)numPoints.ToUInt32();

        }
    }
}
