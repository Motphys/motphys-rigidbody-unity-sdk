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

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using Unity.Profiling.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Motphys.Rigidbody.RawApi.Editor
{
    [UnityEngine.TestTools.ExcludeFromCoverage] // Editor GUI , not testable
    [ProfilerModuleMetadata("Motphys-Physics")]
    internal class PhysicsProfilerModule : ProfilerModule
    {

        internal struct MetricsDescriptor
        {
            public bool isMain;
            public string name;
            public ProfilerCategory category;

            public MetricsDescriptor(string name)
            {
                this.name = name;
                isMain = true;
                category = ProfilerCategory.Physics;
            }
        }
        internal static readonly MetricsDescriptor[] s_metricsDescriptors = new MetricsDescriptor[]{
            new MetricsDescriptor("Motphys.ActiveDynamicCount"),
            new MetricsDescriptor("Motphys.SleepingBodyCount"),
            new MetricsDescriptor("Motphys.BroadPhase.PairCount"),
            new MetricsDescriptor("Motphys.AwakeIslandCount"),
            new MetricsDescriptor("Motphys.ManifoldCount"),
            new MetricsDescriptor("Motphys.ContactCount"),
            new MetricsDescriptor("Motphys.CollisionEnterPairs"),
            new MetricsDescriptor("Motphys.CollisionExitPairs"),
        };

        internal static ProfilerCounterDescriptor[] GetProfilerCounterDescriptors()
        {
            return s_metricsDescriptors.Where(x => x.isMain).Select(x => new ProfilerCounterDescriptor(x.name, x.category)).ToArray();
        }

        private static readonly string[] s_autoEnabledCategoryNames = new string[]
        {
            ProfilerCategory.Physics.Name
        };

        public PhysicsProfilerModule() : base(GetProfilerCounterDescriptors(), autoEnabledCategoryNames: s_autoEnabledCategoryNames)
        {

        }

        public override ProfilerModuleViewController CreateDetailsViewController()
        {
            return new PhysicsDetailsViewController(ProfilerWindow, this);
        }
    }

    [UnityEngine.TestTools.ExcludeFromCoverage] // Editor GUI , not testable
    internal class PhysicsDetailsViewController : ProfilerModuleViewController
    {
        struct Counter
        {
            public string name;
            public long value;
        }

        private PhysicsProfilerModule _module;

        private List<Counter> _counters = new List<Counter>();

        public PhysicsDetailsViewController(ProfilerWindow profilerWindow, PhysicsProfilerModule module) : base(profilerWindow)
        {
            _module = module;
        }

        protected override VisualElement CreateView()
        {
            ReloadData(ProfilerWindow.selectedFrameIndex);
            ProfilerWindow.SelectedFrameIndexChanged += OnSelectedFrameIndexChanged;
            return new IMGUIContainer(OnGUI);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            ProfilerWindow.SelectedFrameIndexChanged -= OnSelectedFrameIndexChanged;
            base.Dispose(disposing);
        }

        void OnSelectedFrameIndexChanged(long selectedFrameIndex)
        {
            ReloadData(selectedFrameIndex);
        }

        void OnGUI()
        {
            var labelWidthBackup = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 200;
            foreach (var c in _counters)
            {
                EditorGUILayout.LabelField(c.name, c.value.ToString());
            }

            EditorGUIUtility.labelWidth = labelWidthBackup;
        }

        void ReloadData(long selectedFrameIndex)
        {
            _counters.Clear();
            var selectedFrameIndexInt32 = Convert.ToInt32(selectedFrameIndex);
            using (var frameData = UnityEditorInternal.ProfilerDriver.GetRawFrameDataView(selectedFrameIndexInt32, 0))
            {
                if (frameData == null || !frameData.valid)
                {
                    return;
                }

                foreach (var c in PhysicsProfilerModule.s_metricsDescriptors)
                {
                    var markerId = frameData.GetMarkerId(c.name);
                    var counterValue = frameData.GetCounterValueAsLong(markerId);
                    _counters.Add(new Counter() { name = c.name, value = counterValue });
                }
            }
        }
    }
}
