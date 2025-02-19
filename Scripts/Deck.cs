using Godot;
using System;
using System.Collections.Generic;

public partial class Deck : Node2D
{

	private const float CARD_DRAW_SEEP = 0.2f;

	private PlayerHand _playerHand;

	private RichTextLabel _richTextLabel;

	private const string CARD_SCENE_PATH = "res://Scenes/Card.tscn";

	private List<string> _playerDeckList = new() {
		"Knight",
		"Knight",
		"Knight"
	};


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_playerHand = GetParent().GetNode<PlayerHand>("PlayerHand");
		_richTextLabel = GetNode<RichTextLabel>("RichTextLabel");
		_richTextLabel.BbcodeEnabled = true;
		_richTextLabel.Text = "[center]" + _playerDeckList.Count + "[/center]";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void DrawCard()
	{
		_playerDeckList.RemoveAt(0);

		if (_playerDeckList.Count == 0)
		{
			GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Disabled = true;
			GetNode<Sprite2D>("DeckImage").Visible = false;
			_richTextLabel.Visible = false;
		}

		_richTextLabel.Text = "[center]" + _playerDeckList.Count + "[/center]";

		CardManager cardManager = GetParent().GetNode<CardManager>("CardManager");
		PackedScene cardScene = GD.Load<PackedScene>(CARD_SCENE_PATH);

		var newCard = cardScene.Instantiate<Card>();
		cardManager.AddChild(newCard);
		newCard.Name = "Card";

		_playerHand.AddCardToHand(newCard, CARD_DRAW_SEEP);
	}
}
