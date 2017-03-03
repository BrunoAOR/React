using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSign : MonoBehaviour, IPoolable<ClickSign> {

	public float lifeTime;
	public Vector2 initialSize;
	public Vector2 finalSize;

	public UnspawnDelegate<ClickSign> Unspawn { get; set;}

	private RectTransform _rectTransform;
	private float timeStart;

	private void Awake () {
		_rectTransform = GetComponent<RectTransform> ();
	}

	private void OnEnable () {
		timeStart = Time.unscaledTime;
	}

	private void Update () {

		if (Time.unscaledTime - timeStart > lifeTime) {
			_rectTransform.sizeDelta = initialSize;
			Unspawn (this);
			return;
		}

		float u = (Time.unscaledTime - timeStart) / lifeTime;
		Vector2 currentSize = Vector2.Lerp (initialSize, finalSize, u);

		_rectTransform.sizeDelta = currentSize;
	}

	public void SetPosition (Vector2 position) {
		_rectTransform.position = position;
	}

	public void SetParent (Transform parent, bool worldPositionStays) {
		_rectTransform.SetParent (parent, worldPositionStays);
	}





}
