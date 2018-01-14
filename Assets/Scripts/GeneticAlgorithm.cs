using System;
using System.Collections;

public class GeneticAlgorithm {

	const int MUTATION_RATE = 1;
	const int TOURNAMENT_SIZE = 3;
	const int GAMES_VS_OTHERS = 4;
	const int GAMES_VS_REFERENCE = 10;
	const int POPULATION_SIZE = 10;
	AIPlayer_GeneticUtilityAI[] population;
	AIPlayer referenceAI;
	Game game;
	Random random = new Random();

	public GeneticAlgorithm() {
		game = new Game();
		referenceAI = new AIPlayer_UtilityAI(game);

		population = new AIPlayer_GeneticUtilityAI[POPULATION_SIZE];
		for (int i = 0; i < POPULATION_SIZE; i++) {
			population[i] = new AIPlayer_GeneticUtilityAI(game);
		}
	}

	// First value is the winrate, after that the winning DNA
	public float[] CalcStatistics() {
		int bestAI = BestAI(Evaluate());
		int score = 0;
		bool first = true;
		referenceAI = new AIPlayer_UtilityAI(game);
		for (int i = 0; i < 100; i++) {
			if (Evaluate(bestAI, first))
				score++;
			first = !first;
		}

		float[] stats = new float[AIPlayer_GeneticUtilityAI.DNA_LENGTH + 1];
		stats[0] = score / 100f;
		for (int i = 0; i < AIPlayer_GeneticUtilityAI.DNA_LENGTH; i++) {
			stats[i+1] = population[bestAI].dna[i];
		}
		return stats;
	}

	public void NextGeneration() {
		int[] scores = Evaluate();
		float[][] newDNAs = new float[POPULATION_SIZE][];
		newDNAs[0] = population[BestAI(scores)].dna;
		for (int i = 1; i < POPULATION_SIZE; i++) {
			newDNAs[i] = Mutate(Crossover(Select(scores), Select(scores)));
		}
		for (int i = 0; i < POPULATION_SIZE; i++)
			population[i].dna = newDNAs[i];
	}

	private int[] Evaluate() {
		int[] scores = new int[POPULATION_SIZE];
		bool first = true;
		for (int i = 0; i < POPULATION_SIZE; i++) {
			for (int j = 0; j < GAMES_VS_REFERENCE; j++) {
				if (Evaluate(i, first))
					scores[i]++;
				first = !first;
			}
			for (int j = 0; j < GAMES_VS_OTHERS; j++) {
				for (int vs = i; vs < POPULATION_SIZE; vs++) {
					if (Evaluate(i, first, vs))
						scores[i]++;
					else
						scores[vs]++;
					first = !first;
				}
			}
		}
		return scores;
	}

	private bool Evaluate(int aiIndex, bool first = true, int vsIndex = -1) {
		AIPlayer ai = population[aiIndex];
		AIPlayer opponent = vsIndex == -1 ? referenceAI : population[vsIndex];
		game.ResetGame();

		AIPlayer[] players = first ? new AIPlayer[] { ai, opponent } : new AIPlayer[] { opponent, ai };
		while (!game.IsGameOver())
			players[game.CurrPlayer].play();
		return game.Scores[first ? 0 : 1] > game.Scores[first ? 1 : 0];
	}

	private int BestAI(int[] scores) {
		int bestAI = 0;
		int bestScore = 0;
		for (int i = 1; i < scores.Length; i++) {
			if (scores[i] > bestScore) {
				bestAI = i;
				bestScore = scores[i];
			}
		}
		return bestAI;
	}

	private float[] Select(int[] scores) {
		int bestAI = 0;
		int bestScore = 0;
		for (int i = 0; i < TOURNAMENT_SIZE; i++) {
			int ai = random.Next(scores.Length);
			if (scores[ai] > bestScore) {
				bestAI = ai;
				bestScore = scores[ai];
			}
		}
		return population[bestAI].dna;
	}

	private float[] Crossover(float[] a, float[] b) {
		int crossPoint = random.Next(2, AIPlayer_GeneticUtilityAI.DNA_LENGTH - 2);
		float[] result = new float[AIPlayer_GeneticUtilityAI.DNA_LENGTH];
		for (int i = 0; i < result.Length; i++)
			result[i] = (i < crossPoint) ? a[i] : b[i];
		return result;
	}

	private float[] Mutate(float[] dna) {
		for (int i = 0; i < dna.Length; i++) {
			if (random.Next(0, 100) < MUTATION_RATE)
				dna[i] = (float) random.NextDouble();
		}
		return dna;
	}
}

