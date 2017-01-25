using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode {Standard,	Inverse, Target}
public enum Difficulty {Easy, Medium, Hard}

public class EnumsManager : MonoBehaviour {

	public GameModeLogic[] sortedGameModeLogics;
	public GameDifficulty[] sortedGameDifficulties;

	public GameModeLogic GetGameModeLogic (GameMode gameMode) {
		return (sortedGameModeLogics[(int) gameMode]);
	}

	public GameDifficulty GetGameDifficulty (Difficulty difficulty) {
		return (sortedGameDifficulties [(int)difficulty]);
	}
}
