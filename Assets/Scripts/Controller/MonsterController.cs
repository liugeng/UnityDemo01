using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour {

	public enum Act {
		Idle, Walk, Attack
	}
	bool _isPatrol = true; //巡逻
	public Act _curAct;
	public float _duration;
	public float _elapsed;
	public float _moveSpeed = 0.8f;
	public Vector3 _speedVec;
	public Quaternion _targetRotation;

	CharacterController _controller;
	Animator _animator;

	// Use this for initialization
	void Start () {
		gameObject.layer = GameLayer.Player;
		_controller = GetComponent<CharacterController>();
		_animator = GetComponent<Animator>();
		if (_isPatrol) {
			DoSomething();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (_isPatrol) {
			_elapsed += Time.deltaTime;
			if (_elapsed >= _duration) {
				DoSomething();

			} else if (_curAct == Act.Walk) {
				transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, Time.deltaTime * 5);
				_controller.SimpleMove(_speedVec);
			}
		}
	}

	void DoSomething() {
		_animator.SetBool("run", false);

		_curAct = (Act)(Random.Range(0, 10) % 2);
		_elapsed = 0;
		_duration = Random.Range(2f, 5f);

		if (_curAct == Act.Walk) {
			_speedVec.x = Random.Range(-10f, 10f);
			_speedVec.z = Random.Range(-10f, 10f);
			_speedVec = _speedVec.normalized * _moveSpeed;

			_targetRotation = Quaternion.LookRotation(_speedVec);
			_targetRotation.x = 0f;
			_targetRotation.z = 0f;

			_animator.SetBool("run", true);
		}
	}
}
