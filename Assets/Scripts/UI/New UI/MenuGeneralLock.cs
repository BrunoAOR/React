using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGeneralLock : MonoBehaviour {
	private UIShaker _uiShaker;

	void Awake () {
		_uiShaker = GetComponent<UIShaker> ();
	}

	public void OnClick () {
		Managers.Audio.PlaySFX (SFX.IconClicked_Locked);
		if (_uiShaker != null) {
			_uiShaker.StartShakeRotate ();
		}
	}
}
