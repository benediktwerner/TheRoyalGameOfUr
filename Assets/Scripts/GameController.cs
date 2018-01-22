using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour {

	public static GameController Instance { get; protected set; }
	public static Game Game { get { return Instance._game; } }
	Game _game;

	public Transform[] Player1Tiles;
	public Transform[] Player2Tiles;

	AIPlayer[] players;

	public enum PlayerType {
		Human, BasicAI, UtilityAI
	}
	public PlayerType player1Type;
	public PlayerType player2Type;

	public float animationSpeed = 0.2f;

	[HideInInspector]
	public int AnimationsPlaying = 0;
	public bool IsAnimating { get { return AnimationsPlaying != 0; } }

	public GameController () {
		_game = new Game();
		Instance = this;

	}

	void Start() {
		Application.runInBackground = true;
		_game.StoneMovedEvent += OnStoneMoved;
		_game.StoneKilledEvent += OnStoneKilled;

		players = new AIPlayer[] {
			GetPlayerFromPlayerType(player1Type),
			GetPlayerFromPlayerType(player2Type)
		};

		PlayerStone.smoothTime = animationSpeed;
	}


	float timeUntilNextAiMove = 0;
	const float TIME_BETWEEN_AI_MOVES = 0.1f;

	void Update() {
		if (!IsAnimating && IsAiPlayer() && timeUntilNextAiMove <= Time.time && !_game.IsGameOver()) {
			players[_game.CurrPlayer].play();
			timeUntilNextAiMove = Time.time + TIME_BETWEEN_AI_MOVES;
		}
	}

	public void RollDice() {
		if (IsAiPlayer())
			return;

		_game.RollDice();
	}

	[HideInInspector]
	public PlayerStone LastClickedStone = null;

	void OnStoneMoved(int startPosition, int endPosition) {
		if (LastClickedStone == null)
			LastClickedStone = FindObjectsOfType<PlayerStone>()
				.FirstOrDefault(stone => stone.Position == startPosition && stone.Player == _game.CurrPlayer);
		
		if (LastClickedStone != null)
			LastClickedStone.MoveTo(endPosition);
		else Debug.LogError("No stone to move from " + startPosition + " to " + endPosition);
		LastClickedStone = null;
	}

	void OnStoneKilled(int position) {
		PlayerStone stoneToMove = FindObjectsOfType<PlayerStone>()
			.FirstOrDefault(stone => stone.Position == position && stone.Player == _game.OtherPlayer);

		if (stoneToMove != null)
			stoneToMove.MoveHome();
		else Debug.LogError("No stone to kill at " + position);
	}

	public Transform GetTileTransform(int player, int position) {
		if (player == 0)
			return Player1Tiles[position];
		return Player2Tiles[position];
	}

	public bool IsAiPlayer() {
		return players[_game.CurrPlayer] != null;
	}

	AIPlayer GetPlayerFromPlayerType(PlayerType playerType) {
		switch (playerType) {
			case PlayerType.BasicAI:
				return new AIPlayer(_game);
			case PlayerType.UtilityAI:
				return new AIPlayer_UtilityAI(_game);
			default:
				return null;
		}
	}
}
