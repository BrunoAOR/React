using UnityEngine;
using System.Collections;

public class PointsSpawner : MonoBehaviour {

	public static PointsSpawner S;

	public AnimatedUIText pointsPrefab;
	public AnimatedUIText goodTimeBonusPrefab;
	public AnimatedUIText badTimeBonusPrefab;

	public Transform pointsPoolTransform;
	public Transform goodTimeBonusPoolTransform;
	public Transform badTimeBonusPoolTransform;

	private ComponentPool<AnimatedUIText> pointsPool;
	private ComponentPool<AnimatedUIText> goodTimeBonusPool;
	private ComponentPool<AnimatedUIText> badTimeBonusPool;

	void Awake () {
		S = this;
		pointsPool = new ComponentPool<AnimatedUIText> (pointsPrefab, pointsPoolTransform);
		goodTimeBonusPool = new ComponentPool<AnimatedUIText> (goodTimeBonusPrefab, goodTimeBonusPoolTransform);
		badTimeBonusPool = new ComponentPool<AnimatedUIText> (badTimeBonusPrefab, badTimeBonusPoolTransform);
	}

	public void ClearPools () {
		badTimeBonusPool.ClearPool ();
	}

	public void AnimatePoints (string points, Vector2 fromViewportPoint, Vector2 toViewportPoint) {
		AnimatedUIText pointsObj = pointsPool.Spawn ();
		pointsObj.transform.SetParent (transform, false);
		pointsObj.transform.localPosition = Vector3.zero;
		pointsObj.startAnchor = fromViewportPoint;
		pointsObj.targetAnchor = toViewportPoint;
		pointsObj.ApplyStartConditions ();
		pointsObj.SetDisplayText (points);
	}

	public void AnimateGoodTimeBonus (string timeBonus, Vector2 viewportOrigin) {
		Vector2 viewportTarget = viewportOrigin;
		viewportTarget.y += 0.01f;
		AnimateTimeBonus (goodTimeBonusPool, timeBonus, viewportOrigin, viewportTarget); 
	}

	public void AnimateBadTimeBonus (string timeBonus, Vector2 viewportOrigin) {
		AnimateTimeBonus (badTimeBonusPool, timeBonus, viewportOrigin, viewportOrigin);
	}

	private void AnimateTimeBonus (ComponentPool<AnimatedUIText> sourcePool, string timeBonus, Vector2 fromViewportPoint, Vector2 toViewportPoint) {
		AnimatedUIText uiObj = sourcePool.Spawn ();
		uiObj.transform.SetParent (transform, false);
		uiObj.transform.localPosition = Vector3.zero;
		uiObj.startAnchor = fromViewportPoint;
		uiObj.targetAnchor = toViewportPoint;
		uiObj.ApplyStartConditions ();
		uiObj.SetDisplayText (timeBonus);
	}

}
