# Motphys Physics Debugger 用户手册

Motphys Physics Debugger 是 Motphys 提供的针对场景中物体的可视化工具。

## 支持的 Unity/团结引擎版本

-   Unity 2021.3
-   Unity 2022.3
-   Unity 2023.2
-   Unity 6
-   团结引擎 1.2

## 支持的操作系统

目前公开版的 Motphys Rigidbody Unity SDK 的编辑器及运行时支持以下操作系统：

-   Windows x64
-   macOS Arm64 (Apple silicon)
-   macOS x64 (Intel)

非公开版本额外支持以下平台：

-   Android
-   iOS
-   Linux x64
-   OpenHarmony
-   WebAssembly
-   微信小游戏

如需上述平台支持，或需要在未列出的其它平台上运行 SDK，请联系 bd@motphys.com。

## 安装

1. 在 Unity/团结引擎编辑器中，打开 `Edit/Project Setting/PackageManager`，按以下参数添加一个 `Scoped Registry`：

```text
Name: package.openupm.com
URL: https://package.openupm.com
Scope(s): com.motphys
```

1. 点 `Save` 或者 `Apply`，保存设置
2. 打开 `Window/Package Manager`
3. 点左上角的 `+`
4. 选择 `Add package by name...`
5. `name` 填 `com.motphys.debugdraw.editor`
6. `version` 填 `2.0.0-beta.9`

# 开始使用

安装 完成后，选择 `Windows/Analysis/Motphys Physics Debugger` 可打开工具窗口。

