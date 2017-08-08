using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameLabel : MonoBehaviour {

	Quaternion direction;

	void Start () {
		direction = Quaternion.FromToRotation(new Vector3(0,1,0), new Vector3(0,1,0));
	}
	
	void Update () {
		transform.rotation = Camera.main.transform.rotation;
	}
}
