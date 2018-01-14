using System;
using System.Collections.Generic;

public class Game {
	public const int TILE_COUNT = 14;
	public const int STONES_COUNT = 6;

	public Tile[,] Board { get; protected set; }
	private int _currPlayer;
	private int _otherPlayer;
	public int CurrPlayer {
		get {return _currPlayer; }
		protected set { _currPlayer = value; _otherPlayer = (value + 1) % 2; }
	}
	public int OtherPlayer { get { return _otherPlayer; } }
	public int DiceValue { get; protected set; }
	public int[] Scores { get; protected set; }
	public int[] StonesAtHome { get; protected set; }

	Random random = new Random();

	public event System.Action<int> DiceChangedEvent;
	public event System.Action<int, int> StoneMovedEvent;
	public event System.Action<int> StoneKilledEvent;

	public Game() {
		Board = CreateBoard();
		ResetGame();
	}

	public int RollDice() {
		if (IsGameOver() || DiceValue >= 0)
			return -1;
		
		DiceValue = 0;
		for (int i = 0; i < 4; i++)
			DiceValue += random.Next(2);
		
		if (DiceChangedEvent != null)
			DiceChangedEvent(DiceValue);
		
		if (GetLegalMoves().Length == 0) {
			NextPlayer();
			// Debug.Log("No legal moves! Next Player");
		}
		return DiceValue;
	}

	public bool MoveStone(int position) {
		if (IsGameOver() || DiceValue <= 0 || position >= TILE_COUNT || !IsLegalMove(position, DiceValue))
			return false;

		int targetPosition = (position < 0 ? -1 : position) + DiceValue;
		if (StoneMovedEvent != null)
			StoneMovedEvent(position, targetPosition);

		if (position < 0) StonesAtHome[CurrPlayer]--;
		else Board[CurrPlayer, position].PlayerStone = Tile.EMPTY;
		
		if (targetPosition == TILE_COUNT) {
			Scores[CurrPlayer]++;
			NextPlayer();
			return true;
		}

		Tile targetTile = Board[CurrPlayer, targetPosition];
		if (targetTile.PlayerStone == OtherPlayer) {
			StonesAtHome[OtherPlayer]++;
			if (StoneKilledEvent != null)
				StoneKilledEvent(targetPosition);
		}
		targetTile.PlayerStone = CurrPlayer;

		if (targetTile.IsRollAgain)
			DiceValue = -1;
		else NextPlayer();
		return true;
	}

	public bool IsLegalMove(int position, int moves) {
		if (position >= 0 && Board[CurrPlayer, position].PlayerStone != CurrPlayer)
			return false;
		if (moves <= 0)
			return false;
		if (position < 0) {
			if (StonesAtHome[CurrPlayer] <= 0)
				return false;
			position = -1;
		}

		int targetPosition = position + moves;
		if (targetPosition == TILE_COUNT)
			return true;
		if (targetPosition > TILE_COUNT)
			return false;

		Tile targetTile = Board[CurrPlayer, targetPosition];
		return targetTile.PlayerStone == Tile.EMPTY || (targetTile.PlayerStone == OtherPlayer && !targetTile.IsRollAgain);
	}

	public int[] GetLegalMoves() {
		List<int> legalMoves = new List<int>();

		if (DiceValue == 0) {
			return legalMoves.ToArray();
		}

		for (int i = 0; i < TILE_COUNT; i++) {
			if (Board[CurrPlayer, i].PlayerStone == CurrPlayer && IsLegalMove(i, DiceValue))
				legalMoves.Add(i);
		}
		if (IsLegalMove(-1, DiceValue))
			legalMoves.Add(-1);

		return legalMoves.ToArray();
	}

	public bool IsGameOver() {
		return Scores[0] >= STONES_COUNT || Scores[1] >= STONES_COUNT;
	}

	private void NextPlayer() {
		DiceValue = -1;
		CurrPlayer = OtherPlayer;
	}

	private static Tile[,] CreateBoard() {
		var newBoard = new Tile[2, TILE_COUNT];
		for (int i = 0; i < TILE_COUNT; i++) {
			if (3 < i && i < 12) {
				var tile = new Tile(i, i == 7, false);
				newBoard[0, i] = tile;
				newBoard[1, i] = tile;
			}
			else {
				newBoard[0, i] = new Tile(i, i == 3 || i == 13, true);
				newBoard[1, i] = new Tile(i, i == 3 || i == 13, true);
			}
		}
		return newBoard;
	}

	public void ResetGame() {
		CurrPlayer = 0;
		Scores = new int[2];
		StonesAtHome = new int[] {STONES_COUNT, STONES_COUNT};
		DiceValue = -1;
		for (int i = 0; i < TILE_COUNT; i++) {
			Board[0, i].PlayerStone = Tile.EMPTY;
			Board[1, i].PlayerStone = Tile.EMPTY;
		}
	}
}
