using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class AnalyticsManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void Examples () {
		int testInt = 5;
		float testFloat = 1.2f;
		string testString = "Testing_001";

		Analytics.CustomEvent ("TestEvent", new Dictionary<string, object> {
			{"testInt", testInt},
			{"testFloat", testFloat},
			{"testString", testString}
		});

		Vector3 testPosition = new Vector3 (1.1f, 2.2f, 3.3f);
		Analytics.CustomEvent ("TestPositionEvent", testPosition);

		Analytics.Transaction("12345abcde", 0.99m, "USD", null, null);

		Gender gender = Gender.Female;
		Analytics.SetUserGender(gender);


		int birthYear = 2014;
		Analytics.SetUserBirthYear(birthYear);
	}
}
