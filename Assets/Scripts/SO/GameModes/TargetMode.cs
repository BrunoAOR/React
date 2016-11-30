using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[CreateAssetMenu (menuName = "Game Mode Logic/Target", fileName = "Target Mode")]
public class TargetMode : GameModeLogic {

	[Header ("Specific mode info")]
	public int totalButtonsOnPerRound = 1;
	public int goodButtonsOnPerRound = 1;
	public Color[] lightColors = new Color[] {Color.white, Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, Color.yellow};

	[HideInInspector] public Image targetImage;
	private UIAnchorPosition targetImagePositioner;
	private Color targetColor;
	private int goodButtonsOnRemaining;


	void OnEnable () {
		InitializeGameMode ();
	}

	public override void InitializeGameMode () {
		targetImage = null;
		targetImagePositioner = null;
		targetColor = Color.clear;
		goodButtonsOnRemaining = 0;
	}

	public override int TurnOnButtons (Button[] buttons)	{
		if (targetImagePositioner == null) {
			targetImagePositioner = targetImage.GetComponent<UIAnchorPosition> ();
			if (targetImagePositioner == null) {
				targetImagePositioner = targetImage.gameObject.AddComponent<UIAnchorPosition> ();
			}
		}

		int changeInButtonsOnAmount = 0;

		// Select the target color for this set of buttons
		int targetColorIndex = Random.Range(0, lightColors.Length);
		targetColor = lightColors[targetColorIndex];

		// Prepare a "usableColors" array out of lightColors by removing the targetColor
		Color[] usableColors = new Color[lightColors.Length - 1];
		int LCAIndex = 0;
		for (int i = 0; i < usableColors.Length; i++) {
			if (LCAIndex == targetColorIndex) {
				LCAIndex++;	// Skip the index if it is the same as the targeColorIndex;
			}
			usableColors [i] = lightColors [LCAIndex];
			LCAIndex++;
		}

		// Move the targetImage left or right (random)
		if (Random.value < 0.5) {
			// LEFT
			targetImagePositioner.MoveToAnchorPointX (0.25f);
		} else {
			// RIGHT
			targetImagePositioner.MoveToAnchorPointX (0.75f);
		}

		// Turn on the targetImage and set it to the targetColor
		targetImage.gameObject.SetActive (true);
		targetImage.color = targetColor;

		// Now turn on the buttons, ensuring the first one AND only the first one has the target color
		for (int i = 0; i < totalButtonsOnPerRound; i++) {
			bool buttonLit = false;
			while (!buttonLit) {
				int randomBIndex = Random.Range (0, buttons.Length);
				if (!buttons [randomBIndex].isLit) {
					Color selectedColor;
					if (i < goodButtonsOnPerRound) {	// First buttons that will be turned on should have targetColor
						selectedColor = targetColor;
						goodButtonsOnRemaining++;
					} else {	// All other buttons get their color from the usableColors array
						selectedColor = usableColors [Random.Range (0, usableColors.Length)];
					}
					buttons [randomBIndex].SetLightColor (selectedColor);
					buttons [randomBIndex].TurnLightOn ();
					changeInButtonsOnAmount++;
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
			}
		}
		goodButtonsOnRemaining = 0;
		return changeInButtonsOnAmount;
	}


	public override int ButtonPressed (Button button, out float timeBonus)	{
		timeBonus = 0;
		int changeInButtonsOnAmount = 0;
		if (button.isLit) {
			if (button.GetLightColor () == targetColor) {	// CORRECT Button pressed (button was ON)
				button.TurnLightOff ();
				button.SpawnGoodTimeBonus (goodTimeBonus);
				changeInButtonsOnAmount--;
				goodButtonsOnRemaining--;
				timeBonus = goodTimeBonus;

				Vector2 buttonViewportPos = Camera.main.WorldToViewportPoint (button.transform.position);
				Managers.Score.AnimateAddPoints (buttonViewportPos);
			} else {	// WRONG button pressed (button was ON)
				// Light remains on
				button.SpawnBadTimeBonus (badTimeBonus);
				// No change in buttonsOn
				timeBonus = badTimeBonus;
				// No points are added, but the multiplier is reset
				Managers.Score.ResetMultiplier ();
			}
		} else {	// WRONG button pressed (button was oFF)
			// Light remains off
			button.SpawnBadTimeBonus (badTimeBonus);
			// No change in buttonsOn
			timeBonus = badTimeBonus;
			// No points are added, but the multiplier is reset
			Managers.Score.ResetMultiplier ();
		}

		if (goodButtonsOnRemaining == 0) {
			RoundManager.S.TurnOffButtons ();
			targetImage.gameObject.SetActive (false);
		}

		return changeInButtonsOnAmount;
	}


	void OnValidate () {
		if (goodButtonsOnPerRound <= 0)
			goodButtonsOnPerRound = 1;

		if (totalButtonsOnPerRound <= 1)
			totalButtonsOnPerRound = 2;

		if (lightColors.Length < 2) {
			Debug.LogWarning ("Light Colors array is too small, it has be resized to contain 2 colors");
			Color[] newArray = new Color[2];
			for (int i = 0; i < newArray.Length; i++) {
				if (i < lightColors.Length) {
					newArray [i] = lightColors [i];
				} else {
					newArray [i] = Random.ColorHSV (0f, 1f, 1f, 1f, 1f, 1f);
				}
			}
			lightColors = newArray;
		}
	}
}
