using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : IRole {

	private enum Act {
		Idle, Walk, Attack
	}
	private bool _isPatrol = false; //巡逻
	private Act _curAct;
	private float _duration;
	private float _elapsed;
	private float _moveSpeed = 5f;
	private Vector3 _speedVec;
	private Quaternion _targetRotation;


	// Use this for initialization
	void Start () {
		kind = RoleKind.Monster;
		gameObject.tag = GameTag.Monster;

		if (_isPatrol) {
			DoSomething();
		}

	}

	// Update is called once per frame
	new void Update () {
		base.Update();

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
		_duration = Random.Range(0.5f, 2f);

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
