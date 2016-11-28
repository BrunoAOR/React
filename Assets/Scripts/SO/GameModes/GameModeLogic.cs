using UnityEngine;
using System.Collections;

public abstract class GameModeLogic : ScriptableObject {

	[Header ("Game mode info")]
	/// <summary>
	/// The name of the mode.
	/// </summary>
	public string modeName = "Mode name";
	public string modeDescription = "Mode description";
	[Header ("Round timer")]
	public int startTime = 10;
	[Header ("Wait time to turn on buttons")]
	public float minWaitTime = 0.5f;
	public float maxWaitTime = 1.0f;

	[Header ("Time bonuses")]
	public float goodTimeBonus = 0.1f;
	public float badTimeBonus = -0.1f;

	/// <summary>
	/// Initializes the game mode.
	/// </summary>
	public abstract void InitializeGameMode ();

	/// <summary>
	/// Turns buttons on according to the selected Game Logic. Returns the change in the number of buttons that are lit ON (positive if buttons were lit ON and negative if buttons were turned OFF).
	/// </summary>
	/// <returns>The change in number of buttons that are lit ON.</returns>
	/// <param name="buttons">Array of the buttons in the play area.</param>
	public abstract int TurnOnButtons (Button[] buttons);

	/// <summary>
	/// Turns all buttons off. Returns the change in the number of buttons that are lit ON (positive if buttons were lit ON and negative if buttons were turned OFF).
	/// </summary>
	/// <returns>The change in number of buttons that are lit ON.</returns>
	/// <param name="buttons">Array of the buttons in the play area.</param>
	public abstract int TurnOffButtons (Button[] buttons);

	/// <summary>
	/// Reacts to a button being pressed according to the selected Game Logic. Returns the change in the number of buttons that are lit ON (positive if buttons were lit ON and negative if buttons were turned OFF).
	/// </summary>
	/// <returns>The change in number of buttons that are lit ON.</returns>
	/// <param name="button">The button that was pressed down.</param>
	public abstract int ButtonPressed (Button button, out float timeBonus);

	private void OnValidate () {
		if (startTime < 1) {
			startTime = 1;
		}
		if (minWaitTime < 0) {
			minWaitTime = 0;
		}
		if (maxWaitTime < 0) {
			maxWaitTime = 0;
		}
		if (maxWaitTime < minWaitTime) {
			maxWaitTime = minWaitTime;
		}
		if (goodTimeBonus < 0) {
			goodTimeBonus = 0;
		}
		if (badTimeBonus > 0) {
			badTimeBonus = 0;
		}
	}
}
