using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof (LanguageTextElement))]
public class LanguageTextElementDrawer : PropertyDrawer {

	int numberOfLines = 4;
	Rect titleRect, textAreaRect;
	SerializedProperty language, text;

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		titleRect = new Rect (
			position.x,
			position.y,
			position.width,
			EditorGUIUtility.singleLineHeight
		);

		textAreaRect = new Rect (
			position.x,
			position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing),
			position.width,
			3 * EditorGUIUtility.singleLineHeight
		);

		language = property.FindPropertyRelative ("language");
		text = property.FindPropertyRelative ("text");

		// Draw label
		GUIStyle italicLabel = new GUIStyle (EditorStyles.label);
		italicLabel.fontStyle = FontStyle.Italic;
		EditorGUI.LabelField (titleRect, language.stringValue, italicLabel);

		// Draw textArea
		GUIStyle wrapTextArea = new GUIStyle (EditorStyles.textArea);
		wrapTextArea.wordWrap = true;
		EditorGUI.BeginProperty (textAreaRect, GUIContent.none, property);
		EditorGUI.BeginChangeCheck ();
		string newText = EditorGUI.TextArea (textAreaRect, text.stringValue, wrapTextArea);

		if (EditorGUI.EndChangeCheck ()) {
			text.stringValue = newText;
		}
		EditorGUI.EndProperty ();
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		return (numberOfLines * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
	}
}
