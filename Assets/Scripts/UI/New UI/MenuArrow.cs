using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuArrow : MonoBehaviour {

	public enum Direction {
		Left = -1,
		Right = 1
	}

	public MenuGameModePanel parentModePanel;
	public Direction direction;

	void Reset () {
		parentModePanel = GetComponentInParent<MenuGameModePanel> ();
	}

	public void OnClick () {
		parentModePanel.OnArrowClicked (direction);
	}
}
