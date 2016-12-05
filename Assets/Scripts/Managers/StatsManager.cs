using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameMode {Standard,	Inverse, Target}
public enum GamePace {Relaxed, Hectic}
public enum GridSize {Small = 3, Medium, Large}
public enum Behaviour {None, Ghost, Motion, GhostMotion}

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

	public void RegisterPlay (GameMode gameMode, GamePace gamePace, GridSize gridSize, Behaviour behaviour, int score) {
		int testInt;

		// Play Count
		stats.totalPlayCount++;
		if (stats.gameModePlayCount.TryGetValue (gameMode, out testInt)) {
			stats.gameModePlayCount [gameMode]++;
		} else {
			stats.gameModePlayCount.Add (gameMode, 1);
		}
		if (stats.gamePacePlayCount.TryGetValue (gamePace, out testInt)) {
			stats.gamePacePlayCount [gamePace]++;
		} else {
			stats.gamePacePlayCount.Add (gamePace, 1);
		}
		if (stats.gridSizePlayCount.TryGetValue (gridSize, out testInt)) {
			stats.gridSizePlayCount [gridSize]++;
		} else {
			stats.gridSizePlayCount.Add (gridSize, 1);
		}
		if (stats.behaviourPlayCount.TryGetValue (behaviour, out testInt)) {
			stats.behaviourPlayCount [behaviour]++;
		} else {
			stats.behaviourPlayCount.Add (behaviour, 1);
		}
		IncreaseSpecificPlayCount (gameMode, gamePace, gridSize, behaviour);

		// CumulativeScore
		stats.totalCumulativeScore += score;
		if (stats.gameModeCumulativeScore.TryGetValue (gameMode, out testInt)) {
			stats.gameModeCumulativeScore [gameMode] += score;
		} else {
			stats.gameModeCumulativeScore.Add (gameMode, score);
		}
		if (stats.gamePaceCumulativeScore.TryGetValue (gamePace, out testInt)) {
			stats.gamePaceCumulativeScore [gamePace] += score;
		} else {
			stats.gamePaceCumulativeScore.Add (gamePace, score);
		}
		if (stats.gridSizeCumulativeScore.TryGetValue (gridSize, out testInt)) {
			stats.gridSizeCumulativeScore [gridSize] += score;
		} else {
			stats.gridSizeCumulativeScore.Add (gridSize, score);
		}
		if (stats.behaviourCumulativeScore.TryGetValue (behaviour, out testInt)) {
			stats.behaviourCumulativeScore [behaviour] += score;
		} else {
			stats.behaviourCumulativeScore.Add (behaviour, score);
		}
		IncreaseSpecificCumulativeScore (gameMode, gamePace, gridSize, behaviour, score);

		SaveData ();
	}


	private void IncreaseSpecificPlayCount (GameMode gameMode, GamePace gamePace, GridSize gridSize, Behaviour behaviour) {
		Dictionary<GamePace,Dictionary<GridSize,Dictionary<Behaviour,int>>> gamePaceDictionary;
		Dictionary<GridSize,Dictionary<Behaviour,int>> gridSizeDictionary;
		Dictionary<Behaviour,int> behaviourDictionary;
		int playCount;

		if (stats.specificPlayCount.TryGetValue (gameMode, out gamePaceDictionary)) {
			if (gamePaceDictionary.TryGetValue (gamePace, out gridSizeDictionary)) {
				if (gridSizeDictionary.TryGetValue (gridSize, out behaviourDictionary)) {
					if (behaviourDictionary.TryGetValue (behaviour, out playCount)) {
						behaviourDictionary [behaviour]++;
					} else {
						// No playCount found for bahaviour
						behaviourDictionary.Add (behaviour, 1);
					}
				} else {
					// No behaviourDictionary found for gridSize
					behaviourDictionary = new Dictionary<Behaviour, int> ();
					behaviourDictionary.Add (behaviour, 1);
					gridSizeDictionary.Add (gridSize, behaviourDictionary);
				}
			} else {
				// No gridSizeDictionary found for gamePace
				behaviourDictionary = new Dictionary<Behaviour, int> ();
				behaviourDictionary.Add (behaviour, 1);
				gridSizeDictionary = new Dictionary<GridSize, Dictionary<Behaviour, int>> ();
				gridSizeDictionary.Add (gridSize, behaviourDictionary);
				gamePaceDictionary.Add (gamePace, gridSizeDictionary);
			}
		} else {
			// No gamePaceDictionary found for gameMode
			behaviourDictionary = new Dictionary<Behaviour, int> ();
			behaviourDictionary.Add (behaviour, 1);
			gridSizeDictionary = new Dictionary<GridSize, Dictionary<Behaviour, int>> ();
			gridSizeDictionary.Add (gridSize, behaviourDictionary);
			gamePaceDictionary = new Dictionary<GamePace, Dictionary<GridSize, Dictionary<Behaviour, int>>> ();
			gamePaceDictionary.Add (gamePace, gridSizeDictionary);
			stats.specificPlayCount.Add (gameMode, gamePaceDictionary);
		}
	}


	private void IncreaseSpecificCumulativeScore (GameMode gameMode, GamePace gamePace, GridSize gridSize, Behaviour behaviour, int score) {
		Dictionary<GamePace,Dictionary<GridSize,Dictionary<Behaviour,int>>> gamePaceDictionary;
		Dictionary<GridSize,Dictionary<Behaviour,int>> gridSizeDictionary;
		Dictionary<Behaviour,int> behaviourDictionary;
		int cumulativeScore;

		if (stats.specificCumulativeScore.TryGetValue (gameMode, out gamePaceDictionary)) {
			if (gamePaceDictionary.TryGetValue (gamePace, out gridSizeDictionary)) {
				if (gridSizeDictionary.TryGetValue (gridSize, out behaviourDictionary)) {
					if (behaviourDictionary.TryGetValue (behaviour, out cumulativeScore)) {
						behaviourDictionary [behaviour] += score;
					} else {
						// No playCount found for bahaviour
						behaviourDictionary.Add (behaviour, score);
					}
				} else {
					// No behaviourDictionary found for gridSize
					behaviourDictionary = new Dictionary<Behaviour, int> ();
					behaviourDictionary.Add (behaviour, score);
					gridSizeDictionary.Add (gridSize, behaviourDictionary);
				}
			} else {
				// No gridSizeDictionary found for gamePace
				behaviourDictionary = new Dictionary<Behaviour, int> ();
				behaviourDictionary.Add (behaviour, score);
				gridSizeDictionary = new Dictionary<GridSize, Dictionary<Behaviour, int>> ();
				gridSizeDictionary.Add (gridSize, behaviourDictionary);
				gamePaceDictionary.Add (gamePace, gridSizeDictionary);
			}
		} else {
			// No gamePaceDictionary found for gameMode
			behaviourDictionary = new Dictionary<Behaviour, int> ();
			behaviourDictionary.Add (behaviour, score);
			gridSizeDictionary = new Dictionary<GridSize, Dictionary<Behaviour, int>> ();
			gridSizeDictionary.Add (gridSize, behaviourDictionary);
			gamePaceDictionary = new Dictionary<GamePace, Dictionary<GridSize, Dictionary<Behaviour, int>>> ();
			gamePaceDictionary.Add (gamePace, gridSizeDictionary);
			stats.specificCumulativeScore.Add (gameMode, gamePaceDictionary);
		}
	}


	public int GetPlayCount () {
		return stats.totalPlayCount;
	}

	public int GetPlayCount (GameMode gameMode) {
		if (stats.gameModePlayCount.ContainsKey (gameMode)) {
			return stats.gameModePlayCount [gameMode];
		} else {
			return 0;
		}
	}

	public int GetPlayCount (GamePace gamePace) {
		if (stats.gamePacePlayCount.ContainsKey (gamePace)) {
			return stats.gamePacePlayCount [gamePace];
		} else {
			return 0;
		}
	}

	public int GetPlayCount (GridSize gridSize) {
		if (stats.gridSizePlayCount.ContainsKey (gridSize)) {
			return stats.gridSizePlayCount [gridSize];
		} else {
			return 0;
		}
	}

	public int GetPlayCount (Behaviour behaviour) {
		if (stats.behaviourPlayCount.ContainsKey (behaviour)) {
			return stats.behaviourPlayCount [behaviour];
		} else {
			return 0;
		}
	}

	public int GetPlayCount (GameMode gameMode, GamePace gamePace, GridSize gridSize, Behaviour behaviour) {
		Dictionary<GamePace,Dictionary<GridSize,Dictionary<Behaviour,int>>> gamePaceDictionary;
		Dictionary<GridSize,Dictionary<Behaviour,int>> gridSizeDictionary;
		Dictionary<Behaviour,int> behaviourDictionary;
		int playCount;

		if (stats.specificPlayCount.TryGetValue (gameMode, out gamePaceDictionary)) {
			if (gamePaceDictionary.TryGetValue (gamePace, out gridSizeDictionary)) {
				if (gridSizeDictionary.TryGetValue (gridSize, out behaviourDictionary)) {
					if (behaviourDictionary.TryGetValue (behaviour, out playCount)) {
						return playCount;
					}
				}
			}
		}

		return 0;
	}


	public int GetCumulativeScore () {
		return stats.totalCumulativeScore;
	}

	public int GetCumulativeScore (GameMode gameMode) {
		if (stats.gameModeCumulativeScore.ContainsKey(gameMode)) {
			return stats.gameModeCumulativeScore [gameMode];
		} else {
			return 0;
		}
	}

	public int GetCumulativeScore (GamePace gamePace) {
		if (stats.gamePaceCumulativeScore.ContainsKey (gamePace)) {
			return stats.gamePaceCumulativeScore [gamePace];
		} else {
			return 0;
		}
	}

	public int GetCumulativeScore (GridSize gridSize) {
		if (stats.gridSizeCumulativeScore.ContainsKey (gridSize)) {
			return stats.gridSizeCumulativeScore [gridSize];
		} else {
			return 0;
		}
	}

	public int GetCumulativeScore (Behaviour behaviour) {
		if (stats.behaviourCumulativeScore.ContainsKey (behaviour)) {
			return stats.behaviourCumulativeScore [behaviour];
		} else {
			return 0;
		}
	}

	public int GetCumulativeScore (GameMode gameMode, GamePace gamePace, GridSize gridSize, Behaviour behaviour) {
		Dictionary<GamePace,Dictionary<GridSize,Dictionary<Behaviour,int>>> gamePaceDictionary;
		Dictionary<GridSize,Dictionary<Behaviour,int>> gridSizeDictionary;
		Dictionary<Behaviour,int> behaviourDictionary;
		int cumulativeScore;

		if (stats.specificCumulativeScore.TryGetValue (gameMode, out gamePaceDictionary)) {
			if (gamePaceDictionary.TryGetValue (gamePace, out gridSizeDictionary)) {
				if (gridSizeDictionary.TryGetValue (gridSize, out behaviourDictionary)) {
					if (behaviourDictionary.TryGetValue (behaviour, out cumulativeScore)) {
						return cumulativeScore;
					}
				}
			}
		}
		return 0;
	}
}

