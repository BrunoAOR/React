using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Highscores {

	private Dictionary<string, Dictionary<int,Dictionary<int,int>>> gameModeDictionary;	// gameModeName, gridSize, behavioursMixID, score


	public Highscores () {
		gameModeDictionary = new Dictionary<string, Dictionary<int,Dictionary<int,int>>> ();
	}

	public bool SetHighscore (int newScore, string gameModeName, int gridSize, ButtonsBehaviour[] behaviours) {
		int behaviourMixID = 0;

		for (int i = 0; i < behaviours.Length; i++) {
			if (behaviours [i] != null) {
				behaviourMixID += (int)Mathf.Pow (2, behaviours [i].ID);
			}
		}

		Dictionary<int,Dictionary<int,int>> gridSizeDictionary;
		Dictionary<int,int> behavioursMixIDDictionary;
		int score;
		if (gameModeDictionary.TryGetValue (gameModeName, out gridSizeDictionary)) {
			if (gridSizeDictionary.TryGetValue (gridSize, out behavioursMixIDDictionary)) {
				if (behavioursMixIDDictionary.TryGetValue (behaviourMixID, out score)) {
					if (score >= newScore) {
						return false;
					} else {
						behavioursMixIDDictionary [behaviourMixID] = newScore;
					}
				} else {
					behavioursMixIDDictionary.Add (behaviourMixID, newScore);
				}
			} else {
				behavioursMixIDDictionary = new Dictionary<int,int> ();
				behavioursMixIDDictionary.Add (behaviourMixID, newScore);
				gridSizeDictionary.Add (gridSize, behavioursMixIDDictionary);
			}
		} else {
			behavioursMixIDDictionary = new Dictionary<int,int> ();
			behavioursMixIDDictionary.Add (behaviourMixID, newScore);
			gridSizeDictionary = new Dictionary<int,Dictionary<int,int>> ();
			gridSizeDictionary.Add (gridSize, behavioursMixIDDictionary);
			gameModeDictionary.Add (gameModeName, gridSizeDictionary);
		}
		return true;
	}


	public int GetHighscore (string gameModeName, int gridSize, ButtonsBehaviour[] behaviours) {
		int behaviourMixID = 0;

		for (int i = 0; i < behaviours.Length; i++) {
			if (behaviours [i] != null) {
				behaviourMixID += (int)Mathf.Pow (2, behaviours [i].ID);
			}
		}

		Dictionary<int,Dictionary<int,int>> gridSizeDictionary;
		Dictionary<int,int> behavioursMixIDDictionary;
		int score;
		if (gameModeDictionary.TryGetValue (gameModeName, out gridSizeDictionary)) {
			if (gridSizeDictionary.TryGetValue (gridSize, out behavioursMixIDDictionary)) {
				if (behavioursMixIDDictionary.TryGetValue (behaviourMixID, out score)) {
					return score;
				}
			}
		}

		return 0;
	}
}