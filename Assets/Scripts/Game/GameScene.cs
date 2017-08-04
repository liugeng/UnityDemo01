using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScene : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		//如果是直接运行的某个场景，先切回到Main初始化，再切回来
		if (!GameSettings.instance) {
			GameSettings.debugSceneName = SceneManager.GetActiveScene().name;
			SceneManager.LoadScene("Main");
			return;
		}

		HighlightingEffect highlight = gameObject.AddComponent<HighlightingEffect>();
		highlight.downsampleFactor = 1;
		highlight.iterations = 1;
		highlight.blurMinSpread = 0.75f;
		highlight.blurSpread = 0.0f;
		highlight.blurIntensity = 0.35f;


		//创建主角
		RoleManager.instance.CreateHero();

		UIManager.instance.CloseUI(UI.Loading);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
