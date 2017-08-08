using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDefine : MonoBehaviour {

	[System.Serializable]
	public class Define {
		public GameObject prefab;
		public int order = 100;
		public bool modal;
		public string description;
	}

	public static UIDefine instance;

	// List 用于在 Inspector 面板赋值，转换成 Dictionary 后用于在代码中查询

	// 定义UI界面的信息
	public List<Define> uiList = new List<Define>();
	private Dictionary<string, Define> uiDict;

	private void Awake() {
		instance = this;
	}

	public GameObject GetPrefab(string uiname) {
		InitDictIfNeeded();
		if (uiDict.ContainsKey(uiname)) {
			return uiDict[uiname].prefab;
		}
		return null;
	}

	public int GetOrder(string uiname) {
		InitDictIfNeeded();
		if (uiDict.ContainsKey(uiname)) {
			return uiDict[uiname].order;
		}
		return 100;
	}

	public bool IsModal(string uiname) {
		InitDictIfNeeded();
		if (uiDict.ContainsKey(uiname)) {
			return uiDict[uiname].modal;
		}
		return false;
	}

	private void InitDictIfNeeded() {
		if (uiDict == null) {
			uiDict = new Dictionary<string, Define>();
			foreach (var def in uiList) {
				if (def.prefab) {
					uiDict.Add(def.prefab.name, def);
				}
			}
		}
	}
}