[System.Serializable]
public class StatsData {

	// PlayCount
	public int totalPlayCount;
	public Dictionary<GameMode, int> gameModePlayCount;
	public Dictionary<GamePace, int> gamePacePlayCount;
	public Dictionary<GridSize, int> gridSizePlayCount;
	public Dictionary<Behaviour, int> behaviourPlayCount;
	public Dictionary<GameMode, Dictionary<GamePace,Dictionary<GridSize,Dictionary<Behaviour,int>>>> specificPlayCount;

	// CumulativeScore
	public int totalCumulativeScore;
	public Dictionary<GameMode, int> gameModeCumulativeScore;
	public Dictionary<GamePace, int> gamePaceCumulativeScore;
	public Dictionary<GridSize, int> gridSizeCumulativeScore;
	public Dictionary<Behaviour, int> behaviourCumulativeScore;
	public Dictionary<GameMode, Dictionary<GamePace,Dictionary<GridSize,Dictionary<Behaviour,int>>>> specificCumulativeScore;

	public StatsData () {
		// PlayCount
		totalPlayCount = 0;
		gameModePlayCount = new Dictionary<GameMode, int> ();
		gamePacePlayCount = new Dictionary<GamePace, int> ();
		gridSizePlayCount = new Dictionary<GridSize, int> ();
		behaviourPlayCount = new Dictionary<Behaviour, int> ();
		specificPlayCount = new Dictionary<GameMode, Dictionary<GamePace,Dictionary<GridSize,Dictionary<Behaviour,int>>>> ();

		// CumulativeScore
		totalCumulativeScore = 0;
		gameModeCumulativeScore = new Dictionary<GameMode, int> ();
		gamePaceCumulativeScore = new Dictionary<GamePace, int> ();
		gridSizeCumulativeScore = new Dictionary<GridSize, int> ();
		behaviourCumulativeScore = new Dictionary<Behaviour, int> ();
		specificCumulativeScore = new Dictionary<GameMode, Dictionary<GamePace, Dictionary<GridSize, Dictionary<Behaviour, int>>>> ();
	}
}


