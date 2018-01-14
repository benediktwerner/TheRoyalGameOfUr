using UnityEngine;

public class CameraDolly : MonoBehaviour {

	public float PivotAngle = 35f;
	float pivotVelocity;

	void Update() {
		if (GameController.Instance.IsAiPlayer() || GameController.Instance.IsAnimating)
			return;
		
		float angle = transform.rotation.eulerAngles.y;
		if (angle > 180)
			angle -= 360f;
		
		float targetAngle = PivotAngle;
		if (GameController.Game.IsGameOver())
			targetAngle = 0;
		else if (GameController.Game.CurrPlayer == 1)
			targetAngle = -PivotAngle;

		angle = Mathf.SmoothDamp(
			angle, 
			targetAngle, 
			ref pivotVelocity, 
			0.25f
		);

		transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
	}
}
