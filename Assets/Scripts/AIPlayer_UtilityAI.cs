using System.Collections.Generic;
using UnityEngine;

public class AIPlayer_UtilityAI : AIPlayer {
	protected float[] tileDanger;
	protected float[] DANGER_FACTOR = {0, 0.2f, 0.3f, 0.2f, 0.05f};
	public const float NEW_STONE = 0.2f;
	public const float BLOCK_ROLL_AGAIN = 0.3f;
	public const float SCORE_STONE = 0.25f;
	public const float ROLL_AGAIN = 0.5f;
	public const float KILL_STONE = 0.6f; // 0.5f
	public const float KILL_STONE_LATE_BONUS = 0.7f; // 0.5f
	public const float DANGER_LATE_BONUS = 0.5f;

	public AIPlayer_UtilityAI(Game game) : base(game) {}

	override protected int PickMove(int[] legalMoves) {
		CalcTileDanger();

		int bestMove = -10;
		float maxGoodness = float.NegativeInfinity;

		Debug.Log("Picking move with dice " + game.DiceValue);
		foreach (int move in legalMoves) {
			float g = GetMoveGoodness(move);
			Debug.Log("Move " + move + ": " + g);
			if (bestMove == -10 || g > maxGoodness) {
				bestMove = move;
				maxGoodness = g;
			}
		}

		return bestMove;
	}

	virtual protected void CalcTileDanger() {
		tileDanger = new float[Game.TILE_COUNT];

		for (int i = 0; i < tileDanger.Length; i++) {
			if (game.Board[game.OtherPlayer, i].PlayerStone != game.OtherPlayer)
				continue;

			for (int moves = 1; moves <= 4; moves++) {
				if (i + moves >= Game.TILE_COUNT)
					break;
				
				Tile t = game.Board[game.CurrPlayer, i + moves];

				if (t.IsSideline || t.IsRollAgain) {
					// This tile is not a danger zone, so we can ignore it.
					continue;
				}
                
				tileDanger[t.Position] += DANGER_FACTOR[moves];
			}
		}
	}

	virtual protected float GetMoveGoodness(int move) {
		float goodness = (move + 1) * 0.001f;
		Tile startTile = move < 0 ? null : game.Board[game.CurrPlayer, move];
		Tile endTile = move + game.DiceValue >= Game.TILE_COUNT ? null : game.Board[game.CurrPlayer, move + game.DiceValue];

		if (startTile == null) {
			// We aren't on the board yet, and it's always nice to add more to the board to open up more options.
			goodness += NEW_STONE;
		}

		if (startTile != null && (startTile.IsRollAgain == true && startTile.IsSideline == false)) {
			// We are sitting on a roll-again space in the middle. Let's resist moving just because
			// it blocks the space from our opponent
			goodness -= BLOCK_ROLL_AGAIN;
		}

		if (endTile == null) {
			goodness += SCORE_STONE;
		}
		else {
			if (endTile.IsRollAgain == true) {
				goodness += ROLL_AGAIN;
			}

			if (endTile.PlayerStone == game.OtherPlayer) {
				// There's an enemy stone to kill!
				goodness += KILL_STONE;
				if (endTile.Position > 7)
					goodness += (endTile.Position - 7) * 0.1f * KILL_STONE_LATE_BONUS;
			}
		}

		float startDanger = (startTile == null ? 0 : tileDanger[startTile.Position]);
		float endDanger = (endTile == null ? 0 : tileDanger[endTile.Position]);
		if (startTile != null && startTile.Position > 7)
			startDanger *= 1 + (startTile.Position - 7) * 0.25f * DANGER_LATE_BONUS;
		goodness += startDanger - endDanger;

		// TODO:  Add goodness for tiles that are behind enemies, and therefore likely to contribute to future boppage
		// TODO:  Add goodness for moving a stone forward when we might be blocking friendlies

		return goodness;
	}
}

