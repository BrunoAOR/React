using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;	// FileStream, File and FileMode
using System.Runtime.Serialization.Formatters.Binary;	// BinaryFormatter

public static class DataManager {

	[System.Serializable]
	private class SaveData {

		public SaveData () {
			highscores = new Highscores ();
			livesData = new LivesData ();
		}

		// For ScoreManager
		public Highscores highscores;

		// For LivesManager
		public LivesData livesData;
	}

	private static string fileName = "gameData.dat";
	private static SaveData localData;

	private static void Save () {
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/" + fileName);	
		bf.Serialize(file, localData);
		file.Close ();

		Debug.Log ("DATA SAVED");
	}
		

	private static void Load () {
		if (File.Exists (Application.persistentDataPath + "/" + fileName)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/" + fileName, FileMode.Open);
			localData = (SaveData)bf.Deserialize (file); 
			file.Close ();
			Debug.Log ("DATA LOADED");
		} else {
			localData = new SaveData ();
			Debug.Log ("No saved data found!");
		}
	}


	public static void DeleteData () {
		if (File.Exists (Application.persistentDataPath + "/" + fileName)) {
			File.Delete (Application.persistentDataPath + "/" + fileName);
			Debug.Log ("DATA DELETED");
		} else {
			Debug.Log ("No saved data found to delete.");
		}
	}


	public static void Save_Highscores (Highscores newHighscores) {
		if (localData == null)
			Load ();
		
		localData.highscores = newHighscores;
		Save ();
	}


	public static Highscores Load_Highscores () {
		if (localData == null)
			Load ();

		return (localData.highscores);
	}


	public static void Save_LivesData (LivesData newLivesData) {
		if (localData == null)
			Load ();

		localData.livesData = newLivesData;
		Save ();
	}


	public static LivesData Load_LivesData () {
		if (localData == null)
			Load ();

		return (localData.livesData);
	}
}
