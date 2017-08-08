using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidgetDefine : MonoBehaviour {

	public static WidgetDefine instance;

	// 定义Widgets
	public List<GameObject> widgetList = new List<GameObject>();
	private Dictionary<string, GameObject> widgetDict;


	private void Awake() {
		instance = this;
	}

	private GameObject CreateWidget(string widgetName, Transform parent) {
		InitDictIfNeeded();
		if (widgetDict.ContainsKey(widgetName)) {
			GameObject go = widgetDict[widgetName];
			if (go) {
				if (parent == null) {
					parent = UIManager.instance.transform;
				}
				return Instantiate(go, parent);
			}
		}
		return null;
	}

	private void InitDictIfNeeded() {
		if (widgetDict == null) {
			widgetDict = new Dictionary<string, GameObject>();
			foreach (var go in widgetList) {
				if (go) {
					widgetDict.Add(go.name, go);
				}
			}
		}
	}

	public static Progress CreateProgress(Transform parent) {
		if (instance) {
			GameObject go = instance.CreateWidget("Progress", parent);
			if (go) {
				return go.GetComponent<Progress>();
			}
		}
		return null;
	}
}
