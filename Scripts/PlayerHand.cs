using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerHand : Node2D
{
	private const int CARD_WIDTH = 200;
	private const int HAND_Y_POSITION = 890;

	private const float DEFAULT_CARD_MOVE_SPEED = 0.1f;

	private float _centerScreenX;

	private readonly List<Card> _playerHandList = new List<Card>();

	public override void _Ready()
	{
		// 当前屏幕宽中心
		_centerScreenX = GetViewport().GetVisibleRect().Size.X / 2;


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// 卡片加入手牌数组
	public void AddCardToHand(Card card, float speed)
	{
		// 如果卡片不在手牌中,就加入手牌
		if (!_playerHandList.Contains(card))
		{
			_playerHandList.Insert(0, card);
			UpdateHandPositions(speed);
		}
		// 卡片弹回原来位置
		else
		{
			AnimateCardToPosition(card, card.startingPosition, DEFAULT_CARD_MOVE_SPEED);
		}

	}

	// 调整卡牌的位置顺序
	private void UpdateHandPositions(float speed)
	{
		for (int i = 0; i < _playerHandList.Count(); i++)
		{
			Vector2 newPosition = new Vector2(CalculateCardPosition(i), HAND_Y_POSITION);

			Card card = _playerHandList[i];
			card.startingPosition = newPosition;

			AnimateCardToPosition(card, newPosition, speed);
		}
	}

	// 移动卡牌到指定位置
	private void AnimateCardToPosition(Card card, Vector2 newPosition, float speed)
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(card, "position", newPosition, speed);
	}

	// TODO 删除手牌中卡牌的位置
	public void RemoveCardFromHand(Card card)
	{
		// 检查该卡片是否在手牌中
		if (_playerHandList.Contains(card))
		{
			// List删除指定元素
			_playerHandList.Remove(card);

			UpdateHandPositions(DEFAULT_CARD_MOVE_SPEED);
		}
	}

	// 根据索引来处理卡牌的位置
	private float CalculateCardPosition(int index)
	{
		int totalWidth = (_playerHandList.Count() - 1) * CARD_WIDTH;

		float xOffset = _centerScreenX + index * CARD_WIDTH - totalWidth / 2;

		return xOffset;
	}
}
