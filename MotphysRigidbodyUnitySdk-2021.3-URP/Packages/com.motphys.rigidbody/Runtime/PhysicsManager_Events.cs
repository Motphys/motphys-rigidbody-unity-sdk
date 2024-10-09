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

namespace Motphys.Rigidbody
{
    public partial class PhysicsManager
    {

        /// <summary>
        /// The global "OnCollisionEnter" event
        /// <see cref="BaseCollider.onCollisionEnter"/>
        /// </summary>
        public static event System.Action<CollisionEvent> onCollisionEnter;
        /// <summary>
        /// The global "OnCollisionStay" event
        /// <see cref="BaseCollider.onCollisionStay"/>
        /// </summary>
        public static event System.Action<CollisionEvent> onCollisionStay;
        /// <summary>
        /// The global "OnCollisionExit" event
        /// <see cref="BaseCollider.onCollisionExit"/>
        /// </summary>
        public static event System.Action<CollisionEvent> onCollisionExit;
        /// <summary>
        /// The global "OnTriggerEnter" event
        /// <see cref="BaseCollider.onTriggerEnter"/>
        /// </summary>
        public static event System.Action<TriggerEvent> onTriggerEnter;
        /// <summary>
        /// The global "OnTriggerStay" event
        /// <see cref="BaseCollider.onTriggerStay"/>
        /// </summary>
        public static event System.Action<TriggerEvent> onTriggerStay;
        /// <summary>
        /// The global "OnTriggerExit" event
        /// <see cref="BaseCollider.onTriggerExit"/>
        /// </summary>
        public static event System.Action<TriggerEvent> onTriggerExit;
        /// <summary>
        /// The global "rigidbody wake up" event.
        /// </summary>
        public static event System.Action<Rigidbody3D> onWakeUp;
        /// <summary>
        /// The global "rigidbody sleep down" event
        /// </summary>
        public static event System.Action<Rigidbody3D> onSleepDown;
        /// <summary>
        /// The event before each engine update.
        /// </summary>
        public static event System.Action onPreUpdate;
        /// <summary>
        /// The event after each engine update.
        /// </summary>
        public static event System.Action onPostUpdate;

        /// <summary>
        /// Ignore all collisions between any collider in layerA and any collider in layerB.
        /// It does not affect the generated colliders temporarily.
        /// </summary>
        /// <param name="layerA">Valid layer is in [0, 31]</param>
        /// <param name="layerB">Valid layer is in [0, 31]</param>
        /// <param name="ignore"></param>
        /// <returns>Return false if the input layer is invalid </returns>
        public static bool IgnoreLayerCollision(int layerA, int layerB, bool ignore = true)
        {
            return PhysicsProjectSettings.Instance.IgnoreLayerCollision(layerA, layerB, ignore);
        }

        private static void FireCollisionEnter(CollisionEvent evt)
        {
            var c1 = evt.collider1;
            var c2 = evt.collider2;

            c1.FireCollisionEnter(new Collision(c2, new Internal.ColliderPair(evt.id1, evt.id2)));
            c2.FireCollisionEnter(new Collision(c1, new Internal.ColliderPair(evt.id1, evt.id2)));

            onCollisionEnter?.Invoke(evt);
        }

        private static void FireCollisionStay(CollisionEvent evt)
        {
            var c1 = evt.collider1;
            var c2 = evt.collider2;

            c1.FireCollisionStay(new Collision(c2, new Internal.ColliderPair(evt.id1, evt.id2)));
            c2.FireCollisionStay(new Collision(c1, new Internal.ColliderPair(evt.id1, evt.id2)));

            onCollisionStay?.Invoke(evt);
        }

        private static void FireCollisionExit(CollisionEvent evt)
        {
            var c1 = evt.collider1;
            var c2 = evt.collider2;

            c1.FireCollisionExit(new Collision(c2, new Internal.ColliderPair(evt.id1, evt.id2)));
            c2.FireCollisionExit(new Collision(c1, new Internal.ColliderPair(evt.id1, evt.id2)));

            onCollisionExit?.Invoke(evt);
        }

