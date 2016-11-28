using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BGLineMover : MonoBehaviour {

	public float speed = 1;
	public float lifeTime = 3;

	public RectTransform rectTransform;
	public Image image;

	private float screenWidth;


	void Reset () {
		rectTransform = GetComponent<RectTransform> ();
		image = GetComponent <Image> ();
	}

	void Start () {
		screenWidth = Camera.main.pixelWidth;
		Destroy (gameObject, lifeTime);
	}

	void Update () {

		transform.Translate (Vector3.right * screenWidth * speed * Time.deltaTime, Space.Self);

	}

}
