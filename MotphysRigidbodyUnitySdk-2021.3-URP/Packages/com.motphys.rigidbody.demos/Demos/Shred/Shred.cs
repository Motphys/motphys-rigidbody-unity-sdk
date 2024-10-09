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

using System.Collections.Generic;
using UnityEngine;

namespace Motphys.Rigidbody.Demos
{
    public class Shred : MonoBehaviour
    {
        [Range(7, 100)]
        public int generateCount = 20;
        [Range(2, 20)]
        public int unitSphereCount = 3;
        [Range(0, 10000)]
        public float breakTorque = 500;
        [Range(0.01f, 1.0f)]
        public float radius = 0.25f;
        [Range(1, 100)]
        public float maxLinearSpeed = 8;
        public Vector3 spawnRoot = new Vector3(0, 3, 0);
        [SerializeField]
        public Material[] materials;

        private void Start()
        {
            PhysicsManager.numSubstep = 4;
            PhysicsManager.defaultSolverIterations = 2;
            PhysicsManager.defaultSolverVelocityIterations = 2;
            Initialize();
        }

        protected void Initialize()
        {
            if (materials.Length < 7)
            {
                return;
            }

            var yOffset = unitSphereCount * radius * 15;
            var startIndex = Random.Range(0, 7);
            for (var i = 0; i < generateCount; i++)
            {
                var pos = spawnRoot + new Vector3(Random.Range(-1.5f, 1.5f), i * yOffset, Random.Range(-0.7f, 0.7f));
                switch (startIndex)
                {
                    case 0:
                        CreateSquare(pos, radius, materials[0]);
                        break;
                    case 1:
                        CreateStick(pos, radius, materials[1]);
                        break;
                    case 2:
                        CreateRightGun(pos, radius, materials[2]);
                        break;
                    case 3:
                        CreateLeftGun(pos, radius, materials[3]);
                        break;
                    case 4:
                        CreateRightSnake(pos, radius, materials[4]);
                        break;
                    case 5:
                        CreateLeftSnake(pos, radius, materials[5]);
                        break;
                    case 6:
                        CreateT(pos, radius, materials[6]);
                        break;
                }

                startIndex = (startIndex + 1) % 7;
            }
        }

        /// <summary>
        /// Standard size : 3x3x3
        /// </summary>
        private void CreateSquare(Vector3 spawnPos, float radius, Material material)
        {
            CreateBasicShape(spawnPos, unitSphereCount, unitSphereCount, unitSphereCount, radius, null, material);
        }

        /// <summary>
        /// Stick shape : 12x3x3
        /// </summary>
        private void CreateStick(Vector3 spawnPos, float radius, Material material)
        {
            CreateBasicShape(spawnPos, unitSphereCount * 3, unitSphereCount, unitSphereCount, radius, null, material);
        }

        /// <summary>
        /// LeftGun 9x3x3 and right bottom 3x3x3
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <param name="radius"></param>
        private void CreateLeftGun(Vector3 spawnPos, float radius, Material material)
        {
            var groupA = new List<Rigidbody3D>();
            var groupB = new List<Rigidbody3D>();

            CreateBasicShape(spawnPos, 3 * unitSphereCount, unitSphereCount, unitSphereCount, radius, groupA, material);
            var newPos = spawnPos + new Vector3(2 * unitSphereCount * radius * 2, -unitSphereCount * radius * 2, 0);
            CreateBasicShape(newPos, unitSphereCount, unitSphereCount, unitSphereCount, radius, groupB, material);

            //if unit count = 3
            //index : 6,7,8; 15,16,17;24,25,26 of groupA connect to index : 18~26 of groupB;
            var baseIndex = 2 * unitSphereCount;
            var linkIndex = unitSphereCount * unitSphereCount * (unitSphereCount - 1);
            var lineOffset = 3 * unitSphereCount;
            for (var i = 0; i < unitSphereCount; i++)
            {
                for (var j = 0; j < unitSphereCount; j++)
                {
                    var oneBall = groupA[baseIndex + j + i * lineOffset];
                    var theOtherBall = groupB[linkIndex + j + i * unitSphereCount];
                    CreateFixedJoint(oneBall.gameObject, theOtherBall);
                }
            }
        }

