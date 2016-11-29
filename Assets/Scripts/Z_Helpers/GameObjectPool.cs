using UnityEngine;
using System.Collections.Generic;

public class GameObjectPool : MonoBehaviour {

	// The prefab to be instantiated
	public GameObject prefab;

	// The stack to store all unused available objects
	private Stack<GameObject> inactiveInstances = new Stack<GameObject> ();

	/// <summary>
	/// Returns an instance of the prefab from the GameObject Pool
	/// </summary>
	/// <returns>An instance of the prefab assign in the Inspector</returns>
	public GameObject GetGameObject () {
		GameObject spawnedGameObject;

		// If there is an available inactive instance... 
		if (inactiveInstances.Count > 0) {
			// ... then return it
			spawnedGameObject = inactiveInstances.Pop ();
		} else {
			// ... otherwise, create a new instance
			spawnedGameObject = (GameObject)GameObject.Instantiate (prefab);

			// ... and add a pooledGameObject component to know it came from this GameObjectPool
			PooledGameObject pooledGameObject = spawnedGameObject.AddComponent<PooledGameObject> ();
			pooledGameObject.pool = this;
		}

		// Put the selected instance back to the root and activate it.
		spawnedGameObject.transform.SetParent(null);
		spawnedGameObject.SetActive (true);

		// Return a reference to the selected instance
		return (spawnedGameObject);
	}

	/// <summary>
	/// Allows an instance from the prefab GameObject to be returned to the pool.
	/// </summary>
	/// <param name="toReturn">The GameObject to be returned to the pool</param>
	public void ReturnGameObject (GameObject toReturn) {
		PooledGameObject pooledGameObject = toReturn.GetComponent<PooledGameObject> ();

		// If this object came from this pool...
		if (pooledGameObject != null && pooledGameObject.pool == this) {
			// ... then make the instance a child of this gameObject and deactivate it
			toReturn.transform.SetParent (transform);
			toReturn.SetActive (false);

			// ... and add it back to the stack.
			inactiveInstances.Push (toReturn);
		} else {	// otherwise, if the object is not a pooled object or belonged to another pool...
			// ... destroy it
			Debug.LogWarning (toReturn.name + "was returned to a pool it didn't spawn from. " + toReturn.name + " will be destroyed.");
			Destroy (toReturn);
		}

	}
}

public class PooledGameObject : MonoBehaviour {
	public GameObjectPool pool;
}
