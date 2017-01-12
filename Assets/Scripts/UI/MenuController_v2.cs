using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController_v2 : MenuControllerBase {

	[Header ("Data")]
	public GameModeLogic[] gameModes;
	public GameDifficulty[] gameDifficulties;

	[Header ("Welcome Screen")]
	public MenuIntroController introController;

	[Header ("Panels")]
	public MenuPanelsController panelsController;

	public override void SetActive (bool _) {
		gameObject.SetActive (_);
	}

	void Awake () {
		panelsController.gameObject.SetActive (false);
	}

	IEnumerator Start () {
		yield return (introController.Intro ());
		StartCoroutine (PopMenu ());
	}

	public void PopMenuC () {
		StartCoroutine (PopMenu ());
	}

	public override IEnumerator PopMenu () {
		panelsController.gameObject.SetActive (true);
		yield return (panelsController.ScrollInOut (MenuPanelsController.MenuDirection.In));
	}

	public void LaunchGame (GameMode gameMode, Difficulty difficulty) {
		StartCoroutine (LaunchGameCoroutine (gameMode, difficulty));
	}

	private IEnumerator LaunchGameCoroutine (GameMode gameMode, Difficulty difficulty) {
		yield return (panelsController.ScrollInOut (MenuPanelsController.MenuDirection.Out));
		panelsController.gameObject.SetActive (false);
		RoundManager.S.StartGame (GetGameModeLogic (gameMode), GetGridSize (difficulty), GetButtonsBehaviours (difficulty));
		Debug.Log ("Launch Game Done");
	}

	private GameModeLogic GetGameModeLogic (GameMode gameMode) {
		return gameModes[(int)gameMode];
	}

	private int GetGridSize (Difficulty difficulty) {
		return ((int)(gameDifficulties [(int)difficulty].gridSize));
	}

	private ButtonsBehaviour[] GetButtonsBehaviours (Difficulty difficulty) {
		return (gameDifficulties [(int)difficulty].buttonsBehaviours);
	}
}