        /// <summary>
        /// RightGun 9x3x3 and left bottom 3x3x3
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <param name="radius"></param>
        private void CreateRightGun(Vector3 spawnPos, float radius, Material material)
        {
            var groupA = new List<Rigidbody3D>();
            var groupB = new List<Rigidbody3D>();

            CreateBasicShape(spawnPos, 3 * unitSphereCount, unitSphereCount, unitSphereCount, radius, groupA, material);
            var newPos = spawnPos + new Vector3(0, -unitSphereCount * radius * 2, 0);
            CreateBasicShape(newPos, unitSphereCount, unitSphereCount, unitSphereCount, radius, groupB, material);

            //if unit count = 3
            //index : 0,1,2; 9,10,11;18,19,20 of groupA connect to index : 18~26 of groupB;
            var baseIndex = 0;
            var linkIndex = unitSphereCount * unitSphereCount * (unitSphereCount - 1);
            var lineOffset = 3 * unitSphereCount;
            for (var i = 0; i < unitSphereCount; i++)
            {
                for (var j = 0; j < unitSphereCount; j++)
                {
                    var oneBall = groupA[baseIndex + j + i * lineOffset];
                    var theOtherBall = groupB[linkIndex + j + i * unitSphereCount];
                    CreateFixedJoint(oneBall.gameObject, theOtherBall);
                }
            }
        }

        /// <summary>
        /// T 9x3x3 and middle bottom 3x3x3
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <param name="radius"></param>
        private void CreateT(Vector3 spawnPos, float radius, Material material)
        {
            var groupA = new List<Rigidbody3D>();
            var groupB = new List<Rigidbody3D>();

            CreateBasicShape(spawnPos, 3 * unitSphereCount, unitSphereCount, unitSphereCount, radius, groupA, material);
            var newPos = spawnPos + new Vector3(unitSphereCount * radius * 2, -unitSphereCount * radius * 2, 0);
            CreateBasicShape(newPos, unitSphereCount, unitSphereCount, unitSphereCount, radius, groupB, material);

            //if unit count = 3
            //index : 3,4,5; 12,13,14;21,22,23 of groupA connect to index : 18~26 of groupB;
            var baseIndex = unitSphereCount;
            var linkIndex = unitSphereCount * unitSphereCount * (unitSphereCount - 1);
            var lineOffset = 3 * unitSphereCount;
            for (var i = 0; i < unitSphereCount; i++)
            {
                for (var j = 0; j < unitSphereCount; j++)
                {
                    var oneBall = groupA[baseIndex + j + i * lineOffset];
                    var theOtherBall = groupB[linkIndex + j + i * unitSphereCount];
                    CreateFixedJoint(oneBall.gameObject, theOtherBall);
                }
            }
        }

        /// <summary>
        /// RightSnake 6x3x3 and another 6x3x3
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <param name="radius"></param>
        private void CreateRightSnake(Vector3 spawnPos, float radius, Material material)
        {
            var groupA = new List<Rigidbody3D>();
            var groupB = new List<Rigidbody3D>();

            CreateBasicShape(spawnPos, 2 * unitSphereCount, unitSphereCount, unitSphereCount, radius, groupA, material);
            var newPos = spawnPos + new Vector3(unitSphereCount * radius * 2, unitSphereCount * radius * 2, 0);
            CreateBasicShape(newPos, 2 * unitSphereCount, unitSphereCount, unitSphereCount, radius, groupB, material);

            //if unit count = 3
            //index : 39,41,42; 45,46,47;51,52,53 of groupA connect to index : 0,1,2; 6,7,8,12,13,14 of groupB;
            var baseIndex = 2 * unitSphereCount * unitSphereCount * (unitSphereCount - 1) + unitSphereCount;
            var linkIndex = 0;
            var lineOffset = 2 * unitSphereCount;
            for (var i = 0; i < unitSphereCount; i++)
            {
                for (var j = 0; j < unitSphereCount; j++)
                {
                    var oneBall = groupA[baseIndex + j + i * lineOffset];
                    var theOtherBall = groupB[linkIndex + j + i * unitSphereCount];
                    CreateFixedJoint(oneBall.gameObject, theOtherBall);
                }
            }
        }

