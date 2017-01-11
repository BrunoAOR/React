using System.Collections;

public interface IMenuController
{

	IEnumerator PopMenu ();
	void SetActive (bool _);
}