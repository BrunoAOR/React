using UnityEngine;

[CreateAssetMenu (menuName = "Game Difficulty", fileName = "New Game Difficulty")]
public class GameDifficulty : ScriptableObject {

	public string difficulty;
	public GridSize gridSize;
	public ButtonsBehaviour[] buttonsBehaviours;
}
