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
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Serialization;

namespace Motphys.Rigidbody
{
    /// <summary>
    /// Freeze option, which restricts degrees of freedom along one or more specific axes
    /// </summary>
    [System.Serializable]
    public struct FreezeOptions
    {
        /// <value>
        /// Corresponding to the x-axis
        /// </value>
        public bool x;
        /// <value>
        /// Corresponding to the y-axis
        /// </value>
        public bool y;
        /// <value>
        /// Corresponding to the z-axis
        /// </value>
        public bool z;
    }

    // TODO: improve test coverage.
    // http://gitlab.mp/motphys/motphys_sdk_remake/-/issues/587
    [DefaultExecutionOrder(0)]
    [AddComponentMenu("Motphys/Rigidbody3D")]
    public class Rigidbody3D : MonoBehaviour
    {

        [SerializeField]
        [FormerlySerializedAs("mass")]
        private float _mass = 1.0f;

        [SerializeField]
        private bool _enableGravity = true;

        [SerializeField]
        private bool _kinematics = false;

        [SerializeField]
        private bool _enablePostTransformControl = false;

        [SerializeField]
        private FreezeOptions _freezePosition;

        [SerializeField]
        private FreezeOptions _freezeRotation;

        [SerializeField]
        [FormerlySerializedAs("_linearDamper")]
        private float _drag = 0f;

        [SerializeField]
        [FormerlySerializedAs("_angularDamper")]
        private float _angularDrag = 0f;

        [SerializeField]
        private float _maxLinearVelocity = 100f;

        [SerializeField]
        private float _maxAngularVelocity = 2 * Mathf.PI;

        [SerializeField]
        private float _sleepEnergyThreshold = 0.04f;

        private Transform _transform;
        private bool _transformInitialized = false;
        private bool _detectCollision = true;
        private BodyNativeBridge _nativeBridge;
        private BodyActivationTrack _bodyActivationTrack;

        // These colliders are attached to this rigidbody3d component in native.
        private HashSet<BaseCollider> _childColliders = new HashSet<BaseCollider>();

        private Vector3 _lastPosition;
        private Quaternion _lastRotation;

        private void AddColliderInChildren(Transform transform)
        {
            foreach (Transform child in transform)
            {
                if (!child.GetComponent<Rigidbody3D>())
                {
                    foreach (var collider in GetColliderComponents(child.gameObject))
                    {
                        if (collider.enabled)
                        {
                            collider.AttachToRigidBody3D(this);
                        }
                    }

                    AddColliderInChildren(child);
                }
            }
        }

        private void Awake()
        {
            _bodyActivationTrack = gameObject.AddComponent<BodyActivationTrack>();
            _bodyActivationTrack.rigidbody3d = this;

            _nativeBridge = new BodyNativeBridge(this);

            foreach (var collider in GetColliderComponents(gameObject))
            {
                if (collider.enabled)
                {
                    collider.AttachToRigidBody3D(this);
                }
            }

            AddColliderInChildren(transform);

            Register(this);

            if (_enablePostTransformControl)
            {
                RegisterPostControlBody();
            }
        }

        private void OnDestroy()
        {
            // BodyActivationTrack judges the reason for Disable based on whether rigidbody3d is null.
            _bodyActivationTrack.rigidbody3d = null;
            Destroy(_bodyActivationTrack);

            Unregister(nativeBridge.id);
            UnregisterPostControlBody();

            // engine may have been disposed already when OnDestroy is called on application quit phase.
            if (!nativeBridge.isEngineDisposed)
            {
                nativeBridge.DestroyNativeImmendiate();
                _nativeBridge = null;
            }

            foreach (var collider in _childColliders)
            {
                collider.OnRigidBody3DDestroy();
            }

            _childColliders.Clear();
        }

        internal void OnGameObjectActivate()
        {
            Debug.Assert(_nativeBridge != null);

            nativeBridge.handle.SetActive(true);
        }

        internal void OnGameObjectDeactivate()
        {
            Debug.Assert(_nativeBridge != null);

            if (!nativeBridge.isEngineDisposed)
            {
                nativeBridge.handle.SetActive(false);
            }
        }

        internal void AddColliderComponent(BaseCollider collider)
        {
            _childColliders.Add(collider);
        }

        internal void RemoveColliderComponent(BaseCollider collider)
        {
            _childColliders.Remove(collider);
        }

