using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	
	public static UIManager instance;
	[SerializeField]
	private Dictionary<string, GameObject> _uiDict = new Dictionary<string, GameObject>();

	void Awake () {
		instance = this;
		DontDestroyOnLoad(gameObject);
		DontDestroyOnLoad(GameObject.Find("EventSystem"));
	}

	public GameObject OpenUI(string uiname) {
		GameObject prefab = Resources.Load<GameObject>("GUI/" + uiname);
		if (prefab) {
			GameObject go = Instantiate(prefab, transform) as GameObject;
			go.name = uiname;
			go.SetActive(true);

			// 根据定义的UI层级对窗口进行排序
			int order = UIOrder.Get(uiname);
			bool sorted = false;
			if (transform.childCount > 0) {
				for (int i = transform.childCount - 1; i >= 0; i--) {
					var child = transform.GetChild(i);
					if (child.gameObject == go) {
						continue;
					}
					if (order >= UIOrder.Get(child.gameObject.name)) {
						go.transform.SetSiblingIndex(i + 1);
						sorted = true;
						break;
					}
				}
			}
			
			if (!sorted) {
				go.transform.SetSiblingIndex(0);
			}

			//Debug.Log("------------Dump child------------");
			//for (int i = 0; i < transform.childCount; i++) {
			//	var child = transform.GetChild(i);
			//	Debug.Log("child sibling index: " + child.gameObject.name + "  " + child.GetSiblingIndex());
			//}
			//Debug.Log("------------end");

			_uiDict.Add(uiname, go);

			return go;
		}
		return null;
	}
	
	public void CloseUI(string uiname) {
		if (_uiDict.ContainsKey(uiname)) {
			Destroy(_uiDict[uiname]);
			_uiDict.Remove(uiname);
		}
	}

	public void OnExitGameScene() {
		foreach (var ui in _uiDict) {
			Destroy(ui.Value);
		}
		_uiDict.Clear();
	}

}
