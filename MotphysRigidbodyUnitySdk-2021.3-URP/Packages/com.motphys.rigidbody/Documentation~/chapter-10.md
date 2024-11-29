# 其他

## 质心显示

当我们选择一个刚体对象时，在编辑器【运行时】，可以看见质心的位置，如下图中蓝色球形。

质心可能会被渲染网格遮挡，此时在 `SceneView` 中，使用线框模式能看见质心。

![](https://docs.motphys.com/Images/KjIcbZYreoH8RHxOhdpcBvMon9d.png)

![](https://docs.motphys.com/Images/EkK3bBxjOoHSUuxWBADcBKkun0f.png)

## 碰撞组件编号

当一个物体上有复数个碰撞组件时，在 `SceneView` 中会进行序号显示。序号即为在编辑器面板中的组件顺序。

![](https://docs.motphys.com/Images/OADjbOPRsoTzLox3zbfcAHWSnpb.png)

## 偏好设置

在 Unity 菜单栏中，`Editor/Preference` 可打开 `Preference` 面板，用于配置一些显示。

![](https://docs.motphys.com/Images/NhPQbrjudoRdgHxp0FtcLw5inUf.png)

| 名称<br/>                                       | 释义<br/>                                       |
| ----------------------------------------------- | ----------------------------------------------- |
| 显示碰撞序号（Show Editor Collider Index）<br/> | 是否显示一个游戏对象下复数碰撞组件的序号。<br/> |
| 显示质心（Show Center Of Mass）<br/>            | 是否显示一个刚体的质心。<br/>                   |
| 质心颜色（Center Of Mass Color）<br/>           | 绘制质心时，所使用的颜色。<br/>                 |
| 质心半径（The Radius Of Centroid）<br/>         | 绘制质心时，所使用的球半径。<br/>               |

## 提取碰撞组件

![](https://docs.motphys.com/Images/PqPtbMpKbofwNyxzRgdceJaZnkb.png)

当一个刚体对象下有复数个子碰撞体时，如果子对象仅仅是作为碰撞组件的载体，可以使用 `Rigidbody3D` 的额外方法 `ExtractColliders` 提取这些组件。

当点击后，原本的组件会从子对象上删除，添加到刚体对象上。

![](https://docs.motphys.com/Images/R4RcbLmBVo9PyqxsMq7cvXemnhf.png)

## 隐藏物理组件的 Gizmos 绘制

默认情况下，Unity 会在场景中所有物理组件上绘制其图标，这可能带来视觉上的不便。在视图右上角的 Gizmos 选项中可以关闭物理组件的图标的显示。

![](https://docs.motphys.com/Images/IuwfbRDOMokuR1xmcMbc6Y57nRc.png)
