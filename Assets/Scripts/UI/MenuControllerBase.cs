using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuControllerBase : MonoBehaviour {

	public abstract IEnumerator PopMenu ();
	public abstract void SetActive (bool _); 
}
