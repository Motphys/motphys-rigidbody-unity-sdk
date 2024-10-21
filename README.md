<div align="center">
  <a href="https://www.motphys.com/">
    <img src="https://www.motphys.com/logo-blue.svg" alt="Logo" width="400" >
  </a>

  <h3 align="center">Motphys Rigidbody Unity SDK</h3>

  <p align="center">
    Motphys 刚体物理引擎 Unity SDK
  </p>
</div>

## 关于项目

Motphys Physics 是一个使用 Rust 语言编写的高性能、高精度且稳定的实时物理引擎，能够处理复杂场景中的大量物理交互，并在大规模实时模拟中提供高质量的结果。

Motphys Rigidbody Unity SDK 是该引擎的刚体物理库在 Unity 中的集成解决方案，使开发者能够轻松在 Unity 项目中利用 Motphys Physics 的强大功能，实现逼真的刚体物理效果。

您也可以访问 Motphys 的[官网](https://motphys.com/)来了解其他信息。

## 如何使用

关于如何在 Unity 项目中使用 Motphys 刚体物理引擎 Unity SDK，请参考[用户手册](https://docs.motphys.com/Packages/com.motphys.rigidbody@2.0.0-beta.8/manual/index.html)。

### 示例场景

Motphys 提供了一系列典型的示例场景，用于展示程序的核心功能和操作流程。这些场景涵盖了常见的使用情况，通过这些场景，用户可以快速上手并进行实际操作。详情参考[`com.motphys.rigidbody.demos`](MotphysRigidbodyUnitySdk-2021.3-URP/Packages/com.motphys.rigidbody.demos)。

## **项目结构**

本项目由多个版本的 Unity / 团结引擎 示例工程组成，他们使用 Unity Package Manager 引用了相同的 SDK。

| 目录                                                                                   | 引擎版本                  |
| -------------------------------------------------------------------------------------- | ------------------------- |
| [`MotphysRigidbodyUnitySdk-2021.3-URP`](./MotphysRigidbodyUnitySdk-2021.3-URP)         | Unity 2021.3.39f1         |
| [`MotphysRigidbodyUnitySdk-2022.3-URP`](./MotphysRigidbodyUnitySdk-2022.3-URP)         | Unity 2022.3.33f1         |
| [`MotphysRigidbodyUnitySdk-2023.2-URP`](./MotphysRigidbodyUnitySdk-2023.2-URP)         | Unity 2023.2.20f1         |
| [`MotphysRigidbodyUnitySdk-6000.0-URP`](./MotphysRigidbodyUnitySdk-6000.0-URP)         | Unity 6000.0.5f1          |
| [`MotphysRigidbodyUnitySdk-Tuanjie1.2-URP`](./MotphysRigidbodyUnitySdk-Tuanjie1.2-URP) | 团结 1.2.2（2022.3.2t15） |

## 包

Motphys Unity SDK 包含了多个包 (packages)，每个包都解决了特定的功能需求，并相互协作，具体功能请参考随附的包介绍表格。

| 包<br/>                                                                                                                                                    | 简介<br/>                                                                                                                                                    |
| ---------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| [`com.motphys.rigidbody`](MotphysRigidbodyUnitySdk-2021.3-URP/Packages/com.motphys.rigidbody)<br/>                                                         | 基于 MonoBehaviour 的封装，对用户提供的接口。使用方法见[手册](https://docs.motphys.com/Packages/com.motphys.rigidbody@2.0.0-beta.8/manual/index.html)。<br/> |
| [`com.motphys.rigidbody.demos`](MotphysRigidbodyUnitySdk-2021.3-URP/Packages/com.motphys.rigidbody.demos)<br/>                                             | 示例场景。可以通过下载本仓库来获取。<br/>                                                                                                                    |
| [`com.motphys.debugdraw.editor`](MotphysRigidbodyUnitySdk-2021.3-URP/Packages/com.motphys.debugdraw.editor)<br/>                                           | 编辑器内的可视化调试工具。使用方法见[手册](https://docs.motphys.com/Packages/com.motphys.debugdraw.editor@2.0.0-beta.8/manual/index.html)。<br/>             |
| [`com.motphys.rigidbody.native.standard`](MotphysRigidbodyUnitySdk-2021.3-URP/Packages/com.motphys.rigidbody.native.standard)<br/>                         | 使用 Rust 开发的项目 Native 运行时的标准版，在非 Development Build 出包时使用。<br/>                                                                         |
| [`com.motphys.rigidbody.native.standard.development`](MotphysRigidbodyUnitySdk-2021.3-URP/Packages/com.motphys.rigidbody.native.standard.development)<br/> | 使用 Rust 开发的项目 Native 运行时的开发版，在编辑器中或 Development Build 出包时使用。相对于前者额外增加了一些运行时检查和 Unity Profiler 集成。<br/>       |
| [`com.motphys.rigidbody.native`](MotphysRigidbodyUnitySdk-2021.3-URP/Packages/com.motphys.rigidbody.native)<br/>                                           | 上面两个包的聚合。<br/>                                                                                                                                      |
| [`com.motphys.rigidbody.rawapi`](MotphysRigidbodyUnitySdk-2021.3-URP/Packages/com.motphys.rigidbody.rawapi)<br/>                                           | 调用 Native 运行时的数据结构和接口。<br/>                                                                                                                    |
| [`com.motphys.core`](MotphysRigidbodyUnitySdk-2021.3-URP/Packages/com.motphys.core)<br/>                                                                   | C#基础数据结构。<br/>                                                                                                                                        |
| [`com.motphys.debugdraw.core`](MotphysRigidbodyUnitySdk-2021.3-URP/Packages/com.motphys.debugdraw.core)<br/>                                               | 可视化调试的核心库。<br/>                                                                                                                                    |

用户使用时只需要引用 [`com.motphys.rigidbody`](MotphysRigidbodyUnitySdk-2021.3-URP/Packages/com.motphys.rigidbody) 包即可，如需使用可视化调试，再引用 [`com.motphys.debugdraw.editor`](MotphysRigidbodyUnitySdk-2021.3-URP/Packages/com.motphys.debugdraw.editor)。其它依赖会被间接引入。

## 许可证

见 [LICENSE.md](LICENSE.md) 文件。
