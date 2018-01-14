using UnityEngine;

public class StoneStorage : MonoBehaviour {
	
	public GameObject StonePrefab;

	void Start() {
		// Create one stone for each placeholder spot
		for (int i = 0; i < transform.childCount; i++) {
			GameObject newStone = Instantiate(StonePrefab);
			newStone.GetComponent<PlayerStone>().Home = this;
			newStone.GetComponent<PlayerStone>().MoveHome(animate: false);
		}
	}
}
