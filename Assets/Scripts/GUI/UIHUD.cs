using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LGUI;

public class UIHUD : MonoBehaviour {

	public Progress hpBar;
	public Progress mpBar;

	// Use this for initialization
	void Start () {
		SetHp(100, false);
		SetMp(100, false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnAttackClick() {
		HeroFight.instance.Attack();
	}

	public void OnReturnClick() {
		UIManager.instance.OnExitGameScene();
		RoleManager.instance.OnExitGameScene();

		SceneManager.LoadScene("CreateRole");
	}

	void SetHp(int hp, bool animte = true) {
		if (hpBar) {
			hpBar.SetValue(hp, animte);
		}
	}

	void SetMp(int mp, bool animte = true) {
		if (mpBar) {
			mpBar.SetValue(mp, animte);
		}
	}
}
