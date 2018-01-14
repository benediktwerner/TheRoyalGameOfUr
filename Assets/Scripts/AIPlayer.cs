using UnityEngine;
using System.Collections.Generic;

public class AIPlayer {

	protected Game game;

	public AIPlayer(Game game) {
		this.game = game;
	}
	
	public void play() {
		if (game.DiceValue == -1) {
			game.RollDice();
		}

		moveStone();
	}

	virtual protected void moveStone() {
		int[] legalMoves = game.GetLegalMoves();

		if (legalMoves.Length == 0)
			return;

		game.MoveStone(PickMove(legalMoves));
	}

	virtual protected int PickMove(int[] legalMoves) {
		return legalMoves[Random.Range(0, legalMoves.Length)];
	}
}

