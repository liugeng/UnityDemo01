using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 角色状态
enum HeroState {
	Idle,
	Run,
	Trace,
	Attack,
	SelTarget,	//选择目标，瞬时行为，执行完后会立即结束，不同于持续行为
	Max
}


public class HeroController : IRole, IStateManager {

	public static HeroController instance;

	//状态
    public StateManager _sm;
	private HeroMove _heroMove;
	private HeroFight _heroFight;


	new void Awake () {
		base.Awake();
		kind = RoleKind.Hero;

		instance = this;
		
		//定义状态关系
		//0 StopCurrent  1 KeepCurrent  2 Coexist
		_sm = new StateManager(this, new int[(int)HeroState.Max, (int)HeroState.Max] {
			//cur \ new          Idle  Run   Trace  Attack  SelTarget
			/*Idle*/			{ 1,    0,    0,      0,      2 },
			/*Run*/				{ 0,    0,    0,      0,      0 },
			/*Trace*/		    { 0,    0,    0,      0,      0 },
			/*Attack*/			{ 0,    0,    0,      1,      0 },
			/*SelTarget*/		{ 0,    0,    0,      0,      0 }
		});
		_heroMove = GetComponent<HeroMove>();
		_heroFight = GetComponent<HeroFight>();

		//选中特效
		var selectedEffect = Instantiate(GameSettings.instance.selectedEffect, transform);
	}
	
	new void Update () {
		base.Update();
		CheckInput();
	}

	void CheckInput() {
		// 点击地图移动
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100f, 1 << GameLayer.Ground | 1 << GameLayer.Role)) {
				if (hit.collider.tag == GameTag.Ground) {
					Instantiate(GameSettings.instance.mouseGroundEffect, new Vector3(hit.point.x, hit.point.y + 0.2f, hit.point.z), hit.collider.transform.rotation);
					_heroMove.MoveToTargetPositin(hit.point);

				} else if (hit.collider.tag == GameTag.Monster) {
					_heroFight.SelecteTarget(hit.collider.gameObject.GetComponent<IRole>());
				}
			}

		} else {
			_heroMove.CheckInput();
		}
	}

	public float GetDistance(IRole r) {
		if (r != null) {
			return Vector3.Distance(gameObject.transform.position, r.gameObject.transform.position);
		}
		return 0f;
	}


	#region IStateManager Methods

	public void OnStartState(int stateType) {
		HeroState state = (HeroState)stateType;
		Debug.Log("Start State: " + stateType);

		if (state == HeroState.Run) {
			_animator.SetBool("run", true);

		} else if (state == HeroState.Trace) {
			_animator.SetBool("run", true);

		} else if (state == HeroState.Attack) {
			_animator.SetTrigger("attack");
		}
	}

	public void OnInterruptState(int stateType) {
		HeroState state = (HeroState)stateType;

		if (state == HeroState.Run) {
			_animator.SetBool("run", false);
			_heroMove.Reset();

		} else if (state == HeroState.Trace) {
			_animator.SetBool("run", false);
			_heroMove.Reset();

		} else if (state == HeroState.Attack) {
			_animator.SetBool("attack", false);
			_heroFight.CancelInvoke();
		}
	}

	public void OnStateEnd(int stateType) {
		HeroState state = (HeroState)stateType;

		if (state == HeroState.Run) {
			_animator.SetBool("run", false);

		} else if (state == HeroState.Trace) {
			_animator.SetBool("run", false);
			
			IRole r = _heroFight.GetTraceTarget();
			if (r.kind == RoleKind.Monster) {
				_heroFight.Attack();

			} else if (r.kind == RoleKind.Npc) {

			}
		} else if (state == HeroState.Attack) {
			_heroFight.Attack();
		}
	}

	#endregion
}
