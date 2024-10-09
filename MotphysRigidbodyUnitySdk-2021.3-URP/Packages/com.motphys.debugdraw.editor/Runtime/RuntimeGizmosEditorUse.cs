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

using Motphys.DebugDraw.Core;
using Motphys.Rigidbody;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Motphys.DebugDraw.Editor
{
    public class RuntimeGizmosEditorUse
        : MonoBehaviour
    {
        public static RuntimeGizmosEditorUse Instance => s_instance;
        public bool drawContacts { get; set; }
        public bool drawAabb { get; set; }
        public bool drawJointPair { get; set; }
        public bool drawCollisionPair { get; set; }

        private NativeList<Line> _contactLines;
        private NativeList<float3> _contactPoints;
        private NativeList<Line> _lines;
        private NativeList<Aabb3> _aabbs;

        private static RuntimeGizmosEditorUse s_instance;

        private CommandBuffer _cmd;

        public Color jointPairColor;
        public Color collisionPairColor;
        public Color contactPointColor;
        public Color contactLineColor;
        public Color aabbColor;

        private void Awake()
        {
            if (s_instance != null)
            {
                Destroy(s_instance.gameObject);
            }

            s_instance = this;
            _lines = new NativeList<Line>(Allocator.Persistent);
            _contactPoints = new NativeList<float3>(Allocator.Persistent);
            _aabbs = new NativeList<Aabb3>(Allocator.Persistent);
            _contactLines = new NativeList<Line>(Allocator.Persistent);

            _cmd = new CommandBuffer();
        }

        public void DrawRuntimeGizmos()
        {
            _cmd.Clear();

            DebugDrawer.DrawAabbs(_cmd, _aabbs, aabbColor);
            DebugDrawer.DrawLines(_cmd, _lines);
            DebugDrawer.DrawPoints(_cmd, _contactPoints, this.contactPointColor, 0.15f);
            Graphics.ExecuteCommandBuffer(_cmd);
        }

        private void OnDestroy()
        {
            _lines.Dispose();
            _contactPoints.Dispose();
            _aabbs.Dispose();
            _contactLines.Dispose();

            s_instance = null;
        }

        private bool ClearData()
        {

            if (_lines.IsCreated)
            {
                _lines.Clear();
            }

            if (_contactPoints.IsCreated)
            {
                _contactPoints.Clear();
            }

            if (_aabbs.IsCreated)
            {
                _aabbs.Clear();
            }

            if (_contactLines.IsCreated)
            {
                _contactLines.Clear();
            }

            var flag = _lines.IsCreated && _contactPoints.IsCreated && _aabbs.IsCreated && _contactLines.IsCreated;
            return flag;
        }

        private void Start()
        {
            PhysicsManager.RequestVisualizeDataOnce(PackType(), GetVisualizationData);
        }

        private void GetVisualizationData(VisualizeData visualizeData)
        {
            if (!ClearData())
            {
                return;
            }

            var visualType = PackType();
            if (visualType.HasFlag(Rigidbody.VisualizeDataType.ActiveAabbs))
            {
                if (visualizeData.activeAabbs.IsCreated)
                {
                    var aabbs = visualizeData.activeAabbs;
                    _aabbs.AddRange(aabbs);
                }
            }

            if (visualType.HasFlag(Rigidbody.VisualizeDataType.ActiveJointPositionPairs))
            {
                if (visualizeData.activeJointPositionPairs.IsCreated)
                {
                    var jointPairs = visualizeData.activeJointPositionPairs;
                    for (var i = 0; i < jointPairs.Length; i++)
                    {
                        var line = ToLine(jointPairs[i]);
                        line.color = ToFloat3(this.jointPairColor);
                        _lines.Add(line);
                    }
                }
            }

            if (visualType.HasFlag(Rigidbody.VisualizeDataType.PotentialCollisionPositionPairs))
            {
                var pairs = visualizeData.potentialCollisionPositionPairs;
                if (pairs.IsCreated)
                {
                    for (var i = 0; i < pairs.Length; i++)
                    {
                        var line = ToLine(pairs[i]);
                        line.color = ToFloat3(this.collisionPairColor);
                        _lines.Add(line);
                    }
                }
            }

            if (drawContacts)
            {
                var contacts = PhysicsManager.Statistics.contacts;

                for (var i = 0; i < contacts.Length; i++)
                {
                    var line = new Line()
                    {
                        begin = contacts[i].position,
                        end = contacts[i].position + contacts[i].normal,
                        color = ToFloat3(this.contactLineColor)
                    };

                    _lines.Add(line);
                    _contactPoints.Add(contacts[i].position);
                }

                contacts.Dispose();
            }

            PhysicsManager.RequestVisualizeDataOnce(visualType, GetVisualizationData);
        }

        private static Line ToLine(PositionPair pair)
        {
            return new Line() { begin = pair.positionA, end = pair.positionB };
        }

        private static float3 ToFloat3(Color color)
        {
            return new float3(color.r, color.g, color.b);
        }

        private VisualizeDataType PackType()
        {
            var type = VisualizeDataType.None;
            if (drawAabb)
            {
                type |= VisualizeDataType.ActiveAabbs;
            }

            if (drawJointPair)
            {
                type |= VisualizeDataType.ActiveJointPositionPairs;
            }

            if (drawCollisionPair)
            {
                type |= VisualizeDataType.PotentialCollisionPositionPairs;
            }

            return type;
        }
    }
}

