using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleManager : MonoBehaviour {

	public static RoleManager instance;

	public GameObject[] _roleModels;
	public int _characterIdx;

	void Awake() {
		instance = this;
		_characterIdx = -1;
	}

	// Update is called once per frame
	void Update() {

	}

	public GameObject CreateHero(int? modelIdx = null) {

		_characterIdx = modelIdx ?? _characterIdx;

		GameObject hero = Instantiate(GetRoleModel(_characterIdx));
		if (hero) {
			GameObject bornPos = GameObject.Find("BornPos");
			if (!bornPos) {
				Debug.LogError("BornPos not found!");
				Destroy(hero);
				return null;
			}

			hero.tag = GameTag.Hero;
			hero.name = GameTag.Hero;

			hero.transform.position = bornPos.transform.position;
			hero.transform.rotation = bornPos.transform.rotation;

			var heroCtrl = hero.AddComponent<HeroController>();

			// 设置 JoyStick
			JoyStick.instance.handler = heroCtrl;

			// 设置摄像机焦点
			var cameraCtrl = Camera.main.gameObject.AddComponent<CameraController>();
			cameraCtrl.target = hero;
			cameraCtrl._yOffset = hero.GetComponent<CharacterController>().height * 0.5f;
		}
		return hero;
	}

	// select: 获取模型的同时如果要将其作为主角模型的话值为true
	public GameObject GetRoleModel(int idx, bool select = false) {
		if (idx >= 0 && idx < _roleModels.Length) {
			if (select) {
				_characterIdx = idx;
			}
			return _roleModels[idx];
		}
		return  null;
	}
}