        /// <summary>
        /// Iterate over all colliders attached to this rigidbody
        /// </summary>
        internal void ForeachAttachedCollider(System.Action<BaseCollider> action)
        {
            foreach (var collider in _childColliders)
            {
                action(collider);
            }
        }

        private IEnumerable<Renderer> IterChildRendererImpl(Transform transform)
        {
            var rendererOnSelf = transform.GetComponent<Renderer>();
            if (rendererOnSelf)
            {
                yield return rendererOnSelf;
            }

            foreach (Transform child in transform)
            {
                if (!child.GetComponent<Rigidbody3D>())
                {
                    foreach (var renderer in IterChildRendererImpl(child))
                    {
                        yield return renderer;
                    }
                }
            }
        }

        internal IEnumerable<Renderer> IterChildRenderer()
        {
            return IterChildRendererImpl(transform);
        }

        internal BodyNativeBridge nativeBridge
        {
            get
            {
                return _nativeBridge;
            }
        }

        internal bool isNativeValid => nativeBridge != null;

        /// <value>
        /// The transform component which the rigidbody attached to.
        /// </value>
        public new Transform transform
        {
            get
            {
                // we use _transformInitialized instead of _transform == null
                // because the null check for transform has native call cost.
                if (!_transformInitialized)
                {
                    _transform = GetComponent<Transform>();
                    _transformInitialized = true;
                }

                return _transform;
            }
        }

        /// <value>
        /// If true, the gameobject's transform information will be synchronized to the physics engine.
        /// </value>
        public bool enablePostTransformControl
        {
            get { return _enablePostTransformControl; }
            set
            {
                if (_enablePostTransformControl == value)
                {
                    return;
                }

                _enablePostTransformControl = value;

                if (isNativeValid)
                {
                    if (value)
                    {
                        RegisterPostControlBody();
                    }
                    else
                    {
                        UnregisterPostControlBody();
                    }
                }
            }
        }

        /// <value>
        /// Whether the rigidbody is affected by gravity.
        /// </value>
        public bool useGravity
        {
            get
            {
                return _enableGravity;
            }
            set
            {
                _enableGravity = value;

                if (isNativeValid)
                {
                    nativeBridge.useGravity = value;
                }
            }
        }

        /// <value>
        /// The maximum mass of a rigidbody.
        /// </value>
        public const float MAX_MASS = 1e9f;

        /// <value>
        /// The mass of the rigidbody. 0 means infinite mass.
        ///
        /// If the set value is negative, it will be clamped to 0.
        /// </value>
        public float mass
        {
            get
            {
                return _mass;
            }
            set
            {
                value = Mathf.Clamp(value, 0, MAX_MASS);
                _mass = value;
                if (isNativeValid)
                {
                    nativeBridge.mass = value;
                }
            }
        }
        /// <value>
        /// The center of mass of the rigidbody in local space.
        /// </value>
        public Vector3 centerOfMass
        {
            set
            {
                if (isNativeValid)
                {
                    nativeBridge.SetBodyCenterOfMass(value);
                }
            }

            get
            {
                if (isNativeValid)
                {
                    return nativeBridge.GetBodyCenterOfMass();
                }

                return Vector3.zero;
            }
        }

        /// <value>
        /// The center of mass of the rigidbody in world space.
        /// </value>
        public Vector3 worldCenterOfMass
        {
            get
            {
                var tr = Matrix4x4.TRS(this.transform.position, this.transform.rotation, Vector3.one);
                var position = centerOfMass;
                return tr * new Vector4(position.x, position.y, position.z, 1.0f);
            }
        }

        /// <value>
        /// Drag can slow down the rigidbody. The higher the drag the more the object slows down.
        /// </value>
        public float drag
        {
            get
            {
                return _drag;
            }
            set
            {
                _drag = Mathf.Max(value, 0);

                if (isNativeValid)
                {
                    nativeBridge.linearDamper = _drag;
                }
            }
        }

        /// <value>
        /// Angular drag can be used to slow down the rotation of an object. The higher the drag the more the rotation slows down.
        /// </value>
        public float angularDrag
        {
            get
            {
                return _angularDrag;
            }
            set
            {
                _angularDrag = Mathf.Max(value, 0);

                if (isNativeValid)
                {
                    nativeBridge.angularDamper = _angularDrag;
                }
            }
        }

