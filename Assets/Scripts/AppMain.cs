using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppMain : MonoBehaviour {

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(GameObject.Find("Managers"));
		DontDestroyOnLoad(GameObject.Find("UIDefine"));

		// 初始化随机数种子
        Random.InitState((int)Tools.GetTimeStamp());

		// 调试模式
		if (GameSettings.debugSceneName != "") {
			RoleManager.instance._characterIdx = 0;
			SceneManager.LoadScene(GameSettings.debugSceneName);
			UIManager.instance.OpenUI(UI.HUD);

		} else {
			SceneManager.LoadScene("CreateRole");
		}
	}
}
