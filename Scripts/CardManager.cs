using Godot;
using System;

using GodotCollectionsDictionary = Godot.Collections.Dictionary;

public partial class CardManager : Node2D
{
	public const uint COLLISION_MASK_CARD = 1;

	public const uint COLLISION_MASK_CARD_SLOT = 2;

	private Card cardBeingDragged;

	private Vector2 screenSize;

	private bool isHoveringOnCard;

	private PlayerHand playerHandReference;

	public override void _Ready()
	{
		screenSize = GetViewportRect().Size;
		playerHandReference = GetParent().GetNode<PlayerHand>("PlayerHand");
	}

	public override void _Process(double delta)
	{
		// 左键点击的是卡片，就把鼠标位置给到卡牌，让卡牌随着鼠标移动
		if (cardBeingDragged != null)
		{
			Vector2 mousePosition = GetGlobalMousePosition();

			// 为了使拖动不超出屏幕范围，让最大值不超过屏幕范围尺寸
			cardBeingDragged.Position = new Vector2(
				Clamp(mousePosition.X, 0, screenSize.X), Clamp(mousePosition.Y, 0, screenSize.Y)
			);
		}
	}

	public override void _Input(InputEvent @event)
	{
		// 左键按下的时候，判断是否为卡牌，如果是卡牌就存入临时变量,如果不是就清空临时变量
		if (@event is InputEventMouseButton inputEvent && inputEvent.ButtonIndex == MouseButton.Left)
		{
			if (inputEvent.Pressed)
			{
				Node2D cardNode = RaycastCheckForCard();
				if (cardNode != null)
				{
					StartDrag(cardNode);
				}
			}
			else
			{
				if (cardBeingDragged != null)
				{
					FinishDrag();
				}

			}

		}
	}

	// 链接来自Card的信号，绑定到卡片管理器中
	public void ConnectCardSignals(Card card)
	{
		card.Hovered += OnHoveredOverCard;
		card.HoveredOff += OnHoveredOffCard;
	}

	private void OnHoveredOverCard(Card card)
	{
		if (!isHoveringOnCard)
		{
			isHoveringOnCard = true;
			HighlightCard(card, true);
		}
	}

	private void OnHoveredOffCard(Card card)
	{
		if (cardBeingDragged == null)
		{
			HighlightCard(card, false);
			// 判断卡片是否在另一个卡片之上
			Node2D newCardHovered = RaycastCheckForCard();
			if (newCardHovered != null)
			{
				HighlightCard(newCardHovered, false);
			}
			else
			{
				isHoveringOnCard = false;
			}
		}

	}

	// 开始拖动卡牌
	private void StartDrag(Node2D card)
	{
		cardBeingDragged = (Card)card;
		card.Scale = new Vector2(1f, 1f);
	}

	// 结束拖动卡牌
	private void FinishDrag()
	{
		cardBeingDragged.Scale = new Vector2(1.05f, 1.05f);

		CardSlot cardSlotFound = RaycastCheckForCardSlot();

		if (cardSlotFound != null && !cardSlotFound.CardInSlot)
		{
			playerHandReference.RemoveCardFromHand(cardBeingDragged);

			cardBeingDragged.Position = new Vector2(
				x: cardSlotFound.Position.X,
				y: cardSlotFound.Position.Y
			);
			cardBeingDragged.GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Disabled = true;
			cardSlotFound.CardInSlot = true;
		}
		else
		{
			playerHandReference.AddCardToHand(cardBeingDragged);
		}

		cardBeingDragged = null;
	}

	// 高亮卡片
	private static void HighlightCard(Node2D card, bool hovered)
	{
		if (hovered)
		{
			card.Scale = new Vector2(1.05f, 1.05f);
			card.ZIndex = 2;
		}
		else
		{
			card.Scale = new Vector2(1f, 1f);
			card.ZIndex = 1;
		}
	}

	// 射线投射检测卡牌
	private Node2D RaycastCheckForCard()
	{
		PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;

		PhysicsPointQueryParameters2D parameters = new PhysicsPointQueryParameters2D
		{
			Position = GetGlobalMousePosition(),
			CollideWithAreas = true,
			CollisionMask = COLLISION_MASK_CARD
		};

		Godot.Collections.Array<GodotCollectionsDictionary> result = spaceState.IntersectPoint(parameters);

		if (result.Count > 0)
		{
			return GetCardWithHighestZIndex(result);
		}

		return null;
	}

	// 射线投射检测卡槽
	private CardSlot RaycastCheckForCardSlot()
	{
		PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;

		PhysicsPointQueryParameters2D parameters = new PhysicsPointQueryParameters2D
		{
			Position = GetGlobalMousePosition(),
			CollideWithAreas = true,
			CollisionMask = COLLISION_MASK_CARD_SLOT
		};

		Godot.Collections.Array<GodotCollectionsDictionary> result = spaceState.IntersectPoint(parameters);

		if (result.Count > 0)
		{
			Area2D node = (Area2D)result[0]["collider"];
			return node.GetParent<CardSlot>();
		}

		return null;
	}

	// 获取最高ZIndex的卡片
	private static Node2D GetCardWithHighestZIndex(Godot.Collections.Array<GodotCollectionsDictionary> cars)
	{
		Area2D node = (Area2D)cars[0]["collider"];

		Node2D highestZCard = node.GetParent<Node2D>();
		int highestZIndex = highestZCard.ZIndex;

		int count = cars.Count;

		if (count > 1)
		{
			for (int i = 1; i < count; i++)
			{
				Area2D currentNode = (Area2D)cars[i]["collider"];
				Node2D currentNode2D = currentNode.GetParent<Node2D>();

				if (currentNode2D is Card currentCard)
				{
					if (currentCard.ZIndex > highestZIndex)
					{
						highestZCard = currentCard;
						highestZIndex = currentCard.ZIndex;
					}
				}
			}
		}

		return highestZCard;
	}


	// 实现类似GDScript中的clamp方法
	public static float Clamp(float value, float min, float max)
	{
		return Mathf.Max(min, Mathf.Min(max, value));
	}
}
