using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDifficultyButton : MonoBehaviour {

	public MenuGameModePanel parentModePanel;
	public Difficulty difficulty;

	void Reset () {
		parentModePanel = GetComponentInParent<MenuGameModePanel> ();
	}

	public void OnClick () {
		parentModePanel.OnDifficultyButtonClicked (difficulty);
	}
}
