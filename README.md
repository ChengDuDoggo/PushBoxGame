# PushBoxGame
极简推箱子小游戏
根据自己的需求扩展游戏关卡、UI、音乐和动画内容。

## 功能特性

- 多个关卡支持
- 关卡重新开始和撤销功能
- 关卡通关判断
- 简单易用的关卡编辑

## 文件结构
PushBoxGamePage.cs：主游戏脚本，处理关卡加载、玩家移动、撤销、重新开始等功能。
BoxSlot.cs：格子脚本，定义了格子的属性和状态。
Player.cs：玩家脚本，处理玩家移动逻辑。
Box.cs：箱子脚本，目前为空，可以根据需要扩展。

## 使用说明

主游戏脚本（PushBoxGamePage.cs）
OnInit()：初始化按钮监听器并加载第一个关卡。
LoadLevel(int levelIndex)：加载指定关卡。
JudgmentPass()：判断当前关卡是否通过。
Undo()：撤销上一步操作。
RestartLevel()：重新开始当前关卡。
SaveGameState()：保存当前游戏状态。
RestoreGameState(GameState gameState)：恢复指定游戏状态。

格子脚本（BoxSlot.cs）
SlotType：格子类型枚举（空格子、墙、路、目标）。
BoxSlotCoordinates：格子坐标。
IsHaveBox：当前格子是否放着箱子。
IsHavePlayer：当前格子是否站着玩家。
IsFinishTargetSlot：当前格子是否是完成状态（箱子推进了目标格子）。

玩家脚本（Player.cs）
Move(int x1Coordinates, int x2Coordinates, int y1Coordinates, int y2Coordinates)：玩家实际移动函数，根据输入的坐标变化参数移动玩家。
