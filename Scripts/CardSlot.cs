using Godot;
using System;

public partial class CardSlot : Node2D
{
	public bool cardInSlot = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Area2D CardSlotArea2D = GetNode<Area2D>("Area2D");

		CardSlotArea2D.CollisionLayer = 2;
		CardSlotArea2D.CollisionMask = 2;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