        private static void FireTriggerEnter(TriggerEvent evt)
        {
            var c1 = evt.collider1;
            var c2 = evt.collider2;

            c1.FireTriggerEnter(c2);
            c2.FireTriggerEnter(c1);

            onTriggerEnter?.Invoke(evt);
        }

        private static void FireTriggerStay(TriggerEvent evt)
        {
            var c1 = evt.collider1;
            var c2 = evt.collider2;

            c1.FireTriggerStay(c2);
            c2.FireTriggerStay(c1);

            onTriggerStay?.Invoke(evt);
        }

        private static void FireTriggerExit(TriggerEvent evt)
        {
            var c1 = evt.collider1;
            var c2 = evt.collider2;

            c1.FireTriggerExit(c2);
            c2.FireTriggerExit(c1);

            onTriggerExit?.Invoke(evt);
        }
        private static void FireWakeUp(Rigidbody3D body)
        {
            onWakeUp?.Invoke(body);
        }

        private static void FireSleepDown(Rigidbody3D body)
        {
            onSleepDown?.Invoke(body);
        }

        internal static void FirePreUpdate()
        {
            onPreUpdate.Invoke();
        }

        internal static void FirePostUpdate()
        {
            onPostUpdate.Invoke();
        }

        private static void DispatchEvents()
        {
            Internal.ProfilerSamplers.s_dispatchEvents.Begin();
            var events = defaultWorld.QueryWorldEvents();
            foreach (var jointId in events.brokenJoints)
            {
                BaseJoint.DestroyJointWithoutNotifyNative(jointId);
            }

            foreach (var pair in events.triggerEnters)
            {
                var c1 = BaseCollider.Get(pair.Item1);
                var c2 = BaseCollider.Get(pair.Item2);
                var evt = new TriggerEvent(c1, c2);
                PhysicsManager.FireTriggerEnter(evt);
            }

            foreach (var pair in events.triggerExits)
            {
                var c1 = BaseCollider.TryGet(pair.Item1);
                var c2 = BaseCollider.TryGet(pair.Item2);
                if (c1 && c2)
                {
                    var evt = new TriggerEvent(c1, c2);
                    PhysicsManager.FireTriggerExit(evt);
                }
                else
                {
                    // if we failed to get any collider, it means the exits event may caused by the destruction of the collider.
                }
            }

            foreach (var pair in events.triggerStays)
            {
                var c1 = BaseCollider.Get(pair.Item1);
                var c2 = BaseCollider.Get(pair.Item2);
                var evt = new TriggerEvent(c1, c2);
                PhysicsManager.FireTriggerStay(evt);
            }

            foreach (var pair in events.collidingEnters)
            {
                var c1 = BaseCollider.Get(pair.Item1);
                var c2 = BaseCollider.Get(pair.Item2);
                var evt = new CollisionEvent(pair.Item1, pair.Item2, c1, c2);
                PhysicsManager.FireCollisionEnter(evt);
            }

            foreach (var pair in events.collidingExits)
            {
                var c1 = BaseCollider.TryGet(pair.Item1);
                var c2 = BaseCollider.TryGet(pair.Item2);
                if (c1 && c2)
                {
                    var evt = new CollisionEvent(pair.Item1, pair.Item2, c1, c2);
                    PhysicsManager.FireCollisionExit(evt);
                }
                else
                {
                    // if we failed to get any collider, it means the exits event may caused by the destruction of the collider.
                }
            }

            foreach (var pair in events.collidingStays)
            {
                var c1 = BaseCollider.Get(pair.Item1);
                var c2 = BaseCollider.Get(pair.Item2);
                var evt = new CollisionEvent(pair.Item1, pair.Item2, c1, c2);
                PhysicsManager.FireCollisionStay(evt);
            }

            foreach (var id in events.wakeUpBodies)
            {
                var body = Rigidbody3D.Get(id);
                body.OnWakeUp();
                PhysicsManager.FireWakeUp(body);
            }

            foreach (var id in events.sleepDownBodies)
            {
                var body = Rigidbody3D.Get(id);
                body.OnSleepDown();
                PhysicsManager.FireSleepDown(body);
            }

            Internal.ProfilerSamplers.s_dispatchEvents.End();
        }
    }
}
