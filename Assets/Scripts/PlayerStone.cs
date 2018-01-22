using System.Collections.Generic;
using UnityEngine;

public class PlayerStone : MonoBehaviour {

	public int Player;
	public int Position { get; protected set; }
	public StoneStorage Home;

	Queue<Vector3> moveQueue = new Queue<Vector3>();
	Vector3 targetPosition;
	Vector3 velocity;
	bool isAnimating = false;

	public static float smoothTime = 0.2f;
	float smoothDistance = 0.05f;
	const float smoothHeight = 0.5f;
	Vector3 smoothUp = smoothHeight * Vector3.up;

	void Start() {
		targetPosition = transform.position;
	}


	void OnMouseUp() {
		if (Player != GameController.Game.CurrPlayer || GameController.Instance.IsAnimating)
			return;

		GameController.Instance.LastClickedStone = this;
		Move();
	}

	public void Move() {
		GameController.Game.MoveStone(Position);
	}

	public void MoveTo(int position) {
		transform.SetParent(null);
		isAnimating = true;
		GameController.Instance.AnimationsPlaying++;
		EnqueueMovesTo(position);
		Position = position;
	}

	public void MoveHome(bool animate = true) {
		for (int i = 0; i < Home.transform.childCount; i++) {
			Transform p = Home.transform.GetChild(i);
			if (p.childCount == 0) {
				// This placeholder is empty!
				Position = -1;
				transform.SetParent(p);
				if (animate) {
					targetPosition = transform.position + smoothUp;
					moveQueue.Enqueue(new Vector3(p.position.x, targetPosition.y, p.position.z));
					moveQueue.Enqueue(p.position);
					GameController.Instance.AnimationsPlaying++;
					isAnimating = true;
				}
				else {
					targetPosition = p.position;
					transform.position = p.position;
				}
				return;
			}
		}
		Debug.LogError("Stone could not move home, because no empty placeholder was found!");
	}

	void EnqueueMovesTo(int position) {
		bool moveUp = !IsPathFreeTo(position);
		for (int i = Position + 1; i <= position; i++) {
			moveQueue.Enqueue(GameController.Instance.GetTileTransform(Player, i).position + (moveUp ? smoothUp : Vector3.zero));
		}
		if (moveUp) {
			targetPosition = transform.position + smoothUp + (Position < 0 ? Vector3.up : Vector3.zero);
			if (position < Game.TILE_COUNT)
				moveQueue.Enqueue(GameController.Instance.GetTileTransform(Player, position).position);
		} else if (Position < 0) {
			Vector3 finalPosition = GameController.Instance.GetTileTransform(Player, position).position;
			targetPosition = new Vector3(transform.position.x, finalPosition.y, transform.position.z);
		} else {
			targetPosition = moveQueue.Dequeue();
		}
	}

	bool IsPathFreeTo(int position) {
		if (position >= Game.TILE_COUNT)
			return false;
		for (int i = Position + 1; i < position; i++)
			if (GameController.Game.Board[Player, i].PlayerStone != Tile.EMPTY)
				return false;
		return true;
	}

	void Update() {
		if (!isAnimating)
			return;
		
		if (Vector3.Distance(transform.position, targetPosition) < smoothDistance) {
			if (moveQueue.Count == 0) {
				GameController.Instance.AnimationsPlaying--;
				isAnimating = false;

				if (Position >= Game.TILE_COUNT)
					GetComponent<Rigidbody>().isKinematic = false;
				return;
			}

			targetPosition = moveQueue.Dequeue();
		} else {
			transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
		}
	}
}
