# 与 Unity 的差异

## 移动

Unity 内置物理引擎在“运行时”，会自动同步 `Transform` 的信息。而 Motphys 不会。

对于静态刚体（只有碰撞体组件的游戏物体被视为“静态刚体”），物理世界认为其 `Transform` 永远不会改变，所以 Motphys 不支持在“运行时”移动这类物体。

同理，使用代码修改 `Transform` 信息也无效。

![](https://docs.motphys.com/Images/FSJBbqrgxosirqxfBvJcPNiAnkf.png)

![](https://docs.motphys.com/Images/FWXdbJ7sKoqY2nxxEbuc3a8FnXc.gif)

（黄色为静态刚体，青色为动态刚体，运行时移动黄色刚体无效，物理世界认为黄色刚体没有移动。）

对于动态刚体（带有 `Rigidbody3D` 组件的物体被视为动态刚体），当且仅当在组件上勾选 `Enable Post Transform Control` 时，才会同步 `Transform` 信息。

否则，将不会同步 `Transform` 信息，使用代码修改 `Transform` 将无效，只有修改 `Rigidbody3D` 信息才有效。

![](https://docs.motphys.com/Images/S4fAbDFg6ojsd6xCA6tcGCZ0nsh.png)

![](https://docs.motphys.com/Images/LvYbbGMu9owzKIxVtX8cuQsHn0b.gif)

（黄色为动态刚体，青色为动态刚体，勾选 `Enable Post Transform Control`，移动黄色刚体生效。）

对于运动学刚体（带有 `Rigidbody3D` 组件，且勾选 `Kinematics` 的物体被视为运动学刚体），与动态刚体一样，需要勾选 `Enable Post Transform Control` 才可使用 `Transform` 修改物理信息。

## 旋转

与 `Position` 类似，静态刚体不支持旋转修改。动态刚体/运动学刚体需要勾选 `Enable Post Transform Control` 才有效。

## 缩放

对于在“运行时”的各种碰撞体，非常不推荐进行 `Transform.Scale` 的修改。默认情况下，Motphys 不会同步碰撞体 `Transform.Scale` 数值。

在某些特殊情况下，非常想调整 `Scale` 的话，需要开启碰撞体上的 `Support Dynamic Scale` 选项，当然，这会带来些许性能损失。

![](https://docs.motphys.com/Images/WGtabyTZ2oxRLrxnL77cWMFZnDe.png)

未勾选 `Support Dynamic Scale`，缩放一个碰撞体是无效的。

![](https://docs.motphys.com/Images/P8m4b5DWUoEuFexjdaWc9wdsn8c.gif)

勾选 `Support Dynamic Scale`，物理引擎才会同步尺寸，动态刚体发生下落。

![](https://docs.motphys.com/Images/F0EWb77c1oKTYCx2sIbckmRNnFh.gif)
