# 创建基本组件

组件都可以从 `AddComponent` 处添加：

![](https://docs.motphys.com/Images/VDUFbqAJfo03wux4WltcvbE6nih.png)

## 碰撞体

### 从菜单栏中创建

在 `Hierarchy` 中点击鼠标右键，选择 `Motphys` 菜单，选择目标形状。

![](https://docs.motphys.com/Images/Yl61bxR6YoVXrdxxE7ScDH2ZnZe.png)

### 在物体上添加组件

选择目标 `GameObject`，点击 `AddComponent`，选择 ` Motphys/``Colliders `，选择目标形状。

![](https://docs.motphys.com/Images/NpU6b8NfSoFf4Kxcf7qcOexqnrh.png)

## 物理材质

在 `Project` 视图中，单击鼠标右键，选择 `Create/Motphys/PhysicsMaterial` 创建物理材质，可对碰撞体赋予不同摩擦系数、弹性等。

![](https://docs.motphys.com/Images/Okojbsm1voAxmXxccApcSWNBn0b.gif)

![](https://docs.motphys.com/Images/FE6UbrrhKoprZOxy5HOcG5R7nLe.png)

| 名称<br/>                                        | 释义<br/>                                                                                                     |
| ------------------------------------------------ | ------------------------------------------------------------------------------------------------------------- |
| 弹性（Bounciness）<br/>                          | 物体弹性系数。0：完全不反弹；1：完全反弹。<br/>                                                               |
| 动摩擦（Dynamic Friction）<br/>                  | 动摩擦系数，值越大，物体在相对运动时，受到的摩擦力越大。其值永远小于等于静摩擦系数。<br/>                     |
| 静摩擦（Static Friction）<br/>                   | 静摩擦系数，值越大，物体在静止时，受到的摩擦力越大。其值永远大于等于动摩擦系数。<br/>                         |
| 启用碰撞事件（Enable Collision Event）<br/>      | 默认勾选。勾选后，此碰撞体将会产生碰撞事件，否则不产生事件，也就没有 `OnCollisionEnter` 等回调函数触发。<br/> |
| 弹性合并模式（Bounce Combine）<br/>              | 当两个不同材质的物体发生碰撞时，弹性系数如何该如何计算？<br/>                                                 |
| - Average <br/>                                  | 对两个系数求均值，即 $0.5 * (a+b)$<br/>                                                                       |
| - Minimum <br/>                                  | 两个系数中取小者，即$min(a,b)$<br/>                                                                           |
| - Multiply <br/>                                 | 对两个系数进行相乘，即$a*b$<br/>                                                                              |
| - Maximum <br/>                                  | 两个系数中取大者，即$max(a,b)$<br/>                                                                           |
| 摩擦合并模式（Friction Combine）<br/>            | 当两个不同材质的物体发生摩擦时，摩擦系数如何该如何计算？<br/>                                                 |
| - Average <br/>                                  | 对两个系数求均值，即 $0.5 * (a+b)$<br/>                                                                       |
| - Minimum <br/>                                  | 两个系数中取小者，即$min(a,b)$<br/>                                                                           |
| - Multiply <br/>                                 | 对两个系数进行相乘，即$a*b$<br/>                                                                              |
| - Maximum <br/>                                  | 两个系数中取大者，即$max(a,b)$<br/>                                                                           |
| 碰撞事件合并模式（Collision Event Combine）<br/> | 当两个不同材质的物体发生碰撞时，是否产生碰撞事件？<br/>                                                       |
| - And<br/>                                       | 当且仅当两个材质都开启时碰撞事件时，产生碰撞事件。<br/>                                                       |
| - Or<br/>                                        | 当存在一个物体开启事件时，产生碰撞事件。<br/>                                                                 |

**注 1**：当两个物体的 `Bounce/Friction Combine` 设置不同时，我们按照如下的优先级选择模式：

Maxinum > Multiply > Minimun > Average。

例如物体 A 的 `Bounce/Friction Combine` 配置是 `Average`，物体 B 的 `Bounce/Friction Combine` 配置是 `Maximum`，那么当他们发生交互时，最终采用的模式是 `Maximum`。

**注 2**：当两个物体的 `Collision Event Combine` 设置不同时，我们按照如下的优先级选择模式：

Or > And。

例如物体 A 的 `Collision Event Combine` 配置是 `And`，开启事件；物体 B 的 `Collision Event Combine` 配置是 `Or`，关闭事件。那么当他们发生交互时，最终采用的模式是 `Or`。A 和 B 都会产生碰撞事件。
