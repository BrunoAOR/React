using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "Buttons Behaviours/Paths/Path List", fileName = "Paths for Grid Size #")]
public class MotionPathSubList : ScriptableObject {

	public int gridSize = 3;
	public bool allPathsValid;
	public Path[] paths;


	public Path GetPath (int pathIndex = -1) {
		if (paths.Length == 0)
			return null;

		if (pathIndex == -1 || pathIndex >= paths.Length)
			pathIndex = Random.Range (0, paths.Length);
		
		return (paths [pathIndex]);

	}


	void OnValidate () {
		if (gridSize <= 3)
			gridSize = 3;

		allPathsValid = true;
		for (int i = 0; i < paths.Length; i++) {
			if (paths[i].motions.Length != gridSize * gridSize) {
				paths[i].motions = new int[gridSize * gridSize];
			}

			bool valid = true;
			bool[] checkArray = new bool[paths[i].motions.Length];
			for (int j = 0; j < checkArray.Length; j++) {
				checkArray [j] = false;
			}

			for (int j = 0; j < paths[i].motions.Length; j++) {
				if (paths [i].motions [j] < 0 || paths [i].motions [j] > checkArray.Length - 1) {
					valid = false;
					allPathsValid = false;
					break;
				}
				if (checkArray [paths[i].motions [j]] == false) {
					checkArray [paths[i].motions [j]] = true;
				} else {
					valid = false;
					allPathsValid = false;
					break;
				}
			}
			paths[i].isValid = valid;
		}
	}

}

[System.Serializable]
public class Path {

	public bool isValid;
	public int[] motions;

}
