﻿using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "Game Mode Logic/Standard", fileName = "Standard Mode")]
public class StandardMode : GameModeLogic {

	[Header ("Specific mode info")]
	public Color[] lightColors = new Color[] {Color.white, Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, Color.yellow};


	void OnEnable () {
		InitializeGameMode ();
	}

	public override void InitializeGameMode () {
		// No initialization needed.
	}

	public override int TurnOnButtons (CircleButton[] buttons, int buttonsToClick)	{
		// Verify buttonsToClick
		if (buttonsToClick < 1) {
			buttonsToClick = 1;
		}
		if (buttonsToClick > buttons.Length) {
			buttonsToClick = buttons.Length;
		}

		// Turn buttons on
		int changeInButtonsOnAmount = 0;
		for (int i = 0; i < buttonsToClick; i++) {
			bool buttonLit = false;
			while (!buttonLit) {
				int randomBIndex = Random.Range (0, buttons.Length);
				if (!buttons [randomBIndex].isLit) {
					Color randomColor = lightColors[Random.Range(0, lightColors.Length)];
					buttons [randomBIndex].SetLightColor (randomColor);
					buttons [randomBIndex].TurnLightOn ();
					changeInButtonsOnAmount++;
					buttonLit = true;
				}
			}
		}
		return changeInButtonsOnAmount;
	}

	public override int TurnOffButtons (CircleButton[] buttons) {
		int changeInButtonsOnAmount = 0;
		for (int i = 0; i < buttons.Length; i++) {
			if (buttons [i].isLit) {
				buttons [i].ForceLightOff ();
				changeInButtonsOnAmount--;
			}
		}
		return changeInButtonsOnAmount;
	}

	public override int ButtonPressed (CircleButton button, out float timeBonus)	{
		timeBonus = 0;
		int changeInButtonsOnAmount = 0;
		if (button.isLit) {		// CORRECT button pressed (button was ON)
			button.TurnLightOff ();
			button.SpawnGoodTimeBonus (goodTimeBonus);
			changeInButtonsOnAmount--;
			timeBonus = goodTimeBonus;

			Vector2 buttonViewportPos = Camera.main.WorldToViewportPoint (button.transform.position);
			Managers.Score.AnimateAddPoints (buttonViewportPos);
		} else {	// WRONG button pressed (button was OFF)
			// Light remains off
			button.SpawnBadTimeBonus (badTimeBonus);
			// No change in buttonsOn
			timeBonus = badTimeBonus;
			// No points are added, but the multiplier is reset
			Managers.Score.ResetMultiplier ();
		}

		return changeInButtonsOnAmount;
	}

}
