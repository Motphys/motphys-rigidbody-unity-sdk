# 碰撞

碰撞体定义刚体的形状体积，未添加任何碰撞体的刚体被视为一个“质点”。

特别的，碰撞体有一种特殊类型，被称之为触发器（Trigger）。

当一个碰撞体组件上的 `Is Trigger` 被启用时，它将被视为一个“触发器”，其不会参与物理模拟，不受力的作用，仅参与重叠检测。

## 碰撞事件

当两个碰撞体均未开启 `Is Trigger` 而发生重叠时，会产生碰撞事件。一般的，有三种类型的碰撞事件。

| 名称<br/>             | 释义<br/>                                     |
| --------------------- | --------------------------------------------- |
| OnCollisionEnter<br/> | 当两个刚体发生碰撞时。<br/>                   |
| OnCollisionStay<br/>  | 当两个刚体持续处于接触状态时，持续产生。<br/> |
| OnCollisionExit<br/>  | 当两个刚体脱离接触时。<br/>                   |

需要注意的是，Motphys 的全局物理配置 `Enable Contact Event` 可以开启/关闭碰撞事件的产生。

同时，物理材质的 `Enable Collision Event` 属性也会控制碰撞事件的产生。

以上配置均默认开启。

### 触发器事件

在两个碰撞体之间，至少一个开启 `Is Trigger` 而发生重叠时，会产生触发器事件。一般的，有三种类型的触发器事件。

| 名称<br/>           | 释义<br/>                                 |
| ------------------- | ----------------------------------------- |
| OnTriggerEnter<br/> | 当触发器发生接触时。<br/>                 |
| OnTriggerStay<br/>  | 当触发器持续处于接触状态，持续产生。<br/> |
| OnTriggerExit<br/>  | 当触发器脱离接触时。<br/>                 |

触发器事件恒有效，不会被全局配置或者物理材质关闭。

### 事件的产生

我们把刚体分为：

-   静态刚体
-   动态刚体
-   运动学刚体

同样的，当对应刚体上的碰撞体启用 `Is Trigger` 时，我们可以把触发器分为：

-   静态触发器
-   动态触发器
-   运动学触发器

能产生“碰撞事件”的类型组合如下：

| <br/>             | 静态刚体<br/> | 动态刚体<br/> | 运动学刚体<br/> | 静态触发器<br/> | 动态触发器<br/> | 运动学触发器<br/> |
| ----------------- | ------------- | ------------- | --------------- | --------------- | --------------- | ----------------- |
| 静态刚体<br/>     | <br/>         | √<br/>        | <br/>           | <br/>           | <br/>           | <br/>             |
| 动态刚体<br/>     | √<br/>        | √<br/>        | √<br/>          | <br/>           | <br/>           | <br/>             |
| 运动学刚体<br/>   | <br/>         | √<br/>        | <br/>           | <br/>           | <br/>           | <br/>             |
| 静态触发器<br/>   | <br/>         | <br/>         | <br/>           | <br/>           | <br/>           | <br/>             |
| 动态触发器<br/>   | <br/>         | <br/>         | <br/>           | <br/>           | <br/>           | <br/>             |
| 运动学触发器<br/> | <br/>         | <br/>         | <br/>           | <br/>           | <br/>           | <br/>             |

能产生“触发器事件”的类型组合如下：

| <br/>             | 静态刚体<br/> | 动态刚体<br/> | 运动学刚体<br/> | 静态触发器<br/> | 动态触发器<br/> | 运动学触发器<br/> |
| ----------------- | ------------- | ------------- | --------------- | --------------- | --------------- | ----------------- |
| 静态刚体<br/>     | <br/>         | <br/>         | <br/>           | <br/>           | √<br/>          | √<br/>            |
| 动态刚体<br/>     | <br/>         | <br/>         | <br/>           | √<br/>          | √<br/>          | √<br/>            |
| 运动学刚体<br/>   | <br/>         | <br/>         | <br/>           | √<br/>          | √<br/>          | √<br/>            |
| 静态触发器<br/>   | <br/>         | √<br/>        | √<br/>          | <br/>           | √<br/>          | √<br/>            |
| 动态触发器<br/>   | √<br/>        | √<br/>        | √<br/>          | √<br/>          | √<br/>          | √<br/>            |
| 运动学触发器<br/> | √<br/>        | √<br/>        | √<br/>          | √<br/>          | √<br/>          | √<br/>            |

