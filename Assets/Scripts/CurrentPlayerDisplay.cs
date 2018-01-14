using UnityEngine;
using UnityEngine.UI;

public class CurrentPlayerDisplay : MonoBehaviour {

	Text currPlayerText;

	readonly string[] numberWords = { "White", "Red" };

	void Start() {
		currPlayerText = GetComponent<Text>();
	}

	void Update() {
		currPlayerText.text = "Current Player: " + numberWords[GameController.Game.CurrPlayer];
	}
}
