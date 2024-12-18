# Changelog

# v2.0.0

## General

### Added

- Support for Motphys physics debugger to visualize the physics world.
- Support integrated profiling to monitor, analyze and optimize application performance.

## Rigidbody

### Added

- Rigidbody Collision: Detects when and where objects with colliders intersect and calculates appropriate physical response.

  - Collider Shapes:
    - Infinite plane
    - Sphere
    - Capsule
    - Box
    - Cylinder
    - Convex hull
  - Multiple Colliders: Allow you to combine several colliders to create more complex shapes.
  - Broad Phase:
    - Grid SAP
  - Collision Filter: Manage which objects can collide.
  - Callback:
    - Collision event
    - Trigger event
- Rigidbody Dynamics: Takes into account factors like mass, velocity, and forces to predict rigidbody movement.
- Joints: Simulate constraints between bodies.

  - Fixed Joint
  - Distance Joint
  - Slider Joint
  - Hinge Joint
  - Ball Joint
  - Ellipsoid Joint
  - Universal Joint
  - Motors: Drive the constraints.
  - Spring: Soft constraint.
  - Breakage: Breaking the connection.

## Scene Query

### Added

- Raycast: Casts a ray from a point in a specified direction to see if it intersects with any colliders in the scene.

  - Query Mode:
    - Multiple Hit
    - Any Hit
    - Closest Hit
- Overlap Test: Checks for any colliders that overlap with a given shape.
- Layer Mask Filter: Used in queries to include or exclude objects based on their layer.

## Supported Unity Versions

- Unity 2021.3
- Unity 2022.3
- Unity 2023.2
- Unity 6
- Tuanjie Engine 1.2

## Supported Platforms

- Windows x64
- macOS Arm64 (Apple silicon)
- macOS x64 (Intel)

## **Known Issues And Limitations**

- By default, the icons for physics components are rendered in the scene, and need to be manually disabled.
- The editor-related logic in Unity 6 has undergone changes, so support for this version remains unstable.

# v2.0.0

## 通用

### 新增

- 支持 Motphys Physics Debugger，用于可视化物理世界。
- 支持集成性能分析功能，能够监控、分析并优化应用程序性能。

## 刚体

### 新增

- 刚体碰撞：检测带有碰撞体的物体何时、何地相交，并计算相应的物理作用。

  - 碰撞体形状：
    - 无限平面
    - 球体
    - 胶囊体
    - 盒子
    - 圆柱体
    - 凸包
  - 多重碰撞体：允许你组合多个碰撞体以创建更复杂的形状。
  - 宽阶段碰撞检测：
    - Grid SAP
  - 碰撞过滤器：管理哪些物体可以发生碰撞。
  - 回调：
    - 碰撞事件
    - 触发事件 (Trigger)
- 刚体动力学：考虑质量、速度和力等因素来预测刚体运动。
- 关节：模拟物体之间的约束。

  - 固定关节
  - 距离关节
  - 滑动关节
  - 合页关节
  - 球关节
  - 椭球关节
  - 万向关节
  - 马达：驱动这些约束。
  - 弹簧：软约束。
  - 断裂：断开连接。

## 场景查询

### 新增

- 射线检测：从一点向指定方向投射一条射线，检查是否与场景中的任何碰撞体相交。

  - 查询模式：
    - 多次命中
    - 任意命中
    - 距离最近命中
- 重叠测试：检查给定形状是否与任何碰撞体重叠。
- 图层掩码过滤器：在场景查询中使用，用于根据图层包含或排除物体。

## 支持的 Unity 版本

- Unity 2021.3
- Unity 2022.3
- Unity 2023.2
- Unity 6
- 团结引擎 1.2

## 支持的平台

- Windows x64
- macOS Arm64（Apple Silicon）
- macOS x64（Intel）

## 已知问题和限制

- 物理组件的图标在场景视图中会默认渲染，需要手动禁用。
- Unity 6 中的编辑器相关逻辑发生了变化，因此对该版本的支持尚不稳定。

# 2.0.0-beta.9

## **Rigid Bodies**

- Support for configuring the iteration count of joints individually.
- Support for scaling the inertia of bodies on a joint during simulation using a scale configuration.
- Support for creating a ConvexHull from meshes with a thickness of 0.
- When generating the convex hull, meshes like spheres and capsules no longer fail and are instead substituted with simpler shapes.

## **Performance Optimization**

- Optimized collision detection performance on mobile devices.

# 2.0.0-beta.9

## 刚体

- 支持单独配置 Joint 的迭代次数
- 支持通过配置 scale 在解算时缩放 Joint 上 body 的 interia
- 支持基于厚度为 0 的网格来创建 ConvexHull
- 创建凸包时，球体、胶囊体等网格不再发生失败，而是用简单的形状代替

## 性能优化

- 优化了移动端上碰撞检测的性能

# 2.0.0-beta.10

## **Rigid Bodies**

- The convex hull is constructed using a subset of vertices when the maximum number of faces is reached.

# 2.0.0-beta.10

## 刚体

- 当达到最大面数限制时，将使用部分顶点构建凸包。