### 注册事件

与 Unity 不同的是，Motphys 的碰撞事件/触发器事件需要手动注册。

```csharp
using Motphys.Rigidbody;
using UnityEngine;

public class EventSample : MonoBehaviour
{
    public BaseCollider baseCollider;

    void Start()
    {
        baseCollider.onCollisionEnter += CollisionEnter;
        baseCollider.onCollisionStay += CollisionStay;
        baseCollider.onCollisionExit += CollisionExit;

        baseCollider.onTriggerEnter += TriggerEnter;
        baseCollider.onTriggerStay += TriggerStay;
        baseCollider.onTriggerExit += TriggerExit;
    }

    private void CollisionEnter(Motphys.Rigidbody.Collision collision)
    {
        Debug.Log("Collision enter happens.");
    }

    private void CollisionStay(Motphys.Rigidbody.Collision collision)
    {
        Debug.Log("Collision stay happens.");
    }

    private void CollisionExit(Motphys.Rigidbody.Collision collision)
    {
        Debug.Log("Collision exit happens.");
    }

    private void TriggerEnter(BaseCollider collider)
    {
        Debug.Log("Trigger enter happens.");
    }

    private void TriggerStay(BaseCollider collider)
    {
        Debug.Log("Trigger stay happens.");
    }

    private void TriggerExit(BaseCollider collider)
    {
        Debug.Log("Trigger exit happens.");
    }
}
```

特别的，在 Motphys 中，我们可以注册全局碰撞事件。

```csharp
using Motphys.Rigidbody;
using UnityEngine;

public class EventSample : MonoBehaviour
{
    private void Start()
    {
        PhysicsManager.onCollisionEnter += CollisionEnter;
    }

    private void OnDestroy()
    {
        PhysicsManager.onCollisionEnter -= CollisionEnter;
    }

    private void CollisionEnter(CollisionEvent collisionEvent)
    {
        Debug.Log($"GameObject {collisionEvent.collider1.gameObject.name} and GameObject {collisionEvent.collider2.gameObject.name} collides");
    }
}
```

## 碰撞层级（Collision Layer）

我们可以在 `Editor/Project Settings/Motphys` 中配置碰撞矩阵，让物理引擎忽视某两个层级之间的刚体碰撞。

