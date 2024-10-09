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

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class RenderingManager : MonoBehaviour
{
    public bool postProcess;
    public bool renderReflectionRT;

    public Material skyBox;

    private Camera _mainCamera;
    private UniversalAdditionalCameraData _renderData;
    private ReflectionCamera _reflection;

    private void Start()
    {
        Initialize();
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void Update()
    {
        if (_renderData != null)
        {
            _renderData.renderPostProcessing = postProcess;
        }

        if (_reflection != null)
        {
            _reflection.renderReflectionRT = renderReflectionRT;
        }
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        Initialize();
    }

    private void Initialize()
    {
        _mainCamera = Camera.main;
        if (_mainCamera != null)
        {
            _renderData = _mainCamera.GetComponent<UniversalAdditionalCameraData>();
        }

        _reflection = GetComponent<ReflectionCamera>();

        if (skyBox != null)
        {
            RenderSettings.skybox = skyBox;
        }
    }
}
