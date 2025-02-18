using Godot;
using System;

public partial class Card : Node2D
{
	// 创建自定义的两个信号,分别是鼠标进入和鼠标离开
	[Signal]
	public delegate void HoveredEventHandler(Card card);
	[Signal]
	public delegate void HoveredOffEventHandler(Card card);

	public Vector2 startingPosition;

	public override void _Ready()
	{
		// 进入卡片就把卡片信号连接到卡片管理器(CardManager)
		GetParent<CardManager>().ConnectCardSignals(this);
	}

	public override void _Process(double delta)
	{
	}

	// 鼠标进入Area2D(Card)就被触发
	public void OnAreaMouseEntered()
	{
		// Area2D的 mouse_entered 被触发的时候，就把信号通过自定义信号 Hovered 发送到 卡片管理器(CardManager)
		// 由于自定义信号强制使用EventHandler结尾,所以发送的时候要去掉EventHandler
		EmitSignal(nameof(Hovered), this);
	}

	// 鼠标离开Area2D(Card)就被触发
	public void OnAreaMouseExited()
	{
		EmitSignal(nameof(HoveredOff), this);
	}
}
