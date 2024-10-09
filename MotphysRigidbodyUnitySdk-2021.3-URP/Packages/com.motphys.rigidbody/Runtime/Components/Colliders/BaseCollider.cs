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
using UnityEngine.Serialization;

namespace Motphys.Rigidbody
{
    // TODO: improve test coverage.
    // http://gitlab.mp/motphys/motphys_sdk_remake/-/issues/587
    [DefaultExecutionOrder(5)]
    public abstract class BaseCollider : MonoBehaviour
    {
        [Tooltip("The shared physics material used by the collider.")]
        [SerializeField]
        [FormerlySerializedAs("_material")]
        private PhysicMaterial _sharedMaterial;

        [Tooltip("If true, the collider will raise trigger events and will not generate any contacts with other colliders.")]
        [SerializeField]
        private bool _isTrigger = false;

        [Header("Collider Local Transform")]
        [Tooltip("The position of the collider in the local space of the GameObject.")]
        [SerializeField]
        private Vector3 _translation = Vector3.zero;

        [Tooltip("The rotation of the collider in the local space of the GameObject.")]
        [SerializeField]
        private Vector3 _rotation = Vector3.zero;

        [SerializeField]
        [HideInInspector]
        [Tooltip("Whether automatically update shape size when transform scale changed. Has little performance impact. Enable it only when you really need it.")]
        private bool _supportDynamicScale = false;

        private CollisionSetting _collisionSetting = CollisionSetting.Default;

        private Rigidbody3D _attachedRigidbody;

        private BodyNativeBridge _nativeBridge;

        private PhysicMaterial _instantiatedMaterial;

        private int _nativeMaterialVersion = 0;
        private int _gameObjectLayer;

        private Vector3 _appliedScale = Vector3.one;
        private ColliderId _colliderId = ColliderId.Invalid;

        /// <summary>
        /// OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider. This event is only raised when the collider's trigger option is off.
        /// </summary>
        public event System.Action<Collision> onCollisionEnter;
        /// <summary>
        /// OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider. This event is only raised when the collider's trigger option is off.
        /// </summary>
        public event System.Action<Collision> onCollisionStay;
        /// <summary>
        /// OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider. This event is only raised when the collider's trigger option is off.
        /// </summary>
        public event System.Action<Collision> onCollisionExit;
        /// <summary>
        /// OnTriggerEnter is called when the Collider other enters the trigger. This event is only raised when the collider's trigger option is on.
        /// </summary>
        public event System.Action<BaseCollider> onTriggerEnter;
        /// <summary>
        /// OnTriggerStay is called once per frame for every Collider other that is touching the trigger. This event is only raised when the collider's trigger option is on.
        /// </summary>
        public event System.Action<BaseCollider> onTriggerStay;
        /// <summary>
        /// OnTriggerExit is called when the Collider other has stopped touching the trigger. This event is only raised when the collider's trigger option is on.
        /// </summary>
        public event System.Action<BaseCollider> onTriggerExit;

        internal bool TryCreateShape(out Shape shape)
        {
            var scale = transform.lossyScale;
            _appliedScale = scale;
            return OnCreateShape(scale, out shape);
        }

        internal abstract bool OnCreateShape(Vector3 scale, out Shape shape);

        internal abstract bool CanCreateShape();

        internal bool TryExtractOptions(out ColliderOptions ret)
        {
            ret = new ColliderOptions();
            var isSuccess = TryCreateShape(out var shape);
            ret.material = actualMaterial ? actualMaterial.data : MaterialData.Default;
            ret.collisionMask = collisionMask;
            ret.shape = shape;
            ret.shapeToActorTransform = expectNativeShapeTransform;
            ret.isTrigger = isTrigger;
            ret.enableSimulation = true;
            ret.collisionSetting = _collisionSetting;
            ret.massOrDensity = ColliderOptions.MassOrDensity.Density;
            ret.massOrDensityValue = 1.0f;
            return isSuccess;
        }

