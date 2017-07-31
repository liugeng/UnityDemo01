using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 角色状态
enum RoleState {
	Idle,
	Run,
	Attack,
	Death
}

// 角色移动控制类型
enum MoveCtrlType {
	None,
	Target,     // 向给定目标点移动
	Keyboard,   // 方向键移动
	JoyStick	// 手柄
}


[RequireComponent(typeof(CharacterController))]
public class HeroController : MonoBehaviour, IJoyStickHandler {

	public static HeroController instance;

    private Animator _animator;
    private CharacterController _controller;
    private Quaternion _tarRotation;
    private Vector3 _tarPosition;
	[SerializeField]
    private float _moveSpeed = 10f;
	private Vector3 _speedVec;
    private RoleState _curState = RoleState.Idle;
	private MoveCtrlType _moveCtrlType = MoveCtrlType.None;


    // Use this for initialization
    void Start () {
		instance = this;
		gameObject.layer = GameLayer.Player;
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController>();
    }
	
	// Update is called once per frame
	void Update () {
		UpdateMove();
	}

	void LateUpdate() {
		CheckInput();
	}

	void CheckInput() {

		if (_curState == RoleState.Attack || _moveCtrlType == MoveCtrlType.JoyStick) {
			return;	
		}

		// 点击地图移动
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100f, 1 << GameLayer.Ground)) {
				if (hit.collider.tag == GameTag.Ground) {
					//Instantiate(GameSetting.Instance.mousefxNormal, new Vector3(h.point.x, h.point.y + 0.02f, h.point.z), h.collider.transform.rotation);
					Instantiate(GameSettings.instance.mouseGroundEffect, new Vector3(hit.point.x, hit.point.y + 0.2f, hit.point.z), hit.collider.transform.rotation);
					MoveToTargetPositin(hit.point);
					_moveCtrlType = MoveCtrlType.Target;
				}
			}
		}
		// 方向键控制移动
		else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
			float x = Input.GetAxisRaw("Horizontal");
			float z = Input.GetAxisRaw("Vertical");
			
			MoveByDirection(x, z, MoveCtrlType.Keyboard);
		}
		else if (_moveCtrlType == MoveCtrlType.Keyboard) {

			_curState = RoleState.Idle;
			_moveCtrlType = MoveCtrlType.None;

			ResetAnimation();
		}
	}

	#region Move

    public void MoveToTargetPositin(Vector3 dest) {
        _tarPosition.x = dest.x;
        _tarPosition.y = dest.y;//transform.position.y;
        _tarPosition.z = dest.z;

        float distance = Vector3.Distance(transform.position, _tarPosition);
        if (distance > 0.5f) {
            _curState = RoleState.Run;
            _tarRotation = Quaternion.LookRotation(_tarPosition - transform.position);
			_tarRotation.x = 0f;
			_tarRotation.z = 0f;

			Quaternion tmp = transform.rotation;
			transform.rotation = _tarRotation;
			_speedVec = transform.TransformDirection(Vector3.forward * _moveSpeed);
			transform.rotation = tmp;

			ResetAnimation();
		}
    }

	void MoveByDirection(float horizontal, float vertical, MoveCtrlType ctrlType) {
		if (Mathf.Abs(horizontal) > 0f || Mathf.Abs(vertical) > 0f) {

			Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
			Vector3 right = new Vector3(forward.z, 0, -forward.x);  //将坐标轴逆时针旋转90°

			_speedVec = (horizontal * right + vertical * forward).normalized * _moveSpeed;

			_tarRotation = Quaternion.LookRotation(_speedVec);
			_tarRotation.x = 0f;
			_tarRotation.z = 0f;

			_curState = RoleState.Run;
			_moveCtrlType = ctrlType;

			ResetAnimation();
		}
	}

	void UpdateMove() {
		if (_curState != RoleState.Run) {
			return;
		}

		transform.rotation = Quaternion.Lerp(transform.rotation, _tarRotation, Time.deltaTime * 5);
		_controller.SimpleMove(_speedVec);

		if (_moveCtrlType == MoveCtrlType.Target) {
			CheckDistance();
		}
	}

	void CheckDistance() {
        float distance = Vector3.Distance(transform.position, _tarPosition);
        if (distance <= 0.5f && _curState == RoleState.Run) {
            _curState = RoleState.Idle;
			ResetAnimation();
		}
    }

	public void OnJoyStickDrag(Vector2 delta) {
		MoveByDirection(delta.x, delta.y, MoveCtrlType.JoyStick);
	}

	public void OnJoyStickStop() {
		_curState = RoleState.Idle;
		_moveCtrlType = MoveCtrlType.None;

		ResetAnimation();
	}

	#endregion

	#region Animation

	void ResetAnimation() {
		_animator.SetBool("run", false);

		switch (_curState) {
			case RoleState.Run:
				_animator.SetBool("run", true);
				break;
			case RoleState.Attack:
				_animator.SetTrigger("attack");
				StartCoroutine(WaitForAttackAnimationEnd());
				break;
			default:
				break;
		}
	}

	// animation event

	void Hit() {

	}

	#endregion

	#region Fight

	public void Attack() {
		if (_curState == RoleState.Attack) {
			return;
		}
		_curState = RoleState.Attack;
		ResetAnimation();
	}

	IEnumerator WaitForAttackAnimationEnd() {
		yield return new WaitForSeconds(1f);
		_curState = RoleState.Idle;
		ResetAnimation();
	}

	#endregion

}
