using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatsManager : MonoBehaviour {

	private StatsData stats;

	void Awake () {
		LoadData ();
	}

	private void SaveData () {
		DataManager.Save_StatsData (stats);
	}

	private void LoadData () {
		stats = DataManager.Load_StatsData ();
	}

	public void RegisterPlay (GameMode gameMode, Difficulty difficulty, int score) {

		// Play Count
		stats.totalPlayCount++;

		if (stats.gameModeDiffPlayCount.ContainsKey (gameMode)) {
			if (stats.gameModeDiffPlayCount [gameMode].ContainsKey (difficulty)) {
				stats.gameModeDiffPlayCount [gameMode] [difficulty]++;
			} else {
				stats.gameModeDiffPlayCount [gameMode].Add (difficulty, 1);
			}
		} else {
			stats.gameModeDiffPlayCount.Add (gameMode, new Dictionary<Difficulty, int> ());
			stats.gameModeDiffPlayCount [gameMode].Add (difficulty, 1);
		}

		// Cumulative Score
		stats.totalCumulativeScore++;

		if (stats.gameModeDiffCumulativeScore.ContainsKey (gameMode)) {
			if (stats.gameModeDiffCumulativeScore [gameMode].ContainsKey (difficulty)) {
				stats.gameModeDiffCumulativeScore [gameMode] [difficulty] += score;
			} else {
				stats.gameModeDiffCumulativeScore [gameMode].Add (difficulty, score);
			}
		} else {
			stats.gameModeDiffCumulativeScore.Add (gameMode, new Dictionary<Difficulty, int> ());
			stats.gameModeDiffCumulativeScore [gameMode].Add (difficulty, score);
		}

		SaveData ();
	}

	public int GetPlayCount () {
		return stats.totalPlayCount;
	}

	public int GetPlayCount (GameMode gameMode) {
		int count;
		if (stats.gameModePlayCount.TryGetValue (gameMode, out count)) {
			return count;
		}
		return 0;
	}

	public int GetPlayCount (GameMode gameMode, Difficulty difficulty) {
		if (stats.gameModeDiffPlayCount.ContainsKey (gameMode)) {
			int count;
			if (stats.gameModeDiffPlayCount [gameMode].TryGetValue (difficulty, out count)) {
				return count;
			}
		}
		return 0;
	}

	public int GetCumulativeScore () {
		return stats.totalCumulativeScore;
	}

	public int GetCumulativeScore (GameMode gameMode) {
		int score;
		if (stats.gameModeCumulativeScore.TryGetValue(gameMode, out score)) {
			return score;
		}
		return 0;
	}

	public int GetCumulativeScore (GameMode gameMode, Difficulty difficulty) {
		Dictionary <Difficulty, int> difficultyDictionary;
		if (stats.gameModeDiffCumulativeScore.TryGetValue (gameMode, out difficultyDictionary)) {
			int score;
			if (difficultyDictionary.TryGetValue (difficulty, out score)) {
				return score;
			}
		}

		return 0;
	}
}

[System.Serializable]
public class StatsData {

	// Play Count
	public int totalPlayCount;
	public Dictionary<GameMode, int> gameModePlayCount;
	public Dictionary<GameMode, Dictionary<Difficulty,int>> gameModeDiffPlayCount; 
	
	// Cumulative Score
	public int totalCumulativeScore;
	public Dictionary<GameMode, int> gameModeCumulativeScore;
	public Dictionary<GameMode, Dictionary<Difficulty, int>> gameModeDiffCumulativeScore;

	public StatsData () {
		// Play Count
		totalPlayCount = 0;
		gameModePlayCount = new Dictionary<GameMode, int> ();
		gameModeDiffPlayCount = new Dictionary<GameMode, Dictionary <Difficulty, int>> ();

		// Cumulative Score
		totalCumulativeScore = 0;
		gameModeCumulativeScore = new Dictionary<GameMode, int> ();
		gameModeDiffCumulativeScore = new Dictionary<GameMode, Dictionary <Difficulty, int>> ();
	}
}