        internal ChildColliderKey childIndex => _colliderId.childId;

        internal bool isNativeValid => childIndex.isValid;

        /// <value>
        /// Is scale changed since last time shape is created?
        /// </value>
        internal bool hasScaleChanged
        {
            get
            {
                var lossyScale = transform.lossyScale;
                if (Vector3.SqrMagnitude(lossyScale - _appliedScale) < 1e-4)
                {
                    return false;
                }

                return true;
            }
        }

        /// <value>
        /// Whether automatically update shape size when transform scale changed.
        ///
        /// Has little performance impact. Enable it only when you really need it.
        /// </value>
        public bool supportDynamicScale
        {
            get
            {
                return _supportDynamicScale;
            }
            set
            {
                _supportDynamicScale = value;
            }
        }

        /// <value>The collider shape's translation in gameObject's local space</value>
        public Vector3 shapeTranslation
        {
            get => _translation;
            set
            {
                _translation = value;

                if (isNativeValid)
                {
                    var pose = expectNativeShapeTransform;
                    nativeBridge.SetShapeTransform(childIndex, pose.position, pose.rotation);
                }
            }
        }

        /// <value>The collider shape's local rotation in gameObject's local space</value>
        public Quaternion shapeRotation
        {

            get => Quaternion.Euler(_rotation);
            set
            {
                _rotation = value.eulerAngles;

                if (isNativeValid)
                {
                    var pose = expectNativeShapeTransform;
                    nativeBridge.SetShapeTransform(childIndex, pose.position, pose.rotation);
                }
            }
        }

        /// <value>
        /// The matrix that transforms a point from collider shape's local space to world space.
        ///
        /// Node: In renderer,if you have a parent transform with scale and a child that is arbitrarily rotated, it's scale will be skewed.
        ///
        /// However in physics world, we should keep the shape rigid and it should not be skewed.
        /// </value>
        public Matrix4x4 shapeToWorldMatrix
        {
            get
            {
                var localToWorld = transform.localToWorldMatrix;
                var position = localToWorld.MultiplyPoint3x4(shapeTranslation);
                var rotation = transform.rotation * shapeRotation;
                var scale = transform.lossyScale;
                return Matrix4x4.TRS(position, rotation, scale);
            }
        }

        private Transform actorTransform
        {
            get
            {
                if (attachedRigidbody)
                {
                    return attachedRigidbody.transform;
                }
                else
                {
                    return transform;
                }
            }
        }

        internal Vector3 expectNativeShapeTranslation
        {
            get
            {
                // calculate shape's world position and convert it to actor's local space.
                var shapeWorldPosition = transform.TransformPoint(shapeTranslation);
                var actorToWorld = Matrix4x4.TRS(actorTransform.position, actorTransform.rotation, Vector3.one);
                return actorToWorld.inverse.MultiplyPoint3x4(shapeWorldPosition);
            }
        }

        internal Isometry expectNativeShapeTransform
        {
            get
            {
                // calculate shape's world position and convert it to actor's local space.
                var shapeWorldPosition = transform.TransformPoint(shapeTranslation);
                var shapeWorldRotation = transform.rotation * shapeRotation;
                var worldToActor = Matrix4x4.TRS(actorTransform.position, actorTransform.rotation, Vector3.one).inverse;

                return new Isometry(worldToActor.MultiplyPoint3x4(shapeWorldPosition), worldToActor.rotation * shapeWorldRotation);
            }
        }

        private BodyNativeBridge nativeBridge => _nativeBridge;

        internal void AttachToRigidBody3D(Rigidbody3D rigidbody3D)
        {
            Debug.Assert(enabled);
            Debug.Assert(rigidbody3D.isActiveAndEnabled);

            var hasNativeInitialized = isNativeValid;

            DetachSelf();

            if (hasNativeInitialized)
            {
                var success = TryCreateCollider(rigidbody3D);
                Debug.Assert(success);
            }
        }

