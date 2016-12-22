using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof (ShowOnlyAttribute))]
public class ShowOnlyPropertyDrawer : PropertyDrawer {

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		string displayText;

		switch (property.propertyType) {
		case SerializedPropertyType.Boolean:
			displayText = property.boolValue.ToString ();
			break;
		case SerializedPropertyType.Integer:
			displayText = property.intValue.ToString ();
			break;
		case SerializedPropertyType.Float:
			displayText = property.floatValue.ToString ("F4");
			break;
		case SerializedPropertyType.String:
			displayText = property.stringValue;
			break;
		case SerializedPropertyType.Enum:
			displayText = property.enumNames [property.enumValueIndex].ToString ();
			break;
		default:
			displayText = "(Type not supported)";
			break;
		}

		EditorGUI.LabelField (position, label.text, displayText);
	}
}
