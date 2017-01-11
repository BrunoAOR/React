﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuIntroController : MonoBehaviour {

	[Header ("Welcome Screen")]
	public UIMover logoMover;
	public Text welcomeText;
	public GameObject tapPrompt;
	public GameObject livesPanel;

	private ColorBlenderUIGraphic _welcomeTextBlender;

	void Awake () {
		_welcomeTextBlender = welcomeText.GetComponent<ColorBlenderUIGraphic> ();
		livesPanel.SetActive (false);
	}

	IEnumerator Start () {
		logoMover.SnapToStart ();
		welcomeText.gameObject.SetActive (true);

		tapPrompt.SetActive (true);

		while (!Input.GetMouseButtonDown (0))
			yield return null;

		tapPrompt.SetActive (false);
		Managers.Audio.PlaySFX (SFX.TapPrompt);
	}

	public IEnumerator Intro () {

		while (!Input.GetMouseButtonDown (0))
			yield return null;
		Managers.Audio.PlaySFX (SFX.TapPrompt);
		tapPrompt.SetActive (false);

		// Blend out and setActive(false) for the welcomeText
		StartCoroutine (_welcomeTextBlender.StartColorBlend () );

		// Dim out and then back to full alpha for the logo...
		StartCoroutine (logoMover.GetComponent<ColorBlenderUIGraphic> ().StartColorBlend (false));
		// ... as it moves up
		yield return (logoMover.MoveToTarget ());

		livesPanel.SetActive (true);
	}

}
