using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//如果是直接运行的某个场景，先切回到Main初始化，再切回来
		if (!GameSettings.instance) {
			GameSettings.debugSceneName = SceneManager.GetActiveScene().name;
			SceneManager.LoadScene("Main");
			return;
		}

		//创建主角
        RoleManager.instance.CreateHero();

		UIManager.instance.CloseUI(UI.Loading);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
