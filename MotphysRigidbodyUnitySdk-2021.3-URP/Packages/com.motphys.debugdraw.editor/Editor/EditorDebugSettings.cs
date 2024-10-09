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

using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Motphys.DebugDraw.Editor
{
    [ExcludeFromCoverage] // editor used settings
    internal class EditorDebugSettings
    {
        private static readonly Color s_triggerColor = new Color(0.9882353f, 1.0f, 0.05882353f);
        private static readonly Color s_staticColor = new Color(0.1843137f, 0.8313726f, 0.3411765f);
        private static readonly Color s_rigidBodyColor = new Color(0.8901961f, 0.07450981f, 0.1019608f);
        private static readonly Color s_sleepColor = new Color(0.6313726f, 0.0823529f, 0.6156863f);
        private static readonly Color s_kinematicColor = new Color(0.08235294f, 0.6705883f, 0.9254902f);
        private static readonly Color s_aabbColor = new Color(0.9f, 0.9f, 0.9f);
        private static readonly Color s_collisionPairColor = new Color(0.75f, 0.55f, 0.32f);
        private static readonly Color s_contactLineColor = new Color(0.12f, 0.75f, 0.65f);
        private static readonly Color s_contactPointColor = new Color(0.7843137f, 0.2549019f, 0.614581f);
        private static readonly Color s_jointPairColor = new Color(0.9433962f, 0.8963096f, 0.4218583f);

        internal enum DrawType
        {
            All,
            Wireframe,
            Mesh
        }

        internal static DrawType drawType
        {
            get
            {
                return (DrawType)EditorPrefs.GetInt("motphys.drawType", 1);
            }
            set
            {
                EditorPrefs.SetInt("motphys.drawType", (int)value);
            }
        }

        internal static bool showStaticColliders
        {
            get
            {
                return EditorPrefs.GetBool("motphys.showStaticColliders", true);
            }
            set
            {
                EditorPrefs.SetBool("motphys.showStaticColliders", value);
            }
        }
        internal static bool showTriggers
        {
            get
            {
                return EditorPrefs.GetBool("motphys.showTriggers", true);
            }
            set
            {
                EditorPrefs.SetBool("motphys.showTriggers", value);
            }
        }
        internal static bool showRigidbodies
        {
            get
            {
                return EditorPrefs.GetBool("motphys.showRigidbodies", true);
            }
            set
            {
                EditorPrefs.SetBool("motphys.showRigidbodies", value);
            }
        }
        internal static bool showKinematicBodies
        {
            get
            {
                return EditorPrefs.GetBool("motphys.showKinematicBodies", true);
            }
            set
            {
                EditorPrefs.SetBool("motphys.showKinematicBodies", value);
            }
        }
        internal static bool showSleepingBodies
        {
            get
            {
                return EditorPrefs.GetBool("motphys.showSleepingBodies", true);
            }
            set
            {
                EditorPrefs.SetBool("motphys.showSleepingBodies", value);
            }
        }

        internal static bool showBoxColliders
        {
            get
            {
                return EditorPrefs.GetBool("motphys.showBoxColliders", true);
            }
            set
            {
                EditorPrefs.SetBool("motphys.showBoxColliders", value);
            }
        }
        internal static bool showSphereColliders
        {
            get
            {
                return EditorPrefs.GetBool("motphys.showSphereColliders", true);
            }
            set
            {
                EditorPrefs.SetBool("motphys.showSphereColliders", value);
            }
        }
        internal static bool showCapsuleColliders
        {
            get
            {
                return EditorPrefs.GetBool("motphys.showCapsuleColliders", true);
            }
            set
            {
                EditorPrefs.SetBool("motphys.showCapsuleColliders", value);
            }
        }
        internal static bool showCylinderColliders
        {
            get
            {
                return EditorPrefs.GetBool("motphys.showCylinderColliders", true);
            }
            set
            {
                EditorPrefs.SetBool("motphys.showCylinderColliders", value);
            }
        }
        internal static bool showMeshColliders_convex
        {
            get
            {
                return EditorPrefs.GetBool("motphys.showMeshColliders_convex", true);
            }
            set
            {
                EditorPrefs.SetBool("motphys.showMeshColliders_convex", value);
            }
        }

        internal static bool showCollisionPair
        {
            get
            {
                return EditorPrefs.GetBool("motphys.showCollisionPair", true);
            }
            set
            {
                EditorPrefs.SetBool("motphys.showCollisionPair", value);
            }
        }

        internal static bool showJointPair
        {
            get
            {
                return EditorPrefs.GetBool("motphys.showJointPair", true);
            }
            set
            {
                EditorPrefs.SetBool("motphys.showJointPair", value);
            }
        }

        internal static bool showAabb
        {
            get
            {
                return EditorPrefs.GetBool("motphys.showAabb", true);
            }
            set
            {
                EditorPrefs.SetBool("motphys.showAabb", value);
            }
        }

        internal static bool showContacts
        {
            get
            {
                return EditorPrefs.GetBool("motphys.showContacts", true);
            }
            set
            {
                EditorPrefs.SetBool("motphys.showContacts", value);
            }
        }

        internal static Color triggerColor
        {
            get
            {
                return GetColor("motphys.triggerColor", s_triggerColor, transparency);
            }
            set
            {
                SetColor("motphys.triggerColor", value);
            }
        }

        internal static Color staticColor
        {
            get
            {
                return GetColor("motphys.staticColor", s_staticColor, transparency);
            }
            set
            {
                SetColor("motphys.staticColor", value);

            }
        }

        internal static Color rigidBodyColor
        {
            get
            {
                return GetColor("motphys.rigidBodyColor", s_rigidBodyColor, transparency);
            }
            set
            {
                SetColor("motphys.rigidBodyColor", value);
            }
        }

        internal static Color sleepColor
        {
            get
            {
                return GetColor("motphys.sleepColor", s_sleepColor, transparency);
            }
            set
            {
                SetColor("motphys.sleepColor", value);

            }
        }
        internal static Color kinematicColor
        {
            get
            {
                return GetColor("motphys.kinematicColor", s_kinematicColor, transparency);
            }
            set
            {
                SetColor("motphys.kinematicColor", value);

            }
        }

        internal static Color aabbColor
        {
            get
            {
                return GetColor("motphys.aabbColor", s_aabbColor);
            }
            set
            {
                SetColor("motphys.aabbColor", value);

            }
        }

        internal static Color collisionPairColor
        {
            get
            {
                return GetColor("motphys.collisionPairColor", s_collisionPairColor);
            }
            set
            {
                SetColor("motphys.collisionPairColor", value);
            }
        }

        internal static Color contactLineColor
        {
            get
            {
                return GetColor("motphys.contactLineColor", s_contactLineColor);
            }
            set
            {
                SetColor("motphys.contactLineColor", value);
            }
        }

        internal static Color contactPointColor
        {
            get
            {
                return GetColor("motphys.contactPointColor", s_contactPointColor);
            }
            set
            {
                SetColor("motphys.contactPointColor", value);
            }
        }

        internal static Color jointPairColor
        {
            get
            {
                return GetColor("motphys.jointPairColor", s_jointPairColor);
            }
            set
            {
                SetColor("motphys.jointPairColor", value);
            }
        }

        internal static float transparency
        {
            get
            {
                return EditorPrefs.GetFloat("motphys.transparency", 0.56f);
            }
            set
            {
                EditorPrefs.SetFloat("motphys.transparency", value);

            }
        }

        internal static float viewDistance
        {
            get
            {
                return EditorPrefs.GetFloat("motphys.viewDistance", 1000);
            }
            set
            {
                EditorPrefs.SetFloat("motphys.viewDistance", value);
            }
        }

        internal static float variation
        {
            get
            {
                return EditorPrefs.GetFloat("motphys.variation", 0.0f);
            }
            set
            {
                EditorPrefs.SetFloat("motphys.variation", value);
            }
        }

        private static Color GetColor(string name, Color defaultColor, float alpha = 1.0f)
        {
            var r = EditorPrefs.GetFloat($"{name}.r", defaultColor.r);
            var g = EditorPrefs.GetFloat($"{name}.g", defaultColor.g);
            var b = EditorPrefs.GetFloat($"{name}.b", defaultColor.b);
            return new Color(r, g, b, alpha);
        }

        private static void SetColor(string name, Color color)
        {
            EditorPrefs.SetFloat($"{name}.r", color.r);
            EditorPrefs.SetFloat($"{name}.g", color.g);
            EditorPrefs.SetFloat($"{name}.b", color.b);
        }

        public static void Reset()
        {
            drawType = 0;
            showStaticColliders = true;
            showTriggers = true;
            showRigidbodies = true;
            showKinematicBodies = true;
            showSleepingBodies = true;
            showBoxColliders = true;
            showSphereColliders = true;
            showCapsuleColliders = true;
            showCylinderColliders = true;
            showMeshColliders_convex = true;

            showCollisionPair = true;
            showJointPair = true;
            showAabb = true;
            showContacts = true;

            triggerColor = s_triggerColor;
            staticColor = s_staticColor;
            rigidBodyColor = s_rigidBodyColor;
            sleepColor = s_sleepColor;
            kinematicColor = s_kinematicColor;
            aabbColor = s_aabbColor;
            contactLineColor = s_contactLineColor;
            contactPointColor = s_contactPointColor;
            jointPairColor = s_jointPairColor;
            collisionPairColor = s_collisionPairColor;

            transparency = 0.56f;
            viewDistance = 1000;
            variation = 0;
        }
    }
}