![](https://docs.motphys.com/Images/TB2ZbwfkToFpopxpFYsc6DtPnWe.gif)

刚体的碰撞层级与 Unity GameObject 的 `Layer` 对应。

![](https://docs.motphys.com/Images/RjKNbGKcbo4hwUxYfQmc2PGlnad.png)

一般的，碰撞矩阵如下。

![](https://docs.motphys.com/Images/KWHEb6f6ooyMQOxMI54cuiSpnVg.png)

图中，`Water` 与 `Default` 对应的勾选框未勾选，表示处于 `Water` 层级的刚体不会与 `Default` 层级的刚体发生碰撞。

## 碰撞形状（Collider Shapes）

### 方盒碰撞体（BoxCollider3D）

![](https://docs.motphys.com/Images/EmREbmIm1oP8LHxC1R4cVyNonPd.png)

| 名称<br/>                                       | 释义<br/>                                                                                                                                                                                                                                                                                                                                                                    |
| ----------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 共享材质（Shared Material）<br/>                | 碰撞体所使用的 `物理材质`，来自于工程中的创建的资源 `.asset`，不同的碰撞体可以共享同一个物理材质。在这种情况下，修改这份材质参数将会影响所有相关碰撞体。<br/>                                                                                                                                                                                                                |
| 触发器（Is Trigger）<br/>                       | 这个物体是否是触发器？如果是，则产生触发事件，不产生碰撞事件，不参与碰撞解算。<br/>                                                                                                                                                                                                                                                                                          |
| 局部坐标（Collider Local Transform）<br/>       | 碰撞体的局部坐标，实际的物理解算会在世界空间下进行，碰撞体会经历"碰撞体局部坐标"->"游戏物体局部坐标"->"世界坐标"的转换。<br/>                                                                                                                                                                                                                                                |
| 方盒尺寸（Size）<br/>                           | 方盒碰撞体的尺寸。<br/>                                                                                                                                                                                                                                                                                                                                                      |
| 支持动态缩放（Support Dynamic Scale）<br/><br/> | `SupportDynamicScale` 表示当游戏对象的 `Scale` 在"运行时"发生改变时，是否尝试重建碰撞体尺寸。<br/>比如一个 `Scale` 为 `(1,1,1)` 的游戏对象，挂载尺寸为 `(1,1,1)` 的 `BoxCollider3D`，则实际的碰撞体尺寸为 `(1,1,1)`。<br/>在运行时，当游戏对象的 `Scale` 变成 `(1,2,3)` 时，开启 `SupportDynamicScale`，则碰撞体尺寸会更新为 `(1,2,3)`；否则，将保持 `(1,1,1)` 的尺寸。<br/> |

### 球碰撞体（SphereCollider3D）

![](https://docs.motphys.com/Images/U2PPb54BfoXhSQxVlrxcYAocn4c.png)

| 名称<br/>                                  | 释义<br/>                                                                                                                                                                                                                                                                                                                                                                                       |
| ------------------------------------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 共享材质（Shared Material）<br/>           | 碰撞体所使用的 `物理材质`，来自于工程中的创建的资源 `.asset`，不同的碰撞体可以共享同一个物理材质。在这种情况下，修改这份材质参数将会影响所有相关碰撞体。<br/>                                                                                                                                                                                                                                   |
| 触发器（Is Trigger）<br/>                  | 这个物体是否是触发器？如果是，则产生触发事件，不产生碰撞事件，不参与碰撞解算。<br/>                                                                                                                                                                                                                                                                                                             |
| 局部坐标（Collider Local Transform）<br/>  | 碰撞体的局部坐标，实际的物理解算会在世界空间下进行，碰撞体会经历"碰撞体局部坐标"->"游戏物体局部坐标"->"世界坐标"的转换。<br/>                                                                                                                                                                                                                                                                   |
| 球半径（Radius）<br/>                      | 球碰撞体的半径。<br/>                                                                                                                                                                                                                                                                                                                                                                           |
| 支持动态缩放（Support Dynamic Scale）<br/> | `SupportDynamicScale` 表示当游戏对象的 `Scale` 在"运行时"发生改变时，是否尝试重建碰撞体形状。<br/>比如一个 `Scale` 为 `(1,1,1)` 的游戏对象，挂载半径为的 `0.5` 的 `SphereCollider3D`，则实际的球碰撞体半径为 `0.5`。<br/>在运行时，当游戏对象的 `Scale` 变成 `(1,2,3)` 时，开启 `SupportDynamicScale`，则碰撞体半径会更新为 `1.5`（设置半径乘以最大缩放分量）；否则，将保持 `0.5` 的半径。<br/> |

### 胶囊碰撞体（CapsuleCollider3D）

![](https://docs.motphys.com/Images/AVEZbtspLo9RTCx8givclp58nOh.png)

| 名称<br/>                                  | 释义<br/>                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| ------------------------------------------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 共享材质（Shared Material）<br/>           | 碰撞体所使用的 `物理材质`，来自于工程中的创建的资源 `.asset`，不同的碰撞体可以共享同一个物理材质。在这种情况下，修改这份材质参数将会影响所有相关碰撞体。<br/>                                                                                                                                                                                                                                                                                                               |
| 触发器（Is Trigger）<br/>                  | 这个物体是否是触发器？如果是，则产生触发事件，不产生碰撞事件，不参与碰撞解算。<br/>                                                                                                                                                                                                                                                                                                                                                                                         |
| 局部坐标（Collider Local Transform）<br/>  | 碰撞体的局部坐标，实际的物理解算会在世界空间下进行，碰撞体会经历"碰撞体局部坐标"->"游戏物体局部坐标"->"世界坐标"的转换。<br/>                                                                                                                                                                                                                                                                                                                                               |
| 半球半径（Radius）<br/>                    | 胶囊体的上下两个半球的半径。<br/>                                                                                                                                                                                                                                                                                                                                                                                                                                           |
| 整体高度（Height）<br/>                    | 胶囊体的整体高度（包括 2 个半球半径）。<br/>                                                                                                                                                                                                                                                                                                                                                                                                                                |
| 支持动态缩放（Support Dynamic Scale）<br/> | `SupportDynamicScale` 表示当游戏对象的 `Scale` 在"运行时"发生改变时，是否尝试重建碰撞体形状。<br/>比如一个 `Scale` 为 `(1,1,1)` 的游戏对象，挂载半径为的 `0.5`，高度为 `2` 的 `CapsuleCollider3D`，则实际的碰撞体半径为 `0.5`，高度为 `2`。<br/>在运行时，当游戏对象的 `Scale` 变成 `(1,2,3)` 时，开启 `SupportDynamicScale`，则碰撞体半径会更新为 `1.5`（半径乘以 Max(scale.x, scale.z)），高度为 `4`（高度乘以 scale.y）；否则，将保持 `0.5` 的半径，与 `2` 的高度。<br/> |

### 圆柱碰撞体（CylinderCollider3D）

![](https://docs.motphys.com/Images/SrCAbSIrRoHuzAxAaXfcxviJndO.png)

| 名称<br/>                                  | 释义<br/>                                                                                                                                                                                                                                                                                                                                                                                                                                                                    |
| ------------------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 共享材质（Shared Material）<br/>           | 碰撞体所使用的 `物理材质`，来自于工程中的创建的资源 `.asset`，不同的碰撞体可以共享同一个物理材质。在这种情况下，修改这份材质参数将会影响所有相关碰撞体。<br/>                                                                                                                                                                                                                                                                                                                |
| 触发器（Is Trigger）<br/>                  | 这个物体是否是触发器？如果是，则产生触发事件，不产生碰撞事件，不参与碰撞解算。<br/>                                                                                                                                                                                                                                                                                                                                                                                          |
| 局部坐标（Collider Local Transform）<br/>  | 碰撞体的局部坐标，实际的物理解算会在世界空间下进行，碰撞体会经历"碰撞体局部坐标"->"游戏物体局部坐标"->"世界坐标"的转换。<br/>                                                                                                                                                                                                                                                                                                                                                |
| 半径（Radius）<br/>                        | 圆柱体的半径。<br/>                                                                                                                                                                                                                                                                                                                                                                                                                                                          |
| 高度（Height）<br/>                        | 圆柱体的高度。<br/>                                                                                                                                                                                                                                                                                                                                                                                                                                                          |
| 支持动态缩放（Support Dynamic Scale）<br/> | `SupportDynamicScale` 表示当游戏对象的 `Scale` 在"运行时"发生改变时，是否尝试重建碰撞体形状。<br/>比如一个 `Scale` 为 `(1,1,1)` 的游戏对象，挂载半径为的 `0.5`，高度为 `2` 的 `CylinderCollider3D`，则实际的碰撞体半径为 `0.5`，高度为 `2`。<br/>在运行时，当游戏对象的 `Scale` 变成 `(1,2,3)` 时，开启 `SupportDynamicScale`，则碰撞体半径会更新为 `1.5`（半径乘以 Max(scale.x, scale.z)），高度为 `4`（高度乘以 scale.y）；否则，将保持 `0.5` 的半径，与 `2` 的高度。<br/> |

### 无限平面碰撞体（InfinitePlaneCollider3D）

![](https://docs.motphys.com/Images/TGwnbKCuMo4mWjxOHjdcZFnXnNd.png)

| 名称<br/>                                 | 释义<br/>                                                                                                                                                     |
| ----------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 共享材质（Shared Material）<br/>          | 碰撞体所使用的 `物理材质`，来自于工程中的创建的资源 `.asset`，不同的碰撞体可以共享同一个物理材质。在这种情况下，修改这份材质参数将会影响所有相关碰撞体。<br/> |
| 触发器（Is Trigger）<br/>                 | 这个物体是否是触发器？如果是，则产生触发事件，不产生碰撞事件，不参与碰撞解算。<br/>                                                                           |
| 局部坐标（Collider Local Transform）<br/> | 碰撞体的局部坐标，实际的物理解算会在世界空间下进行，碰撞体会经历"碰撞体局部坐标"->"游戏物体局部坐标"->"世界坐标"的转换。<br/>                                 |

### 复合碰撞体（Compound Collider）

复合碰撞体是一些碰撞体的集合。当我们在一个刚体上添加复数个碰撞体组件时，这些碰撞体所构成的一个整体，我们称其为“复合碰撞体”。

一般的，对于带有 `Rigidbody3D` 组件的父物体，我们认为其所有的带有 `Collider` 组件的子物体（包括孙子物体），都是复合碰撞体的一部分。

![](https://docs.motphys.com/Images/OnZYbiaYjo2F89xtn7UcHmwPn0g.png)

也可以在同一个 `GameObject` 上添加 `Rigidbody3D` 与 `Collider` 组件。

![](https://docs.motphys.com/Images/SUV7bKyXKoYOb3xcURBcblQAnhh.png)

在复合碰撞体中，对于每一个单独的碰撞体，我们都能配置其对应的物理材质，或者启用 `Is Trigger`。

### 网格碰撞体（MeshCollider3D）

在 Motphys 中，不支持“凹面”碰撞，仅支持“凸面”碰撞。这意味实际的物理网格碰撞体，可能与原始的渲染网格有偏差。

![](https://docs.motphys.com/Images/NSfMbDHeeoB2Z3xeyfDcov8Tn6e.png)

由于网格碰撞体相较于其他基本形状，在性能上有较大的开销，一个比较好的做法是，让场景中的静态物体使用网格碰撞体，动态物体使用复合碰撞体。

![](https://docs.motphys.com/Images/For8bOt6moGdo1xC2lzc3GA0nBe.png)

| 名称<br/>                                 | 释义<br/>                                                                                                                                                     |
| ----------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 共享材质（Shared Material）<br/>          | 碰撞体所使用的 `物理材质`，来自于工程中的创建的资源 `.asset`，不同的碰撞体可以共享同一个物理材质。在这种情况下，修改这份材质参数将会影响所有相关碰撞体。<br/> |
| 触发器（Is Trigger）<br/>                 | 这个物体是否是触发器？如果是，则产生触发事件，不产生碰撞事件，不参与碰撞解算。<br/>                                                                           |
| 局部坐标（Collider Local Transform）<br/> | 碰撞体的局部坐标，实际的物理解算会在世界空间下进行，碰撞体会经历"碰撞体局部坐标"->"游戏物体局部坐标"->"世界坐标"的转换。<br/>                                 |
| 网格（Mesh）<br/>                         | 碰撞体所参考的网格数据。一般的，网格碰撞体会对网格“凸包化”，最终的碰撞几何与实际渲染几何可能存在偏差。<br/>                                                   |
