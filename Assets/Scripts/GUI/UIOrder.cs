using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class UIOrder {

	private static Dictionary<string, int> _dict = new Dictionary<string, int>();
	private static int _maxCount;

	static UIOrder() {
		string filepath = Application.dataPath + "/Resources/uidefine.json";

		string str = File.ReadAllText(filepath);
		if (str != "") {
			JsonData json = JsonMapper.ToObject(str);
			_maxCount = json.Count;

			for (int i = 0; i < json.Count; i++) {
				JsonData d = json[i];

				if ((d as IDictionary).Contains("name")) {
					_dict[d["name"].ToString()] = i;

				} else if ((d as IDictionary).Contains("group")) {
					var group = d["group"];
					for (int j = 0; j < group.Count; j++) {
						_dict[group[j]["name"].ToString()] = i;
					}
				}
			}
		}

		//foreach (var k in _dict) {
		//	Debug.Log(k.Key + ": " + k.Value);
		//}
	}

	public static int Get(string uiname) {
		if (_dict.ContainsKey(uiname)) {
			//Debug.Log("[UIOrder] " + uiname + ": " + _dict[uiname]);
			return _dict[uiname];
		}
		Debug.LogWarning("[UIOrder] not found ui: " + uiname);
		return _maxCount;
	}
}

