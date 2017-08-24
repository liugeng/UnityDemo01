using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
public class HeroMove : MonoBehaviour, IJoyStickHandler {

	// 角色移动控制类型
	enum MoveCtrlType {
		None,
		Target,     // 向给定目标点移动
		Keyboard,   // 方向键移动
		JoyStick,   // 手柄
		Trace		// 追踪
	}

	public static HeroMove instance;

	private StateManager _sm;

	private bool _isMoving;
	private Quaternion _tarRotation;
	private Vector3 _tarPosition;
	[SerializeField]
	private float _moveSpeed = 10f;
	private Vector3 _speedVec;
	private MoveCtrlType _moveCtrlType = MoveCtrlType.None;
	private CharacterController _controller;


	void Start() {
		instance = this;
		_controller = GetComponent<CharacterController>();
		_sm = HeroController.instance._sm;
	}

	void Update() {
		if (_isMoving) {
			UpdateMove();
		}
	}

	//HeroController check first, then call this function
	public void CheckInput() {
		if (!_sm.CanSwitch(HeroState.CtrlMove)) {
			return;
		}

		if (_moveCtrlType == MoveCtrlType.JoyStick) {
			return;
		}

		// 方向键控制移动
		else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
			float x = Input.GetAxisRaw("Horizontal");
			float z = Input.GetAxisRaw("Vertical");

			MoveByDirection(x, z, MoveCtrlType.Keyboard);

		} else if (_moveCtrlType == MoveCtrlType.Keyboard) {

			OnMoveEnd();
		}
	}

	//移动到目标点
	public void MoveToTargetPositin(Vector3 dest) {
		//_tarPosition.x = dest.x;
		//_tarPosition.y = dest.y;//transform.position.y;
		//_tarPosition.z = dest.z;

		_tarPosition = dest;

		float distance = Vector3.Distance(transform.position, _tarPosition);
		if (distance > 0.5f) {
			_tarRotation = Quaternion.LookRotation(_tarPosition - transform.position);
			_tarRotation.x = 0f;
			_tarRotation.z = 0f;

			//Quaternion tmp = transform.rotation;
			transform.rotation = _tarRotation;
			_speedVec = transform.TransformDirection(Vector3.forward * _moveSpeed);
			//transform.rotation = tmp;

			if (_moveCtrlType != MoveCtrlType.Target) {
				_sm.Switch(HeroState.TargetMove);
				_isMoving = true;
				_moveCtrlType = MoveCtrlType.Target;
			}
		}
	}

	//沿方向移动
	void MoveByDirection(float horizontal, float vertical, MoveCtrlType ctrlType) {
		if (Mathf.Abs(horizontal) > 0f || Mathf.Abs(vertical) > 0f) {

			Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
			Vector3 right = new Vector3(forward.z, 0, -forward.x);  //将坐标轴逆时针旋转90°

			_speedVec = (horizontal * right + vertical * forward).normalized * _moveSpeed;

			_tarRotation = Quaternion.LookRotation(_speedVec);
			_tarRotation.x = 0f;
			_tarRotation.z = 0f;

			if (_moveCtrlType != ctrlType) {
				_sm.Switch(HeroState.CtrlMove);
				_isMoving = true;
				_moveCtrlType = ctrlType;
			}
		}
	}

	//追踪移动
	public void TraceMove() {
		if (_sm.CanSwitch(HeroState.Trace)) {
			_sm.Switch(HeroState.Trace);

			_isMoving = true;
			_moveCtrlType = MoveCtrlType.Trace;
		}
	}

	void UpdateMove() {

		if (_moveCtrlType != MoveCtrlType.Target && _moveCtrlType != MoveCtrlType.Trace) {
			transform.rotation = Quaternion.Lerp(transform.rotation, _tarRotation, Time.deltaTime * 10);
		}

		if (_moveCtrlType == MoveCtrlType.Target) {
			_controller.SimpleMove(_speedVec);
			CheckDistance();

		} else if (_moveCtrlType == MoveCtrlType.Trace) {
			IRole traceTarget = HeroFight.instance.GetTraceTarget();
			if (traceTarget != null) {
				Vector3 delta = traceTarget.gameObject.transform.position - transform.position;
				Quaternion rotation = Quaternion.LookRotation(delta);
				rotation.x = 0f;
				rotation.z = 0f;
				transform.rotation = rotation;
				
				if (delta.magnitude <= HeroFight.instance.traceDistance) {
					Reset();
					_sm.End(HeroState.Trace);
				} else {
					_controller.SimpleMove(delta.normalized * _moveSpeed);
				}

			} else {
				_sm.Interrupt(HeroState.Trace);
				Reset();
			}

		} else {
			_controller.SimpleMove(_speedVec);
		}
	}

	void CheckDistance() {
		float distance = Vector3.Distance(transform.position, _tarPosition);
		//float distance = Mathf.Sqrt((transform.position.x-_tarPosition.x)*(transform.position.x-_tarPosition.x)+
		//	(transform.position.z-_tarPosition.z)*(transform.position.z-_tarPosition.z));
		if (distance <= 0.5f && _isMoving) {
			OnMoveEnd();
		}
	}

	public void OnJoyStickDrag(Vector2 delta) {
		MoveByDirection(delta.x, delta.y, MoveCtrlType.JoyStick);
	}

	public void OnJoyStickStop() {
		OnMoveEnd();
	}

	public void OnMoveEnd() {
		if (_moveCtrlType == MoveCtrlType.Target) {
			_sm.End(HeroState.TargetMove);
		} else {
			_sm.End(HeroState.CtrlMove);
		}
		_moveCtrlType = MoveCtrlType.None;
		_isMoving = false;
	}

	public void Reset() {
		_moveCtrlType = MoveCtrlType.None;
		_isMoving = false;
	}
}
