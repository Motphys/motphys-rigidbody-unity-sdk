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
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ReflectionCamera : MonoBehaviour
{
    public RenderTexture rt;
    [HideInInspector]
    public bool renderReflectionRT = true;

    private Vector3 _normal = Vector3.up;
    private Vector3 _pointOnPlane = Vector3.zero;

    private bool _invertCulling;
    private Camera _mainCam;
    private Camera _refCam;

    private static ReflectionCamera s_instance;

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }

        RenderPipelineManager.beginCameraRendering += RenderEvent;
    }

    private void Update()
    {
        if (_mainCam == null)
        {
            _mainCam = Camera.main;
        }
    }

    private void OnDestroy()
    {
        if (_refCam != null)
        {
            Destroy(_refCam.gameObject);
        }

        RenderPipelineManager.beginCameraRendering -= RenderEvent;
    }

    private void RenderEvent(ScriptableRenderContext context, Camera camera)
    {
        if (!renderReflectionRT || _mainCam == null)
        {
            return;
        }

        if (_refCam == null)
        {
            _refCam = CreateReflectCamera();
        }

        SetCullingOrder();
        UpdateReflectionCamera(_mainCam);
        _refCam.targetTexture = rt;

        var pos = _mainCam.transform.position;
        pos.y *= -1;
        _refCam.transform.position = pos;

        //Prevent errors when the camera view direction is parallel to the reflective plane
        if (!(pos.y == 0 && _refCam.transform.rotation == Quaternion.identity))
        {
            UniversalRenderPipeline.RenderSingleCamera(context, _refCam);
        }

        ResetCullingOrder();
    }

    private void UpdateReflectionCamera(Camera curCamera)
    {
        Vector3 planeNormal = _normal;
        Vector3 planePos = _pointOnPlane;

        UpdateCamera(curCamera, _refCam);

        var planVS = new Vector4(planeNormal.x, planeNormal.y, planeNormal.z, -Vector3.Dot(planeNormal, planePos));
        Matrix4x4 reflectionMat = CalculateReflectionMatrix(planVS);
        _refCam.worldToCameraMatrix = curCamera.worldToCameraMatrix * reflectionMat;

        var clipPlane = CameraSpacePlane(_refCam, planePos, planeNormal, 1.0f);
        if (Mathf.Abs(clipPlane.w) >= curCamera.farClipPlane)
        {
            return;
        }

        var newProjectionMat = CalculateObliqueMatrix(curCamera, clipPlane);
        _refCam.projectionMatrix = newProjectionMat;
    }

    private void UpdateCamera(Camera src, Camera dest)
    {
        if (dest == null)
        {
            return;
        }

        dest.aspect = src.aspect;
        dest.cameraType = src.cameraType;
        dest.clearFlags = src.clearFlags;
        dest.fieldOfView = src.fieldOfView;
        dest.depth = src.depth;
        dest.farClipPlane = src.farClipPlane;
        dest.focalLength = src.focalLength;
        dest.useOcclusionCulling = false;
        dest.cullingMask = src.cullingMask;
    }

    private Matrix4x4 CalculateObliqueMatrix(Camera cam, Vector4 plane)
    {
        var qClip = new Vector4(Mathf.Sign(plane.x), Mathf.Sign(plane.y), 1f, 1f);
        Vector4 qView = cam.projectionMatrix.inverse.MultiplyPoint(qClip);

        Vector4 scaledPlane = plane * 2.0f / Vector4.Dot(plane, qView);
        Vector4 m3 = scaledPlane - cam.projectionMatrix.GetRow(3);

        Matrix4x4 newM = cam.projectionMatrix;
        newM.SetRow(2, m3);

        return newM;
    }

    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        var offsetPos = pos;
        var m = cam.worldToCameraMatrix;
        var cameraPosition = m.MultiplyPoint(offsetPos);
        var cameraNormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cameraNormal.x, cameraNormal.y, cameraNormal.z, -Vector3.Dot(cameraPosition, cameraNormal));
    }

    private static Matrix4x4 CalculateReflectionMatrix(Vector4 plane)
    {
        Matrix4x4 reflectionMat = Matrix4x4.identity;
        reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2F * plane[3] * plane[0]);

        reflectionMat.m10 = (-2F * plane[1] * plane[0]);
        reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2F * plane[3] * plane[1]);

        reflectionMat.m20 = (-2F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2F * plane[2] * plane[1]);
        reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2F * plane[3] * plane[2]);

        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;

        return reflectionMat;
    }

    private void ResetCullingOrder()
    {
        GL.invertCulling = _invertCulling;
    }

    private void SetCullingOrder()
    {
        _invertCulling = GL.invertCulling;
        GL.invertCulling = !_invertCulling;
    }

    private Camera CreateReflectCamera()
    {
        var go = new GameObject(gameObject.name + " Planar Reflection Camera", typeof(Camera));
        var cameraData = go.AddComponent(typeof(UniversalAdditionalCameraData)) as UniversalAdditionalCameraData;

        cameraData.requiresColorOption = CameraOverrideOption.Off;
        cameraData.requiresDepthOption = CameraOverrideOption.Off;
        cameraData.renderShadows = false;
        cameraData.SetRenderer(0);

        var t = transform;
        var reflectionCamera = go.GetComponent<Camera>();
        reflectionCamera.transform.SetPositionAndRotation(transform.position, t.rotation);
        reflectionCamera.depth = -10;
        reflectionCamera.enabled = false;
        go.hideFlags = HideFlags.HideAndDontSave;

        return reflectionCamera;
    }
}

