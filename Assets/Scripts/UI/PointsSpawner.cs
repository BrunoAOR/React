using UnityEngine;
using System.Collections;

public class PointsSpawner : MonoBehaviour {

	public static PointsSpawner S;

	public AnimatedUIText pointsPrefab;
	public AnimatedUIText goodTimeBonus;
	public AnimatedUIText badTimeBonus;

	void Awake () {
		S = this;
	}

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

	public void AnimateGoodTimeBonus (string timeBonus, Vector2 viewportOrigin) {
		Vector2 viewportTarget = viewportOrigin;
		viewportTarget.y += 0.01f;
		AnimateTimeBonus (goodTimeBonus, timeBonus, viewportOrigin, viewportTarget); 
	}

	public void AnimateBadTimeBonus (string timeBonus, Vector2 viewportOrigin) {
		AnimateTimeBonus (badTimeBonus, timeBonus, viewportOrigin, viewportOrigin);
	}

	private void AnimateTimeBonus (AnimatedUIText bonusPrefab, string timeBonus, Vector2 fromViewportPoint, Vector2 toViewportPoint) {
		AnimatedUIText uiObj = (AnimatedUIText)Instantiate (bonusPrefab);
		uiObj.transform.SetParent (transform);
		uiObj.transform.localPosition = Vector3.zero;
		uiObj.transform.localScale = Vector3.one;
		uiObj.startAnchor = fromViewportPoint;
		uiObj.targetAnchor = toViewportPoint;
		uiObj.ApplyStartConditions ();
		uiObj.SetDisplayText (timeBonus);
	}



}
