using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "Buttons Behaviours/Paths/Paths Handler", fileName = "Paths Handler")]
public class MotionPaths : ScriptableObject {

	public MotionPathSubList[] motionPathSubLists;


	public Path GetPath (int gridSize) {
		for (int i = 0; i < motionPathSubLists.Length; i++) {
			if (motionPathSubLists [i].gridSize == gridSize) {
				return (motionPathSubLists [i].GetPath ());
			}
		}
		return null;
	}

}
