using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "Buttons Behaviours/Paths/Path List", fileName = "Paths for Grid Size #")]
public class MotionPathSubList : ScriptableObject {

	public int gridSize = 3;

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

		foreach (Path path in paths) {
			if (path.motions.Length != gridSize * gridSize) {
				path.motions = new int[gridSize * gridSize];
			}
		}

	}

}

[System.Serializable]
public class Path {
	public int[] motions;
}
