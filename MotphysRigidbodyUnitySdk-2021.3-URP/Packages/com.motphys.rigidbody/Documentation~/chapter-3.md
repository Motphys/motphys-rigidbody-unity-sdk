# 运行物理

当导入了 `Motphys` 包后，运行游戏，场景里会自动创建 `PhysicsUpdater` 游戏对象，进行物理模拟。

![](https://docs.motphys.com/Images/EMXtbUgA7ojANDxoac3cSN6Dn4g.png)

## 配置引擎参数

特别的，我们可以在 `Edit/Project Settings/Motphys` 中配置引擎运行参数。

![](https://docs.motphys.com/Images/ELy3b7zqwosV04xkDMCc51g6nUg.png)

| 名称<br/>                                          | 释义<br/>                                                                                                                                         |
| -------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------- |
| 粗检测模式（BroadPhase Type）<br/>                 | 粗检测阶段所使用的模式，现在只提供了 `GridSAP`。<br/><br/>                                                                                        |
| - 网格配置（Grid Config）<br/>                     | 同一场景下，不同的网格配置可能会得到不同的性能。<br/>                                                                                             |
| - 自动世界包围盒（Auto World Aabb）<br/>           | 勾选时，会尝试自动重建世界包围盒。否则，将保持初始配置。<br/>                                                                                     |
| - 世界包围盒最小/最大角（World Aabb Min/Max）<br/> | 世界包围盒的最小角与最大角，两个边角构成一个 Aabb 包围盒。一般的，场景中的刚体的坐标不建议超出这个范围。<br/>                                     |
| - X 轴网格数（Num Cells In X）<br/>                | x 轴向上划分的网格数量。<br/>                                                                                                                     |
| - Z 轴网格数（Num Cells In Z）<br/>                | z 轴向上划分的网格数量。<br/>                                                                                                                     |
| 子步次数（Num Substep）<br/>                       | 一次物理解算迭代中，子步的次数。数量越高，解算结果越稳定，耗时越多。<br/>                                                                         |
| 解算迭代次数（Num Solver Iter）<br/>               | 一次子步中的迭代次数。迭代数量越高，解算结果越稳定，耗时越多。<br/><br/>                                                                          |
| 速度解算迭代次数（Num Solver Velocity Iter）<br/>  | 一次子步中，速度解算的迭代次数。迭代数量越高，解算结果越稳定，耗时越多。<br/>                                                                     |
| 重力（Gravity）<br/>                               | 物理引擎中的全局重力。默认（0,-9.8,0）。<br/>                                                                                                     |
| 模拟模式（Simulation Mode）<br/>                   | 物理引擎进行模拟调用的方式。<br/>                                                                                                                 |
| - Fixed Update<br/>                                | 在每次 Unity 的 `FixedUpdate` 中进行模拟。<br/>                                                                                                   |
| - Update<br/>                                      | 在每次 Unity 的 `Update` 中进行模拟。<br/>                                                                                                        |
| - Script<br/>                                      | 使用自定义代码，手动调用物理模拟。<br/>                                                                                                           |
| 启用场景查询（Enable Scene Query）<br/>            | 是否启用场景查询。当禁用时，射线检测（Raycast）与重叠检测（OverlapTest）将失效，可以获得更好的性能。<br/>默认开启。<br/>                          |
| 启用碰撞事件（Enable Contact Event）<br/>          | 是否启用碰撞事件。当禁用时，碰撞事件失效，意味着不再会有 `OnCollisionEnter` 等回调函数触发。<br/>触发器事件恒有效。<br/>默认开启。<br/>           |
| 允许扩大推测边缘（Allow Expand Speculative）<br/>  | 如果开启，引擎会扩大所有碰撞体的推测边缘，便于更大范围的连续碰撞检测（CCD）<br/>                                                                  |
| 日志级别（Log Level Filter）<br/>                  | 决定哪些引擎内部日志类型会被输出到 Unity Console 中。<br/>                                                                                        |
| 碰撞矩阵（Collision Matrix）<br/>                  | 碰撞矩阵，决定不同游戏层级（Layer）之间是否发生碰撞。默认所有层级之间都能碰撞。<br/>针对特定的层级之间取消勾选后，这两者之间将不会发生碰撞。<br/> |
