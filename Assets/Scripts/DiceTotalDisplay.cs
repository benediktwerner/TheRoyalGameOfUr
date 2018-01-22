using UnityEngine;
using UnityEngine.UI;

public class DiceTotalDisplay : MonoBehaviour {
	void Start() {
		GameController.Game.DiceChangedEvent += OnDiceChanged;
	}

	void OnDiceChanged(int newValue) {
		GetComponent<Text>().text = "= " + newValue;
	}

	void Update() {
		if (!GameController.Instance.IsAnimating && GameController.Game.DiceValue == -1) {
			GetComponent<Text>().text = "= ?";
		}
	}
}
