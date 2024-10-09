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

using Motphys.Rigidbody.Internal;
using NUnit.Framework;

namespace Motphys.Rigidbody.RawApi.Tests
{

    internal class PhysicsNativeDataTests
    {

        [Test]
        public void TestSpringDamper()
        {
            var spring = new SpringDamper(1f, 2f);
            Assert.That(spring.compliance, Is.EqualTo(1f));
            Assert.That(spring.damper, Is.EqualTo(2f));

            // set compliance to negative value will throw an exception
            Assert.That(() =>
            {
                spring.compliance = -1f;

            }, Throws.ArgumentException);

            // set stiffness to negative value will throw an exception
            Assert.That(() =>
            {
                spring.stiffness = -1f;

            }, Throws.ArgumentException);

            // set compliance to valid value.
            spring.compliance = 3f;
            Assert.That(spring.compliance, Is.EqualTo(3f));

            // stiffness is the inverse of compliance
            Assert.That(spring.stiffness, Is.EqualTo(1f / 3f));

            // compliance is also the inverse of stiffness
            spring.stiffness = 3f;
            Assert.That(spring.compliance, Is.EqualTo(1f / 3f));

            // 1.0 / 0 = infinity
            spring.compliance = 0f;
            Assert.That(spring.stiffness, Is.EqualTo(float.PositiveInfinity));

            // set damper to negative value will throw an exception
            Assert.That(() =>
            {
                spring.damper = -1f;
            }, Throws.ArgumentException);

            // set damper to valid value.
            spring.damper = 4f;
            Assert.That(spring.damper, Is.EqualTo(4f));

            // new SpringDamper with invalid values will throw an exception
            Assert.That(() =>
            {
                new SpringDamper(-1f, 1f);
            }, Throws.ArgumentException);

            Assert.That(() =>
          {
              new SpringDamper(1f, -1f);
          }, Throws.ArgumentException);
        }

        [Test]
        public void TestActorOptions()
        {

            // negative mass is not allowed
            Assert.That(() =>
            {

                var actor = new ActorOptions(Motion.CreateDynamic(new DynamicOptions()
                {
                    mass = -1
                }), new ColliderOptions[] { });
            }, Throws.ArgumentException);

            // infinite mass is not allowed
            Assert.That(() =>
            {
                var actor = new ActorOptions(Motion.CreateDynamic(new DynamicOptions()
                {
                    mass = float.PositiveInfinity
                }), new ColliderOptions[] { });
            }, Throws.ArgumentException);

            // Nan is not allowed
            Assert.That(() =>
            {
                var actor = new ActorOptions(Motion.CreateDynamic(new DynamicOptions()
                {
                    mass = float.NaN
                }), new ColliderOptions[] { });

            }, Throws.ArgumentException);

            var actor = new ActorOptions(Motion.CreateDynamic(new DynamicOptions()
            {
                mass = 10.0f,
            }), new ColliderOptions[] { });

            var success = actor.motion.TryGetDynamicOptions(out var dynamic);
            Assert.That(success);
            Assert.That(dynamic.mass, Is.EqualTo(10.0f));

            // assert that the default values are set correctly
            {
                var options = new DynamicOptions(false);
                Assert.That(options.centerOfMass.type, Is.EqualTo(CenterOfMassType.Automatic));
                Assert.That(options.inertia.type, Is.EqualTo(InertiaType.Automatic));
            }
        }

        [Test]
        public void TestCenterOfMass()
        {
            var com = CenterOfMass.Automatic;
            Assert.That(com.type, Is.EqualTo(CenterOfMassType.Automatic));

            com = CenterOfMass.Custom(new UnityEngine.Vector3(1, 2, 3));
            Assert.That(com.type, Is.EqualTo(CenterOfMassType.Custom));
            Assert.That(com.position, Is.EqualTo(new UnityEngine.Vector3(1, 2, 3)));
        }

        [Test]
        public void TestInertia()
        {
            var inertia = Inertia.Automatic;
            Assert.That(inertia.type, Is.EqualTo(InertiaType.Automatic));

            inertia = Inertia.Custom(UnityEngine.Quaternion.AngleAxis(45, UnityEngine.Vector3.left), new UnityEngine.Vector3(1, 2, 3));
            Assert.That(inertia.type, Is.EqualTo(InertiaType.Custom));
            Assert.That(inertia.rotation, Is.EqualTo(UnityEngine.Quaternion.AngleAxis(45, UnityEngine.Vector3.left)));
            Assert.That(inertia.diagInertia, Is.EqualTo(new UnityEngine.Vector3(1, 2, 3)));
        }