        /// <value>
        /// The mass-normalized energy threshold, below which objects start going to sleep.
        /// </value>
        public float sleepThreshold
        {
            get
            {
                return _sleepEnergyThreshold;
            }
            set
            {
                _sleepEnergyThreshold = Mathf.Max(value, 0);

                if (isNativeValid)
                {
                    nativeBridge.sleepThreshold = _sleepEnergyThreshold;
                }
            }
        }

        /// <value>
        /// The position of the rigidbody in world space.
        ///
        /// Note: modifying this position won't immediately change the position of the transform.
        /// </value>
        public Vector3 position
        {
            get
            {
                if (isNativeValid)
                {
                    return nativeBridge.position;
                }
                else
                {
                    return transform.position;
                }
            }
            set
            {
                if (isNativeValid)
                {
                    if (_kinematics)
                    {
                        transform.position = value;
                    }

                    nativeBridge.position = value;
                }
                else
                {
                    transform.position = value;
                }
            }
        }

        /// <value>
        /// The rotation of the rigidbody in world space.
        ///
        /// Note: modifying this rotation won't immediately change the rotation of the transform.
        /// </value>
        public Quaternion rotation
        {
            get
            {
                if (isNativeValid)
                {
                    return nativeBridge.rotation;
                }
                else
                {
                    return transform.rotation;
                }
            }
            set
            {
                if (isNativeValid)
                {
                    if (_kinematics)
                    {
                        transform.rotation = value;
                    }

                    nativeBridge.rotation = value;
                }
                else
                {
                    transform.rotation = value;
                }
            }
        }

        /// <value>
        /// The linear velocity of the rigidbody in world space.
        ///
        /// If the rigidbody is kinematic, the velocity is always zero.
        /// </value>
        public Vector3 linearVelocity
        {
            get
            {
                if (!isNativeValid)
                {
                    return Vector3.zero;
                }
                else
                {
                    return nativeBridge.linearVelocity;
                }
            }
            set
            {
                if (isNativeValid)
                {
                    nativeBridge.linearVelocity = value;
                }
            }
        }

        /// <value>
        /// The angular velocity of the rigidbody in world space.
        ///
        /// If the rigidbody is kinematic, the velocity is always zero.
        /// </value>
        public Vector3 angularVelocity
        {
            get
            {
                if (!isNativeValid)
                {
                    return Vector3.zero;
                }
                else
                {
                    return nativeBridge.angularVelocity;
                }
            }
            set
            {
                if (isNativeValid)
                {
                    nativeBridge.angularVelocity = value;
                }
            }
        }

        /// <value>
        /// Whether this rigidbody is a kinematic object.
        /// </value>
        public bool isKinematic
        {
            get { return _kinematics; }
            set
            {
                if (_kinematics == value)
                {
                    return;
                }

                _kinematics = value;

                if (isNativeValid)
                {
                    nativeBridge.isKinematic = value;

                    if (!value)
                    {
                        transformAccessArrayBuilder.TryInsert(nativeBridge.handle.id, transform);
                    }
                    else
                    {
                        transformAccessArrayBuilder.Remove(nativeBridge.handle.id);
                    }
                }
            }
        }

        /// <value>
        /// The position freeze options
        /// </value>
        public FreezeOptions freezePosition
        {
            get { return _freezePosition; }
            set
            {
                _freezePosition = value;
                if (isNativeValid)
                {
                    nativeBridge.UpdateFreezeOptions();
                    WakeUp();
                }
            }
        }

        /// <value>
        /// The rotation freeze options
        /// </value>
        public FreezeOptions freezeRotation
        {
            get { return _freezeRotation; }
            set
            {
                _freezeRotation = value;
                if (isNativeValid)
                {
                    nativeBridge.UpdateFreezeOptions();
                    WakeUp();
                }
            }
        }

        /// <summary>
        /// Set a target transform to the kinematic rigidbody. This is a physical moving.
        /// </summary>
        public void SetKinematicTarget(Vector3 position, Quaternion rotation)
        {
            if (this.isKinematic)
            {
                nativeBridge.SetKinematicTarget(position, rotation);
                this.transform.position = position;
                this.transform.rotation = rotation;
            }
        }

        /// <summary>
        /// Add a force to the rigidbody at the specified position in world space. The force is only accumulated at the time of call.
        /// </summary>
        /// <param name="force"></param>
        /// <param name="position"></param>
        /// <param name="forceMode"></param>
        public void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode forceMode)
        {
            nativeBridge.AddForceAtPosition(force, position, forceMode);
        }

