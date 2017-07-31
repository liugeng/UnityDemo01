using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UICreateRole : MonoBehaviour {

	public Transform _selectMask;
	public GameObject[] _roleIcons;
	public Text _nameText;
	public Text _descText;

	private GameObject _roleObj;
	private int _curRoleIdx = -1;

	struct RoleInfo {
		public string name;
		public string description;
	}
	RoleInfo[] _roleInfos = {
		new RoleInfo() { name="神拳张三", description="拳打李四" },
		new RoleInfo() { name="无影腿李四", description="脚踢张三" },
		new RoleInfo() { name="飞天菜花", description="飞啊飞" }
	};

	// Use this for initialization
	void Start() {
		int idx = RoleManager.instance._characterIdx;
		if (idx == -1) {
			idx = Random.Range(0, _roleIcons.Length);
		}
		OnRoleIconClick(idx);
	}

	// button event
	public void OnEnterGameClick() {
		UIManager.instance.OpenUI(UI.Loading);
		Invoke("LoadScene", 0.1f);
	}

	public void OnRoleIconClick(int idx) {
		if (_curRoleIdx == idx || idx < 0 || idx >= _roleIcons.Length) {
			return;
		}

		_curRoleIdx = idx;
		_selectMask.parent = _roleIcons[idx].transform;
		_selectMask.localPosition = Vector3.zero;

		if (idx < _roleInfos.Length) {
			_nameText.text = _roleInfos[idx].name;
			_descText.text = _roleInfos[idx].description;
		} else {
			_nameText.text = "";
			_descText.text = "";
		}

		ShowRoleModel(idx);
	}

	private void LoadScene() {
		SceneManager.LoadScene("Scene01");

		UIManager.instance.CloseUI(UI.CreateRole);
		UIManager.instance.OpenUI(UI.HUD);
	}

	void ShowRoleModel(int idx) {

		if (this._roleObj) {
			Destroy(this._roleObj);
			this._roleObj = null;
		}

		GameObject model = RoleManager.instance.GetRoleModel(idx, true);
		if (model == null) {
			return;
		}

		GameObject parent = GameObject.Find("RolePos");
		if (parent == null) {
			Debug.LogError("RolePos not found");
			return;
		}
		_roleObj = Instantiate(model, parent.transform);
		_roleObj.transform.localPosition = Vector3.zero;
		_roleObj.AddComponent<RoleController>();
		Destroy(_roleObj.GetComponent<FootstepPlayer>());
	}
}
