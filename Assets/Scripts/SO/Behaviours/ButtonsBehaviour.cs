using UnityEngine;
using System.Collections;

public abstract class ButtonsBehaviour : ScriptableObject {

	public string behaviourName = "Behaviour name";

	public abstract void InitializeBehaviour (CircleButton[] buttons);
	public abstract void RunBehaviour (float unpauseDuration = 0);
	public abstract void StopBehaviour ();
}