        private void OnEnable()
        {
            TryCreateCollider();
        }

        private bool TryCreateCollider(Rigidbody3D rigidBody3d = null)
        {
            Debug.Assert(nativeBridge == null);

            var success = false;

            if (rigidBody3d == null)
            {
                rigidBody3d = FindRigidBodyOnSelfAndSuperior();
            }

            if (rigidBody3d)
            {
                Debug.Assert(rigidBody3d.isActiveAndEnabled);

                if (CanCreateShape())
                {
                    attachedRigidbody = rigidBody3d;
                    _nativeBridge = rigidBody3d.nativeBridge;
                    id = nativeBridge.AttachCollider(this);
                    rigidBody3d.AddColliderComponent(this);

                    success = true;
                }
            }
            else if (CanCreateShape())
            {
                _nativeBridge = new BodyNativeBridge(this);
                id = new ColliderId(handle.id, handle.FirstShapeKey());
                success = true;
            }

            if (success)
            {
                UpdateNativeMaterialVersion();
                Register(this);
            }

            return success;
        }

        private void OnDisable()
        {
            DetachSelf();
        }

        private void DetachNative()
        {
            if (isNativeValid)
            {
                Debug.Assert(nativeBridge != null);

                if (!nativeBridge.isEngineDisposed)
                {
                    if (nativeBridge.rigidbody == null)
                    {
                        nativeBridge.DestroyNativeImmendiate();
                    }
                    else
                    {
                        nativeBridge.DetachCollider(childIndex);
                    }
                }

                _nativeBridge = null;
                Unregister(id);
            }

            id = ColliderId.Invalid;
        }

        private void DetachSelf()
        {
            DetachNative();

            if (attachedRigidbody)
            {
                attachedRigidbody.RemoveColliderComponent(this);
                attachedRigidbody = null;
            }
        }

        private Rigidbody3D FindRigidBody(Transform transform)
        {
            var parentTransform = transform;

            while (parentTransform)
            {
                var rigidbody3D = parentTransform.GetComponentInParent<Rigidbody3D>();

                if (rigidbody3D != null && rigidbody3D.isActiveAndEnabled)
                {
                    return rigidbody3D;
                }

                if (rigidbody3D)
                {
                    parentTransform = rigidbody3D.transform.parent;
                }
                else
                {
                    parentTransform = null;
                }
            }

            return null;
        }

        private Rigidbody3D FindRigidBodyOnSuperior()
        {
            return FindRigidBody(transform.parent);
        }

        internal void OnRigidBody3DDestroy()
        {
            Debug.Assert(enabled);
            Debug.Assert(isNativeValid);

            // Before this, the collider corresponds to a collider in native (isNativeValid == true), so a new collider can be created.
            Debug.Assert(CanCreateShape());

            var newRigidbody3D = FindRigidBodyOnSuperior();

            Unregister(id);
            _colliderId = ColliderId.Invalid;

            if (newRigidbody3D)
            {
                attachedRigidbody = newRigidbody3D;
                _nativeBridge = newRigidbody3D.nativeBridge;
                id = nativeBridge.AttachCollider(this);
                newRigidbody3D.AddColliderComponent(this);
                Register(this);
            }
            else
            {
                attachedRigidbody = null;
                _nativeBridge = new BodyNativeBridge(this);
                id = new ColliderId(handle.id, handle.FirstShapeKey());
                Register(this);
            }
        }

        private Rigidbody3D FindRigidBodyOnSelfAndSuperior()
        {
            return FindRigidBody(transform);
        }

