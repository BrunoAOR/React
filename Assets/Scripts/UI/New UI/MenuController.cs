using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MenuControllerBase {

	[Header ("Welcome Screen")]
	public MenuIntroController introController;

	[Header ("Panels")]
	public MenuPanelsController panelsController;

	[Header ("Buttons")]
	public Color unlockedColor;
	public Color lockedColor;
	public Color lockImageColor;

	public override void SetActive (bool _) {
		gameObject.SetActive (_);
	}

	void Awake () {
		panelsController.gameObject.SetActive (false);
		panelsController.SetButtonsColors (unlockedColor, lockedColor, lockImageColor);
	}

	IEnumerator Start () {
		yield return (introController.Intro ());
		StartCoroutine (PopMenu ());
	}

	public void PopMenuC () {
		StartCoroutine (PopMenu ());
	}

	public override IEnumerator PopMenu () {
		UpdateUnlockStates ();
		panelsController.gameObject.SetActive (true);
		yield return (panelsController.ScrollInOut (MenuPanelsController.MenuDirection.In));
	}

	private void UpdateUnlockStates () {
		panelsController.UpdateUnlockStates (Managers.Unlockables.GetUnlockStates(), Managers.Unlockables.GetUnlockConditions());
	}

	public void LaunchGame (GameMode gameMode, Difficulty difficulty) {
		if (Managers.Lives.UseLife ()) {
			Managers.Audio.PlaySFX (SFX.MenuButton_GO);
			StartCoroutine (LaunchGameCoroutine (gameMode, difficulty));
		} else {
			Managers.Audio.PlaySFX (SFX.IconClicked_Unlocked);
			Debug.Log ("No tries left!");
			RoundManager.S.PromptForAds ();
		}
	}

	private IEnumerator LaunchGameCoroutine (GameMode gameMode, Difficulty difficulty) {
		yield return (panelsController.ScrollInOut (MenuPanelsController.MenuDirection.Out));
		panelsController.gameObject.SetActive (false);
		RoundManager.S.StartGame (gameMode, difficulty);
		Debug.Log ("Launch Game Done");
	}

}