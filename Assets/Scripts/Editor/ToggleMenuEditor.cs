using UnityEngine;
using UnityEditor;

/*
[CustomEditor(typeof(ToggleMenu))]
public class ToggleMenuEditor : Editor {

	public override void OnInspectorGUI() {
		ToggleMenu tm = (ToggleMenu)target;

		EditorGUI.BeginChangeCheck();
		SerializedObject sobj = new SerializedObject(tm);
		EditorGUILayout.PropertyField(sobj.FindProperty("_buttons"), true);
		if (EditorGUI.EndChangeCheck()) {
			sobj.ApplyModifiedProperties();
		}

		tm._toggleBtn = EditorGUILayout.ObjectField("ToggleButton", tm._toggleBtn, typeof(GameObject), true) as GameObject;
	}
}
*/