        /// <summary>
        /// Add a force to the rigidbody at the centroid in world space.
        /// </summary>
        /// <param name="force"></param>
        /// <param name="forceMode"></param>
        public void AddForce(Vector3 force, ForceMode forceMode)
        {
            nativeBridge.AddForce(force, forceMode);
        }

        /// <summary>
        /// Add a force to the rigidbody at the centroid in local space.
        /// </summary>
        /// <param name="torque"></param>
        /// <param name="forceMode"></param>
        public void AddRelativeForce(Vector3 force, ForceMode forceMode)
        {
            nativeBridge.AddRelativeForce(force, forceMode);
        }

        /// <summary>
        /// Add a torque to the rigidbody at the centroid in world space.
        /// </summary>
        /// <param name="torque"></param>
        /// <param name="forceMode"></param>
        public void AddTorque(Vector3 torque, ForceMode forceMode)
        {
            nativeBridge.AddTorque(torque, forceMode);
        }

        /// <summary>
        /// Add a torque to the rigidbody at the centroid in local space.
        /// </summary>
        /// <param name="force"></param>
        /// <param name="forceMode"></param>
        public void AddRelativeTorque(Vector3 torque, ForceMode forceMode)
        {
            nativeBridge.AddRelativeTorque(torque, forceMode);
        }

        /// <summary>
        /// Reset the center of mass of the rigidbody.
        /// Calculate the position of the center of mass automatically.
        /// </summary>
        public void ResetBodyCenterOfMass()
        {
            nativeBridge.ResetBodyCenterOfMass();
        }

        internal Freeze ExtractFreeze()
        {

            var freezePosition = this.freezePosition;
            var freezeRotation = this.freezeRotation;
            return new Freeze
            {
                freezePositionX = freezePosition.x,
                freezePositionY = freezePosition.y,
                freezePositionZ = freezePosition.z,
                freezeRotationX = freezeRotation.x,
                freezeRotationY = freezeRotation.y,
                freezeRotationZ = freezeRotation.z
            };
        }

        /// <value>
        /// The native rigidbody id. Returns an invalid id if the native one has not been initialized.
        ///
        /// Note: always return invalid in editor mode.
        /// </value>
        internal RigidbodyId rigidbodyId
        {
            get { return nativeBridge.id; }
        }

        internal ulong numShapes
        {
            get
            {
                if (isNativeValid)
                {
                    return nativeBridge.handle.numShapes;
                }

                return 0;
            }
        }

        internal Vector3 inertiaPrincipal
        {
            get
            {
                if (isNativeValid)
                {
                    return nativeBridge.handle.inertiaPrincipal;
                }

                return Vector3.zero;
            }
        }

        /// <value>
        /// Whether this rigidbody is sleeping.
        /// </value>
        public bool isSleeping
        {
            get
            {
                if (isNativeValid)
                {
                    return nativeBridge.sleepStatus == SleepStatus.Sleeping;
                }
                else
                {
                    return true;
                }
            }
        }
        internal float wakeCounter
        {
            get
            {
                if (isNativeValid)
                {
                    return nativeBridge.wakeCounter;
                }
                else
                {

                    return 0;
                }
            }
        }
        /// <value>
        /// The maximum linear velocity this rigidbody can achieve.
        /// </value>
        public float maxLinearVelocity
        {
            get { return _maxLinearVelocity; }
            set
            {
                _maxLinearVelocity = Mathf.Max(0.0f, value);

                if (isNativeValid)
                {
                    nativeBridge.maxLinearVelocity = _maxLinearVelocity;
                }
            }
        }

        /// <value>
        /// The maximum angular velocity this rigidbody can achieve.
        /// </value>
        public float maxAngularVelocity
        {
            get { return _maxAngularVelocity; }
            set
            {
                _maxAngularVelocity = Mathf.Max(0.0f, value);

                if (isNativeValid)
                {
                    nativeBridge.maxAngularVelocity = _maxAngularVelocity;
                }
            }
        }

        /// <value>
        /// This property is not serialized and not appeared in inspector. You can only set it in code. By default, it is always true.
        /// </value>
        /// <value>Should collision detection be enabled?(By default always enabled)</value>
        public bool detectCollisions
        {
            get => _detectCollision;
            set
            {
                _detectCollision = value;
                foreach (var collider in GetColliderComponents(gameObject))
                {
                    if (collider.enabled && collider.isNativeValid)
                    {
                        collider.detectCollisions = value;
                    }
                }
            }
        }

