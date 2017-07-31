using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyForTime : MonoBehaviour {

	public float delayTime = 0f;
	
	// Update is called once per frame
	void Update () {
		Destroy(gameObject, delayTime);
	}
}
