using UnityEngine;
using System.Collections;

public abstract class ButtonsBehaviour : ScriptableObject {

	public string behaviourName = "Behaviour name";
	public Behaviour statsBehaviour;
	public int ID = 0;

	public abstract void InitializeBehaviour (Button[] buttons);
	public abstract void RunBehaviour (float unpauseDuration = 0);
	public abstract void StopBehaviour ();
}