        private void OnTransformParentChanged()
        {
            // If it is disabled, it means the collider is not added to the physics world, so no processing is needed.
            if (!enabled)
            {
                return;
            }

            var oldRigidbody3d = attachedRigidbody;
            var newRigidbody3d = FindRigidBodyOnSelfAndSuperior();

            if (oldRigidbody3d && newRigidbody3d)
            {
                if (oldRigidbody3d != newRigidbody3d)
                {
                    AttachToRigidBody3D(newRigidbody3d);
                }
            }
            // Originally bound to a Rigidbody3D, so it was a dynamic rigidbody collider.
            // Now it is no longer bound to any Rigidbody3D, so it is a static rigidbody collider.
            else if (oldRigidbody3d)
            {
                DetachSelf();

                if (CanCreateShape())
                {
                    _nativeBridge = new BodyNativeBridge(this);
                    id = new ColliderId(handle.id, handle.FirstShapeKey());
                    UpdateNativeMaterialVersion();
                    Register(this);
                }
            }
            else if (newRigidbody3d)
            {
                AttachToRigidBody3D(newRigidbody3d);
            }
        }

        internal RigidbodyHandle handle
        {
            get
            {
                return nativeBridge.handle;
            }
        }

        internal RigidbodyId bodyId => id.bodyId;

        internal ColliderId id
        {
            get => _colliderId;
            set => _colliderId = value;
        }

        /// <value>
        /// The rigidbody this collider attached.
        /// </value>
        public Rigidbody3D attachedRigidbody
        {
            get => _attachedRigidbody;
            private set
            {
                _attachedRigidbody = value;
            }
        }

        internal void UpdateNativeShape()
        {
            if (Application.isPlaying)
            {
                if (isNativeValid)
                {
                    if (!nativeBridge.TryUpdateNativeShape(childIndex, this))
                    {
                        DetachNative();
                    }
                }
                else
                {
                    TryCreateCollider();
                }
            }
        }

        /// <value>
        /// shared material may be shared by multiple colliders. If we changed the property of the shared material, all related colliders will be affected.
        ///
        /// Use `material` property if you want to change for a single collider.
        /// </value>
        /// <seealso cref="material"/>
        public PhysicMaterial sharedMaterial
        {
            get
            {
                return _sharedMaterial;
            }
            set
            {
                if (_sharedMaterial == value)
                {
                    // nothing changed
                    return;
                }

                _sharedMaterial = value;

                if (_instantiatedMaterial == null)
                {
                    // we only need to update native material if we are not using the instantiated material
                    UpdateNativeMaterial();
                }
            }
        }

        /// <value>
        /// The physic material used by the collider. If material is shared by colliders, it will duplicate the material and assign it to the collider.
        /// </value>
        public PhysicMaterial material
        {
            get
            {
                if (_instantiatedMaterial == null)
                {
                    if (_sharedMaterial == null)
                    {
                        _instantiatedMaterial = ScriptableObject.CreateInstance<PhysicMaterial>();
                    }
                    else
                    {
                        _instantiatedMaterial = _sharedMaterial.Clone();
                    }
                }

                return _instantiatedMaterial;
            }
            set
            {
                if (_instantiatedMaterial == value)
                {
                    // nothing changed
                    return;
                }

                _instantiatedMaterial = value;
                UpdateNativeMaterial();
            }
        }

        internal PhysicMaterial actualMaterial
        {
            get
            {
                return _instantiatedMaterial ?? _sharedMaterial;
            }
        }

        internal MaterialData actualMaterialData => actualMaterial?.data ?? MaterialData.Default;

        internal MaterialData nativeMaterial
        {
            get
            {
                if (isNativeValid)
                {
                    return nativeBridge.GetMaterial(childIndex);
                }

                return default;
            }
        }

        /// <value>
        /// The collision mask of the collider.
        /// </value>
        public CollisionMask collisionMask
        {
            get
            {
                var gameobjectLayer = this.gameObject.layer;
                var layerMask = 1u << gameobjectLayer;
                var mask = PhysicsProjectSettings.Instance.GetCollisionMask(gameobjectLayer);
                return new CollisionMask()
                {
                    group = layerMask,
                    collideMask = mask
                };
            }
        }

