using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanelsController : MonoBehaviour {

	public MenuGameModePanel[] gameModePanels;
	private int currentPanelIndex;
	public float scrollDuration = 1f;

	void Awake () {
		currentPanelIndex = 0;
		gameModePanels [currentPanelIndex].SetEnabled (true);
	}

	void Reset () {
		gameModePanels = GetComponentsInChildren<MenuGameModePanel> ();
	}

	public void OnArrowClicked (MenuGameModePanel callingPanel, MenuArrow.Direction direction) {
		if (direction == MenuArrow.Direction.Left && currentPanelIndex == 0)
			return;

		if (direction == MenuArrow.Direction.Right && currentPanelIndex == gameModePanels.Length - 1)
			return;

		Debug.Log ("Scrolling " + direction + " from " + gameModePanels [currentPanelIndex].gameMode.ToString () + " to " + gameModePanels [currentPanelIndex + (int)direction].gameMode.ToString ());

		StartCoroutine (ScrollPanelsTowards (direction));

	}

	private IEnumerator ScrollPanelsTowards (MenuArrow.Direction direction) {
		gameModePanels [currentPanelIndex].SetEnabled (false);

		float startXPos = transform.localPosition.x;
		float targetXPos = -gameModePanels [currentPanelIndex + (int)direction].transform.localPosition.x;

		Debug.Log ("Moving from " + startXPos + " to " + targetXPos);

		Vector3 currentPos = new Vector3 (startXPos, transform.localPosition.y, transform.localPosition.z);
		float u = 0;
		float startTime = Time.time;

		while ((Time.time - startTime) < scrollDuration) {
			u = (Time.time - startTime) / scrollDuration;
			currentPos.x = (1 - u) * startXPos + u * targetXPos;
			transform.localPosition = currentPos;
			yield return null;
		}

		currentPos.x = targetXPos;
		transform.localPosition = currentPos;

		currentPanelIndex += (int)direction;
		gameModePanels [currentPanelIndex].SetEnabled (true);
	}

}
