# 刚体

在物理中，刚体指“在运动中和受力作用后，形状和大小不变，而且内部各点的相对位置不变的物体”。

在物理引擎中，模拟基于物理的行为，需要为游戏对象赋予刚体组件 `Rigidbody3D`。此时，此对象被视为一个“质点”，不会发生碰撞。

特别的，当一个物体被视为刚体时，我们应该使用 `Rigidbody3D` 的属性来控制物体，而不是 `Transform`。每个物理帧模拟结束后，物理引擎会自动将物理属性（位置、旋转）同步至 `Transform` 中。

如果非常想使用 `Transform` 来控制刚体运动，在 `Rigidbody3D` 中，有一个 `Enable Post Transform Control` 布尔值。开启时，修改 `Transform` 后，物理引擎会自动同步信息到物理世界中。

## 运动类型

物理世界中，刚体对象有三种不同的类型。

| 名称<br/>                    | 释义<br/>                                                                                                     |
| ---------------------------- | ------------------------------------------------------------------------------------------------------------- |
| 静态刚体（Static）<br/>      | 表示场景中静止不动的刚体，例如地面或者建筑。不会受力的作用，也不会有速度。但是可以给予动态刚体碰撞反馈。<br/> |
| 动态刚体（Dynamic）<br/>     | 表示进行动力学模拟的物理，会受力的作用，也有速度。<br/>                                                       |
| 运动学刚体（Kinematic）<br/> | 此类型则介于两者之间，它可以被用户移动，并且可以对动态刚体施加物理作用，但其自身但不会受力的作用。<br/>       |

有一个或复数个碰撞体组件，没有 `Rigidbody3D` 组件的游戏对象，被视为【静态刚体】。

有一个或复数个碰撞体组件，有 `Rigidbody3D` 组件的游戏对象，被视为【动态刚体】。

有一个或复数个碰撞体组件，有 `Rigidbody3D` 组件的游戏对象，且刚体组件的 `Kinematics` 为 `True`，被视为【运动学刚体】。

特别的，一个带有刚体组件的物体，其所有子物体所携带的碰撞体组件都视为此刚体的碰撞体。

## 配置碰撞

碰撞体组件 `Collider` 定义刚体的形状、体积。若想让两个刚体间发生碰撞，则这两个刚体都必须添加某种碰撞体组件。

![](https://docs.motphys.com/Images/Z4H7bzFlwogNrMx3nBKckUfCnNd.png)

## 激活/休眠

从性能角度考虑，当刚体的动能持续低于某个阈值时，Motphys 将会使其进入休眠状态，休眠状态的刚体将耗费非常小的计算量。 直到它被另一个刚体接触、或者被用户手动唤醒。

### 休眠阈值

当刚体的动能持续低于此阈值时，Motphys 将会使其进入休眠状态。因此，在一些极端情况下，速度十分缓慢的物体会被判定为休眠。可以通过设置 `RigidBody3D` 的 `睡眠阈值（Sleep Energy Threshold）` 来避免。

![](https://docs.motphys.com/Images/QzPwbkQFao2sCvxgIGfcaUaVnVH.png)

### 激活/休眠事件

Motphys 支持对外提供刚体的休眠和激活事件，从而允许用户根据这些事件来驱动上层应用逻辑。

| 名称<br/>        | 释义<br/>                                   |
| ---------------- | ------------------------------------------- |
| OnWakeUp<br/>    | 当有刚体从“休眠”状态进入“激活”状态时。<br/> |
| OnSleepDown<br/> | 当有刚体从“激活”状态进入“休眠”状态时。<br/> |

```csharp
using Motphys.Rigidbody;
using UnityEngine;

public class EventSample : MonoBehaviour
{
    private void Start()
    {
        PhysicsManager.onWakeUp += OnWakeUp;
        PhysicsManager.onSleepDown += OnWakeUp;
    }

    private void OnDestroy()
    {
        PhysicsManager.onWakeUp -= OnWakeUp;
        PhysicsManager.onSleepDown -= OnWakeUp;
    }

    private void OnWakeUp(Rigidbody3D body)
    {
        Debug.Log($"Gameobject : {body.gameObject.name} wake up!");
    }

    private void OnSleepDown(Rigidbody3D body)
    {
        Debug.Log($"Gameobject : {body.gameObject.name} sleep down!");
    }
}
```

## 刚体组件（RigidyBody3D）

![](https://docs.motphys.com/Images/Rp6GbxXKgobwM5xD53bcNEJBnwg.png)

| 名称<br/>                                            | 释义<br/>                                                                                                                                                                     |
| ---------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 质量（Mass）<br/>                                    | 一般的，质量越大的物体，对其他刚体产生的影响“越大”，收到其他刚体产生的影响“越小”。<br/>                                                                                       |
| 重力（Enable Gravity）<br/>                          | 取消重力勾选，此刚体将不受重力影响。<br/>                                                                                                                                     |
| 运动学刚体（Kinematics）<br/>                        | 运动学物体，勾选时，此刚体将不再受其他任何力的影响，但可对动态刚体产生物理作用。<br/>                                                                                         |
| 启用后处理变换（Enable Post Transform Control）<br/> | 勾选时，运行时改变此游戏对象的 `Transform` 信息时，会自动同步到物理世界。<br/>否则，改变此游戏对象的 `Transform` 信息将不会被物理引擎读取，修改其位置将无效。<br/>            |
| 冻结自由度（Freeze Position/Rotation）<br/>          | 当勾选某一个轴向时，物理引擎将会冻结其自由度，`Transform` 相关轴向的数值将不会再发生改变。<br/>                                                                               |
| 阻尼（Drag/Angular Drag）<br/>                       | `Drag`：数值越大，刚体在进行位移时的阻尼越大，移动速度越小。<br/>`Angular Drag`：数值越大，刚体在进行旋转时的阻尼越大，角速度越小。<br/>                                      |
| 最大速度（Max Linear/Angular Velocity）<br/>         | `Max Linear Velocity`：刚体最大线速度。无论何时，刚体的线速度永远小于等于这个值。<br/>`Max Angular Velocity`：刚体最大角速度，无论何时，刚体的角速度永远小于等于这个值。<br/> |
| 睡眠阈值（Sleep Energy Threshold）<br/>              | 当刚体的能量小于此值时，物理引擎认为此刚体进入睡眠状态，停止运动，不再参与碰撞解算。<br/>                                                                                     |
