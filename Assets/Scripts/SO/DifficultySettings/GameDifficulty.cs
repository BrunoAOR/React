using UnityEngine;

public enum GridSize {Small = 3, Medium, Large}

[CreateAssetMenu (menuName = "Game Difficulty", fileName = "New Game Difficulty")]
public class GameDifficulty : ScriptableObject {

	public string difficulty;
	public GridSize gridSize;
	public ButtonsBehaviour[] buttonsBehaviours;

	[Header ("Number of buttons to click")]
	public int startButtonsToClick = 2;
	public int PlusOneAfterRounds = 5;
	public int finalButtonsToClick = 4;

	[Header ("Wait time to turn on buttons")]
	public float startMinWaitTime = 1.0f;
	public float startMaxWaitTime = 2.0f;
	public int useStartTimeUntilRound = 10;
	public int useFinalTimeFromRound = 20;
	public float finalMinWaitTime = 0f;
	public float finalMaxWaitTime = 0f;

	public int GetButtonsToClick (int roundNumber) {
		int currentButtonsToClick;
		if (PlusOneAfterRounds == 0) {
			currentButtonsToClick = finalButtonsToClick;
		} else {
			// increaseSteps will return the integer part only (so, not the modulo)
			int increaseSteps = (roundNumber - 1) / PlusOneAfterRounds;

			currentButtonsToClick = Mathf.Clamp (startButtonsToClick + increaseSteps, startButtonsToClick, finalButtonsToClick);
		}
		//Debug.Log ("Round: " + roundNumber + " || buttons to click: " + currentButtonsToClick);
		return (currentButtonsToClick);
	}

	public float GetWaitTime (int roundNumber) {
		float u = ((float)(roundNumber - useStartTimeUntilRound)) / (useFinalTimeFromRound - useStartTimeUntilRound);
		u = Mathf.Clamp01 (u);

		float currentMinWaitTime = (1 - u) * startMinWaitTime + u * finalMinWaitTime;
		float currentMaxWaitTime = (1 - u) * startMaxWaitTime + u * finalMaxWaitTime;

		float waitTime = Random.Range (currentMinWaitTime, currentMaxWaitTime);
		//Debug.Log ("Round: " + roundNumber + "|| u = " + u + " || Min: " + currentMinWaitTime + " | Max: " + currentMaxWaitTime + " | Actual Wait: " + waitTime);
		if (waitTime < 0) {
			waitTime = 0;
		}
		return (waitTime);
	}

	void OnValidate () {
		// Buttons to click
		if (startButtonsToClick < 1) {
			startButtonsToClick = 1;
		}
		if (finalButtonsToClick < 1) {
			finalButtonsToClick = 1;
		}
		if (PlusOneAfterRounds < 0) {
			PlusOneAfterRounds = 0;
		}

		// Time: Start Time
		if (startMinWaitTime < 0) {
			startMinWaitTime = 0;
		}
		if (startMaxWaitTime < 0) {
			startMaxWaitTime = 0;
		}
		if (startMaxWaitTime < startMinWaitTime) {
			startMaxWaitTime = startMinWaitTime;
		}

		// Time: Final
		if (finalMinWaitTime < 0) {
			finalMinWaitTime = 0;
		}
		if (finalMaxWaitTime < 0) {
			finalMaxWaitTime = 0;
		}
		if (finalMaxWaitTime < finalMinWaitTime) {
			finalMaxWaitTime = finalMinWaitTime;
		}

		// Time: Round numbers
		if (useStartTimeUntilRound < 0) {
			useStartTimeUntilRound = 0;
		}
		if (useFinalTimeFromRound < 1) {
			useFinalTimeFromRound = 1;
		}
		if (useFinalTimeFromRound <= useStartTimeUntilRound) {
			useFinalTimeFromRound = useStartTimeUntilRound + 1;
		}
	}
}