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
    public class BreakoutGame : MonoBehaviour
    {
        [Header("Custom Settings")]
        [SerializeField]
        private Rigidbody3D _player;
        [SerializeField]
        private BaseCollider _playerCollider;

        [Range(0, 100)]
        [SerializeField]
        private float _playerRunningSpeed;
        [SerializeField]
        [Range(1, 100)]
        private float _platformMoveSpeed;
        [SerializeField]
        [Range(0, 50)]
        private int _rockXCount = 20;
        [SerializeField]
        [Range(0, 50)]
        private int _rockZCount = 5;
        [SerializeField]
        [Range(0, 50)]
        private float _xInterval = 2;
        [SerializeField]
        [Range(0, 50)]
        private float _zInterval = 1;

        [SerializeField]
        private Vector3 _spawnPosition = new Vector3(0, 0.5f, -12);
        [SerializeField]
        private Vector3 _platformPosition = new Vector3(0, 0, -14);
        [SerializeField]
        private BaseCollider _bottomTrigger;
        [SerializeField]
        private GameObject _platform;
        [SerializeField]
        private List<GameObject> _rockPrefabs;

        private int _score = 0;

        private List<BaseCollider> _rocks;

        [SerializeField]
        private float _xLimit = 20;
        private bool _gameover;
        private Vector3 _hitPoint;
        private Vector3 _rayPoint;
        private Vector3 _hitNormal;
        private Vector3 _leftBottom = new Vector3(-20, 0, 0);

        private List<GameObject> _dyingRocks;

        [SerializeField]
        private UnityEngine.UI.Text _scoreText;
        [SerializeField]
        private UnityEngine.UI.Text _tip;

        private Vector3 _moveDirection = Vector3.zero;

        private void Start()
        {
            _dyingRocks = new List<GameObject>();

            GameStart();

            _playerCollider.onCollisionEnter += OnCollision;
            _bottomTrigger.onTriggerEnter += OnTrigger;
        }

        private void OnDisable()
        {
            _playerCollider.onCollisionEnter -= OnCollision;
            _bottomTrigger.onTriggerEnter -= OnTrigger;
        }

        private void OnCollision(Motphys.Rigidbody.Collision collision)
        {
            if (_gameover)
            {
                return;
            }

            if (collision.gameObject.tag != "Finish" && collision.gameObject != _platform)
            {
                _dyingRocks.Add(collision.gameObject);

                UpdateScore();
            }

            var dir = _player.linearVelocity.normalized;
            _player.linearVelocity = dir * _playerRunningSpeed;
        }

        private void GenerateRocks()
        {

            if (_rocks == null)
            {
                _rocks = new List<BaseCollider>();
            }
            else
            {
                foreach (var rock in _rocks)
                {
                    Destroy(rock.gameObject);
                }

                _rocks.Clear();
            }

            var count = _rockPrefabs.Count;

            for (var i = 0; i < _rockXCount; i++)
            {
                for (var j = 0; j < _rockZCount; j++)
                {
                    var index = Random.Range(0, count);

                    var prefab = _rockPrefabs[index];
                    var position = new Vector3(i * _xInterval, 0, j * _zInterval) + _leftBottom;
                    var rock = Instantiate(prefab, position, Quaternion.identity);
                    var collider = rock.GetComponent<BaseCollider>();
                    _rocks.Add(collider);
                }
            }
        }

        private void Update()
        {
            if (_gameover)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    GameRestart();
                }
            }
            else
            {
                ClearDying();
                HandlePlatform();
            }
#if UNITY_EDITOR
            UpdateGizmos();
#endif
        }

        private void OnTrigger(Motphys.Rigidbody.BaseCollider baseCollider)
        {
            if (baseCollider.gameObject == _player.gameObject)
            {
                GameOver();
            }
        }

        private void HandlePlatform()
        {
            if (!Input.touchSupported)
            {
                _moveDirection = Vector3.zero;

                if (Input.GetKey(KeyCode.A))
                {
                    _moveDirection = Vector3.left;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    _moveDirection = Vector3.right;
                }
            }

            _platform.transform.Translate(_moveDirection * _platformMoveSpeed * Time.deltaTime);
            var position = _platform.transform.position;
            if (position.x > _xLimit)
            {
                position.x = _xLimit;
                _platform.transform.position = position;
            }
            else if (position.x < -_xLimit)
            {
                position.x = -_xLimit;
                _platform.transform.position = position;
            }
        }

        private void ClearDying()
        {
            foreach (var dying in _dyingRocks)
            {
                dying.SetActive(false);
            }

            _dyingRocks.Clear();
        }

        private void GameStart()
        {
            GenerateRocks();
            this._score = 0;
            _scoreText.text = "0";
            _tip.gameObject.SetActive(false);
            _player.position = this._spawnPosition;
            _platform.transform.position = _platformPosition;
            _player.linearVelocity = new Vector3(_playerRunningSpeed, 0, _playerRunningSpeed);
        }

        private void GameOver()
        {
            _player.linearVelocity = Vector3.zero;
            _gameover = true;
            _scoreText.text = "Game Over";
            _tip.gameObject.SetActive(true);
        }

        private void UpdateScore()
        {
            this._score++;
            _scoreText.text = _score.ToString();
        }
        private void GameRestart()
        {
            _gameover = false;
            GameStart();
        }

#if UNITY_EDITOR
        private void UpdateGizmos()
        {
            var position = _player.transform.position;
            var direction = _player.linearVelocity.normalized;
            var hitCount = PhysicsManager.RaycastClosest(position + direction * 2, direction, out var hit, 300);
            if (hitCount)
            {
                _hitPoint = hit.point;
                _hitNormal = hit.normal;
            }

            _rayPoint = position + direction * 2;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_rayPoint, _hitPoint);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_hitPoint, _hitPoint + _hitNormal * 5);
        }
#endif
    }
}