        /// <value>
        /// The speculative margin of the collider. Used to control how far of the distance between two colliders will be considered a collision.
        /// </value>
        ///
        /// <exception cref="System.ArgumentException">separationOffset must be less than contactOffset</exception>
        public float contactOffset
        {
            get
            {
                return _collisionSetting.contactOffset;
            }
            set
            {
                _collisionSetting.contactOffset = value;

                if (isNativeValid)
                {
                    nativeBridge.SetCollisionSetting(childIndex, _collisionSetting);
                }
            }
        }

        /// <value>
        /// The actual separation bound offset of the collider.
        /// </value>
        ///
        /// <exception cref="System.ArgumentException">separationOffset must be less than contactOffset</exception>
        public float separationOffset
        {
            get
            {
                return _collisionSetting.separationOffset;
            }
            set
            {
                _collisionSetting.separationOffset = value;

                if (isNativeValid)
                {
                    nativeBridge.SetCollisionSetting(childIndex, _collisionSetting);
                }
            }
        }

        /// <value>
        /// The collision setting of the collider.
        /// </value> 
        internal CollisionSetting collisionSetting
        {
            get
            {
                return _collisionSetting;
            }
            set
            {
                _collisionSetting = value;

                if (isNativeValid)
                {
                    nativeBridge.SetCollisionSetting(childIndex, value);
                }
            }
        }

        /// <value>
        /// Whether the collider is a trigger.
        /// </value>
        public bool isTrigger
        {
            get
            {
                return _isTrigger;
            }
            set
            {
                _isTrigger = value;

                if (isNativeValid)
                {
                    nativeBridge.SetIsTrigger(childIndex, value);
                }
            }
        }

        internal bool isTriggerInNative
        {
            get
            {
                if (isNativeValid)
                {
                    return nativeBridge.IsTrigger(childIndex);
                }

                return false;
            }
        }

        /// <value>
        /// This property of all colliders of the same body should have the same value.
        /// </value>
        internal bool detectCollisions
        {
            get
            {
                if (isNativeValid)
                {
                    return nativeBridge.IsCollisionEnabled(childIndex);
                }

                return false;
            }
            set
            {
                if (isNativeValid)
                {
                    nativeBridge.SetCollisionEnabled(childIndex, value);
                }
            }
        }

        /// <value>
        /// The motion aabb of the collider.
        ///
        /// Currently only support at runtime.
        /// </value>
        internal Aabb3 motionAABB
        {
            get
            {
                if (isNativeValid)
                {
                    return nativeBridge.GetColliderAabb(childIndex);
                }

                return default;
            }
        }

        internal void UpdateNativeMaterial()
        {
            if (isNativeValid)
            {
                var material = actualMaterial;
                if (material == null)
                {
                    _nativeMaterialVersion = 0;
                    nativeBridge.SetMaterial(childIndex, MaterialData.Default);
                }
                else
                {
                    _nativeMaterialVersion = material.version;
                    nativeBridge.SetMaterial(childIndex, material.data);
                }
            }
        }

        private void UpdateNativeMaterialVersion()
        {
            var material = actualMaterial;
            if (material == null)
            {
                _nativeMaterialVersion = 0;
            }
            else
            {
                _nativeMaterialVersion = material.version;
            }
        }

        /// <summary>
        /// sync data from managed to native
        /// </summary>
        internal void UpdateNative()
        {
            Debug.Assert(nativeBridge != null);

            var actualMaterial = this.actualMaterial;
            if (actualMaterial != null && _nativeMaterialVersion != actualMaterial.version)
            {
                UpdateNativeMaterial();
            }
        }

        internal int nativeMaterialVersion
        {
            get
            {
                return _nativeMaterialVersion;
            }
        }

        internal void FireCollisionEnter(Collision collision)
        {
            onCollisionEnter?.Invoke(collision);
        }

        internal void FireCollisionStay(Collision collision)
        {
            onCollisionStay?.Invoke(collision);
        }

