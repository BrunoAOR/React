using UnityEngine;
using System.Collections;

public class UnlockablesManager : MonoBehaviour {

	public GameModeUnlockables[] gameModes;
	private bool[][] isDifficultyUnlocked;

	void Awake () {
		isDifficultyUnlocked = new bool[gameModes.Length][];
		for (int i = 0; i < isDifficultyUnlocked.Length; i++) {
			isDifficultyUnlocked[i] = new bool[gameModes[i].difficulties.Length];
			for (int j = 0; j < gameModes [i].difficulties.Length; j++) {
				isDifficultyUnlocked[i][j] = false;
			}
		}
		isDifficultyUnlocked [0] [0] = true;
	}

	public UnlockCondition GetUnlockCondition (int gameModeIndex, int difficultyIndex) {
		return (gameModes [gameModeIndex].difficulties [difficultyIndex].conditionToUnlock);
	}

	public bool[][] GetUnlockStates () {
		UpdateUnlockConditionResults ();
		return isDifficultyUnlocked;
	}

	public UnlockCondition[][] GetUnlockConditions () {
		UnlockCondition[][] conditionsArray = new UnlockCondition[gameModes.Length][];
		for (int i = 0; i < gameModes.Length; i++) {
			conditionsArray[i] = new UnlockCondition[gameModes[i].difficulties.Length];
			for (int j = 0; j < gameModes[i].difficulties.Length; j++) {
				conditionsArray[i][j] = gameModes [i].difficulties [j].conditionToUnlock;
			}
		}

		return conditionsArray;
	}

	public bool GetUnlockState (int gameModeIndex, int difficultyIndex) {
		if (gameModeIndex < 0 || gameModeIndex >= isDifficultyUnlocked.Length || difficultyIndex < 0 || difficultyIndex >= isDifficultyUnlocked [gameModeIndex].Length) {
			return false;
		}

		UpdateUnlockConditionResults ();
		return isDifficultyUnlocked[gameModeIndex][difficultyIndex];
	}

	private void UpdateUnlockConditionResults () {
		for (int i = 0; i < isDifficultyUnlocked.Length; i++) {
			for (int j = 0; j < isDifficultyUnlocked [i].Length; j++) {
				isDifficultyUnlocked [i] [j] = IsConditionAchieved (gameModes [i].difficulties [j].conditionToUnlock);
			}
		}
	}

	private bool IsConditionAchieved (UnlockCondition condition) {
		int currentValue = int.MinValue;

		switch (condition.aCountOf) {
		case UnlockCondition.CountOf.PlayCount:
			currentValue = Managers.Stats.GetPlayCount (condition.gameMode, condition.difficulty);
			break;
		case UnlockCondition.CountOf.CumulativeScore:
			currentValue = Managers.Stats.GetCumulativeScore (condition.gameMode, condition.difficulty);
			break;
		}

		if (currentValue >= condition.minValue) {
			return true;
		} else {
			return false;
		}
	}
}

[System.Serializable] 
public class GameModeUnlockables {
	public GameMode gameMode;
	public UnlockableDifficulty[] difficulties;
}

[System.Serializable]
public class UnlockableDifficulty {
	public Difficulty difficulty;
	public UnlockCondition conditionToUnlock;
}

[System.Serializable]
public class UnlockCondition {
	public enum CountOf {PlayCount, CumulativeScore}
	public GameMode gameMode;
	public Difficulty difficulty;
	public CountOf aCountOf;
	public int minValue;

	public string GetText () {
		string message;
		switch (aCountOf) {
		case UnlockCondition.CountOf.PlayCount:
			message = string.Format ("Play {0} times in {1} mode ({2}).", minValue, gameMode.ToString(), difficulty.ToString());
			break;
		case UnlockCondition.CountOf.CumulativeScore:
			message = string.Format ("Reach {0} points in {1} mode ({2}).", minValue, gameMode.ToString(), difficulty.ToString());
			break;
		default:
			message = "";
			break;
		}

		return message;
	}
}