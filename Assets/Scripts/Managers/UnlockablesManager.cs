using UnityEngine;
using System.Collections;

public class UnlockablesManager : MonoBehaviour {

	public MenuController menuController;
	public UnlockConditionGroup[] unlockConditionGroups;
	private bool[][] isIconSetIconUnlocked;


	void Awake () {
		isIconSetIconUnlocked = new bool[4][];
		isIconSetIconUnlocked [0] = new bool[3] {true, false, false};
		isIconSetIconUnlocked [1] = new bool[2] {true, false};
		isIconSetIconUnlocked [2] = new bool[3] {true, false, false};
		isIconSetIconUnlocked [3] = new bool[4] {true, false, false, false};
	}

	public UnlockCondition GetUnlockCondition (int iconSetIndex, int iconIndex) {
		return (unlockConditionGroups [iconSetIndex].unlockConditions [iconIndex]);
	}

	public bool[][] GetUnlockStates () {
		UpdateUnlockConditionResults ();
		return isIconSetIconUnlocked;
	}

	public bool GetUnlockState (int iconSetIndex, int iconIndex) {
		if (iconSetIndex < 0 || iconSetIndex >= isIconSetIconUnlocked.Length || iconIndex < 0 || iconIndex >= isIconSetIconUnlocked [iconSetIndex].Length) {
			return false;
		}

		UpdateUnlockConditionResults ();
		return isIconSetIconUnlocked[iconSetIndex][iconIndex];
	}

	private void UpdateUnlockConditionResults () {
		for (int i = 0; i < isIconSetIconUnlocked.Length; i++) {
			for (int j = 0; j < isIconSetIconUnlocked [i].Length; j++) {
				isIconSetIconUnlocked [i] [j] = ConditionAchieved (unlockConditionGroups [i].unlockConditions [j]);
			}
		}
	}

	private bool ConditionAchieved (UnlockCondition condition) {
		int currentValue = int.MinValue;

		switch (condition.fieldA) {
		case UnlockCondition.FieldA.GameMode:
			switch (condition.fieldB) {
			case UnlockCondition.FieldB.PlayCount:
				currentValue = Managers.Stats.GetPlayCount (condition.gameMode);
				break;
			case UnlockCondition.FieldB.CumulativeScore:
				currentValue = Managers.Stats.GetCumulativeScore (condition.gameMode);
				break;
			}
			break;
		case UnlockCondition.FieldA.GamePace:
			switch (condition.fieldB) {
			case UnlockCondition.FieldB.PlayCount:
				currentValue = Managers.Stats.GetPlayCount (condition.gamePace);
				break;
			case UnlockCondition.FieldB.CumulativeScore:
				currentValue = Managers.Stats.GetCumulativeScore (condition.gamePace);
				break;
			}
			break;
		case UnlockCondition.FieldA.GridSize:
			switch (condition.fieldB) {
			case UnlockCondition.FieldB.PlayCount:
				currentValue = Managers.Stats.GetPlayCount (condition.gridSize);
				break;
			case UnlockCondition.FieldB.CumulativeScore:
				currentValue = Managers.Stats.GetCumulativeScore (condition.gridSize);
				break;
			}
			break;
		case UnlockCondition.FieldA.Behaviour:
			switch (condition.fieldB) {
			case UnlockCondition.FieldB.PlayCount:
				currentValue = Managers.Stats.GetPlayCount (condition.behaviour);
				break;
			case UnlockCondition.FieldB.CumulativeScore:
				currentValue = Managers.Stats.GetCumulativeScore (condition.behaviour);
				break;
			}
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
public class UnlockConditionGroup {
	public UnlockCondition[] unlockConditions;
}

[System.Serializable]
public class UnlockCondition {
	public enum FieldA {GameMode, GamePace, GridSize, Behaviour}
	public enum FieldB {PlayCount, CumulativeScore}

	public FieldA fieldA;
	public GameMode gameMode;
	public GamePace gamePace;
	public GridSize gridSize;
	public Behaviour behaviour;
	public FieldB fieldB;
	public int minValue;
}