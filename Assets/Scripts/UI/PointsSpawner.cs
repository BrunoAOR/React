using UnityEngine;
using System.Collections;

public class PointsSpawner : MonoBehaviour {

	public AnimatedUIText pointsPrefab;

	public void AnimatePoints (string points, Vector2 fromViewportPoint, Vector2 toViewportPoint) {
		AnimatedUIText pointsObj = (AnimatedUIText)Instantiate (pointsPrefab);
		pointsObj.transform.SetParent (transform);
		pointsObj.transform.localPosition = Vector3.zero;
		pointsObj.transform.localScale = Vector3.one;
		pointsObj.startAnchor = fromViewportPoint;
		pointsObj.targetAnchor = toViewportPoint;
		pointsObj.ApplyStartConditions ();
		pointsObj.SetDisplayText (points);
	}
}
