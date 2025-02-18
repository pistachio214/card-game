using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerHand : Node2D
{
	private const uint HAND_COUNT = 8;
	private const string CARD_SCENE_PATH = "res://Scenes/Card.tscn";
	private const int CARD_WIDTH = 200;
	private const int HAND_Y_POSITION = 890;

	private float centerScreenX;

	private readonly List<Card> playerHand = new List<Card>();

	public override void _Ready()
	{
		// 当前屏幕宽中心
		centerScreenX = GetViewport().GetVisibleRect().Size.X / 2;

		CardManager cardManager = GetParent().GetNode<CardManager>("CardManager");
		PackedScene cardScene = GD.Load<PackedScene>(CARD_SCENE_PATH);

		for (int i = 0; i < HAND_COUNT; i++)
		{
			var newCard = cardScene.Instantiate<Card>();
			cardManager.AddChild(newCard);
			newCard.Name = "Card" + (i + 1).ToString();

			AddCardToHand(newCard);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// 卡片加入手牌数组
	public void AddCardToHand(Card card)
	{
		// 如果卡片不在手牌中,就加入手牌
		if (!playerHand.Contains(card))
		{
			playerHand.Insert(0, card);
			UpdateHandPositions();
		}
		// 卡片弹回原来位置
		else
		{
			AnimateCardToPosition(card, card.startingPosition);
		}

	}

	// 调整卡牌的位置顺序
	private void UpdateHandPositions()
	{
		for (int i = 0; i < playerHand.Count(); i++)
		{
			Vector2 newPosition = new Vector2(CalculateCardPosition(i), HAND_Y_POSITION);

			Card card = playerHand[i];
			card.startingPosition = newPosition;

			AnimateCardToPosition(card, newPosition);
		}
	}

	// 移动卡牌到指定位置
	private void AnimateCardToPosition(Card card, Vector2 newPosition)
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(card, "position", newPosition, 0.1);
	}

	// TODO 删除手牌中卡牌的位置
	public void RemoveCardFromHand(Card card)
	{
		// 检查该卡片是否在手牌中
		if (playerHand.Contains(card))
		{
			// List删除指定元素
			playerHand.Remove(card);

			UpdateHandPositions();
		}
	}

	// 根据索引来处理卡牌的位置
	private float CalculateCardPosition(int index)
	{
		int totalWidth = (playerHand.Count() - 1) * CARD_WIDTH;

		float xOffset = centerScreenX + index * CARD_WIDTH - totalWidth / 2;

		return xOffset;
	}
}