        [Test]
        public void TestAxisFrame()
        {
            var axisFrame = new AxisFrame(new UnityEngine.Vector3(1, 0, 0), new UnityEngine.Vector3(0, 0, 90));
            Assert.That(axisFrame.axisX == UnityEngine.Vector3.up);
            Assert.That(axisFrame.axisY == UnityEngine.Vector3.left);

            var matrix = axisFrame.ToMatrix();
            Assert.That(matrix[0, 3], Is.EqualTo(1));

            var rotation = axisFrame.rotation;
            Assert.That(rotation != UnityEngine.Quaternion.identity);

            var identity = AxisFrame.Identity;
            Assert.That(identity.anchor == UnityEngine.Vector3.zero);
            Assert.That(identity.axisRotation == UnityEngine.Vector3.zero);
        }

        [Test]
        public void TestAngularMotorDrive()
        {
            var defaultMotor = AngularMotorDrive.Default;
            Assert.That(defaultMotor.spring.stiffness == 100);
            Assert.That(defaultMotor.spring.compliance == 0.01f);
            Assert.That(defaultMotor.spring.damper == 10);
            Assert.That(defaultMotor.maxTorque > 0);

            var spring = new SpringDamper(200, 20);
            var myMotor = new AngularMotorDrive(spring, 1000);
            Assert.That(myMotor.spring.stiffness == 200);
            Assert.That(myMotor.spring.compliance == 1.0f / 200.0f);
            Assert.That(myMotor.spring.damper == 20);
            Assert.That(myMotor.maxTorque == 1000);
        }

        [Test]
        public void TestD3Motion()
        {
            var motion = new D3Motion(JointDegreeMotion.Free, JointDegreeMotion.Locked, JointDegreeMotion.Limited);
            Assert.That(motion.x == JointDegreeMotion.Free);
            Assert.That(motion.y == JointDegreeMotion.Locked);
            Assert.That(motion.z == JointDegreeMotion.Limited);

            motion.x = JointDegreeMotion.Locked;
            motion.y = JointDegreeMotion.Limited;
            motion.z = JointDegreeMotion.Free;

            Assert.That(motion.x == JointDegreeMotion.Locked);
            Assert.That(motion.y == JointDegreeMotion.Limited);
            Assert.That(motion.z == JointDegreeMotion.Free);
        }

        [Test]
        public void TestAnchorFramePair()
        {
            var posA = new UnityEngine.Vector3(4, 2, 1);
            var posB = new UnityEngine.Vector3(-3, 4, 0);
            var pair = AnchorFramePair.FromAnchorPosition(posA, posB);
            Assert.That(pair.frameA.position == posA);
            Assert.That(pair.frameB.position == posB);
            Assert.That(pair.frameA.rotation == UnityEngine.Quaternion.identity);
            Assert.That(pair.frameB.rotation == UnityEngine.Quaternion.identity);

            var rotationA = UnityEngine.Quaternion.Euler(30, 0, 0);
            var rotationB = UnityEngine.Quaternion.Euler(0, 30, 0);
            var isoA = new Isometry(posB, rotationA);
            var isoB = new Isometry(posA, rotationB);

            var newPair = new AnchorFramePair(isoA, isoB);
            Assert.That(newPair.frameA.position == posB);
            Assert.That(newPair.frameB.position == posA);
            Assert.That(newPair.frameA.rotation == rotationA);
            Assert.That(newPair.frameB.rotation == rotationB);

            var identity = AnchorFramePair.Identity;
            Assert.That(identity.frameA.position == UnityEngine.Vector3.zero);
            Assert.That(identity.frameB.position == UnityEngine.Vector3.zero);
            Assert.That(identity.frameA.rotation == UnityEngine.Quaternion.identity);
            Assert.That(identity.frameB.rotation == UnityEngine.Quaternion.identity);

            var axisFrameA = new AxisFrame(new UnityEngine.Vector3(1, 0, 0), UnityEngine.Vector3.zero);
            var axisFrameB = new AxisFrame(new UnityEngine.Vector3(0, 0, 1), UnityEngine.Vector3.zero);

            var pair2 = AnchorFramePair.FromAxisFrame(axisFrameA, axisFrameB);
            Assert.That(pair2.frameA.position == UnityEngine.Vector3.right);
            Assert.That(pair2.frameB.position == UnityEngine.Vector3.forward);
            Assert.That(pair2.frameA.rotation == UnityEngine.Quaternion.identity);
            Assert.That(pair2.frameB.rotation == UnityEngine.Quaternion.identity);
        }

