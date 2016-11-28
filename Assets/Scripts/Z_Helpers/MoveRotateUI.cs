using UnityEngine;
using System.Collections;

[RequireComponent (typeof (RectTransform))]
public class MoveRotateUI : MonoBehaviour {

	public enum ActionOrder {
		MoveAndRotate,
		MoveThenRotate,
		RotateThenMove,
		MoveOnly,
		RotateOnly
	}

	public enum ActionDirection {
		StartToTarget,
		TargetToStart,
		CurrentToTarget,
		CurrentToStart
	}

	[Space (10)]
	public RectTransform childImage;

	[Header ("Move Options")]
	public float moveDuration = 1f;
	public Vector2 startAnchoredPos;
	public Vector2 targetAnchoredPos;

	[Header ("Rotate Options")]
	public bool shouldCounterRotateChild = true;
	public float rotateDuration = 1f;
	public Vector3 startRotation;
	public Vector3 targetRotation;

	private RectTransform parentPivot;
	private Quaternion childStartRotation;


	void Awake () {
		parentPivot = GetComponent<RectTransform> ();
		childStartRotation = Quaternion.identity;
	}


	void Start () {
		SnapToStart ();
	}


	public void SnapToStart () {
		childImage.anchoredPosition = startAnchoredPos;
		parentPivot.localRotation = Quaternion.Euler(startRotation);
		childImage.rotation = childStartRotation;
	}


	public IEnumerator MoveRotate () {
		yield return (MoveRotate (ActionOrder.MoveThenRotate, ActionDirection.StartToTarget, true));
	}


	public IEnumerator MoveRotate (ActionOrder actionOrder, ActionDirection actionDirection, bool shouldSnapToStart) {
		if (shouldSnapToStart)
			SnapToStart ();

		Vector2 fromAnchorPos = Vector3.zero;
		Vector2 toAnchorPos = Vector3.zero;
		Quaternion fromRot = Quaternion.identity;
		Quaternion toRot = Quaternion.identity;

		switch (actionDirection) {
		case ActionDirection.StartToTarget:
			fromAnchorPos = startAnchoredPos;
			toAnchorPos = targetAnchoredPos;
			fromRot = Quaternion.Euler (startRotation);
			toRot = Quaternion.Euler (targetRotation);
			break;
		case ActionDirection.TargetToStart:
			fromAnchorPos = targetAnchoredPos;
			toAnchorPos = startAnchoredPos;
			fromRot = Quaternion.Euler (targetRotation);
			toRot = Quaternion.Euler (startRotation);
			break;
		case ActionDirection.CurrentToStart:
			fromAnchorPos = childImage.anchoredPosition;
			toAnchorPos = startAnchoredPos;
			fromRot = parentPivot.localRotation;
			toRot = Quaternion.Euler (startRotation);
			break;
		case ActionDirection.CurrentToTarget:
			fromAnchorPos = childImage.anchoredPosition;
			toAnchorPos = targetAnchoredPos;
			fromRot = parentPivot.localRotation;
			toRot = Quaternion.Euler (targetRotation);
			break;
		}

		switch (actionOrder) {
		case ActionOrder.MoveAndRotate:
			if (moveDuration < rotateDuration) {	// Rotate last longer, so weĺl yield for that
				StartCoroutine (Move (fromAnchorPos, toAnchorPos));
				yield return (Rotate (fromRot, toRot));
			} else {	// Move last longer, so weĺl yield for that
				StartCoroutine (Rotate (fromRot, toRot));
				yield return (Move (fromAnchorPos, toAnchorPos));
			}
			break;
		case ActionOrder.MoveThenRotate:
			yield return (Move (fromAnchorPos, toAnchorPos));
			yield return (Rotate (fromRot, toRot));
			break;
		case ActionOrder.RotateThenMove:
			yield return (Rotate (fromRot, toRot));
			yield return  (Move (fromAnchorPos, toAnchorPos));
			break;
		case ActionOrder.MoveOnly:
			yield return (Move (fromAnchorPos, toAnchorPos));
			break;
		case ActionOrder.RotateOnly:
			yield return (Rotate (fromRot, toRot));
			break;
		}

		yield break;
	}


	private IEnumerator Move (Vector3 fromPosition, Vector3 toPosition) {
		
		float timeStart = Time.time;
		float u;
		float u2;

		while ((Time.time - timeStart) < moveDuration) {
			u = (Time.time - timeStart) / moveDuration;

			if ( u <= 0.5f ) {
				u2 = 0.5f * Mathf.Pow( u*2, 2 );
			} else {
				u2 = 0.5f + 0.5f * (  1 - Mathf.Pow( 1-(2*(u-0.5f)), 2 )  );
			}

			childImage.anchoredPosition = Vector2.LerpUnclamped (fromPosition, toPosition, u2);
			yield return null;
		}
		childImage.anchoredPosition = toPosition;
	}


	private IEnumerator Rotate (Quaternion fromRotation, Quaternion toRotation) {

		float timeStart = Time.time;
		float u;
		float u2;

		while ((Time.time - timeStart) < rotateDuration) {
			u = (Time.time - timeStart) / rotateDuration;

			if ( u <= 0.5f ) {
				u2 = 0.5f * Mathf.Pow( u*2, 2 );
			} else {
				u2 = 0.5f + 0.5f * (  1 - Mathf.Pow( 1-(2*(u-0.5f)), 2 )  );
			}

			parentPivot.localRotation = Quaternion.SlerpUnclamped (fromRotation, toRotation, u2);

			if (shouldCounterRotateChild) {
				childImage.rotation = childStartRotation;
			}
			yield return null;
		}
		parentPivot.localRotation = toRotation;
	}

}