        internal void FireCollisionExit(Collision collision)
        {
            onCollisionExit?.Invoke(collision);
        }

        internal void FireTriggerEnter(BaseCollider other)
        {
            onTriggerEnter?.Invoke(other);
        }

        internal void FireTriggerStay(BaseCollider other)
        {
            onTriggerStay?.Invoke(other);
        }

        internal void FireTriggerExit(BaseCollider other)
        {
            onTriggerExit?.Invoke(other);
        }

        internal void PushColliderOptions()
        {
            if (!isNativeValid)
            {
                return;
            }

            UpdateNativeMaterial();

            nativeBridge.SetIsTrigger(childIndex, isTrigger);
            nativeBridge.SetCollisionMask(childIndex, collisionMask);
            nativeBridge.SetCollisionSetting(childIndex, collisionSetting);

            if (TryCreateShape(out var builder))
            {
                handle.UpdateShape(childIndex, builder);
            }

            handle.SetShapeTransform(childIndex, expectNativeShapeTranslation, shapeRotation);
        }

        private void Awake()
        {
            _gameObjectLayer = this.gameObject.layer;
            _collisionSetting = new CollisionSetting()
            {
                contactOffset = PhysicsManager.defaultContactOffset,
                separationOffset = PhysicsManager.defaultSeparationOffset,
            };
        }

        protected void Update()
        {
            var layer = this.gameObject.layer;
            if (layer != _gameObjectLayer && isNativeValid)
            {
                nativeBridge.WakeUp();
                nativeBridge.SetCollisionMask(childIndex, collisionMask);
                _gameObjectLayer = layer;
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// Used for drawing collider index in editor,
        /// to distinguish the colliders on a same gameobject.
        /// </summary>
        [HideInInspector]
        internal int _editorColliderIndex = -1;

        protected virtual void OnValidate()
        {
            if (Application.isPlaying)
            {
                PushColliderOptions();
            }
        }
#endif

        private static readonly Dictionary<ColliderId, BaseCollider> s_active_colliders =
            new Dictionary<ColliderId, BaseCollider>();

        private static readonly Dictionary<ColliderId, BaseCollider> s_unregisteringColliders = new Dictionary<ColliderId, BaseCollider>();

        private static void Register(BaseCollider collider)
        {
            var id = collider.id;
            Debug.Assert(id.childId != ChildColliderKey.Invalid);

            if (s_unregisteringColliders.Remove(id, out var c))
            {
                Debug.Assert(c == collider);
            }

            s_active_colliders.Add(id, collider);
        }

        private static void Unregister(ColliderId id)
        {
            if (s_active_colliders.Remove(id, out var collider))
            {
                s_unregisteringColliders.Add(id, collider);
            }
        }

        internal static BaseCollider Get(ColliderId id)
        {
            if (s_active_colliders.TryGetValue(id, out var collider))
            {
                return collider;
            }

            return s_unregisteringColliders[id];
        }

        internal static BaseCollider TryGet(ColliderId id)
        {
            if (s_active_colliders.TryGetValue(id, out var collider))
            {
                return collider;
            }

            if (s_unregisteringColliders.TryGetValue(id, out var collider1))
            {
                return collider1;
            }

            return null;
        }

        internal static void EachActiveCollider<T>(System.Action<BaseCollider, T> action, T context)
        {
            foreach (var kv in s_active_colliders)
            {
                action(kv.Value, context);
            }
        }

        internal static void UpdateAllNative()
        {
            ProfilerSamplers.s_update_native_colliders.Begin();
            foreach (var kv in s_active_colliders)
            {
                kv.Value.UpdateNative();
            }

            ProfilerSamplers.s_update_native_colliders.End();
        }

        internal static void ProcessUnregisters()
        {
            s_unregisteringColliders.Clear();
        }

        /// <summary>
        /// Should only be called when the engine is shutting down.
        /// </summary>
        internal static void Shutdown()
        {
            s_unregisteringColliders.Clear();
        }
    }
}
