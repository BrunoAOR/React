using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController_v2 : MenuControllerBase {


	public override void SetActive (bool _) {
		gameObject.SetActive (_);
	}

	public override IEnumerator PopMenu () {

		yield break;
	}
}
