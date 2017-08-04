using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLayer {
	public static int UI		{ get { return LayerMask.NameToLayer("UI"); } }
    public static int Ground	{ get { return LayerMask.NameToLayer("Ground"); } }
	public static int Role    { get { return LayerMask.NameToLayer("Role"); }}
}

public class GameTag {
    public static string Ground		= "Ground";
	public static string Hero		= "Hero";
    public static string Monster		= "Monster";
}


public class GameSettings : MonoBehaviour {

    public static GameSettings instance;

	//测试场景模式
	public static string debugSceneName = "";

	//点击地面特效
	public GameObject mouseGroundEffect;

    
    void Start () {
        instance = this;
	}
}
