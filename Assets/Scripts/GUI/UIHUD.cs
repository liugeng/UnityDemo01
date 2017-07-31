using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHUD : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnAttackClick() {
		HeroController.instance.Attack();
	}

	public void OnReturnClick() {
		UIManager.instance.ExitGameScene();
		SceneManager.LoadScene("CreateRole");
	}
}
