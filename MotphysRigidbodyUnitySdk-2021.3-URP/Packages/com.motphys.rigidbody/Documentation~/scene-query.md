# 场景查询

当全局配置 `Edit/Project Settings/Motphys` 中的 `Enable Scene Query` 为开启时，Motphys 可以在 `Runtime` 环境下使用代码进行场景查询，暂不支持 `Editor` 模式下的查询。

## 射线检测

`PhysicsManager.RaycastAllNonAlloc` 会记录所有命中刚体。但是如果保存的数组长度小于命中刚体数量，返回的结果会丢弃额外的命中信息，详情见 API 文档。

![](https://docs.motphys.com/Images/N9l8bLupWo3KiFxqGoXcUGc2nu0.png)

`PhysicsManager.RaycastAny` 检测是否命中某一个刚体，命中对象不一定是最近距离的刚体。它通常用于判断“是否命中了刚体”，而不是“命中了哪一个刚体”。

![](https://docs.motphys.com/Images/FY1GbzZW6opJ9kxwIH3cd3dEn5g.png)

`PhysicsManager.RaycastClosest` 检测射线命中的距离最近的刚体。

![](https://docs.motphys.com/Images/SAnlbcMCLo62vTxD8AgcA4JanLf.png)

## 相交检测

`PhysicsManager.OverlapBoxNonAlloc` 检测场景中是否有刚体与给定参数的 `Box` 相交。

![](https://docs.motphys.com/Images/OV7Rb72aLoLxbKxd9ojc4Mntnaf.png)

`PhysicsManager.OverlapSphereNonAlloc` 检测场景中是否有刚体与给定参数的 `Sphere` 相交。

![](https://docs.motphys.com/Images/PYRebXyKlon3OJxLLJ6cbqx3nIf.png)

`PhysicsManager.OverlapCapsuleNonAlloc` 检测场景中是否有刚体与给定参数的 `Capsule` 相交。

![](https://docs.motphys.com/Images/WUUzbkYqZo5Mtwx0wO8cqXFLnob.png)
