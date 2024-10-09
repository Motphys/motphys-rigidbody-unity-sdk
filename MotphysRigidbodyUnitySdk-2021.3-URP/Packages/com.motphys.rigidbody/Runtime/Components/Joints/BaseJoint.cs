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
using Motphys.Rigidbody.Internal;
using UnityEngine;

namespace Motphys.Rigidbody
{
    [RequireComponent(typeof(Rigidbody3D))]
    [DefaultExecutionOrder(100)]
    public abstract class BaseJoint : NativeObject
    {
        /// <value>
        /// Whether configure connected body's anchor frame automatically.
        /// </value>
        [SerializeField]
        protected bool _autoConfigureConnected = true;

        /// <value>
        /// The connected body.
        /// </value>
        [SerializeField]
        protected Rigidbody3D _connectBody;

        [SerializeField]
        private AxisFrame _anchorFrame = AxisFrame.Identity;
        [SerializeField]
        private AxisFrame _connectedAnchorFrame = AxisFrame.Identity;

        [SerializeField]
        private bool _ignoreCollision = true;

        [SerializeField]
        private float _breakForce = float.PositiveInfinity;
        [SerializeField]
        private float _breakTorque = float.PositiveInfinity;

        private JointId _id;
        private bool _dirty = false;
        private bool _active = true;

        private bool _destroyWithoutNotifyNative = false;

        /// <summary>
        /// If a joint is makred as dirty. it will send all it's properties from managed to native before simulation,
        /// and also will wake up all bodies that connected by this joint.
        /// </summary>
        internal void SetDirty()
        {
            if (hasNativeInitialized)
            {
                _dirty = true;
                s_dirtyJoints.Add(this);
            }
        }

        /// <value>
        /// Whether configure connected body's anchor frame automatically.
        /// </value>
        public bool autoConfigureConnected
        {
            get => _autoConfigureConnected;
            set
            {
                if (_autoConfigureConnected == value)
                {
                    return;
                }

                _autoConfigureConnected = value;
                if (TryAutoConfigureConnected())
                {
                    SetDirty();
                }
            }
        }

        /// <value>
        /// If true, the collision of the two connected rigidbody will be ignored.
        /// </value>
        public bool ignoreCollision
        {
            get { return _ignoreCollision; }
            set
            {
                if (_ignoreCollision == value)
                {
                    return;
                }

                _ignoreCollision = value;
                SetDirty();
            }
        }

        /// <value>
        /// The anchor frame defined in body A's local space.
        /// </value>
        public AxisFrame anchorFrame
        {
            get
            {
                return _anchorFrame;
            }
            set
            {
                _anchorFrame = value;
                SetDirty();
            }
        }

        /// <value>
        /// The connected body's anchor frame in connected object's local space.
        ///
        /// Note: If autoConfigureConnected is true, the set value will be ignored.
        /// </value>
        public AxisFrame connectedAnchorFrame
        {
            get
            {
                return _connectedAnchorFrame;
            }
            set
            {
                if (autoConfigureConnected)
                {
                    return;
                }

                _connectedAnchorFrame = value;
                SetDirty();
            }
        }

        /// <value>
        /// The body A's anchor position in gameObject's local space.
        /// </value>
        public Vector3 anchorPosition
        {
            get
            {
                return _anchorFrame.anchor;
            }
            set
            {
                _anchorFrame.anchor = value;
                SetDirty();
            }
        }

        /// <value>
        /// The body A's anchor position in world space.
        /// </value>
        public Vector3 worldAnchorPosition
        {
            get
            {
                var scaledPos = Vector3.Scale(anchorPosition, transform.lossyScale);
                return transform.rotation * scaledPos + transform.position;
            }
        }

        /// <value>
        /// The body A's anchor rotation in world space.
        /// </value>
        public Quaternion worldAnchorRotation
        {
            get
            {
                return transform.rotation * _anchorFrame.rotation;
            }
        }

        /// <value>
        /// The connected body's anchor in connected object's local space.
        ///
        /// Note: if autoConfigureConnected is true, the set value will be ignored.
        /// </value>
        public Vector3 connectedAnchorPosition
        {
            get
            {
                return _connectedAnchorFrame.anchor;
            }
            set
            {
                if (autoConfigureConnected)
                {
                    return;
                }

                _connectedAnchorFrame.anchor = value;
                SetDirty();
            }
        }

        /// <value>
        /// The connected body's anchor position in world space
        /// </value>
        public Vector3 worldConnectedAnchorPosition
        {
            get
            {
                if (_connectBody)
                {
                    var connectedTransform = _connectBody.transform;
                    return connectedTransform.rotation * connectedAnchorPosition + connectedTransform.position;
                }
                else
                {
                    return connectedAnchorPosition;
                }
            }
        }

