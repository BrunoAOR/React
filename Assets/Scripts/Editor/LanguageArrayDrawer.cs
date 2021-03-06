﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof(LanguageArray))]
public class LanguageArrayDrawer : PropertyDrawer {
	
	Rect titleRect, keywordLabelRect, keywordFieldRect;
	Rect[] languagesRects;
	SerializedProperty languageTextElements, keyword, showLanguages;

	// Language Text Elements
	float lteHeight = 0;

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		AssignSerializedProperties (property);

		// Calculate Rects
		titleRect = new Rect (
			position.x,
			position.y,
			position.width,
			EditorGUIUtility.singleLineHeight
		);

		float keywordLabelWidth = (position.width / 4f) >= 80 ? 80f : position.width / 4f;
		keywordLabelRect = new Rect (
			position.x,
			position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing),
			keywordLabelWidth,
			EditorGUIUtility.singleLineHeight
		);

		keywordFieldRect = new Rect (
			position.x + keywordLabelWidth,
			position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing),
			position.width - 2 * keywordLabelWidth,
			EditorGUIUtility.singleLineHeight
		);

		languagesRects = new Rect[languageTextElements.arraySize];
		for (int i = 0; i < languagesRects.Length; i++) {
			languagesRects [i] = new Rect (
				position.x,
				position.y + 2 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) + lteHeight * i,
				position.width,
				lteHeight
			);
		}
			
		// General label
		GUIStyle boldFoldout = new GUIStyle (EditorStyles.foldout);
		boldFoldout.fontStyle = FontStyle.Bold;

		EditorGUI.BeginProperty (titleRect, GUIContent.none, showLanguages);
		EditorGUI.BeginChangeCheck ();
		bool newShowLanguage = EditorGUI.Foldout (titleRect, showLanguages.boolValue, property.displayName, true, boldFoldout);
		if (EditorGUI.EndChangeCheck ()) {
			showLanguages.boolValue = newShowLanguage;
		}
		EditorGUI.EndProperty ();

		if (showLanguages.boolValue && languageTextElements.isArray && languageTextElements.arraySize > 0) {
			// Keyword
			EditorGUI.LabelField (keywordLabelRect, "Keyword");
			EditorGUI.PropertyField (keywordFieldRect, keyword, GUIContent.none);

			// Language Text elements
			EditorGUI.indentLevel++;
			for (int i = 0; i < languageTextElements.arraySize; i++) {
				EditorGUI.PropertyField (languagesRects [i], languageTextElements.GetArrayElementAtIndex (i));
			}
			EditorGUI.indentLevel--;
		}
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		AssignSerializedProperties (property);

		if (languageTextElements.arraySize > 0) {
			lteHeight = EditorGUI.GetPropertyHeight (languageTextElements.GetArrayElementAtIndex (0));
		} else {
			lteHeight = 0;
		}

		float height = 0;

		if (showLanguages.boolValue) {
			height = 2 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) + (languageTextElements.arraySize * lteHeight);
		} else {
			height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}

		return (height);
	}

	private void AssignSerializedProperties (SerializedProperty property) {
		keyword					= property.FindPropertyRelative ("keyword");
		languageTextElements	= property.FindPropertyRelative ("languageTextElements");
		showLanguages			= property.FindPropertyRelative ("showLanguages");
	}

}
