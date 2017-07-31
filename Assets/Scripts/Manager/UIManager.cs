using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	
	public static UIManager instance;
	[SerializeField]
	private UIDefine _def;
	private Dictionary<string, GameObject> _uiDict = new Dictionary<string, GameObject>();

	void Awake () {
		instance = this;
		DontDestroyOnLoad(gameObject);
		DontDestroyOnLoad(GameObject.Find("EventSystem"));
	}

	public GameObject OpenUI(string uiname) {
		GameObject prefab = _def.GetPrefab(uiname);
		if (prefab) {
			GameObject go = Instantiate(prefab, transform) as GameObject;
			go.name = uiname;
			go.SetActive(true);
			go.transform.SetSiblingIndex(_def.GetOrder(uiname));

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

	public void ExitGameScene() {
		foreach (var ui in _uiDict) {
			Destroy(ui.Value);
		}
		_uiDict.Clear();
	}

}
