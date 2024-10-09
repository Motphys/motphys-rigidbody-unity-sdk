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

namespace Motphys.Rigidbody.Demos
{
    public class ExplosionDemo : MonoBehaviour
    {
        private bool _explosion = false;

        [Header("Common Setting")]
        [SerializeField]
        private float _countdown = 1.5f;
        [SerializeField]
        private float _force = 100;
        [SerializeField]
        [Range(0, 50)]
        private int _maxTestColliderCount = 20;

        [Header("Emission Setting")]
        [SerializeField]
        private AnimationCurve _emissionCurve;
        [SerializeField]
        [ColorUsage(true, true)]
        private Color _initColor;
        [ColorUsage(true, true)]
        [SerializeField]
        private Color _finalColor;
        [SerializeField]
        private MeshRenderer _bomb;
        private Material _emissionMaterial;

        [Header("Explosion Setting")]
        [SerializeField]
        private MeshRenderer _distortionObject;
        [SerializeField]
        private AnimationCurve _explosionCurve;
        [SerializeField]
        [Range(0, 100)]
        private float _maxRange = 0;
        [SerializeField]
        private Material _distortionMaterial;
        [SerializeField]
        [Range(0.0f, 0.5f)]
        private float _maxDistort = 0.1f;

        private float _explosionTime = 0;

        private BaseCollider[] _motphysColliders;

        public void Start()
        {
            //Ensure the render pipeline supports opaque texture.
            var urpAssets = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (urpAssets != null && !urpAssets.supportsCameraOpaqueTexture)
            {
                urpAssets.supportsCameraOpaqueTexture = true;
            }

            _motphysColliders = new BaseCollider[_maxTestColliderCount];
            if (_bomb != null)
            {
                _emissionMaterial = _bomb.sharedMaterial;
                _emissionMaterial.SetColor("_EmissionColor", _initColor);
            }
        }

        private void Update()
        {
            if (!_explosion)
            {
                var normalizeTime = 1 - _countdown / 1.5f;
                UpdateEmission(normalizeTime);
            }
            else
            {
                UpdateDistortion();
                UpdateEmission(1 - _explosionTime);
            }

            _countdown -= Time.deltaTime;

            if (_countdown <= 0 && !_explosion)
            {
                _explosion = true;
                var bombCenter = _bomb.transform.position;
                var numColliders = PhysicsManager.OverlapSphereNonAlloc(bombCenter, 6, _motphysColliders);

                for (var i = 0; i < numColliders; i++)
                {
                    var collider = _motphysColliders[i];
                    if (collider != null)
                    {
                        var rigidBody = collider.GetComponent<Rigidbody3D>();
                        if (rigidBody != null)
                        {
                            var direction = collider.transform.position - bombCenter;
                            rigidBody.AddForceAtPosition(direction.normalized * _force, collider.transform.position, Rigidbody.ForceMode.Force);
                        }
                    }
                }
            }
        }

        private void UpdateEmission(float normalizeTime)
        {
            var value = _emissionCurve.Evaluate(normalizeTime);
            var color = Color.Lerp(_initColor, _finalColor, value);
            _emissionMaterial.SetColor("_EmissionColor", color);
        }
        private void UpdateDistortion()
        {
            _explosionTime += Time.deltaTime;
            _explosionTime = Mathf.Clamp01(_explosionTime);
            var value = _explosionCurve.Evaluate(_explosionTime);
            var bombSize = Mathf.Lerp(0, _maxRange, value);
            _distortionObject.transform.localScale = Vector3.one * bombSize;

            var distortionValue = Mathf.Lerp(_maxDistort, 0, value);
            _distortionMaterial.SetFloat("_distortedInt", distortionValue);
        }
    }
}
