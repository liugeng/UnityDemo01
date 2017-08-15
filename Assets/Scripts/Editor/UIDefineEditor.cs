using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(UIDefine))]
public class UIDefineEditor : Editor {

	UIDefine def;
	List<bool> showFold = new List<bool>();

	private void OnEnable() {
		def = target as UIDefine;
		for (int i = 0; i < def.uiList.Count; i++) {
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
			def.uiList.Insert(0, new UIDefine.Define());
			showFold.Insert(0, true);
		}
		if (GUILayout.Button("排序")) {
			for (int i = 0; i < def.uiList.Count-1; i++) {
				for (int j = i+1; j < def.uiList.Count; j++) {
					if (def.uiList[i].order < def.uiList[j].order) {
						var tmp = def.uiList[i];
						def.uiList[i] = def.uiList[j];
						def.uiList[j] = tmp;
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


		for (int i = 0; i < def.uiList.Count; i++) {

			var d = def.uiList[i];

			GUILayout.Label("---------------------------------------------------------------------------------");

			EditorGUILayout.BeginHorizontal();
			showFold[i] = EditorGUILayout.Foldout(showFold[i], d.prefab ? d.prefab.name : "<None>");
			if (GUILayout.Button("删除")) {
				def.uiList.RemoveAt(i);
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
					def.uiList[i].order = order;
				}
				def.uiList[i].modal = EditorGUILayout.Toggle("modal", d.modal);
				//def.list[i].type = (UIDefine.Type)EditorGUILayout.EnumPopup("type", d.type);
				def.uiList[i].description = EditorGUILayout.TextField("description", def.uiList[i].description);
				EditorGUI.indentLevel--;
			}
		}

		if (GUI.changed) {
			EditorUtility.SetDirty(def);
		}
	}

	void SetNewPrefab(GameObject go, int i) {
		if (go == null) {
			def.uiList[i].prefab = go;
			return;
		}

		foreach (var d in def.uiList) {
			if (d.prefab && d.prefab.name == go.name) {
				return;
			}
		}
		def.uiList[i].prefab = go;
	}
}
