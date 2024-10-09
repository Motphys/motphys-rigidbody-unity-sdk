# 与 Unity 的差异

## 移动刚体

Unity 内置物理引擎在“运行时”，会自动同步 `Transform` 的信息。而 Motphys 不会。

对于静态物体（只有碰撞体组件的游戏物体被视为“静态物体”），物理世界认为其 `Transform` 永远不会改变，所以 Motphys 不支持在“运行时”移动这类物体。

同理，使用代码修改 `Transform` 信息也无效。

![](https://docs.motphys.com/Images/FSJBbqrgxosirqxfBvJcPNiAnkf.png)

![](https://docs.motphys.com/Images/FWXdbJ7sKoqY2nxxEbuc3a8FnXc.gif)

（黄色为静态物体，青色为动态物体，运行时移动黄色物体无效，物理世界认为黄色物体没有移动。）

对于动态物体（带有 `Rigidbody3D` 组件的物体被视为动态物体），当且仅当在组件上勾选 `Enable Post Transform Control` 时，才会同步 `Transform` 信息。

否则，将不会同步 `Transform` 信息，使用代码修改 `Transform` 将无效，只有修改 `Rigidbody3D` 信息才有效。

![](https://docs.motphys.com/Images/S4fAbDFg6ojsd6xCA6tcGCZ0nsh.png)

![](https://docs.motphys.com/Images/LvYbbGMu9owzKIxVtX8cuQsHn0b.gif)

（黄色为动态物体，青色为动态物体，勾选 `Enable Post Transform Control`，移动黄色物体生效。）

对于运动学物体（带有 `Rigidbody3D` 组件，且勾选 `Kinematics` 的物体被视为运动学物体），与动态物体一样，需要勾选 `Enable Post Transform Control` 才可使用 `Transform` 修改物理信息。
