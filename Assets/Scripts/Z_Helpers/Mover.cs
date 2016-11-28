using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {
	public Vector3 startPosition = Vector3.zero;
	public Vector3 targetPosition = Vector3.up;
	public float moveTime = 1f;


	public void MoveToTarget (bool shouldStartAtCurrentPosition) {
		MoveToTarget (shouldStartAtCurrentPosition, targetPosition);
	}


	public void MoveToTarget (bool shouldStartAtCurrentPosition, Vector3 targetPos) {
		StartCoroutine (MoveToTargetCoroutine (shouldStartAtCurrentPosition));
	}


	public IEnumerator MoveToTargetCoroutine (bool shouldStartAtCurrentPosition) {
		yield return (MoveToTargetCoroutine (shouldStartAtCurrentPosition, targetPosition));
	}


	public IEnumerator MoveToTargetCoroutine (bool shouldStartAtCurrentPosition, Vector3 targetPos) {

		if (moveTime == 0)
			yield break;

		Vector3 startPos;
		if (shouldStartAtCurrentPosition) {
			startPos = transform.position;
		} else {
			startPos = startPosition;
		}
			
		Vector3 endPos = targetPos;

		float startTime = Time.time;
		float u;

		Vector3 currentPos;

		while ((Time.time - startTime) <= moveTime) {
			u = (Time.time - startTime) / moveTime;
			currentPos = Vector3.LerpUnclamped (startPos, endPos, u);
			transform.position = currentPos;
			yield return null;
		}

		transform.position = endPos;
	}

}
