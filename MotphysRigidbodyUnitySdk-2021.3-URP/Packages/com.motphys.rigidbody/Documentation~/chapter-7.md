# 约束

约束组件可以创建两个刚体之间的约束，限制两个刚体之间的运动自由度（位移与旋转）。

**锚点**

当约束连接刚体时，会在刚体上抽象出“锚点”（Anchor）的概念，锚点的坐标配置被称之为“Anchor Frame”。

约束实际作用力在锚点上，以 `FixedJoint3D` 为例，`FixedJoint3D` 包含两个子约束：

-   距离约束会让两个刚体的“锚点位置”重合。
-   旋转约束会让两个刚体的“锚点坐标轴”朝向一致。

对于如下配置：

![](https://docs.motphys.com/Images/ABvubSTZ8olvmIxExQecfHH1nXd.png)

`FixedJoint3D` 作用结果如下：

![](https://docs.motphys.com/Images/Kwd9bOtZuoFyd1xPsOvcBxYwnJb.png)

对于不同的 Joint 类型，有不同的约束自由度。

## 编辑器面板

一般的，对于一种约束类型，有三个面板选项，属于不同的功能模块。

![](https://docs.motphys.com/Images/R25ibIOepoUallx52qJcjM8Mnyc.png)

| 名称<br/>           | 释义<br/>                                                     |
| ------------------- | ------------------------------------------------------------- |
| 锚点（Anchor）<br/> | 配置所连接的刚体对象，约束锚点的位置、旋转等。<br/>           |
| 限制（Limit）<br/>  | 配置约束的自由度，比如一定范围内移动、一定角度内摆动等。<br/> |
| 马达（Motor）<br/>  | 配置马达如何驱动连接刚体间的位移，旋转等。<br/>               |

## 椭球约束（EllipsoidJoint3D）

椭球约束拥有 0 个位移自由度，3 个旋转自由度，通常可以用于模拟类似骨骼关节的约束。在此约束下，我们会将两个刚体的相对旋转分解为扭曲（Twist）和摆动（Swing）两个运动。默认扭曲轴为 +X 轴。

![](https://docs.motphys.com/Images/YPhRboZLCo6xeQxCYOQcC43nnVf.gif)

![](https://docs.motphys.com/Images/GGmTbn5l4oQpZcxmCGMc5XJ1nrg.png)

| 名称<br/>                                   | 释义<br/>                                                                                                                                          |
| ------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| 连接刚体（Connect Body）<br/>               | 约束组件连接其所在刚体，与一个其他刚体。未连接其他刚体时，组件不生效。<br/>                                                                        |
| 锚点（Anchor Frame）<br/>                   | 锚点的局部坐标配置。<br/>                                                                                                                          |
| - 锚点位置（Anchor）<br/>                   | 相当于约束局部空间下的原点，一般可以与渲染几何中心对齐（0,0,0），也可以手动设置偏移量。<br/>                                                       |
| - 锚点旋转（Axis Rotation）<br/>            | 意味着旋转约束局部空间，约束默认以局部空间的 +X 轴为旋转轴。如果想让约束以世界空间 +Z 轴为旋转轴，可以设置为（0, -90, 0），均为“左手坐标系”。<br/> |
| 忽视碰撞（Ignore Collision）<br/>           | 当勾选时，约束连接的两个刚体将不会发生碰撞。<br/>                                                                                                  |
| 断裂力（Break Force）<br/>                  | 约束发生断裂所需要的力。`infinity` 表示不会发生断裂。<br/>                                                                                         |
| 断裂扭矩（Break Torque）<br/>               | 约束发生断裂所需要的扭矩。`infinity` 表示不会发生断裂。<br/>                                                                                       |
| 细节与自定义（Details）<br/>                | 一些额外的配置细节。<br/>                                                                                                                          |
| - 自动配置（Auto Configure Connected）<br/> | 是否自动配置连接刚体的锚点。当开启时，认为连接刚体的锚点与本锚点对齐。<br/>                                                                        |
| - 连接锚点（Connected Anchor Frame）<br/>   | 配置连接锚点，作用与锚点类似。<br/>                                                                                                                |

![](https://docs.motphys.com/Images/NYzFbjdk4oDrb9x2sOQchxyenfa.png)

| 名称<br/>                        | 释义<br/>                                                       |
| -------------------------------- | --------------------------------------------------------------- |
| 扭曲范围（Twist Limit）<br/>     | 以局部空间 +X 轴为旋转轴，可旋转的范围。<br/>                   |
| - 最小值（Twist Low）<br/>       | 扭曲角度的最小值，取值范围[-180,180]。<br/>                     |
| - 最大值（Twist High）<br/>      | 扭曲角度的最大值，取值范围[-180,180]。<br/>                     |
| 摆动范围 Y（Swing Limit Y）<br/> | 以局部空间 +Y 轴为旋转轴，可旋转的范围。<br/>                   |
| - 最小值（Low）<br/>             | 取值范围[-180,0]。<br/>                                         |
| - 最大值（High）<br/>            | 取值范围[0,180]。<br/>                                          |
| 摆动范围 Z（Swing Limit Z）<br/> | 以局部空间 +Z 轴为旋转轴，可旋转的范围。<br/>                   |
| - 最小值（Low）<br/>             | 取值范围[-180,0]。<br/>                                         |
| - 最大值（High）<br/>            | 取值范围[0,180]。<br/>                                          |
| 使用弹簧（Use Spring）<br/>      | 是否使用弹簧。取消勾选时，生效的弹簧刚度为无限，阻尼为 0。<br/> |
| 弹簧阻尼（Limit Spring）<br/>    | 用于控制刚体的运动表现。<br/>                                   |
| - 刚度（Stiffness）<br/>         | 值越大，刚体可超出约束的范围越少。<br/>                         |
| - 阻尼（Damper）<br/>            | 值越大，刚体超出约束范围时，受到的阻力越大。<br/>               |
| 角度阻尼（Angular Damper）<br/>  | 值越大，刚体在运动时，旋转阻力越大。<br/>                       |

![](https://docs.motphys.com/Images/UlyKbyqSmobdJFx0ae5ccuNpnuh.png)

| 名称<br/>                                              | 释义<br/>                                                |
| ------------------------------------------------------ | -------------------------------------------------------- |
| 使用马达（Use Motor）<br/>                             | 勾选时，将使用马达，否则不使用。<br/>                    |
| 马达模式（Angular Motor Drive Mode）<br/>              | 马达的一些基础模式。<br/>                                |
| - 四元数插值（S Lerp）<br/>                            | 以更平滑的运动靠近目标角度。<br/>                        |
| - 扭曲与摆动（Twist Swing）<br/>                       | 扭曲 + 摆动结合模式，以逐轴变换的方式靠近目标角度。<br/> |
| - 角速度驱动（Velocity）<br/>                          | 马达驱动刚体到达恒定角速度。<br/>                        |
| 目标角度/速度（Target Rotation/Angular Velocity）<br/> | 驱动的目标角度/速度。不同模式所暴露的字段不一样。<br/>   |
| 强度（Strength）<br/>                                  | 驱动强度。数值越大，刚体越容易到达目标角度/角速度。<br/> |
| 阻尼（Damper）<br/>                                    | 驱动阻尼，数值越小，刚体能更快到达目标角度/角速度。<br/> |

## 球形约束（BallJoint3D）

球形约束属于椭球约束的特例，两个摆动轴角度相等且对称。

![](https://docs.motphys.com/Images/YFl9b5Ss3o6gfSxaj6XcoFcSnwD.gif)

![](https://docs.motphys.com/Images/R5oEb6YICoPwDpxGnwDcctryn7e.png)

| 名称<br/>                                   | 释义<br/>                                                                                                                                          |
| ------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| 连接刚体（Connect Body）<br/>               | 约束组件连接其所在刚体，与一个其他刚体。未连接其他刚体时，组件不生效。<br/>                                                                        |
| 锚点（Anchor Frame）<br/>                   | 锚点的局部坐标配置。<br/>                                                                                                                          |
| - 锚点位置（Anchor）<br/>                   | 相当于约束局部空间下的原点，一般可以与渲染几何中心对齐（0,0,0），也可以手动设置偏移量。<br/>                                                       |
| - 锚点旋转（Axis Rotation）<br/>            | 意味着旋转约束局部空间，约束默认以局部空间的 +X 轴为旋转轴。如果想让约束以世界空间 +Z 轴为旋转轴，可以设置为（0, -90, 0），均为“左手坐标系”。<br/> |
| 忽视碰撞（Ignore Collision）<br/>           | 当勾选时，约束连接的两个刚体将不会发生碰撞。<br/>                                                                                                  |
| 断裂力（Break Force）<br/>                  | 约束发生断裂所需要的力。`infinity` 表示不会发生断裂。<br/>                                                                                         |
| 断裂扭矩（Break Torque）<br/>               | 约束发生断裂所需要的扭矩。`infinity` 表示不会发生断裂。<br/>                                                                                       |
| 细节与自定义（Details）<br/>                | 一些额外的配置细节。<br/>                                                                                                                          |
| - 自动配置（Auto Configure Connected）<br/> | 是否自动配置连接刚体的锚点。当开启时，认为连接刚体的锚点与本锚点对齐。<br/>                                                                        |
| - 连接锚点（Connected Anchor Frame）<br/>   | 配置连接锚点，作用与锚点类似。<br/>                                                                                                                |

![](https://docs.motphys.com/Images/OHcebfa9go7UhdxuKsGcCxiRnmh.png)

| 名称<br/>                       | 释义<br/>                                                       |
| ------------------------------- | --------------------------------------------------------------- |
| 扭曲范围（Twist Limit）<br/>    | 以局部空间 +X 轴为旋转轴，可旋转的范围。<br/>                   |
| - 最小值（Twist Low）<br/>      | 扭曲角度的最小值，取值范围[-180,180]。<br/>                     |
| - 最大值（Twist High）<br/>     | 扭曲角度的最大值，取值范围[-180,180]。<br/>                     |
| 摆动范围（Swing Limit）<br/>    | 表示可以摆动的范围，取值范围[0,180]。<br/>                      |
| 使用弹簧（Use Spring）<br/>     | 是否使用弹簧。取消勾选时，生效的弹簧刚度为无限，阻尼为 0。<br/> |
| 弹簧阻尼（Limit Spring）<br/>   | 用于控制刚体的运动表现。<br/>                                   |
| - 刚度（Stiffness）<br/>        | 值越大，刚体可超出约束的范围越少。<br/>                         |
| - 阻尼（Damper）<br/>           | 值越大，刚体超出约束范围时，受到的阻力越大。<br/>               |
| 角度阻尼（Angular Damper）<br/> | 值越大，刚体在运动时，旋转阻力越大。<br/>                       |

![](https://docs.motphys.com/Images/YuqFb09K8o8qqJx9CWEcARWynih.png)

| 名称<br/>                                              | 释义<br/>                                                |
| ------------------------------------------------------ | -------------------------------------------------------- |
| 使用马达（Use Motor）<br/>                             | 勾选时，将使用马达，否则不使用。<br/>                    |
| 马达模式（Angular Motor Drive Mode）<br/>              | 马达的一些基础模式。<br/>                                |
| - 四元数插值（S Lerp）<br/>                            | 以更平滑的运动靠近目标角度。<br/>                        |
| - 扭曲与摆动（Twist Swing）<br/>                       | 扭曲 + 摆动结合模式，以逐轴变换的方式靠近目标角度。<br/> |
| - 角速度驱动（Velocity）<br/>                          | 马达驱动刚体到达恒定角速度。<br/>                        |
| 目标角度/速度（Target Rotation/Angular Velocity）<br/> | 驱动的目标角度/速度。不同模式所暴露的字段不一样。<br/>   |
| 强度（Strength）<br/>                                  | 驱动强度。数值越大，刚体越容易到达目标角度/角速度。<br/> |
| 阻尼（Damper）<br/>                                    | 驱动阻尼，数值越小，刚体能更快到达目标角度/角速度。<br/> |

## 距离约束（DistanceJoint3D）

距离约束可以让两个刚体之间的距离保持在指定范围内， 通常可以用来模拟类似绳子一样的约束。

![](https://docs.motphys.com/Images/XApQbFbBcorqhFx25NEcPQ6rnWe.gif)

![](https://docs.motphys.com/Images/KAZUbP7vSoDMVCxANbLcRydkn2U.png)

| 名称<br/>                                   | 释义<br/>                                                                                    |
| ------------------------------------------- | -------------------------------------------------------------------------------------------- |
| 连接刚体（Connect Body）<br/>               | 约束组件连接其所在刚体，与一个其他刚体。未连接其他刚体时，组件不生效。<br/>                  |
| 锚点（Anchor）<br/>                         | 相当于约束局部空间下的原点，一般可以与渲染几何中心对齐（0,0,0），也可以手动设置偏移量。<br/> |
| 忽视碰撞（Ignore Collision）<br/>           | 当勾选时，约束连接的两个刚体将不会发生碰撞。<br/>                                            |
| 断裂力（Break Force）<br/>                  | 约束发生断裂所需要的力。`infinity` 表示不会发生断裂。<br/>                                   |
| 断裂扭矩（Break Torque）<br/>               | 约束发生断裂所需要的扭矩。`infinity` 表示不会发生断裂。<br/>                                 |
| 细节与自定义（Details）<br/>                | 一些额外的配置细节。<br/>                                                                    |
| - 自动配置（Auto Configure Connected）<br/> | 是否自动配置连接刚体的锚点。当开启时，认为连接刚体的锚点与本锚点对齐。<br/>                  |
| - 连接锚点（Connected Anchor Frame）<br/>   | 配置连接锚点，作用与锚点类似。<br/>                                                          |

![](https://docs.motphys.com/Images/B5w1b1G0MospRgxpcWzcWOfenEc.png)

| 名称<br/>                           | 释义<br/>                                                                     |
| ----------------------------------- | ----------------------------------------------------------------------------- |
| 使用限度（Use Limit）<br/>          | 勾选时，将使用限度，将刚体的运动固定在一定范围。否则，限制范围是[0, 0]。<br/> |
| 最小距离（Min Distance）<br/>       | 两个刚体之间的最小距离。<br/>                                                 |
| 最大距离（Max Distance）<br/>       | 两个刚体之间的最大距离。<br/>                                                 |
| 使用弹簧（Use Spring）<br/>         | 是否使用弹簧。取消勾选时，生效的弹簧刚度为无限，阻尼为 0。<br/>               |
| 弹簧阻尼（Limit Spring）<br/>       | 用于控制刚体的运动表现。<br/>                                                 |
| - 刚度（Stiffness）<br/>            | 值越大，刚体可超出约束的范围越少。<br/>                                       |
| - 阻尼（Damper）<br/>               | 值越大，刚体超出约束范围时，受到的阻力越大。<br/>                             |
| 速度阻尼（Velocity Damper）<br/>    | 用于控制刚体的速度。<br/>                                                     |
| - 线速度阻尼（Linear Damper）<br/>  | 值越大，刚体位移运动时受到阻力越大。<br/><br/>                                |
| - 角速度阻尼（Angular Damper）<br/> | 值越大，刚体旋转运动时受到阻力越大。<br/>                                     |

![](https://docs.motphys.com/Images/XhvAbc5u5opxngxo1eJcFc47ndf.png)

| 名称<br/>                                   | 释义<br/>                                              |
| ------------------------------------------- | ------------------------------------------------------ |
| 使用马达（Use Motor）<br/>                  | 勾选时，将使用马达，否则不使用。<br/>                  |
| 马达模式（Motor Type）<br/>                 | 马达的一些基础模式。<br/>                              |
| - 距离驱动（Distance Drive）<br/>           | 马达驱动刚体满足目标距离。<br/>                        |
| - 速度驱动（Speed Drive）<br/>              | 马达驱动刚体间的距离以恒定速度变化。<br/>              |
| 目标距离/速度（Target Distance/Speed）<br/> | 驱动的目标距离/速度。不同模式所暴露的字段不一样。<br/> |
| 强度（Strength）<br/>                       | 驱动强度。数值越大，刚体越容易到达目标距离/速度。<br/> |
| 阻尼（Damper）<br/>                         | 驱动阻尼，数值越小，刚体越快到达目标距离。<br/>        |

## 固定约束（FixedJoint3D）

固定约束可以将两个刚体固定到一起，使得两者之间的相对姿态保持不变。

![](https://docs.motphys.com/Images/B3bQbGgRAoDOl0xYnRfcDrGsnzf.gif)

![](https://docs.motphys.com/Images/RWeMbgVGLo7raSxCf80cLuj8nSN.png)

| 名称<br/>                         | 释义<br/>                                                                   |
| --------------------------------- | --------------------------------------------------------------------------- |
| 连接刚体（Connect Body）<br/>     | 约束组件连接其所在刚体，与一个其他刚体。未连接其他刚体时，组件不生效。<br/> |
| 忽视碰撞（Ignore Collision）<br/> | 当勾选时，约束连接的两个刚体将不会发生碰撞。<br/>                           |
| 断裂力（Break Force）<br/>        | 约束发生断裂所需要的力。`infinity` 表示不会发生断裂。<br/>                  |
| 断裂扭矩（Break Torque）<br/>     | 约束发生断裂所需要的扭矩。`infinity` 表示不会发生断裂。<br/>                |

## 合页约束（HingeJoint3D）

合页约束只有一个旋转自由度，通常用来模拟像门轴这样的约束。以局部空间 +X 轴为旋转轴。

![](https://docs.motphys.com/Images/NdJ6byyEOoMqIfxtAMOcgl4TnMe.gif)

![](https://docs.motphys.com/Images/IKt0bKvfioKhCJxT8yXcMUGWnAc.png)

| 名称<br/>                                   | 释义<br/>                                                                                                                                          |
| ------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| 连接刚体（Connect Body）<br/>               | 约束组件连接其所在刚体，与一个其他刚体。未连接其他刚体时，组件不生效。<br/>                                                                        |
| 锚点（Anchor Frame）<br/>                   | 锚点的局部坐标配置。<br/>                                                                                                                          |
| - 锚点位置（Anchor）<br/>                   | 相当于约束局部空间下的原点，一般可以与渲染几何中心对齐（0,0,0），也可以手动设置偏移量。<br/>                                                       |
| - 锚点旋转（Axis Rotation）<br/>            | 意味着旋转约束局部空间，约束默认以局部空间的 +X 轴为旋转轴。如果想让约束以世界空间 +Z 轴为旋转轴，可以设置为（0, -90, 0），均为“左手坐标系”。<br/> |
| 忽视碰撞（Ignore Collision）<br/>           | 当勾选时，约束连接的两个刚体将不会发生碰撞。<br/>                                                                                                  |
| 断裂力（Break Force）<br/>                  | 约束发生断裂所需要的力。`infinity` 表示不会发生断裂。<br/>                                                                                         |
| 断裂扭矩（Break Torque）<br/>               | 约束发生断裂所需要的扭矩。`infinity` 表示不会发生断裂。<br/>                                                                                       |
| 细节与自定义（Details）<br/>                | 一些额外的配置细节。<br/>                                                                                                                          |
| - 自动配置（Auto Configure Connected）<br/> | 是否自动配置连接刚体的锚点。当开启时，认为连接刚体的锚点与本锚点对齐。<br/>                                                                        |
| - 连接锚点（Connected Anchor Frame）<br/>   | 配置连接锚点，作用与锚点类似。<br/>                                                                                                                |

![](https://docs.motphys.com/Images/FZYybsR1QoOo5lx6Rykc2gusnNh.png)

| 名称<br/>                       | 释义<br/>                                                                           |
| ------------------------------- | ----------------------------------------------------------------------------------- |
| 使用限度（Use Limit）<br/>      | 勾选时，将使用限度，将刚体的运动固定在一定范围。取消勾选时，范围为[-180,180]。<br/> |
| 角度范围（Angle Limit）<br/>    | 以局部空间 +X 轴为旋转轴，可旋转的范围。<br/>                                       |
| - 最小值（Low）<br/>            | 旋转角度的最小值，取值范围[-180,180]。<br/>                                         |
| - 最大值（High）<br/>           | 旋转角度的最大值，取值范围[-180,180]。<br/>                                         |
| 使用弹簧（Use Spring）<br/>     | 是否使用弹簧。取消勾选时，生效的弹簧刚度为无限，阻尼为 0。<br/>                     |
| 弹簧阻尼（Limit Spring）<br/>   | 用于控制刚体的运动表现。<br/>                                                       |
| - 刚度（Stiffness）<br/>        | 值越大，刚体可超出约束的范围越少。<br/>                                             |
| - 阻尼（Damper）<br/>           | 值越大，刚体超出约束范围时，受到的阻力越大。<br/>                                   |
| 角度阻尼（Angular Damper）<br/> | 值越大，刚体在运动时，旋转阻力越大。<br/>                                           |

![](https://docs.motphys.com/Images/BTRqbmRDCoNxRmxd2tYcFZDmnXg.png)

| 名称<br/>                                | 释义<br/>                                              |
| ---------------------------------------- | ------------------------------------------------------ |
| 使用马达（Use Motor）<br/>               | 勾选时，将使用马达，否则不使用。<br/>                  |
| 马达模式（Motor Type）<br/>              | 马达的一些基础模式。<br/>                              |
| - 角度驱动（Angle Drive）<br/>           | 马达驱动刚体满足目标角度。<br/>                        |
| - 速度驱动（Speed Drive）<br/>           | 马达驱动刚体满足目标角速度。<br/>                      |
| 目标角度/速度（Target Angle/Speed）<br/> | 驱动的目标角度/速度。不同模式所暴露的字段不一样。<br/> |
| 强度（Strength）<br/>                    | 驱动强度。数值越大，刚体越容易到达目标角度/速度。<br/> |
| 阻尼（Damper）<br/>                      | 驱动阻尼，数值越小，刚体越快到达目标角度。<br/>        |

## 滑动约束（SliderJoint3D）

滑动约束只有一个位移自由度， 通常可以用来模拟类似滑轨这样的约束。

它默认 +X 轴为位移轴，在 Y/Z 轴上保持 0 相对位移。

![](https://docs.motphys.com/Images/SpgrbbGnGoXySOxzJJHcOSd0nue.gif)

![](https://docs.motphys.com/Images/LalabkWvyo2Ng1xhch4cabOBnkf.png)

| 名称<br/>                                   | 释义<br/>                                                                                                                                          |
| ------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| 连接刚体（Connect Body）<br/>               | 约束组件连接其所在刚体，与一个其他刚体。未连接其他刚体时，组件不生效。<br/>                                                                        |
| 锚点（Anchor Frame）<br/>                   | 锚点的局部坐标配置。<br/>                                                                                                                          |
| - 锚点位置（Anchor）<br/>                   | 相当于约束局部空间下的原点，一般可以与渲染几何中心对齐（0,0,0），也可以手动设置偏移量。<br/>                                                       |
| - 锚点旋转（Axis Rotation）<br/>            | 意味着旋转约束局部空间，约束默认以局部空间的 +X 轴为旋转轴。如果想让约束以世界空间 +Z 轴为旋转轴，可以设置为（0, -90, 0），均为“左手坐标系”。<br/> |
| 忽视碰撞（Ignore Collision）<br/>           | 当勾选时，约束连接的两个刚体将不会发生碰撞。<br/>                                                                                                  |
| 断裂力（Break Force）<br/>                  | 约束发生断裂所需要的力。`infinity` 表示不会发生断裂。<br/>                                                                                         |
| 断裂扭矩（Break Torque）<br/>               | 约束发生断裂所需要的扭矩。`infinity` 表示不会发生断裂。<br/>                                                                                       |
| 细节与自定义（Details）<br/>                | 一些额外的配置细节。<br/>                                                                                                                          |
| - 自动配置（Auto Configure Connected）<br/> | 是否自动配置连接刚体的锚点。当开启时，认为连接刚体的锚点与本锚点对齐。<br/>                                                                        |
| - 连接锚点（Connected Anchor Frame）<br/>   | 配置连接锚点，作用与锚点类似。<br/>                                                                                                                |

![](https://docs.motphys.com/Images/T6ORbdr02oaAZ5xkDyJcSZGcnLf.png)

| 名称<br/>                       | 释义<br/>                                                                                                 |
| ------------------------------- | --------------------------------------------------------------------------------------------------------- |
| 使用限度（Use Limit）<br/>      | 勾选时，将使用限度，将刚体的运动固定在一定范围。取消勾选时，范围为[float.MinValue, float.MaxValue]。<br/> |
| 位移距离（Distance Limit）<br/> | 刚体可以位移的距离范围。<br/>                                                                             |
| - 最小值（Low）<br/>            | 位移的最小距离。<br/>                                                                                     |
| - 最大值（High）<br/>           | 位移的最大距离。<br/>                                                                                     |
| 使用弹簧（Use Spring）<br/>     | 是否使用弹簧。取消勾选时，生效的弹簧刚度为无限，阻尼为 0。<br/>                                           |
| 弹簧阻尼（Limit Spring）<br/>   | 用于控制刚体的运动表现。<br/>                                                                             |
| - 刚度（Stiffness）<br/>        | 值越大，刚体可超出约束的范围越少。<br/>                                                                   |
| - 阻尼（Damper）<br/>           | 值越大，刚体超出约束范围时，受到的阻力越大。<br/>                                                         |
| 线性阻尼（Linear Damper）<br/>  | 值越大，刚体在运动时，位移阻力越大。<br/>                                                                 |

![](https://docs.motphys.com/Images/DGQwb4uGzorP4Lxo4yHcB2ibnWd.png)

| 名称<br/>                                   | 释义<br/>                                              |
| ------------------------------------------- | ------------------------------------------------------ |
| 使用马达（Use Motor）<br/>                  | 勾选时，将使用马达，否则不使用。<br/>                  |
| 马达模式（Motor Type）<br/>                 | 马达的一些基础模式。<br/>                              |
| - 距离驱动（Distance Drive）<br/>           | 马达驱动刚体满足目标距离。<br/>                        |
| - 速度驱动（Speed Drive）<br/>              | 马达驱动刚体间的距离以恒定速度变化。<br/>              |
| 目标距离/速度（Target Distance/Speed）<br/> | 驱动的目标距离/速度。不同模式所暴露的字段不一样。<br/> |
| 强度（Strength）<br/>                       | 驱动强度。数值越大，刚体越容易到达目标距离/速度。<br/> |
| 阻尼（Damper）<br/>                         | 驱动阻尼，数值越小，刚体越快到达目标距离。<br/>        |

## 万向约束（UniversalJoint3D）

万向约束是一种只有 2 个摆动旋转自由度的约束，相较于 **EllipsoidJoint3D**，它没有 `Twist` 自由度。一般用于传递 `Twist` 角度。

![](https://docs.motphys.com/Images/VWvCbNFReoDn7exAywRcrF6BnHg.gif)

![](https://docs.motphys.com/Images/JLKnb9y0zovgPcxsBr3ccMMRnLc.png)

| 名称<br/>                                   | 释义<br/>                                                                                                                                          |
| ------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| 连接刚体（Connect Body）<br/>               | 约束组件连接其所在刚体，与一个其他刚体。未连接其他刚体时，组件不生效。<br/>                                                                        |
| 锚点（Anchor Frame）<br/>                   | 锚点的局部坐标配置。<br/>                                                                                                                          |
| - 锚点位置（Anchor）<br/>                   | 相当于约束局部空间下的原点，一般可以与渲染几何中心对齐（0,0,0），也可以手动设置偏移量。<br/>                                                       |
| - 锚点旋转（Axis Rotation）<br/>            | 意味着旋转约束局部空间，约束默认以局部空间的 +X 轴为旋转轴。如果想让约束以世界空间 +Z 轴为旋转轴，可以设置为（0, -90, 0），均为“左手坐标系”。<br/> |
| 忽视碰撞（Ignore Collision）<br/>           | 当勾选时，约束连接的两个刚体将不会发生碰撞。<br/>                                                                                                  |
| 断裂力（Break Force）<br/>                  | 约束发生断裂所需要的力。`infinity` 表示不会发生断裂。<br/>                                                                                         |
| 断裂扭矩（Break Torque）<br/>               | 约束发生断裂所需要的扭矩。`infinity` 表示不会发生断裂。<br/>                                                                                       |
| 细节与自定义（Details）<br/>                | 一些额外的配置细节。<br/>                                                                                                                          |
| - 自动配置（Auto Configure Connected）<br/> | 是否自动配置连接刚体的锚点。当开启时，认为连接刚体的锚点与本锚点对齐。<br/>                                                                        |
| - 连接锚点（Connected Anchor Frame）<br/>   | 配置连接锚点，作用与锚点类似。<br/>                                                                                                                |

![](https://docs.motphys.com/Images/JBcUbV10noiXdhxrhH6crPdWndh.png)

| 名称<br/>                        | 释义<br/>                                 |
| -------------------------------- | ----------------------------------------- |
| 摆动范围 Y（Swing Limit Y）<br/> | 表示可以绕 +Y 轴摆动的范围。<br/>         |
| - 最小值（Low）<br/>             | 摆动的最小角度，取值范围[-180,180]。<br/> |
| - 最大值（High）<br/>            | 摆动的最大角度，取值范围[-180,180]。<br/> |
| 摆动范围 Y（Swing Limit Z）<br/> | 表示可以绕 +Z 轴摆动的范围。<br/>         |
| - 最小值（Low）<br/>             | 摆动的最小角度，取值范围[-180,180]。<br/> |
| - 最大值（High）<br/>            | 摆动的最大角度，取值范围[-180,180]。<br/> |
| 角度阻尼（Angular Damper）<br/>  | 值越大，刚体在运动时，旋转阻力越大。<br/> |

## 约束位置迭代次数（Num Pos Solver Iter）

在 Anchor 界面的 Details 中，用户可以设定这个约束的位置解算迭代次数。

![](https://docs.motphys.com/Images/OrC4b7HJ0oH303xHldjcDv6Snlg.png)

增加位置迭代次数可以让约束的表现更稳定。在一些复杂的场景中，求解器需要更多的迭代次数才能达到收敛。这里的位置迭代次数和引擎配置中的 [Motphys Unity SDK 用户手册](https://motphys.feishu.cn/wiki/BPJYwldXUi0N1MkagRecBLbCnpg#share-Wt3FdL5nioIba8x40pvcBbJCnXd)解算迭代次数是相同的配置，引擎会取两者中的最大值使用。

## 惯性张量缩放系数（Inertia Scale）

在 Anchor 界面的 Details 中，用户可以设定被约束物体的惯性张量缩放系数，这个参数需要是正数。

![](https://docs.motphys.com/Images/DjTFbTpxQoigzhxGL0bc5yslnph.png)

当约束表现不稳定时，除了增加迭代次数，还可以通过增大 inertia scale 来加速解算器收敛。增加 inertia scale 不会对性能产生显著影响，但这个参数本身是不符合物理的。在例如游戏等应用场景下，inertia scale 可以帮助用户实现看上去更“正确”的效果。