        /// <summary>
        /// LeftSnake 6x3x3 and another 6x3x3
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <param name="radius"></param>
        private void CreateLeftSnake(Vector3 spawnPos, float radius, Material material)
        {
            var groupA = new List<Rigidbody3D>();
            var groupB = new List<Rigidbody3D>();

            CreateBasicShape(spawnPos, 2 * unitSphereCount, unitSphereCount, unitSphereCount, radius, groupA, material);
            var newPos = spawnPos + new Vector3(unitSphereCount * radius * 2, -unitSphereCount * radius * 2, 0);
            CreateBasicShape(newPos, 2 * unitSphereCount, unitSphereCount, unitSphereCount, radius, groupB, material);

            //if unit count = 3
            //index : 3,4,5; 9,10,10;15,16,17 of groupA connect to index : 36,37,38; 42,43,44;48,49,50 of groupB;
            var baseIndex = 2 * unitSphereCount * unitSphereCount * (unitSphereCount - 1) + unitSphereCount;
            var linkIndex = 2 * unitSphereCount * unitSphereCount * (unitSphereCount - 1);
            var lineOffset = 2 * unitSphereCount;
            for (var i = 0; i < unitSphereCount; i++)
            {
                for (var j = 0; j < unitSphereCount; j++)
                {
                    var oneBall = groupA[baseIndex + j + i * lineOffset];
                    var theOtherBall = groupB[linkIndex + j + i * unitSphereCount];
                    CreateFixedJoint(oneBall.gameObject, theOtherBall);
                }
            }
        }

        private void CreateBasicShape(Vector3 spawnPos, int length, int width, int height, float radius, List<Rigidbody3D> rigids, Material material)
        {
            var lastPlane = new Rigidbody3D[length * width];
            var lastLines = new Rigidbody3D[length];

            for (var i = 0; i < height; i++)
            {
                CreatPlane(spawnPos, length, width, radius, lastLines, lastPlane, i, rigids, material);
            }
        }

        private void CreatPlane(Vector3 spawnPos, int length, int width, float radius,
         Rigidbody3D[] lastLines, Rigidbody3D[] lastPlane, float depth, List<Rigidbody3D> rigids, Material material)
        {
            //radius?
            Rigidbody3D lastSphere = default;

            for (var j = 0; j < width; j++)
            {
                for (var i = 0; i < length; i++)
                {
                    var currentPos = spawnPos + new Vector3(i * radius * 2, depth * radius * 2, j * radius * 2);
                    var currentInstance = PhysicsUtils.CreatePrimitive(PrimitiveType.Sphere, currentPos);
                    var index = i + length * j + depth * length * width;

                    currentInstance.GetComponent<BaseCollider>().supportDynamicScale = true;
                    currentInstance.transform.localScale = Vector3.one * (radius / 0.5f);
                    currentInstance.name = $"Sphere_{index}";
                    currentInstance.AddComponent<Rigidbody3D>();
                    currentInstance.GetComponent<MeshRenderer>().sharedMaterial = material;

                    var rigidbody = currentInstance.GetComponent<Rigidbody3D>();
                    rigidbody.maxLinearVelocity = maxLinearSpeed;

                    //link last sphere
                    if (i > 0)
                    {
                        CreateFixedJoint(currentInstance, lastSphere);
                    }

                    //link last line sphere
                    var lastLineSphere = lastLines[i];
                    if (j > 0)
                    {
                        CreateFixedJoint(currentInstance, lastLineSphere);
                    }

                    //link last plane sphere
                    var lastPlaneSphere = lastPlane[i + length * j];
                    if (depth > 0)
                    {
                        CreateFixedJoint(currentInstance, lastPlaneSphere);
                    }

                    lastSphere = rigidbody;
                    lastLines[i] = rigidbody;
                    lastPlane[i + length * j] = rigidbody;

                    if (rigids != null)
                    {
                        rigids.Add(rigidbody);
                    }
                }
            }
        }

        private FixedJoint3D CreateFixedJoint(GameObject bodyA, Rigidbody3D bodyB)
        {
            var joint = bodyA.AddComponent<FixedJoint3D>();
            joint.connectedBody = bodyB;
            joint.breakForce = breakTorque;
            joint.breakTorque = breakTorque;
            joint.ignoreCollision = true;
            return joint;
        }
    }
}
