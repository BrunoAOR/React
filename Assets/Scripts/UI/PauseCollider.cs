using UnityEngine;
using System.Collections;

public class PauseCollider : MonoBehaviour {

	private void Awake () {
		transform.position = new Vector3 (0, 0, -0.1f);
		transform.localScale = Vector3.one;
		GetComponent<BoxCollider2D> ().size = new Vector2 (2 * Camera.main.orthographicSize * Camera.main.aspect, 2 * Camera.main.orthographicSize);
	}
}
