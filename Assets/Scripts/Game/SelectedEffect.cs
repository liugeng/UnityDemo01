using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedEffect : MonoBehaviour {

	private Quaternion rotation = new Quaternion();
	private Vector3 eularAngle = new Vector3(90, 0, 0);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		eularAngle.y = Time.time * 30;
		rotation.eulerAngles = eularAngle;
		transform.rotation = rotation;
	}
}
