using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDetection : MonoBehaviour {

	public ClickSign clickSignPrefab;
	public Transform clickSignPoolTransform;

	private ComponentPool<ClickSign> clickSignPool;

	private void Awake () {
		clickSignPool = new ComponentPool<ClickSign> (clickSignPrefab, clickSignPoolTransform);
		clickSignPrefab.gameObject.SetActive (false);

	}

	private void Update () {
		if (Input.GetMouseButtonDown (0)) {
			ShowClick (Input.mousePosition);
		}
	}

	private void ShowClick (Vector2 screenPosition) {
		ClickSign sign = clickSignPool.Spawn ();
		sign.SetParent (transform, false);
		sign.SetPosition (screenPosition);
	}

}