        [Test]
        public void TestBroadPhaseType()
        {
            var broadPhaseType = new BroadPhaseType(BroadPhaseAlgorithm.GridSAP);
            Assert.That(broadPhaseType.algorithm == BroadPhaseAlgorithm.GridSAP);
            Assert.That(!broadPhaseType.gridConfig.autoWorldAabb);
            Assert.That(broadPhaseType.gridConfig.numCellsInX == 4);
            Assert.That(broadPhaseType.gridConfig.numCellsInZ == 4);
            Assert.That(broadPhaseType.gridConfig.worldAabbMin == new UnityEngine.Vector3(-500, -500, -500));
            Assert.That(broadPhaseType.gridConfig.worldAabbMax == new UnityEngine.Vector3(500, 500, 500));

            var gridSAP = BroadPhaseType.GridSAP;
            Assert.That(gridSAP.algorithm == BroadPhaseAlgorithm.GridSAP);

            broadPhaseType.gridConfig.numCellsInX = 4;
            broadPhaseType.gridConfig.numCellsInZ = 4;

            Assert.That(broadPhaseType.gridConfig.numCellsInX == 4);
            Assert.That(broadPhaseType.gridConfig.numCellsInZ == 4);
        }

        [Test]
        public void TestShapes()
        {
            var capsule = new Capsule() { halfHeight = 2.0f, radius = 1.0f };
            var identity = Capsule.Identity;
            var capsuleShape = (Shape)capsule;

            Assert.That(identity.radius == 0.5f);
            Assert.That(identity.halfHeight == 0.5f);

            Assert.That(capsule.radius == 1.0f);
            Assert.That(capsule.halfHeight == 2.0f);

            Assert.That(capsuleShape.type == ShapeType.Capsule);
            Assert.That(capsuleShape.variants._capsule.radius == 1.0f);
            Assert.That(capsuleShape.variants._capsule.halfHeight == 2.0f);

            var cuboid = new Cuboid(new UnityEngine.Vector3(2, 3, 4));
            var cube = (Shape)cuboid;
            Assert.That(cube.type == ShapeType.Cube);
            var identityCube = Cuboid.Identity;
            Assert.That(identityCube.halfExt == UnityEngine.Vector3.one * 0.5f);

            var sphere = new Sphere() { radius = 1.2f };
            var sphereShape = (Shape)sphere;
            Assert.That(sphereShape.type == ShapeType.SolidSphere);
            var identitySphere = Sphere.Identity;
            Assert.That(identitySphere.radius == 1.0f);

            var cylinder = new Cylinder() { halfHeight = 2.3f, radius = 0.7f };
            var cylinderShape = (Shape)cylinder;
            Assert.That(cylinderShape.type == ShapeType.Cylinder);
            var identityCylinder = Cylinder.Identity;
            Assert.That(identityCylinder.radius == 0.5f);
            Assert.That(identityCylinder.halfHeight == 1.0f);

            var plane = new InfinitePlane();
            var planeShape = (Shape)plane;
            Assert.That(planeShape.type == ShapeType.InfinitePlane);
            var identityPlane = InfinitePlane.Identity;

            var shapeId = new ShapeId(357);
            var convexHullBuilder = new ConvexHullBuilder() { id = shapeId };
            var shape = (Shape)convexHullBuilder;
            Assert.That(shape.type == ShapeType.ConvexHull);
            Assert.That(shape.variants._convexHull.Equals(convexHullBuilder));

            var meshShapeId = new ShapeId(333);
            var meshBuilder = new MeshBuilder() { id = meshShapeId };
            var meshShape = (Shape)meshBuilder;
            Assert.That(meshShape.type == ShapeType.Mesh);
            Assert.That(meshShape.variants._mesh.Equals(meshBuilder));
        }

        [Test]
        public void TestChildColliderKey()
        {
            var id = (123L << 32) | 14482L;
            var colliderKey = new ChildColliderKey(id);
            var anotherKey = new ChildColliderKey(id);

            Assert.That(colliderKey.value == id);
            Assert.That(colliderKey.Equals(anotherKey));

            Assert.That(colliderKey.GetHashCode() != 0);
            Assert.That(colliderKey.isValid);

            var version = colliderKey.version;
            var index = colliderKey.index;
            Assert.That(index == 14482);
            Assert.That(version == 123);

            var name = colliderKey.ToString();
            Assert.That(colliderKey == anotherKey);
            Assert.That(!(colliderKey != anotherKey));

            var invalid = ChildColliderKey.Invalid;
            Assert.That(!invalid.isValid);
        }

