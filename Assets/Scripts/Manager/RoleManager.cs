using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleManager : MonoBehaviour {

	public static RoleManager instance;

	public int characterIdx { get; set; }
	[SerializeField]
	private GameObject[] _characterModels;

	// <IRole.id, IRole>
	private Dictionary<int, IRole> _roles = new Dictionary<int, IRole>();


	void Awake() {
		instance = this;
		characterIdx = -1;
	}

	// Update is called once per frame
	void Update() {

	}

	public GameObject CreateHero(int? modelIdx = null) {

		characterIdx = modelIdx ?? characterIdx;

		GameObject hero = Instantiate(GetRoleModel(characterIdx));
		if (hero) {
			//transform
			GameObject bornPos = GameObject.Find("BornPos");
			if (!bornPos) {
				Debug.LogError("BornPos not found!");
				Destroy(hero);
				return null;
			}

			hero.transform.position = bornPos.transform.position;
			hero.transform.rotation = bornPos.transform.rotation;
			
			//tag and name
			hero.tag = GameTag.Hero;
			hero.name = GameTag.Hero;
			hero.layer = GameLayer.Role;

			//components
			JoyStick.instance.handler = hero.AddComponent<HeroMove>();
			hero.AddComponent<HeroFight>();
			hero.AddComponent<HeroController>();

			// 设置摄像机焦点
			var cameraCtrl = Camera.main.gameObject.AddComponent<CameraController>();
			cameraCtrl.target = hero;
			cameraCtrl._yOffset = hero.GetComponent<CharacterController>().height * 0.5f;
		}
		return hero;
	}

	// select: 获取模型的同时如果要将其作为主角模型的话值为true
	public GameObject GetRoleModel(int idx, bool select = false) {
		if (idx >= 0 && idx < _characterModels.Length) {
			if (select) {
				characterIdx = idx;
			}
			return _characterModels[idx];
		}
		return  null;
	}

	public void Push(IRole r) {
		if (r) {
			_roles[r.id] = r;
		}
	}

	public IRole Find(int roleId) {
		if (_roles.ContainsKey(roleId)) {
			return _roles[roleId];
		}
		return null;
	}

	public IRole GetTargetByDistance(int range) {

		IRole nearest = null;
		float minDistance = float.MaxValue;
		Vector3 heroPos = HeroController.instance.gameObject.transform.position;

		foreach (var r in _roles) {
			if (r.Value.kind == RoleKind.Hero) {
				continue;
			}
			float dis = Vector3.Distance(heroPos, r.Value.gameObject.transform.position);
			if (dis <= range && dis < minDistance) {
				nearest = r.Value;
				minDistance = dis;
			}
		}

		return nearest;
	}

	public void OnExitGameScene() {
		_roles.Clear();
	}
}