        /// <value>
        /// The connected body's anchor rotation in world space
        /// </value>
        public Quaternion worldConnectedAnchorRotation
        {
            get
            {
                if (_connectBody)
                {
                    return _connectBody.transform.rotation * _connectedAnchorFrame.rotation;
                }
                else
                {
                    return _connectedAnchorFrame.rotation;
                }
            }
        }

        /// <value>
        /// The rigidbody that this joint is attached to
        /// </value>
        public Rigidbody3D body
        {
            get
            {
                return gameObject.GetComponent<Rigidbody3D>();
            }
        }

        /// <value>
        /// The rigidbody that this joint is connected to
        /// </value>
        ///
        /// <exception cref="System.ArgumentException">Thrown when trying to set the connected body to the same object that the joint is attached to.</exception>
        public Rigidbody3D connectedBody
        {
            get
            {
                return _connectBody;
            }
            set
            {
                if (_connectBody == value)
                {
                    return;
                }

                if (_connectBody != null && _connectBody.gameObject == gameObject)
                {
                    throw new System.ArgumentException("A joint cannot be connected to the same object it is attached to.");
                }

                _connectBody = value;
                TryAutoConfigureConnected();
                SetDirty();
            }
        }

        internal bool isDirty
        {
            get { return _dirty; }
        }

        internal bool active
        {
            get { return _active; }
        }

        /// <value>
        /// The force required to break joint.
        /// </value>
        public float breakForce
        {
            get { return _breakForce; }
            set
            {
                if (_breakForce == value)
                {
                    return;
                }

                _breakForce = value;
                SetDirty();
            }
        }

        /// <value>
        /// The torque required to break joint.
        /// </value>
        public float breakTorque
        {
            get { return _breakTorque; }
            set
            {
                if (_breakTorque == value)
                {
                    return;
                }

                _breakTorque = value;
                SetDirty();
            }
        }

        /// <value>
        /// The force currently acting on the joint.
        /// </value>
        internal Vector3 force
        {
            get
            {
                if (!hasNativeInitialized)
                {
                    return Vector3.zero;
                }

                return PhysicsManager.defaultWorld.GetJointForce(_id);
            }
        }

        /// <value>
        /// The torque currently acting on the joint.
        /// </value>
        internal Vector3 torque
        {
            get
            {
                if (!hasNativeInitialized)
                {
                    return Vector3.zero;
                }

                return PhysicsManager.defaultWorld.GetJointTorque(_id);
            }
        }

        internal bool TryAutoConfigureConnected()
        {
            if (autoConfigureConnected && connectedBody)
            {
                var connectedAnchorPosition = connectedBody.transform.InverseTransformPoint(worldAnchorPosition);
                var rotation = Quaternion.Inverse(connectedBody.transform.rotation) * worldAnchorRotation;
                _connectedAnchorFrame = new AxisFrame()
                {
                    anchor = connectedAnchorPosition,
                    axisRotation = rotation.eulerAngles,
                };
                return true;
            }

            return false;
        }

        internal abstract bool TryCreateTypedJointConfig(out TypedJointConfig typedConfig);

        internal bool TryCreateJointConfig(out JointConfig config)
        {
            config = default;

            if (!TryCreateTypedJointConfig(out var typedConfig))
            {
                return false;
            }

            var anchorFrameA = new AxisFrame()
            {
                anchor = Vector3.Scale(anchorFrame.anchor, transform.lossyScale),
                axisRotation = anchorFrame.axisRotation,
            };

            var anchorFrameB = new AxisFrame()
            {
                anchor = _connectBody == null ? connectedAnchorFrame.anchor : Vector3.Scale(connectedAnchorFrame.anchor, connectedBody.transform.lossyScale),
                axisRotation = connectedAnchorFrame.axisRotation,
            };

            config = new JointConfig()
            {
                active = active,
                ignoreCollision = ignoreCollision,
                anchorFramePair = AnchorFramePair.FromAxisFrame(anchorFrameA, anchorFrameB),
                type = typedConfig,
                breakForce = _breakForce,
                breakTorque = _breakTorque,
            };

            return true;
        }

        protected override void Awake()
        {
            _active = enabled;
            TryAutoConfigureConnected();
            base.Awake();
            _active = hasNativeInitialized;
        }

        private bool TryCreateJointBuilder(out JointBuilder jointBuilder)
        {
            jointBuilder = default;

            var bodyA = GetComponent<Rigidbody3D>();

            var bodyIdA = bodyA == null ? RigidbodyId.Invalid : bodyA.rigidbodyId;
            var bodyIdB = _connectBody == null ? RigidbodyId.Invalid : _connectBody.rigidbodyId;

            if (!TryCreateJointConfig(out var config))
            {
                return false;
            }

            jointBuilder = new JointBuilder(bodyIdA, bodyIdB, config);

            return true;
        }

