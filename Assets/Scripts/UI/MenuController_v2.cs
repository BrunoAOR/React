using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController_v2 : MenuControllerBase {

	[Header ("Welcome Screen")]
	public MenuIntroController introController;
	public Transform gameModePanels;

	public override void SetActive (bool _) {
		gameObject.SetActive (_);
	}

	void Awake () {
		gameModePanels.gameObject.SetActive (false);
	}

	IEnumerator Start () {
		yield return (introController.Intro ());
		StartCoroutine (PopMenu ());
	}

	public override IEnumerator PopMenu () {

		yield return new WaitForSeconds (1);
		gameModePanels.gameObject.SetActive (true);
		yield break;
	}
}
