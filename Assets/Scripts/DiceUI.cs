using UnityEngine;
using UnityEngine.UI;

public class DiceUI : MonoBehaviour {

	public Sprite[] DiceImageOne;
	public Sprite[] DiceImageZero;

	void Start() {
		GameController.Game.DiceChangedEvent += OnDiceChanged;
	}

	void OnDiceChanged(int newValue) {
		int[] values = GetDiceValues(newValue);
		for (int i = 0; i < 4; i++) {
			if (values[i] == 0) {
				transform.GetChild(i).GetComponent<Image>().sprite = DiceImageZero[Random.Range(0, DiceImageZero.Length)];
			} else {
				transform.GetChild(i).GetComponent<Image>().sprite = DiceImageOne[Random.Range(0, DiceImageOne.Length)];                
			}
		}
	}

	private int[] GetDiceValues(int targetValue) {
		if (targetValue == 0)
			return new int[4];
		if (targetValue == 4)
			return new int[] { 1, 1, 1, 1 };
		
		while (true) {
			int total = 0;
			int[] result = new int[4];

			for (int i = 0; i < 4; i++) {
				result[i] = Random.Range(0, 2);
				total += result[i];
			}

			if (total == targetValue)
				return result;
		}
	}
}
