using System.Collections;
using System.Linq;
using UnityEngine;

public class AITest : MonoBehaviour {
	
	GeneticAlgorithm geneticAlg;
	Game game;
	bool stop = false;

	void Start() {
		Application.runInBackground = true;
	}

	public void TestAI() {
		game = new Game();
		AIPlayer player1 = new AIPlayer_GeneticUtilityAI(
			game,
			new float[] {
				0.20f, // New stone
				0.30f, //0.10f, // Block roll again
				0.25f, //0.50f, // Score stone
				0.50f, // Roll again
				0.60f, // Kill stone
				0.70f, // Kill stone late bonus
				0.50f, // Danger late bonus
				0.2f,
				0.3f,
				0.2f,
				0.05f
			}
		);
		AIPlayer player2 = new AIPlayer_UtilityAI(game);
		int score = 0;
		for (int i = 0; i < 1000; i++) {
			if (Evaluate(player1, player2))
				score++;
		}
		Debug.Log("Winrate: " + score / 10f + " %");
	}

	bool Evaluate(AIPlayer player1, AIPlayer player2) {
		game.ResetGame();

		bool first = (Random.Range(0, 2) == 0);
		AIPlayer[] players = first ? new AIPlayer[] { player1, player2 } : new AIPlayer[] { player2, player1 };
		while (!game.IsGameOver())
			players[game.CurrPlayer].play();
		return game.Scores[first ? 0 : 1] > game.Scores[first ? 1 : 0];
	}

	public void RunTest() {
		geneticAlg = new GeneticAlgorithm();
		StartCoroutine("RunOnce");
	}

	IEnumerator RunOnce() {
		for (int i = 0; i < 100; i++) {
			Debug.Log(i);
			for (int j = 0; j < 10; j++) {
				geneticAlg.NextGeneration();
				if (stop)
					yield break;
				yield return null;
			}
		}
		float[] stats = geneticAlg.CalcStatistics();
		Debug.Log("Winrate: " + (stats[0] * 100) + " %");
		Debug.Log("Winning DNA:");
		Debug.Log(string.Join(",", stats.Skip(1).Select(f => f.ToString()).ToArray()));
	}

	public void Stop() {
		stop = true;
	}
}
