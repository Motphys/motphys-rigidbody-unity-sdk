# Motphys Rigidbody Unity SDK 用户手册

# 简介

Motphys Physics 是一个使用 Rust 语言编写的高性能、高精度且稳定的实时物理引擎，能够处理复杂场景中的大量物理交互，并在大规模实时模拟中提供高质量的结果。Motphys Rigidbody Unity SDK 是该引擎的刚体物理库在 Unity 中的集成解决方案，使开发者能够轻松在 Unity 项目中利用 Motphys Physics 的强大功能，实现逼真的刚体物理效果。

除此之外，Motphys 还在研发流体和布料仿真，以便逼真模拟液体流动及织物的动态行为。如需上述功能支持，或需要其他未列出的特性，请联系 bd@motphys.com。

您也可以访问 Motphys 的[官网](https://motphys.com/)来了解其他信息。

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

```
Name: package.openupm.com
URL: https://package.openupm.com
Scope(s): com.motphys
```

1. 点 `Save` 或者 `Apply`，保存设置
2. 打开 `Window/Package Manager`
3. 点左上角的 `+`
4. 选择 `Add package by name...`
5. `name` 填 `com.motphys.rigidbody`
6. `version` 填 `2.0.0-beta.6`

除此之外，也可以访问我们的 [GitHub 仓库](https://github.com/Motphys/motphys-rigidbody-unity-sdk)来下载 SDK 和了解其他信息。

## 示例场景

Motphys 提供了一系列典型的示例场景，用于展示程序的核心功能和操作流程。这些场景涵盖了常见的使用情况，通过这些场景，用户可以快速上手并进行实际操作。我们所有的示例场景及相关代码都放在 GitHub 仓库中，您可以访问我们的 [GitHub 仓库](https://github.com/Motphys/motphys-rigidbody-unity-sdk/tree/main/MotphysRigidbodyUnitySdk-2021.3-URP/Packages/com.motphys.rigidbody.demos)下载并体验完整的示例项目。

| 场景                                                 | 描述                                             | 图片                                                                 |
| ---------------------------------------------------- | ------------------------------------------------ | -------------------------------------------------------------------- |
| Demos/CollisionShapes/CollisionShapes.unity          | 展示基本碰撞形状和多重碰撞器。                   | ![](https://docs.motphys.com/Images/KwevbSMLGopEx1x6BOxcHXiUnwg.gif) |
| Demos/BambooCopter/BambooCopter.unity                | 编辑力和扭矩，制作旋转飞行的竹蜻蜓。             | ![](https://docs.motphys.com/Images/EWvgbDlwpoPDBrxIlS8cTXcXn7e.gif) |
| Demos/Seesaw/Seesaw.unity                            | 调整物体的质量属性，使跷跷板两端的物体质量不同。 | ![](https://docs.motphys.com/Images/WK6pbqs6zoVXmixDFigc67yXnHe.gif) |
| Demos/FrictionAndBouncinessDemo/FrictionDemo.unity   | 演示物理材质的不同摩擦系数。                     | ![](https://docs.motphys.com/Images/HDnGbxqhEoBSr3xZRyscgUwvnfc.gif) |
| Demos/FrictionAndBouncinessDemo/BouncinessDemo.unity | 演示物理材质的不同弹性系数。                     | ![](https://docs.motphys.com/Images/EH5qbgZy7oEItvx1vUPceBggn3g.gif) |
| Demos/LayerCollision/LayerCollision.unity            | 演示如何设置碰撞过滤。                           | ![](https://docs.motphys.com/Images/AMxobXOQwom5jAxOLwTc2LuonBf.gif) |
| Demos/Explosion/Explosion.unity                      | 使用重叠测试来制作炸弹。                         | ![](https://docs.motphys.com/Images/Xqbrb6NemodbKAxUZ9DcqdE0nvd.gif) |
| Demos/ConveyorBelt/ConveyorBelt.unity                | 使用运动学移动来制作传送带。                     | ![](https://docs.motphys.com/Images/Vcp3bBUKQoJAwfxrdJIciM9qnCZ.gif) |
| Demos/Tornado/Tornado.unity                          | 演示触发器的使用方法。                           | ![](https://docs.motphys.com/Images/TB5ubZwAAo66Gpx6USYcagHGnJg.gif) |
| Demos/BreakoutGame/BreakoutGame.unity                | 制作一个打砖块小游戏，演示碰撞事件的使用方法。   | ![](https://docs.motphys.com/Images/YeE0bBfutoQuJ8xW6mZckRiynKc.gif) |
| Demos/JointTest/JointTest.unity                      | 展示各种类型的关节。                             | ![](https://docs.motphys.com/Images/YdzWbDWwZoxqaNxEwZicdqQYnaf.gif) |
| Demos/Shred/Shred.unity                              | 演示可断裂的关节。                               | ![](https://docs.motphys.com/Images/VhJmbnvnHo1wdpxuxYFcmzipnke.gif) |
| Demos/BobbleHead/BobbleHead.unity                    | 利用关节制作弹簧。                               | ![](https://docs.motphys.com/Images/FZQDbZ26zojHCsxLvducuK36nrf.gif) |
| Demos/Donut/Donut.unity                              | 利用关节制作甜甜圈。                             | ![](https://docs.motphys.com/Images/KiiPbkoTIo2z3TxX2RCcCNJRnzx.gif) |
