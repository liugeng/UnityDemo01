using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(UIDefine))]
public class UIDefineEditor : Editor {

	UIDefine def;
	List<bool> showFold = new List<bool>();

	private void OnEnable() {
		def = target as UIDefine;
		for (int i = 0; i < def.list.Count; i++) {
			showFold.Add(true);
		}
	}

	public override void OnInspectorGUI() {
		//base.OnInspectorGUI();

		GUILayout.Label(
			new GUIContent("" +
				"定义UI界面的一些配置\n" +
				"  prefab\t     UI界面的预制体\n" +
				"  order\t     控制UI的层级顺序，>= 0\n" +
				"  modal\t     是否是模态窗口，模态窗口会隐藏其他一些界面\n" +
				"  description  界面简介"
			),
			new GUIStyle("Box") {
				padding = new RectOffset(5,5,5,5),
				alignment = TextAnchor.MiddleLeft,
				fontSize = 12
			},
			GUILayout.MinWidth(350)
		);

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("添加")) {
			def.list.Insert(0, new UIDefine.Define());
			showFold.Insert(0, true);
		}
		if (GUILayout.Button("排序")) {
			for (int i = 0; i < def.list.Count-1; i++) {
				for (int j = i+1; j < def.list.Count; j++) {
					if (def.list[i].order < def.list[j].order) {
						var tmp = def.list[i];
						def.list[i] = def.list[j];
						def.list[j] = tmp;
					}
				}
			}
		}
		if (GUILayout.Button("全部展开")) {
			for (int i = 0; i < showFold.Count; i++) {
				showFold[i] = true;
			}
		}
		if (GUILayout.Button("全部收缩")) {
			for (int i = 0; i < showFold.Count; i++) {
				showFold[i] = false;
			}
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Separator();


		for (int i = 0; i < def.list.Count; i++) {

			var d = def.list[i];

			GUILayout.Label("---------------------------------------------------------------------------------");

			EditorGUILayout.BeginHorizontal();
			showFold[i] = EditorGUILayout.Foldout(showFold[i], d.prefab ? d.prefab.name : "<None>");
			if (GUILayout.Button("删除")) {
				def.list.RemoveAt(i);
				showFold.RemoveAt(i);
				i--;
				continue;
			}
			EditorGUILayout.EndHorizontal();

			if (showFold[i]) {
				EditorGUI.indentLevel++;
				GameObject go = EditorGUILayout.ObjectField("prefab", d.prefab, typeof(GameObject), false) as GameObject;
				SetNewPrefab(go, i);

				int order = EditorGUILayout.IntField("order", d.order);
				if (order >= 0) {
					def.list[i].order = order;
				}
				def.list[i].modal = EditorGUILayout.Toggle("modal", d.modal);
				//def.list[i].type = (UIDefine.Type)EditorGUILayout.EnumPopup("type", d.type);
				def.list[i].description = EditorGUILayout.TextField("description", def.list[i].description);
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.Separator();
		}

		if (GUI.changed) {
			EditorUtility.SetDirty(def);
			def.AssignDict();
		}
	}

	void SetNewPrefab(GameObject go, int i) {
		if (go == null) {
			def.list[i].prefab = go;
			return;
		}

		foreach (var d in def.list) {
			if (d.prefab && d.prefab.name == go.name) {
				return;
			}
		}
		def.list[i].prefab = go;
	}
}