        /// <summary>
        /// Wake up this rigidbody, to update its status.
        /// </summary>
        public void WakeUp()
        {
            if (!isKinematic && isNativeValid)
            {
                nativeBridge.WakeUp();
            }
        }

        internal void OnWakeUp()
        {
            Debug.Assert(isNativeValid);

            if (!isKinematic)
            {
                transformAccessArrayBuilder.TryInsert(nativeBridge.id, this.transform);
            }
        }

        internal void OnSleepDown()
        {
            Debug.Assert(isNativeValid);

            if (!this.isKinematic)
            {
                transformAccessArrayBuilder.Remove(nativeBridge.id);
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            _maxAngularVelocity = Mathf.Max(0.0f, _maxAngularVelocity);
            _maxLinearVelocity = Mathf.Max(0.0f, _maxLinearVelocity);
            _drag = Mathf.Max(0.0f, _drag);
            _angularDrag = Mathf.Max(0.0f, _angularDrag);
            _sleepEnergyThreshold = Mathf.Max(0.0f, _sleepEnergyThreshold);
            _mass = Mathf.Clamp(_mass, 0, MAX_MASS);

            if (Application.isPlaying && isNativeValid)
            {
                nativeBridge.UpdateNativeRigidbodyProperties();
                WakeUp();

                var bodyId = rigidbodyId;
                if (isKinematic && transformAccessArrayBuilder.Contains(bodyId))
                {
                    transformAccessArrayBuilder.Remove(bodyId);
                }
                else if (!isKinematic && !transformAccessArrayBuilder.Contains(bodyId))
                {
                    transformAccessArrayBuilder.Insert(bodyId, transform);
                }

                if (_enablePostTransformControl)
                {
                    RegisterPostControlBody();
                }
                else
                {
                    UnregisterPostControlBody();
                }
            }
        }
#endif

        private static readonly Dictionary<RigidbodyId, Rigidbody3D> s_bodies =
            new Dictionary<RigidbodyId, Rigidbody3D>();
        private static TransformAccessArrayBuilder s_transformAccessArrayBuilder = null;

        private static readonly HashSet<Rigidbody3D> s_postControlBodies = new HashSet<Rigidbody3D>();

        internal static void Initialize()
        {
            s_transformAccessArrayBuilder = new TransformAccessArrayBuilder(1000);
        }

        internal static TransformAccessArrayBuilder transformAccessArrayBuilder
        {
            get
            {
                return s_transformAccessArrayBuilder;
            }
        }

        internal static int bodiesCount
        {
            get { return s_bodies.Count; }
        }

        internal static void RecordLastTransform()
        {
            foreach (var body in s_postControlBodies)
            {
                body._lastPosition = body.transform.position;
                body._lastRotation = body.transform.rotation;
            }
        }

        internal bool IsTransformChanged(out Vector3 position, out Quaternion rotation)
        {
            position = this.transform.position;
            rotation = this.transform.rotation;
            return position != _lastPosition || rotation != _lastRotation;
        }

        private void RegisterPostControlBody()
        {
            _lastPosition = this.transform.position;
            _lastRotation = this.transform.rotation;
            s_postControlBodies.Add(this);
        }

        private void UnregisterPostControlBody()
        {
            s_postControlBodies.Remove(this);
        }

        private static void Register(Rigidbody3D body)
        {
            var bodyId = body.rigidbodyId;
            s_bodies.Add(bodyId, body);

            if (!body.isKinematic)
            {
                //dynamic
                transformAccessArrayBuilder.Insert(bodyId, body.transform);
            }
        }

        private static void Unregister(RigidbodyId id)
        {
            s_bodies.Remove(id);
            if (transformAccessArrayBuilder != null)
            {
                transformAccessArrayBuilder.Remove(id);
            }
            else
            {
                // In this case, the PhysicsManager.Shutdown has been called before all rigidbody component's OnDestroy called.
                // we just ignore it.
            }
        }

        internal static Rigidbody3D Get(RigidbodyId rigidbodyId)
        {
            return s_bodies[rigidbodyId];
        }

        internal static Dictionary<RigidbodyId, Rigidbody3D>.Enumerator GetBodiesEnumerator()
        {
            return s_bodies.GetEnumerator();
        }

        internal static HashSet<Rigidbody3D>.Enumerator GetPostControlEnumerator()
        {
            return s_postControlBodies.GetEnumerator();
        }

        internal static void Dispose()
        {
            if (s_transformAccessArrayBuilder != null)
            {
                s_transformAccessArrayBuilder.DisposeIfNot();
                s_transformAccessArrayBuilder = null;
            }
        }

        private static List<BaseCollider> s_bufferForGetColliders = new List<BaseCollider>();
        private static List<BaseCollider> GetColliderComponents(GameObject gameObject)
        {
            gameObject.GetComponents<BaseCollider>(s_bufferForGetColliders);

            return s_bufferForGetColliders;
        }

        /// <summary>
        /// Incrementally build TransformAccessArray.
        /// </summary>
        internal class TransformAccessArrayBuilder : System.IDisposable
        {
            private TransformAccessArray _dynamicTransformAccesses;
            private NativeArray<RigidbodyId> _rigidbodyIds;
            private Dictionary<RigidbodyId, int> _id2index;
            private int _length;
            private int _capacity;

            public TransformAccessArrayBuilder(int capacity)
            {
                _length = 0;
                _capacity = capacity;
                _dynamicTransformAccesses = new TransformAccessArray(capacity);
                _rigidbodyIds = new NativeArray<RigidbodyId>(capacity, Allocator.Persistent);
                _id2index = new Dictionary<RigidbodyId, int>();
            }

            public int length
            {
                get
                {
                    return _length;
                }
            }

            private bool isDisposed
            {
                get
                {
                    return _capacity < 0;
                }
            }

            private void ThrowExceptionIfDisposed()
            {
                if (isDisposed)
                {
                    throw new System.ObjectDisposedException(this.GetType().Name);
                }
            }
            private void EnsureCapacity(int additional)
            {
                if (_length + additional > _capacity)
                {
                    _capacity *= 2;
                    //Extend TransformAccessArray
                    var newTransformArray = new TransformAccessArray(_capacity);
                    for (var i = 0; i < _length; i++)
                    {
                        newTransformArray.Add(_dynamicTransformAccesses[i]);
                    }

                    _dynamicTransformAccesses.Dispose();
                    _dynamicTransformAccesses = newTransformArray;

                    var newRigidBodyIds = new NativeArray<RigidbodyId>(_capacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                    newRigidBodyIds.Slice(0, _length).CopyFrom(_rigidbodyIds.Slice());
                    _rigidbodyIds.Dispose();
                    _rigidbodyIds = newRigidBodyIds;
                }
            }

            public void Insert(RigidbodyId id, Transform transform)
            {
                ThrowExceptionIfDisposed();
                EnsureCapacity(1);
                _id2index.Add(id, _length);
                _dynamicTransformAccesses.Add(transform);
                _rigidbodyIds[_length] = id;
                _length++;
            }

            /// <summary>
            /// Insert if not exists.
            ///
            /// Returns true if inserted successfully.
            /// </summary>
            public bool TryInsert(RigidbodyId id, Transform transform)
            {
                if (Contains(id))
                {
                    return false;
                }

                Insert(id, transform);
                return true;
            }

            public bool Contains(RigidbodyId id)
            {
                ThrowExceptionIfDisposed();
                return _id2index.ContainsKey(id);
            }

            public NativeSlice<RigidbodyId> rigidbodyIds
            {
                get
                {
                    return _rigidbodyIds.Slice(0, _length);
                }
            }

            public TransformAccessArray transformAccessArray
            {
                get
                {
                    return _dynamicTransformAccesses;
                }
            }

            public bool Remove(RigidbodyId id)
            {
                if (this.isDisposed)
                {
                    return false;
                }

                if (_id2index.Remove(id, out var index))
                {
                    var backIndex = _length - 1;
                    var backBodyId = _rigidbodyIds[backIndex];
                    //swap remove native array

                    _rigidbodyIds[index] = _rigidbodyIds[backIndex];
                    _dynamicTransformAccesses.RemoveAtSwapBack(index);

                    //Swap back object to current index.
                    if (backBodyId != id)
                    {
                        _id2index[backBodyId] = index;
                    }

                    _length--;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public void DisposeIfNot()
            {
                if (this.isDisposed)
                {
                    return;
                }

                this.Dispose();
            }

            public void Dispose()
            {
                ThrowExceptionIfDisposed();
                _id2index.Clear();
                _dynamicTransformAccesses.Dispose();
                _rigidbodyIds.Dispose();
                _capacity = -1;
                _length = -1;
            }
        }
    }
}