![](https://docs.motphys.com/Images/ICykbPG2qox1Y7xzW1gcbTk5nyf.png)

![](https://docs.motphys.com/Images/YKfCbBU0toUnkAx5Wt5ch46rnEf.png)

打开工具窗口后，在场景视图的右下角，会有 `Physics Debug Window` 面板，作为刚体可视化渲染的开关。

（注：此工具的绘制与 Unity Scene View Gizmos 有相同的可见性）

| 碰撞几何（Collision Geometry）<br/>    | 是否显示场景中碰撞体的几何形状。<br/>                               |
| -------------------------------------- | ------------------------------------------------------------------- |
| 碰撞信息（Collision Information）<br/> | 是否显示场景中的碰撞对、接触点、接触法线等（只在运行时生效）。<br/> |

## 碰撞几何面板

![](https://docs.motphys.com/Images/OHtFbMfuYocjfQxpgpvc7RSan6c.png)

| 名称<br/>                                        | 释义<br/>                                                                         |
| ------------------------------------------------ | --------------------------------------------------------------------------------- |
| 绘制类型（DrawType）<br/>                        | 决定碰撞几何以哪种方式绘制。<br/>                                                 |
| - 全部（All）<br/>                               | 同时绘制线框与网格。<br/>                                                         |
| - 线框（Wireframe）<br/>                         | 只绘制线框。<br/>                                                                 |
| - 网格（Mesh）<br/>                              | 只绘制网格。<br/>                                                                 |
| 选择对象（Selected Object Info）<br/>            | 场景中选择的对象信息。<br/>                                                       |
| 渲染层级（Show Layers）<br/>                     | 目标渲染层级，默认 `EveryThing`，可配置层级用于只显示某一层级的刚体对象。<br/>    |
| 显示静态刚体（Show Static Colliders）<br/>       | 勾选时，显示场景中的静态刚体。<br/>                                               |
| 显示触发器（Show Triggers）<br/>                 | 勾选时，显示场景中的触发器。<br/>                                                 |
| 显示动态刚体（Show RigidBodies）<br/>            | 勾选时，显示场景中的动态刚体。<br/>                                               |
| 显示运动学刚体（Show Kinematic Bodies）<br/>     | 勾选时，显示场景中的运动学刚体。<br/>                                             |
| 显示睡眠刚体（Show Sleeping Bodies）<br/>        | 勾选时，显示场景中的睡眠刚体（运行时生效）。<br/>                                 |
| 碰撞几何类型（Collider Types）<br/>              | <br/>                                                                             |
| - 显示方盒碰撞体（Show Box Colliders）<br/>      | 勾选时，显示场景中的方盒碰撞体。<br/>                                             |
| - 显示球碰撞体（Show Sphere Colliders）<br/>     | 勾选时，显示场景中的球碰撞体。<br/>                                               |
| - 显示胶囊碰撞体（Show Capsule Colliders）<br/>  | 勾选时，显示场景中的胶囊碰撞体。<br/>                                             |
| - 显示圆柱碰撞体（Show Cylinder Colliders）<br/> | 勾选时，显示场景中的圆柱碰撞体。<br/>                                             |
| - 显示网格碰撞体（Show Mesh Colliders）<br/>     | 勾选时，显示场景中的网格碰撞体。<br/>                                             |
| - 不显示/显示全体（Show None/Show All）<br/>     | 点击“不显示”，取消所有勾选；点击“显示全体”，勾选所有。<br/>                       |
| 渲染颜色（Colors）<br/>                          | 配置不同类型的渲染颜色。<br/>                                                     |
| - 触发器颜色（Trigger Color）<br/>               | 渲染“触发器”所使用的颜色。<br/>                                                   |
| - 静态刚体颜色（Trigger Color）<br/>             | 渲染“静态刚体”所使用的颜色。<br/>                                                 |
| - 动态刚体颜色（RigidBody Color）<br/>           | 渲染“动态刚体”所使用的颜色。<br/>                                                 |
| - 睡眠颜色（Sleep Color）<br/>                   | 渲染“睡眠状态的动态刚体”所使用的颜色。<br/>                                       |
| - 运动学颜色（Kinematic Color）<br/>             | 渲染“运动学刚体”所使用的颜色。<br/>                                               |
| - 差异度（Variation）<br/>                       | 值越大，绘制的颜色更偏离原本的指定的颜色，对同种类型的几何形状产生一定色差。<br/> |
| 渲染配置（Rendering）<br/>                       | <br/>                                                                             |
| - 透明度（Transparency）<br/>                    | 渲染网格时的透明度。<br/>                                                         |
| - 可视距离（View Distance）<br/>                 | 刚体与场景摄像机距离在可视距离内才进行渲染。<br/>                                 |

## 碰撞信息面板

![](https://docs.motphys.com/Images/Y0egbd9a8ovIAbxwTrxcZBWQnJc.png)

| 名称<br/>                                 | 释义<br/>                                                                                                                             |
| ----------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------- |
| 显示碰撞对（Show Collision Pair）<br/>    | 运行时，在场景中以连线形式绘制碰撞对。<br/>                                                                                           |
| 显示约束对（Show Joint Pair）<br/>        | 运行时，在场景中以连线形式绘制约束对。<br/>                                                                                           |
| 显示包围盒（Show Aabb）<br/>              | 运行时，在场景中绘制包围盒。<br/>                                                                                                     |
| 显示接触信息（Show Contacts）<br/>        | 运行时，在场景中绘制接触点与接触法线。<br/>当刚体材质或全局设置中的“Enable Collision/Contact Event”为 false 时，将不会产生数据。<br/> |
| 渲染颜色（Colors）<br/>                   | 配置不同类型的渲染颜色。<br/>                                                                                                         |
| - 包围盒颜色（Aabb Color）<br/>           | 渲染“包围盒”所用的颜色。<br/>                                                                                                         |
| - 碰撞对颜色（Collision Pair Color）<br/> | 渲染“碰撞对”所用的颜色。<br/>                                                                                                         |
| - 接触点颜色（Contact Point Color）<br/>  | 渲染“接触点”所用的颜色。<br/>                                                                                                         |
| - 接触法线颜色（Contact Line Color）<br/> | 渲染“接触法线”所用的颜色。<br/>                                                                                                       |
| - 约束对颜色（Joint Pair Color）<br/>     | 渲染“约束对”所用的颜色。<br/>                                                                                                         |