        [Test]
        public void TestContact()
        {
            var contact = new Contact()
            {
                normal = UnityEngine.Vector3.up,
                localOffsetA = UnityEngine.Vector3.zero,
                localOffsetB = UnityEngine.Vector3.one,
                worldPositionA = new UnityEngine.Vector3(2, 0.2f, 3),
                worldPositionB = new UnityEngine.Vector3(3, 0.1f, 0.8f)
            };

            var fliped = contact.Flip();
            Assert.That(fliped.normal == UnityEngine.Vector3.down);
            Assert.That(fliped.localOffsetA == UnityEngine.Vector3.one);
            Assert.That(fliped.localOffsetB == UnityEngine.Vector3.zero);
            Assert.That(fliped.worldPositionA == new UnityEngine.Vector3(3, 0.1f, 0.8f));
            Assert.That(fliped.worldPositionB == new UnityEngine.Vector3(2, 0.2f, 3));
        }

        [Test]
        public void TestJoints()
        {
            var fixedJoint = new FixedJointConfig();
            var config = (TypedJointConfig)fixedJoint;
            Assert.That(config.type == JointType.Fixed);
            Assert.That(config._fix.Equals(fixedJoint));

            var distanceJoint = new DistanceJointConfig(new MinMax(2, 6));
            config = (TypedJointConfig)distanceJoint;
            Assert.That(config.type == JointType.Distance);
            Assert.That(config._distance.Equals(distanceJoint));

            var hingeJoint = new HingeJointConfig()
            {
                angleLimitLower = 10,
                angleLimitHigher = 30,
                limitSpring = SpringDamper.Default,
                angularDamper = 0.2f
            };
            config = (TypedJointConfig)hingeJoint;
            Assert.That(config.type == JointType.Hinge);
            Assert.That(config._hinge.Equals(hingeJoint));

            var ballJoint = new BallJointConfig()
            {
                twistLimit = new MinMax(-20, 20),
                swingLimit = new MinMax(-10, 10),
                angularDamper = 1.4f,
                limitSpring = SpringDamper.Default
            };
            config = (TypedJointConfig)ballJoint;
            Assert.That(config.type == JointType.Ball);
            Assert.That(config._ball.Equals(ballJoint));

            var ellipsoidJoint = new EllipsoidJointConfig()
            {
                twistLimit = new MinMax(-3, 7),
                swingLimitY = new MinMax(10, 30),
                swingLimitZ = new MinMax(5.8f, 7.7f),
                limitSpring = SpringDamper.Default,
                angularDamper = 2.2f
            };
            config = (TypedJointConfig)ellipsoidJoint;
            Assert.That(config.type == JointType.Ellipsoid);
            Assert.That(config._ellipsoid.Equals(ellipsoidJoint));

            var sliderJoint = new SliderJointConfig(new MinMax(-14, 56));
            config = (TypedJointConfig)sliderJoint;
            Assert.That(config.type == JointType.Slider);
            Assert.That(config._slider.Equals(sliderJoint));

            var universalJoint = new UniversalJointConfig()
            {
                swingAxisLimitZLow = 10,
                swingAxisLimitZHigh = 20,
                swingAxisLimitYLow = 30,
                swingAxisLimitYHigh = 40,
                angularDamper = 2
            };
            config = (TypedJointConfig)universalJoint;
            Assert.That(config.type == JointType.Universal);
            Assert.That(config._universal.Equals(universalJoint));

            var d6joint = new D6JointConfig()
            {
                swingLimitY = new MinMax(2, 10),
                swingLimitZ = new MinMax(1, 20),
            };
            config = (TypedJointConfig)d6joint;
            Assert.That(config.type == JointType.D6);
            Assert.That(config._d6.Equals(d6joint));
        }

        [Test]
        public void TestCommon()
        {
            var minmax = new MinMax(-123, 40);
            Assert.That(minmax.min == -123);
            Assert.That(minmax.max == 40);

            minmax.min = 60;
            Assert.That(minmax.min == 40);
            minmax.max = 20;
            Assert.That(minmax.max == 40);

            var tripleUnit = new TripleUint(1, 2, 3);
            Assert.That(tripleUnit._a == 1);
            Assert.That(tripleUnit._b == 2);
            Assert.That(tripleUnit._c == 3);
        }

