using System;
using System.Collections.Generic;

public class AIPlayer_GeneticUtilityAI : AIPlayer_UtilityAI {
	public float[] dna;
	public const int DNA_NEW_STONE = 0;
	public const int DNA_BLOCK_ROLL_AGAIN = 1;
	public const int DNA_SCORE_STONE = 2;
	public const int DNA_ROLL_AGAIN = 3;
	public const int DNA_KILL_STONE = 4;
	public const int DNA_KILL_STONE_LATE_BONUS = 5;
	public const int DNA_DANGER_LATE_BONUS = 6;
	public const int DNA_DANGER_FACTORS = 7;
	public const int DNA_LENGTH = 11;

	public AIPlayer_GeneticUtilityAI(Game game) : base(game) {
		dna = new float[DNA_LENGTH];
		Random random = new Random();
		for (int i = 0; i < DNA_LENGTH; i++) {
			dna[i] = (float) random.NextDouble();
		}
	}

	public AIPlayer_GeneticUtilityAI(Game game, float[] dna) : base(game) {
		this.dna = dna;
	}

	override protected void CalcTileDanger() {
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

				tileDanger[t.Position] += dna[DNA_DANGER_FACTORS + moves - 1];
			}
		}
	}

	override protected float GetMoveGoodness(int move) {
		float goodness = 0;
		Tile startTile = move < 0 ? null : game.Board[game.CurrPlayer, move];
		Tile endTile = move + game.DiceValue >= Game.TILE_COUNT ? null : game.Board[game.CurrPlayer, move + game.DiceValue];

		if (startTile == null) {
			// We aren't on the board yet, and it's always nice to add more to the board to open up more options.
			goodness += dna[DNA_NEW_STONE];
		}

		if (startTile != null && (startTile.IsRollAgain == true && startTile.IsSideline == false)) {
			// We are sitting on a roll-again space in the middle. Let's resist moving just because
			// it blocks the space from our opponent
			goodness -= dna[DNA_BLOCK_ROLL_AGAIN];
		}

		if (endTile == null) {
			goodness += dna[DNA_SCORE_STONE];
		}
		else {
			if (endTile.IsRollAgain == true) {
				goodness += dna[DNA_ROLL_AGAIN];
			}

			if (endTile.PlayerStone == game.OtherPlayer) {
				// There's an enemy stone to kill!
				goodness += dna[DNA_KILL_STONE];
				if (endTile.Position > 7)
					goodness += (endTile.Position - 7) * 0.1f * dna[DNA_KILL_STONE_LATE_BONUS];
			}
		}

		float startDanger = (startTile == null ? 0 : tileDanger[startTile.Position]);
		float endDanger = (endTile == null ? 0 : tileDanger[endTile.Position]);
		if (startTile != null && startTile.Position > 7)
			startDanger *= 1 + (startTile.Position - 7) * 0.25f * dna[DNA_DANGER_LATE_BONUS];
		goodness += startDanger - endDanger;

		return goodness;
	}
}
