using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentPool<T> where T : Component, IPoolable<T>  {

	// The prefab to be instantiated
	private T _prefab;

	// The default transform where the pooled objects are stored
	private Transform _poolTransform;

	// The stack to store all unused available objects
	private Stack<T> _inactiveInstances = new Stack<T> ();

	// A List of all the InstanceIDs of instantiated objects
	private List<int> instanceIDs = new List<int> ();

	/// <summary>
	/// Initializes a new instance of the <see cref="MonoBehaviourPool"/> class.
	/// </summary>
	/// <param name="prefab">The MonoBehaviour that will be instantiated and pooled.</param>
	/// <param name="poolParentTransform">The transform that will hold the pooled (inactive) GameObjects parent when not in use.</param>
	public ComponentPool (T prefab, Transform poolParentTransform, int initialObjectCount = 0) {
		_prefab = prefab;
		_poolTransform = poolParentTransform;

		if (initialObjectCount > 0) {
			RefillPool (initialObjectCount);
		}
	}

	public System.Type GetPoolObjectType () {
		return (typeof(T));
	}

	/// <summary>
	/// Ensures that the pool has at least objectCount objects.
	/// </summary>
	/// <param name="objectCount">Minimum desired amount of objects in the pool</param>
	public void RefillPool (int objectCount) {
		if (objectCount > _inactiveInstances.Count) {
			for (int i = _inactiveInstances.Count; i < objectCount; i++) {
				InstantiateNewObject ();
			}
		}
	}

	private void InstantiateNewObject () {
		T obj = GameObject.Instantiate<T> (_prefab, _poolTransform, false);
		obj.gameObject.SetActive (false);
		instanceIDs.Add (obj.GetInstanceID ());

		obj.Unspawn = Unspawn;

		_inactiveInstances.Push (obj);
	}

	/// <summary>
	/// Destroys all objects in the pool
	/// </summary>
	public void ClearPool () {
		while (_inactiveInstances.Count > 0) {
			Object.Destroy (_inactiveInstances.Pop ().gameObject);
		}
	}

	/// <summary>
	/// Returns and object of type T from the pool if one is available or creates a new one if none is available.
	/// </summary>
	public T Spawn (bool setActive = true) {
		T spawnedObject = null;

		// If there is no available inactive instance...
		if (_inactiveInstances.Count == 0) {
			// ... then create one
			InstantiateNewObject ();
		}
		// Get an object from the pool
		spawnedObject = _inactiveInstances.Pop ();

		// Check with objectNotNull performed in case an element from the pool was destoyed by another script.
		bool objectNotNull = false;

		while (!objectNotNull) {
			if (spawnedObject == null) {
				InstantiateNewObject ();
				spawnedObject = _inactiveInstances.Pop ();
				objectNotNull = true;
			} else {
				objectNotNull = true;
			}
		}

		// Parent it to the world (with false as worldPositionStays to prevent scale issues with UI objects) and activate it
		spawnedObject.transform.SetParent (null, false);
		spawnedObject.gameObject.SetActive (true);

		return (spawnedObject);
	}

	/// <summary>
	/// Places objectToUnspawn back into the pool. If objectToUnspawn was NOT created by the MonoBehaviourPool, then nothing happens.
	/// </summary>
	/// <param name="objectToUnspawn">The object that should be returned to the pool.</param>
	private void Unspawn (T objectToUnspawn) {
		// Check if the object came from the pool
		int objID = objectToUnspawn.GetInstanceID ();

		int i = 0;
		for (; i < instanceIDs.Count; i++) {
			if (objID == instanceIDs [i]) {
				break;
			}
		}

		if (i == instanceIDs.Count) {
			// Object rejected
			return;
		}
		// Object accepted
		objectToUnspawn.gameObject.SetActive (false);
		objectToUnspawn.transform.position = Vector3.zero;
		objectToUnspawn.transform.rotation = Quaternion.identity;
		objectToUnspawn.transform.SetParent (_poolTransform, false);
		_inactiveInstances.Push (objectToUnspawn);
	}

}