        [Test]
        public void TestShapeId()
        {
            var id = 534L << 32 | 88746L;

            var shapeId = new ShapeId(id);
            var anotherId = new ShapeId(id);
            Assert.That(!shapeId.Equals(null));
            Assert.That(shapeId.Equals(anotherId));

            var hashCode = shapeId.GetHashCode();
            Assert.That(hashCode != 0);
            Assert.That(shapeId.isValid);
            Assert.That(shapeId.version == 534);
            Assert.That(shapeId.index == 88746);
            Assert.That(shapeId == anotherId);
            var invalid = ShapeId.Invalid;

            Assert.That(invalid != shapeId);
        }

        [Test]
        public void TestJointId()
        {
            var id = 7364L << 32 | 1494681L;
            var jointId = new JointId(id);
            var anotherId = new JointId(id);
            var invalidId = JointId.Invalid;
            Assert.That(!jointId.Equals(null));
            Assert.That(jointId.Equals(anotherId));
            Assert.That(jointId.GetHashCode() != 0);
            Assert.That(jointId.isValid);
            Assert.That(jointId.version == 7364);
            Assert.That(jointId.index == 1494681);
            Assert.That(jointId == anotherId);
            Assert.That(jointId != invalidId);
        }

        [Test]
        public void TestGrid2DConfig()
        {
            var config = Grid2DConfig.Default;
            Assert.That(!config.autoWorldAabb);
            Assert.That(config.worldAabbMin == new UnityEngine.Vector3(-500, -500, -500));
            Assert.That(config.worldAabbMax == new UnityEngine.Vector3(500, 500, 500));
            Assert.That(config.numCellsInX == 4);
            Assert.That(config.numCellsInZ == 4);
        }

        [Test]
        public void TestFreeze()
        {
            var freeze = new Freeze();
            freeze.freezePositionX = true;
            Assert.That(freeze.freezePositionX);
            freeze.freezePositionY = true;
            Assert.That(freeze.freezePositionY);
            freeze.freezePositionZ = true;
            Assert.That(freeze.freezePositionZ);
            freeze.freezeRotationX = true;
            Assert.That(freeze.freezeRotationX);
            freeze.freezeRotationY = true;
            Assert.That(freeze.freezeRotationY);
            freeze.freezeRotationZ = true;
            Assert.That(freeze.freezeRotationZ);
        }

#if MOTPHYS_RIGIDBODY_EXPERIMENTAL
        [Test]
        public void TestStableOptions()
        {
            var stableOptions = new StableOptions(true, false, true, false, 0.43f);
            var defaultEnableOptions = StableOptions.DefaultEnable;
            var defaultDisableOptions = StableOptions.DefaultDisable;
            Assert.That(stableOptions.disableSlightChange);
            Assert.That(!stableOptions.allowAveragedAnchor);
            Assert.That(stableOptions.allowContactGraph);
            Assert.That(!stableOptions.allowShockPropagation);
            Assert.That(!stableOptions.useJacobiIterationLocally);
            Assert.That(stableOptions.shockPropagationScale == 0.43f);

            Assert.That(!defaultEnableOptions.useJacobiIterationLocally);
            Assert.That(defaultEnableOptions.disableSlightChange);
            Assert.That(!defaultEnableOptions.allowAveragedAnchor);
            Assert.That(defaultEnableOptions.allowContactGraph);
            Assert.That(defaultEnableOptions.allowShockPropagation);
            Assert.That(defaultEnableOptions.shockPropagationScale == 10.0f);

            Assert.That(!defaultDisableOptions.useJacobiIterationLocally);
            Assert.That(!defaultDisableOptions.disableSlightChange);
            Assert.That(!defaultDisableOptions.allowAveragedAnchor);
            Assert.That(!defaultDisableOptions.allowContactGraph);
            Assert.That(!defaultDisableOptions.allowShockPropagation);
            Assert.That(defaultDisableOptions.shockPropagationScale == 1.0f);
        }
#endif

        [Test]
        public void TestCollisionSetting()
        {
            var collisionSetting = CollisionSetting.Default;
            Assert.That(collisionSetting.contactOffset == 0.005f);
            Assert.That(collisionSetting.separationOffset == 0.0f);

            collisionSetting.contactOffset = 0.01f;
            Assert.That(collisionSetting.contactOffset == 0.01f);

            collisionSetting.separationOffset = 0.005f;
            Assert.That(collisionSetting.separationOffset == 0.005f);
        }
    }
}
