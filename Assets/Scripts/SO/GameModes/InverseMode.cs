﻿using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "Game Mode Logic/Inverse", fileName = "Inverse Mode")]
public class InverseMode : GameModeLogic {

	[Header ("Specific mode info")]
	public int buttonsLeftOffPerRound = 2;
	public Color[] lightColors = new Color[] {Color.white, Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, Color.yellow};

	private int buttonsOffRemaining;


	private void OnEnable () {
		buttonsOffRemaining = 0;
	}


	private void OnDisable () {
		buttonsOffRemaining = 0;
	}


	public override int TurnOnButtons (Button[] buttons)	{
		int totalButtonCount = RoundManager.S.boardManager.gridSize * RoundManager.S.boardManager.gridSize;
		buttonsOffRemaining = totalButtonCount;
		int changeInButtonsOnAmount = 0;

		for (int i = 0; i < (totalButtonCount - buttonsLeftOffPerRound); i++) {
			bool buttonLit = false;
			while (!buttonLit) {
				int randomBIndex = Random.Range (0, buttons.Length);
				if (!buttons [randomBIndex].isLit) {
					Color randomColor = lightColors[Random.Range(0, lightColors.Length)];
					buttons [randomBIndex].SetLightColor (randomColor);
					buttons [randomBIndex].TurnLightOn ();
					changeInButtonsOnAmount++;
					buttonsOffRemaining--;
					buttonLit = true;
				}
			}
		}
		return changeInButtonsOnAmount;
	}

	public override int TurnOffButtons (Button[] buttons) {
		int changeInButtonsOnAmount = 0;
		for (int i = 0; i < buttons.Length; i++) {
			if (buttons [i].isLit) {
				buttons [i].ForceLightOff ();
				changeInButtonsOnAmount--;
				buttonsOffRemaining++;
			}
		}
		return changeInButtonsOnAmount;
	}

	public override int ButtonPressed (Button button, out float timeBonus)	{
		timeBonus = 0;
		int changeInButtonsOnAmount = 0;
		if (!button.isLit) {
			button.TurnLightOn ();
			button.SpawnGoodTimeBonus (goodTimeBonus);
			changeInButtonsOnAmount++;
			buttonsOffRemaining--;
			timeBonus = goodTimeBonus;
			Managers.Score.AddPoints ();
		}

		if (buttonsOffRemaining == 0) {
			RoundManager.S.TurnOffButtons ();
		}

		return changeInButtonsOnAmount;
	}
}
