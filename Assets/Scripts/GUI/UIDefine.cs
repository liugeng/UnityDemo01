using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDefine : MonoBehaviour {

	public enum Type {
		Window,
		Static,
		Popup,
		Alert
	}
	
	[System.Serializable]
	public class Define {
		public GameObject prefab;
		public int order = 100;
		public bool modal;
		//public Type type;
		public string description;
	}

	public static UIDefine instance;
	public List<Define> list = new List<Define>();
	private Dictionary<string, Define> dict = new Dictionary<string, Define>();

	private void Awake() {
		instance = this;
		AssignDict();
	}

	public void AssignDict() {
		dict.Clear();
		foreach (var d in list) {
			if (d.prefab) {
				dict.Add(d.prefab.name, d);
			}
		}
	}

	public GameObject GetPrefab(string uiname) {
		if (dict.ContainsKey(uiname)) {
			return dict[uiname].prefab;
		}
		return null;
	}

	public int GetOrder(string uiname) {
		if (dict.ContainsKey(uiname)) {
			return dict[uiname].order;
		}
		return 100;
	}

	public bool IsModal(string uiname) {
		if (dict.ContainsKey(uiname)) {
			return dict[uiname].modal;
		}
		return false;
	}
}