        protected override void OnCreateNative()
        {
            if (TryCreateJointBuilder(out var builder))
            {
                _id = PhysicsManager.defaultWorld.AddJoint(builder);
            }
        }

        protected override void OnDestroyNative()
        {
            if (PhysicsManager.isEngineCreated && !_destroyWithoutNotifyNative)
            {
                PhysicsManager.defaultWorld.RemoveJoint(_id);
            }
        }

        protected virtual void OnEnable()
        {
            Register(this);
            if (!_active)
            {
                _active = true;
                if (hasNativeInitialized)
                {
                    PhysicsManager.defaultWorld.SetJointActive(_id, true);
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (hasNativeInitialized)
            {
                Unregister(id);
                _active = false;
                if (PhysicsManager.isEngineCreated && !_destroyWithoutNotifyNative)
                {
                    PhysicsManager.defaultWorld.SetJointActive(_id, false);
                }
            }
        }

        private void DestroyWithoutNotifyNative()
        {
            _destroyWithoutNotifyNative = true;
            Destroy(this);
        }

        protected void ApplyProperties()
        {
            if (this.hasNativeInitialized)
            {
                if (TryCreateJointBuilder(out var joint))
                {
                    PhysicsManager.engine.defaultWorld.UpdateJointProperties(_id, joint);
                    _dirty = false;
                }
                else
                {
                    Debug.LogError("failed to create joint builder", this);
                }
            }
        }

        public override string ToString()
        {
            return string.Format(
                "type = {0}, name = {1} , id = {2}",
                this.GetType(),
                this.name,
                _id
            );
        }

        internal JointId id
        {
            get { return _id; }
        }

        private static Dictionary<JointId, BaseJoint> s_joints =
            new Dictionary<JointId, BaseJoint>();
        private static HashSet<BaseJoint> s_dirtyJoints = new HashSet<BaseJoint>();

        internal static bool DestroyJointWithoutNotifyNative(JointId id)
        {
            if (s_joints.Remove(id, out var joint))
            {
                joint.DestroyWithoutNotifyNative();
                return true;
            }

            return false;
        }

        private static void Register(BaseJoint joint)
        {
            s_joints.Add(joint.id, joint);
        }

        private static void Unregister(JointId id)
        {
            s_joints.Remove(id);
        }

        /// <value>
        /// All active joints
        /// </value>
        internal static IReadOnlyDictionary<JointId, BaseJoint> joints
        {
            get { return s_joints; }
        }

        internal static void CleanDirtyJoints()
        {
            foreach (var joint in s_dirtyJoints)
            {
                if (joint)
                {
                    joint.ApplyProperties();
                }
            }

            s_dirtyJoints.Clear();
        }

#if UNITY_EDITOR
        [System.Serializable]
        [UnityEngine.TestTools.ExcludeFromCoverage]
        internal class D3AngularMotorPack
        {
            [SerializeField]
            internal D3AngularMotor _rotationMotor = D3AngularMotor.Default;
            [SerializeField]
            internal D3AngularMotor _velocityMotor = D3AngularMotor.Default;
        }
        [System.Serializable]
        [UnityEngine.TestTools.ExcludeFromCoverage]
        internal class LinearMotorPack
        {
            [SerializeField]
            public LinearMotor _distanceMotor = LinearMotor.Default;
            [SerializeField]
            public LinearMotor _speedMotor = LinearMotor.Default;
        }

        protected virtual void OnValidate()
        {
            ValidateConnectedBody();
            ValidateAtNotPlaying();
            SetDirty();
        }

        /// <summary>
        /// Invalid connected body only happens when user drag and assign in editor inspector. So exclude it from test coverage.
        /// </summary>
        [UnityEngine.TestTools.ExcludeFromCoverage]
        private void ValidateConnectedBody()
        {
            if (_connectBody != null && _connectBody.gameObject == gameObject)
            {
                _connectBody = null;
            }
        }

        /// <summary>
        /// Only execute when user updates properties on editor inspector. So exclude it from test coverage.
        /// </summary>
        [UnityEngine.TestTools.ExcludeFromCoverage]
        private void ValidateAtNotPlaying()
        {
            if (!Application.isPlaying)
            {
                TryAutoConfigureConnected();
            }
        }

        /// <summary>
        /// Used to draw gizmos in editor. So exclude it from test coverage.
        /// </summary>
        [UnityEngine.TestTools.ExcludeFromCoverage]
        protected virtual void OnDrawGizmosSelected()
        {
        }
#endif
    }
}
