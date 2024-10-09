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
using Motphys.DebugDraw.Core;
using Motphys.Rigidbody;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Motphys.DebugDraw.Editor
{
    [ExcludeFromCoverage] // editor used settings
    internal partial class PhysicsDebugWindow : EditorWindow
    {
        [Overlay(typeof(SceneView), "PhysicsDebugWindow", "Physics Debug Window", false)]
        private class HandleOverlay : IMGUIOverlay, ITransientOverlay
        {
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
            internal static void TryToCreateRuntimeGizmos()
            {
                if (s_window?._drawCollisionInfo ?? false)
                {
                    var instance = new GameObject("RuntimeGizmosEditorUse");
                    instance.AddComponent<RuntimeGizmosEditorUse>();
                    DontDestroyOnLoad(instance);
                }
            }

            public bool visible => s_window != null;
            public override void OnGUI()
            {
                if (s_window != null)
                {
                    EditorGUI.BeginChangeCheck();

                    var geometry = s_window._drawColliderGeometry;
                    geometry = GUILayout.Toggle(geometry, "Collision Geometry");
                    var collisionInfo = s_window._drawCollisionInfo;
                    collisionInfo = GUILayout.Toggle(collisionInfo, "Collision Information");

                    if (EditorGUI.EndChangeCheck())
                    {
                        s_window._drawColliderGeometry = geometry;
                        s_window._drawCollisionInfo = collisionInfo;
                    }
                }
            }
        }

        public static PhysicsDebugWindow s_window;
        public static bool s_visiable;

        private bool _drawCollisionInfo
        {
            get
            {
                return EditorPrefs.GetBool("motphys.drawCollisionInfo", true);
            }
            set
            {
                if (Application.isPlaying && RuntimeGizmosEditorUse.Instance == null)
                {
                    var instance = new GameObject("RuntimeGizmosEditorUse");
                    instance.AddComponent<RuntimeGizmosEditorUse>();
                    DontDestroyOnLoad(instance);
                }

                EditorPrefs.SetBool("motphys.drawCollisionInfo", value);
            }
        }

        private bool _drawColliderGeometry
        {
            get
            {
                return EditorPrefs.GetBool("motphys.drawColliderGeometry", true);
            }
            set
            {
                EditorPrefs.SetBool("motphys.drawColliderGeometry", value);
            }
        }

        private readonly string[] _settingTypeTexts = new string[2] { "Geometry", "Collision" };

        private const string GizmosSettingsPath = "Packages/com.motphys.debugdraw.core/Settings/GizmosSettings.asset";
        private const float WireframeExtend = 0.005f;
        private const float MeshExtend = 0.002f;

        private GUILayoutOption _labelWidth = GUILayout.Width(220);

        private BaseCollider[] _colliders;
        private Rigidbody3D[] _rigidbody3Ds;

        private CommandBuffer _meshCommandBuffer;
        private CommandBuffer _wireFrameCommandBuffer;

        private Vector3 _sceneViewCamPos;

        private bool _selectObj;
        private bool _colliderTypesGroup;
        private bool _colorsGroup;
        private bool _renderingGroup;

        private int _showLayer;
        private string[] _layerNames;

        private bool _lastPlayingMode;
        private int _selectedPanel = 0;

        [MenuItem("Window/Analysis/Motphys Physics Debugger", false, 10)]
        private static void Open()
        {
            var window = EditorWindow.GetWindow<PhysicsDebugWindow>();
            window.minSize = new Vector2(500, 500);
            window.Show();
        }

        private void OnEnable()
        {
            s_window = this;

            SceneView.duringSceneGui += OnSceneViewDraw;
            EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;

            _lastPlayingMode = Application.isPlaying;
            Initialize();

        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneViewDraw;
            EditorSceneManager.activeSceneChangedInEditMode -= OnSceneChanged;

            if (!Application.isPlaying)
            {
                DebugDrawer.Dispose();
            }

            _meshCommandBuffer?.Dispose();
            _wireFrameCommandBuffer?.Dispose();
        }

        private void OnBecameVisible()
        {
            s_window = this;
            s_visiable = true;
            FindPhysicObjects();
        }

        private void OnBecameInvisible()
        {
            s_visiable = false;
            s_window = null;
        }

        private void OnSceneChanged(Scene a, Scene b)
        {
            DebugDrawer.CreateRenderResource();
        }

        private void OnSceneViewDraw(SceneView sceneView)
        {
            if (!s_visiable)
            {
                return;
            }

            var playingMode = Application.isPlaying;
            if (_lastPlayingMode != playingMode)
            {
                FindPhysicObjects();
                DebugDrawer.Dispose();
                var gizmosSettings = AssetDatabase.LoadAssetAtPath<GizmosSettings>(GizmosSettingsPath);
                DebugDrawer.Initialize(gizmosSettings);
                _lastPlayingMode = playingMode;
            }

            sceneView.Repaint();
            _sceneViewCamPos = sceneView.camera.transform.position;

            if (!sceneView.drawGizmos)
            {
                return;
            }

            if (Event.current.type == EventType.Repaint)
            {
                DrawGeometry();
                RuntimeDraw();
            }
        }

        private Vector2 _scrollderRect;

        private void OnGUI()
        {
            HeaderPanel();

            _selectedPanel = GUILayout.SelectionGrid(_selectedPanel, _settingTypeTexts, 2);

            using (var scrollder = new EditorGUILayout.ScrollViewScope(_scrollderRect))
            {
                EditorGUILayout.Space(2);

                switch (_selectedPanel)
                {
                    case 0:
                        EditorGizmosPanel();
                        break;
                    case 1:
                        RuntimePanel();
                        break;
                }

                _scrollderRect = scrollder.scrollPosition;
            }
        }

        private void DrawGeometry()
        {
            if (!_drawColliderGeometry)
            {
                return;
            }

            _meshCommandBuffer.Clear();
            _wireFrameCommandBuffer.Clear();

            DebugDrawer.SetupNormalExtendParameters(_meshCommandBuffer, MeshExtend);
            DebugDrawer.SetupNormalExtendParameters(_wireFrameCommandBuffer, WireframeExtend);

            ShowStaticColliders();
            ShowTriggers();
            ShowRigidbodies();
            ShowKinematicBodies();

            if (EditorDebugSettings.drawType == EditorDebugSettings.DrawType.Wireframe || EditorDebugSettings.drawType == EditorDebugSettings.DrawType.All)
            {
                GL.wireframe = true;
                Graphics.ExecuteCommandBuffer(_wireFrameCommandBuffer);
            }

            if (EditorDebugSettings.drawType == EditorDebugSettings.DrawType.Mesh || EditorDebugSettings.drawType == EditorDebugSettings.DrawType.All)
            {
                GL.wireframe = false;
                Graphics.ExecuteCommandBuffer(_meshCommandBuffer);
            }

            GL.wireframe = false;
        }

        private void Initialize()
        {
            // load draw settings
            var gizmosSettings = AssetDatabase.LoadAssetAtPath<GizmosSettings>(GizmosSettingsPath);
            DebugDrawer.Initialize(gizmosSettings);
            _meshCommandBuffer = new CommandBuffer();
            _wireFrameCommandBuffer = new CommandBuffer();

            // find objects
            FindPhysicObjects();

            // setup layer filter
            var layers = new List<string>();
            for (var i = 0; i < 32; i++)
            {
                var name = LayerMask.LayerToName(i);
                if (name != null && name != "")
                {
                    layers.Add(name);
                }
            }

            _layerNames = layers.ToArray();
            _showLayer = -1;
        }

        private void HeaderPanel()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Reset", EditorStyles.toolbarButton))
                {
                    _showLayer = -1;
                    EditorDebugSettings.Reset();
                }
            }
        }

        private void EditorGizmosPanel()
        {
            using (new EditorGUILayout.VerticalScope("frameBox"))
            {
                EditorDebugSettings.drawType = (EditorDebugSettings.DrawType)EditorGUILayout.EnumPopup("Draw Type", EditorDebugSettings.drawType);

                _selectObj = EditorGUILayout.BeginFoldoutHeaderGroup(_selectObj, "Selected Object Info");
                if (_selectObj)
                {
                    var selection = Selection.activeGameObject;
                    if (selection != null)
                    {
                        EditorGUI.indentLevel++;

                        GUI.enabled = false;
                        EditorGUILayout.TextField("GameObject", selection.name);
                        EditorGUILayout.TextField("Scene", selection.scene.name);
                        GUI.enabled = true;

                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUILayout.EndFoldoutHeaderGroup();

                _showLayer = EditorGUILayout.MaskField("Show Layers", _showLayer, _layerNames);

                EditorDebugSettings.showStaticColliders = EditorGUILayout.Toggle("Show Static Colliders", EditorDebugSettings.showStaticColliders);
                EditorDebugSettings.showTriggers = EditorGUILayout.Toggle("Show Triggers", EditorDebugSettings.showTriggers);
                EditorDebugSettings.showRigidbodies = EditorGUILayout.Toggle("Show Rigidbodies", EditorDebugSettings.showRigidbodies);
                EditorDebugSettings.showKinematicBodies = EditorGUILayout.Toggle("Show Kinematic Bodies", EditorDebugSettings.showKinematicBodies);
                EditorDebugSettings.showSleepingBodies = EditorGUILayout.Toggle("Show Sleeping Bodies", EditorDebugSettings.showSleepingBodies);

                _colliderTypesGroup = EditorGUILayout.BeginFoldoutHeaderGroup(_colliderTypesGroup, "ColliderTypes");
                if (_colliderTypesGroup)
                {
                    EditorGUI.indentLevel++;
                    EditorDebugSettings.showBoxColliders = DrawAlignToggle(EditorDebugSettings.showBoxColliders, "Show Box Colliders");
                    EditorDebugSettings.showSphereColliders = DrawAlignToggle(EditorDebugSettings.showSphereColliders, "Show Sphere Colliders");
                    EditorDebugSettings.showCapsuleColliders = DrawAlignToggle(EditorDebugSettings.showCapsuleColliders, "Show Capsule Colliders");
                    EditorDebugSettings.showCylinderColliders = DrawAlignToggle(EditorDebugSettings.showCylinderColliders, "Show Cylinder Colliders");
                    EditorDebugSettings.showMeshColliders_convex = DrawAlignToggle(EditorDebugSettings.showMeshColliders_convex, "Show Mesh Colliders");
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndFoldoutHeaderGroup();

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Show None"))
                    {
                        SetColliders(false);
                    }

                    if (GUILayout.Button("Show All"))
                    {
                        SetColliders(true);
                    }
                }

                _colorsGroup = EditorGUILayout.BeginFoldoutHeaderGroup(_colorsGroup, "Colors");
                if (_colorsGroup)
                {
                    EditorDebugSettings.triggerColor = EditorGUILayout.ColorField(EditorGUIUtility.TrTempContent("Trigger Color"), EditorDebugSettings.triggerColor, true, false, false);
                    EditorDebugSettings.staticColor = EditorGUILayout.ColorField(EditorGUIUtility.TrTempContent("Static Color"), EditorDebugSettings.staticColor, true, false, false);
                    EditorDebugSettings.rigidBodyColor = EditorGUILayout.ColorField(EditorGUIUtility.TrTempContent("RigidBody Color"), EditorDebugSettings.rigidBodyColor, true, false, false);
                    EditorDebugSettings.sleepColor = EditorGUILayout.ColorField(EditorGUIUtility.TrTempContent("Sleep Color"), EditorDebugSettings.sleepColor, true, false, false);
                    EditorDebugSettings.kinematicColor = EditorGUILayout.ColorField(EditorGUIUtility.TrTempContent("Kinematic Color"), EditorDebugSettings.kinematicColor, true, false, false);
                    EditorDebugSettings.variation = EditorGUILayout.Slider(EditorGUIUtility.TrTempContent("Variation"), EditorDebugSettings.variation, 0.0f, 1.0f);
                }

                EditorGUILayout.EndFoldoutHeaderGroup();

                _renderingGroup = EditorGUILayout.BeginFoldoutHeaderGroup(_renderingGroup, "Rendering");
                if (_renderingGroup)
                {
                    EditorDebugSettings.transparency = EditorGUILayout.Slider("Transparency", EditorDebugSettings.transparency, 0.0f, 1.0f);
                    EditorDebugSettings.viewDistance = EditorGUILayout.FloatField("View Distance", EditorDebugSettings.viewDistance);
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
        private void ShowStaticColliders()
        {
            if (!EditorDebugSettings.showStaticColliders || _colliders == null)
            {
                return;
            }

            var color = EditorDebugSettings.staticColor;
            foreach (var col in _colliders)
            {
                if (col == null || col.isTrigger)
                {
                    continue;
                }

                if (!Application.isPlaying)
                {
                    if (col.GetComponentInParent<Rigidbody3D>() != null)
                    {
                        continue;
                    }
                }
                else if (col.attachedRigidbody)
                {
                    continue;
                }

                var filter = ObjectFilter(col.gameObject);

                if (filter)
                {
                    DrawColliders(col, VaryColor(color, col.GetInstanceID()));
                }
            }
        }

        private void ShowTriggers()
        {
            if (!EditorDebugSettings.showTriggers || _colliders == null)
            {
                return;
            }

            var color = EditorDebugSettings.triggerColor;

            foreach (var col in _colliders)
            {
                if (col == null || !col.isTrigger)
                {
                    continue;
                }

                var filter = ObjectFilter(col.gameObject);
                if (filter)
                {
                    DrawColliders(col, VaryColor(color, col.GetInstanceID()));
                }
            }
        }

        private void ShowRigidbodies()
        {
            if (!EditorDebugSettings.showRigidbodies || _rigidbody3Ds == null)
            {
                return;
            }

            foreach (var rigid in _rigidbody3Ds)
            {
                if (rigid == null)
                {
                    continue;
                }

                var filter = ObjectFilter(rigid.gameObject);

                if (filter && !rigid.isKinematic)
                {
                    Color color;

                    if (EditorDebugSettings.showSleepingBodies && rigid.isSleeping)
                    {
                        color = EditorDebugSettings.sleepColor;
                    }
                    else
                    {
                        color = EditorDebugSettings.rigidBodyColor;
                    }

                    if (!Application.isPlaying)
                    {
                        var cols = rigid.GetComponentsInChildren<BaseCollider>();
                        foreach (var col in cols)
                        {
                            if (!col.isTrigger)
                            {
                                DrawColliders(col, VaryColor(color, rigid.GetInstanceID()));
                            }
                        }
                    }
                    else
                    {
                        rigid.ForeachAttachedCollider((collider) =>
                        {
                            if (!collider.isTrigger)
                            {
                                DrawColliders(collider, VaryColor(color, rigid.GetInstanceID()));
                            }
                        });
                    }
                }
            }
        }

        private void ShowKinematicBodies()
        {
            if (!EditorDebugSettings.showKinematicBodies || _rigidbody3Ds == null)
            {
                return;
            }

            foreach (var rigid in _rigidbody3Ds)
            {
                if (rigid == null)
                {
                    continue;
                }

                var filter = ObjectFilter(rigid.gameObject);

                if (filter && rigid.isKinematic)
                {
                    Color color = EditorDebugSettings.kinematicColor;

                    if (!Application.isPlaying)
                    {
                        var cols = rigid.GetComponentsInChildren<BaseCollider>();
                        foreach (var col in cols)
                        {
                            if (!col.isTrigger)
                            {
                                DrawColliders(col, VaryColor(color, rigid.GetInstanceID()));
                            }
                        }
                    }
                    else
                    {
                        rigid.ForeachAttachedCollider((collider) =>
                        {
                            if (!collider.isTrigger)
                            {
                                DrawColliders(collider, VaryColor(color, rigid.GetInstanceID()));
                            }
                        });
                    }
                }
            }
        }

        private bool ObjectFilter(GameObject obj)
        {
            var pos = obj.transform.position;
            var dis = Vector3.Distance(_sceneViewCamPos, pos);
            if (dis > EditorDebugSettings.viewDistance)
            {
                return false;
            }

            var layer = obj.layer;
            var name = LayerMask.LayerToName(layer);
            var index = -1;
            for (var i = 0; i < _layerNames.Length; i++)
            {
                if (_layerNames[i].Equals(name))
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                return false;
            }

            return (1 << index & _showLayer) != 0;
        }

        private void DrawColliders(BaseCollider collider, Color color)
        {
            if (!collider.enabled)
            {
                return;
            }

            if (EditorDebugSettings.showBoxColliders && collider is BoxCollider3D)
            {
                var boxCol = collider as BoxCollider3D;
                var transform = collider.transform;
                var absScale = new Vector3(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y), Mathf.Abs(transform.lossyScale.z));

                var localToWorld = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                var colliderTRS = Matrix4x4.TRS(Vector3.Scale(transform.lossyScale, collider.shapeTranslation),
                    collider.shapeRotation,
                    Vector3.Scale(boxCol.size, absScale));

                DebugDrawer.DrawCube(_meshCommandBuffer, localToWorld * colliderTRS, MeshColor(color));
                DebugDrawer.DrawCube(_wireFrameCommandBuffer, localToWorld * colliderTRS, WireframeColor(color));
            }
            else if (EditorDebugSettings.showSphereColliders && collider is SphereCollider3D)
            {
                var sphereCol = collider as SphereCollider3D;
                var transform = collider.transform;

                var maxSize = Mathf.Max(Mathf.Max(transform.lossyScale.x, transform.lossyScale.y), transform.lossyScale.z);
                var localToWorld = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                var pos = new Vector3(sphereCol.shapeTranslation.x * transform.lossyScale.x,
                    sphereCol.shapeTranslation.y * transform.lossyScale.y,
                    sphereCol.shapeTranslation.z * transform.lossyScale.z);

                var radius = sphereCol.radius;
                var colliderTRS = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one * radius * 2 * Mathf.Abs(maxSize));

                DebugDrawer.DrawSphere(_meshCommandBuffer, localToWorld * colliderTRS, MeshColor(color));
                DebugDrawer.DrawSphere(_wireFrameCommandBuffer, localToWorld * colliderTRS, WireframeColor(color));
            }
            else if (EditorDebugSettings.showCapsuleColliders && collider is CapsuleCollider3D)
            {
                var capsuleCol = collider as CapsuleCollider3D;
                var transform = collider.transform;
                var scalex = transform.lossyScale.x;
                var scaley = transform.lossyScale.y;
                var scalez = transform.lossyScale.z;

                var objMat = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

                var shapePos = new Vector3(capsuleCol.shapeTranslation.x * scalex,
                            capsuleCol.shapeTranslation.y * scaley,
                            capsuleCol.shapeTranslation.z * scalez);
                var shapeRot = capsuleCol.shapeRotation;
                var shapeMat = Matrix4x4.TRS(shapePos, shapeRot, Vector3.one);

                var radius = Mathf.Max(Mathf.Abs(scalex), Mathf.Abs(scalez)) * capsuleCol.radius;
                var height = Mathf.Abs(scaley) * capsuleCol.height;

                DebugDrawer.DrawCapsule(_meshCommandBuffer, objMat * shapeMat, radius, height, MeshColor(color));
                DebugDrawer.DrawCapsule(_wireFrameCommandBuffer, objMat * shapeMat, radius, height, WireframeColor(color));
            }
            else if (EditorDebugSettings.showCylinderColliders && collider is CylinderCollider3D)
            {
                var cylinderCol = collider as CylinderCollider3D;

                var transform = collider.transform;
                var scalex = transform.lossyScale.x;
                var scaley = transform.lossyScale.y;
                var scalez = transform.lossyScale.z;

                var objMat = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

                var shapePos = new Vector3(cylinderCol.shapeTranslation.x * scalex,
                            cylinderCol.shapeTranslation.y * scaley,
                            cylinderCol.shapeTranslation.z * scalez);
                var shapeRot = cylinderCol.shapeRotation;

                var radius = Mathf.Max(Mathf.Abs(scalex), Mathf.Abs(scalez)) * cylinderCol.radius;
                //standard cylinder radius : 0.5f
                radius /= 0.5f;
                var height = Mathf.Abs(scaley) * cylinderCol.height;
                //standard cylinder height : 2.0f
                height /= 2.0f;
                var scale = new Vector3(radius, height, radius);
                var shapeMat = Matrix4x4.TRS(shapePos, shapeRot, scale);

                DebugDrawer.DrawCylinder(_meshCommandBuffer, objMat * shapeMat, MeshColor(color));
                DebugDrawer.DrawCylinder(_wireFrameCommandBuffer, objMat * shapeMat, WireframeColor(color));
            }
            else if (EditorDebugSettings.showMeshColliders_convex && collider is MeshCollider3D)
            {
                var meshCol = collider as MeshCollider3D;
                if (!meshCol.convex)
                {
                    return;
                }

                var convexMesh = meshCol._displayMesh;
                if (convexMesh?.Mesh != null)
                {
                    var transform = collider.transform;

                    var localToWorld = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                    var colliderTRS = Matrix4x4.TRS(Vector3.Scale(transform.lossyScale, collider.shapeTranslation),
                        collider.shapeRotation,
                        Vector3.one);

                    DebugDrawer.DrawMesh(_meshCommandBuffer, convexMesh.Mesh, localToWorld * colliderTRS, MeshColor(color));
                    DebugDrawer.DrawMesh(_wireFrameCommandBuffer, convexMesh.Mesh, localToWorld * colliderTRS, WireframeColor(color));
                }
            }
        }

        private void SetColliders(bool flag)
        {
            EditorDebugSettings.showStaticColliders = flag;
            EditorDebugSettings.showTriggers = flag;
            EditorDebugSettings.showRigidbodies = flag;
            EditorDebugSettings.showKinematicBodies = flag;
            EditorDebugSettings.showSleepingBodies = flag;
            EditorDebugSettings.showCylinderColliders = flag;
            EditorDebugSettings.showBoxColliders = flag;
            EditorDebugSettings.showSphereColliders = flag;
            EditorDebugSettings.showCapsuleColliders = flag;
            EditorDebugSettings.showMeshColliders_convex = flag;
        }

        private bool DrawAlignToggle(bool value, string label)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, _labelWidth);
            value = EditorGUILayout.Toggle(value);
            EditorGUILayout.EndHorizontal();
            return value;
        }

        private void FindPhysicObjects()
        {
            _colliders = GameObject.FindObjectsOfType<BaseCollider>();
            _rigidbody3Ds = GameObject.FindObjectsOfType<Rigidbody3D>();
        }

        private void OnHierarchyChange()
        {
            FindPhysicObjects();
        }

        private Color VaryColor(Color inputColor, int hash)
        {
            var variation = EditorDebugSettings.variation;

            var hashR = (hash & 0xF) / 16.0f;
            var hashG = ((hash >> 4) & 0xF) / 16.0f;
            var hashB = ((hash >> 12) & 0xF) / 16.0f;

            var hashColor = new Color(hashR, hashG, hashB, inputColor.a);
            return Color.Lerp(inputColor, hashColor, variation);
        }

        private Color MeshColor(Color color)
        {
            var newColor = color;
            newColor.a = Mathf.Lerp(0, 0.75f, color.a);
            return newColor;
        }

        private Color WireframeColor(Color color)
        {
            var newColor = color;
            newColor.a = 1.0f;
            return newColor;
        }
    }
}
