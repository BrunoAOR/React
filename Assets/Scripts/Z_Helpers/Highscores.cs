using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Highscores {

	private Dictionary<GameMode, Dictionary <Difficulty, int>> gameModeDiffDictionary;

	public Highscores () {
		gameModeDiffDictionary = new Dictionary<GameMode, Dictionary<Difficulty, int>> ();
	}

	public bool SetHighscore (GameMode gameMode, Difficulty difficulty, int newScore) {
		if (newScore <= 0) {
			return false;
		}

		Dictionary<Difficulty, int> difficultyDictionary;
		int currentScore;

		if (gameModeDiffDictionary.TryGetValue (gameMode, out difficultyDictionary)) {
			if (difficultyDictionary.TryGetValue (difficulty, out currentScore)) {
				if (currentScore >= newScore) {
					return false;
				} else {
					difficultyDictionary [difficulty] = newScore;
				}
			} else {
				difficultyDictionary.Add (difficulty, newScore);
			}
		} else {
			gameModeDiffDictionary.Add (gameMode, new Dictionary<Difficulty, int> ());
			gameModeDiffDictionary [gameMode].Add (difficulty, newScore);
		}
		return true;
	}

	public int GetHighscore (GameMode gameMode, Difficulty difficulty) {
		Dictionary<Difficulty,int> difficultyDictionary;
		int currentScore;

		if (gameModeDiffDictionary.TryGetValue (gameMode, out difficultyDictionary)) {
			if (difficultyDictionary.TryGetValue (difficulty, out currentScore)) {
				return currentScore;
			}
		}

		return 0;
	}
}