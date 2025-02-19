using Godot;
using System;

using GodotCollectionsDictionary = Godot.Collections.Dictionary;

public partial class InputManager : Node2D
{
	// 声明左键点击和左键释放信号
	[Signal]
	public delegate void LeftMouseButtonClickedEventHandler();

	[Signal]
	public delegate void LeftMouseButtonReleasedEventHandler();

	public const uint COLLISION_MASK_CARD = 1;

	public const uint COLLISION_MASK_DECK = 4;

	private CardManager _cardManagerReference;

	private Deck _deckReference;

	public override void _Ready()
	{
		_cardManagerReference = GetParent().GetNode<CardManager>("CardManager");
		_deckReference = GetParent().GetNode<Deck>("Deck");
	}

	public override void _Input(InputEvent @event)
	{
		// 左键按下的时候，判断是否为卡牌，如果是卡牌就存入临时变量,如果不是就清空临时变量
		if (@event is InputEventMouseButton inputEvent && inputEvent.ButtonIndex == MouseButton.Left)
		{
			if (inputEvent.Pressed)
			{
				EmitSignal(nameof(LeftMouseButtonClicked));
				RaycastAtCursor();
			}
			else
			{
				EmitSignal(nameof(LeftMouseButtonReleased));
			}

		}
	}

	// 射线投射检测卡牌
	private void RaycastAtCursor()
	{

		PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;

		PhysicsPointQueryParameters2D parameters = new PhysicsPointQueryParameters2D
		{
			Position = GetGlobalMousePosition(),
			CollideWithAreas = true,
		};

		Godot.Collections.Array<GodotCollectionsDictionary> result = spaceState.IntersectPoint(parameters);

		if (result.Count > 0)
		{
			Area2D node = (Area2D)result[0]["collider"];

			uint resultCollisionMask = node.CollisionMask;

			if (resultCollisionMask == COLLISION_MASK_CARD)
			{
				Card cardFound = node.GetParent<Card>();
				if (cardFound != null)
				{
					_cardManagerReference.StartDrag(cardFound);
				}
			}

			if (resultCollisionMask == COLLISION_MASK_DECK)
			{
				_deckReference.DrawCard();
			}
		}
	}

}
