using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleController : MonoBehaviour {

    private Animator _animator;
    private float _time0 = 0f;
    private string[] _aniNames = { "idle", "run", "attack" };


	// Use this for initialization
	void Start () {
        _animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = Vector3.zero;

        _time0 += Time.deltaTime;
        if (_time0 > 3) {
            foreach (var name in _aniNames) {
                _animator.SetBool(name, false);
            }

            _time0 = 0;
            int n = (int)Random.Range(0f, _aniNames.Length);
            _animator.SetBool(_aniNames[n], true);
        }
    }

	// animation event

	void FootL() {

	}

	void FootR() {

	}

	void Hit() {

	}
}
