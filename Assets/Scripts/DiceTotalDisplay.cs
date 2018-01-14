using UnityEngine;
using UnityEngine.UI;

public class DiceTotalDisplay : MonoBehaviour {
	void Start() {
		GameController.Game.DiceChangedEvent += OnDiceChanged;
	}

	void OnDiceChanged(int newValue) {
		if (newValue == -1) {
			GetComponent<Text>().text = "= ?";
		} else {
			GetComponent<Text>().text = "= " + newValue;
		}
	}
}
